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
(function() {

    angular
        .module("profile")
        .service("addPhotosModalService", addPhotosModalService);

    addPhotosModalService.$inject = ["$uibModal", "PAHT_TEMPLATES"];

    function addPhotosModalService($uibModal, PAHT_TEMPLATES) {
        const service = {
            openModal: openModal
        };

        return service;

        function openModal(uploadPhotoCallback) {
            const modalInstance = $uibModal.open({
                animation: true,
                templateUrl: PAHT_TEMPLATES + "addPhotosModal.html",
                controller: modalController,
                controllerAs: "modalVM",
                resolve: {
                    uploadPhotoCallBack: function() {
                        return uploadPhotoCallback;
                    }
                }
            });
        }
    }

    function modalController($uibModalInstance, photoService, FileUploader, uploadPhotoCallBack) {
        const modalVM = this;
        var closeWhenUploadQueueIsEmpty = false;
        var uploadQueueSize = 0;

        modalVM.uploader = new FileUploader();
        modalVM.uploader.onAfterAddingFile = afterAddingFile;
        modalVM.uploadPhoto = uploadPhoto;
        modalVM.removePhoto = removePhoto;
        modalVM.uploadAllPhotos = uploadAllPhotos;

        modalVM.done = done;
        modalVM.cancel = cancel;

        //
        function afterAddingFile(selectedPhoto) {
            const uploader = new FileUploader();
            uploader.queue.push(selectedPhoto);
            selectedPhoto.uploader = uploader;
            photoService.computeDescriptionAndHashTags(selectedPhoto);
        }

        function uploadPhoto(selectedPhoto) {
            selectedPhoto.isUploading = true;

            selectedPhoto.onComplete = function(response) {
                selectedPhoto.isUploaded = false;
                selectedPhoto.isUploading = false;
                selectedPhoto.isSuccess = false;
                selectedPhoto.isError = false;

                if (response.success) {
                    selectedPhoto.isSuccess = true;
                    showAlerts(response.alerts);

                    $.extend(selectedPhoto, response.data);
                    uploadPhotoCallBack(selectedPhoto);

                    if (--uploadQueueSize === 0 && closeWhenUploadQueueIsEmpty)
                        done();
                }
                else {
                    selectedPhoto.isError = true;
                    showAlerts(response.alerts);
                }
            };

            photoService.uploadPhoto(selectedPhoto);
        };

        function removePhoto(selectedPhoto) {
            modalVM.uploader.removeFromQueue(selectedPhoto);
        }

        function uploadAllPhotos() {
            closeWhenUploadQueueIsEmpty = true;
            uploadQueueSize = modalVM.uploader.queue.length;
            modalVM.uploader.queue.forEach(function(item) {
                if (item.descriptionComputed && !(item.isUploading || item.isSuccess))
                    uploadPhoto(item);
            });
        }

        function done() {
            $uibModalInstance.close();
        }

        function cancel() {
            $uibModalInstance.dismiss("cancel");
        }
    }

}());
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
                        vm.userName = response.data.userName;
                        vm.currentUserName = response.data.currentUserName;
                        vm.showAddButton = vm.userName === vm.currentUserName;
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