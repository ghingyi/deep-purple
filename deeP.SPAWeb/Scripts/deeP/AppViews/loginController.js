/// <reference path="../lodash.js" />
(function () {
    "use strict";
    /*global _ */
    angular.module("deeP")
        .controller("loginController", ["$location", "navigationUris", "accountService",
            function ($location, navigationUris, accountService) {
                var _this = this;
                _this.navigationUris = navigationUris;

                _this.cred = {
                    username: "",
                    password: ""
                };
                _this.error = null;

                // Request login for current credentials
                _this.login = function () {
                    // Copy credential object
                    var currentCred = _.clone(_this.cred);

                    // Send credentials to server for authentication
                    accountService.login(currentCred).then(
                        function (/*response*/) {
                            // Success; clear previous errors if any and send user to the user view
                            _this.error = null;
                            $location.path(navigationUris.user);
                        },
                        function (error) {
                            // Failure; present error details and reset creds
                            _this.error = error;
                            _this.cred.username = "";
                            _this.cred.password = "";
                        });
                };
            }]);
})();