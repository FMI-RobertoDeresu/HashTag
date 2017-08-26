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