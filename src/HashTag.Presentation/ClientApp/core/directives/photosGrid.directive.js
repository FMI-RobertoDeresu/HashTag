(function() {

    angular
        .module("core")
        .directive("photosGrid", photosGridDirective);

    photosGridDirective.$inject = ["$timeout", "PATH_DIRECTIVES_TEMPLATES", "Lightbox"];

    function photosGridDirective($timeout, PATH_DIRECTIVES_TEMPLATES, Lightbox) {

        const directive = {
            link: link,
            restrict: "E",
            scope: {
                photos: "=",
                defaultUrl: "=",
                searchByHashTagFn: "&",
                setPhotoAsProfileFn: "&",
                deletePhotoFn: "&"
            },
            templateUrl: PATH_DIRECTIVES_TEMPLATES + "photosGrid.html"
        };

        return directive;

        function link(scope, element, attrs) {
            scope.searchByHashTag = searchByHashTag;
            scope.setPhotoAsProfile = setPhotoAsProfile;
            scope.deletePhoto = deletePhoto;
            scope.openLightboxModal = openLightboxModal;

            function searchByHashTag(hashTag) {
                scope.searchByHashTagFn({ hashTag: hashTag });
            }

            function setPhotoAsProfile(id) {
                scope.setPhotoAsProfileFn({ id: id });
            }

            function deletePhoto(id) {
                scope.deletePhotoFn({ id: id });
            }

            function openLightboxModal(index) {
                Lightbox.openModal(scope.photos, index);
            };
        }
    }

})();