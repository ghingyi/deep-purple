(function () {
    "use strict";

    // Specify configuration constants
    var deePConfig = angular.module("deePConfig", []);
    deePConfig.constant("authorizationServiceUrl", "//localhost:62635/");
    deePConfig.constant("propertyServiceUrl", "//localhost:65120/");
    deePConfig.constant("navigationUris",
        {
            home: "/",
            login: "/login",
            registerSeller: "/register/seller",
            registerBuyer: "/register/buyer",
            user: "/user",
            propertyDetails: "/property"
        });
    deePConfig.constant("roleNames",
        {
            seller: "Seller",
            buyer: "Buyer",
            admin: "Admin"
        });
})();