$(document).ready(function () {
    BlindShipmentAccount();
});
var Ids;
function beginBSCallBack(s, e) {
    var pid = prdid.GetValue();
    if ($("#Edit").val() === "true") {
        pid = ProductIdOnEdit;
        e.customArgs["key"] = Ids;
        e.customArgs["Edit"] = true;

    }
    e.customArgs["pid"] = pid;


}

function BSsubmitProduct() {

    var delivery = $('#DeliveryNo').val();
    var product = prdid.GetValue();
    var account = $("#AccountID option:selected").val();
    var quantity = Quantity.GetNumber();
    if (quantity == undefined || quantity == null) {
        return alert("Please Add Quantity First!");
    }
    var ID = $(".proddesc").val();
    var FSC = $(".prodFSC").val();
    var PEFC = $(".prodPEFC").val();
    var FscPercent = $(".prodFscPercent").val();
    var ProductGroupId = $("#drpPG :selected").val();
    var ProductCategoryId = $("#drpPC :selected").val();
    LoadingPanel.Show();
    var value = $('#delivery').val();
    var type = 13;
    var Key = $("#KeyId").val();
    var data = { ProductId: product, IsSerial: 'False', SKU: prdid.GetText(), LocationId: $("#LocationId option:selected").val(), LocationName: $("#WarehouseId option:selected").text(), AccountId: account, Delivery: delivery, Quantity: quantity, ProductDesc: ID, FSC: FSC, PEFC: PEFC, FscPercent: FscPercent, Id: Key, ProductGroupId: ProductGroupId };
    $.post("/PurchaseOrders/_SubmitProduct", data, function (result) {
        $("#Edit").val(false);
        LoadingPanel.Hide();
        ModelBS.Hide();
        prdid.PerformCallback();
        BlindShipment.Refresh();

    })
        .fail(function (error) {
            alert(error.Message);
            LoadingPanel.Hide();
        });
}
function BSVerifySerial(e, cnt) {
    var type = parseInt($('#type').val(), 10);
    if (serialArray)
        data = { serial: $(cnt).val(), pid: prdid.GetValue(), orderid: $('#order').val(), del: $('#delivery').val(), type: type };
    if (e.keyCode == 13) {
        if ($.inArray($(cnt).val(), serialArray) >= 0) {
            $(cnt).css("border-color", "red");
            alert("Serial number already exists");
            return;
        }
        if (type !== 2) {
            var value = $(cnt).val();
            if (value.length <= 0) return;

            for (var ctr = 0; ctr < serialArray.length; ctr++) {
                var item = serialArray[ctr].serial;
                if (item == value) {
                    $(cnt).css("border-color", "red");
                    alert("serial number already exists");
                    return;
                }
            }

            var dto = { Serial: $(cnt).val(), IsSerial: 'True', LocationId: $("#Locations option:selected").val(), LocationName: $("#Locations option:selected").text(), ProductId: prdid.GetValue(), SKU: prdid.GetText(), Quantity: 1 };
            // Cancel the default action on keypress event
            $('#serialWrapper').append("<div class='form-group'> <label class='control-label col-md-2'>Add Serial Number</label><div class='col-md-10'> <input class='serial' onkeypress='BSVerifySerial(event,this)' type='text'/><a href='#' onclick='BSRemoveSerial(event,this)'>Remove</a></div></div>");
            $($("#serialWrapper").find("input:last")).focus();
            serialArray.push(dto);
            e.preventDefault();
        }
        else {
            value = $(cnt).val();
            LoadingPanel.Show();
            e.preventDefault();
            $.ajax(
                {
                    type: "GET",
                    url: "/Order/_VerifySerial/",
                    data: data,
                    dataType: 'json',
                    success: function (data) {
                        LoadingPanel.Hide();
                        $(cnt).val("");
                        $(cnt).focus();
                        if (data === true) {
                            serialRange = GetSerialInputForProduct('', value);
                            $('#serialWrapper').append(serialRange);
                            serialArray.push(value);
                        }
                        else {
                            if (type === 12) {
                                if (data.errorcode === 1) {
                                    alert("Item already in stock, cannot be returned!");
                                }
                                else if (data.errorcode === 2) {
                                    if (confirm("Item was not dispatched against selected order, Are you still want to continue?")) {
                                        serialArray.push(value);
                                    }
                                }
                                else {
                                    $(cnt).css("border-color", "red");
                                    alert("Serial number is not available to process");
                                }
                            }
                            else {
                                $(cnt).css("border-color", "red");
                                alert("Serial number is not available to process");
                            }
                        }
                    }
                });


        }
    }
}

function BSRemoveSerial(e, c) {

    if (serialArray.length > 0) {
        var val = $(c).closest('.form-group').find('input').val();
        for (var ctr = 0; ctr < serialArray.length; ctr++) {
            var item = serialArray[ctr].serial;
            if (item == val)
                serialArray.splice(ctr, 1);
            $(c).closest('.form-group').remove();
            $($("#serialWrapper").find("input:last")).focus();
        }
    }
}

function BScollectSerial() {
    var data = [];
    if (serialArray.length > 0) {
        var ProductId = prdid.GetValue();
        LoadingPanel.Show();
        for (i = 0; i < serialArray.length; i++) {
            data.push({
                "serial": serialArray[i],
                "ProductId": ProductId
            });
        }

        var datalist = { products: data };

        $.post("/PurchaseOrders/_SubmitSerial", datalist, function (result) {
            var info = "Item submitted!";
            LoadingPanel.Hide();
            ModelBS.Hide();
            BlindShipment.Refresh();
            prdid.PerformCallback();
        });
    }
}

var GetSerialInputForProduct = function (addHtml, tValue) {

    var serialTextBox = "<input placeholder='Scan or enter serial number and press enter' class='serial pull-left col-md-10 p-0' onkeypress='VerifySerial(event,this)' type='text' value='" + (tValue !== null ? tValue : '') + "' disabled />";
    var removeButton = "<a href='#' class='pull-left col-md-2' onclick='RemoveSerials(event,this)'>Remove</a>";
    return $("<div class='form-group col-md-12 p-0 serial-range-init pull-left'>" + serialTextBox + removeButton + addHtml + "</div>");
};

function BSConfirm() {
    var type = $("#type").val();
    var FSC = $(".prodFSC").val();
    var PEFC = $(".prodPEFC").val();
    if ($('#ShipmentAddressLine1').val() === "") {
        alert("Please enter Address Line 1");
        formmodified = 1;
        return;
    }

    var accountShipmentInfo = {
        ShipmentAddressLine1: $('#ShipmentAddressLine1').val(),
        ShipmentAddressLine2: $('#ShipmentAddressLine2').val(),
        ShipmentAddressLine3: $('#ShipmentAddressLine3').val(),
        ShipmentAddressTown: $('#ShipmentAddressTown').val(),
        ShipmentAddressPostcode: $('#ShipmentAddressPostcode').val(),
        FSC: FSC,
        PEFC: PEFC

    };
    LoadingPanel.Show();
    if (BlindShipment.cpRowCount <= 0) {
        alert("Please add some products!");
        LoadingPanel.Hide();

    }
    else {
        data = {
            account: $("#AccountID option:selected").val(), delivery: $('#DeliveryNo').val(), type: type, accountShipmentInfo: accountShipmentInfo
        };
        var url = "/PurchaseOrders/_ConfirmBS/";
        $.post(url, data, function (result) {

            LoadingPanel.Hide();
            $("#infoMsg").html("Shipment Processed!").show();
            $('#infoMsg').delay(2000).fadeOut();
            formmodified = 0;
            if (type === "2") {
                window.location.href = '/SalesOrders/Consignments/';
            }
            else {
                window.location.href = '/PurchaseOrders/Deliveries/';
            }

        });
    }
}

function removeBSItem(id) {
    if (confirm("Are you sure to delete?")) {
        LoadingPanel.Show();

        $.ajax({
            type: "GET",
            url: "/PurchaseOrders/_RemoveItem/",
            data: { id: id },
            success: function (data) {
                LoadingPanel.Hide();
                BlindShipment.Refresh();
                prdid.PerformCallback();

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                LoadingPanel.Hide();
                alert('Error' + textStatus + "/" + errorThrown);

            }

        });
    }
}
function editBsItem(id) {

    LoadingPanel.Show();

    $.ajax({
        type: "GET",
        url: "/PurchaseOrders/_EditItem/",
        data: { id: id },
        success: function (data) {

            LoadingPanel.Hide();
            ProductIdOnEdit = data.ProductId;
            $("#Edit").val(true);
            Ids = id;
            ModelBS.Show();




        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            // alert(xhr.status);
            LoadingPanel.Hide();
            alert('Error' + textStatus + "/" + errorThrown);

        }

    });

}

function BlindShipmentAccount() {

    var blindshipment = $("#blindshipment").val();
    if (blindshipment === 'True') {
        var address = $("#ShipmentAccountAddressId option:selected").text();
        if (address !== null && address !== 'Select') {
            var addressParts = address.split(';');
            if (addressParts.length < 1) return;
            $("#ShipmentAddressName").val(addressParts[0] === undefined ? '' : addressParts[0].trim());
            $("#ShipmentAddressLine1").val(addressParts[1] === undefined ? '' : addressParts[1].trim());
            $("#ShipmentAddressLine2").val(addressParts[2] === undefined ? '' : addressParts[2].trim());
            $("#ShipmentAddressLine3").val(addressParts[3] === undefined ? '' : addressParts[3].trim());
            $("#ShipmentAddressTown").val(addressParts[4] === undefined ? '' : addressParts[4].trim());
            $("#ShipmentAddressPostcode").val(addressParts[5] === undefined ? '' : addressParts[5].trim());
        }
    }
}

function CreateProduct() {

    var delivery = $('#DeliveryNo').val();
    var product = prdid.GetValue();
    var account = $("#AccountID option:selected").val();
    var quantity = $("#QuantityNew").val();
    var price = $("#PriceShipment").val();
    var ProductGroupId = $("#drpPG :selected").val();
    var ProductCategoryId = $("#drpPC : selected").val();
    var ProductDepartmentId = $("#drpPD :selected").val();
    if (price == "" || price == null || price == undefined) {
        price = 0;
    }
    LoadingPanel.Show();
    var value = $('#delivery').val();
    var type = 13;
    var data = {
        ProductId: 0,
        IsNewProduct: $("#IsNewProduct").prop("checked"),
        ProductName: $("#ProductName").val(),
        IsSerial: 'False',
        TaxId: $("#TaxIds :selected").val(),
        LocationId: $("#LocationId option:selected").val(),
        LocationName: $("#WarehouseId option:selected").text(),
        AccountId: account,
        Delivery: delivery,
        Quantity: quantity,
        Price: price,
        ProductGroupId: ProductGroupId,
        ProductCategoryId: ProductCategoryId,
        ProductDepartmentId: ProductDepartmentId,
        ProductDesc: $("#ProductDesc").val()
    };
    data = { product: data };
    $.post("/PurchaseOrders/CreateProduct", data, function (result) {

        LoadingPanel.Hide();
        ModelBS.Hide();
        BlindShipment.Refresh();
        prdid.PerformCallback();
        $("#divNewProductAddForm input").val('');

        if (result !== "") {
            alert(result);
            $("#ProductName").focus();
        }
        else {
            $("#IsNewProduct").prop("checked", false),
                $('#divNewProductAddForm').hide();
            prdid.SetFocus();
        }

    })
        .fail(function (error) {
            alert(error.Message);
            LoadingPanel.Hide();
        });

}
function BScollectPalletSerial() {
    var serials = [];
    if ($(".tableSerial > tbody > tr").length <= 0) {
        var res = alert("Please scaned pallet First!");
        return;
    }
    $(".quantity").each(function () {
        var quantity = $(this).text();
        var serial = $(this).parent().children('td.pallettracking').html();
        var dto = { Serial: serial, IsSerial: 'False', LocationId: $("#Locations option:selected").val(), LocationName: $("#Locations option:selected").text(), ProductId: prdid.GetValue(), SKU: prdid.GetValue(), Quantity: $(this).html() };
        serials.push(dto);
    });
    if (serials.length > 0) {
        LoadingPanel.Show();
        data = { products: serials };

        $.post("/PurchaseOrders/_SubmitSerial", data, function (result) {
            var info = "Item submitted!";
            LoadingPanel.Hide();
            serialArray = [];
            ModelBS.Hide();
            BlindShipment.Refresh();
            prdid.PerformCallback();
        });
    }
}


function RefreshDepartment(e) {
    var data = e.currentTarget.dataset.target;
    data = { value: data };
    $.post("/PurchaseOrders/RefreshProductGroupAndDepartment", data, function (result) {
        // Do something with the response `res`
        if (result !== "") {
            if (data.value === "Group") {
                $('.drpPG').empty();
                $('#drpPG').empty();
                $.each(result, function (i, item) {
                    $('.drpPG').append($('<option></option>').val(item.Value).html(item.Text));
                    $('#drpPG').append($('<option></option>').val(item.Value).html(item.Text));
                });
                $(".drpPG").trigger("chosen:updated");
                $("#drpPG").trigger("chosen:updated");
            }
            else {
                $('.drpPD').empty();
                $.each(result, function (i, item) {
                    $('.drpPD').append($('<option></option>').val(item.Value).html(item.Text));
                });

                $(".drpPD").trigger("chosen:updated");
            }
        }
    });

}

function onchangeDepartment() {

    var departmentId = $(".drpPD").val();
    $('#drpPD').val(departmentId);
    $("#drpPD").trigger("chosen:updated");
}
function onchangeGroup() {

    var groupId = $(".drpPG").val();
    $('#drpPG').val(groupId);
    $("#drpPG").trigger("chosen:updated");
}
function onchangeCategory() {

    var categoryId = $(".drpPC").val();
    $('#drpPC').val(categoryId);
    $("#drpPC").trigger("chosen:updated");
}





