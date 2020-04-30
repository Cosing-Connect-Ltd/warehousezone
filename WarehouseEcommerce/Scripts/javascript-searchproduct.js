function SetProductGroupId(event) {
    $("#ProductGroupId").val(event.currentTarget.value);
}

function searchPoducts() {
    debugger;
    var sortvalue = $("#SortedValues").val();
    var groupId = $("#ProductGroups").val();
    if (groupId === "" || groupId === undefined || groupId === null) { groupId = $("#ProductGroupsId").val(); }
    var pagenumber = $("#pagenumber").val();
    var departmentId = $("#departmentId").val() == undefined ? "" : $("#departmentId").val();
    var currentFilter = $("#currentFiltervalue").val();
    var searchstring = $("#searchString").val();
    var pageSize = $("#input-limit :selected").val();
    var valuesparam = $("#valuesParameter").val() == undefined ? "" : $("#valuesParameter").val();
    var SubCategory = $("#SubCategoryId").val() == undefined ? "" : $("#SubCategoryId").val();
    window.location.href = basePath + "/Products/list?group=" + groupId + "&sort=" + sortvalue + "&filter=" + currentFilter + "&search=" + searchstring + "&page=" + pagenumber + "&pagesize=" + pageSize + "&department=" + departmentId + "&category=" + SubCategory + "&values=" + valuesparam;
}

function SearchPostCode() {
    var searchString = $(".text-search-postcode").val();
    var selOption = '';
    $.ajax({
        url: basePath + '/Orders/GetApiAddressAsync',
        method: 'post',
        data: { postCode: searchString },
        dataType: 'json',
        success: function (data) {
            debugger;
            $('#selectAddresss').show();
            if (data.length > 0) {
                $('#selectApiAddress').empty();
                $('#selectApiAddress').append('<option>Select Address</option>');
                $.each(data, function (i, item) {
                    $('#selectApiAddress').append($('<option></option>').val(data[i]).html(data[i]));

                });
            }
        },
        error: function (err) {
            alert("Invalid Postcode please enter the correct format");
        }
    });
}

function OnchangeDropdownAddress() {
    debugger;
    var selOption = $('#selectApiAddress :selected').val().split(",");
    var PostCode = document.getElementById("postCode").value;

    if (selOption.length > 0) {
        $("#AddressLine1").val(selOption[0]);
        $("#AddressLine2").val(selOption[5]);
        $("#AddressLine3").val(selOption[6]);
        $("#PostCode").val(PostCode);
    }
}

function SearchProductCategory() {
    var productgroupId = $("#ProductGroups").val() == undefined ? "" : $("#valuesParameter").val();
    var searchstring = $(".text-search").val();
    var valuesparam = $("#valuesParameter").val();
    var SubCategory = $("#SubCategoryId").val() == undefined ? "" : $("#SubCategoryId").val();
    var departmentId = $("#departmentId").val() == undefined ? "" : $("#departmentId").val();
    window.location.href = basePath + "/Products/list?group=" + productgroupId + "&department=" + departmentId + "&search=" + searchstring + "&category=" + SubCategory + "&values=" + valuesparam;
}

var searchvalues;
var seeall = true;
$('.text-search').on('input', function () {
    $(this).autocomplete({
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
        open: function (data) {
            if (seeall) {
                debugger;
                var $li = $("<li>");
                var $link = $("<a>", {
                    href: basePath + "/Products/list?group=" + data.Group + "&search=" + data.SKUCode + "&department=" + data.Department,
                    class: "see-all"
                }).html("See All Results").appendTo($li);
                $li.appendTo($('.ui-autocomplete'));
            }
        },
        focus: updateTextBox,
        select: updateTextBox
    }).autocomplete('instance')._renderItem = function (ul, item) {
        debugger;
        return $('<li class="search-item">').append("<img style='width: 46px; height:46px;' src=" + item.Path + " alt=" + item.Name + "/>")
            .append("<a href=" + basePath + "/Products/list?group=" + item.Group + "&search=" + item.SKUCode + "&department=" + item.Department + ">" + item.Name + "</a>").appendTo(ul);
    };
});

function updateTextBox(event, ui) {
    $(this).val(ui.item.Name);
    return false;
}
function getTopCategoryProducts(ProductGroupId) {
    $.ajax({
        type: "GET",
        url: basePath + "/Home/_TopCategoryProductsPartial/",
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
        url: basePath + "/Products/_CartItemsPartial/",
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
        url: basePath + "/Products/_CartItemsPartial/",
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
        url: basePath + "/Products/_CartItemsPartial/",
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
        url: basePath + "/Products/CartItemsCount/",
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
    $('#top-header').load(basePath + "/Home/_TopHeaderPartial");
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

    window.location.href = basePath + "/Orders/ConfirmOrder/?AccountAddressId=" + AccountAddressIds + "&PaymentTypeId=" + paymentMethodId + "&ShippmentTypeId=" + ShippingTypeId;
}

function ChooseShippingAddress(accountid, billingaddressId, shippingaddressid, status) {
    var ShippmentId = $("input[name='optradio']:checked").val();
    if (ShippmentId === undefined) {
        alert("Please select shipping method.");
        return;
    }

    location.href = basePath + "/Orders/GetAddress?AccountId=" + accountid + "&AccountBillingId=" + billingaddressId + "&AccountShippingId=" + shippingaddressid + "&ShipingAddress=" + status + "&ShippmentTypeId=" + ShippmentId;
}
function ChoosePaymentMethod(accountid, shippingaddressid, shipmentMethodType) {
    var paymenttypeId = $("input[name='paymentMethod']:checked").val();
    if (paymenttypeId === undefined) {
        alert("Please select payment method.");
        return;
    }
    window.location.href = basePath + "/Orders/ConfirmOrder?AccountId=" + accountid + "&ShipmentAddressId=" + shippingaddressid + "&PaymentTypeId=" + paymenttypeId + "&ShippmentTypeId=" + shipmentMethodType;
    location.href = basePath + "/Orders/GetAddress?AccountId=" + accountid + "&AccountBillingId=" + billingaddressId + "&AccountShippingId=" + shippingaddressid + "&ShipingAddress=" + status + "&ShippmentTypeId=" + ShippmentId;
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
    $("#selectAddresss").hide();

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

$("input[type=checkbox]").on("change", function () {
    debugger;
    var arr = []
    var data = "";
    var str = $(location).attr('href');
    var pagerep = new RegExp("&page=\\d+");
    str = str.replace(pagerep, '')
    str = removeURLParameter(str, "page");
    str = removeURLParameter(str, "values");
    var parameter = "&values=";
    str = str + parameter;
    $(":checkbox").each(function () {

        if (this.checked) {
            if (str.indexOf($(this).val()) < 0) {
                arr.push($(this).val())
                data = this.id;
                if (str.indexOf(data) < 0) {
                    var indexof = str.lastIndexOf("=");
                    var checkpreviousString = str.substr((indexof - 6), 6);
                    var found = str.charAt((indexof + 1));
                    if (checkpreviousString === "values" && found === "") {
                        str = str + data + "-" + $(this).val();
                    }
                    else {
                        str = str + "/" + data + "-" + $(this).val();
                    }
                }
                else {
                    if (str.indexOf($(this).val()) < 0) {
                        str = str + "," + $(this).val();
                    }
                }
            }

        }

    });
    //if (arr.length <= 0) {
    //    if (str.indexOf("&") > 0) {
    //        var result = str.substring(str.indexOf("&"), (str.length));
    //        str = str.replace(result, "");
    //    }
    //}
    location.href = str;



});

function removeURLParameter(url, parameter) {
    //prefer to use l.search if you have a location/link object
    var urlparts = url.split('?');
    if (urlparts.length >= 2) {

        var prefix = encodeURIComponent(parameter) + '=';
        var pars = urlparts[1].split(/[&;]/g);
        for (var i = pars.length; i-- > 0;) {

            if (pars[i].lastIndexOf(prefix, 0) !== -1) {
                pars.splice(i, 1);
            }
        }

        url = urlparts[0] + '?' + pars.join('&');
        return url;
    } else {
        return url;
    }
}