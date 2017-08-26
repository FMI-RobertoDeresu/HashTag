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