//////////////////////////////////////////////////////
////                ACCOUNT FUNCTIONS   //////////////
/////////////////////////////////////////////////////
var tempid = 0;
function saveAccountAddress() {
    $("#vldAccountAddress").removeClass("validation-summary-errors");
    $("#vldAccountAddress").addClass("validation-summary-valid");
    if (IsValidForm('#frmAccountAddress')) {
        var data = $("#frmAccountAddress").serializeArray();

        tempid--;
        data[0].value = tempid;
        $('#AccountAddressIds').append('<option selected value=' + tempid + '>' + data[2].value + '</option>');
        $("#AccountAddressIds").trigger("chosen:updated");
        $.post("/Account/_SaveAddresses", data, function (result) {
            // Do something with the response `res`
            LoadingPanel.Hide();
            ModalAccountAddress.Hide();
        })
    }
}
function saveAccountContact() {
    $("#vldAccountContact").removeClass("validation-summary-errors");
    $("#vldAccountContact").addClass("validation-summary-valid");
    if (IsValidForm('#frmAccountContact')) {
        var data = $("#frmAccountContact").serializeArray();

        tempid--;
        data[0].value = tempid;
        $('#AccountContactIds').append('<option selected value=' + tempid + '>' + data[2].value + '</option>');
        $("#AccountContactIds").trigger("chosen:updated");
        $.post("/Account/_SaveAccountContact", data, function (result) {
            // Do something with the response `res`
            LoadingPanel.Hide();
            ModalAccountContact.Hide();
        })
    }
}

function RefreshAccountTransactions() {
    var grid = GetDevexControlByName('gridviewAccountTransactions' + $("#AccountID").val());
    if (grid != null) {
        grid.Refresh();
    }
    if ($("#gridviewAccountTransactions").length > 0) {
        gridviewAccountTransactions.Refresh();
    }
}
function RecordAccountTransaction(isAdd) {
    $('#SelectedTransactionIDForPopup').val(isAdd ? 0 : parseInt($('#selkeysavetransactions').val()));
    ModalEditAccountTransaction.Show();
}
function OnBeginCallbackEditAccountTransaction(s, e) {
    e.customArgs["id"] = $('#SelectedTransactionIDForPopup').val();
    e.customArgs["accountId"] = $('#AccountID').val();
}

function SaveAccountTransaction() {
    $("#vldAccountContact").removeClass("validation-summary-errors");
    $("#vldAccountContact").addClass("validation-summary-valid");
    if (IsValidForm('#frmAccountTransaction')) {
        var data = $("#frmAccountTransaction").serializeArray();
        $.post("/Finances/SaveAccountTransaction",
            data,
            function (result) {
                // Do something with the response `res`
                LoadingPanel.Hide();
                ModalEditAccountTransaction.Hide();
                RefreshAccountTransactions();
            });
    }
}
