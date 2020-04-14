﻿//////////////////PRODUCT PAGES FUNCTIONS////////////////////////////////////////
var tempid = 0;

function productPriceSave() {

    //DiscontDate.SetEnabled(true);
    var price, startdate, enddate;

    if (IsValidForm('#frmProductPrice')) {

        $.each($('#frmProductPrice').serializeArray(), function (i, field) {
            if (field.name == 'Price') {
                price = field.value;
            }
            else if (field.name == 'DateStartEffect') {
                startdate = field.value;
            }
            else if (field.name == 'DateEndEffect') {
                enddata = field.value;
            }
            else {
            }
        });

        $('#ProductPricesIds').append('<option selected value=0?' + price + '?' + startdate + '?' + enddata + '>(' + price + ') ' + startdate + '-' + enddata + '</option>');
        $("#ProductPricesIds").trigger("chosen:updated");
        LoadingPanel.Hide();
        pcModalproductprice.Hide();
    }

}

function ModalProductRecipeAdd_BeginCallback(s, e) {
    var parentProductId = $("#ProductId").val();
    e.customArgs["productId"] = parentProductId;
}
function ModalProductRecipeAdd_EndCallback(s, e) {
    loadKitProductGridItemEvents();
}
function ModalProductKitAdd_BeginCallback(s, e) {
    var parentProductId = $("#ProductId").val();
    e.customArgs["productId"] = parentProductId;
}
function ModalProductKitAdd_EndCallback(s, e) {
    loadKitProductGridItemEvents();
}

var loadKitProductGridItemEvents = function () {
    $(".recipe-item-qty-text .fa-pencil").on("click",
        function () {

            $(".kititem-edit-qty-buttons.btn-danger").each(function () {
                $(this).parents(".recipe-item-qty-edit").hide();
                $(this).parents(".recipe-item-qty-edit").siblings(".recipe-item-qty-text").show();
            });

            $(this).closest(".recipe-item-qty-text").hide();
            $(this).parent().siblings(".recipe-item-qty-edit").show();
        });

    $(".kititem-edit-qty-buttons.btn-danger").on("click",
        function () {
            $(this).parents(".recipe-item-qty-edit").hide();
            $(this).parents(".recipe-item-qty-edit").siblings(".recipe-item-qty-text").show();
        });
    $(".kititem-edit-qty-buttons.btn-primary").on("click",
        function () {
            var qty = $(this).parents(".recipe-item-qty-edit").find(".recipe-item-qty-textbox").val();
            if (qty == null || qty == '') {
                qty = 0;
            }
            var kitProductId = $(this).data("id");
            updateKitProductQty(kitProductId, qty, $(this).data('action'));

            $(this).parents(".recipe-item-qty-edit").hide();
            $(this).parents(".recipe-item-qty-edit").siblings(".recipe-item-qty-text").show();
        });
}

var updateKitProductQty = function (kitProductId, qty, actionName) {
    var data = { ParentProductId: $("#ProductId").val(), ProductId: kitProductId, Quantity: qty };
    $.post("/Products/" + actionName,
        data,
        function (result) {
            var productid = $("#ProductId").val();

            if (actionName == 'EditSelectedKitItem') {
                ProductKitSelectedItemsGrid.Refresh();
            }
            else if (actionName == 'UpdateKitItemProduct') {
                GetDevexControlByName(productid + "productKitItems").Refresh();
            }
            else if (actionName == "EditSelectedRecipetem") {
                ProductRecipeSelectedItemsGrid.Refresh();
            }
            else {
                GetDevexControlByName(productid + "productRecipeItems").Refresh();
            }
        });
}

function _AddRecipeProduct(productId, productName) {
    var qtyControl = GetDevexControlByName('RecipeProductQty_' + productId);
    var qty = qtyControl.GetValue();
    qtyControl.SetValue("");
    var parentProductId = $("#ProductId").val();
    if (qty <= 0) {
        $("#" + 'RecipeProductQty_' + productId + "_I").css({ "background-color": "#f9baba" });
        setTimeout(function () {
            $("#" + 'RecipeProductQty_' + productId + "_I").removeAttr("style");
        }, 400);
        return false;
    }
    var data = { ParentProductId: parentProductId, ProductId: productId, ProductName: productName, Quantity: qty };
    $.post("/Products/AddProductRecipeItem",
        data,
        function (result) {
            if ($("#ProductRecipeSelectedItemsGrid").length > 0) {
                ProductRecipeSelectedItemsGrid.Refresh();
            }
        });
}

function _AddKitProduct(productId, productName) {
    var qtyControl = GetDevexControlByName('KitProductQty_' + productId);
    var qty = qtyControl.GetValue();
    qtyControl.SetValue("");
    var parentProductId = $("#ProductId").val();
    if (qty <= 0) {
        $("#" + 'KitProductQty_' + productId + "_I").css({ "background-color": "#f9baba" });
        setTimeout(function () {
            $("#" + 'KitProductQty_' + productId + "_I").removeAttr("style");
        }, 400);
        return false;
    }
    var data = { ParentProductId: parentProductId, ProductId: productId, ProductName: productName, Quantity: qty };
    $.post("/Products/AddProductKitItem",
        data,
        function (result) {
            if ($("#ProductKitSelectedItemsGrid").length > 0) {
                ProductKitSelectedItemsGrid.Refresh();
            }
        });
}

function ConfirmAddedRecipeProducts() {
    var productId = $("#ProductId").val();
    var data = { Id: productId };
    $.post("/Products/ConfirmAddedRecipeProducts",
        data,
        function (result) {
            GetDevexControlByName(productId + "productRecipeItems").Refresh();
            pcModalProductRecipeItemsEditor.Hide();
        });
}
function ConfirmAddedKitProducts() {
    var productId = $("#ProductId").val();
    var data = { Id: productId };
    $.post("/Products/ConfirmAddedKitProducts",
        data,
        function (result) {
            GetDevexControlByName(productId + "productKitItems").Refresh();
            pcModalProductKitItemsEditor.Hide();
        });
}

function productaccountSave() {
    $("#vldPrAccountCode").removeClass("validation-summary-errors");
    $("#vldPrAccountCode").addClass("validation-summary-valid");

    if (IsValidForm('#frmproductaccountcode')) {
        var value;
        var code;
        var deliveryType;
        var orderingNotes;
        $.each($('#frmproductaccountcode').serializeArray(), function (i, field) {

            if (field.name == 'AccountID') {
                value = field.value;
            }
            else if (field.name == 'ProdAccCode') {
                code = field.value;


            }
            else if (field.name == 'ProdDeliveryType') {
                deliveryType = field.value;


            }
            else if (field.name == 'ProdOrderingNotes') {
                orderingNotes = field.value;


            }

            else {




            }
        });
        var dataRec =
        {
            "AccountID": value,
            "ProdAccCode": code,
            "ProdDeliveryType": deliveryType,
            "ProdOrderingNotes": orderingNotes
        };
        $.ajax({
            url: "/Products/_CreateProductAccount",
            data: dataRec,
            success: function (result) {

                $('#ProductAccountCodeIds').append('<option selected value=' + result.ProdAccCodeID + '>' + result.ProdAccCode + '</option>');
                $("#ProductAccountCodeIds").trigger("chosen:updated");

            }
        });




        LoadingPanel.Hide();
        pcModalproductaccount.Hide();

    }
    else {


    }
}
function cancelproductaccount() {
    pcModalproductaccount.Hide();


}
function productCategoryPost() {
    $("#vldProductCategory").removeClass("validation-summary-errors");
    $("#vldProductCategory").addClass("validation-summary-valid");
    if (IsValidForm('#frmproductCategory')) {
        LoadingPanel.ContainerElementID = "pcModalproductcategory";
        LoadingPanel.Show();
        var data = $("#frmproductCategory").serializeArray();
        var AccountId = $("#AccountID").val();
        data.push({ name: 'AccountId', value: AccountId });
        $.post("/Products/_ProductCategorySubmit", data, function (result) {
            // Do something with the response `res`
            if (result.error == false) {
                if ($("#UpdateOldProductData").val() == "True") {
                    $('.drpPD').append('<option selected value=' + result.id + '>' + result.category + '</option>');
                    $(".drpPD").trigger("chosen:updated");
                }
                else {
                    $('#drpPD').append('<option selected value=' + result.id + '>' + result.category + '</option>');
                    $("#drpPD").trigger("chosen:updated");
                }
                LoadingPanel.Hide();
                pcModalproductcategory.Hide();
            }
            else {
                var ul = $("#vldProductCategory ul");
                $("#vldProductCategory").addClass("validation-summary-errors");
                $("#vldProductCategory").removeClass("validation-summary-valid");
                ul.html("");
                ul.append("<li> " + result.errormessage + "</li>");
                LoadingPanel.Hide();
            }
        }).fail(function (result) {
            var ul = $("#vldProductCategory ul");
            $("#vldProductCategory").addClass("validation-summary-errors");
            $("#vldProductCategory").removeClass("validation-summary-valid");
            ul.html("");
            ul.append("<li> " + result.errormessage + "</li>");
            LoadingPanel.Hide();
        });
    }

}

function cancelproductCategoryadd() {
    pcModalproductcategory.Hide();

}

function cancelproductgroupadd() {
    pcModalproductgroup.Hide();

}

function productGroupPost() {

    $("#vldProductGroup").removeClass("validation-summary-errors");
    $("#vldProductGroup").addClass("validation-summary-valid");


    if (IsValidForm('#frmprgroup')) {

        LoadingPanel.ContainerElementID = "pcModalproductgroup";
        LoadingPanel.Show();

        var data = $("#frmprgroup").serializeArray();
        var departmentId = $("select[name='DepartmentId']").val();
        data.push({ name: 'DepartmentId', value: departmentId });

        $.post("/Products/_ProductGroupSubmit", data, function (result) {
            // Do something with the response `res`
            if (result.error == false) {
                $('#ProductGroupId').append('<option selected value=' + result.id + '>' + result.productgroup + '</option>');
                $("#ProductGroupId").trigger("chosen:updated");

                if ($("#UpdateOldProductData").val() == "True") {
                    $('.drpPG').append('<option selected value=' + result.id + '>' + result.productgroup + '</option>');
                    $(".drpPG").trigger("chosen:updated");
                }
                else {
                    $('#drpPG').append('<option selected value=' + result.id + '>' + result.productgroup + '</option>');
                    $("#drpPG").trigger("chosen:updated");
                }


                LoadingPanel.Hide();
                pcModalproductgroup.Hide();
            }

            else {
                var ul = $("#vldProductGroup ul");
                $("#vldProductGroup").addClass("validation-summary-errors");
                $("#vldProductGroup").removeClass("validation-summary-valid");
                ul.html("");
                ul.append("<li> " + result.errormessage + "</li>");
                LoadingPanel.Hide();
            }

        }).fail(function (result) {
            var ul = $("#vldProductGroup ul");
            $("#vldProductGroup").addClass("validation-summary-errors");
            $("#vldProductGroup").removeClass("validation-summary-valid");
            ul.html("");
            ul.append("<li> " + result.errormessage + "</li>");
            LoadingPanel.Hide();
        })
    }
    else {


    }
}
function showproductgrouppopup() {

    pcModalproductgroup.Show();

}
function showlocationpopup() {

    pcModalLocations.Show();

}
function attributeValueSave() {
    $("#vldAttributeValue").removeClass("validation-summary-errors");
    $("#vldAttributeValue").addClass("validation-summary-valid");
    if (IsValidForm('#frmattribute')) {
        LoadingPanel.ContainerElementID = "pcModalAttributesValues";
        LoadingPanel.Show();
        var data = $("#frmattribute").serialize();
        $.post("/Products/_AttributeValueSubmit", data, function (result) {
            // Do something with the response `res`
            if (result.error == true) {
                var ul = $("#vldAttributeValue ul");
                $("#vldAttributeValue").addClass("validation-summary-errors");
                $("#vldAttributeValue").removeClass("validation-summary-valid");
                ul.html("");
                ul.append("<li> " + result.errormessage + "</li>");
                LoadingPanel.Hide();
            }
            else {
                var $lastOptGroup = $('<optgroup label="' + $("#drpattribute option:selected").text() + '">');
                $lastOptGroup.append('<option selected value=' + result.Id + '>' + result.value + '</option>');
                $('#ProductAttributesIds').append($lastOptGroup);

                $("#ProductAttributesIds").trigger("chosen:updated");
                LoadingPanel.Hide();
                pcModalAttributesValues.Hide();

            }
            $("#ProductAttributesIds").trigger("chosen:updated");


        }).fail(function (response) {
            var ul = $("#vldAttributeValue ul");
            $("#vldAttributeValue").addClass("validation-summary-errors");
            $("#vldAttributeValue").removeClass("validation-summary-valid");
            ul.html("");
            ul.append("<li> " + result.errormessage + "</li>");
            LoadingPanel.Hide();
        })

    }

}
function attributeSave() {
    $("#vldAttribute").removeClass("validation-summary-errors");
    $("#vldAttribute").addClass("validation-summary-valid");
    if (IsValidForm('#frmattributename')) {
        LoadingPanel.ContainerElementID = "pcModalAttributes";
        LoadingPanel.Show();
        var data = $("#frmattributename").serialize();
        $.post("/Products/_AttributeSubmit", data, function (result) {
            // Do something with the response `res`
            if (result.error == true) {
                var ul = $("#vldAttribute ul");
                $("#vldAttribute").addClass("validation-summary-errors");
                //0347-5501552 rafiq
                $("#vldAttribute").removeClass("validation-summary-valid");
                ul.html("");
                ul.append("<li> " + result.errormessage + "</li>");
                LoadingPanel.Hide();
            }
            else {
                var newOption = "<option selected value=" + result.id + ">" + result.name + "</option>";
                //$("#lstattributes").append('<option selected value=' + result.id + '>' + result.name + '</option>');
                $("#drpattribute").append('<option selected value=' + result.id + '>' + result.name + '</option>');
                $("#drpattribute").trigger("chosen:updated");
                LoadingPanel.Hide();
                pcModalAttributes.Hide();

            }
        }).fail(function (response) {
            var ul = $("#vldAttribute ul");
            $("#vldAttribute").addClass("validation-summary-errors");
            $("#vldAttribute").removeClass("validation-summary-valid");
            ul.html("");
            ul.append("<li> " + result.errormessage + "</li>");
            LoadingPanel.Hide();
        })

    }

}

function cancellocationadd() {
    pcModalLocations.Hide();

}
function cancelattributevalueadd() {
    pcModalAttributesValues.Hide();

}
function cancelattributeadd() {
    pcModalAttributes.Hide();

}
function IsValidForm(form) {
    InitializeValidationRulesForForm(form);
    var validator = $.data($(form)[0], 'validator');
    if (validator == null) return true;
    return validator.form();
}
function InitializeValidationRulesForForm(form) {
    var form = $(form);
    if (form.attr("executed"))
        return;
    form.removeData("validator");
    $.validator.unobtrusive.parse(document);
    form.executed = true;
}

function SubmitLocation() {
    $("#vldLocations").removeClass("validation-summary-errors");
    $("#vldLocations").addClass("validation-summary-valid");
    if (IsValidForm('#frmlocations')) {
        LoadingPanel.ContainerElementID = "pcModalLocations";
        LoadingPanel.Show();

        var data = $("#frmlocations").serialize();
        $.post("/Products/_LocationSubmit", data, function (result) {
            // Do something with the response `res`
            if (result.error == false) {
                $('#ProductLocationIds').append('<option selected value=' + result.id + '>' + result.location + '</option>');
                $("#ProductLocationIds").trigger("chosen:updated");
                $("select #LocationTypeId").trigger("chosen:updated");
                LoadingPanel.Hide();
                pcModalLocations.Hide();
            }
            else {
                var ul = $("#vldLocations ul");
                $("#vldLocations").addClass("validation-summary-errors");
                $("#vldLocations").removeClass("validation-summary-valid");
                ul.html("");
                ul.append("<li>" + result.errormessage + "</li>");
                $("#drp").focus();
                LoadingPanel.Hide();
            }
        }).fail(function (result) {
            var ul = $("#vldLocations ul");
            $("#vldLocations").addClass("validation-summary-errors");
            $("#vldLocations").removeClass("validation-summary-valid");
            ul.html("");
            ul.append("<li>" + result.errormessage + "</li>");
            $("#drp").focus();
        })

    }

}
function opengrouppopup(s, e) {


    var group = Productgroup.GetValue();
    if (group == null) return;
    $.ajax({
        url: "/Products/_CreateProductGroup?productGroup=" + group,
        success: function (result) {
            $('#ProductGroupIds').append('<option selected value=' + result.id + '>' + group + '</option>');
            $("#ProductGroupIds").trigger("chosen:updated");
            pcModalMode.Hide();
        }
    });

}
function openlocationpopup(s, e) {
    var location = Productlocation.GetValue();
    if (location == null) return;
    else
        alert(location);
}
function ModalBC_StockMove(s, e) {

    e.customArgs["Id"] = $('#selkeyMoveStock').val();


}
function moveStockSubmit() {
    $("#vldOrdDet").removeClass("validation-summary-errors");
    $("#vldOrdDet").addClass("validation-summary-valid");
    if (IsValidForm('#frmMoveStock')) {
        var data = $("#frmMoveStock").serializeArray();

        LoadingPanel.Show();
        $.post("/MoveStock/_MoveStockSubmit", data, function (result) {
            // Do something with the response `res`
            _stockListgv.Refresh();
            ModalStockMove.Hide();
            LoadingPanel.Hide();




        })
    }

}
function ShowPHistoryModal(e) {
    var productid = prdid.GetValue();

    if (productid == null || productid == "") { return alert("Please select product"); }


    ModelPriceHistory.Show();
    return false;

}
function movestockshow() {
    var id = $('#selkeyMoveStock').val();
    if (id == undefined || id == '') {
        $("#infoMsg").html("There is no product(s) under this location").show();
        $('#infoMsg').delay(3000).fadeOut();

    }
    else
        ModalStockMove.Show();

}
//////////////////////////////////////////////////////
////                Product Kit and Recipe FUNCTIONS   //////////////
/////////////////////////////////////////////////////
function ProductKitChanges(s, e) {

    var comboproductIds = comboBox.GetValue();
    $("#ProductKit").val(comboproductIds);

}
function AddKitProduct(kitTypes, tab) {
    debugger;
    var qty = $("#Quantity").val();
    var parentProductId = $(".productIds").val();
    var productId = prdid.GetValue();
    var productName = prdid.GetText();
    var ProductkitType = $("#ProductKitTypeId").val();
    if (kitTypes == 2) {
        if (ProductkitType == "" || ProductkitType == undefined || ProductkitType == null) {
            alert("Please select product kit types")
            return false;
        }

    }
    else {
        if (qty <= 0) {
            $("#Quantity").css({ "background-color": "#f9baba" });
            setTimeout(function () {
                $("#Quantity").removeAttr("style");
            }, 400);
            return false;
        }
    }
    data = { ParentProductId: parentProductId, ProductId: productId, ProductName: productName, Quantity: qty, KitType: kitTypes, ProductkitType: ProductkitType };
    var url = "/Products/AddProductKitItemsWithQuantity";
    if (tab) {
        url = "/Products/ConfirmAddedKitProducts";
    }
    $.post(url, data, function (result) {
        pcModalKitProduct.Hide();
        if (tab) {
            if (kitTypes == 1) {
                GetDevexControlByName(parentProductId + "productKitItems").Refresh();
            }
            if (kitTypes == 2) {
                GetDevexControlByName(parentProductId + "productgroupedItems").Refresh();
            }
            else if (kitTypes == 3) {
                GetDevexControlByName(parentProductId + "productRecipeItems").Refresh();
            }

        }
        else {

            $('#ProductKitIds').append('<option selected value=' + result.ProductId + '>' + result.ProductName + "" + "(" + (result.Quantity) + ")" + '</option>');
            $("#ProductKitIds").trigger("chosen:updated");
        }


    });
}
function AddKitGroup() {
    var ProductkitType = $("#ProductKitTypeId").val();
    var parentProductId = $(".productIds").val();
    var productId = prdid.GetValue();
    var productName = prdid.GetText();
    var qty = $("#Quantity").val();
    if (ProductkitType <= 0 || ProductkitType == null || ProductkitType == "" || ProductkitType == undefined) {
        alert("Product Kit is required.");
        return false;
    }
    data = { ParentProductId: parentProductId, ProductId: productId, ProductName: productName, Quantity: qty, ProductkitType: ProductkitType };
    var url = "/Products/AddProductKitItemsWithQuantity";
    $.post(url, data, function (result) {
        pcModalKitProduct.Hide();


        $('#ProductGroupIds').append('<option selected value=' + result.ProductId + '>' + result.ProductName + '</option>');
        $("#ProductGroupIds").trigger("chosen:updated");



    });

}
function EditKitProductView() {
    debugger;
    var qty = $("#Quantity").val();
    var kitId = $("#Id").val();
    var kitTypes = $("#ProductKitType").val();
    var productId = prdid.GetValue();
    var parentProductId = $(".parentProductId").val();
    var productKitTypeIds = $("#ProductKitTypeId").val();
    if (kitTypes == "Grouped") {
        if (productKitTypeIds == "" || productKitTypeIds == undefined || productKitTypeIds == null) {
            alert("Please select product kit types")
            return false;
        }

    }
    else {
        if (qty <= 0) {
            $("#Quantity").css({ "background-color": "#f9baba" });
            setTimeout(function () {
                $("#Quantity").removeAttr("style");
            }, 400);
            return false;
        }
    }
    data = { KitId: kitId, Quantity: qty, ProductId: productId, productKitTypeId: productKitTypeIds };
    var url = "/Products/SaveEditKitProduct";

    $.post(url, data, function (result) {
        pcModalKitProduct.Hide();
        if (kitTypes == "Kit") {
            GetDevexControlByName(parentProductId + "productKitItems").Refresh();
        }
        else if (kitTypes == "Grouped") {
            GetDevexControlByName(parentProductId + "productgroupedItems").Refresh();
        }
        else if (kitTypes == "Recipe") {
            GetDevexControlByName(parentProductId + "productRecipeItems").Refresh();
        }





    });

}
var producttypekit;
var ProductKitId;
var grouped = false;
function ProductKitPopUpCallBack(s, e) {
    debugger;
    e.customArgs["kitTypeId"] = producttypekit;
    e.customArgs["ProductKitId"] = ProductKitId;
    e.customArgs["Grouped"] = grouped;

}
function ProductKitPopUp(isEdit) {
    debugger;
    if (isEdit == undefined) { grouped = false; }
    producttypekit = isEdit;
    
    ProductKitId = null;
    pcModalKitProduct.Show();
}
function EditKitProduct(kitId, kitType) {

    producttypekit = kitType;
    ProductKitId = kitId;
    pcModalKitProduct.Show();
}
function ShowProductGroupPopUp() {
    grouped = true;
    pcModalKitProduct.Show();

}



/////////////////////////////////////////////////////


$(document).ready(function () {
    loadKitProductGridItemEvents();
});