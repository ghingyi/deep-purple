/// <reference path="../lodash.js" />
(function () {
    "use strict";
    /*global _ */
    angular.module("deeP")
        .controller("loginController", ["$location", "navigationUris", "accountService",
            function ($location, navigationUris, accountService) {
                this.cred = {
                    username: "",
                    password: ""
                };
                this.error = null;

                // Request login for current credentials
                this.login = function () {
                    var _this = this;

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