$(function () {
    $(".add-kit-product").on("click", function () {
        var wizardStep = 0;
        var same = 1;
        var bodyContent = "";

        $(".kit-cart-item-details").each(function () {
            var skuCode = $(this).find(".sku-code").text();
            var quantity = $(this).find(".product-quantity").text();
            $.ajax({
                url: basePath + '/Products/_KitProductAttributeDetail',
                method: 'Get',
                async: false,
                data: { skuCode: skuCode, quantity: quantity },
                dataType: 'html',
                success: function (data) {
                    if (data === "False") {
                        return;
                    }

                    for (var i = 1; i <= parseInt(quantity); i++) {
                        wizardStep++;
                        bodyContent += "<fieldset data-step=" + wizardStep + " id=fieldset_" + wizardStep + (parseInt(quantity) == 1 ? " data-same=" + (same++) : "") + " > " + data + "</fieldset>";
                    }
                },
                error: function (err) {
                    alert(err);
                }
            });
        });

        if (wizardStep > 0 && bodyContent !== undefined) {
            $(".kit-model-body").html(bodyContent);
            $("#kit-product").modal("show");
        }
    });
});

function getSelectedAttributes(e, skuCode, productId, quantity, stepNumber) {
    $.ajax({
        url: basePath + '/Products/_KitProductAttributeDetail',
        method: 'Get',
        async: false,
        data: { skuCode: skuCode, quantity: quantity, productId: productId },
        dataType: 'html',
        success: function (data) {
            var fieldsetElementId = "#fieldset_" + (!!stepNumber ? stepNumber : $("#kit-product").attr("data-current-step"));
            $(fieldsetElementId).html(data);
        },
        error: function (err) {
            alert(err);
        }
    });

}