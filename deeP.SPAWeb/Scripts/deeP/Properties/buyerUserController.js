
(function () {
    "use strict";
    angular.module("deePProperties")
        .controller("buyerUserController", ["$scope", "$location", "$timeout", "navigationUris",
            function ($scope, $location, $timeout, navigationUris) {
                var _this = this;

                _this.propertySelected = function (property) {
                    $location.path(navigationUris.propertyDetails + "/" + property.id);
                };

                $timeout(function () {
                    $scope.$broadcast("refreshPropertyList", false);
                    // True means that we want to restrict the list to see only our own bids
                    $scope.$broadcast("refreshBidList", true, false);
                });
            }]);
})();