
(function () {
    "use strict";
    angular.module("deePProperties")
        .controller("placeBidController", ["$scope", "$uibModalInstance",
            function ($scope, $uibModalInstance) {
                this.ok = function () {
                    $uibModalInstance.close({ bid: null });
                };

                this.cancel = function () {
                    $uibModalInstance.dismiss();
                };
            }]);
})();