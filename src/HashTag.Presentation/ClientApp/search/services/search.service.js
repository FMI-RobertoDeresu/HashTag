(function() {

    angular
        .module("search")
        .factory("searchService", searchService);

    searchService.$inject = ["communicationService"];

    function searchService(communicationService) {
        const service = {
            search: search,
            searchByHashTag: searchByHashTag,
            searchByDescription: searchByDescription,
            searchByPrediction: searchByPrediction
        };

        return service;

        function search(currentFeedSize) {
            return communicationService.post("api/search", { currentFeedSize: currentFeedSize });
        }

        function searchByHashTag(hashTag, currentFeedSize) {
            return communicationService.post("api/search/byHashTag",
                { hashTag: hashTag, currentFeedSize: currentFeedSize });
        }

        function searchByDescription(description, currentFeedSize) {
            return communicationService.post("api/search/byDescription/",
                { description: description, currentFeedSize: currentFeedSize });
        }

        function searchByPrediction(prediction, currentFeedSize) {
            return communicationService.post("api/search/byPrediction/",
                { prediction: prediction, currentFeedSize: currentFeedSize });
        }
    }

})();