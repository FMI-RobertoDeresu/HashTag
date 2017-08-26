(function() {

    var clientAppUrl;
    angular.injector(["ng", "core"]).invoke(function (CLIENT_APP_URL) {
        clientAppUrl = CLIENT_APP_URL;
    });

    angular
        .module("profile", ["core", "ngRoute", "angularFileUpload", "ui.bootstrap", "luegg.directives", "infinite-scroll"])
        .config(configure)
        .constant("PAHT_TEMPLATES", clientAppUrl + "profile/templates/");

    function configure($routeProvider, PAHT_TEMPLATES) {
        $routeProvider.when("/",
        {
            templateUrl: PAHT_TEMPLATES + "profile.html",
            controller: "profileController",
            controllerAs: "vm"
        });
    }

}());