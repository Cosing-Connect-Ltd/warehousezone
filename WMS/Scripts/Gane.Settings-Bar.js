﻿$(function () {
    setSettingsBarState();
});

var baseUISetting = {};

function setSettingsBarState() {
    var isSettingsBarExpanded = parseInt(localStorage.getItem('is-settings-bar-expanded')) == 1;
    if (isSettingsBarExpanded) {
        expandSettingsBar();
    }
}

function onUISettingChanged(s) {

    var itemKey = $("[name='" + s.name.replace('Value', 'UISettingItem.Key') + "']")[0].value

    var uiSettings = JSON.parse(localStorage.getItem("ui-settings"));

    var itemValue = !!s.color ? s.color : s.value;

    uiSettings[itemKey] = itemValue;

    localStorage.setItem("ui-settings", JSON.stringify(uiSettings));

    applyUISettings(itemKey, itemValue);
}

function saveUISettings() {
    if (IsValidForm('#frmSettingsBar')) {
        var data = $("#frmSettingsBar").serializeArray();

        LoadingPanel.Show();

        $.ajax({
            type: "POST",
            url: "/UISettings/Save",
            data: data,
            success: function () {
                LoadingPanel.Hide();
                collapseSettingsBar();

                baseUISettings = localStorage.getItem("ui-settings");
            },
            error: function (xhr, status, error) {
                console.log(error);
                LoadingPanel.Hide();
            }

        });
    }
}

function applyUISettings(settingsKey, itemValue) {
    $("body *").css({ "transition": "background-color 0.5s ease" });
    document.documentElement.style.setProperty(settingsKey, itemValue);
    setTimeout(function () { $("body *").css({ "transition": "" }) }, 500);
}

function undoUISettings() {
    localStorage.setItem("ui-settings", baseUISettings);
    applyAllUISettings();
    collapseSettingsBar();
}

function toggleSettingsBar() {

    if ($(".settings-bar").attr("expanded") != undefined) {

        collapseSettingsBar();
    }
    else {
        expandSettingsBar();
    }
}

function collapseSettingsBar() {
    $(".settings-bar").removeAttr("expanded");
    $(".settings-bar-switch").animate({ left: "-36px", borderWidth: "1px" }, 400);
    $(".settings-bar").animate({ right: "-300px" }, 400);
    localStorage.setItem("is-settings-bar-expanded", 0);
}

function expandSettingsBar() {
    $(".settings-bar").attr("expanded", '');
    $(".settings-bar-switch").animate({ left: "0px", borderWidth: "0px" }, 400);
    $(".settings-bar").animate({ right: "20px" }, 400);
    localStorage.setItem("is-settings-bar-expanded", 1);
}