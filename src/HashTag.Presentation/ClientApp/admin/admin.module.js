(function() {

    var clientAppUrl;
    angular.injector(["ng", "core"]).invoke(function (CLIENT_APP_URL) {
        clientAppUrl = CLIENT_APP_URL;
    });

    angular
        .module("admin", ["core", "ngRoute", "ngMaterial"])
        .config(configure)
        .constant("PAHT_TEMPLATES", clientAppUrl + "admin/templates/");

    function configure($routeProvider, PAHT_TEMPLATES) {
        $routeProvider.when("/",
        {
            templateUrl: PAHT_TEMPLATES + "admin.html",
            controller: "adminController",
            controllerAs: "adminVM"
        });
    }

}());