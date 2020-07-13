function showProductByAttributeSelector(skuCode) {
    $(".product-by-attribute-selector-model-body").html("");
    startLoading();
    $.ajax({
        url: basePath + '/Products/_ProductByAttributeSelector',
        method: 'Get',
        data: { skuCode: skuCode },
        dataType: 'html',
        success: function (data) {
            stopLoading();
            if (data !== "False") {
                $(".product-by-attribute-selector-model-body").html(data);
                $("#product-by-attribute-selector").modal("show");
            }
        },
        error: function (err) {
            stopLoading();
            alert(err);
        }
    });
}

function getSelectedAttributesProduct(e, skuCode, productId, quantity) {
    var body = $(".product-by-attribute-selector-model-body");
    startLoading();
    $.ajax({
        url: basePath + '/Products/_ProductByAttributeSelector',
        method: 'Get',
        data: { skuCode: skuCode, quantity: quantity, productId: productId },
        dataType: 'html',
        success: function (data) {
            stopLoading();
            body.html("");
            body.html(data);
        },
        error: function (err) {
            stopLoading();
            alert(err);
        }
    });
}

function addProductByAttributeToCart() {
    var productId = $.find("#ProductId")[0].value;

    if (productId == "" || productId == undefined || productId == null) {
        return;
    }

    addToCart(productId);

    $("#product-by-attribute-selector").modal("hide");
}