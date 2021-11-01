var tempid = 0;
var total = 0;
function setLocValue(val) {

    $.get({
        url: "/Locations/_SetValue",
        data: { "Id": val },
        success: function (e) {
            ModalLocations.Show();
        },
    });
}

function locationTypePost() {

    $("#vldLocationType").removeClass("validation-summary-errors");
    $("#vldLocationType").addClass("validation-summary-valid");
    if (IsValidForm('#frmltype')) {
         LoadingPanel.Show();
        var data = $("#frmltype").serializeArray();
        $.post("/Locations/_LocationTypeSubmit", data, function (result) {
            if (result.error == false) {
                $('#drpltype').append('<option selected value=' + result.id + '>' + result.type + '</option>');
                $("#drpltype").trigger("chosen:updated");
                $("select #LocationTypeId").trigger("chosen:updated");

                ModalLocationType.Hide();
                LoadingPanel.Hide();
            }
            else {
                var ul = $("#vldLocationType ul");
                $("#vldLocationType").addClass("validation-summary-errors");
                //0347-5501552 rafiq
                $("#vldLocationType").removeClass("validation-summary-valid");
                ul.html("");
                ul.append("<li> " + result.msg + "</li>");
                LoadingPanel.Show();
            }
        })
    }
}
function locationGroupPost() {

    $("#vldLocationGroup").removeClass("validation-summary-errors");
    $("#vldLocationGroup").addClass("validation-summary-valid");
    if (IsValidForm('#frmlgroup')) {
         LoadingPanel.Show();
        var data = $("#frmlgroup").serializeArray();
        $.post("/Locations/_LocationGroupSubmit", data, function (result) {
            if (result.error == false) {
                $('#drplgroup').append('<option selected value=' + result.id + '>' + result.type + '</option>');
                $("#drplgroup").trigger("chosen:updated");
                ModalLocationGroup.Hide();
                LoadingPanel.Hide();
            }
            else {
                var ul = $("#vldLocationGroup ul");
                $("#vldLocationGroup").addClass("validation-summary-errors");
                //0347-5501552 rafiq
                $("#vldLocationGroup").removeClass("validation-summary-valid");
                ul.html("");
                ul.append("<li> " + result.msg + "</li>");
                LoadingPanel.Hide();
            }
        })
    }
}