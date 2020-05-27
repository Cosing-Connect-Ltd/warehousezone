$(function () {
    applyAllUISettings();

    getLatestUISettings();
});

var baseUISetting = {};

function getLatestUISettings() {
    $.ajax({
        type: "GET",
        url: "/UISettings/GetWebsiteUISetting",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (uiSettings) {

            applyAllUISettings(uiSettings);

            baseUISettings = JSON.stringify(uiSettings);
            localStorage.setItem("UISettings", baseUISettings);
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function applyAllUISettings(uiSettings) {
    if (!uiSettings) {
        var uiSettings = JSON.parse(localStorage.getItem("UISettings"));
    }

    for (var key in uiSettings) {
        document.documentElement.style.setProperty(key, uiSettings[key]);
    }
}