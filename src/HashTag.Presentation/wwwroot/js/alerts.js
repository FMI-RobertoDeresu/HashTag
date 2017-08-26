"use strict";

(function () {

    //toastr config
    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "showDuration": "1000",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "5000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

    //
    function showAlerts(alerts) {
        if (isNullOrUndefined(alerts))
            return;
            
        for (let i = 0; i < alerts.length; i++)
            showAlert(alerts[i]);
    }

    function showAlert(alert) {
        if (isNullOrUndefined(alert) || isNullOrUndefined(alert.message))
            return;

        toastr[alert.class](alert.message);
    }

    //
    window.showAlerts = showAlerts;
    window.showAlert = showAlert;

})();