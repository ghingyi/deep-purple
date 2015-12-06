
(function () {
    "use strict";
    angular.module("deePProperties")
        .controller("buyerUserController", ["$location", "navigationUris",
            function ($location, navigationUris) {
                this.propertySelected = function (property) {
                    $location.path(navigationUris.propertyDetails + "/" + property.id);
                };
            }]);
})();