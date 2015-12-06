(function () {
    "use strict";
    angular.module("deePAuth")
        .directive("accountInfo", [
                function () {
                    return {
                        restrict: "E",
                        templateUrl: "/content/templates/directives/accountInfo.html",
                        replace: true,
                        transclude: false,
                        controllerAs: "accountInfoCtrl",
                        controller: ["$location", "accountContext", "accountService", "navigationUris",
                            function ($location, accountContext, accountService, navigationUris) {

                                this.accountContext = accountContext;
                                this.navigationUris = navigationUris;

                                this.logout = function () {
                                    accountService.logout();
                                    $location.path(navigationUris.home);
                                };
                            }],
                        link: function (/*scope, element, attrs, ctrl*/) {
                        }
                    };
                }]);
})();