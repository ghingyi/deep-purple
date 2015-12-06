/// <reference path="../jasmine/jasmine.js" />
/// <reference path="../angular.js" />
/// <reference path="../angular-mocks.js" />
/// <reference path="../deeP/deeP-init.js" />
/// <reference path="../deeP/app-accountService.js" />

describe("AccountService", function () {

    beforeEach(module("deePAuth"));

    beforeEach(inject(function ($httpBackend) {
        this.httpBackend = $httpBackend;
    }));

    afterEach(function () {
        this.httpBackend.verifyNoOutstandingRequest();
        this.httpBackend.verifyNoOutstandingExpectation();
    });

    it("login succeeds", inject(function (authorizationServiceUrl, accountService) {

        // These are the credentials we want authroized
        var creds = {
            username: "Dummy",
            password: "Pass123"
        };

        // Expect good and bad requests and respond accordingly
        this.httpBackend.expect("POST", authorizationServiceUrl + "oauth/token").respond(
            function (method, url, data, headers, params) {
                if (data === "grant_type=password&username=Dummy&password=Pass123") {
                    return [200, { access_token: "testaccesstoken" }];
                } else {
                    return [401, { error_description: "Invalid user name or password." }];
                }
            });
        this.httpBackend.expect("GET", authorizationServiceUrl + "api/accounts/getuser").respond({ roles: ["Seller"] });

        // Call login; we expect two requests before the promise evaluates
        var result;
        accountService.login(creds).then(
            function (response) {
                result = response;
            },
            function (error) {
                result = error;
            });

        // Flush calls and check expectations
        this.httpBackend.flush();

        // Check results
        expect(result.username).toBe(creds.username);
        expect(result.accessToken).toBe("testaccesstoken");
        expect(result.roles).toEqual(["Seller"]);
    }));

    it("login fails", inject(function (authorizationServiceUrl, accountService) {

        // These are the credentials we want authroized
        var creds = {
            username: "SomeOtherDummy",
            password: "WrongPass"
        };

        // Expect good and bad requests and respond accordingly
        this.httpBackend.expect("POST", authorizationServiceUrl + "oauth/token").respond(
            function (method, url, data, headers, params) {
                if (data === "grant_type=password&username=Dummy&password=Pass123") {
                    return [200, { access_token: "testaccesstoken" }];
                } else {
                    return [401, { error_description: "Invalid user name or password." }];
                }
            });

        // Call login; we expect only the first request to be made before the promise evaluates
        var result;
        accountService.login(creds).then(
            function (response) {
                result = response;
            },
            function (error) {
                result = error;
            });

        // Flush calls and check expectations
        this.httpBackend.flush();

        // Check results
        expect(result).toBeDefined();
        expect(result.data).toBeDefined();
        expect(result.data.error_description).toBe("Invalid user name or password.");
    }));

    it("logout cleans up", inject(function (accountContext, accountService) {
        accountContext.username = "Dummy";
        accountContext.accessToken = "testaccesstoken";
        accountContext.roles = ["Seller"];

        // Call logout; we expect it to clean up
        accountService.logout();

        // Check results
        expect(accountContext).toBeDefined();
        expect(accountContext.username).toBe(null);
        expect(accountContext.accessToken).toBe(null);
        expect(accountContext.roles).toBe(null);
    }));

    it("register seller makes call", inject(function (authorizationServiceUrl, accountService) {
        // These are the details we want registered
        var userDetails = {
            email: "dummy@mail.com",
            firstName: "Bugs",
            middleNames: "Animaniac",
            lastName: "Bunny",
            username: "Dummy",
            password: "Pass123",
            confirmPassword: "Pass123"
        };

        // Expect good and bad requests and respond accordingly
        this.httpBackend.expect("POST", authorizationServiceUrl + "api/accounts/createseller").respond(
            function (method, url, data, headers, params) {
                if (angular.equals(angular.fromJson(data), userDetails)) {
                    return [200, {}];
                } else {
                    return [400, { modelState: { "": "Invalid user details." } }];
                }
            });

        // Call registerSeller; we expect only once call to be made
        var result;
        accountService.registerSeller(userDetails).then(
            function (response) {
                result = true;
            },
            function (error) {
                result = false;
            });

        // Flush calls and check expectations
        this.httpBackend.flush();

        // Check results
        expect(result).toBe(true);
    }));

    it("register buyer makes call", inject(function (authorizationServiceUrl, accountService) {
        // These are the details we want registered
        var userDetails = {
            email: "dummy@mail.com",
            firstName: "Bugs",
            middleNames: "Animaniac",
            lastName: "Bunny",
            username: "Dummy",
            password: "Pass123",
            confirmPassword: "Pass123"
        };

        // Expect good and bad requests and respond accordingly
        this.httpBackend.expect("POST", authorizationServiceUrl + "api/accounts/createbuyer").respond(
            function (method, url, data, headers, params) {
                if (angular.equals(angular.fromJson(data), userDetails)) {
                    return [200, {}];
                } else {
                    return [400, { modelState: { "": "Invalid user details." } }];
                }
            });

        // Call registerSeller; we expect only once call to be made
        var result;
        accountService.registerBuyer(userDetails).then(
            function (response) {
                result = true;
            },
            function (error) {
                result = false;
            });

        // Flush calls and check expectations
        this.httpBackend.flush();

        // Check results
        expect(result).toBe(true);
    }));
});
