(function() {

    angular
        .module("profile")
        .service("profileService", profileService);

    profileService.$inject = ["communicationService"];

    function profileService(communicationService) {
        const service = {
            getProfileInfo: getProfileInfo,
            getProfilePhotos: getProfilePhotos,
            setPhotoAsProfile: setPhotoAsProfile
        };

        return service;

        function getProfileInfo(userName) {
            return communicationService.get(`api/users/get/${userName}`);
        }

        function getProfilePhotos(userName, currentFeedSize) {
            return communicationService.get(`api/users/getProfilePhotos/${userName}/${currentFeedSize}`);
        }

        function setPhotoAsProfile(id) {
            return communicationService.post("api/users/setProfilePhoto", { id });
        }
    }

}());