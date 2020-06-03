$(function () {
    setSettingsBarState();
});

function onUISettingChanged(s) {
    var itemKey = s.getAttribute('data-settingsKey');

    applyUISettings(itemKey, s.value);
}

function saveUISettings() {
    if (IsValidForm('#frmSettingsBar')) {
        var data = $("#frmSettingsBar").serializeArray();

        $.ajax({
            type: "POST",
            url: "/UISettings/Save",
            data: data,
            success: function () {
                collapseSettingsBar();
            },
            error: function (xhr, status, error) {
                console.log(error);
            }

        });
    }
}

function setUISettingIdsInModel(data) {
    for (var i = 0; i < Object.keys(data).length; i++) {
        $("[name='[" + i + "].Id']")[0].value = data[$("[name='[" + i + "].UISettingItem.Key']")[0].value];
    }
}

function applyUISettings(settingsKey, itemValue) {
    if (itemValue == undefined || itemValue == null || itemValue == '') { return; }

    $("body *").css({ "transition": "background-color 0.5s ease" });
    $('body')[0].style.setProperty(settingsKey, itemValue);
    setTimeout(function () { $("body *").css({ "transition": "" }) }, 500);
}

function cancelChanges() {
    setUISettings(false);
    collapseSettingsBar();
}

function setDefaultValues() {
    setUISettings(true);
}

function setUISettings(isResetToDefault) {
    $.ajax({
        type: "GET",
        url: "/UISettings/GetUISettingValues",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (uiSettings) {
            for (var key in uiSettings) {
                var value = uiSettings[key][isResetToDefault ? 'DefaultValue' : 'Value'];

                applyUISettings(key, value);

                var elements = $('.settings-bar-content-items input[data-settingsKey="' + key + '"], .settings-bar-content-items select[data-settingsKey="' + key + '"]');

                for (var i = 0; i < elements.length; i++) {
                    !!elements[i].jscolor && elements[i].jscolor.fromString(value);
                    elements[i].value = value;
                }
            }
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function IsValidForm(form) {
    InitializeValidationRulesForForm(form);
    var validator = $.data($(form)[0], 'validator');
    if (validator == null) return true;
    return validator.form();
}

function InitializeValidationRulesForForm(form) {
    var form = $(form);
    if (form.attr("executed"))
        return;
    form.removeData("validator");
    $.validator.unobtrusive.parse(document);
    form.executed = true;
}

function setSettingsBarState() {
    var isSettingsBarExpanded = parseInt(localStorage.getItem('is-settings-bar-expanded')) == 1;
    if (isSettingsBarExpanded) {
        expandSettingsBar();
    }
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
    $(".settings-bar").animate({ right: "-350px" }, 400);
    localStorage.setItem("is-settings-bar-expanded", 0);
}

function expandSettingsBar() {
    $(".settings-bar").attr("expanded", '');
    $(".settings-bar-switch").animate({ left: "0px", borderWidth: "0px" }, 400);
    $(".settings-bar").animate({ right: "10px" }, 400);
    localStorage.setItem("is-settings-bar-expanded", 1);
}