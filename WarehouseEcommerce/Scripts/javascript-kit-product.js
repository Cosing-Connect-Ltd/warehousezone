
$(function () {

    $(".add-kit-product").on("click", function () {
        var wizardStep = 0;
        var same = 1;
        var ajaxCalls = [];
        var bodyContent = "";
        $(".kit-model-body").html("");
        $(".kit-cart-item-details").each(function () {


            var skuCode = $(this).find(".sku-code").text();
            var quantity = $(this).find(".product-quantity").text();
            ajaxCalls.push($.ajax({
                url: basePath + '/Products/_KitProductAttributeDetail',
                method: 'Get',
                data: { skuCode: skuCode, quantity: quantity },
                dataType: 'html',
                success: function (data) {

                    if (data === "False") {
                        return;
                    }

                    for (var i = 1; i <= parseInt(quantity); i++) {

                        wizardStep++;
                        bodyContent += "<fieldset data-step=" + wizardStep + " id=fieldset_" + wizardStep + (parseInt(quantity) == 1 ? "" : " data-same=" + (same)) + " > " + data + " </fieldset>";

                    }
                    same++;
                },
                error: function (err) {
                    alert(err);
                }
            }));
        });

        Promise.all(ajaxCalls).then(() => {
            if (wizardStep > 0 && bodyContent !== undefined) {

                $(".kit-model-body").html(bodyContent);
                $("#kit-product").modal("show");
                GenrateDropDownForEachStep();
            }
        }).catch(() => {
            alert("Something went wrong with getting the products!");
        });
    });
});

function getSelectedAttributes(e, skuCode, productId, quantity, stepNumber) {
    $.ajax({
        url: basePath + '/Products/_KitProductAttributeDetail',
        method: 'Get',
        data: { skuCode: skuCode, quantity: quantity, productId: productId },
        dataType: 'html',
        success: function (data) {
            var fieldsetElementId = "#fieldset_" + (!!stepNumber ? stepNumber : $("#kit-product").attr("data-current-step"));
            var dropdownFieldsBeforeRefresh = $(fieldsetElementId).find(".attribute-dropdown").html();
            $(fieldsetElementId).html(data);
            $(fieldsetElementId).find(".attribute-dropdown").html("").html(dropdownFieldsBeforeRefresh);
        },
        error: function (err) {
            alert(err);
        }
    });

}
function GenrateDropDownForEachStep(updatedstep = null) {

    var previousProduct = null;
    var qunatity = 0;
    var step = 1;
    $("[id^=fieldset_]").each(function (e) {
        if (previousProduct == null) {
            qunatity = $(this).find("#QunatityCheckBox").val();
            previousProduct = $(this).data("same");
        }
        else if (previousProduct == $(this).data("same") && previousProduct !== null) {

            $(this).find("#DDQuantity").find("option").slice(0, (step > qunatity) ? (step - 1) : step).remove();
            step++;
            previousProduct = $(this).data("same");
        }
        else {
            previousProduct = $(this).data("same");
            qunatity = $(this).find("#customCheck").val();
            step = 1;

        }

    });
}


