(function () {
    "use strict";
    angular.module("deePProperties")
        .directive("propertySummary", [
                function () {
                    return {
                        restrict: "E",
                        scope: {
                            property: "=",
                            propertySelected: "&?"
                        },
                        templateUrl: "/content/templates/directives/propertySummary.html",
                        replace: true,
                        transclude: false,
                        controllerAs: "propertySummaryCtrl",
                        controller: [
                            function () {

                            }],
                        link: function (/*scope, element, attrs, ctrl*/) {
                        }
                    };
                }]);
})();