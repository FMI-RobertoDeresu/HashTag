(function () {

    var clientAppUrl;
    angular.injector(["ng", "core"]).invoke(function (CLIENT_APP_URL) {
        clientAppUrl = CLIENT_APP_URL;
    });

    angular
        .module("search", ["core", "ngRoute", "angularFileUpload", "infinite-scroll"])
        .config(configure)
        .constant("PAHT_TEMPLATES", clientAppUrl + "search/templates/");

    function configure($routeProvider, PAHT_TEMPLATES) {
        $routeProvider.when("/",
        {
            templateUrl: PAHT_TEMPLATES + "search.html",
            controller: "searchController",
            controllerAs: "searchVM"
        });
    }

})();