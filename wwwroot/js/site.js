// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

document.addEventListener('DOMContentLoaded', function () {
    var consent = getCookie("userConsent");
    if (consent == null) {
        document.getElementById("cookieConsent").style.display = "block";
    } else {
        document.getElementById("cookieConsent").style.display = "none";
    }

    var acceptButton = document.getElementById("acceptCookieConsent");
    if (acceptButton) {
        acceptButton.addEventListener("click", function () {
            setCookie("userConsent", "true", 365); // Set the consent cookie for 1 year
            document.getElementById("cookieConsent").style.display = "none";
        });
    }
});
