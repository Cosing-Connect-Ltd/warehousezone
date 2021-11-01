var command = "";
function beginCallBackEmployeeShift(s, e) {
    if (command !== "add") {
        var id = $('.selkey').val();
        e.customArgs["ShiftId"] = $('.selkey').val();
    }
}

function endOrderDetailCallback(s, e) {
    if (command !== "add") {
        var id = $('.selkey').val();
        e.customArgs["ShiftId"] = $('.selkey').val();
    }
}

function PopupFormValidationCheck(e) {
    $.validator.setDefaults({ ignore: ":hidden:not(.chosen-select)" });
    var form = $('.validate-popup-form');

    $(form).data('validator', null);
    $.validator.unobtrusive.parse(form);

    form.validate();
    if (form.valid() !== true) {
        e.preventDefault();
        return false;
    }
    else {
        return true;
    }
}

function AddEditTimings(e) {
    command = e.id;
    ModelCreateEmployeeShifts.Show();
}

function OnSuccess(data) {
    EmployeeShiftsGridview.Refresh();
    ModelCreateEmployeeShifts.Hide();

}

function deleteResourcesShift() {

    var res = confirm("Are you sure you want to delete?");
    if (!res)
        return false;

    var id = $('.selkey').val();
    if (id <= 0) {
        alert("No timelog selected to delete");
        return false;
    }

    $.ajax({
        type: "POST",
        url: '/EmployeeShifts/deleteResourceShift',
        data: { id: id },
        dataType: 'json',
        success: function (data) {
            if (data) {
                EmployeeShiftsGridview.Refresh();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            // alert(xhr.status);
            alert('Error' + textStatus + "/" + errorThrown);

        }
    });

}