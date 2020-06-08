$(function () {
    setSettingsBarState();
});

function onUISettingChanged(s) {
    var itemKey = s.getAttribute('data-settingsKey');

    var selector = s.getAttribute('data-selector');

    applyUISettings(itemKey, s.value, selector);
}

function saveUISettings() {
    if (IsValidForm('#frmSettingsBar')) {
        var data = $("#frmSettingsBar").serializeArray();

        $.ajax({
            type: "POST",
            url: "/UISettings/Save",
            data: data,
            success: function () {
                LoadingPanel.Hide();
                collapseSettingsBar();
            },
            error: function (xhr, status, error) {
                console.log(error);
                LoadingPanel.Hide();
            }

        });
    }
}

function setUISettingIdsInModel(data) {
    for (var i = 0; i < Object.keys(data).length; i++) {
        $("[name='[" + i + "].Id']")[0].value = data[$("[name='[" + i + "].UISettingItem.Key']")[0].value];
    }
}

function applyUISettings(settingsKey, itemValue, selector) {
    if (itemValue == undefined || itemValue == null || itemValue == '') { return; }

    $("body *").css({ "transition": "background-color 0.5s ease" });

    var targetElements = !selector ? $('body') : $(selector);

    for (var i = 0; i < targetElements.length; i++) {
        targetElements[i].style.setProperty(settingsKey, itemValue);
    }

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
            for (var i = 0; i < uiSettings.length; i++) {
                var key = uiSettings[i].UISettingItem.Key;
                var value = isResetToDefault ? uiSettings[i].UISettingItem['DefaultValue'] : uiSettings[i]['Value'];
                var selector = uiSettings[i].UISettingItem.Selector;

                applyUISettings(key, value, selector);

                var elementDataAttributes = '[data-settingsKey="' + key + '"]' + (!selector ? '[data-selector]' : '[data-selector= "' + selector + '"]');

                var elements = $('.settings-bar-content-items input' + elementDataAttributes + ', .settings-bar-content-items select' + elementDataAttributes);

                for (var j = 0; j < elements.length; j++) {
                    !!elements[j].jscolor && elements[j].jscolor.fromString(value);
                    elements[j].value = value;
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