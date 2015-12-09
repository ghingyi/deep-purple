(function () {
    "use strict";
    angular.module("deePProperties")
        .factory("dialogService", ["$uibModal",
            function ($uibModal) {

                // Open 'add property' dialog
                var openPropertyEditDialog = function (property) {
                    var modalInstance = $uibModal.open({
                        templateUrl: "/content/templates/dialogs/editPropertyDialog.html",
                        controller: "editPropertyController",
                        controllerAs: "editPropertyCtrl",
                        size: "lg",
                        backdrop: "static",
                        keyboard: "true",
                        resolve: {
                            property: function () { return property; }
                        }
                    });
                    return modalInstance.result;
                };

                // Open 'place bid' dialog
                var _openPlaceBidDialog = function (property) {
                    var modalInstance = $uibModal.open({
                        templateUrl: "/content/templates/dialogs/placeBidDialog.html",
                        controller: "placeBidController",
                        controllerAs: "placeBidCtrl",
                        size: "lg",
                        backdrop: "static",
                        keyboard: "true",
                        resolve: {
                            property: function () { return property; }
                        }
                    });
                    return modalInstance.result;
                };

                return {
                    openPropertyEditDialog: openPropertyEditDialog,
                    openPlaceBidDialog: _openPlaceBidDialog
                };
            }]);
})();