
(function () {
    "use strict";
    angular.module("deeP")
        .controller("propertyDetailsController", ["$location", "navigationUris", "accountContext", "dialogService", "propertyService", "$routeParams",
            function ($location, navigationUris, accountContext, dialogService, propertyService, $routeParams) {
                var _this = this;

                _this.canPlaceBid = true;
                _this.navigationUris = navigationUris;

                _this.placeBid = function () {
                    dialogService.openPlaceBidDialog(_this.property).then(
                        function (result) {
                            $location.path(navigationUris.user);
                        },
                        function (error) {
                            // User alert and offline trace collection would be better
                            if (error) {
                                console.error(error);
                            }
                        });
                };

                // load property details
                propertyService.queryProperties({ propertyId: $routeParams.propertyId }, 0).then(
                       function (result) {
                           _this.property = result.data[0];
                       },
                        function (error) {
                            // User alert and offline trace collection would be better
                            if (error) {
                                console.error(error);
                            }
                            $location.path(navigationUris.user);
                        });

                // check if user has outstanding bid for the property before allowing posting another
                propertyService.queryBids({ propertyId: $routeParams.propertyId, buyerName: accountContext.username }).then(
                    function (result) {
                        if (result.data.length > 0) {
                            // There is an outstanding bid for this user
                            _this.canPlaceBid = false;
                        }
                    },
                    function (error) {
                        // User alert and offline trace collection would be better
                        if (error) {
                            console.error(error);
                        }
                        $location.path(navigationUris.user);
                    });
            }]);
})();