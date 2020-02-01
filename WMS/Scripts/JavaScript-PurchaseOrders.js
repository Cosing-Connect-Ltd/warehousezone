function IsValidEmail(email) {
    var regExp = /(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])/i;
    return regExp.test(email);
}
var toggleProductAddForm = function () {
    $("#IsNewProduct").on("change",
        function () {
            if ($("#IsNewProduct").prop('checked')) {
                $("#divNewProductAddForm").slideDown();
                $("#prdid_chosen").slideUp();
                $(".prdlbl").hide();
                $("#divNewProductAddForm #ProductCode").attr("required", "required");
                $("#divNewProductAddForm #ProductName").attr("required", "required");
                $("#UpdateOldProductData").val(false);
            } else {
                $("#divNewProductAddForm").slideUp();
                $("#prdid_chosen").slideDown();
                $(".prdlbl").show();
                $("#UpdateOldProductData").val(true);
                $("#divNewProductAddForm #ProductCode").removeAttr("required");
                $("#divNewProductAddForm #ProductName").removeAttr("required");
            }
        });
}

var i = 0;
$(document).ready(function () {
    updateAccountEmails();
    $('#DirectShip').change(function () {
        var check = $('#DirectShip').is(':checked');
        if (check) {
            $('#directShipdd').show();
        }
        else {
            $('#directShipdd').hide();
        }

    });
 

    function updateAccountEmails() {
        var orderId = $("#OrderID").val();
        var accountId = $("#AccountID option:selected").val();
        if (orderId !== null && orderId !== "" && orderId !== undefined && accountId !== null && accountId !== "" && accountId !== undefined) {
            $.ajax({
                type: "GET",
                url: "/PurchaseOrders/_GetAccountAddress",
                data: { accountId: accountId, orderId: orderId },


                success: function (data) {
                    var j = 0;
                    $.each(data, function (i, item) {
                        if (item.Selected) {
                            $('#emailWithaccount').append('<option selected value=' + item.Value + '>' + item.Text + '</option>');

                        }
                        else {
                            $('#emailWithaccount').append('<option value=' + item.Value + '>' + item.Text + '</option>');
                        }
                        $("#emailWithaccount").trigger("chosen:updated");
                        //if (id > 0 && $("#SendEmailWithAttachment").prop('checked')) {
                        //    $("#emailWithaccount").val(id);
                        //    $("#emailWithaccount").trigger("chosen:updated")
                        //} var id = $("#AccountContactId :selected").val();
                        //else {
                        //    $("#emailWithaccount").val($("#emailWithaccount option:first").val());
                        //    
                        //}

                    });


                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    // alert(xhr.status);
                    LoadingPanel.Hide();
                    alert('Error' + textStatus + "/" + errorThrown);

                }

            });
        }

    }

    var updateAccountContactDetails = function () {

        if ($("#AccountContactId").val() > 0) {
            //updateAccountContactRecipients();
        } else {
            var accountId = $("#AccountID").val();
            if (accountId == null || accountId < 1) {
                return;
            }
            //$.get("/PurchaseOrders/GetAccountRecipient/" + accountId,
            //    function (s, e) {
            //        if (s.length > 0) {
            //            $("#CustomRecipients").val(s + ';');
            //        }
            //    });
        }
    }

    $("#frmPurchaseOrderCreate #AccountID").on("change", function () {
        $("#divEmailNotifications input[type=text]").val("");
        updateAccountContactDetails();
    });
    $(".frmOrders").unbind().on("submit",
        function () {
            var allEmailsValid = true;
            var notifyEnabled = $("#SendEmailWithAttachment").prop('checked');



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










        });


    //var populateShipmentAddress = function (selectedAddress) {
    //    if ($("input[name=ShipmentDestination]:checked").length < 1)
    //        return;

    //    if ($("#radioShipToCustomProperty:checked").length > 0)
    //        return;
    //    $("#divFinalShipmentAddress input[type=text]").val('');
    //    var address = selectedAddress;
    //    var addressLines = address.split(',');

    //    if (addressLines.length < 1) return;

    //    $("#divFinalShipmentAddress").slideDown();
    //    var postCode = addressLines[addressLines.length - 1];
    //    $("#ShipmentAddressPostcode").val(postCode);

    //    $("#ShipmentAddressLine1").val(addressLines[0]);

    //    if (addressLines.length < 3) return;
    //    $("#ShipmentAddressLine2").val(addressLines[1]);
    //    if (addressLines.length < 4) return;
    //    $("#ShipmentAddressLine3").val(addressLines[2]);
    //    if (addressLines.length < 5) return;
    //    $("#ShipmentAddressLine4").val(addressLines[3]);

    //}

    var updateShipmentInfoVisibility = function () {

        if ($("#radioShipToWarehouse").prop('checked')) {
            $("#divDisplayTenantAddresses").css("visibility", "visible").slideDown();
            $("#divDisplayPropertyAddresses").css("visibility", "hidden").slideUp();
            $("#divDisplayAccountAddresses").css("visibility", "hidden").slideUp();
            $("#divDisplaySupplierAddresses").css("visibility", "hidden").slideUp();
            $("#IsCollectionFromCustomerSide").val(false);
        }
        if ($("#radioShipToWorkProperty").prop('checked')) {
            $("#divDisplayTenantAddresses").css("visibility", "hidden").slideUp();
            $("#divDisplayAccountAddresses").css("visibility", "hidden").slideUp();
            $("#divDisplayPropertyAddresses").css("visibility", "visible").slideDown();
            $("#divDisplaySupplierAddresses").css("visibility", "hidden").slideUp();
            $("#IsCollectionFromCustomerSide").val(false);
        }
        if ($("#radioShipToCustomProperty").prop('checked')) {
            $("#divDisplayTenantAddresses").css("visibility", "hidden").slideUp();
            $("#divDisplayPropertyAddresses").css("visibility", "hidden").slideUp();
            $("#divDisplayAccountAddresses").css("visibility", "hidden").slideUp();
            $("#divFinalShipmentAddress").slideDown();
            $("#IsCollectionFromCustomerSide").val(false);
            $("#divDisplaySupplierAddresses").css("visibility", "hidden").slideUp();
        }
        if ($("#radioShipToCollection").prop('checked'))
        {
            $("#divDisplayTenantAddresses").css("visibility", "hidden").slideUp();
            $("#divDisplayPropertyAddresses").css("visibility", "hidden").slideUp();
            $("#divDisplayAccountAddresses").css("visibility", "hidden").slideUp();
            $("#divFinalShipmentAddress").slideDown();
            $("#divDisplaySupplierAddresses").css("visibility", "visible").slideDown();

        }

        if ($("#radioShipToAccountAddress").prop('checked')) {
            $("#divDisplayTenantAddresses").css("visibility", "hidden").slideUp();
            $("#divDisplayPropertyAddresses").css("visibility", "hidden").slideUp();
            $("#divDisplayAccountAddresses").css("visibility", "visible").slideDown();
            $("#IsCollectionFromCustomerSide").val(false);
            $("#divDisplaySupplierAddresses").css("visibility", "hidden").slideUp();
        }


        if ($("#radioShipToWarehouse:checked").length > 0) {
            populateShipmentAddress($("#TenantAddressID").find("option:selected").text());
        }
        if ($("#radioShipToWorkProperty:checked").length > 0) {
            populateShipmentAddress($("#PPropertyId").find("option:selected").text());
        }
        if ($("#radioShipToAccountAddress:checked").length > 0) {
            populateShipmentAddress($("#AccountAddressId").find("option:selected").text());
        }
        if ($("#radioShipToCollection:checked").length > 0) {
            populateShipmentAddress($("#ShipmentAccountAddressId").find("option:selected").text());
        }



    };


    $("#radioShipToWarehouse,#radioShipToWorkProperty,#radioShipToCustomProperty,#radioShipToAccountAddress,#radioShipToCollection").on("click", function () {

        updateShipmentInfoVisibility();
    });

    $("#radioShipToWarehouse").on("click", function () {


        populateShipmentAddress($("#TenantAddressID").find("option:selected").text());
    });
    $("#radioShipToAccountAddress").on("click", function () {
        populateShipmentAddress($("#AccountAddressId").find("option:selected").text());

    });
    $("#radioShipToWorkProperty").on("click", function () {
        populateShipmentAddress($("#PPropertyId").find("option:selected").text());
    });
    $("#radioShipToCustomProperty").on("click", function () {
        $("#divFinalShipmentAddress input[type=text]").val('');
    });
    $("#radioShipToCollection").on("click", function () {
        
        $("#divFinalShipmentAddress input[type=text]").val('');
        $("#ShipmentAddressPostcode").val("");
        populateShipmentAddress($("#ShipmentAccountAddressId").find("option:selected").text());
        //$("#ShipmentAddressLine1").val("Collect from Supplier Premises");
        //$("#ShipmentAddressLine2").val("");
        //$("#ShipmentAddressLine3").val("");
        //$("#ShipmentAddressLine4").val("");
        $("#IsCollectionFromCustomerSide").val(true);

    });

    $("#TenantAddressID_chosen").on("click",
        function () {
            populateShipmentAddress($("#TenantAddressID").find("option:selected").text());
        });


    $("#TenantAddressID").on("change",
        function () {
            populateShipmentAddress($(this).find("option:selected").text());
        });
    $("#AccountAddressId").on("change",
        function () {
            populateShipmentAddress($("#AccountAddressId").find("option:selected").text());
        });

    $("#PPropertyId").on("change",
        function () {
            populateShipmentAddress($(this).find("option:selected").text());
        });
    $("#ShipmentAccountAddressId").on("change",
        function () {
            populateShipmentAddress($("#ShipmentAccountAddressId").find("option:selected").text());
        });

    setTimeout(function () {
        if ($("#divEmailNotifications").length > 0) {
            updateShipmentInfoVisibility();
            updateAccountContactDetails();
        }
    }, 200);

    $("div.order-shipmentinfo-panes[style='visibility: hidden']").slideUp();
});

function UpdatePalletDate(id) {
    LoadingPanel.Show();
    $.post({
        url: "/PalletTracking/SyncDate",
        data: { "palletTrackingId": id },
        success: function (e) {

            _PalletTrackingListGridView.Refresh();

            LoadingPanel.Hide();
        }
    });

}

var palletId;
var type;
function SearchSalesOrderPopup(id) {

    palletId = id;
    OrderAuthorizationModel.Show();

}

function EndOrderAuthCallBack() {
    var callback = function (result) {

        if (result == 4) {
            $(".hold-pallet").hide();
            $(".unhold-pallet").show()
        }
        type = "";

    }
    var data = {
        "palletTrackingId": palletId
    };
    Gane.Helpers.AjaxPost('/PalletTracking/GetPalletbyPalletId', data, null, callback, false);




}


function togglecomboxBox(e) {

    if (e.target.value === "1" || e.target.value === "3") {
        type = parseInt(e.target.value);
        $(".search-box").hide();

    }
    else { $(".search-box").show(); }

}

function OrderAuthCallBack(s, e) {


}
function addOrderId() {
    var orderId = OrderAuth.GetValue();
    var palletTrackingId = palletId;
    var types = type;
    if ((orderId !== "" && orderId !== null) || (type !== "" && type !== null && type !== undefined)) {
        LoadingPanel.Show();
        $.post({
            url: "/PalletTracking/AddOrderId",
            data: { "OrderId": orderId, "palletTrackingId": palletTrackingId, type: types },
            success: function (e) {
                LoadingPanel.Hide();
                _PalletTrackingListGridView.Refresh();
                OrderAuthorizationModel.Hide();
            }

        });
    }
    else {
        alert("Please select any action to performed");
    }

}

function GetOrderDetail() {
    var accounId = $(".orderactcnts :selected").val();
    var orderId = $("#DirectShipOrders :selected").val();
    if (accounId == null || accounId == "") {
        alert("Please Select Account First");
        return;
    }
    if (orderId == null || orderId == "") {
        alert("Please Select Order");
        return;
    }
    if (gridViewOrdDet.cpRowCount > 0) {

        if (confirm("Your Current added product will be removed! Do you want to continue? ")) {

            LoadingPanel.Show();
            $.post({
                url: "/PurchaseOrders/_GetOrderDetail",
                data: { "OrderId": orderId, "accounId": accounId, "PageSession": sessionStorage["PageSessionToken"] },
                success: function (e) {
                    gridViewOrdDet.Refresh();
                    // _PalletTrackingListGridView.Refresh();

                    LoadingPanel.Hide();
                }
            });
        }

    }
    else {

        LoadingPanel.Show();
        $.post({
            url: "/PurchaseOrders/_GetOrderDetail",
            data: { "OrderId": orderId, "accounId": accounId, "PageSession": sessionStorage["PageSessionToken"] },
            success: function (e) {
                gridViewOrdDet.Refresh();
                // _PalletTrackingListGridView.Refresh();

                LoadingPanel.Hide();
            }
        });

    }

}

function OnEndCallbackProductKit(s, e) {
    comboBox.SetValue(2);
}


