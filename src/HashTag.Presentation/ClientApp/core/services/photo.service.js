(function() {

    angular
        .module("core")
        .service("photoService", photoService);

    photoService.$inject = ["communicationService"];

    function photoService(communicationService) {
        const service = {
            computePrediction: computePrediction,
            computeDescriptionAndHashTags: computeDescriptionAndHashTags,
            uploadPhoto: uploadPhoto,
            deletePhoto: deletePhoto
        };

        return service;

        function computePrediction(photo, callback) {
            photo.url = communicationService.appUrl("api/photos/prediction");
            photo.isSuccess = false;
            photo.isUploaded = false;
            photo.prediction = [];

            photo.onComplete = function(response) {
                photo.isSuccess = false;
                photo.isUploaded = false;
                if (response.success) {
                    photo.prediction = response.data.prediction;
                    callback(response.data.prediction);
                }
            };
            photo.upload();
        }

        function computeDescriptionAndHashTags(photo) {
            photo.url = communicationService.appUrl("api/photos/descriptionAndHashTags");
            photo.description = "";
            photo.isSuccess = false;
            photo.isUploaded = false;

            photo.onComplete = function(response) {
                photo.isSuccess = false;
                photo.isUploaded = false;
                photo.descriptionComputed = true;
                if (response.success) {
                    photo.description = response.data.description;
                    photo.hashTags = response.data.hashTags;
                }
                else {
                    photo.description = "";
                    photo.hashTags = "";
                }
            };
            photo.upload();
        }

        function uploadPhoto(photo) {
            photo.url = communicationService.appUrl("api/photos/post");
            photo.formData[0] = {
                description: photo.description,
                hashtags: photo.hashTags
            };
            photo.upload();
        }

        function deletePhoto(id) {
            return communicationService.post("api/photos/delete", { id });
        }
    }

}());