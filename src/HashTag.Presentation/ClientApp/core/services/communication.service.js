(function() {

    angular
        .module("core")
        .service("communicationService", communicationService);

    communicationService.$inject = ["$http", "DEFAULT_URL"];

    function communicationService($http, DEFAULT_URL) {
        const service = {
            appUrl: appUrl,
            get: get,
            post: post,
            put: put,
            'delete': del
        };

        return service;

        //
        function appUrl(url) {
            return DEFAULT_URL + url;
        }

        function get(url) {
            return $http.get(appUrl(url)).then(successCallback, errorCallback);
        }

        function post(url, data) {
            return $http.post(appUrl(url), data).then(successCallback, errorCallback);
        }

        function put(url, data) {
            return $http.put(appUrl(url), data).then(successCallback, errorCallback);
        }

        function del(url) {
            return $http.delete(appUrl(url)).then(successCallback, errorCallback);
        }

        function successCallback(response) {
            showAlerts(response.data.alerts);
            return response.data;
        }

        function errorCallback(response) {
            if (response.status === 404)
                response.data = {
                    succes: false,
                    alerts: [{ "class": "error", message: "Page not found!" }],
                    data: {}
                };

            showAlerts(response.data.alerts);
            return response.data;
        }
    }

}());