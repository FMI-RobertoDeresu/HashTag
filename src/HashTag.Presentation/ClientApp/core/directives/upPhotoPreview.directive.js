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