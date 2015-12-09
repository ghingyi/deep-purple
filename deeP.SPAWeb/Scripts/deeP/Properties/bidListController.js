
(function () {
    "use strict";
    angular.module("deePProperties")
        .controller("bidListController", ["$scope", "accountContext", "propertyService",
            function ($scope, accountContext, propertyService) {
                var _this = this;

                _this.filter = {}

                _this.bids = null;

                _this.refreshList = function (restricted, seller) {
                    // Restricted means that we only want to get bids that are relevant for the user in his/her role.
                    // However, fooling this will not mean much; the server does authrize edit requests separately.
                    var currentFilter = _.clone(_this.filter);
                    if (restricted) {
                        if (seller){
                            currentFilter.sellerName = accountContext.username;
                        } else {
                            currentFilter.buyerName = accountContext.username;
                        }
                    }

                    propertyService.queryBids(currentFilter).then(
                        function (result) {
                            // Note: in this sample, we'll not implement paging; however, the API method and the repository supports skip and take parameters
                            _this.bids = result.data;
                        },
                        function (error) {
                            // User alert and offline trace collection would be better
                            console.error(error);
                        });
                };

                $scope.$on("refreshBidList", function ($e, restrict, seller) {
                    _this.refreshList(restrict, seller);
                });
            }]);
})();