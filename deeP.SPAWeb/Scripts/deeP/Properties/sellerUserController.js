(function () {
    "use strict";
    angular.module("deePProperties")
        .controller("sellerUserController", ["$scope", "$location", "navigationUris", "dialogService", "propertyService",
            function ($scope, $location, navigationUris, dialogService, propertyService) {
                this.acceptBid = function (bid) {

                };

                this.rejectBid = function (bid) {

                };

                this.propertySelected = function (property) {


                    dialogService.openPropertyEditDialog(property).then(
                        function (result) {

                        },
                        function (error) {
                        });
                };

                this.addProperty = function () {
                    dialogService.openPropertyEditDialog().then(
                        function (result) {

                        },
                        function (error) {
                        });
                };
            }]);
})();