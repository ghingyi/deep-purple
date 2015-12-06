(function () {
    "use strict";
    angular.module("deePProperties")
        .directive("bidSummary", [
                function () {
                    return {
                        restrict: "E",
                        scope: {
                            bid: "=",
                            bidAccept: "&?",
                            bidReject: "&?"
                        },
                        templateUrl: "/content/templates/directives/bidSummary.html",
                        replace: true,
                        transclude: false,
                        controllerAs: "bidSummaryCtrl",
                        controller: [
                            function () {

                            }],
                        link: function (/*scope, element, attrs, ctrl*/) {
                        }
                    };
                }]);
})();