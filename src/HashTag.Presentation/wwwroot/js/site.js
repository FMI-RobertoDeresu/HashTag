"use strict";

$(function() {
    var $body = $("body");
    var $checkboxToggleDisplay = $("input[type='checkbox'].toggle-display");

    $body.on("change", $checkboxToggleDisplay, toggleDisplay);

    function toggleDisplay(e) {
        var $checkbox = $(e.target);
        var $target = $($checkbox.data("toogle-display-target"));
        $target.toggle();
    }
});