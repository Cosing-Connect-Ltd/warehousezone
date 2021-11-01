function beginGRCallBack(s, e) {
    var Req_url = window.location.href;
    var id = prdid.GetValue();
    var orderid = $('#grorderid').val();
    if (id === null || id === undefined || id === "") {
        id = $("#wrProducts option:selected").val();
        orderid = 0;
    }
    e.customArgs["pid"] = id;
    e.customArgs["order"] = orderid;
    e.customArgs["looslyRetrun"] = $(".losslyreturn").prop("checked");
}

function beginAddAdjustmentCallBack(s, e) {
    e.customArgs["prodId"] = $('#prodId').val();
    e.customArgs["detail"] = $('#detail').val();
}
function beginchequeCallBack(s, e) {
    e.customArgs["accounttransId"] = $('#accounttransId').val();
   
}

function searchDetail(pid, detail) {

    $('#prodId').val(pid);
    $('#detail').val(detail);
    if (pid !== "" || pid !== null || pid > 0) {
        ModelAdjustmentInformation.Show();
    }

}

function chequeDetail(AccountTransactionId) {

    if (AccountTransactionId !== "" || AccountTransactionId !== null || AccountTransactionId > 0) {
        $("#accounttransId").val(AccountTransactionId);
        ModelChequeDetail.Show();
    }

}


function directAdjustmentIn(palletTrackingId) {

    if (confirm('Are you sure to book in this pallet without PO?')) {
        var url = "/inventorytransaction/_SubmitPalleteSerials/";
        var type = 6;
        var cases = [];
        var data = { serialList: cases, pid: null, orderId: null, type: type, palletTrackingId: palletTrackingId, groupToken:"" };
        LoadingPanel.Show();
        $.post(url, data, function (result) {
            LoadingPanel.Hide();
            _PalletTrackingListGridView.Refresh();
        });
    }

}
//-------------------------pallet related functions written here---------------------------------
function DeletePallet(palletId) {
    if (confirm('Are you sure you want to delete this pallet?')) {
        var url = "/Pallets/DeletePallet/";
        var data = { palletId: palletId };
        LoadingPanel.Show();
        $.post(url, data, function (result) {
            LoadingPanel.Hide();
            PalletsListGridView1.Refresh();

        });
    }

}
