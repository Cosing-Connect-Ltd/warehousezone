$(function () {

    $(".add-kit-product").on("click", function () {
        var productId;
        var skucode;
        var qty;
        var step = 0;
        var same = 1;
        var Tabs = [];
        $(".kit-cartitem-details").each(function () {
            $(".kit-model-body").html("");
            skucode = $(this).find(".sku-code").text();
            qty = $(this).find(".prod-qty").text();
            productId = $(this).find(".prod-id").text();
            $.ajax({
                url: basePath + '/Products/_KitProductAttributeDetail',
                method: 'Get',
                async: false,
                data: { skuCode: skucode, quantity: qty },
                dataType: 'html',
                success: function (data) {
                    debugger;
                    if (data !== "False") {
                        if (parseInt(qty) > 1) {
                            var totqty = qty;

                            for (i = 1; i <= parseInt(qty); i++) {
                                step = step + 1;
                                totqty = (i == 1 ? qty : (totqty - 1));
                                var tab = "<fieldset data-step=" + step + " id=fieldset_" + step + " data-same=" + same + " > " + data + "</fieldset> ";
                                Tabs.push(tab);
                            }
                            same = same + 1;
                        }
                        else {
                            step = step + 1;
                            var tab = "<fieldset data-step=" + step + " id=fieldset_" + step + ">" + data + "</fieldset>";
                            Tabs.push(tab);

                        }


                    }
                },
                error: function (err) {
                    alert(err);
                }
            });
        });
        if (step > 0 && Tabs !== undefined) {

            $(".kit-model-body").html(Tabs.join(''));

            $("#kit-product").modal("show");


        }

    });

});

function GetSelectedAttributes(e, skucode, productId, qty, stepNumber) {
    var step = $("#kit-product").attr("data-current-step");
    if (stepNumber !== undefined && stepNumber !== null && stepNumber !== "") {
        step = stepNumber;

    }
    $.ajax({
        url: basePath + '/Products/_KitProductAttributeDetail',
        method: 'Get',
        async: false,
        data: { skuCode: skucode, quantity: qty, productId: productId },
        dataType: 'html',
        success: function (data) {
            var fieldset = "#fieldset_" + step;
            $(fieldset).html("");
            $(fieldset).html(data);
        },
        error: function (err) {
            alert(err);
        }
    });

}