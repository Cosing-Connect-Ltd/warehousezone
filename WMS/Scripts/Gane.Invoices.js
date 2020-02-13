var updateAccountDetails = function () {
    PaymentToday.SetEnabled($("#AccountId").val() > 0);

    $.get('/Invoice/GetAccountDetails/' + $("#AccountId").val())
        .done(function (currentAccount) {
            $(".invoice-header-accountinfo strong[data-attr=InvoiceAddress]").html(currentAccount.AccountAddress);
            $(".invoice-header-accountinfo strong[data-attr=AccountCode]").html(currentAccount.AccountCode);
            $(".invoice-header-taxinfo strong[data-attr=InvoiceCurrency]").html(currentAccount.Currency);
            $(".invoice-header-taxinfo strong[data-attr=InvoiceDate]").html(currentAccount.InvoiceDate);
        });
}
var pricealert;
var updateProductPrice = function () {
    
    var model = { ProductId: prdid.GetValue(), AccountId: $("#AccountId").val() };

    if ((model.ProductId < 0) || model.AccountId < 0) return;
    

    $.get('/Invoice/GetProductPrice/', model)
        .done(function (data) {
            pricealert = data.ShowPriceAlert;
            var buyPrice = data.minimumSalePrice;
            var margin = data.PercentMargin;
            percentageValue = data.percentageValue;
            if ((buyPrice > data.SellPrice) && pricealert) {
                alert("Selling price should not be less than : " + margin);

            }
            //else if ((buyPrice > margin || data.SellPrice < margin) && pricealert) {
            //    alert("Selling price should not be less than " + margin);

            //}

            $("#BuyPrice").val(buyPrice);
            PriceAnalysis();
            InvoiceProductPrice.SetValue(data.SellPrice);

            if (!data.AllowModifyPrice) {
                InvoiceProductPrice.GetInputElement().readOnly = true;
            }
            if (data.EnableTax) {
                $("#TaxId").val(data.ProductTaxID);
                $("#TaxId").trigger("chosen:updated");
            }
            else {
                $("#TaxId").val(0);
                $("#TaxId").trigger("chosen:updated");
            }
            if (data.EnableWarranty) {
                $(".WarrantyDetail").show();

            }
            else {
                $(".WarrantyDetail").hide();
                $("#WarrantyId").val(0);
                $("#WarrantyId").trigger("chosen:updated");
                
            }



            updateAddItemButtonText();
        });
}

var GProductId = 0;
var isEdit = false;
var ProductIdEdit;
var updateAddItemButtonText = function () {
    if ($("#invoice-products-table tr[data-id=" + ProductIdEdit + "]").length > 0) {
        $(".btn-invoice-add-product span").text('Update Item');
        isEdit = true;
    }

    else if ($("#invoice-products-table tr[data-id=" + GProductId + "]").length > 0) {
        $(".btn-invoice-add-product span").text('Update Item');
        isEdit = true;
    }
    else {
        $(".btn-invoice-add-product span").text('Add Item');
        isEdit = false;
    }
}


var updateInvoiceRowHiddenValues = function (index, productId, qtyProcessed, taxTotal, netTotal, warrantyAmt, price, warrantyId, taxId, discount) {
    $("#frmDirectCreate input[type=hidden][id^=AllInvoiceProducts" + index).remove();
    
    var result = "<input type='hidden' id='AllInvoiceProducts" + index + "_ProductId' name='AllInvoiceProducts[" + index + "].ProductId' value='" + productId + "' />" +
        "<input type='hidden' id='AllInvoiceProducts" + index + "_QtyProcessed' name='AllInvoiceProducts[" + index + "].QtyProcessed' value='" + qtyProcessed + "' />" +
        "<input type='hidden' id='AllInvoiceProducts" + index + "_NetAmount' name='AllInvoiceProducts[" + index + "].NetAmount' value='" + netTotal + "' />" +
        "<input type='hidden' id='AllInvoiceProducts" + index + "_Price' name='AllInvoiceProducts[" + index + "].Price' value='" + price + "' />" +
        "<input type='hidden' id='AllInvoiceProducts" + index + "_WarrantyAmount' name='AllInvoiceProducts[" + index + "].WarrantyAmount' value='" + warrantyAmt + "' />" +
        "<input type='hidden' id='AllInvoiceProducts" + index + "_WarrantyID' name='AllInvoiceProducts[" + index + "].WarrantyID' value='" + warrantyId + "' />" +
        "<input type='hidden' id='AllInvoiceProducts" + index + "_TaxAmounts' name='AllInvoiceProducts[" + index + "].TaxAmounts' value='" + taxTotal + "' />" +
        "<input type='hidden' id='AllInvoiceProducts" + index + "_TaxID' name='AllInvoiceProducts[" + index + "].TaxID' value='" + taxId + "' />" +
        "<input type='hidden' id='AllInvoiceProducts" + index + "_TaxAmountsInvoice' name='AllInvoiceProducts[" + index + "].TaxAmountsInvoice' value='" + taxTotal + "' />" +
        "<input type='hidden' id='AllInvoiceProducts" + index + "_TaxAmountsInvoice' name='AllInvoiceProducts[" + index + "].DiscountAmount' value='" + discount + "' />";

    return result;
}

var updateInvoiceDate = function () {

    var invoiceDate = InvoiceDate.GetFormattedText("dd/MM/yyyy");
    $(".invoice-header-taxinfo strong[data-attr=InvoiceDate]").html(invoiceDate);
    $("#frmDirectCreate #InvoiceDate").val(invoiceDate);
    $("#frmeditDsale #EditInvoiceDate").val(invoiceDate);
}


$(document).ready(function () {
    $("#AccountId").on("change",
        function () {

            updateAccountDetails();
            //updateProductPrice();
        });

    $("#ProductId").on("change",
        function () {
            updateProductPrice();

        });

    $(".btn-invoice-add-product").on("click", function () {
        if (isEdit) {
            updateProductDetails(ProductIdEdit);
        }
        else {
            addProductDetails();
        }
    });

    var exportInvoice = function () {
        var html = $("#invoice-preview-container").html();
        $("#frmPDFExporter #ExportHtml").val(escape(html));
        frmPDFExporter.submit();
        $("#dvbusy").hide();
        $("#infoMsg").hide();
    }

    $("#btn-export-invoice").on("click", function () {
        exportInvoice();
    });

    var $form = $("#frmDirectCreate");
    $form.submit(function (e) {
        
        var abc = $("#AccountId").val();
        var isValid = true;
        if ($("#AccountId").val() < 0.01) {
            var paid = PaymentToday.GetValue();
            var toPay = parseFloat($("#frmDirectCreate #InvoiceTotal").val()).toFixed(2);
            if (paid < toPay) {
                $("#ds-error-message").html("New Accounts must pay balance in Full.");
                $("#ds-error-message").slideDown();
                isValid = false;
            }
        }

        if (!isValid) {
            e.preventDefault();
            return false;
        }

        //frmDirectCreate.submit();
    });

    updateInvoiceTotals();
    updateInvoiceDate();
});

var getTaxAmountById = function (taxId, price, isPercent) {
    var tax = 0;
    var percent = 0;
    var taxInfo = jQuery.parseJSON($("#TaxDataHelper").val());
    for (var i = 0; i < taxInfo.length; i++) {
        if (taxInfo[i].TaxID === parseInt(taxId)) {
            percent = taxInfo[i].PercentageOfAmount;
            tax = (price / 100) * percent;
        }
    }
    if (isPercent) {
        return percent;
    } else {
        return tax;
    }
}

var getDiscountAmount = function (totalamount) {
    var value = DiscountBox.GetValue();
    var percent = (value / 100);
    var taxamount = (totalamount * percent);
    return taxamount.toFixed(2);
}

var getWarrantyAmountById = function (warrantyId, price) {
    var warranty = 0;
    var warrantyInfo = jQuery.parseJSON($("#WarrantyDataHelper").val());
    for (var i = 0; i < warrantyInfo.length; i++) {
        if (warrantyInfo[i].WarrantyID === parseInt(warrantyId)) {
            if (warrantyInfo[i].IsPercent) {
                warranty = (price / 100) * warrantyInfo[i].PercentageOfPrice;
            } else {
                warranty = warrantyInfo[i].FixedPrice;
            }
        }
    }

    return warranty;
}

var updateProductDetails = function (editPrdouctId) {
    
    var productId = GProductId;
    var productQty = InvoiceProductQty.GetValue();
    var productPrice = InvoiceProductPrice.GetValue();

    var productTaxId = $("#TaxId").val();

    var productWarrantyId = $("#WarrantyId").val();
    var buyPrice = $("#BuyPrice").val();
    var productTax = getTaxAmountById(productTaxId, productPrice) * productQty;
    var productTaxPc = getTaxAmountById(productTaxId, productPrice, true);
    var productWarranty = getWarrantyAmountById(productWarrantyId, productPrice) * productQty;
    var productName = prdid.GetText();
    var totalAmount = (productPrice * productQty);
    var DiscountAmount = getDiscountAmount(totalAmount);

    if (DiscountAmount > totalAmount) {

        return alert("Discount price is not greater than Total amount");
    }

    var netAmount = totalAmount + productTax + productWarranty;
    var net = totalAmount + productWarranty;
    var $table = $("#invoice-products-table");
    var rowIndex = 1;
    var $rowToReplace = null;
    if ($table.find("tr[data-id=" + editPrdouctId + "]").length > 0) {
        $rowToReplace = $table.find("tr[data-id=" + editPrdouctId + "]");
        rowIndex = $rowToReplace.index();
    }
    if (productQty == null) {
        return alert("Please enter quantity");
    }
    if (productPrice == null) {
        return alert("Please enter price");
    }
    formmodified = 1;

    var $row = $("<tr data-productname='" + productName + "' data-qty='" + productQty + "' data-buyprice='" + buyPrice + "' data-id='" + productId + "' data-warranty='" + productWarranty + "' data-unitprice='" + productPrice + "' data-taxid='" + productTaxId + "' data-warrid='" + productWarrantyId + "'   data-discount='" + DiscountAmount + "'   data-net='" + netAmount + "' data-tax='" + productTax.toFixed(2) + "' data-tax-pc='" + productTaxPc + "' data-netam='" + net + "'><td>" + productName + "</td><td>" + productPrice.toFixed(2) + "</td><td>" + productQty + "</td><td>" + productWarranty.toFixed(2) + "</td><td>" + productTax.toFixed(2) + " (" + productTaxPc.toFixed(2) + "%)</td><td>" + DiscountAmount + "(" + DiscountBox.GetValue().toFixed(2) + "%)</td><td>" + netAmount.toFixed(2) + "</td><td style='display:none;'>" + net.toFixed(2) + "</td><td><b class='btn btn-sm fa fa-trash'></b><b class='btn btn-sm fa fa-pencil'></b></td></tr>");
    updateRowActions($row);
    if ($rowToReplace != null) {
        rowIndex = $rowToReplace.index() - 1;
        $row.insertBefore($rowToReplace);
        $rowToReplace.remove();
    } else {
        $table.append($row);
        rowIndex = $row.index() - 1;
    }

    var hiddenValues = updateInvoiceRowHiddenValues(rowIndex, productId, productQty, productTax.toFixed(2), netAmount.toFixed(2), productWarranty.toFixed(2), productPrice, productWarrantyId, productTaxId, DiscountAmount, net.toFixed(2));

    $("#frmDirectCreate").append(hiddenValues);

    updateInvoiceTotals();
    DisableChosenDropdown("AccountId", true);
    isEdit = false;

}
var addProductDetails = function () {
    
    var productId = GProductId;
    var productQty = InvoiceProductQty.GetValue();
    var productPrice = InvoiceProductPrice.GetValue();

    var productTaxId = $("#TaxId").val();
    var productWarrantyId = $("#WarrantyId").val();
    var buyPrice = $("#BuyPrice").val();
    var productTax = getTaxAmountById(productTaxId, productPrice) * productQty;
    var productTaxPc = getTaxAmountById(productTaxId, productPrice, true);
    var productWarranty = getWarrantyAmountById(productWarrantyId, productPrice) * productQty;
    var productName = prdid.GetText();
    var totalAmount = (productPrice * productQty);
    var DiscountAmount = getDiscountAmount(totalAmount);

    if (DiscountAmount > totalAmount) {

        return alert("Discount price is not greater than Total amount");
    }

    var netAmount = totalAmount + productTax + productWarranty;
    var net = totalAmount + productWarranty;
    var $table = $("#invoice-products-table");
    var rowIndex = 1;
    var $rowToReplace = null;
    if ($table.find("tr[data-id=" + productId + "]").length > 0) {
        $rowToReplace = $table.find("tr[data-id=" + productId + "]");
        rowIndex = $rowToReplace.index();
    }
    if (productQty == null) {
        return alert("Please enter quantity");
    }
    if (productPrice == null) {
        return alert("Please enter price");
    }
    formmodified = 1;

    var $row = $("<tr data-productname='" + productName + "' data-buyprice='" + buyPrice + "'  data-qty='" + productQty + "' data-id='" + productId + "' data-warranty='" + productWarranty + "' data-warrId='" + productWarrantyId + "'   data-unitprice='" + productPrice + "' data-taxid='" + productTaxId + "' data-discount='" + DiscountAmount + "'   data-net='" + netAmount + "' data-tax='" + productTax.toFixed(2) + "' data-tax-pc='" + productTaxPc + "' data-netam='" + net + "'><td>" + productName + "</td><td>" + productPrice.toFixed(2) + "</td><td>" + productQty + "</td><td>" + productWarranty.toFixed(2) + "</td><td>" + productTax.toFixed(2) + " (" + productTaxPc.toFixed(2) + "%)</td><td>" + DiscountAmount + "(" + DiscountBox.GetValue().toFixed(2) + "%)</td><td>" + netAmount.toFixed(2) + "</td><td style='display:none;'>" + net.toFixed(2) + "</td><td><b class='btn btn-sm fa fa-trash'></b><b class='btn btn-sm fa fa-pencil'></b></td></tr>");
    updateRowActions($row);
    if ($rowToReplace != null) {
        rowIndex = $rowToReplace.index() - 1;
        $row.insertBefore($rowToReplace);
        $rowToReplace.remove();
    } else {
        $table.append($row);
        rowIndex = $row.index() - 1;
    }

    var hiddenValues = updateInvoiceRowHiddenValues(rowIndex, productId, productQty, productTax.toFixed(2), netAmount.toFixed(2), productWarranty.toFixed(2), productPrice, productWarrantyId, productTaxId, DiscountAmount, net.toFixed(2));

    $("#frmDirectCreate").append(hiddenValues);

    updateInvoiceTotals();
    DisableChosenDropdown("AccountId", true);


}

var updateRowActions = function ($row) {
    $row.find(".fa-trash").on("click", function () {
        if (confirm("Are you sure you want to delete?")) {
            


            var productId = $(this).closest("tr").data('id');
            var $rowToReplaces = null;
            var rowsIndex = null;

            var $table = $("#invoice-products-table");
            if ($table.find("tr[data-id=" + productId + "]").length > 0) {
                $rowToReplaces = $table.find("tr[data-id=" + productId + "]");
                rowsIndex = $rowToReplaces.index();
            }

            $("#frmDirectCreate input[type=hidden][id^=AllInvoiceProducts" + (rowsIndex - 1)).remove();
            $(this).closest("tr").remove();

            updateProductDetailsAfterDelete();
            updateInvoiceTotals();
        }

    });
    $row.find(".fa-pencil").on("click", function ()
    {
        
        isEdit = true;
        var productId = $(this).closest("tr").data('id');
        ProductIdEdit = productId;
        GProductId = productId;
        var unitprice = $(this).closest("tr").data('unitprice');
        var qty = $(this).closest("tr").data('qty');
        var taxId = $(this).closest("tr").data('taxid');
        $("#TaxId").val(taxId);
        $("#TaxId").trigger("chosen:updated");
        var warrId = $(this).closest("tr").data('warrid');
        if (warrId > 0) {
            $(".WarrantyDetail").show();
            $("#WarrantyId").val(warrId);
            $("#WarrantyId").trigger("chosen:updated");
        }
        else { $(".WarrantyDetail").hide(); }
        var buyingPrice = $(this).closest("tr").data('buyprice');
        $("#BuyPrice").val(buyingPrice);
        InvoiceProductPrice.SetValue(unitprice);
        InvoiceProductQty.SetValue(qty);
        $.ajax({
            url: '/OrderDetail/EditProductLargeCombobox',
            type: "GET",
            data: { ProductId: parseInt(productId) },
            success: function (data) {
                $('.largecomboboxEdit').html("");
                $('.largecomboboxEdit').html(data);
            },
            error: function (xhr, textStatus, errorThrown) {
                alert('Request Status: ' + xhr.status + '; Status Text: ' + textStatus + '; Error: ' + errorThrown);
            }
        });
        //updateProductPrice();
        updateAddItemButtonText();

    });
}

function AddedPaymentDirectSales() {
    var balance = $("#frmDirectCreate #InvoiceTotal").val();
    var paid = PaymentToday.GetValue();
    if (paid != null && paid > 0) {
        var amt = parseFloat(paid);
        if (!isNaN(amt)) {
            balance = balance - amt;
        }
    }
    $("#divOutstandingBalance").html(balance.toFixed(2));
}

var updateInvoiceTotals = function () {
    
    var finalTotal = 0;
    var taxTotal = 0;
    var finalInvoiceTotal = 0;
    var warrantyTotal = 0;
    var discount = 0;
    var netamount = 0;
    $(".invoice-product-details table:eq(0)").find("tr").each(function () {
        if ($(this).find("td").length > 0) {

            finalTotal += parseFloat($(this).data("net"));
            netamount += parseFloat($(this).data("netam"));
            taxTotal += parseFloat($(this).data("tax"));
            warrantyTotal += parseFloat($(this).data("warranty"));
            discount += parseFloat($(this).data("discount"));
        }
    });
    finalInvoiceTotal = parseFloat(finalTotal - discount);
    $("table.invoice-footer-summary").find("strong[data-attr=Net]").text(netamount.toFixed(2));
    $("table.invoice-footer-summary").find("strong[data-attr=TotalNet]").text(finalTotal.toFixed(2));
    $("table.invoice-footer-summary").find("strong[data-attr=TotalVat]").text(taxTotal.toFixed(2));
    $("table.invoice-footer-summary").find("strong[data-attr=TotalWarranty]").text(warrantyTotal.toFixed(2));
    $("table.invoice-footer-summary").find("strong[data-attr=TotalDiscount]").text(discount.toFixed(2));
    $("table.invoice-footer-summary").find("strong[data-attr=InvoiceTotal]").text(finalInvoiceTotal.toFixed(2));


    $("#frmDirectCreate #NetAmount").val(finalTotal.toFixed(2));
    $("#frmDirectCreate #TaxAmount").val(taxTotal.toFixed(2));
    $("#frmDirectCreate #WarrantyAmount").val(warrantyTotal.toFixed(2));
    $("#frmDirectCreate #InvoiceTotal").val(finalInvoiceTotal.toFixed(2));
    $("#divOutstandingBalance").text(finalInvoiceTotal.toFixed(2));
    $("#DiscountAmount").val(discount.toFixed(2));


}
function DisableChosenDropdown(dropdownId, disable) {

    if (disable) {
        $("#" + dropdownId).prop("disabled", true);
        if ($("input[type=hidden][id=" + dropdownId + "]").length < 1) {
            $("<input type='hidden' id='" + dropdownId + "' name='" + dropdownId + "' value='" + $("#" + dropdownId).val() + "'/>").insertAfter($("#" + dropdownId));
        } else {
            $("input[name=AccountId]").val($("select[name=AccountId]").val());
        }
    } else {
        $("#" + dropdownId).removeAttr("disabled");
        $("input[type=hidden][id=" + dropdownId + "]").remove();
    }
    $("#" + dropdownId).trigger("chosen:updated");
}
function formsubmit() {
    
    formmodified = 0;
    $('#frmDirectCreate').submit();
}
var updateProductDetailsAfterDelete = function () {
    
    var rowsIndex = 0;
    $("#invoice-products-table").find("tr[data-id]").each(function () {
        var productId = $(this).data('id');
        var productQty = $(this).data('qty');
        var productPrice = parseFloat($(this).data('unitprice'));
        var productTax = parseFloat($(this).data('tax'));
        var productTaxPc = $(this).data('tax-pc');
        var productWarranty = parseFloat($(this).data('warranty'));
        var productName = $(this).data('productname');
        var totalAmount = (productPrice * productQty);
        var DiscountAmount = parseFloat($(this).data('discount'));
        var productTaxId = $(this).data('taxid');
        var productWarrantyId = $("#WarrantyId").val();
        var buyPrice = $(this).data('buyprice');
        if (DiscountAmount > totalAmount) {

            return alert("Discount price is not greater than Total amount");
        }

        var netAmount = totalAmount + productTax + productWarranty;
        var net = totalAmount + productWarranty;
        var $table = $("#invoice-products-table");

        var $rowToReplace = null;
        if ($table.find("tr[data-id=" + productId + "]").length > 0) {
            $rowToReplace = $table.find("tr[data-id=" + productId + "]");

        }
        if (productQty == null) {
            return alert("Please enter quantity");
        }
        if (productPrice == null) {
            return alert("Please enter price");
        }
        formmodified = 1;

        var $row = $("<tr data-productname='" + productName + "' data-qty='" + productQty + "' data-buyprice='" + buyPrice + "'   data-id='" + productId + "' data-warranty='" + productWarranty + "' data-unitprice='" + productPrice + "' data-taxid='" + productTaxId + "' data-discount='" + DiscountAmount + "'   data-net='" + netAmount + "' data-tax='" + productTax.toFixed(2) + "' data-tax-pc='" + productTaxPc + "' data-netam='" + net + "'><td>" + productName + "</td><td>" + productPrice.toFixed(2) + "</td><td>" + productQty + "</td><td>" + productWarranty.toFixed(2) + "</td><td>" + productTax.toFixed(2) + " (" + productTaxPc.toFixed(2) + "%)</td><td>" + DiscountAmount + "(" + DiscountBox.GetValue().toFixed(2) + "%)</td><td>" + netAmount.toFixed(2) + "</td><td style='display:none;'>" + net.toFixed(2) + "</td><td><b class='btn btn-sm fa fa-trash'></b><b class='btn btn-sm fa fa-pencil'></b></td></tr>");
        updateRowActions($row);
        if ($rowToReplace != null) {

            $row.insertBefore($rowToReplace);
            var index = this.rowIndex;
            $("#frmDirectCreate input[type=hidden][id^=AllInvoiceProducts" + (index - 1)).remove();
            $rowToReplace.remove();


        }
        else {
            $table.append($row);

        }

        var hiddenValues = updateInvoiceRowHiddenValues(rowsIndex, productId, productQty, productTax.toFixed(2), netAmount.toFixed(2), productWarranty.toFixed(2), productPrice, productWarrantyId, productTaxId, DiscountAmount, net.toFixed(2));

        $("#frmDirectCreate").append(hiddenValues);

        updateInvoiceTotals();
        DisableChosenDropdown("AccountId", true);
        rowsIndex++;
    });
}
