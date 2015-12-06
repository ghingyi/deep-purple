(function () {
    "use strict";

    // Bootstrap module of authorization utilities
    var deePAuth = angular.module("deePAuth", ["deePConfig"]);
    deePAuth.value("accountContext", { username: null, roles: null, accessToken: null });
})();