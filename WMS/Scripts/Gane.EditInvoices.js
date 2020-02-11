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

    var model = { ProductId: GProductId, AccountId: $("#AccountId").val() };
    if ((model.ProductId < 0) || model.AccountId < 0) return;


    $.get('/Invoice/GetProductPrice/', model)
        .done(function (data) {
            pricealert = data.ShowPriceAlert;
            var buyPrice = data.minimumSalePrice;
            percentageValue = data.percentageValue;
            if ((buyPrice > data.SellPrice) && pricealert) {

                alert("Selling price is less than buying price and buying price is: " + buyPrice);
            }
            //else if ((buyPrice > margin) && pricealert) {
            //    alert("Selling price should not be less than " + (margin));
            //}

            $("#BuyPrice").val(buyPrice);
            PriceAnalysis();
            InvoiceProductPrice.SetValue(data.SellPrice);
            //InvoiceProductPrice.SetMinValue(data.MinimumThresholdPrice);


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
        isEdit = false;
        $(".btn-invoice-add-product span").text('Add Item');
    }
}


var updateInvoiceRowHiddenValues = function (index, productId, qtyProcessed, taxTotal, netTotal, warrantyAmt, price, warrantyId, taxId, discount, TaxPercent) {
    $("#frmInvoicesCreate input[type=hidden][id^=AllInvoiceProducts" + index).remove();
    var result = "<input type='hidden' id='AllInvoiceProducts" + index + "_ProductId' name='AllInvoiceProducts[" + index + "].ProductId' value='" + productId + "' />" +
        "<input type='hidden' id='AllInvoiceProducts" + index + "_QtyProcessed' name='AllInvoiceProducts[" + index + "].QtyProcessed' value='" + qtyProcessed + "' />" +
        "<input type='hidden' id='AllInvoiceProducts" + index + "_NetAmount' name='AllInvoiceProducts[" + index + "].NetAmount' value='" + netTotal + "' />" +
        "<input type='hidden' id='AllInvoiceProducts" + index + "_Price' name='AllInvoiceProducts[" + index + "].Price' value='" + price + "' />" +
        "<input type='hidden' id='AllInvoiceProducts" + index + "_Price' name='AllInvoiceProducts[" + index + "].TaxPercent' value='" + TaxPercent + "' />" +
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
    $("#frmInvoicesCreate #InvoiceDate").val(invoiceDate);
    $("#frmeditDsale #EditInvoiceDate").val(invoiceDate);
}


$(document).ready(function () {
    var invoiceMasterId = $("#InvoiceMasterId").val();
    EditInvoiceDetail(invoiceMasterId);


    $("#AccountId").on("change",
        function () {

            //updateAccountDetails();
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
    };

    $("#btn-export-invoice").on("click", function () {
        exportInvoice();
    });

    var $form = $("#frmInvoicesCreate");
    $form.submit(function (e) {
        var abc = $("#AccountId").val();
        var isValid = true;
        if ($("#AccountId").val() < 0.01) {
            var paid = PaymentToday.GetValue();
            var toPay = parseFloat($("#frmInvoicesCreate #InvoiceTotal").val()).toFixed(2);
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

        frmInvoicesCreate.submit();
    });

    updateInvoiceTotals();
    updateInvoiceDate();

    $(".fa-trash").on("click", function () {

        // under alert  are you want to delete this item
        if (confirm("This operation is irreversible, are you sure to delete?")) {
            var $rowToReplaces = null;
            var editrows = null;
            editrows = $(this).closest("tr").data('index');
            
            var count = $("#frmInvoicesCreate input[type=hidden][id^=AllInvoiceProducts" + editrows);
            $("#frmInvoicesCreate input[type=hidden][id^=AllInvoiceProducts" + editrows).remove();
            $(this).closest("tr").remove();
            $("#invoice-products-table tr").remove();
            if ($("#invoice-products-table tr").length <= 1) {

                DisableChosenDropdown("AllAccounts", true);
            }
            updateInvoiceTotals();
        }
    });
    $(".fa-pencil").on("click", function () {
        isEdit = true;
        var productId = $(this).closest("tr").data('id');
        ProductIdEdit = productId;
        GProductId = productId;
        var unitprice = $(this).closest("tr").data('unitprice');
        var qty = $(this).closest("tr").data('qty');
        var TaxId = $(this).closest("tr").data('taxid');
        var buyingPrice = $(this).closest("tr").data('buyprice');
        $("#BuyPrice").val(buyingPrice);
        $("#TaxId").val(TaxId);
        $("#TaxId").trigger("chosen:updated");
        var warrId = $(this).closest("tr").data('warrid');

        if (warrId > 0) {
            $("#WarrantyId").val(warrId);
            $("#WarrantyId").trigger("chosen:updated");
            $(".WarrantyDetail").show();
        }
        else {
            $("#WarrantyId").val(0);
            $("#WarrantyId").trigger("chosen:updated");
            $(".WarrantyDetail").hide();
        }
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

        updateProductPrice();


    });
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

var updateProductDetails = function (editProductId) {
   
    var productId = prdid.GetValue();
    var productQty = InvoiceProductQty.GetValue();
    var productPrice = InvoiceProductPrice.GetValue();

    var productTaxId = $("#TaxId").val();
    var productWarrantyId = $("#WarrantyId").val();

    var productTax = getTaxAmountById(productTaxId, productPrice) * productQty;
    var productTaxPc = getTaxAmountById(productTaxId, productPrice, true);
    var productWarranty = getWarrantyAmountById(productWarrantyId, productPrice) * productQty;
    var productName = prdid.GetText();
    var totalAmount = (productPrice * productQty);
    var DiscountAmount = getDiscountAmount(totalAmount);
    var buyPrice = $("#BuyPrice").val();

    if (DiscountAmount > totalAmount) {

        return alert("Discount price is not greater than Total amount");
    }

    var netAmount = totalAmount + productTax + productWarranty;
    var net = totalAmount + productWarranty;
    var $table = $("#invoice-products-table");
    var rowIndex = 1;
    var $rowToReplace = null;
    if ($table.find("tr[data-id=" + editProductId + "]").length > 0) {
        $rowToReplace = $table.find("tr[data-id=" + editProductId + "]");
        rowIndex = $rowToReplace.index();
    }
    if (productQty == null) {
        return alert("Please enter quantity");
    }
    if (productPrice == null) {
        return alert("Please enter price");
    }
    formmodified = 1;
    var $row = $("<tr data-productname='" + productName + "' data-qty='" + productQty + "' data-buyprice='" + buyPrice + "'  data-id='" + productId + "' data-warranty='" + productWarranty + "' data-warrid='" + productWarrantyId + "'  data-unitprice='" + productPrice + "' data-taxid='" + productTaxId + "' data-discount='" + DiscountAmount + "'   data-net='" + netAmount + "' data-tax='" + productTax.toFixed(2) + "' data-tax-pc='" + productTaxPc + "' data-netam='" + net + "'  data-index=" + rowIndex + "><td>" + productName + "</td><td>" + productPrice.toFixed(2) + "</td><td>" + productQty + "</td><td>" + productWarranty.toFixed(2) + "</td><td>" + productTaxPc.toFixed(2) + "%</td><td>" + productTax.toFixed(2) + "</td><td>" + DiscountAmount + "(" + DiscountBox.GetValue().toFixed(2) + "%)</td><td>" + netAmount.toFixed(2) + "</td><td style='display:none;'>" + net.toFixed(2) + "</td><td><b class='btn btn-sm fa fa-trash'></b><b class='btn btn-sm fa fa-pencil'></b></td></tr>");
    updateRowActions($row);
    if ($rowToReplace != null) {
        rowIndex = $rowToReplace.index() - 1;
        $row.insertBefore($rowToReplace);
        $rowToReplace.remove();
    } else {
        $table.append($row);
        rowIndex = $row.index() - 1;
    }

    var hiddenValues = updateInvoiceRowHiddenValues(rowIndex, productId, productQty, productTax.toFixed(2), netAmount.toFixed(2), productWarranty.toFixed(2), productPrice, productWarrantyId, productTaxId, DiscountAmount, net.toFixed(2), productTaxPc);

    $("#frmInvoicesCreate").append(hiddenValues);

    updateInvoiceTotals();
    DisableChosenDropdown("AllAccounts", true);
    isEdit = false;


}
var addProductDetails = function () {
  
    updateProductDetailsAfterDelete();
    var productId = prdid.GetValue();
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
    var rowCount = $('#invoice-products-table >tbody> tr').length;
    if (rowCount > 0) {
        rowIndex = (rowCount - 1);
    }
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
    var $row = $("<tr data-productname='" + productName + "' data-qty='" + productQty + "' data-buyprice='" + buyPrice + "'  data-id='" + productId + "' data-warranty='" + productWarranty + "' data-warrid='" + productWarrantyId + "'  data-unitprice='" + productPrice + "' data-taxid='" + productTaxId + "' data-discount='" + DiscountAmount + "'   data-net='" + netAmount + "' data-tax='" + productTax.toFixed(2) + "' data-tax-pc='" + productTaxPc + "' data-netam='" + net + "'  data-index=" + rowIndex + "><td>" + productName + "</td><td>" + productPrice.toFixed(2) + "</td><td>" + productQty + "</td><td>" + productWarranty.toFixed(2) + "</td><td>" + productTaxPc.toFixed(2) + "%</td><td>" + productTax.toFixed(2) + "</td><td>" + DiscountAmount + "(" + DiscountBox.GetValue().toFixed(2) + "%)</td><td>" + netAmount.toFixed(2) + "</td><td style='display:none;'>" + net.toFixed(2) + "</td><td><b class='btn btn-sm fa fa-trash'></b><b class='btn btn-sm fa fa-pencil'></b></td></tr>");
    updateRowActions($row);
    if ($rowToReplace != null) {
        rowIndex = $rowToReplace.index() - 1;
        $row.insertBefore($rowToReplace);
        $rowToReplace.remove();
    } else {
        $table.append($row);
        //rowIndex = $row.index() - 1;
    }

    var hiddenValues = updateInvoiceRowHiddenValues(rowIndex, productId, productQty, productTax.toFixed(2), netAmount.toFixed(2), productWarranty.toFixed(2), productPrice, productWarrantyId, productTaxId, DiscountAmount, net.toFixed(2), productTaxPc);

    $("#frmInvoicesCreate").append(hiddenValues);

    updateInvoiceTotals();
    DisableChosenDropdown("AllAccounts", true);


}

var updateRowActions = function ($row) {
    $row.find(".fa-trash").on("click", function () {
        if (confirm("This operation is irreversible, are you sure to delete?")) {
           
            var productId = $(this).closest("tr").data('id');
            var $rowToReplaces = null;
            var rowsIndex = null;
          
            var $table = $("#invoice-products-table");
            if ($table.find("tr[data-id=" + productId + "]").length > 0) {
                $rowToReplaces = $table.find("tr[data-id=" + productId + "]");
                rowsIndex = $rowToReplaces.index();
            }

            $("#frmInvoicesCreate input[type=hidden][id^=AllInvoiceProducts" + (rowsIndex - 1)).remove();
            $(this).closest("tr").remove();
            if ($("#invoice-products-table tr").length <= 1) {

                DisableChosenDropdown("AllAccounts", false);
            }
            updateProductDetailsAfterDelete();
            updateInvoiceTotals();
        }
    });
    $row.find(".fa-pencil").on("click", function () {
        isEdit = true;
        var productId = $(this).closest("tr").data('id');
        ProductIdEdit = productId;
        GProductId = productId;
        var unitprice = $(this).closest("tr").data('unitprice');
        var qty = $(this).closest("tr").data('qty');
        var TaxId = $(this).closest("tr").data('taxid');
        var warrId = $(this).closest("tr").data('warrid');
        $("#TaxId").val(TaxId);
        $("#TaxId").trigger("chosen:updated");
        var buyingPrice = $(this).closest("tr").data('buyprice');
        $("#BuyPrice").val(buyingPrice);
        if (warrId > 0) {
            $("#WarrantyId").val(warrId);
            $("#WarrantyId").trigger("chosen:updated");
            $(".WarrantyDetail").show();
        }
        else {
            $("#WarrantyId").val(0);
            $("#WarrantyId").trigger("chosen:updated");
            $(".WarrantyDetail").hide();
        }
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
    finalInvoiceTotal = finalTotal;

    $("table.invoice-footer-summary").find("strong[data-attr=Net]").text(netamount.toFixed(2));
    $("table.invoice-footer-summary").find("strong[data-attr=TotalNet]").text(finalTotal.toFixed(2));
    $("table.invoice-footer-summary").find("strong[data-attr=TotalVat]").text(taxTotal.toFixed(2));
    $("table.invoice-footer-summary").find("strong[data-attr=TotalWarranty]").text(warrantyTotal.toFixed(2));
    $("table.invoice-footer-summary").find("strong[data-attr=TotalDiscount]").text(discount.toFixed(2));
    $("table.invoice-footer-summary").find("strong[data-attr=InvoiceTotal]").text(finalInvoiceTotal.toFixed(2));


    $("#frmInvoicesCreate #NetAmount").val(finalTotal.toFixed(2));
    $("#frmInvoicesCreate #TaxAmount").val(taxTotal.toFixed(2));
    $("#frmInvoicesCreate #WarrantyAmount").val(warrantyTotal.toFixed(2));
    $("#frmInvoicesCreate #InvoiceTotal").val(finalInvoiceTotal.toFixed(2));
    $("#divOutstandingBalance").text(finalInvoiceTotal.toFixed(2));
    $("#DiscountAmount").val(discount.toFixed(2));


};

function DisableChosenDropdown(dropdownId, disable) {

    if (disable) {
        var filedCheck = $("#" + dropdownId).is('input');
        if (filedCheck === false) {
            $("#" + dropdownId).prop("disabled", true);
            if ($("input[type=hidden][id=" + dropdownId + "]").length < 1)
            {
                $("<input type='hidden' id='" + dropdownId + "' name='" + dropdownId + "' value='" + $("#" + dropdownId).val() + "'/>").insertAfter($("#" + dropdownId));
            } else {
                $("input[name=AccountId]").val($("select[name=AccountId]").val());
            }

        }
    }
    else {
        $("#" + dropdownId).removeAttr("disabled");
        $("input[type=hidden][id=" + dropdownId + "]").remove();

    }
    $("#" + dropdownId).trigger("chosen:updated");
}
function formsubmit() {

    formmodified = 0;
    $('#frmInvoicesCreate').submit();
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
        var DiscountAmount = parseFloat("0.00");
        var productTaxId = $(this).data('taxid');
        var productWarrantyId = $(this).data('warrid');
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
        var $row = $("<tr data-productname='" + productName + "' data-qty='" + productQty + "' data-buyprice='" + buyPrice + "'     data-id='" + productId + "' data-warranty='" + productWarranty + "' data-warrid='" + productWarrantyId + "'   data-unitprice='" + productPrice + "' data-taxid='" + productTaxId + "' data-discount='" + DiscountAmount + "'   data-net='" + netAmount + "' data-tax='" + productTax.toFixed(2) + "' data-tax-pc='" + productTaxPc + "' data-netam='" + net + "'><td>" + productName + "</td><td>" + productPrice.toFixed(2) + "</td><td>" + productQty + "</td><td>" + productWarranty.toFixed(2) + "</td><td>" + productTaxPc.toFixed(2) + "%</td><td>" + productTax.toFixed(2) + "</td><td>" + DiscountAmount + "(" + DiscountBox.GetValue().toFixed(2) + "%)</td><td>" + netAmount.toFixed(2) + "</td><td style='display:none;'>" + net.toFixed(2) + "</td><td><b class='btn btn-sm fa fa-trash'></b><b class='btn btn-sm fa fa-pencil'></b></td></tr>");
        updateRowActions($row);
        if ($rowToReplace != null) {
            $row.insertBefore($rowToReplace);
            var index = this.rowIndex;
            $("#frmInvoicesCreate input[type=hidden][id^=AllInvoiceProducts" + (index - 1)).remove();
            $rowToReplace.remove();
        }
        else {
            $table.append($row);
        }

        var hiddenValues = updateInvoiceRowHiddenValues(rowsIndex, productId, productQty, productTax.toFixed(2), netAmount.toFixed(2), productWarranty.toFixed(2), productPrice, productWarrantyId, productTaxId, DiscountAmount, net.toFixed(2));

        $("#frmInvoicesCreate").append(hiddenValues);

        updateInvoiceTotals();
        DisableChosenDropdown("AccountId", true);
        rowsIndex++;
    });
}

var EditInvoiceDetail = function (invoicemasterid) {
    $.ajax({
        url: '/Invoice/GetInvoiceDetail',
        type: "GET",
        data: { masterId: parseInt(invoicemasterid) },
        success: function (data) {
            pricealert = data.ShowPriceAlertInSalesOrder;
            $.each(data.InvoiceProducts, function (index, element) {

                var productId = element.ProductId;
                var productQty = element.QtyProcessed;
                var productPrice = element.Price;
                var productTax = parseFloat(element.TaxAmount);
                var productTaxPc = element.TaxPercent;
                var productWarranty = parseFloat(element.WarrantyAmount);
                var productName = element.ProductName;
                var totalAmount = (productPrice * productQty);
                var DiscountAmount = parseFloat("0.00");
                var productTaxId = element.TaxId;
                var buyPrice = element.MinSellPrice;
                var productWarrantyId = element.WarrantyId;
                if (DiscountAmount > totalAmount) {

                    return alert("Discount price is not greater than Total amount");
                }

                var netAmount = totalAmount + productTax + productWarranty;
                var net = totalAmount + productWarranty;
                var $table = $("#invoice-products-table");
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

                var $row = $("<tr data-productname='" + productName + "' data-qty='" + productQty + "' data-buyprice='" + buyPrice + "'  data-id='" + productId + "' data-warranty='" + productWarranty + "' data-warrid='" + productWarrantyId + "'  data-unitprice='" + productPrice + "' data-taxid='" + productTaxId + "' data-discount='" + DiscountAmount + "'   data-net='" + netAmount + "' data-tax='" + productTax.toFixed(2) + "' data-tax-pc='" + productTaxPc + "' data-netam='" + net + "'><td>" + productName + "</td><td>" + productPrice.toFixed(2) + "</td><td>" + productQty + "</td><td>" + productWarranty.toFixed(2) + "</td><td>" + productTaxPc.toFixed(2) + "%</td><td>" + productTax.toFixed(2) + "</td><td>" + DiscountAmount + "(" + DiscountBox.GetValue().toFixed(2) + "%)</td><td>" + netAmount.toFixed(2) + "</td><td style='display:none;'>" + net.toFixed(2) + "</td><td><b class='btn btn-sm fa fa-trash'></b><b class='btn btn-sm fa fa-pencil'></b></td></tr>");
                updateRowActions($row);
                if ($rowToReplace != null) {
                    $row.insertBefore($rowToReplace);
                    $rowToReplace.remove();
                }
                else {
                    $table.append($row);
                    rowIndex = $row.index() - 1;
                }

                var hiddenValues = updateInvoiceRowHiddenValues(rowIndex, productId, productQty, productTax.toFixed(2), netAmount.toFixed(2), productWarranty.toFixed(2), productPrice, productWarrantyId, productTaxId, DiscountAmount, net.toFixed(2));

                $("#frmInvoicesCreate").append(hiddenValues);

                updateInvoiceTotals();
                DisableChosenDropdown("AccountId", true);
            });

        },
        error: function (xhr, textStatus, errorThrown) {
            alert('Request Status: ' + xhr.status + '; Status Text: ' + textStatus + '; Error: ' + errorThrown);
        }
    });

}