(function () {
    "use strict";
    angular.module("deePAuth")
        .factory("accountService", ["$http", "$q", "authorizationServiceUrl", "accountContext",
            function ($http, $q, authorizationServiceUrl, accountContext) {

                // Gets user details for currently logged in user
                var _getUser = function () {
                    return $http.get(authorizationServiceUrl + "api/accounts/getuser");
                };

                // Logs out the currently logged in user, if any
                var _logout = function () {
                    accountContext.username = null;
                    accountContext.roles = null;
                    accountContext.accessToken = null;
                };

                // Logs in the user identified by the username/password credentials
                var _login = function (creds) {
                    // Log out previous logged in user if any
                    _logout();

                    var deferred = $q.defer();

                    // Send token request to Authorization Service
                    var data = "grant_type=password&username=" + encodeURIComponent(creds.username) + "&password=" + encodeURIComponent(creds.password);
                    $http.post(authorizationServiceUrl + "oauth/token", data, { headers: { "Content-Type": "application/x-www-form-urlencoded" } }).then(
                        function (response) {
                            // Success; store user name and access token for logged in user
                            accountContext.username = creds.username;
                            accountContext.accessToken = response.data.access_token;

                            // Request details for logged in user
                            _getUser().then(
                                function (user) {
                                    // Store user roles
                                    accountContext.roles = user.data.roles;

                                    deferred.resolve(accountContext);
                                },
                                function (error, status) {
                                    deferred.reject(error, status);
                                });
                        },
                        function (error, status) {
                            // Failure - 401 are handled by redirection to login view; nothing happens if we were there already
                            deferred.reject(error, status);
                        });

                    return deferred.promise;
                };

                // Registers a new seller with the identity database of the Authorization Service
                var _registerSeller = function (userDetails) {
                    // Log out previous logged in user if any
                    _logout();

                    return $http.post(authorizationServiceUrl + "api/accounts/createseller", userDetails);
                };

                // Registers a new buyer with the identity database of the Authorization Service
                var _registerBuyer = function (userDetails) {
                    // Log out previous logged in user if any
                    _logout();

                    return $http.post(authorizationServiceUrl + "api/accounts/createbuyer", userDetails);
                };

                return {
                    login: _login,
                    logout: _logout,
                    registerSeller: _registerSeller,
                    registerBuyer: _registerBuyer
                };
            }]);
})();