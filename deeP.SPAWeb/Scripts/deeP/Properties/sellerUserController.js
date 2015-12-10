(function () {
    "use strict";
    angular.module("deePProperties")
        .controller("sellerUserController", ["$scope", "$timeout", "navigationUris", "dialogService", "propertyService",
            function ($scope,  $timeout, navigationUris, dialogService, propertyService) {
                var _this = this;

                _this.acceptBid = function (bid) {
                    propertyService.closeBid(bid, true).then(
                          function (response) {
                              // Success; refresh bids
                              $scope.$broadcast("refreshBidList", true, true, false);
                              $scope.$broadcast("refreshPropertyList", true);
                          },
                          function (error) {
                              // Failure; present error details
                              _this.error = error;
                              // User alert and offline trace collection would be better
                              if (error) {
                                  console.error(error);
                              }
                          });
                };

                _this.rejectBid = function (bid) {
                    propertyService.closeBid(bid, false).then(
                          function (response) {
                              // Success; refresh bids and properties
                              $scope.$broadcast("refreshBidList", true, true, false);
                              $scope.$broadcast("refreshPropertyList", true);
                          },
                          function (error) {
                              // Failure; present error details
                              _this.error = error;
                              // User alert and offline trace collection would be better
                              if (error) {
                                  console.error(error);
                              }
                          });
                };

                this.propertySelected = function (property) {
                    dialogService.openPropertyEditDialog(property).then(
                        function (result) {
                            $scope.$broadcast("refreshPropertyList", true);
                        },
                        function (error) {
                            // User alert and offline trace collection would be better
                            if (error){
                                console.error(error);
                            }
                        });
                };

                _this.addProperty = function () {
                    dialogService.openPropertyEditDialog().then(
                        function (result) {
                            $scope.$broadcast("refreshPropertyList", true);
                        },
                        function (error) {
                            // User alert and offline trace collection would be better
                            if (error) {
                                console.error(error);
                            }
                        });
                };

                $timeout(function () {
                    // True means that we want to restrict the list to see only our own properties
                    $scope.$broadcast("refreshPropertyList", true);
                    $scope.$broadcast("refreshBidList", true, true, false);
                });
            }]);
})();