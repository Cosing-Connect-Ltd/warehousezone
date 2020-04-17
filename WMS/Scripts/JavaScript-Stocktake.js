//JS Methods
var deleteSelectedStockTakeDetails = function () {
    var stockTakeDetailId = $("#divStockTakeDetailDeleteConfirm").attr("data-id");
    var data = { StockTakeDetailId: stockTakeDetailId, WarehouseId: $("#StocktakeCurrentWarehouseId").val() };
    $.post('/api/stocktake/stockdetail-archive',
        data,
        function (result) {
            StocktakeCurrentScans.Refresh();
            StockTakeDetailDeleteConfirm.Hide();
        });
}
var bindAdjustStockSerialEvents = function (s, e) {

    $("#btnConfirmAdjustStockSerials").on("click", function () {
        InventorySerialAdjustPopup.Hide();
    });
    $("#btnConfirmAdjustStockSerialsCancel").on("click", function () {
        InventorySerialAdjustPopup.Hide();
        $("#scanSerialsRowExistingSerials").html("");
    });

    $("#txtInventoryStockSerials").on("keypress",
        function (e) {
            if (e.which == 13) {
                if ($(this).val().length < 1) return false;
                $("#divExistingScannedSerialsError").slideUp();
                if ($("#scanSerialsRowExistingSerials input[type=hidden][name*=SerialItems][value='" +
                    $(this).val() +
                    "']").length >
                    0) {
                    var errorMsg = "Serial Number " + $(this).val() + " is already scanned.";
                    $("#divExistingScannedSerialsError").html(errorMsg);
                    $("#divExistingScannedSerialsError").slideDown();
                    $(this).val("");
                    $(this).focus();
                    return false;
                }
                var inStock =
                    ($("#scanSerialsRowExistingSerials input[type=hidden][value=" + $(this).val() + "]").length > 0
                        ? "<label class='alert alert-success'>Yes</label>"
                        : "<label class='alert alert-danger'>No</label>");
                $("#divExistingScannedSerialsTable")
                    .append("<tr><td>" + $(this).val() + "</td><td>" + inStock + "</td></tr>");
                var index = $("#divExistingScannedSerialsTable tr").length - 1;


                $("#scanSerialsRowExistingSerials")
                    .append("<input type='hidden' name='SerialItems[" +
                        index +
                        "]' value='" +
                        $(this).val() +
                        "'/>");
                $(this).val("");
                $(this).focus();
            }
        });
};

var bindStockTakeSerialPopupEvents = function (s, e) {

    $("#txtBarcodeScannerInputs").focus();

    $("#txtBarcodeScannerSerialInputs").on("keypress",
        function (e) {
            if (e.which == 13) {
                var currentStockTakeId = $("#CurrentStocktakeId").val();
                var currentSerial = $(this).val();
                $("#divStockTakeBarcodeErrors").html("");
                $("#txtBarcodeScannerSerialInputs").val("");
                var currentProductCode = $("#CurrentSerialisedProductCode").val();
                if (currentProductCode == currentSerial) {
                    $("#divProductSerialsInputContainer").css("background-color", "#ea3939");
                    return false;
                } else {
                    $("#divProductSerialsInputContainer").css("background-color", "#043304");
                }

                var data = {
                    StockTakeId: currentStockTakeId,
                    ProductSerial: currentSerial,
                    ProductCode: currentProductCode,
                    AuthUserId: $("#StocktakeCurrentUserId").val(),
                    WarehouseId: $("#StocktakeCurrentWarehouseId").val(),
                    CurrentTenantId: $("#StocktakeCurrentTenantId").val(),
                    LocationCode: $("#txtBarcodeLocation").val()
                };
                $.post('/api/stocktake/record-stockscan/',
                    data,
                    function (result) {
                        updateScannedStockTakes(result);
                    });
            };
        });
    stockTakeSerialScanEndCallback(s, e);
}

var bindStockTakePalletSerialPopupEvents = function (s, e) {

    $("#txtBarcodeScannerInputs").focus();

    $("#txtBarcodeScannerPalletsInputs").on("keypress",
        function (e) {
            if (e.which == 13) {
                var currentStockTakeId = $("#CurrentStocktakeId").val();
                var currentPalletSerial = $(this).val();
                $("#txtBarcodeScannerPalletsInputs").val("");
                var currentProductCode = $("#CurrentSerialisedProductCode").val();
                if (currentProductCode == currentPalletSerial) {
                    $("#divProductSerialsInputContainer").css("background-color", "#ea3939");
                    return false;
                } else {
                    $("#divProductSerialsInputContainer").css("background-color", "#043304");
                }

                var data = {
                    StockTakeId: currentStockTakeId,
                    PalletSerial: currentPalletSerial,
                    ProductCode: currentProductCode,
                    AuthUserId: $("#StocktakeCurrentUserId").val(),
                    WarehouseId: $("#StocktakeCurrentWarehouseId").val(),
                    CurrentTenantId: $("#StocktakeCurrentTenantId").val(),
                    LocationCode: $("#txtBarcodeLocation").val()
                };
                $.post('/api/stocktake/record-stockscan/',
                    data,
                    function (result) {

                        updateScannedStockTakes(result);
                    });
            };
        });
    stockTakeSerialScanEndCallback(s, e);
}

var bindStockTakeGridEvents = function (s, e) {

    $(".stocktake-delete .fa-trash").on("click",
        function () {
            var detailId = $(this).closest(".stocktake-delete").data("id");
            $("#divStockTakeDetailDeleteConfirm").attr("data-id", detailId);
            StockTakeDetailDeleteConfirm.Show();
        });

    var updateStockDetailQuantity = function (detialId, newQuantity) {

        var data = { StockTakeDetailId: detialId, NewQuantity: newQuantity };
        $.post('/api/stocktake/stockdetail-updatequantity',
            data,
            function (result) {
                $("#divStocktakeEditcontent").slideUp();
            });
    }

    $(".stocktake-editcontent #btnUpdateQtyConfirm").on("click",
        function () {
            var stockDetailId = $(this).closest("td").closest("tr").prev().data("id");

            var newQty = $("#txt-stocktake-change-qty").val();
            $(this).closest("td").closest("tr").prev().attr("data-qty", newQty);
            $(this).closest("td").closest("tr").prev().find("td:eq(3)").html(newQty);

            updateStockDetailQuantity(stockDetailId, newQty);

            $("#divStocktakeEditcontent").slideUp();
            $("#divStocktakeEditcontent").insertAfter($("#divProductSerialsInputContainer"));
            $("#stocktakeTempUpdateRow").remove();
        });
}


var loadProductCreateForm = function (code, serialised) {

    var isSerialised = serialised || $("#CodeTypeSerial:checked").length > 0;
    var isProductSku = code || $("#CodeTypeProduct:checked").length > 0;
    var isBarcode = code || $("#CodeTypeBarcode:checked").length > 0;
    var isOuterBarcode = code || $("#CodeTypeOuterBarcode:checked").length > 0;
    var isPalletSerial = code || $("#CodeTypePalletSerial:checked").length > 0;

    var productCode = $("#CodeTypeProductCode").val();
    var productName = $("#CodeTypeProductName").val();
    if (productName == 'undefined') {
        productName = '';
    }

    // TODO: chnage products list to first option when loading product create form
    $("#txt-stocktake-pallet-qty").val("1");
    $('#ProductId').find('option:first').attr('selected', 'selected');
    $("#ProductId").trigger("chosen:updated");

    $("#divStocktakeProductCreate").removeClass("visibility-hidden");
    $("#product-list").addClass("visibility-hidden");
    $("#divStockTakeBarcodeErrors").html("");

    if (isSerialised) {
        $("#product-list").removeClass("visibility-hidden");
        $("#stocktake-create-product-nonserial").slideUp();
        $("#divStockTakePalletSerialCreate").slideUp();
        $("#product-list").css("visibility", "visible");
        $("#Product-serials-pallets").slideDown();
        $("#divStockTakeProductSerialCreate").slideDown();
        $("#txt-stocktake-product-serial").val(productCode);
        $("#txtBarcodeScannerInputs").val(productCode);
        $("#txt-stocktake-product-serial").attr("disabled", "disabled");
    }
    else if (isPalletSerial) {
        $("#product-list").removeClass("visibility-hidden");
        $("#stocktake-create-product-nonserial").slideUp();
        $("#divStockTakeProductSerialCreate").slideUp();
        $("#product-list").css("visibility", "visible");
        $("#Product-serials-pallets").slideDown();
        $("#divStockTakePalletSerialCreate").slideDown();
        $("#txtBarcodeScannerInputs").val(productCode);
        $("#txt-stocktake-pallet-serial").val(productCode);
        $("#txt-stocktake-pallet-serial").prop('disabled', true);
    }
    else if (isProductSku) {
        $("#stocktake-create-product-nonserial").slideDown();
        $("#Product-serials-pallets").slideUp();
        $("#txtBarcodeScannerInputs").val(productCode);
        $("#txt-stocktake-product-code").val(productCode);
        $("#txt-stocktake-product-code").prop('disabled', true);
    }

    else if (isBarcode) {
        $("#stocktake-create-product-nonserial").slideDown();
        $("#Product-serials-pallets").slideUp();
        $("#txtBarcodeScannerInputs").val(productCode);
        $("#txt-stocktake-product-barcode").val(productCode);
        $("#txt-stocktake-product-barcode").prop('disabled', true);
    }
    else if (isOuterBarcode) {
        $("#stocktake-create-product-nonserial").slideDown();
        $("#Product-serials-pallets").slideUp();
        $("#txtBarcodeScannerInputs").val(productCode);
        $("#txt-stocktake-product-outer-barcode").val(productCode);
        $("#txt-stocktake-product-outer-barcode").prop('disabled', true);
    }
}

$(document).ready(function () {
    $(".stockMovement").unbind().on("submit", function () {
        debugger;
        var bulk = $("#BulkStock").val();
        if (bulk) {
            if ($("#StockMovementList").length > 0)
            {
                if (StockMovementList.cpRowCount > 0)
                {
                    Qty.removeClass("invalid").addClass("valid");
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        else {
            var qunatity = Qty.GetValue();
            if (qunatity == null || qunatity <= 0) {
                alert("Quantity is required");
                return false;
            }
        }

    });



    $(".stocktake-delete").click(function () {
        var id = $(this).attr('data-id');
        if (id !== 0 && id !== "" && id !== undefined && id !== null) {
            id = parseInt(id);
            var res = confirm("Are you sure you want to delete this recored");
            if (res) {
                LoadingPanel.Show();
                $.ajax({
                    type: "GET",
                    url: "/stocktakes/delete/",
                    data: { id: id, },
                    dataType: 'json',

                    success: function (data) {
                        if (data) {
                            LoadingPanel.Hide();
                            StocktakeGridDetail.Refresh();
                            var info = "Recored Deleted!";
                            $("#infoMsg").html(info).show();
                            $('#infoMsg').delay(2000).fadeOut();
                        } else {
                            LoadingPanel.Hide();
                            alert("Id not found");
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert('Error' + textStatus + "/" + errorThrown);
                    }
                });
            }
        }
    });
    $("#divStocktakeEditcontent  #btnUpdateQtyCancel").on("click", function () {
        $("#divStocktakeEditcontent").slideUp();
        $("#divStocktakeEditcontent").insertAfter($("#divProductSerialsInputContainer"));
        $("#stocktakeTempUpdateRow").remove();
    });


    $("#btnProductCreateCancel").on("click", function () {

        $('#divStocktakeProductCreate').find('input').val('');
        $('#divStocktakeProductCreate').find('input').prop('disabled', false);
        $("#divStocktakeProductCreate").addClass("visibility-hidden");
        $("#product-list").css("visibility", "hidden");
        $("#txtBarcodeScannerInputs").val("");
        $("#txtBarcodeScannerInputs").focus();

    });

    $("#chkStockTakeProductSerialisedCreate").on("change",
        function () {
            if ($("#chkStockTakeProductSerialisedCreate").prop('checked')) {
                $("#divSerialisedProductCreateMessage").slideDown();
            } else {
                $("#divSerialisedProductCreateMessage").slideUp();
            }
        });


    $("#btnProductCreateConfirm").on("click", function () {

        $("#divStockTakeBarcodeErrors").html("");

        var productName = $("#txt-stocktake-product-name").val();
        var productCode = $("#txt-stocktake-product-code").val();
        var productSerial = $("#txt-stocktake-product-serial").val();
        var palletSerial = $("#txt-stocktake-pallet-serial").val();
        var productBarcode = $("#txt-stocktake-product-barcode").val();
        var productBarcode2 = $("#txt-stocktake-product-outer-barcode").val();

        var data = {
            ProductCode: productCode,
            NewProductName: productName,
            IsSerialised: $("#chkStockTakeProductSerialisedCreate").prop('checked'),
            ProductSerial: productSerial,
            PalletSerial: palletSerial,
            WarehouseId: $("#StocktakeCurrentWarehouseId").val(),
            CurrentTenantId: $("#StocktakeCurrentTenantId").val(),
            AuthUserId: $("#StocktakeCurrentUserId").val(),
            StockTakeId: $("#CurrentStocktakeId").val(),
            NewProductBarcode: productBarcode,
            NewProductBarcode2: productBarcode2,
            NotExistingItem: true,
            IsProcessByPallet: $("#chkStockTakeProductByPalletCreate").prop('checked'),
            ProductId: $("#ProductId").val(),
            ScannedQuantity: $("#txt-stocktake-pallet-qty").val(),
            LocationCode: $("#txtBarcodeLocation").val()

        };
        $.post('/api/stocktake/record-stockscan/',
            data,
            function (result) {
                if (result.Response.Success) {
                    var currentStockTakeId = $("#CurrentStocktakeId").val();

                    $("#divStocktakeProductCreate").addClass("visibility-hidden");
                    $("#product-list").addClass("visibility-hidden");
                    $('#divStocktakeProductCreate').find('input').val('');
                    $('#divStocktakeProductCreate').find('input').prop('disabled', false);
                    $("#txtBarcodeScannerInputs").focus();
                    $("#txtBarcodeScannerInputs").val(productCode);

                    if (result.SerialRequired) {
                        $("#txtBarcodeScannerInputs").val(result.ProductCode);
                        $("#txtBarcodeScannerInputs").attr("disabled", "disabled");
                        $("#CurrentSerialisedProductId").val(result.ProductId);
                        $("#divProductSerialsInputContainer").slideDown();
                        $("#txtBarcodeScannerSerialInputs").focus();
                    }
                    else if (result.PalletSerialRequired) {

                        $("#txtBarcodeScannerInputs").val(result.ProductCode);
                        $("#txtBarcodeScannerInputs").attr("disabled", "disabled");
                        $("#CurrentSerialisedProductId").val(result.ProductId);
                        $("#divPalletSerialsInputContainer").slideDown();
                        $("#txtBarcodeScannerPalletsInputs").focus();
                    }
                    else {
                        updateScannedStockTakes(result);
                        $("#txtBarcodeScannerInputs").focus();
                    }
                }
                else {
                    $("#divStockTakeBarcodeErrors").html(result.Response.FailureMessage);
                    $("#divStockTakeBarcodeErrors").slideDown();
                }
            });
    });

    $("#txtBarcodeScannerInputs").on("keypress", function (e) {

        if (e.which == 13) {
            $("#divStockTakeBarcodeErrors").html("");
            $('#divStocktakeProductCreate').find('input').val('');
            $('#divStocktakeProductCreate').find('input').prop('disabled', false);
            $("#divStocktakeProductCreate").addClass("visibility-hidden");

            var currentCode = $(this).val();

            barcodeScannedForStockTake(currentCode);
        }
    });
});

var logAfterProductCreate = function (currentStockTakeId, productcode, serialcode) {
    var data = {
        StockTakeId: currentStockTakeId,
        ProductSerial: serialcode,
        ProductCode: productcode,
        AuthUserId: $("#StocktakeCurrentUserId").val(),
        WarehouseId: $("#StocktakeCurrentWarehouseId").val(),
        CurrentTenantId: $("#StocktakeCurrentTenantId").val(),
        LocationCode: $("#txtBarcodeLocation").val()
    };
    $.post('/api/stocktake/record-stockscan/',
        data,
        function (result) {
            updateScannedStockTakes(result);
            if (result.SerialRequired) {
                $("#txtBarcodeScannerSerialInputs").focus();
            } else {
                $("#txtBarcodeScannerInputs").focus();
            }
        });
}

var updateScannedStockTakes = function (data) {
    if (data.Response.Success) {
        if (!data.SerialRequired) {
            $("#txtBarcodeScannerInputs").val("");
        } else {
            $("#txtBarcodeScannerInputs").val(data.ProductCode);
        }
        $("#CurrentSerialisedProductCode").val(data.ProductCode);

        $("#divStockTakeBarcodeErrors").html("");
        var $scanRecord = $("<tr data-id='" + data.StockTakeDetailId + "' data-qty='1'>");
        $scanRecord.append("<td>" + data.ProductCode + "</td>");
        $scanRecord.append("<td>" + data.ProductName + "</td>");

        if (data.ProductSerial != null) {
            $scanRecord.append("<td>" + data.ProductSerial + "</td>");
        }
        else if (data.PalletSerial != null) {
            $scanRecord.append("<td>" + data.PalletSerial + " (Pallet) </td>");
        }
        else {
            $scanRecord.append("<td></td>");
        }
        $scanRecord.append("<td>" + data.ScannedQuantity + "</td>");
        if (data.BatchNumber === "" || data.BatchNumber === null) {
            $scanRecord.append("<td></td>");
        }
        else {
            $scanRecord.append("<td>" + data.BatchNumber + "</td>");

        }
        if (data.ExpiryDate === "" || data.ExpiryDate === null) {
            $scanRecord.append("<td></td>");
        }
        else {

            $scanRecord.append("<td>" + data.ExpiryDate + "</td>");
        }

        if (!data.SerialRequired || !data.PalletSerialRequired) {
            var $editCell = $("<td><i class='fa fa-pencil stocktake-glyph-icons'></i></td>");
            $scanRecord.append($editCell);

            $editCell.on("click", function () {
                var $tempRow = $("<tr id='stocktakeTempUpdateRow'></tr>");
                var $tempCell = $("<td colspan=5></td>");
                $("#divStocktakeEditcontent").appendTo($tempCell);
                $tempRow.append($tempCell);

                $tempRow.insertAfter($(this).closest("tr"));
                var qty = $(this).closest("tr").attr("data-qty");
                if (qty == null) {
                    qty = 1;
                }
                $("#divStocktakeEditcontent #txt-stocktake-change-qty").val(qty);

                $("#divStocktakeEditcontent").slideDown();
            });
        } else {
            $scanRecord.append("<td></td>");
        }

        $("#tblBarcodeScannedStocks tbody").prepend($scanRecord);
        $("#divStockTakeBarcodeErrors").slideUp();

    } else {
        if (!data.SerialRequired) {
            $("#txtBarcodeScannerInputs").val("");
        }

        $("#divStockTakeBarcodeErrors").html(data.Response.FailureMessage);
        if (data.Response.ProductDontExist) {
            if (data.Response.SerialInsteadProduct) {
                loadProductCreateForm(data.ProductCode, data.ProductSerial);
            } else {
                var serialChecked = (data.ProductCode != null && data.ProductSerial != null);
                var productChecked = !serialChecked;
                var barcodeChecked = false;
                var outerBarcodeChecked = false;
                var PalletSerialChecked = false;
                $("#divStockTakeBarcodeErrors").append("<br/><b>Is this? <label class='radio-inline'><input type='radio' name='CodeType' "
                    + (productChecked ? "checked='checked'" : "") + " id='CodeTypeProduct'><b>Product SKU</b></label>       <label class='radio-inline'><input type='radio' name='CodeType' "
                    + (barcodeChecked ? "checked='checked'" : "") + " id='CodeTypeBarcode'><b>Barcode</b></label>    <label class='radio-inline'><input type='radio' name='CodeType' "
                    + (outerBarcodeChecked ? "checked='checked'" : "") + " id='CodeTypeOuterBarcode'><b>Outer Barcode</b></label>        <label class='radio-inline'><input type='radio' name='CodeType' " + (serialChecked ? "checked='checked'" : "")
                    + " id='CodeTypeSerial'><b>Serial Code</b></label>    <label class='radio-inline'><input type='radio' name='CodeType' "
                    + (PalletSerialChecked ? "checked='checked'" : "") + " id='CodeTypePalletSerial'><b>Pallet Serial</b></label></b><br/>");
                $("#divStockTakeBarcodeErrors").append("<br/><a onclick=\"loadProductCreateForm()\" href='javascript:void(0)' class='btn btn-primary'>Add Product</a><a class='btn btn-warning' onclick='backToProductCodeScan()'>Cancel</a><input type='hidden' id='CodeTypeProductCode' value='"
                    + data.ProductCode + "'/><input type='hidden' id='CodeTypeSerialCode' value='" + data.ProductSerial + "'/><input type='hidden' id='CodeTypeProductName' value='" + data.ProductName + "'/>");

            }
        }
        $("#divStockTakeBarcodeErrors").slideDown();
    }

    if (data.Response.MoveToNextProduct) {
        backToProductCodeScan();
    }

    if ($("#tblBarcodeScannedStocks tr").length > 1) {
        $("#tblBarcodeScannedStocks").slideDown();
    } else {
        $("#tblBarcodeScannedStocks").slideUp();
    }
}

var addProductManually = function () {
    var productCode = $("#txtBarcodeScannerInputs").val().trim();
    if (productCode.length < 1) {
        $("#divStockTakeBarcodeErrors").html("Please scan/enter the Product Code and press enter before adding new product.");
        $("#divStockTakeBarcodeErrors").slideDown();
    } else {
        loadProductCreateForm(productCode);
    }
}

var backToProductCodeScan = function (s, e) {
    $("#divStockTakeBarcodeErrors").html("");
    $("#txtBarcodeScannerSerialInputs").html("");
    $("#txtBarcodeScannerPalletsInputs").html("");
    $("#divbatchInputContainer").slideUp();
    $("#divProductSerialsInputContainer").slideUp();
    $("#divPalletSerialsInputContainer").slideUp();
    $("#txtBarcodeScannerInputs").val("");
    $("#txtBarcodeScannerInputs").removeAttr("disabled");
    $("#txtBarcodeScannerInputs").focus();
    $("#txtBatchNumber").val("");
    $("#txtBatchNumber").val("");
    $("#txtquantity").val(1);
}

var barcodeScannedForStockTake = function (code) {
    $("#CurrentSerialisedProductCode").val(code);
    $("#txtBarcodeScannerInputs").val("");
    var currentStockTakeId = $("#CurrentStocktakeId").val();

    if (code == null || code.length < 1) return;

    gridLookupStockTakeProducts.Clear();
    gridLookupStockTakeProducts.HideDropDown();

    var data = {
        StockTakeId: currentStockTakeId,
        ProductCode: code,
        AuthUserId: $("#StocktakeCurrentUserId").val(),
        WarehouseId: $("#StocktakeCurrentWarehouseId").val(),
        CurrentTenantId: $("#StocktakeCurrentTenantId").val(),
        LocationCode: $("#txtBarcodeLocation").val()
    };

    $.post('/api/stocktake/record-stockscan/',
        data,
        function (result) {

            if (result.SerialRequired) {
                if (result.Response.SerialInsteadProduct) {
                    updateScannedStockTakes(result);
                }
                $("#txtBarcodeScannerInputs").val(result.ProductCode);
                $("#txtBarcodeScannerInputs").attr("disabled", "disabled");

                $("#CurrentSerialisedProductId").val(result.ProductId);
                $("#divProductSerialsInputContainer").slideDown();
                $("#txtBarcodeScannerSerialInputs").focus();
                $("#chkStockTakeProductSerialisedCreate").prop('checked', true);

            }
            else if (result.PalletSerialRequired) {

                $("#txtBarcodeScannerInputs").val(result.ProductCode);
                $("#txtBarcodeScannerInputs").attr("disabled", "disabled");

                $("#CurrentSerialisedProductId").val(result.ProductId);
                $("#divPalletSerialsInputContainer").slideDown();
                $("#txtBarcodeScannerPalletsInputs").focus();
                $("#chkStockTakebatchCreate").prop('checked', true);

            }
            else if (result.BatchRequired) {
                $("#txtBarcodeScannerInputs").val(result.ProductCode);
                $("#txtBarcodeScannerInputs").attr("disabled", "disabled");
                $("#txtquantity").val(result.ScannedQuantity)
                $("#CurrentSerialisedProductId").val(result.ProductId);
                $("#divbatchInputContainer").slideDown();
                $("#txtBatchNumber").focus();
                $("#chkStockTakeProductSerialisedCreate").prop('checked', true);

            }


            else {
                updateScannedStockTakes(result);
            }
        });
}


function BindStockBatchNumber() {

    $("#txtBarcodeScannerInputs").focus();

    var currentStockTakeId = $("#CurrentStocktakeId").val();
    var currentProductCode = $("#CurrentSerialisedProductCode").val();
    var ScannedQuantity = $("#txtquantity").val();

    var ExpiryDate = ProcessExpiryDate1.GetFormattedText("MM/dd/yyyy");
    var BatchNumber = $("#txtBatchNumber").val();
    var today = new Date();
    var exdate = ProcessExpiryDate1.GetDate();
    if (BatchNumber === null || BatchNumber === "") {
        alert("Batch number must have some values");
        return;

    }
    if (exdate <= today) {
        alert("Expiry date should be greater than today's date");
        return;
    }

    if (ScannedQuantity === null || ScannedQuantity === "") {

        alert("Scanned Quantity must be greater than zero");
        return;
    }


    var data = {
        StockTakeId: currentStockTakeId,
        ScannedQuantity: ScannedQuantity,
        ExpiryDate: ExpiryDate,
        ProductCode: currentProductCode,
        BatchNumber: BatchNumber,
        BatchRequired: true,
        AuthUserId: $("#StocktakeCurrentUserId").val(),
        WarehouseId: $("#StocktakeCurrentWarehouseId").val(),
        CurrentTenantId: $("#StocktakeCurrentTenantId").val(),
        LocationCode: $("#txtBarcodeLocation").val()
    };
    $.post('/api/stocktake/record-stockscan/',
        data,
        function (result) {
            $("#divbatchInputContainer").slideUp();
            $("#txtBarcodeScannerInputs").removeAttr("disabled");
            $("#txtBarcodeScannerInputs").focus();
            $("#txtBatchNumber").val("");

            StocktakeCurrentScans.Refresh();
            updateScannedStockTakes(result);
        });






}


//Control Callbacks

var getIdByName = function (inputName) {
    inputName = inputName.replace('[', '_');
    inputName = inputName.replace(']', '_');
    inputName = inputName.replace('.', '_');
    return inputName;
}

var getHiddenInputWithValue = function (inputName, value) {
    var $result = $("<input type='hidden' name='" + inputName + "'> id='" + getIdByName(inputName) + "'");
    return $result;
}

function stockTakeScanBeginCallback(s, e) {
    e.customArgs["id"] = $("#CurrentStocktakeId").val();
}


function stocktakeCurrentScansCallback(s, e) {
    bindStockTakeGridEvents();
    $("#txtBarcodeScannerInputs").val("");
    $("#txtBarcodeScannerInputs").focus();
}

function stockTakeSerialScanBeginCallback(s, e) {
    e.customArgs["productId"] = $("#CurrentSerialisedProductId").val();
    e.customArgs["id"] = $("#CurrentStocktakeId").val();
}

function stockTakeSerialScanEndCallback(s, e) {
    $("#txtBarcodeScannerSerialInputs").val("");
    $("#txtBarcodeScannerSerialInputs").focus();
    // StocktakeCurrentSerialScans.Refresh();
}

var stockTakeProductLookupSelected = function (s, e) {
    var productCode = s.GetValue();
    barcodeScannedForStockTake(productCode);
}
function closeStockTakeProductLookup() {
    gridLookupStockTakeProducts.ConfirmCurrentSelection();
    gridLookupStockTakeProducts.HideDropDown();
}

function OnToggleSelectAllStockApplyChanges(isChecked) {
    var controls = ASPxClientControl.GetControlCollection();
    controls.ForEachControl(function (ss, ee) {
        if (ss.name.indexOf("InventoryStock_") >= 0) {
            if (isChecked && !ss.GetChecked()) {
                ss.SetChecked(true);
            }
            if (!isChecked && ss.GetChecked()) {
                ss.SetChecked(false);
            }
        }
    });
}


function OnStockMovementAdjust() {
    var formLocationId = $("#FromLocation").val();
    var toLocationId = $("#ToLocation").val();
    var productId = prdid.GetValue();
    var qty = Qty.GetValue();
    if (formLocationId <= 0 || toLocationId <= 0 || productId <= 0 || productId == null || qty <= 0 || qty == null) {
        alert("All fields are mandatory");
        return;
    }
    stockmovement = {
        "FromLocation": formLocationId, "ToLocation": toLocationId, "Qty": qty, ProductId: productId
    }
    LoadingPanel.Show();
    $.ajax({
        type: "GET",
        url: "/InventoryStocks/SaveMoveStock/",
        data: stockmovement,
        success: function (data) {
            StockMovementList.Refresh();
            prdid.SetValue(null);
            Qty.SetValue(null);
            LoadingPanel.Hide();
            var info = "Stock Movement added!";
            $("#infoMsg").html(info).show();
            $('#infoMsg').delay(2000).fadeOut();

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });



}





