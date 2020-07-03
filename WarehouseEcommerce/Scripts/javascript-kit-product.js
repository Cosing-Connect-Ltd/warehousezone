
$(function () {

    $(".add-kit-product").on("click", function () {
        var wizardStep = 0;
        var sameproduct = 1;
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
                        bodyContent += "<fieldset data-step=" + wizardStep + " id=fieldset-" + wizardStep + (parseInt(quantity) == 1 ? "" : " data-sameproduct=" + (sameproduct)) + " > " + data + " </fieldset>";

                    }
                    sameproduct++;
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
            var fieldsetElementId = "#fieldset-" + (!!stepNumber ? stepNumber : $("#kit-product").attr("data-current-step"));
            var dropdownFieldsBeforeRefresh = $(fieldsetElementId).find(".attribute-dropdown").html();
            var previousStepValue = $(fieldsetElementId).find("#step-number-tracking").val();
            $(fieldsetElementId).html(data);
            $(fieldsetElementId).find(".attribute-dropdown").html("").html(dropdownFieldsBeforeRefresh);
            $(fieldsetElementId).find("#step-number-tracking").val(previousStepValue);
        },
        error: function (err) {
            alert(err);
        }
    });

}
function GenrateDropDownForEachStep(updatedstep = null) {

    var previousProduct = null;
    var step = 1;
    var quantity = 0;
    $("[id^=fieldset-]").each(function (e) {
        if (previousProduct == null) {
            quantity = $(this).find("#quantity-checkbox").val();
            previousProduct = $(this).data("sameproduct");
        }
        else if (previousProduct == $(this).data("sameproduct") && previousProduct !== null) {

            $(this).find("#quantity-dropdown").find("option").remove();
            for (var i = 1; i <= (quantity - step); i++) {
                $(this).find("#quantity-dropdown").append('<option value=' + i + '>' + i + '</option>');;
            }


            step++;
            previousProduct = $(this).data("sameproduct");
        }
        else {
            previousProduct = $(this).data("sameproduct");
            quantity = $(this).find("#quantity-checkbox").val();
            step = 1;

        }

    });
}


