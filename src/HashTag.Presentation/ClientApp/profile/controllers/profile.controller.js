(function() {

    angular
        .module("profile")
        .controller("profileController", profileController);

    profileController.$inject = [
        "$scope", "$window", "$location", "$anchorScroll", "DEFAULT_URL", "communicationService", "profileService",
        "photoService", "addPhotosModalService"
    ];

    function profileController($scope, $window, $location, $anchorScroll, DEFAULT_URL, communicationService, profileService,
        photoService, addPhotosModalService) {
        var vm = this;

        vm.userName = $scope.$parent.userName;
        vm.fullName = "";
        vm.profilePhoto = {};
        vm.feedTrigger = 0;

        vm.openAddPhotosModal = openAddPhotosModal;
        vm.toHashTagSearch = toHashTagSearch;
        vm.feedMore = feedMore;
        vm.setPhotoAsProfile = setPhotoAsProfile;
        vm.deletePhoto = deletePhoto;

        activate();

        //
        function activate() {
            vm.photos = [];
            vm.errors = [];
            vm.isLoading = true;
            vm.feedStopped = false;
            vm.feedTrigger++;

            return profileService.getProfileInfo(vm.userName)
                .then(function(response) {
                    if (response.success) {
                        vm.fullName = response.data.fullName;
                        vm.profilePhoto = response.data.profilePhoto;
                    }
                    else {
                        vm.errors = response.messages;
                    }

                    vm.isLoading = false;
                });
        }

        function openAddPhotosModal() {
            addPhotosModalService.openModal(uploadPhotoCallBack);
        }

        function uploadPhotoCallBack(addedPhoto) {
            vm.photos.splice(0, 0, addedPhoto);
        }

        function toHashTagSearch(hashTag) {
            $window.location.href = communicationService.appUrl(`search/${hashTag}`);
        }

        function feedMore() {
            if (vm.feedStopped)
                return false;

            return profileService.getProfilePhotos(vm.userName, vm.photos.length)
                .then(function(response) {
                    if (response.success)
                        vm.feedStopped = response.data.length === 0;
                    else
                        vm.errors = response.messages;

                    return response;
                });
        }

        function setPhotoAsProfile(id) {
            return profileService.setPhotoAsProfile(id)
                .then(function(response) {
                    activate();
                    $location.hash("profile-top");
                    $anchorScroll();
                    return response;
                });
        }

        function deletePhoto(id) {
            return photoService.deletePhoto(id).then(function(response) {
                activate();
                return response;
            });
        }
    }

}());