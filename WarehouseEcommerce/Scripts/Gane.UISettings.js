$(function () {
    applyAllUISettings();

    getLatestUISettings();
});

var baseUISetting = "";

function getLatestUISettings() {
    $.ajax({
        type: "GET",
        url: "/UISettings/GetWebsiteUISetting",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (uiSettings) {

            uiSettings = fillEmptyUISettingsValue(uiSettings);

            applyAllUISettings(uiSettings);

            baseUISettings = JSON.stringify(uiSettings);
            localStorage.setItem("ui-settings", baseUISettings);
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function fillEmptyUISettingsValue(uiSettings) {
    for (var key in uiSettings) {
        if (uiSettings[key] == undefined || uiSettings[key] == null || uiSettings[key] == '') {
            value = getComputedStyle(document.documentElement).getPropertyValue(key);

            setUIElementValue(key, value);

            uiSettings[key] = value;
        }
    }

    return uiSettings;
}

function setUIElementValue(key , value) {
    var elements = $('input[settingsKey="' + key + '"]');

    for (var i = 0; i < elements.length; i++) {
        !!elements[i].jscolor && elements[i].jscolor.fromString(value);

        elements[i].value = value;
    }
}

function applyAllUISettings(uiSettings) {
    if (!uiSettings) {
        var uiSettings = JSON.parse(localStorage.getItem("ui-settings"));
    }

    for (var key in uiSettings) {
        $('body')[0].style.setProperty(key, uiSettings[key]);

    }
}