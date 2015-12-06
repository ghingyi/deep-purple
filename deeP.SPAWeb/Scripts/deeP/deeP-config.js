﻿(function () {
    "use strict";

    // Specify configuration constants
    var deePConfig = angular.module("deePConfig", []);
    deePConfig.constant("authorizationServiceUrl", "http://localhost:62635/");
    deePConfig.constant("navigationUris",
        {
            home: "/",
            login: "/login",
            registerSeller: "/register/seller",
            registerBuyer: "/register/buyer",
            user: "/user"
        });
    deePConfig.constant("roleNames",
        {
            seller: "Seller",
            buyer: "Buyer",
            admin: "Admin"
        });
})();