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