var tempid = 0;
var total = 0;
var serialArray = [];
var serialsCheck = [];
var checkIsdeleted = false;
var availableProduct = 0.0;
var percentageValue;

//////////////////////////////////==============ORDER DETAIL SPECIFICS==================///////////////////////////
function setValue(val) {
    $.get({
        url: "/OrderDetail/_SetValue",
        data: { "Id": val },
        success: function (e) {
            ModelOrdDetail.Show();
        }
    });
}
function VerifyQunatity() {
    var qty = $("#Qty_I").val();
    var orderdetailId = $("#OrderDetailID").val();
    var Req_url = window.location.href;
    var hash = window.location.hash;
    if (Req_url.indexOf("fragment=TO") !== -1 || Req_url.indexOf("SalesOrders/Edit") !== -1 || Req_url.indexOf("WorksOrders/Edit") !== -1) {
        if (qty !== null && qty !== "" && qty > 0) {
            $.post({
                url: "/OrderDetail/IsQunatityProcessed",
                data: { "Qty": qty, "OrderDetailID": orderdetailId, "isdeleted": checkIsdeleted },
                success: function (e) {
                    if (!e) {
                        $("#Qty_I").val(0);

                        alert("Quantity is already Processed");
                        return;
                    }
                }
            });
        }
    }
}
function updateDetails(id, recqty) {
    $('#selkeyhdPrddet').val(id);
    $('#rec_qty').val(recqty);
    ModelOrdDetail.Show();
}
function deleteDetails(id) {
    if (confirm("Are you sure want to remove product ?")) {
        LoadingPanel.Show();
        $.post({
            url: "/SalesOrders/_deleteProduct",
            data: { "Id": id },
            success: function (e) {
                if ($("#gvODetail").length > 0) {
                    gvODetail.Refresh();
                }
                if ($("#gvWODetail").length > 0) {
                    gvWODetail.Refresh();
                }
                LoadingPanel.Hide();
            }
        });
    }
}
function addDetail(action) {
    var postProductValidationProcess = function (confirmedAck) {
        checkIsdeleted = false;
        VerifyQunatity();
        var productid = prdid.GetValue();

        var warrantyid = $("#WarrantyID").val();
        //$("#prdid :selected").val();
        var Qty = $("#Qty_I").val();
        var price = $("#Price_I").val();
        var type = $("#inventoryType :selected").val();
        if (productid === null || productid === "") {
            if ($("#IsNewProduct").prop("checked") !== true) {
                return alert("Please select product");
            }
        }

        if (Qty === null || Qty === "" || Qty <= 0) { return alert("Please enter quantity"); }
        if (price === null || price === "") { return alert("Please enter price"); }
        if (type === "SalesOrder") {
            if (availableProduct < Qty && confirmedAck) {
                alert("Quantity added is more than available in stock");
            }
        }

        $("#vldOrdDet").removeClass("validation-summary-errors");
        $("#vldOrdDet").addClass("validation-summary-valid");
        if (IsValidForm('#frmorddetails')) {
            var data = $("#frmorddetails").serializeArray();
            if (action == "add") {
                tempid--;
                data[0].value = tempid;
            }

            var casesquantity = $("#CaseQuantity").val();
            if (casesquantity === "" || casesquantity === null || casesquantity === undefined) {
                casesquantity = 1;
            }
            data.push({ name: "casesquantity", value: casesquantity });
            data.push({ name: "ThresholdAcknowledged", value: confirmedAck == true });
            data.push({ name: "AccountId", value: $("#AccountID").val() });
            data.push({ name: "Qty", value: Qty });
            data.push({ name: "WarrantyID", value: $("#WarrantyID").val() });

            data.push({ name: "ProductId", value: productid });
            data.push({ name: "PageSessionToken", value: sessionStorage["PageSessionToken"] });
            LoadingPanel.Show();
            $.post("/OrderDetail/_SaveDetail",
                data,
                function (result) {
                    var callBack = function () {
                        if (window.location.href.indexOf("ProcessOrder") >= 0) {
                            if ($("#gvODetail").length > 0) {
                                gvODetail.Refresh();
                            }
                            if ($("#gvWODetail").length > 0) {
                                gvWODetail.Refresh();
                            }
                        }

                        if ($("#gridViewOrdDet").length > 0) {
                            gridViewOrdDet.Refresh();
                        }
                        if ($("#gridViewOrdDetEdit").length > 0) {
                            gridViewOrdDetEdit.Refresh();
                        }

                        ModelOrdDetail.Hide();
                        LoadingPanel.Hide();
                        if ($("#IsNewProduct").length > 0) {
                            $("#IsNewProduct").prop("checked", false);
                            toggleProductAddForm();
                            $("#prdid").val(result.ProductId);
                            $("#prdid").trigger("chosen:updated");
                            $("#prdid_chosen").slideDown();
                        }
                    };

                    if (!confirmedAck) {
                        if (result.Threshold != null && !result.Threshold.Success && !result.Threshold.CanProceed) {
                            $("#vldOrdDet").addClass("validation-summary-errors");
                            $("#vldOrdDet").html(result.Threshold.FailureMessage);
                            LoadingPanel.Hide();
                            return;
                        }

                        if (result.Threshold != null &&
                            !result.Threshold.Success &&
                            result.Threshold.CanProceed &&
                            result.Threshold.ShowWarning) {
                            return postProductValidationProcess(true);
                        }
                        else {
                            callBack();
                        }
                    }
                    else {
                        callBack();
                    }
                });
        }
    };

    var duplicateProduct = null;

    if ($("#IsNewProduct").prop("checked")) {
        $.get("/purchaseorders/CheckProductExist/" + $("#divNewProductAddForm #ProductName").val(),
            function (data) {
                if (data == true) {
                    $("#divNewProductErrorForm").html(
                        "<span class='validation-summary-errors'>Product with same code already exists.</span>");
                    duplicateProduct = true;
                } else {
                    postProductValidationProcess();
                }
            });
        return;
    } else {
        postProductValidationProcess();
    }
}
function removeDetail(id) {
    if (confirm("Are you sure want to remove product ?")) {
        $.post({
            url: "/OrderDetail/_RemoveProduct",
            data: { "Id": id, "pageSessionToken": sessionStorage["PageSessionToken"] },
            success: function (e) {
                if (e !== "" && e === false) {
                    alert("Quantity is Processed, Not able to delete this product");

                    return;
                }
                if ($("#gridViewOrdDetEdit").length > 0) {
                    gridViewOrdDetEdit.Refresh();
                }
                if ($("#gridViewOrdDet").length > 0) {
                    gridViewOrdDet.Refresh();
                }
            }
        });
    }
}

function ClearPageSessionData() {
    $.get("/Purchaseorders/ClearPageSessionData/" + sessionStorage["PageSessionToken"],
        function (data) {
            return true;
        });
}

function UpdateAccountDDForOrderDetails() {
    if ($("#gridViewOrdDet").length > 0) {
        if (gridViewOrdDet.cpRowCount > 0) {
            DisableChosenDropdown('AccountID', true);
            //$("#AccountID").prop("disabled", true);
            //$("<input type='hidden' id='AccountID' name='AccountID' value='" + $("#AccountID").val() + "'/>").insertAfter($("#AccountID"));
        } else {
            DisableChosenDropdown('AccountID', false);
        }
    }
    RowsCount();
}

function DisableChosenDropdown(dropdownId, disable) {
    if (disable) {
        $("#" + dropdownId).prop("disabled", true);
        if ($("input[type=hidden][id=" + dropdownId + "]").length < 1) {
            $("<input type='hidden' id='" + dropdownId + "' name='" + dropdownId + "' value='" + $("#" + dropdownId).val() + "'/>").insertAfter($("#" + dropdownId));
        } else {
            $("input[name=AccountID]").val($("select[name=AccountID]").val());
        }
    } else {
        $("#" + dropdownId).removeAttr("disabled");
        $("input[type=hidden][id=" + dropdownId + "]").remove();
    }
    $("#" + dropdownId).trigger("chosen:updated");
}

function loadProdAccounts() {
    var acid = $("#AccountID option:selected").val();
    var pid = prdid.GetValue();
    //$("#prdid option:selected").val();
    $('#drpPrdAccount').empty();
    if (acid == null || acid == 0 || pid == null || pid == 0 || isNaN(pid)) return false;

    $.ajax({
        type: "GET",
        url: "/OrderDetail/_GetProductAccounts/",
        data: { accountid: acid, productid: pid },
        dataType: 'json',

        success: function (data) {
            $('#drpPrdAccount').empty();

            if (data.length > 0) {
                $.each(data, function (i, item) {
                    $('#drpPrdAccount').append($('<option></option>').val(item.ProdAccCodeID).html(item.ProdAccCode));
                });
            }

            $("#drpPrdAccount").trigger("chosen:updated");
            GetOrderingNotes();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });
}

function GetOrderingNotes() {
    var productAccountCodeId = $("#drpPrdAccount").val();
    if (productAccountCodeId > 0 && productAccountCodeId !== undefined && productAccountCodeId !== null && productAccountCodeId !== "") {
        $.ajax({
            url: "/Products/_GetOrderingNotes",
            data: { ProdAccId: productAccountCodeId },
            success: function (result) {
                if (result === "") {
                    $('#OrderingNotes').text(result);
                    $('.ordering-notes').hide();
                }
                else {
                    $('.ordering-notes').show();
                    $('#OrderingNotes').text(result);
                }
            }
        });
    }
}
$("#drpPrdAccount").change(function () {
    GetOrderingNotes();
});

function beginCallbackWithPageSessionToken(s, e) {
    e.customArgs["pageSessionToken"] = sessionStorage["PageSessionToken"];
}

function beginorderDetCallBack(s, e) {
    formmodified = 1;

    e.customArgs["did"] = $('#selkeyhdPrddet').val();

    e.customArgs["rec_qty"] = $('#rec_qty').val();

    var orderId = $('#OrderID').val(); if (location.href.indexOf('BulkOrder') > 0) {
        if (!ModelWorksOrderSingleEdit.GetVisible()) {
            orderId = null;
        }
    }
    e.customArgs['OrderID'] = orderId;
    //e.customArgs['pageSessionToken'] = sessionStorage['PageSessionToken'];
    e.customArgs['IsTransferAdd'] = $("#IsTransferAdd").val();
    var dte;
    e.customArgs["Id"] = $('#frmProcessOrder #OrderID').val();
    e.customArgs["oid"] = $('#frmProcessOrder #OrderID').val();
    e.customArgs["AccountId"] = $('#AccountID').val();

    e.customArgs["ProductId"] = $('#prdid').val();
    e.customArgs["pageSessionToken"] = sessionStorage["PageSessionToken"];
    var Req_url = window.location.href;
    if (Req_url.indexOf("SalesOrders/Create") >= 0 || Req_url.indexOf("SalesOrders/Edit") >= 0) {
        dte = ExpectedDate.GetDate();
    }
    else if (Req_url.indexOf("ProcessOrder") >= 0) {
        e.customArgs["did"] = $('#selkeyhdPrddet').val();
        e.customArgs["oid"] = $('#frmProcessOrder #OrderID').val();
    }
    else if (Req_url.indexOf("CreateBulk") >= 0) {
        e.customArgs["oid"] = $('#frmWorksSingleOrderFormEdit #OrderID').val();
    }
    e.customArgs["exp_date"] = dte == null ? null : dte.toDateString();
}

var productpercase = 0;
var pricealert;
function endOrderDetailCallback() {
    var callback = function (result) {
        var prices = result.Price;
        percentageValue = result.percentageMargin;
        var perMargin = result.PercentMargin;
        var enableWarranty = result.EnableWarranty;
        pricealert = result.ShowPriceAlert;
        if (enableWarranty === true) {
            $(".WarrantyDetail").show();
        }
        else {
            $(".WarrantyDetail").hide();
        }
        if (prices !== null && typeof Price !== 'undefined') {
            if (prices <= 0) {
                Price.SetValue("");
                $(".financialAnalysisMainDiv").hide();
            }
            else if (result.BuyPrice > prices && !result.purchaseOrder && pricealert) {
                alert("Selling price is less than buying price and should not be less than : " + (perMargin));
            }
            else if (perMargin > prices && !result.purchaseOrder && pricealert) {
                alert("Selling price should not be less than " + (perMargin));
            }
            if (prices > 0) {
                $("#BuyPrice").val(perMargin);
                Price.SetValue(prices);
                var inventoryTypes = $('#inventoryType').val();
                if (inventoryTypes != "PurchaseOrder") {
                    // price analysis
                    PriceAnalysis();
                }
            }
        }
        if (prices !== null && typeof Price !== 'undefined' && !result.AllowModifyPrice && (result.AllowZeroSale == null || !result.AllowZeroSale) && result.AllowModifyPrice != undefined) {
            Price.GetInputElement().readOnly = true;
        }

        $("#frmorddetails #ProductsPerCase").val(result.ProductsPerCase);
        if (result.ProductsPerCase > 0) {
            $("#ProcessBy[value=Case]").closest("label").find("span")
                .text("By Case (" + result.ProductsPerCase + " items)");
        } else {
            $("#ProcessBy[value=Case]").closest("label").find("span").text("By Case");
        }

        if (result.ProductsPerPallet > 0) {
            $("#ProcessBy[value=Pallet]").closest("label").find("span")
                .text("By Pallet (" + result.ProductsPerPallet + " items)");
        } else {
            $("#ProcessBy[value=Pallet]").closest("label").find("span").text("By Pallet");
        }

        $("#frmorddetails #CasesPerPallet").val(result.CasesPerPallet);
        $("#frmorddetails #ProductsPerPallet").val(result.ProductsPerPallet);

        if (result.ProcessByCase) {
            $("#ProcessBy[value=Case]").prop("checked", true);
            $(".quantityor").hide();
            $(".casequantity").show();
            $("#processcase").val(result.ProductsPerCase);
        }

        if (result.ProcessByPallet) {
            $("#ProcessBy[value=Pallet]").prop("checked", true);
        }

        updateProcessByValue();
        LoadingPanel.Hide();
    };

    if ($("#frmorddetails #ChosenProductID").length < 1) {
        $("#frmorddetails").append("<input type='hidden' id='ChosenProductID' name='ChosenProductID' value='" +
            prdid.GetValue() +
            //$('#prdid').val() +
            "'/>");
    }

    var orderDetailId = parseInt($("#frmorddetails #OrderDetailID").val());

    var hasProductChanged = $("#frmorddetails #ChosenProductID").val() != prdid.GetValue();
    //$('#prdid').val();

    if (orderDetailId == 0 || hasProductChanged) {
        $("#frmorddetails #ChosenProductID").val(prdid.GetValue());
        var data = { AccountId: $('#AccountID').val(), ProductId: prdid.GetValue() };
        LoadingPanel.Show();
        Gane.Helpers.AjaxPost('/orderdetail/_productpricedetail', data, null, callback, false);
    }

    loadProdAccounts();
}

var updateProcessByValue = function () {
    if ($("#ProcessBy[value=Case]").prop("checked")) {
        $("#frmorddetails #ProcessByType").val("Case");
    } else if ($("#ProcessBy[value=Pallet]").prop("checked")) {
        $("#frmorddetails #ProcessByType").val("Pallet");
    } else $("#frmorddetails #ProcessByType").val("Item");
};

$("#frmorddetails input[type=radio]").on("click", function () { updateProcessByValue(); });

function beginPriceHistoryCallBack(s, e) {
    var Req_url = window.location.href;
    var ordertype;
    var acid = $("#AccountID option:selected").val();
    var pid = prdid.GetValue();
    //$("#prdid option:selected").val();
    if (Req_url.indexOf("PurchaseOrders") >= 0) {
        ordertype = 1;
    }
    else if (Req_url.indexOf("SalesOrders") >= 0) {
        ordertype = 2;
    }
    else if (Req_url.indexOf("WorksOrders") >= 0) {
        ordertype = 8;
    }
    else if (Req_url.indexOf("DirectSales") >= 0) {
        ordertype = 15;
    }
    e.customArgs["product"] = pid;
    e.customArgs["ordertype"] = ordertype;
    e.customArgs["account"] = acid;
}
function getPrice() {
    var price = $('input[name="HPrice"]:checked').val();
    ModelPriceHistory.Hide();
    Price.SetNumber(price);
}

function VerifySerial(e, cnt) {
    var Req_url = window.location.href;
    var type;
    var data = {};
    var url;
    var serialRange;
    var qty = parseInt($('#qty').val(), 10);
    type = parseInt($('#type').val(), 10);

    data = { serial: $(cnt).val(), pid: $('#prodId').val(), orderid: $('#order').val(), del: $('#delivery').val(), type: type };

    $(cnt).css("border-color", "lightgray");

    if (e.keyCode === 13) {
        var value = $(cnt).val();
        if (value.length <= 0) return;
        if ($.inArray(value, serialArray) >= 0) {
            $(cnt).css("border-color", "red");
            alert("Serial number already exists");
            return;
        }
        if (type === 1) {
            if (serialArray.length + parseInt($('#rec_qty').val(), 10) >= qty) {
                alert("Items exceeds number of itmes for this product");
            }
        }
        else if (type === 2 || type === 8) {
            if (serialArray.length + parseInt($('#rec_qty').val(), 10) === qty) {
                alert("Items exceeds number of itmes for this product , no more items are allowed to add for this product");
                return;
            }
        }
        else if (type === 3 || type === 4) {
            //for TOs will add checks later.
        }

        // Cancel the default action on keypress event
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
                                    serialRange = GetSerialInputForProduct('');
                                    $('#serialWrapper').append(serialRange);
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

var failedSerials = [];
var ValidateSerialSequence = function (serialNumbers) {
    var type;
    var data = {};
    var serialNumber = '';

    $.each(serialNumbers, function (i, s) {
        if (!s.RequestComplete) {
            serialNumber = s.Serial;
            return;
        }
    });

    if (serialNumber === '') {
        $.each(serialNumbers,
            function (i, s) {
                if (s.Result != true) {
                    failedSerials.push(s.Serial);
                } else {
                    $('#serialWrapper').append(GetSerialInputForProduct('', s.Serial));
                }
            });

        if (failedSerials.length > 0) {
            var htmlMessage = "<h4>Following serials cannot be added. Please verify they exist in stock.</h4>" +
                failedSerials.join('<br/>');
            Gane.Helpers.ShowPopupMessage("Failed Serials", htmlMessage, PopupTypes.Danger);
            failedSerials = [];
        }
        //$('#serialWrapper').append(GetSerialInputForProduct('', ''));

        return true;
    }

    type = $('#type').val();
    data = { serial: serialNumber, pid: $('#prodId').val(), orderid: $('#order').val(), del: $('#delivery').val(), type: type };

    $.ajax({
        type: "GET",
        url: "/Order/_VerifySerial/",
        data: data,
        dataType: 'json',
        success: function (result) {
            $.each(serialNumbers, function (i, s) {
                if (s.Serial == serialNumber) {
                    s.Result = result == true;
                    s.RequestComplete = true;
                    if (result == true && serialArray.indexOf(serialNumber) < 0) {
                        serialArray.push(serialNumber);
                    }
                    return;
                }
            });

            ValidateSerialSequence(serialNumbers);
        }
    });
};

var initialiseSeriesGenerateActions = function (s, e) {
    var toggleSeriesButton = function () {
        if ($(".serial-range:visible").length > 0) {
            $(".serial-range").slideUp();
            $("#btnToggleSeriesSequence").val('Add Serial Sequence');
        } else {
            $(".serial-range").slideDown();
            $("#btnToggleSeriesSequence").val('Cancel Serial Sequence');
        }
    };

    $("#btnToggleSeriesSequence").on("click",
        function () {
            if ($(".serial")[0]) {
                if ($("#serialWrapper .serial:eq(0)").val().length < 1 || !parseInt($("#serialWrapper .serial:eq(0)").val()[$("#serialWrapper .serial:eq(0)").val().length - 1]) > 0) {
                    Gane.Helpers.ShowPopupMessage("Invalid serial", "You must enter initial serial number which ends with a number. e.g. A00001", PopupTypes.Danger);
                    return false;
                }
            }
            //else {
            //    Gane.Helpers.ShowPopupMessage("Invalid serial", "You must enter initial serial number which ends with a number. e.g. A00001", PopupTypes.Danger);
            //}

            toggleSeriesButton();
        });

    $(".serial-range-button").on("click",
        function () {
            $("#serialWrapper .form-group").each(function () { if ($(this).find(".serial").val() === null || $(this).find(".serial").val().length < 1) { $(this).remove(); } });
            var qty = $(".serial-range").find('.serial-range-qty').val();
            var lastSerial = $('.serial-enter-box').val();
            if (confirm("You Require " + qty + " serials?")) {
                var results = [];
                if (lastSerial !== "") {
                    results.push({ Serial: lastSerial, Result: false, RequestComplete: false });
                }

                for (var i = 1; i < qty; i++) {
                    var prefix = "";
                    var suffix = "";
                    var lastSerialLength = "";
                    if (lastSerial !== "") {
                        prefix = Gane.Helpers.GetSeriesObjectForString(lastSerial).SeriesText;
                        suffix = parseInt(Gane.Helpers.GetSeriesObjectForString(lastSerial).SeriesNumber) + i;
                        lastSerialLength = lastSerial.length;
                    }
                    else {
                        var value = serialArray[0];
                        prefix = Gane.Helpers.GetSeriesObjectForString(value).SeriesText;
                        suffix = parseInt(Gane.Helpers.GetSeriesObjectForString(value).SeriesNumber) + i;
                        lastSerialLength = value.length;
                    }

                    var prefixLength = prefix.length;
                    var nextSerial = prefix + suffix;
                    if (nextSerial.length > lastSerialLength) {
                        var charsToRemove = nextSerial.length - lastSerialLength;
                        prefix = prefix.substr(0, prefixLength - charsToRemove);
                        nextSerial = prefix + suffix;
                    }
                    var result = { Serial: nextSerial, Result: false, RequestComplete: false };
                    results.push(result);
                }

                ValidateSerialSequence(results);
            }
            toggleSeriesButton();
            $('.serial-enter-box').val("");
            $('.serial-enter-box').focus();
        });
};

var GetSerialInputForProduct = function (addHtml, tValue) {
    var serialTextBox = "<input placeholder='Scan or enter serial number and press enter' class='serial pull-left col-md-10 p-0' onkeypress='VerifySerial(event,this)' type='text' value='" + (tValue !== null ? tValue : '') + "' disabled />";
    var removeButton = "<a href='#' class='pull-left col-md-2' onclick='RemoveSerials(event,this)'>Remove</a>";
    return $("<div class='form-group col-md-12 p-0 serial-range-init pull-left'>" + serialTextBox + removeButton + addHtml + "</div>");
};

var GetSerialInputForPallete = function (addHtml, tValue) {
    var serialTextBox = "<input placeholder='Scan or enter pallet serial number and press enter' class='serial' onkeypress='VerifyPallete(event,this)' type='text' value='" + (tValue != null ? tValue : '') + "' />";
    var removeButton = "<a href='#' onclick='RemoveSerials(event,this)'>Remove</a>";
    return $("<div class='form-group'><div class='col-md-10  col-md-offset-1'> " + serialTextBox + removeButton + addHtml + "</div></div>");
};

function RemoveSerials(e, c) {
    e.preventDefault();

    var val = $(c).closest('.form-group').find('input').val();

    serialArray.splice($.inArray(val, serialArray), 1);
    $(c).closest('.form-group').remove();
    $($("#serialWrapper").find("input:last")).focus();
}

function PickLocations(id) {
    Gane.Helpers.LoadPickLocationsReceiveProduct(id);
}

function submitProduct() {
    if ($("#divProcessButtons #ProcessBatchNumber").length > 0) {
        var batchNumber = $("#ProcessBatchNumber").val();
        if (batchNumber == null || batchNumber.trim().length < 1) {
            return Gane.Helpers.AddValidationErrorToElement($("#divProcessButtons #ProcessBatchNumber"), '*Batch number is mandatory');
        }
    } if ($("#divProcessButtons #ProcessExpiryDate").length > 0) {
        var expiryDate = ProcessExpiryDate.GetFormattedDate();
        if (expiryDate == null || expiryDate.trim().length < 1) {
            return Gane.Helpers.AddValidationErrorToElement($("#divProcessButtons #ProcessExpiryDate"), '*Expiry date is mandatory');
        }
    }

    var val = Quantity.GetNumber();

    if (val <= 0) {
        return Gane.Helpers.AddValidationErrorToElement($("#frmprocProduct #Quantity"), '*Quantity is mandatory');
    }

    var Req_url = window.location.href;
    var type;
    var qty = parseInt($('#qty').val(), 10);
    var rec_qty = parseInt($('#rec_qty').val(), 10);
    type = $('#type').val();

    if (val + rec_qty > qty && (type == 8 || type == 9 || type == 1)) {
        if (confirm('Items exceeds number of items for this product, Press OK to continue') == false) {
            return;
        }
    }

    else if (val + rec_qty > qty && (type == 2 || type == 10)) {
        alert("Items exceeds number of items for this product , no more items are allowed to add for this product"); return;
    }

    var total = 0;

    if ($("#tblSOProductLocations").length > 0) {
        $("#divLocationsPicker .txt-pick-qty").each(function () {
            var maxAvailable = parseInt($(this).closest("tr").attr("data-qty"));
            var location = $(this).closest("tr").attr("data-id");
            if ($(this).val() != null && $(this).val().length > 0) {
                var qty = parseInt($(this).val());
                if (qty < 0) {
                    alert('Picked quantity from locations cannot have negative values.');
                    return;
                }

                total = total + qty;
            }
        });
        if (total > val) {
            alert('Items cannot exceed number of items for this product in this order. Max Allowed: ' + val);
            return;
        }
    }
    $("#vldRecPrd").removeClass("validation-summary-errors");
    $("#vldRecPrd").addClass("validation-summary-valid");
    if (IsValidForm('#frmprocProduct')) {
        LoadingPanel.Show();
        var value = $('#delivery').val();
        //var data = { model: $("#frmrecProduct").serializeArray() };
        var data = $("#frmprocProduct").serializeArray();

        var processBatchNumber = $("#ProcessBatchNumber").val();
        var processExpiryDate = $("#ProcessExpiryDate").length > 0 ? ProcessExpiryDate.GetFormattedText("dd/MM/yyyy") : null;

        data.push({ name: 'BatchNumber', value: processBatchNumber });
        data.push({ name: 'ExpiryDate', value: processExpiryDate });
        data.push({ name: 'OrderDetailID', value: $('#line_id').val() });

        data.push({ name: 'ShipmentAddressLine1', value: $('#ShipmentAddressLine1').val() });
        data.push({ name: 'ShipmentAddressLine2', value: $('#ShipmentAddressLine2').val() });
        data.push({ name: 'ShipmentAddressLine3', value: $('#ShipmentAddressLine3').val() });
        data.push({ name: 'ShipmentAddressTown', value: $('#ShipmentAddressTown').val() });
        data.push({ name: 'ShipmentAddressPostcode', value: $('#ShipmentAddressPostcode').val() });

        $.post("/Order/_SubmitRecProduct",
            data,
            function (result) {
                ProcessCallbackFromProcessPost(result);
            })
            .fail(function (error) {
                alert(error.Message);
            });
    }
}

function RefreshOrderDetailGrids() {
    if ($("#gvWODetail").length > 0) {
        gvWODetail.Refresh();
    }
    if ($("#gvODetail").length > 0) {
        gvODetail.Refresh();
    }
    if ($("#_WorksOrderCompletedListGridView").length > 0) {
        _WorksOrderCompletedListGridView.Refresh();
    }
    if ($("#_WorksOrderListGridView").length > 0) {
        _WorksOrderListGridView.Refresh();
    }
    if ($('#selkeyWorksOrderListGridViewGridName').length > 0) {
        var gridview = GetDevexControlByName($('#selkeyWorksOrderListGridViewGridName').val());
        if (gridview != null) {
            gridview.Refresh();
        }
    }
}

function ProcessCallbackFromProcessPost(result) {
    var invTypeId = parseInt($("#frmProcessOrder #InventoryTransactionTypeId").val());
    if (invTypeId == 1) {
        gvPODetail.Refresh();
        ModelRecProduct.Hide();
        LoadingPanel.Hide();
    }
    if (invTypeId == 2) {
        gvODetail.Refresh();
        ModelRecProduct.Hide();
        LoadingPanel.Hide();
        return;
    }
    if (invTypeId == 4 || invTypeId == 3) {
        gvTransferOrderDetails.Refresh();
        LoadingPanel.Hide();
        ModelRecProduct.Hide();
        return;
    }

    if (invTypeId == 8) {
        gvWODetail.Refresh();
        LoadingPanel.Hide();
        ModelRecProduct.Hide();
    }

    if (result != null && result.error == true) {
        return Gane.Helpers.AddValidationErrorToElement($("#frmprocProduct #divLocationsPicker"), result.errorMessage);
    }
    if (type == 1)
        gvPODetail.Refresh();
    else if (type == 2 || type == 13) {
        if ($("#gvWODetail").length > 0) {
            gvWODetail.Refresh();
        }
        if ($("#gvODetail").length > 0) {
            gvODetail.Refresh();
        }
    }
    else if (type == 3 || type == 4)
        gvTransferOrderDetails.Refresh();
    else if (type == 8)
        gvWODetail.Refresh();
    ModelRecProduct.Hide();
    LoadingPanel.Hide();
    data = { product: $('#prodId').val() };
}

function CollectSerials() {
    var type;
    var ordid = $('#order').val();
    var prid = $('#prodId').val();
    var url = "/Order/_SubmitSerials";
    type = $('#type').val();

    var delivery = $('#delivery').val();
    var groupToken = $("#groupToken").val();
    var sellable = $(".sellableCondition").prop("checked");
    var DeliveryNo = $('#delivery').val();
    if (sellable === false) {
        type = "17";
        ordid = $('#grorderid').val();
        prid = prdid.GetValue();
    }
    if (type === "12") {
        ordid = $('#grorderid').val();
        prid = prdid.GetValue();
        delivery = $("#deliveryNumber").val();
    }
    if (type === "14") {
        ordid = $('#grorderid').val();
        prid = $("#wrProducts option:selected").val();
        delivery = $("#deliveryNumber").val();
    }

    var shippingInfo = {
        ShipmentAddressLine1: $('#ShipmentAddressLine1').val(),
        ShipmentAddressLine2: $('#ShipmentAddressLine2').val(),
        ShipmentAddressLine3: $('#ShipmentAddressLine3').val(),
        ShipmentAddressTown: $('#ShipmentAddressTown').val(),
        ShipmentAddressPostcode: $('#ShipmentAddressPostcode').val()
    };

    if (serialArray.length > 0) {
        LoadingPanel.Show();

        data = {
            serialList: serialArray, lineid: $('#line_id').val(), product: prid, delivery: delivery, order: ordid, cons_Type: $("#DeliveryMethod option:selected").val(), location: $("#Locations option:selected").val(), type: parseInt(type), shipmentInfo: shippingInfo, groupToken: groupToken
        };
        $.post(url, data, function (result) {
            if (type === "12" || type === "14" || type === "17") {
                ordernumber = result.orderNumber;
                productId = result.productId;
                orderId = result.orderid;
                inType = type;
                groups = result.groupToken;
            }
            var info = "Item submitted!";
            if (type === "1")
                gvPODetail.Refresh();
            else if (type === "2")
                gvODetail.Refresh();
            else if (type === "14")
                WastageGoodsReturn.Refresh();
            else if (type === "3" || type === "4")
                gvTransferOrderDetails.Refresh();
            else if (type === "8")
                gvWODetail.Refresh();
            else if (type === "12" || type === "17") { GoodsReturn.Refresh(); }
            //
            serialArray = [];
            LoadingPanel.Hide();
            ModelAddSerial.Hide();
            ModelGoodsReturn.Hide();
            $("#infoMsg").html(info).show();
            $('#infoMsg').delay(2000).fadeOut();
        });
    }
}

function addSerial(detid, orderid, pid, ser, qty, rec_qty, type) {
    $('#prodId').val(pid);
    $('#ser').val(ser);
    $('#delivery').val($('#DeliveryNumber').val());
    $('#order').val(orderid);
    $('#qty').val(qty);
    $('#type').val(type);
    $('#rec_qty').val(rec_qty);
    $('#line_id').val(detid);

    if (ser === 2) {
        ModelAddSerial.Show();
    }
    else if (ser === 3) {
        ModelAddPallete.Show();
        $(".scaned").val("").focus();
    }
    else {
        ModelRecProduct.Show();
    }
}

function beginAddSeriesCallBack(s, e) {
    e.customArgs["pid"] = $('#prodId').val();
    e.customArgs["ser"] = $('#ser').val();
    e.customArgs["delivery"] = $('#delivery').val();
    e.customArgs["order"] = $('#order').val();
    e.customArgs["qty"] = $('#qty').val();
    e.customArgs["consignmenttype"] = $("#DeliveryMethod option:selected").val();
    e.customArgs["type"] = $('#type').val();
    e.customArgs["line_id"] = $('#line_id').val();
    e.customArgs["rec_qty"] = $('#rec_qty').val();

    e.customArgs["ShipmentAddressLine1"] = $('#ShipmentAddressLine1').val();
    e.customArgs["ShipmentAddressLine2"] = $('#ShipmentAddressLine2').val();
    e.customArgs["ShipmentAddressLine3"] = $('#ShipmentAddressLine3').val();
    e.customArgs["ShipmentAddressTown"] = $('#ShipmentAddressTown').val();
    e.customArgs["ShipmentAddressPostcode"] = $('#ShipmentAddressPostcode').val();

    serialArray = [];
}
function beginAddSeriesCallBackPalletes(s, e) {
    var type = $('#InventoryTransactionTypeId').val();
    var Req_url = window.location.href;
    if (type === "6" || type === "7") {
        e.customArgs["pid"] = $('#ProductId').val();
        e.customArgs["rec_qty"] = 0;
        e.customArgs["type"] = $('#type').val();
        e.customArgs["qty"] = $('#Quantity').val();
    }
    else {
        e.customArgs["pid"] = $('#prodId').val();
        e.customArgs["ser"] = $('#ser').val();
        e.customArgs["delivery"] = $('#delivery').val();
        e.customArgs["order"] = $('#order').val();
        e.customArgs["qty"] = $('#qty').val();
        e.customArgs["consignmenttype"] = $("#DeliveryMethod option:selected").val();
        e.customArgs["type"] = $('#type').val();
        e.customArgs["line_id"] = $('#line_id').val();
        e.customArgs["rec_qty"] = $('#rec_qty').val();
    }

    serialArray = [];
}

function submitProperty() {
    if (IsValidForm('#frmproperty')) {
        LoadingPanel.Show();
        var value = $('#delivery').val();
        //var data = { model: $("#frmrecProduct").serializeArray() };
        var frmdata = $("#frmproperty").serializeArray();
        $.post('/WorksOrders/_PropertySubmit', frmdata, function (result) {
            if (result.error == false) {
                $('#PPropertyId').append('<option selected value=' + result.id + '>' + result.code + '</option>');
                $("#PPropertyId").trigger("chosen:updated");
                LoadingPanel.Hide();
                ModelProperty.Hide();
            }
            else {
                var ul = $("#vldproperty ul");
                $("#vldproperty").addClass("validation-summary-errors");
                $("#vldproperty").removeClass("validation-summary-valid");
                ul.html("");
                ul.append("<li> " + result.errormessage + "</li>");
                LoadingPanel.Hide();
            }
            updatePropertyTenantsList();
        });
    }
}
function submitLandlord() {
    if (IsValidForm('#frmlandlord')) {
        LoadingPanel.Show();

        var frmdata = $("#frmlandlord").serializeArray();

        $.post('/WorksOrders/_LandlordSubmit', frmdata, function (result) {
            if (result.error == false) {
                $('#CurrentLandlordId').append('<option selected value=' + result.id + '>' + result.code + '</option>');
                $("#CurrentLandlordId").trigger("chosen:updated");
                LoadingPanel.Hide();
                ModelLandlord.Hide();
            }
            else {
                var ul = $("#vldproperty ul");
                $("#vldproperty").addClass("validation-summary-errors");
                $("#vldproperty").removeClass("validation-summary-valid");
                ul.html("");
                ul.append("<li> " + result.errormessage + "</li>");
                LoadingPanel.Hide();
            }
        });
    }
}
function submitPTenent() {
    if (IsValidForm('#frmptenent')) {
        LoadingPanel.Show();

        var frmdata = $("#frmptenent").serializeArray();

        $.post('/WorksOrders/_PTenentSubmit', frmdata, function (result) {
            if (result.error == false) {
                $('#CurrentPTenentId').append('<option selected value=' + result.id + '>' + result.code + '</option>');
                $("#CurrentPTenentId").trigger("chosen:updated");
                LoadingPanel.Hide();
                ModelPTenent.Hide();
            }
            else {
                var ul = $("#vldproperty ul");
                $("#vldproperty").addClass("validation-summary-errors");
                $("#vldproperty").removeClass("validation-summary-valid");
                ul.html("");
                ul.append("<li> " + result.errormessage + "</li>");
                LoadingPanel.Hide();
            }
        });
    }
}

function getCurrentUKDate() {
    var day = new Date().getDate() + 1;
    var month = new Date().getMonth() + 1;
    var year = new Date().getFullYear();

    if (day < 10) { day = "0" + day; } else day = day;
    if (month < 10) { month = "0" + month; } else month = month;

    return day + "/" + month + "/" + year;
}

var noteCnt = 0;
function saveNotes() {
    if (IsValidForm('#frmOrderNotes')) {
        LoadingPanel.Show();
        noteCnt--;
        var frmdata = $("#frmOrderNotes").serializeArray();
        var noteText = $("#frmOrderNotes #Notes").val();
        if (noteText === null || noteText.match(/^ *$/) !== null) {
            LoadingPanel.Hide();
            alert("Please enter some text");
            return;
        }

        frmdata[0].value = noteCnt;
        if ($("#ModelWorksOrderSingleEdit_State").length > 0 && ModelWorksOrderSingleEdit.clientVisible) {
            var order = { name: "OrderID", value: $("#frmOrderNotes #OrderID").val() };
            var orderNote = { name: "OrderNoteId", value: $("#frmOrderNotes #OrderNoteId").val() };
            frmdata[0].value = $("#frmOrderNotes #OrderNoteId").val();
            frmdata.push(order);
            frmdata.push(orderNote);
            frmdata.push({ name: "PageSessionToken", value: sessionStorage["PageSessionToken"] });
        }

        $.ajax({
            type: "POST",
            url: "/Order/_saveNotes/",
            data: frmdata,
            dataType: 'json',
            success: function (data) {
                orderNotesList.Refresh();
                LoadingPanel.Hide();
                $("#frmOrderNotes #Notes").val('');
                $("#frmOrderNotes #OrderNoteId").val('0');
                //var $noteRow = "<tr><td>"+$("#Notes").val() +"</td><td><small>" + $("#CurrentUserName").val() + ": " + getCurrentUKDate() + " </small></td>";
                //$("#tblOrderNotes tbody").append($noteRow);
                //$("#tblOrderNotes").show();
                //$("#Notes").val("");
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                LoadingPanel.Hide();
                alert('Error' + textStatus + "/" + errorThrown);
            }
        });
    }
}

function updateNoteLoad(noteId, notes) {
    notes = notes.replace(new RegExp('DBLQ', 'g'), '"');
    $("#frmOrderNotes #Notes").val(notes);
    $("#frmOrderNotes #OrderNoteId").val(noteId);
}

function updateNote(id, note) {
    LoadingPanel.Show();
    $.ajax({
        type: "POST",
        url: "/Order/_updateNotes/",
        data: { id: id, note: note },
        dataType: 'json',
        success: function (data) {
            orderNotesList.Refresh();
            LoadingPanel.Hide();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            // alert(xhr.status);
            LoadingPanel.Hide();
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });
}
function deleteNote(id, note) {
    if (confirm('Are you sure, you wish to delete this note? The change cannot be undone?')) {
        LoadingPanel.Show();
        $.ajax({
            type: "POST",
            url: "/Order/_deleteNotes/",
            data: { id: id, PageSessionToken: sessionStorage["PageSessionToken"] },
            dataType: 'json',
            success: function (data) {
                orderNotesList.Refresh();
                LoadingPanel.Hide();

                $("#tblOrderNotes tr[data-id=" + id + "]").remove();

                if (id < 0) {
                    $("#tblOrderNotes tr:eq(" + Math.abs(id) + ")").remove();
                }

                if ($("#tblOrderNotes tbody tr").length < 1) {
                    $("#tblOrderNotes").hide();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                LoadingPanel.Hide();
                alert('Error' + textStatus + "/" + errorThrown);
            }
        });
    }
}

//////////////////////////////////==============ORDER PROCESS SPECIFICS==================///////////////////////////

function OnBeginCallbackFormControls(s, e) {
    var container = $(s.GetMainElement());
    var token = $('input[name="__RequestVerificationToken"]', container).val();
    e.customArgs["__RequestVerificationToken"] = token;
}

function OnBeginCallbackBulkOrdersGrid(s, e) {
    e.customArgs["id"] = $("#OrderGroupToken").val();
}

function OnEndCallbackBulkOrdersGrid(s, e) {
    if (WorksOrderBulkCreatedListGridView.pageRowCount > 0) {
        $("#worksorders-bulkform input").prop("readonly", true);

        $("#worksorders-bulkform select").each(function () { if ($(this).val() != null) { $(this).closest("form").append("<input type='hidden' name='" + $(this).attr("name") + "' value='" + $(this).val() + "'>"); } });

        $("#worksorders-bulkform select").prop("disabled", true);

        $("#worksorders-bulkform .btn").prop("disabled", true);
        $("#worksorders-bulkform .chosen-select").trigger("chosen:updated");
        $("#worksorders-bulkform .dxEditors_edtDropDown").removeAttr("onchange, onfocus");
        $("#worksorders-bulkform input").css("background-color", "gainsboro");
        $("#worksorders-bulkform .dxic").css("background-color", "gainsboro");
        $("#worksorders-bulkform .dxEditors_edtDropDown").parent().removeAttr("click, onmouseup, onmousedown");
    }
}

function OnBeginCallbackEditSingleOrder(s, e) {
    var orderId = GetRowKeyByDxGridName(WorksOrderBulkCreatedListGridView);
    e.customArgs["id"] = orderId;
}

function loadOrderDetailCommonEvents() {
    $('.orddet').click(function (e) {
        var id = e.target.id;
        if (id == 'OrderDetailadd') {
            var accounId = $(".orderactcnts :selected").val();
            var warehouseId = $("#TransferWarehouseId :selected").val();
            var transfertype = $("#inventoryType").val();

            if (transfertype === "TransferIn" || transfertype === "TransferOut") {
                if (warehouseId === "" || warehouseId === null || warehouseId == undefined) {
                    return alert("Please select Warehouse first!");
                }
            }
            else {
                if (accounId === "" || accounId === null) {
                    return alert("Please select account first!");
                }
            }

            $('#selkeyhdPrddet').val(0);
            ModelOrdDetail.Show();
        } else if ($('#selkeyhdPrddet').val() != 0 && id != 'OrderDetailDelete') {
            ModelOrdDetail.Show();
        } else if (id == 'OrderDetailDelete' && $('#selkeyhdPrddet').val() != 0 && $('#selkeyhdPrddet').val() != "") {
            removeDetail($('#selkeyhdPrddet').val());
        }
    });

    $("#ProcessBarcode").unbind("keyup").on("keyup", function (e) {
        if (e.keyCode === 13) {
            var textEmptyCheck = $("#ProcessBarcode").val();
            if (textEmptyCheck === "" || textEmptyCheck === null) { return; }
            var matchedProduct = false;
            var productId = 0;
            var orderDetailId = 0;
            var directPostAllowed = false;
            var iscaseQty = false;
            $("div.sales-detail-actions").each(function () {
                var rowCodes = $(this).data("codes").toLowerCase().split(',');
                if (matchedProduct) return;

                if (rowCodes.indexOf($("#ProcessBarcode").val().toLowerCase()) >= 0) {
                    directPostAllowed = $(this).attr("data-direct-post") === "True";

                    if (directPostAllowed) {
                        matchedProduct = true;
                        productId = $(this).data("productid");
                        orderDetailId = $(this).data("id");
                        iscaseQty = rowCodes.indexOf($("#ProcessBarcode").val().toLowerCase()) === 2;
                        return;
                    } else {
                        if ($(this).find("a.process-button").length > 0) {
                            $(this).find("a.process-button").click();
                            matchedProduct = true;
                            return;
                        }
                    }
                }
            });
            if (!matchedProduct) {
                Gane.Helpers.ShowPopupMessage("Invalid code for this order",
                    "There are no products matching this code found in this order.");
            } else {
                if (directPostAllowed) {
                    submitProductProcessing(productId, orderDetailId, iscaseQty);
                }
            }
            $('#ProcessBarcode').val('');
            $('#ProcessBarcode').focus();
        }
    });
}

function submitProductProcessing(productId, orderDetailId, iscaseQty) {
    if (productId === null || productId.length < 1) {
        submitProduct();
        return;
    }

    LoadingPanel.Show();
    var frmdata = {
        ProductID: parseInt(productId),
        Quantity: 1,
        DeliveryNo: $("#DeliveryNumber").val(),
        OrderID: $("#OrderID").val(),
        OrderDetailID: orderDetailId,
        IsCaseQuantity: iscaseQty,
        InventoryTransactionTypeId: $("#InventoryTransactionTypeId").val(),
        ShipmentAddressLine1: $("#ShipmentAddressLine1").val(),
        ShipmentAddressLine2: $("#ShipmentAddressLine2").val(),
        ShipmentAddressLine3: $("#ShipmentAddressLine3").val(),
        ShipmentAddressTown: $("#ShipmentAddressTown").val(),
        ShipmentAddressPostcode: $("#ShipmentAddressPostcode").val()
    };
    $.post('/Order/_SubmitProcessedItems',
        frmdata,
        function (result) {
            if (result.error) {
                alert(result.Message);
                LoadingPanel.Hide();
            } else {
                ProcessCallbackFromProcessPost(result);
            }
        });
}

var completeConfirmed = function (isWorksOrder, suffix) {
    var deliverynumber = $('#DeliveryNumber').val();
    isWorksOrder = isWorksOrder || false;
    suffix = suffix || '';
    if (isWorksOrder) {
        var OrderId = $("#selkeyWorksOrderListGridView" + suffix).val();
        if (OrderId === null || OrderId === "" || OrderId === undefined) { return; }
    }
    var returnUrl = "/PurchaseOrders/Index";

    if (confirm('Are you sure to complete this order?')) {
        LoadingPanel.SetText('Please wait while trying to complete the order...');
        LoadingPanel.Show();

        var frmdata = {
            OrderID: isWorksOrder ? $("#selkeyWorksOrderListGridView" + suffix).val() : $("#OrderID").val()
        };

        $.get('/Order/CanAutoCompleteOrder/' + frmdata.OrderID, function (result) {
            if (!result.Success) {
                var processUrl = '/PurchaseOrders/ReceivePO/' + frmdata.OrderID;
                if (result.Suffix === 'SO') {
                    processUrl = '/SalesOrders/ProcessOrder/' + frmdata.OrderID;
                    returnUrl = "/SalesOrders/Index";
                }
                if (result.Suffix === 'WO') {
                    processUrl = '/WorksOrders/ProcessOrder/' + frmdata.OrderID;
                    returnUrl = "/WorksOrders/Index";
                }
                if (result.Suffix === 'TI' || result.Suffix === 'T0') {
                    processUrl = '/WorksOrders/ProcessOrder/' + frmdata.OrderID;
                    returnUrl = "/WorksOrders/Index";
                }
                if (result.Suffix === 'TO') {
                    processUrl = '/TransferOrders/ProcessOrder/' + frmdata.OrderID;
                    returnUrl = "/TransferOrders/Index";
                }

                var $popupContent = $("<div class='col-md-12 p-0 auto-complete-popup'><div class='col-md-12 h4'>There are still some items that are not dispatched for this order. What would you like to do?</div><div class='col-md-12 div-popup-actions'></div></div>");
                var $dispatchAndConfirmLink = $("<a class='btn btn-primary pull-right bt-d'>Dispatch & Complete</a>");
                var $reviewLink = $("<a class='btn btn-primary pull-right' href='" + processUrl + "'>Review</a>");
                var $forceCompleteLink = $("<a class='btn btn-primary pull-right bt-c'>Complete Without Dispatch</a>");
                //var $cancelActionLink = $("<a class='btn btn-primary pull-left' href='javascript:PopupMessage.Hide()'>Cancel</a>");

                $popupContent.find(".div-popup-actions").append($forceCompleteLink);
                $popupContent.find(".div-popup-actions").append($dispatchAndConfirmLink);
                $popupContent.find(".div-popup-actions").append($reviewLink);
                //$popupContent.find(".div-popup-actions").append($cancelActionLink);

                setTimeout(function () {
                    $(".auto-complete-popup .bt-d").on("click", function () { autoCompleteOrder(frmdata.OrderID, true, true, deliverynumber); });
                    $(".auto-complete-popup .bt-c").on("click", function () { autoCompleteOrder(frmdata.OrderID, false, true, deliverynumber); });
                }, 200);

                Gane.Helpers.ShowPopupMessage('Order is being verified', $popupContent);
            }
            else {
                if (result.Suffix === 'WO') {
                    returnUrl = "/WorksOrders/Index";

                    if ($("#_WorksOrderListGridView").length > 0) {
                        RefreshOrderDetailGrids();
                    }
                    if ($('#selkeyWorksOrderListGridViewGridName').length > 0) {
                        var gridview = GetDevexControlByName($('#selkeyWorksOrderListGridViewGridName').val());
                        if (gridview !== null) {
                            gridview.Refresh();
                        }
                    }

                    LoadingPanel.Hide();
                    location.href = location.origin + returnUrl + '#' + result.Suffix;
                }
                else if (result.Suffix === "TO" || result.Suffix === null || result.Suffix.length < 1) {
                    result.Suffix = "TO";
                    returnUrl = "/TransferOrders/Index";

                    LoadingPanel.Hide();
                    location.href = location.origin + returnUrl + '#' + result.Suffix;
                }
                else if (result.Suffix === "SO" || result.Suffix === null || result.Suffix.length < 1) {
                    result.Suffix = "SO";
                    returnUrl = "/SalesOrders/Index";

                    LoadingPanel.Hide();
                    location.href = location.origin + returnUrl + '#' + result.Suffix;
                }
                else if (result.Suffix === "PO" || result.Suffix === null || result.Suffix.length < 1) {
                    result.Suffix = "PO";
                    returnUrl = "/PurchaseOrders/Index";

                    LoadingPanel.Hide();
                    location.href = location.origin + returnUrl + '#' + result.Suffix;
                }
            }

            LoadingPanel.Hide();
        });
    }
};

function autoCompleteOrder(orderId, includeProcessing, forceComplete, deliverynumber) {
    var frmdata = {
        OrderID: orderId,
        IncludeProcessing: includeProcessing,
        ForceComplete: forceComplete,
        DeliveryNumber: deliverynumber
    };
    LoadingPanel.Show();
    $.post('/Order/AutoCompleteOrder/',
        frmdata,
        function (result) {
            LoadingPanel.Hide();
            var redirectUrl = '';
            var returnUrl = '';

            var urlParams = new URLSearchParams(window.location.search);
            var fragment = urlParams.get('fragment');

            switch (result.Suffix) {
                case 'SO':
                    redirectUrl = '/SalesOrders/ProcessOrder/' + frmdata.OrderID;
                    returnUrl = '/SalesOrders/Index';
                    break;
                case 'WO':
                    redirectUrl = '/WorksOrders/ProcessOrder/' + frmdata.OrderID;
                    returnUrl = '/WorksOrders/Index';
                    break;
                default:
                    redirectUrl = '/PurchaseOrders/ReceivePO/' + frmdata.OrderID;
                    returnUrl = '/PurchaseOrders/Index';
                    break;
            }

            if (!result.Success) {
                location.href = location.origin + redirectUrl + '#' + fragment;
            }
            else {
                PopupMessage.Hide();
                LoadingPanel.Hide();
                location.href = location.origin + returnUrl + '#' + fragment;
            }
        });
}

function completeOrder(isWorksOrder, suffix) {
    isWorksOrder = isWorksOrder || false;
    suffix = suffix || '';

    if (isWorksOrder) {
        completeConfirmed(true);
        LoadingPanel.Hide();
        return;
    }
    else {
        completeConfirmed();
    }
}

function loadOrderDetailProcessPickEvents() {
    if ($("#PickLocationsProductId").length > 0) {
        PickLocations($("#PickLocationsProductId").val());
    }
}

//----------Adjustment --------------------

function RowsCount() {
    if ($("#gridViewOrdDetEdit").length > 0) {
        if (gridViewOrdDetEdit.cpRowCount > 0) {
            DisableChosenDropdown('AccountID', true);
        }
        else {
            DisableChosenDropdown('AccountID', false);
        }
    }
}

//---------------- product  dropdown-------------------------------------

function OnchangeDropdown(s, e) {
    var type = $("#InventoryTransactionTypeId").val();
    var blindshipment = $("#blindshipment").val();
    var prdId = prdid.GetValue();
    if (s != null && s.GetSelectedIndex() < 0) {
        s.SetValue(null);
    }
    else {
        var tule = $("#ProductAllowance").val();
        if (type === "6" || type === "7") {
            if (prdid.GetValue() < 1) return false;
            var text = prdid.GetText();
            $("#divCurrentProductName").text(text);

            $.ajax({
                type: "GET",
                url: '/InventoryStocks/GetProductInformation/' + prdId,
                data: { ProductId: prdId },
                dataType: 'json',
                success: function (data) {
                    if (data.IsSerialised) {
                        //$("#divSerialsContainer").slideDown();
                        $("#ProductId").val(prdid.GetValue());
                        $("#frmInventoryStockAdjustment #Quantity").attr("disabled", "disabled");
                        InventorySerialAdjustPopup.Show();
                        $("#scanSerialsRowExistingSerials").html("");
                        for (var i = 0; i < data.ExistingSerials.length; i++) {
                            $("#scanSerialsRowExistingSerials").append("<input type='hidden' value='" + data.ExistingSerials[i].SerialNo + "' />");
                        }
                    }
                    else if (data.ProcessByPallet) {
                        palletProduct = true;
                        $("#inventoryAdjustCheck").prop('checked', true);
                    }
                    else {
                        $("#divSerialsContainer").slideUp();
                        $("#divExistingScannedSerialsTable").html("");
                        InventorySerialAdjustPopup.Hide();
                        $("#frmInventoryStockAdjustment #Quantity").removeAttr("disabled");
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    // alert(xhr.status);
                    alert('Error' + textStatus + "/" + errorThrown);
                }
            });
        }
        // 100  InventoryTransactionTypeId used  just for Creating invoices.
        else if (type == "15" || type == "100") {
            GProductId = prdid.GetValue();
            updateProductPrice();
            ProductIdEdit = GProductId;
        }
        // This check is for  Create pallet tracking
        else if (type == "200") {
            $.ajax({
                type: "GET",
                url: '/PalletTracking/GetProductCasePerPallet/',
                data: { ProductId: prdId },
                dataType: 'json',
                success: function (data) {
                    if (data !== 0) {
                        TotalCases.SetValue(data);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    // alert(xhr.status);
                    alert('Error' + textStatus + "/" + errorThrown);
                }
            });
        }
        else if (blindshipment == "True") {
            var id = prdid.GetValue();
            if (jQuery.isNumeric(prdId)) {
                ModelBS.Show();
            }
        }

        else if ($("#ProductAllowance").val() == "True") {
            $("#ProductId").val(prdid.GetValue());
        }
        else {
            prdId = prdid.GetValue();
            var item = s.GetSelectedItem();
            availableProduct = parseFloat(item.GetColumnText('InventoryStocks'));
            endOrderDetailCallback();
            $('.ordering-notes').hide();
            loadProdAccounts();
        }
    }
}

function OnStartCallback(s, e) {
    var type = $("#type").val();
    var inventoryTransaction = $("#InventoryTransactionTypeId").val();
    if (type === "12" || type === "17") {
        var orderId = $("#grorderid").val();
        e.customArgs["OrderId"] = $("#grorderid").val();
    }
}
function OnEndCallbackDS(s, e) {
    //var inventoryTransaction = $("#InventoryTransactionTypeId").val();
    //if (inventoryTransaction == "15" || inventoryTransaction == "100") {
    //    if (GProductId > 0) {
    //        prdid.SetValue(GProductId);
    //    }
    //}
}

//------------------------------------------------------------------------
function updatePageSessionToken(newPageGuid, forceRegenerate) {
    if (sessionStorage["PageSessionToken"] == null || forceRegenerate) {
        sessionStorage["PageSessionToken"] = newPageGuid;
        $("form").each(function () {
            if ($(this).find("#PageSessionToken").length < 1) {
                $("form").append("<input type='hidden' id='PageSessionToken' name='PageSessionToken' value='" + newPageGuid + "' />");
            } else {
                $(this).find("#PageSessionToken").val(newPageGuid);
            }
        });
    }
}

function WorksOrderBulkGUID(e) {
    // force to generate new GUID
    var newGuid = guid();
    updatePageSessionToken(newGuid, true);
    if (e != null) {
        e.customArgs["PageSessionToken"] = newGuid;
    }
}

$(document).ready(function () {
    loadOrderDetailCommonEvents();

    $(".lnkDelete").on("click", function () {
        var type = $(this).data("type");
        var orderId;
        if (type === 'AO') {
            orderId = $(this).data("target");
        }
        else {
            var target = $(this).data("target");
            orderId = $("" + target).val();
        }

        DeleteOrder(orderId, type);
    });

    function DeleteOrder(orderId, type) {
        if (orderId == null || orderId < 1) return;

        var confirmedAction = function () {
            $.get("/Order/DeleteOrder/" + orderId,
                function (data) {
                    if (!data) {
                        alert("You don’t have permission to perform this action!");
                    }
                    else {
                        switch (type) {
                            case 'PO':
                                _PurchaseOrderListGridView.Refresh(); break;
                            case 'SO':
                                _SalesOrderListGridView_Active.Refresh(); break;
                            case 'AO':
                                AwaitingAuthGridview.Refresh(); break;
                            default:
                            case 'WO':
                                _WorksOrderListGridView.Refresh();
                                break;
                        }
                    }
                });
        };
        if (confirm("This action cannot be reversed, are you sure you want to delete this order?")) {
            confirmedAction();
        }
    }
});

function LoadSOAccountAddresses() {
    var blindshipment = $("#blindshipment").val();
    var type = $('#inventoryType').val();
    if (type !== "PurchaseOrder") {
        if (blindshipment !== 'True' || blindshipment === "") {
            if ($("input[name='SOShipmentDestination']").length < 1) return;
        }
    }

    var id = $("#AccountID option:selected").val();
    if (id == null || id == "" || id == 0) { return; }
    if (type === "PurchaseOrder") {
        $.get("/PurchaseOrders/GetAccountAddressByAccountId/" + $("#AccountID").val(), function (s, e) {
            $('#ShipmentAccountAddressId').empty();
            if (s.length > 0) {
                $.each(s, function (i, item) {
                    if (i === 0) {
                        $('#ShipmentAccountAddressId').append('<option selected value=' + item.AccountAddressId + '>' + item.AccountAddress + '</option>');
                    }
                    else {
                        $('#ShipmentAccountAddressId').append('<option value=' + item.AccountAddressId + '>' + item.AccountAddress + '</option>');
                    }
                });
                $("#ShipmentAccountAddressId").trigger("chosen:updated");
                if ($("#radioShipToCollection:checked").length > 0) {
                    populateShipmentAddress($("#ShipmentAccountAddressId").find("option:selected").text());
                }
            }
        });
    }

    else {
        $.get("/Order/GetAccountAddresses/" + $("#AccountID").val(),
            function (data) {
                var options = "<option value='0'>Select Address</option>";
                for (var i = 0; i < data.length; i++) {
                    options += "<option value='" + data[i].Value + "'>" + data[i].Text + "</option>";
                }
                $("#ShipmentAccountAddressId").html(options);
                $("#ShipmentAccountAddressId").trigger("chosen:updated");
                $("#ShipmentAccountAddressId").change();
                $("#ShipmentAccountAddressId").val($("#LastShipmentAddressId").val());
                $("#ShipmentAccountAddressId").trigger("chosen:updated");
                updateSOShipmentInfo();
                LoadingPanel.Hide();
            });
    }
}

var updateSOShipmentInfo = function () {
    var blindshipment = $("#blindshipment").val();
    if (blindshipment !== 'True' || blindshipment === "") {
        $("#divDisplayAccountAddresses").css("visibility", "visible");
        $("#so-shipment-info").css("visibility", "visible");
        if ($("#DeliveryMethod").val() === '2' || $("#DeliveryMethod").val() === '') { /*DeliveryMethods.Collection = 2*/
            $("#so-shipment-info").slideUp();
        } else {
            $("#so-shipment-info").slideDown();
        }

        if ($("input[name='SOShipmentDestination']:checked").val() === "2") {
            $("#ShipmentAccountAddressId").val(0);
            return;
        }
    }
    var address = $("#ShipmentAccountAddressId option:selected").text();

    $("#divCustomShipmentAddress input").val('');
    if (address !== null && address !== 'Select') {
        var addressParts = address.split(',');
        if (addressParts.length < 1) return;
        $("#ShipmentAddressLine1").val(addressParts[0] === undefined ? '' : addressParts[0].trim());
        $("#ShipmentAddressLine2").val(addressParts[1] === undefined ? '' : addressParts[1].trim());
        $("#ShipmentAddressLine3").val(addressParts[2] === undefined ? '' : addressParts[2].trim());
        $("#ShipmentAddressTown").val(addressParts[4] === undefined ? '' : addressParts[4].trim());
        $("#ShipmentAddressPostcode").val(addressParts[5] === undefined ? '' : addressParts[5].trim());
    }
};

function InitialiseSalesOrderShipmentEvents() {
    $("input[name='SOShipmentDestination']").click(function () {
        var value = parseInt($(this).val());
        if (value === 1) {
            $("#divCustomShipmentAddress input").val('');
            $("#divDisplayAccountAddresses").slideDown();
            LoadSOAccountAddresses();
        }
        else {
            $("#divCustomShipmentAddress input").val('');
            $("#divDisplayAccountAddresses").slideUp();
            $("#ShipmentAccountAddressId").val(0);
            $("#ShipmentAccountAddressId").trigger("chosen:updated");
        }
    });

    $("#ShipmentAccountAddressId").on("change", function () {
        updateSOShipmentInfo();
    });

    $("#DeliveryMethod").on("change", function () {
        updateSOShipmentInfo();
    });
}

function LoadPriceGroupEditForm(priceGroupId) {
    $("#SelectedPriceGroupID").val(priceGroupId);
    ModalEditPriceGroup.Show();
}

function SavePriceGroup(e) {
    var res = PopupFormValidationCheck(e);
    if (res === false) return;

    var frmdata = $("#frmSaveProductGroup").serializeArray();
    $.post('/ProductSpecialPrice/SavePriceGroup/',
        frmdata,
        function (result) {
            if (!result.Success) {
                Gane.ShowPopupMessage('Error occurred', result.Message);
            }
            else {
                ModalEditPriceGroup.Hide();
                PriceGroupsGridView.Refresh();
            }
        });
}

function DeletePriceGroup(id) {
    var frmdata = {
        id: id
    };
    if (confirm('Are you sure you wish to delete this price group?')) {
        $.post('/ProductSpecialPrice/DeletePriceGroup/',
            frmdata,
            function (result) {
                if (!result.Success) {
                    Gane.ShowPopupMessage('Error occurred', result.Message);
                }
                else {
                    ModalEditPriceGroup.Hide();
                    PriceGroupsGridView.Refresh();
                }
            });
    }
}

// update order status
$(document).ready(function () {
    $('body').on('click', '.unlock-order', function () {
        var orderId = $(this).data("orderid");
        var statusId = $(this).data("statusid");
        var type = $(this).data("type");

        if (confirm("Are you sure to unlock order for picking?")) {
            result = UpdateOrderStatus(orderId, statusId, type);
        }
    });
});

$(document).ready(function () {
    $('body').on('click', '.make-order-active', function () {
        var orderId = $(this).data("orderid");
        var statusId = $(this).data("statusid");
        var type = $(this).data("type");
        if (type === "CO") {
            var orderProcessId = parseInt(orderId);
            if (confirm("Are you sure to mark this delivery complete?")) {
                $.ajax({
                    typae: "GET",
                    url: "/SalesOrders/UpdateDeliveryStatus/",
                    data: { orderProcessId: orderProcessId },
                    dataType: 'json',
                    success: function (data) {
                        LoadingPanel.Hide();
                        if (data) {
                            consignmentgridview.Refresh();
                        }
                    }
                });
            }
        }

        else {
            if (confirm("Are you sure to make this order active?")) {
                result = UpdateOrderStatus(orderId, statusId, type);
            }
        }
    });
});

function UpdateOrderStatus(orderId, statusId, type) {
    $.get("/Order/UpdateOrderStatus/" + orderId + "/" + statusId,
        function (data) {
            UpdateOrderStatusSuccess(data, type);

            if (data != false && data != true) {
                alert(data);
            }
        });
}

function UpdateOrderStatusSuccess(data, type) {
    if (data != false) {
        switch (type) {
            case 'PO':
                _PurchaseOrderListGridView.Refresh();
                _PurchaseOrderListGridView_Completed.Refresh();
                break;
            case 'SO':
                _SalesOrderListGridView_Active.Refresh();
                _SalesOrderListGridView_Completed.Refresh();
                break;
            case 'WO':
                _WorksOrderListGridView.Refresh(); break;
            case 'TO':
                _TransferInOrderListGridView.Refresh();
                _TransferOutOrderListGridView.Refresh();
                break;
            default:
        }
    }
    else {
        alert("Failed to complete operation");
    }
}
var populateShipmentAddress = function (selectedAddress) {
    if ($("input[name=ShipmentDestination]:checked").length < 1)
        return;

    if ($("#radioShipToCustomProperty:checked").length > 0)
        return;
    $("#divFinalShipmentAddress input[type=text]").val('');
    var address = selectedAddress;
    var addressLines = address.split(',');

    if (addressLines.length < 1) return;

    $("#divFinalShipmentAddress").slideDown();
    var postCode = addressLines[addressLines.length - 1];
    $("#ShipmentAddressPostcode").val(postCode.trim());

    $("#ShipmentAddressLine1").val(addressLines[0].trim());

    if (addressLines.length < 3) return;
    $("#ShipmentAddressLine2").val(addressLines[1].trim());
    if (addressLines.length < 4) return;
    $("#ShipmentAddressLine3").val(addressLines[2].trim());
    if (addressLines.length < 5) return;
    $("#ShipmentAddressTown").val(addressLines[3].trim());
};

function VerifyPallete(e, cnt) {
    var Req_url = window.location.href;
    var type;
    var data = {};
    var url;
    type = $('#type').val();
    if (type !== "2") {
        var pid = $('#prodId').val();
        var palletpercase = parseInt($('#palletpercase').val());
        if (cnt.id === "bstext") {
            pid = prdid.GetValue();
        }
        else {
            var qty = parseInt(QuantityP.GetValue());
            var rec_qty = parseInt($('#rec_qty').val());
        }
        var palletTrackingId = $("#palletTrackingId").val();
        data = { serial: $(cnt).val(), pid: pid, type: type };
        $(cnt).css("border-color", "lightgray");

        if (e.keyCode === 13) {
            var chekval = $('.serial').val();
            if ($.inArray(chekval, serialArray) >= 0) {
                alert("Serial number already exists.");
                $(".scaned").val("").focus();
                return;
            }

            LoadingPanel.Show();

            $.ajax({
                type: "GET",
                url: "/PurchaseOrders/_VerifyPallete/",
                data: data,
                dataType: 'json',
                success: function (data) {
                    LoadingPanel.Hide();
                    if (!data) {
                        $(cnt).css("border-color", "red");
                        alert("Serial number is not available to process");
                        $(".scaned").val("").focus();
                        return;
                    }
                    if (data == "Expired") {
                        alert("The Serial number is Expired");
                        $(".scaned").val("").focus();
                        return;
                    }
                    if (parseInt(data[2]) <= 0) {
                        alert("There is no remaining cases for this serial number");
                        return;
                    }
                    var items = parseInt(data[2]) * palletpercase;
                    if (cnt.id === "bstext") {
                        serialArray.push(data[1]);
                        var markup = "<tr><td>" + data[1] + "</td><td  class='pallettracking' hidden>" + data[0] + "</td><td contenteditable='true' class='quantity'>" + data[2] + "</td></tr>";
                        $(".tableSerial tbody").append(markup);
                        $(".scaned").val("").focus();
                    }

                    else {
                        if (items > qty) {
                            var cases = qty / palletpercase;
                            var res = confirm("Items are exceeding? Do you still want to process");
                            if (res) {
                                if (QuantityP.GetValue() === 0) { return; }
                                serialArray.push(data[1]);
                                var markups = "<tr><td class='ser'>" + data[1] + "</td><td  class='pallettracking' hidden>" + data[0] + "</td><td class='quantity'>" + data[2] + "</td></tr>";

                                $(".tableSerial tbody").append(markups);
                                var req = qty - qty;
                                QuantityP.SetValue(parseInt(req));

                                $(".cases").val((req / palletpercase).toFixed(2));
                                updateRecored();
                                $(".scaned").val("").focus();
                                return;
                            }
                            else {
                                if (qty <= 0) { return; }
                                data[2] = cases.toFixed(2);
                                var recal = qty - qty;
                                QuantityP.SetValue(parseInt(recal));

                                var updatedcase = recal / palletpercase;
                                $(".cases").val(updatedcase.toFixed(2));
                            }
                        }
                        else {
                            $('#qty').val(qty);
                            var reqs = qty - items;

                            QuantityP.SetValue(parseInt(qty - items));
                            var updatedcases = reqs / palletpercase;
                            $(".cases").val(updatedcases.toFixed(2));
                        }
                        serialArray.push(data[1]);
                        markup = "<tr><td>" + data[1] + "</td><td  class='pallettracking' hidden>" + data[0] + "</td><td contenteditable='true' class='quantity'>" + data[2] + "</td></tr>";
                        $(".tableSerial tbody").append(markup);
                        $(".scaned").val("").focus();
                    }
                }
            });
        }
    }
    else {
        VerifyPalletSalesOrder(e, cnt);
    }
}
function CloseModel() {
    if ($(".tableSerial > tbody > tr").length > 0) {
        var res = confirm("You have not save the scaned pallet!  You still want to cancel?");

        if (res) {
            ModelAddPallete.Hide();
            ModelGoodsReturn.Hide();
            ModelBS.Hide();
            serialArray = [];
            return;
        }
        return;
    }
    ModelAddPallete.Hide();
    ModelGoodsReturn.Hide();
    ModelBS.Hide();
    serialArray = [];
}

function SaveSerials() {
    if ($(".tableSerial > tbody > tr").length <= 0) {
        var res = alert("Please scaned pallet First!");
        return;
    }

    var cases = [];
    var ordid = $('#order').val();

    var prid = $('#prodId').val();
    var DeliveryNo = $('#delivery').val();
    var orderDetailId = $("#OrderDetailID").val();
    var type = $('#type').val();

    var url = "/PurchaseOrders/_SubmitPalleteSerials/";
    $(".quantity").each(function () {
        var quantity = $(this).text();
        cases.push($(this).parent().children('td.pallettracking').html() + "#+#" + $(this).html());
    });

    var data = { serialList: cases, pid: prid, orderId: ordid, DeliveryNo: DeliveryNo, OrderDetailID: orderDetailId };
    if (type === "2" || type === "8") {
        orderDetailId = $("#selkeyhdPrddet").val();
        if (type === "8") {
            orderDetailId = $("#selkeyworkorder").val();
        }

        url = "/SalesOrders/_SubmitPalleteSerials/";
        data = { serialList: cases, pid: prid, orderId: ordid, DeliveryNo: DeliveryNo, OrderDetailID: orderDetailId, type: type };
    }
    if (orderDetailId === "" || orderDetailId === null || orderDetailId <= 0) { alert("There is no order to be Processed"); }
    if (window.location.href.indexOf("goodsreturn")) {
        ordid = $("#grorderid").val();
    }

    LoadingPanel.Show();
    $.post(url, data, function (result) {
        // Do something with the response `res`
        var info = "Item submitted!";
        if (type === "2") {
            gvODetail.Refresh();
        }
        else if (type === "8") {
            gvWODetail.Refresh();
        }
        else {
            gvPODetail.Refresh();
        }
        $(".scaned").val("").focus();

        LoadingPanel.Hide();
        ModelAddPallete.Hide();

        $("#infoMsg").html(info).show();
        $('#infoMsg').delay(2000).fadeOut();
    });
}

function VerifyPalletSalesOrder(e, cnt) {
    var Req_url = window.location.href;
    var type;
    var data = {};
    var url;
    var palletpercase = parseInt($('#palletpercase').val());
    var pid = $('#prodId').val();
    if (cnt.id === "bstext") {
        pid = prdid.GetValue();
    }
    else {
        var qty = parseInt(QuantityP.GetValue());
        var rec_qty = parseInt($('#rec_qty').val());
    }
    type = $('#type').val();
    var palletTrackingId = $("#palletTrackingId").val();
    var orderId = $("#OrderID").val();
    $(cnt).css("border-color", "lightgray");

    if (e.keyCode === 13) {
        var date = $(".palletserial").text();
        if (palletTrackingId === "3" || palletTrackingId === "4") {
            data = { serial: $(cnt).val(), pid: pid, orderId: orderId, type: type, palletTrackingId: palletTrackingId, date: date };
        }
        else {
            data = { serial: $(cnt).val(), pid: pid, orderId: orderId, type: type, palletTrackingId: palletTrackingId };
        }

        if ($.inArray($('.serial').val(), serialArray) >= 0) {
            alert("Serial number already exists.");
            $(".scaned").val("").focus();
            return;
        }
        if (qty <= 0) {
            alert("You have no more quantity to scan.");
            return;
        }

        var palletId = $("#palletTrackingId").val();
        if (palletId === "1" || palletId === "2") {
            if ($(".palletserial").text() !== $(".scaned").val()) {
                alert("You are Scanning Wrong Pallet.");
                $(".scaned").val("").focus();
                return;
            }
        }

        // Cancel the default action on keypress event
        LoadingPanel.Show();

        $.ajax({
            type: "GET",
            url: "/SalesOrders/_VerifyPallete/",
            data: data,
            dataType: 'json',
            success: function (data) {
                LoadingPanel.Hide();

                if (!data) {
                    $(cnt).css("border-color", "red");
                    alert("Serial number is not available to process");
                    $(".scaned").val("").focus();
                    return;
                }
                if (data == "Expired") {
                    alert("The serial number is expired");
                    $(".scaned").val("").focus();
                    return;
                }
                if (parseInt(data[2]) <= 0) {
                    $(cnt).css("border-color", "red");
                    alert("Items exceeds number of items for this product");
                    $(".scaned").val("").focus();
                    return;
                }

                var items = parseInt(data[2]) * palletpercase;
                if (cnt.id === "bstext") {
                    serialArray.push(data[1]);
                    var markup = "<tr><td>" + data[1] + "</td><td  class='pallettracking' hidden>" + data[0] + "</td><td contenteditable='true' class='quantity'>" + data[2] + "</td></tr>";
                    $(".tableSerial tbody").append(markup);
                    $(".scaned").val("").focus();
                }
                else {
                    if (items > qty) {
                        var cases = qty / palletpercase;
                        var req = qty - qty;
                        var res = alert("items exceding only take " + cases.toFixed(2) + " cases in grid");
                        serialArray.push(data[1]);
                        var markups = "<tr><td class='ser'>" + data[1] + "</td><td  class='pallettracking' hidden>" + data[0] + "</td><td class='quantity'>" + cases.toFixed(2) + "</td></tr>";

                        $(".tableSerial tbody").append(markups);
                        QuantityP.SetValue(parseInt(req));

                        var updatecase = req / palletpercase;
                        $(".cases").val(updatecase.toFixed(2));
                        updateRecored();
                        $(".scaned").val("").focus();
                        return;
                    }

                    else {
                        $('#qty').val(qty);
                        var reqs = qty - items;
                        var updatecases = reqs / palletpercase;

                        QuantityP.SetValue(parseInt(reqs));
                        $(".cases").val(updatecases.toFixed(2));
                    }

                    serialArray.push(data[1]);

                    markup = "<tr><td class='ser'>" + data[1] + "</td><td  class='pallettracking' hidden>" + data[0] + "</td><td class='quantity'>" + data[2] + "</td></tr>";
                    $(".tableSerial tbody").append(markup);

                    updateRecored();
                    $(".scaned").val("").focus();
                }
            }
        });
    }
}

var totalCase;
var orginalcase;
var productId;
var orderId;
var ordernumber;
var inType;
var groups;
///////////////////////////////================Goods Returns///////////////////////////////////
function VerifyPalletReturns(e, cnt) {
    var type;
    var data = {};
    type = $("#type").val();
    if (type === null || type === undefined || type === "") {
        type = $("#InventoryTransactionTypeId").val();
    }
    var qty = $(".casesReturns").val();
    if (qty === "" || qty === 0 || qty === null) {
        alert("Please enter quantity / cases you want to return!");
        QuantityP.SetFocus();

        return;
    }
    if (type === "6" || type === "7") {
        data = { serial: $(cnt).val(), pid: prdid.GetValue(), type: type };
    }
    else {
        data = { serial: $(cnt).val(), pid: prdid.GetValue(), type: type };
    }
    $(cnt).css("border-color", "lightgray");

    if (e.keyCode === 13) {
        var palletpercase = parseInt($('#palletpercase').val());
        var chekval = $('.serial').val();
        if ($.inArray(chekval, serialArray) >= 0) {
            alert("Serial number already exists.");
            $(".scaned").val("").focus();
            return;
        }

        var palletReturns = $('.casesReturns').val();

        if (palletReturns === "" || palletReturns === null || palletReturns === undefined || isNaN(palletReturns)) {
            alert("something wrong.");
            return;
        }

        //qty = qty / palletpercase;

        if (qty <= 0) {
            alert("You have no more qunatity to scan.");
            $(".scaned").val("").focus();
            return;
        }

        // Cancel the default action on keypress event
        LoadingPanel.Show();

        $.ajax({
            type: "GET",
            url: "/inventorytransaction/_VerifyPalleteReturns/",
            data: data,
            dataType: 'json',
            success: function (data) {
                LoadingPanel.Hide();
                if (!data) {
                    $(cnt).css("border-color", "red");
                    alert("Serial number is not available to process");
                    $(".scaned").val("").focus();
                    return;
                }
                if (data == "Expired") {
                    alert("The serial number is expired");
                    $(".scaned").val("").focus();
                    return;
                }

                if (type !== "6" && type !== "7") {
                    if (type == "12") {
                        var items = parseInt(data[3]) - parseInt(data[2]);
                    }
                    else {
                        items = parseInt(data[2]);
                    }
                    if (items === 0) {
                        alert("There is no quantity to return");
                        return;
                    }

                    if (qty > items) {
                        qty = qty - items;
                        QuantityP.SetValue(parseInt(qty * palletpercase));
                        //var res = alert("Cases are exceeding");

                        $('.casesReturns').val(qty.toFixed(2));
                        $(".scaned").val("").focus();
                    }
                    else {
                        items = qty;
                        qty = qty - qty;

                        QuantityP.SetValue(parseInt(qty));
                        $('.casesReturns').val(qty.toFixed(2));
                    }
                    serialArray.push(data[1]);
                    totalCase = data[3] - data[2];
                    orginalcase = items;
                    var markup = "<tr><td>" + data[1] + "</td><td  class='pallettracking' hidden>" + data[0] + "</td><td contenteditable='true' class='quantity' onkeyup='checkTotCase(event,this)'>" + items + "</td></tr>";
                    $(".tableSerial tbody").append(markup);
                    $(".scaned").val("").focus();
                }

                else {
                    items = parseInt(data[2]);
                    var res = qty;

                    if (items === 0) {
                        if (type === "7") {
                            alert("Yor are not allow to do adjustment out");
                        }
                    }

                    if (res > items) {
                        var value = res - items;
                        qty = value * palletpercase;
                        QuantityP.SetValue(parseInt(qty));
                        //var res = alert("Cases are exceeding");

                        $('.casesReturns').val(value.toFixed(2));
                        $(".scaned").val("").focus();
                    }
                    else {
                        items = res;
                        QuantityP.SetValue(0);

                        $('.casesReturns').val(0);
                    }
                    serialArray.push(data[1]);
                    totalCase = data[2];
                    orginalcase = items;

                    if (items == 0) {
                        items = $('.casesReturns').val();
                        QuantityP.SetValue(0);
                        $('.casesReturns').val(0);
                    }

                    markup = "<tr><td>" + data[1] + "</td><td  class='pallettracking' hidden>" + data[0] + "</td><td contenteditable='true' class='quantity'>" + items + "</td></tr>";
                    $(".tableSerial tbody").append(markup);
                    $(".scaned").val("").focus();
                }
            }
        });
    }
}
function SavePalletReturns() {
    var Req_url = window.location.href;
    var sellable = $(".sellableCondition").prop("checked");
    if ($(".tableSerial > tbody > tr").length <= 0) {
        var res = alert("Please scaned pallet First!");
        return;
    }
    var cases = [];
    var ordid = $('#grorderid').val();
    var prid = prdid.GetValue();
    //$('.grDrp :selected').val();
    var type = $("#type").val();
    var url = "/inventorytransaction/_SubmitPalleteSerials/";
    var groupToken = $("#groupToken").val();
    var delivery = $("#deliveryNumber").val();
    $(".quantity").each(function () {
        var quantity = $(this).text();
        cases.push($(this).parent().children('td.pallettracking').html() + "#+#" + $(this).html());
    });
    if (sellable === false) {
        type = "17";
    }
    if (type === null || type === undefined || type === "") {
        type = $('#InventoryTransactionTypeId').val();
        prid = prdid.GetValue();
    }

    var data = { serialList: cases, pid: prid, orderId: ordid, type: type, groupToken: groupToken, deliveryNumber: delivery };

    LoadingPanel.Show();
    $.post(url, data, function (result) {
        ordernumber = result.orderNumber;
        productId = result.productId;
        orderId = result.orderid;
        groups = result.groupToken;
        inType = type;
        LoadingPanel.Hide();
        var info = "Item submitted!";
        if (type === "12" || type === "17") {
            GoodsReturn.Refresh();
            ModelGoodsReturn.Hide();
        }

        else if (type === "6" || type === "7") {
            ModelAddPallete.Hide();
        }
        else {
            WastageGoodsReturn.Refresh();
            ModelGoodsReturn.Hide();
        }

        $(".scaned").val("").focus();

        LoadingPanel.Hide();
        ModelAddPallete.Hide();
        serialArray = [];
        $("#infoMsg").html(info).show();
        $('#infoMsg').delay(2000).fadeOut();
    });
}

function returnnon_ser_Product() {
    LoadingPanel.Show();
    var orderid = $('#grorderid').val();
    var ordernumber = $('#grOrderNumber').val();
    var pid = prdid.GetValue();
    if (pid === null || pid === undefined || pid === "") {
        pid = $("#wrProducts option:selected").val();
    }
    var sku = $("#grProducts option:selected").text();
    var qty = Quantity.GetNumber();
    if ($('#grorderid').val() === null || $('#grorderid').val() === "") {
        addNonSerProduct_Inventory($('#grorderid').val(), pid, qty);
    }
    else {
        $.ajax({
            type: "GET",
            url: "/InventoryTransaction/_IsQuantityValid/",
            data: { order: $('#grorderid').val(), product: pid, quantity: qty },

            success: function (data) {
                if (data.error) {
                    if (data.errorcode === 1) {
                        if (confirm("Product " + sku + " was not sold against order " + ordernumber + ", do you still want to proceed with Return ?")) {
                            //Proceed with return
                            {
                                addNonSerProduct_Inventory($('#grorderid').val(), pid, qty);
                            }
                        }
                        else {
                            LoadingPanel.Hide();
                        }
                    }
                    else if (data.errorcode === 2) {
                        if (confirm("Quantity exceeds with shipped quantity for order " + ordernumber + "for product " + sku + ", do you still want to proceed with Return ?")) {
                            //Proceed with return
                            addNonSerProduct_Inventory($('#grorderid').val(), pid, qty);
                        }
                        else {
                            LoadingPanel.Hide();
                        }
                    }
                }
                else {
                    addNonSerProduct_Inventory($('#grorderid').val(), pid, qty);
                    //Proceed with return
                    LoadingPanel.Hide();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                LoadingPanel.Hide();
                alert('Error' + textStatus + "/" + errorThrown);
                rst = false;
            }
        });
    }
}
function addNonSerProduct_Inventory(orderid, pid, qty) {
    var location = $("#LocationId option:selected").val();
    var type = $("#type").val();
    var groupToken = $("#groupToken").val();
    var sellable = $(".sellableCondition").prop("checked");
    var delivery = $("#deliveryNumber").val();
    if (sellable === false) {
        type = "17";
    }
    $.ajax({
        type: "POST",
        dataType: 'json',
        url: "/InventoryTransaction/_ReturnNon_SerProduct/",
        data: { order: orderid, product: pid, quantity: qty, type: type, groupToken: groupToken, locationid: location, deliveryNumber: delivery },
        success: function (data) {
            if (type === "12" || type === "17") {
                ordernumber = data.orderNumber;
                productId = data.productId;
                orderId = data.orderid;
                inType = type;
                groups = data.groupToken;
                GoodsReturn.Refresh();
            }
            else if (type === "14") {
                ordernumber = data.orderNumber;
                productId = data.productId;
                orderId = data.orderid;
                inType = type;
                groups = data.groupToken;
                WastageGoodsReturn.Refresh();
            }
            ModelGoodsReturn.Hide();
            LoadingPanel.Hide();

            $("#infoMsg").html("Return Succeeded!").show();
            $('#infoMsg').delay(2000).fadeOut();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            // alert(xhr.status);
            LoadingPanel.Hide();
            ModelGoodsReturn.Hide();
            //alert('Error' + textStatus + "/" + errorThrown);
            rst = false;
        }
    });
}

function QunatitytoCases(s, e) {
    var quantity = s.GetValue();
    var palletpercase = parseInt($('#palletpercase').val());
    $(".casesReturns").val((quantity / palletpercase).toFixed(2));
}
function CasestoQunatity() {
    var cases = $(".casesReturns").val();
    var palletpercase = parseInt($('#palletpercase').val());
    QuantityP.SetValue(cases * palletpercase);
}

function OnImageClick(url) {
    url = '/UploadedFiles/DispatchSign/' + url;
    $('#myImg').attr('src', url);
    popupImage.Show();
}

function showattachment(url) {
    var source = url.replace('~', '');
    $('#DynamicPdf').attr('src', source);
    ShowAttachment.Show();
}
/////////////////////////////////////////////////////////

function updateRecored() {
    var palletTrackingId = $("#palletTrackingId").val();
    var palletpercase = parseInt($('#palletpercase').val());
    var qty = parseInt($('#qty').val());
    qty = palletpercase * qty;
    type = $('#type').val();
    if (palletTrackingId !== null && palletTrackingId !== "" && palletTrackingId !== undefined) {
        data = { serial: serialArray, pid: $('#prodId').val(), palletTrackingId: palletTrackingId };

        $.ajax({
            type: "POST",
            url: "/SalesOrders/_GetNextSerials/",
            data: data,
            dataType: 'json',
            success: function (data) {
                LoadingPanel.Hide();
                $('.updatedate').text(data[0]);
                $(".palletserial").text(data[0]);
            }
        });
    }
}

function checkTotCase(event, e) {
    if (totalCase < parseInt(e.innerText)) {
        alert("Cases are exceeding");
        $(e).html("").html(orginalcase);
    }
}

function calculateQuantity(e, res) {
    var cases = e.currentTarget.value;
    if (cases <= 0 || cases == null || cases === undefined) {
        cases = 1;
    }

    if (res) {
        var prodcase = $("#processcase").val();
        if (prodcase <= 0 || prodcase === null || prodcase === undefined) {
            prodcase = 1;
        }
        var qty = prodcase * cases;
        Quantity.SetValue(qty);
    }
    else {
        prodcase = $("#processcase").val();
        if (prodcase <= 0 || prodcase === null || prodcase === undefined) {
            prodcase = 1;
        }
        qty = prodcase * cases;
        Qty.SetValue(qty);
    }
}

function OnBeginGrCallBack(s, e) {
    e.customArgs["prodId"] = productId;
    e.customArgs["ordernumber"] = ordernumber;
    e.customArgs["orderId"] = orderId;
    e.customArgs["InType"] = inType;
    e.customArgs["groupToken"] = groups;
}
function OnBeginConsigment(s, e) {
    var consignment = $("#consigment").val();
    if (consignment === "True") {
        e.customArgs["OrderStatusId"] = 2;
    }
}

var ordersProcessDetailId = 0;
var type;
var productType;
var palletTrackingIds = 0;
function UpdateProcessDetails(OrderprDetID, remove, so) {
    if (remove == true) {
        if (so == 1) {
            type = true;
        }
        productType = remove;
        ordersProcessDetailId = OrderprDetID;
        ModalPopupEditSerializedProduct.Show();
    }
    else if (remove == false) {
        productType = remove
        ordersProcessDetailId = OrderprDetID;
        ModalPopupEditDelivery.Show();
    }
    else if (remove == "pallet") {
        productType = remove;
        palletTrackingIds = OrderprDetID;
        ModalPopupEditDelivery.Show();
    }

    else {
        productType = remove;
        ModalPopupEditDelivery.Show();
    }
}
function BeginEditDliveryPopUp(s, e) {
    e.customArgs["OrderprocessDetailId"] = ordersProcessDetailId;
    e.customArgs["ProductType"] = productType;
    e.customArgs["PalletTrackingId"] = palletTrackingIds;
}

function UpdateInventoryDetails(id) {
    ordersProcessDetailId = id;
    ModalPopupEditInventory.Show();
}

function SaveOrderProcessDetail() {
    var data = {
        OrderProcessDetailId: $("#OrderProcessDetailID").val(),
        Quantity: $("#QtyProcessed").val(),
        wastedReturn: $("#WastedReturn:checked").val(),
        PalletTrackingid: palletTrackingIds
    };
    LoadingPanel.Show();
    $.post("/SalesOrders/UpdateOrderProcessDetail", data, function (result) {
        LoadingPanel.Hide();
        ModalPopupEditDelivery.Hide();
        var value = $("#inventoryTransactionType").val();
        if (value == "1" || value == 1) {
            deliveriesgridview.Refresh();
        }
        else {
            consignmentgridview.Refresh();
        }
        ModalPopupEditInventory.Hide();
        alert(result);
    });
}
function RemoveSerial(serialId, remove) {
    var data = {
        OrderProcessDetailId: ordersProcessDetailId,
        Quantity: 1,
        serialId: serialId,
        wastedReturn: $("#WastedReturn :checked").val()
    };
    LoadingPanel.Show();
    $.post("/SalesOrders/UpdateOrderProcessDetail", data, function (result) {
        LoadingPanel.Hide();
        ModalPopupEditDelivery.Hide();
        var value = $("#inventoryTransactionType").val();
        if (value == "1" || value == 1) {
            deliveriesgridview.Refresh();
        }
        else {
            consignmentgridview.Refresh();
        }
        ModalPopupEditSerializedProduct.Hide();

        alert(result);
    });
}
function InventoryTransactionBeginCallBack(s, e) {
    e.customArgs["OrderprocessDetailId"] = ordersProcessDetailId;
    e.customArgs["type"] = type;
}

function UpdateRemoveDetails(id) {
    $("#InventoryTransactionId").val(id);
    SaveOrderProcessDetail();
}

var ordersIdEmail;
var invoicemasterIds;
var templateId;
function showEmails(orderid, templateIds) {
    ordersIdEmail = orderid;
    templateId = templateIds;
    ModelShowEmail.Show();
}
function beginshowemailCallBack(s, e) {
    e.customArgs["orderId"] = ordersIdEmail;
    e.customArgs["TemplateId"] = templateId;
    if (invoicemasterIds !== undefined) {
        e.customArgs["InvoiceMasterId"] = invoicemasterIds;
    }
}

function showEmailsInvoice(invoicemasterId) {
    invoicemasterIds = invoicemasterId;
    ModelShowEmail.Show();
}

function ApproveOrder(orderId) {
    var data = { OrderID: orderId, AuthorisedNotes: "TODO:", UnAuthorise: false };
    if (confirm("Are you sure to authorise this sales order?")) {
        $.post("/Order/AuthoriseOrder/",
            data,
            function (result) {
                if (result === 2) {
                    _SalesOrderListGridView_Active.Refresh();
                }
                else {
                    AwaitingAuthGridview.Refresh();
                }
            });
        return;
    }
}

function EditAwOrder(orderId) {
    location.href = "/DirectSales/EditDirectSales/" + orderId;
}

function UnathorizeOrder(orderId) {
    var data = { OrderID: orderId, AuthorisedNotes: "TODO:", UnAuthorise: true };
    if (confirm("Are you sure to unauthorise this sales order?")) {
        $.post("/Order/AuthoriseOrder/",
            data,
            function (result) {
                AwaitingAuthGridview.Refresh();
            });
        return;
    }
}
function UpdateDate(orderId) {
    $.post({
        url: "/Order/SyncDate",
        data: { "OrderId": orderId },
        success: function (e) {
            AwaitingAuthGridview.Refresh();
        }
    });
}

function PriceAnalysis() {
    var buyPrice = parseFloat($("#BuyPrice").val());
    var DirectSalestranstype = $("#InventoryTransactionTypeId").val();
    if (percentageValue === undefined || percentageValue === null || percentageValue === "") {
        percentageValue = $("#PercentMargin").val();
        //perentageValue = 0;
    }
    if (DirectSalestranstype == 1) { return; }

    if (DirectSalestranstype == "15" || DirectSalestranstype == "100") {
        var PriceValue = InvoiceProductPrice.GetValue();
    }
    else {
        PriceValue = Price.GetValue();
    }

    var profitMargin = PriceValue - buyPrice;
    var profitPercentage = (profitMargin / PriceValue) * 100;
    profitMargin = profitMargin.toFixed(2);

    if (profitPercentage != null) {
        profitPercentage = profitPercentage.toFixed();
    }

    if (profitPercentage <= 0) {
    }
    else {
        profitPercentage = +profitPercentage + +percentageValue;
    }

    if (profitMargin == "NaN" || profitPercentage == "NaN" || PriceValue == null || profitPercentage == null || buyPrice == null) {
        $(".financialAnalysisMainDiv").hide();
        return;
    }

    if (profitPercentage >= percentageValue) {
        priceAnalysisColorChange('green', 'white', profitPercentage, profitMargin);
    }
    else if (profitPercentage >= 0 && profitPercentage <= percentageValue) {
        priceAnalysisColorChange('yellow', 'red', profitPercentage, profitMargin);
    }
    else {
        priceAnalysisColorChange('red', 'white', profitPercentage, profitMargin);
    }
}

function priceAnalysisColorChange(backgroundColor, fontColor, profitPercentage, profitMargin) {
    $(".financialAnalysisMainDiv").show();
    $("#profitPercentage").val(profitPercentage + ' %');
    $("#profitPercentage").css("border-color", backgroundColor);
    $("#profitPercentage").css("background-color", backgroundColor);
    $("#profitPercentage").css("color", fontColor);

    $("#profitMargin").val(profitMargin);
    $("#profitMargin").css("border-color", backgroundColor);
    $("#profitMargin").css("background-color", backgroundColor);
    $("#profitMargin").css("color", fontColor);
}

function IsAllowZeroSale() {
    var ProductId = prdid.GetValue();
    var allowZPrice = true;
    var DirectSalestranstype = $("#InventoryTransactionTypeId").val();
    var margin = parseFloat($("#PerMargin").val());
    var percentMargin = parseFloat($('#PercentMargin').val());
    var buyPrice = parseFloat($("#BuyPrice").val());
    if (DirectSalestranstype == "15" || DirectSalestranstype == "100") {
        var PriceValue = InvoiceProductPrice.GetValue();
    }
    else {
        PriceValue = Price.GetValue();
    }
    var inventoryTypes = $('#inventoryType').val();

    if (inventoryTypes !== "PurchaseOrder") {
        LoadingPanel.Show();
        $.ajax({
            type: "GET",
            url: "/OrderDetail/IsAllowZeroSale/",
            data: { productid: ProductId },
            dataType: 'json',
            success: function (data) {
                LoadingPanel.Hide();
                pricealert = data.ShowPriceAlert;
                if (PriceValue <= 0) {
                    if (!data.zeroprice) {
                        allowZPrice = false;
                        alert("0 price is not allowed for this product")
                        if (DirectSalestranstype == "15" || DirectSalestranstype == "100") {
                            InvoiceProductPrice.SetValue("");
                        }
                        else {
                            Price.SetValue("");
                        }
                    }
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert('Error' + textStatus + "/" + errorThrown);
            }
        });
    }

    if (inventoryTypes !== "PurchaseOrder") {
        if (!allowZPrice || PriceValue > 0) {
            if ((buyPrice > PriceValue) && pricealert) {
                alert("Selling price is less than buying price " + buyPrice);
            }
        }
    }

    if (inventoryTypes != "PurchaseOrder") {
        // price analysis
        PriceAnalysis();
    }
}

function suggestedPrice() {
    var buyPrice = 0;
    var margin = 0;
    var landedCost = 0;
    var sellPrice = 0;

    if (BuyPrice.GetValue() !== null) {
        buyPrice = parseFloat(BuyPrice.GetValue());
    }
    else {
        buyPrice = 0;
    }

    if ($("#PercentMargin").val() !== null) {
        margin = parseFloat($("#PercentMargin").val());
    }
    else {
        margin = 0;
    }

    if (LandedCost.GetValue() != null) {
        landedCost = parseFloat(LandedCost.GetValue());
    }
    else {
        landedCost = 0;
    }

    if (SellPrice.GetValue() !== null) {
        sellPrice = parseFloat(SellPrice.GetValue());
    }
    else {
        sellPrice = 0;
    }

    var suggestedSellPrice = 0;

    if (buyPrice == null || buyPrice == 0) {
        $("#alertDivBP").show();
    }
    else if (sellPrice == null || sellPrice == 0) {
        $("#alertDivBP").hide();
    }
    var undefined = true;
    if (margin == null || margin == 0 || isNaN(margin) === true) {
        margin = 0;
        $("#alertDivPM").show();
    }
    else if (margin != null || margin != 0) {
        $("#alertDivPM").hide();
    }

    if (landedCost == null || landedCost == 0) {
        $("#alertDivLC").show();
    }
    else if (landedCost != null || landedCost != 0) {
        $("#alertDivLC").hide();
    }

    if ((buyPrice != null) && (margin != null) && (landedCost != null)) {
        suggestedSellPrice = (((buyPrice * margin) / 100) + buyPrice + landedCost);
        suggestedSellPrice = suggestedSellPrice.toFixed(2);
        $("#suggestedDiv").show();
        $("#suggestedSellPrice").val(suggestedSellPrice);
    }
    else {
        $("#suggestedDiv").hide();
    }
    if (SellPrice.GetValue() == null || SellPrice.GetValue() == 0) {
        SellPrice.SetValue(suggestedSellPrice);
    }
}

function setPrice() {
    var suggestedSellPrice = $("#suggestedSellPrice").val();
    SellPrice.SetValue(suggestedSellPrice);
}