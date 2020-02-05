function IsValidEmail(email) {
    var regExp = /(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])/i;
    return regExp.test(email);
}
var ordersId;
var templateid;
var customOrderId;
var orderProcessId;
var ftAccountId;
var startDate;
var endDate;
var PORStatus;
var editdelivery;

function beginAddCustomEmailPopup(s, e) {
    var ftReport = $("#FTReport").val();
    if (ftReport !== undefined) {
        e.customArgs["accountId"] = ftAccountId;
        e.customArgs["StartDate"] = startDate;
        e.customArgs["EndDate"] = endDate;
    }
    else if (PORStatus === true) {
        e.customArgs["orderId"] = ordersId;
        e.customArgs["PORStatus"] = PORStatus;
    }
    else {
        if (customOrderId == undefined) {
            ordersId = $("#selkeySalesOrderList").val();
            if (ordersId == undefined) {
                ordersId = $("#selkeyOrderList").val();
            }
        }
        else {
            ordersId = customOrderId;
        }
        e.customArgs["orderId"] = ordersId;
        e.customArgs["templateId"] = templateid;


    }
}
$(document).ready(function () {
    $('body').on('click', '.custom-email', function () {
        templateid = undefined;
        ModalPopupCustomEmail.Show();

    });
});

function CustomEmails(orderid, template) {
    templateid = template;
    customOrderId = orderid;
    ModalPopupCustomEmail.Show();

}
function CustomEmailReports(accountIdReport, startDates, endDates, orderId) {
    if (orderId !== undefined && orderId > 0 && orderId !== null && orderId !== "") {
        PORStatus = true;
        ordersId = orderId;
        ModalPopupCustomEmail.Show();

    }
    else {
        ftAccountId = accountIdReport;
        startDate = startDates;
        endDate = endDates;
        ModalPopupCustomEmail.Show();

    }
}


function CustomEmailSubmit() {
    var allEmailsValid = true;
    var notifyEnabled = true;
    if (notifyEnabled && $("#CustomRecipients").val().length < 1 && $("#emailWithaccount").val() <= 0) {
        alert('Please add atleast one recipient to send out email confirmation.');
        return false;
    }
    if (notifyEnabled) {
        $("#emailsection input[type=text]").each(function () {
            var emails = $(this).val().split(';');
            for (var i = 0; i < emails.length; i++) {
                if (emails[i] === null || emails[i].length < 1) {
                    continue;
                }
                if (!IsValidEmail(emails[i])) {
                    allEmailsValid = false;
                    return false;
                }
            }
            if (!allEmailsValid) return false;
        });

        if (!allEmailsValid) {
            alert('Please enter valid email address for recipients.');
            return false;
        }
    }
    if (IsValidForm('.frmOrders')) {
        LoadingPanel.Show();
        var data = $("#frmOrders").serialize();

        var AccountEmailContacts = $("#emailWithaccount").val();
        var CustomRecipients = $("#CustomRecipients").val();
        var CustomCCRecipients = $("#CustomCCRecipients").val();
        var CustomBCCRecipients = $("#CustomBCCRecipients").val();
        var CustomMessage = $("#CustomMessage").val();
        var orderId = $("#orderId").val();
        if (templateid == 8) {
            orderId = customOrderId;
        }
        var InventoryTransactionType = $("#InventoryTransactionType").val();
        data = { AccountEmailContacts: AccountEmailContacts, InventoryTransactionType: templateid, orderId: orderId, CustomRecipients: CustomRecipients, CustomCCRecipients: CustomCCRecipients, CustomBCCRecipients: CustomBCCRecipients, CustomMessage: CustomMessage };
        $.post("/SalesOrders/SaveCustomEmails", data,
            function (result) {

                if (result === "Success") {
                    LoadingPanel.Hide();
                    ModalPopupCustomEmail.Hide();
                    $("#infoMsg").html("Email sent!").show();
                    $('#infoMsg').delay(2000).fadeOut();
                }
                else {
                    LoadingPanel.Hide();
                    ModalPopupCustomEmail.Hide();
                    //alert("Could not send email, please contact support!");
                    alert(result);
                }
            })
            .fail(function (error) {
                alert(error.Message);
            });
    }

}

function SendEmailReport() {
    var allEmailsValid = true;
    var notifyEnabled = true;
    if (notifyEnabled && $("#CustomRecipients").val().length < 1 && $("#emailWithaccount").val() <= 0) {
        alert('Please add atleast one recipient to send out email confirmation.');
        return false;
    }
    if (notifyEnabled) {
        $("#emailsection input[type=text]").each(function () {
            var emails = $(this).val().split(';');
            for (var i = 0; i < emails.length; i++) {
                if (emails[i] === null || emails[i].length < 1) {
                    continue;
                }
                if (!IsValidEmail(emails[i])) {
                    allEmailsValid = false;
                    return false;
                }
            }
            if (!allEmailsValid) return false;
        });

        if (!allEmailsValid) {
            alert('Please enter valid email address for recipients.');
            return false;
        }
    }
    if (IsValidForm('.frmOrders')) {
        var data = $("#frmOrders").serialize();
        var AccountEmailContacts = $("#emailWithaccount").val();
        var CustomRecipients = $("#CustomRecipients").val();
        var CustomCCRecipients = $("#CustomCCRecipients").val();
        var CustomBCCRecipients = $("#CustomBCCRecipients").val();
        var CustomMessage = $("#CustomMessage").val();
        var AccountIds = $("#accountId").val();
        var StartDates = $("#StartDate").val();
        var EndDates = $("#EndDate").val();
        var ordersId = $("#OrderId").val();
        data = { AccountEmailContacts: AccountEmailContacts, CustomRecipients: CustomRecipients, CustomCCRecipients: CustomCCRecipients, CustomBCCRecipients: CustomBCCRecipients, CustomMessage: CustomMessage, AccountId: AccountIds, StartDate: StartDates, EndDate: EndDates, orderId: ordersId };
        $.post("/SalesOrders/SendEmailReport", data,
            function (result) {

                if (result === "Success") {

                    ModalPopupCustomEmail.Hide();
                    $("#infoMsg").html("Email sent!").show();
                    $('#infoMsg').delay(2000).fadeOut();
                }
                else {
                    ModalPopupCustomEmail.Hide();
                    //alert("Could not send email, please contact support!");
                    alert(result);
                }
            })
            .fail(function (error) {
                alert(error.Message);
            });
    }


}

function MarkedOrderProcessAsDispatch(OrderProcessID) {

    data = { OrderProcessId: OrderProcessID };
    if (confirm("Are you sure to mark this delivery disaptched?")) {
        $.post("/Pallets/MarkedOrderProcessAsDispatch",
            data,
            function (result) {

                if (result) {
                    LoadingPanel.Hide();
                    consignmentgridview.Refresh();
                    $("#infoMsg").html("Orders Dispatched!").show();
                    $('#infoMsg').delay(2000).fadeOut();
                }

            })
            .fail(function (error) {
                alert(error.Message);
            });
    }


}


function EditDeliveryAddress(OrderProcessID, status) {


    orderProcessId = OrderProcessID;
    editdelivery = status;
    ModelEditDelivery.Show();



}
function BeginEditDeliveryCallBack(s, e) {
    e.customArgs["ProcessId"] = orderProcessId;
    e.customArgs["editdelivery"] = editdelivery;
}

function UpdateDeliveryAddress() {
    LoadingPanel.Show();
    var CreatedDate;
    var deliveryNo;
    var data = $("#frmOrders").serialize();


    var ShipmentAddressLine1 = $("#ShipmentAddressLine1").val();
    var ShipmentAddressLine2 = $("#ShipmentAddressLine2").val();
    var ShipmentAddressLine3 = $("#ShipmentAddressLine3").val();
    var ShipmentAddressLine4 = $("#ShipmentAddressLine4").val();
    var ShipmentAddressPostcode = $("#ShipmentAddressPostcode").val();
    var OrderProcessId = $("#OrderProcessID").val();

    if (editdelivery == 'true') {
        CreatedDate = EditDateCreated.GetText();
        deliveryNo = $("#DeliveryNO").val();
    }

    data = { ShipmentAddressLine1: ShipmentAddressLine1, ShipmentAddressLine2: ShipmentAddressLine2, ShipmentAddressLine3: ShipmentAddressLine3, ShipmentAddressLine4: ShipmentAddressLine4, ShipmentAddressPostcode: ShipmentAddressPostcode, OrderProcessId: OrderProcessId, CreatedDate: CreatedDate, DeliveryNo: deliveryNo };
    $.post("/SalesOrders/UpdateDeliveryAddress",
        data,
        function (result) {

            if (result) {
                LoadingPanel.Hide();
                ModelEditDelivery.Hide();
                consignmentgridview.Refresh();
                $("#infoMsg").html("Delivery addresses updated!").show();
                $('#infoMsg').delay(2000).fadeOut();
            }
            else {
                LoadingPanel.Hide();
                ModelEditDelivery.Hide();
                consignmentgridview.Refresh();
                $("#infoMsg").html("Could not update addresses, please contact support!").show();
                $('#infoMsg').delay(2000).fadeOut();
            }
        })
        .fail(function (error) {
            alert(error.Message);
        });



}

function CreatePalletTracking() {
    var ProductId = prdid.GetValue();

    var NoOfPallet = NoOfPallets.GetValue();
    var TotalCase = TotalCases.GetValue();
    if (ProductId == "" || ProductId == null || ProductId == undefined) {
        alert("Please select product");
    }
    else if (NoOfPallet == "" || NoOfPallet == null || NoOfPallet == undefined || NoOfPallet == 0) {
        alert("Number of pallets should be greater than 1");
    }
    else if (TotalCase == "" || TotalCase == null || TotalCase == undefined || TotalCase == 0) {
        alert("Cases should be greater than 1");
    }
    else {
        ProductId = prdid.GetValue();
        var ExpiryDates = ExpiryDate.GetText();

        var BatchNos = $("#BatchNo").val();
        var comment = $("#Comments").val();
        var data = { ProductId: ProductId, ExpiryDate: ExpiryDates, TotalCase: TotalCase, BatchNo: BatchNos, Comments: comment, NoOfPallets: NoOfPallet };
        LoadingPanel.Show();
        $.post("/PalletTracking/Create", data,
            function (result) {

                if (result !== false) {
                    LoadingPanel.Hide();

                    if (result !== "") {
                        $("#PalletTrackingIds").val(result);
                        $(".print-label").show();
                    }
                    $("#infoMsg").html("Pallet Tracking serial created!").show();
                    $('#infoMsg').delay(2000).fadeOut();


                }
                else {
                    LoadingPanel.Hide();
                    alert("Could not create pallet serial, please contact support!");

                    alert(result);
                }
            })
            .fail(function (error) {
                alert(error.Message);
            });

    }
}






