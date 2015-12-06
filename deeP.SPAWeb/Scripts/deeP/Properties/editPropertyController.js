
(function () {
    "use strict";
    angular.module("deePProperties")
        .controller("editPropertyController", ["$scope", "$uibModalInstance",
            function ($scope, $uibModalInstance) {
                this.ok = function () {
                    $uibModalInstance.close({ property: null });
                };

                this.cancel = function () {
                    $uibModalInstance.dismiss();
                };
            }]);
})();