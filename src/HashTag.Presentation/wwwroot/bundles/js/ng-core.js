(function() {
    const defaultUrl = document.getElementById("default-url").value;
    const clientAppUrl = `${defaultUrl}/clientapp/`.replace("//", "/");

    angular
        .module("core", ["bootstrapLightbox"])
        .constant("DEFAULT_URL", defaultUrl)
        .constant("CLIENT_APP_URL", clientAppUrl)
        .constant("PAHT_TEMPLATES", clientAppUrl + "core/templates/")
        .constant("PATH_DIRECTIVES_TEMPLATES", clientAppUrl + "core/templates/directives/")
        .config(function (LightboxProvider, PAHT_TEMPLATES, $qProvider) {
            LightboxProvider.templateUrl = PAHT_TEMPLATES + "customLightbox.html";
            $qProvider.errorOnUnhandledRejections(false); //to disable Lightbox close errors (-_-)
        });
}());
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
(function() {

    angular
        .module("core")
        .directive("hashtags", hashtagsDirective);

    function hashtagsDirective() {
        const directive = {
            link: link,
            restrict: "A"
        };

        return directive;

        function link(scope, elem) {
            elem.on("keypress", function(e) {
                if (this.value.length === 0)
                    this.value = `#${this.value}`;

                if (e.which === 32) {
                    e.preventDefault();
                    if (this.value[this.value.length - 1] !== "#")
                        this.value = `${this.value}#`;
                }
            });
        }
    }

}());
(function () {

    angular
        .module("core")
        .directive("fileButton", fileButtonDirective);

    fileButtonDirective.$inject = ["$compile"];

    function fileButtonDirective($compile) {
        const directive = {
            link: link,
            restrict: "A",
            scope: {
                uploader: "="
            }
        };

        return directive;

        function link(scope, elem, attributes) {
            const angularElement = angular.element(elem);
            const fileInput = $compile("<input type='file' class='file-upload' " +
                "nv-file-select uploader='uploader' multiple />")(scope);
            angularElement.append(fileInput);
        }
    }

}());
(function() {

    angular
        .module("core")
        .directive("upPhotoPreview", upPhotoPreviewDirective);

    upPhotoPreviewDirective.$inject = ["$window"];

    function upPhotoPreviewDirective($window) {
        var helper = {
            support: !!($window.FileReader && $window.CanvasRenderingContext2D),
            isFile: function(item) {
                return angular.isObject(item) && item instanceof $window.File;
            },
            isImage: function(file) {
                const type = `|${file.type.slice(file.type.lastIndexOf("/") + 1)}|`;
                return "|jpg|png|jpeg|bmp|gif|".indexOf(type) !== -1;
            }
        };

        const directive = {
            link: link,
            restrict: "A",
            scope: {
                photo: "=",
                height: "=",
                width: "=",
                maxWidth: "="
            },
            template: "<canvas/>"
        };

        return directive;

        function link(scope, element, attributes) {
            if (!helper.support) return;

            scope.$watch("photo", function(newValue, oldValue) {
                if (newValue !== undefined || oldValue !== undefined) {
                    if (!helper.isFile(scope.photo) || !helper.isImage(scope.photo)) {
                        clear();
                        return;
                    }

                    renderImage(scope.photo);
                }
            });

            function renderImage(iamge) {
                const reader = new FileReader();
                reader.onload = onLoadFile;
                reader.readAsDataURL(iamge);
            }

            function onLoadFile(event) {
                const img = new Image();
                img.onload = onLoadImage;
                img.src = event.target.result;
            }

            function onLoadImage() {
                let $canvas = element.find("canvas");
                let width =scope.width;
                let height = scope.height;

                if (this.height > this.width)
                    width = Math.min(this.width / this.height * scope.height, scope.width);
                else
                    height = Math.min(this.height / this.width * scope.width, scope.height);
                
                $canvas.attr({ width: width, height: height });
                $canvas[0].getContext("2d").drawImage(this, 0, 0, width, height);
            }

            function clear() {
                const $canvas = element.find("canvas");
                $canvas.removeAttr("height");
                $canvas[0].getContext("2d").clearRect(0, 0, $canvas.width(), $canvas.height());
            }
        }
    }
}());
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