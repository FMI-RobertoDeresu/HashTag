(function() {

    angular
        .module("core")
        .directive("infinitePhotosGrid", infignitePhotosGridDirective);

    infignitePhotosGridDirective.$inject = ["$timeout", "PATH_DIRECTIVES_TEMPLATES"];

    function infignitePhotosGridDirective($timeout, PATH_DIRECTIVES_TEMPLATES) {

        const directive = {
            link: link,
            restrict: "E",
            scope: {
                photos: "=",
                defaultUrl: "=",
                feedTrigger: "=",
                feedMoreFn: "&",
                searchByHashTagFn: "&",
                setPhotoAsProfileFn: "&",
                deletePhotoFn: "&"
            },
            templateUrl: PATH_DIRECTIVES_TEMPLATES + "infinitePhotosGrid.html"
        };

        return directive;

        function link(scope) {
            scope.isFeeding = false;
            scope.feedMore = feedMore;

            scope.searchByHashTag = searchByHashTag;
            scope.setPhotoAsProfile = setPhotoAsProfile;
            scope.deletePhoto = deletePhoto;

            scope.$watch("feedTrigger", function() {
                feedMore();
            }, true);

            function feedMore() {
                scope.isFeeding = true;
                const feedResult = scope.feedMoreFn();

                if (typeof feedResult === "object" && feedResult !== null) {
                    feedResult.then(function(response) {
                        scope.isFeeding = false;
                        if (response.success) {
                            scope.photos.push.apply(scope.photos, response.data);
                            scope.feedStopped = response.data.length === 0;
                        }
                    });
                }
                else {
                    scope.isFeeding = false;
                }
            }

            function searchByHashTag(hashTag) {
                scope.searchByHashTagFn({ hashTag: hashTag });
            }

            function setPhotoAsProfile(id) {
                scope.setPhotoAsProfileFn({ id: id });
            }

            function deletePhoto(id) {
                scope.deletePhotoFn({ id: id });
            }
        }
    }

})();