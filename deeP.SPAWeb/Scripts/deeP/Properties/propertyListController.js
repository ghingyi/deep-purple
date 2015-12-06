
(function () {
    "use strict";
    angular.module("deePProperties")
        .controller("propertyListController", [
            function () {
                this.properties = [{}];
                this.refreshList = function (restricted) {
                    // Note: fooling this can lead to retrieving properties owned by others
                    // with a potential to open edit view on them; the server will reject these attempts

                }
            }]);
})();