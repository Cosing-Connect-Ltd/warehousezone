$(function () {

    $(".add-kit-product").on("click", function () {
        var wizardStep = 0;
        var same = 1;
        var bodyContent = "";
        $(".kit-cartitem-details").each(function () {
            $(".kit-model-body").html("");
            var skuCode = $(this).find(".sku-code").text();
            var quantity = $(this).find(".product-quantity").text();
            $.ajax({
                url: basePath + '/Products/_KitProductAttributeDetail',
                method: 'Get',
                async: false,
                data: { skuCode: skuCode, quantity: quantity },
                dataType: 'html',
                success: function (data) {
                    if (data !== "False") {
                        if (parseInt(quantity) > 1) {

                            for (i = 1; i <= parseInt(quantity); i++) {
                                wizardStep++;
                                bodyContent += "<fieldset data-step=" + wizardStep + " id=fieldset_" + wizardStep + " data-same=" + (same++) + " > " + data + "</fieldset> ";
                            }
                        }
                        else {
                            wizardStep++;
                            bodyContent += "<fieldset data-step=" + wizardStep + " id=fieldset_" + wizardStep + ">" + data + "</fieldset>";

                        }


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
            var fieldset = "#fieldset_" + (!!stepNumber ? stepNumber : $("#kit-product").attr("data-current-step"));
            $(fieldset).html(data);
        },
        error: function (err) {
            alert(err);
        }
    });

}