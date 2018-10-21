(function() {
    const defaultUrl = `${document.getElementById("default-url").value}/`.replace("//", "/");
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