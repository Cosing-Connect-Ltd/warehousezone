function SetProductGroupId(event) {
    $("#ProductGroupId").val(event.currentTarget.value);
}

function searchPoducts() {
    var sortvalue = $("#SortedValues").val();
    var groupId = $("#ProductGroups").val();
    if (groupId === "" || groupId === undefined || groupId === null) { groupId = $("#ProductGroupsId").val(); }
    var pagenumber = $("#pagenumber").val();
    var departmentId = $("#departmentId").val();
    var currentFilter = $("#currentFiltervalue").val();
    var searchstring = $("#searchString").val();
    var pageSize = $("#input-limit :selected").val();
    window.location.href = basePath + "/shop/Products/ProductCategories?productGroupId=" + groupId + "&sortOrder=" + sortvalue + "&currentFilter=" + currentFilter + "&searchString=" + searchstring + "&page=" + pagenumber + "&pagesize=" + pageSize + "&departmentId=" + departmentId;
}

function SearchProductCategory() {
    var productgroupId = $("#ProductGroups").val();
    var searchstring = $("#text-search").val();
    window.location.href = basePath + "/shop/Products/ProductCategories?productGroupId=" + productgroupId + "&searchString=" + searchstring;
}

var searchvalues;
var seeall = true;
$('#text-search').autocomplete({
    minLength: 2,
    source: function (request, response) {
        searchvalues = request.term;
        $.ajax({
            url: basePath + '/Products/searchProduct',
            method: 'post',
            data: { groupId: $("#ProductGroups").val(), searchkey: request.term },
            dataType: 'json',
            success: function (data) {
                seeall = true;
                if (!data.length) {
                    seeall = false;
                    data = [
                        {
                            Name: 'No matches found',
                            Path: baseFilePath + '/UploadedFiles/Products/no_image.gif'
                        }
                    ];
                }

                response(data);
            },
            error: function (err) {
                alert(err);
            }
        });
    },
    open: function () {

        if (seeall) {
            var $li = $("<li>");
            var $link = $("<a>", {
                href: basePath + "/Shop/Products/ProductCategories?productGroupId=" + $("#ProductGroups").val() + "&searchString=" + searchvalues,
                class: "see-all"
            }).html("See All Results").appendTo($li);
            $li.appendTo($('.ui-autocomplete'));
        }
    },
    focus: updateTextBox,
    select: updateTextBox
}).autocomplete('instance')._renderItem = function (ul, item) {
    return $('<li class="search-item">')
        .append("<img style='width: 46px; height:46px;' src=" + item.Path + " alt=" + item.Name + "/>")
        .append("<a href=" + basePath + "/Shop/Products/ProductCategories?productGroupId=" + $("#ProductGroups").val() + "&searchString=" + item.Name + ">" + item.Name + "</a>").appendTo(ul);
};

function updateTextBox(event, ui) {
    $(this).val(ui.item.Name);
    return false;
}
function getTopCategoryProducts(ProductGroupId) {
    $.ajax({
        type: "GET",
        url: basePath + "/shop/Home/_TopCategoryProductsPartial/",
        data: { ProductGroupId: ProductGroupId },
        dataType: 'html',
        success: function (data) {
            $('#top_category_products').empty();
            $('#top_category_products').html(data);
            $('li a').removeClass("blue");
            $("#catID_" + ProductGroupId).addClass("blue");
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });
}
function AddToCart(ProductId) {
    var quantity = $(".input-number").val();
    var detail = $(".input-number").data("detail");
    $.ajax({
        type: "GET",
        url: basePath + "/shop/Products/_CartItemsPartial/",
        data: { ProductId: ProductId, qty: quantity, details: detail },
        dataType: 'html',
        success: function (data) {
            var cardItemsValue = parseInt($("#cart-total").text());
            $("#cart-total").text((cardItemsValue + 1));
            $('.modal-body').empty();
            $('.modal-body').html(data);
            $('#myModal').modal('show');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });
}
function RemoveCartItem(id) {
    $.ajax({
        type: "GET",
        url: basePath + "/shop/Products/_CartItemsPartial/",
        data: { ProductId: id, Remove: true },
        dataType: 'html',
        success: function (data) {
            var cardItemsValue = parseInt($("#cart-total").val());
            $("#cart-total").val((cardItemsValue - 1));
            $('#updateCart').empty();
            $('#updateCart').html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });
}

function UpdateCartItem(ID, event) {
    var quantity = event.value;
    $.ajax({
        type: "GET",
        url: basePath + "/shop/Products/_CartItemsPartial/",
        data: { ProductId: ID, qty: quantity },
        dataType: 'html',
        success: function (data) {
            $('#updateCart').empty();
            $('#updateCart').html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });
}

function CartItemCount() {
    $.ajax({
        type: "GET",
        url: basePath + "/shop/Products/CartItemsCount/",
        dataType: 'json',
        success: function (data) {
            $("#cart-total").text("");
            $("#cart-total").text(data);
            $('#myModal').modal('hide');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });
    $('#top-header').load(basePath + "/shop/Home/_TopHeaderPartial");
}

function ConfirmOrder() {
    var paymentMethodId = $("#PaymentType").val();
    var ShippingTypeId = $("#ShippingType").val();
    var AccountAddressIds = $("#AccountAddressId").val();
    if ((paymentMethodId === undefined || paymentMethodId === 0 || paymentMethodId === null)) {

        alert("Select payment type before placing order");
        return;
    }
    if (ShippingTypeId === undefined || ShippingTypeId === 0 || ShippingTypeId === null) {
        alert("Select shipping type before placing order");
        return;
    }

    window.location.href = basePath + "/shop/Orders/ConfirmOrder/?AccountAddressId=" + AccountAddressIds + "&PaymentTypeId=" + paymentMethodId + "&ShippmentTypeId=" + ShippingTypeId;
}

function ChooseShippingAddress(addressId) {
    var ShippmentId = $("input[name='optradio']:checked").val();
    var accountId = $("#AccountID").val();
    if (ShippmentId === undefined) {
        alert("Please select shipping method.");
        return;
    }

    location.href = basePath + "/shop/Orders/ConfirmOrder?accountId=" + accountId + "&AccountAddressId=" + addressId + "&ShippmentTypeId=" + ShippmentId;
}

$("input[name='paymentMethod']").on("click", function (e) {

    if (e.target.value === "2") {
        $(".paypal-btn").hide();
        $(".cash-btn").show();
    }
    else {
        $(".paypal-btn").show();
        $(".cash-btn").hide();
    }


});
$(document).ready(function () {

    $(".cash-btn").hide();
    $('#PayPal').prop('checked', true);

    var id = $('#ProductGroupIds').val();
    getTopCategoryProducts(id);
});


function onCurrencyChange(event) {

    var currencyId = event.currentTarget.id;
    var cartview = $("#cartView").val();
    if (cartview) {
        var r = confirm("You are going to change the currency!");
        if (r) {
            var cId = parseInt(event.currentTarget.id);
            $.ajax({
                url: basePath + '/Products/CurrencyChanged',
                type: "GET",
                data: { CurrencyId: cId },
                success: function (data) {
                    location.reload();
                    return;
                },
                error: function (xhr, textStatus, errorThrown) {
                    location.reload();
                }
            });
        }
        else {
            return;
        }
    }

    $.ajax({
        url: basePath + '/Base/CurrencyDetail',
        type: "GET",
        data: { CurrencyId: currencyId },
        success: function (data) {
            location.reload();
        },
        error: function (xhr, textStatus, errorThrown) {
            location.reload();
        }
    });
}