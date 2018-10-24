(function() {

    angular
        .module("search")
        .controller("searchController", searchController);

    searchController
        .$inject = ["$scope", "$window", "FileUploader", "searchService", "photoService", "communicationService", "DEFAULT_URL"];

    function searchController($scope, $window, FileUploader, searchService, photoService, communicationService, DEFAULT_URL) {
        const searchVM = this;

        searchVM.defaultUrl = DEFAULT_URL;
        searchVM.feedStopped = false;
        searchVM.errors = [];
        searchVM.searchByPhotoButtonText = "Filter by photo (click to upload)..";
        searchVM.photos = [];
        searchVM.feedTrigger = 0;
        searchVM.lastFilter = "none";

        searchVM.filter = {
            visible: !$scope.$parent.hashTag,
            hashTag: $scope.$parent.hashTag,
            description: "",
            photo: {
                file: {},
                prediction: []
            }
        };

        searchVM.photoUploader = new FileUploader();
        searchVM.photoUploader.onAfterAddingFile = afterAddingPhoto;

        searchVM.search = search;
        searchVM.searchByHashTag = searchByHashTag;
        searchVM.searchByDescription = searchByDescription;
        searchVM.searchByPrediction = searchByPrediction;
        searchVM.resetFilter = resetFilter;

        searchVM.toHashTagSearch = toHashTagSearch;
        searchVM.feedMore = feedMore;

        activate();

        function activate() {
            if (searchVM.filter.hashTag.length > 0)
                searchByHashTag();
        }

        function afterAddingPhoto(selectedPhoto) {
            searchVM.filter.photo = {
                file: {},
                prediction: []
            };
            searchVM.searchByPhotoButtonText = "Uploading photo..";
            photoService.computePrediction(selectedPhoto, function(prediction) {
                searchVM.filter.photo.file = selectedPhoto;
                searchVM.filter.photo.prediction = prediction;
                searchVM.searchByPhotoButtonText = selectedPhoto.file.name;
            });
        }

        function search(isFeedRequest) {
            searchVM.lastFilter = "none";
            return searchServiceCall(isFeedRequest, function() {
                return searchService.search(searchVM.photos.length);
            });
        }

        function searchByHashTag(isFeedRequest) {
            searchVM.lastFilter = "hashtag";
            return searchServiceCall(isFeedRequest, function() {
                return searchService.searchByHashTag(searchVM.filter.hashTag, searchVM.photos.length);
            });
        }

        function searchByDescription(isFeedRequest) {
            searchVM.lastFilter = "description";
            return searchServiceCall(isFeedRequest, function() {
                return searchService.searchByDescription(searchVM.filter.description, searchVM.photos.length);
            });
        }

        function searchByPrediction(isFeedRequest) {
            searchVM.lastFilter = "prediciton";
            return searchServiceCall(isFeedRequest, function() {
                return searchService.searchByPrediction(searchVM.filter.photo.prediction, searchVM.photos.length);
            });
        }

        function resetFilter() {
            searchVM.searchByPhotoButtonText = "Filter by photo (click to upload)..";
            searchVM.lastFilter = "none";
            searchVM.feedStopped = false;
            searchVM.filter = {
                visible: true,
                hashTag: "",
                description: "",
                photo: {
                    file: {},
                    prediction: []
                }
            };
            searchVM.photos = [];
            searchVM.feedTrigger++;
        }

        function toHashTagSearch(hashTag) {
            $window.location.href = communicationService.appUrl(`search/${hashTag}`);
        }

        function feedMore() {
            if (searchVM.feedStopped)
                return false;

            switch (searchVM.lastFilter) {
                case "none":
                    return search(true);
                case "hashtag":
                    return searchByHashTag(true);
                case "description":
                    return searchByDescription(true);
                case "prediciton":
                    return searchByPrediction(true);
                default:
                    return null;
            }
        }

        function searchServiceCall(isFeedRequest, action) {
            if (!isFeedRequest) {
                searchVM.photos = [];
                searchVM.feedStopped = false;
                searchVM.feedTrigger++;
                return null;
            }

            return action().then(function(response) {
                if (response.success)
                    searchVM.feedStopped = response.data.length === 0;
                else
                    searchVM.errors = response.messages;

                return response;
            });
        }
    }
})();