
function SetProductGroupId(event) {
    $("#ProductGroupId").val(event.currentTarget.value);
}
function ValidEmail(email) {

    var regExp = /(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])/i;
    return regExp.test(email);

}
function searchPoducts() {
    var sortvalue = $("#SortedValues").val();
    var category = $("#prod-category").val();
    var pagenumber = $("#pagenumber").val();
    var currentFilter = $("#currentFiltervalue").val();
    var searchstring = $("#searchString").val();
    var pageSize = $("#input-limit :selected").val();
    var valuesparam = $("#valuesParameter").val() == undefined ? "" : $("#valuesParameter").val();
    window.location.href = basePath + "/Products/list?category=" + category + "&sort=" + sortvalue + "&filter=" + currentFilter + "&search=" + searchstring + "&page=" + pagenumber + "&pagesize=" + pageSize + "&values=" + valuesparam;
}

function SearchPostCode() {
    var searchString = $(".text-search-postcode").val();

    $.ajax({
        url: basePath + '/Orders/GetApiAddressAsync',
        method: 'post',
        data: { postCode: searchString },
        dataType: 'json',
        success: function (data) {

            if (data.length > 1) {
                $('#selectAddresss').show();
                $('#selectApiAddress').empty();
                $('#selectApiAddress').append('<option>Select Address</option>');
                $.each(data, function (i, item) {
                    $('#selectApiAddress').append($('<option></option>').val(data[i]).html(data[i]));
                });
            }
            else {
                $.each(data, function (i, item) {
                    alert(data[i]);
                });
            }
        },
        error: function (err) {
            alert(err);
        }
    });
}

function findNearCollectionPoints() {
    var searchString = $(".text-target-postcode").val();

    $.ajax({
        url: basePath + '/Orders/GetNearWarehouses',
        method: 'post',
        data: { postCode: searchString },
        dataType: 'json',
        success: function (data) {
            $('.collection-points').empty();

            $.each(data, function (i, item) {
                var element = $('<div class="col-lg-6">' +
                    '<div class="billigAddrWrap collection-address-div" onclick="selectCollectionPointId(' + item.WarehouseId + ', this)">' +
                    '<p class="address">' +
                    item.WarehouseName + '<br />' +
                    item.PostalCode + '<br />' +
                    item.Address + '<br />' +
                    item.City + '<br />' +
                    item.CountryName + '<br />' +
                    '</p>' +
                    '<p class="miles">' + item.Distance.Distance.Text + '</p>' +
                    '</div></div>');
                $('.collection-points').append(element);
            });
        },
        error: function (err) {
            alert(err);
        }
    });
}

function selectCollectionPointId(collectionPointId, element) {
    $(".collection-point-selected").each(function (i, selectedElement) {
        $(selectedElement).removeClass("collection-point-selected");
    });

    $(element).addClass("collection-point-selected");

    $("#collectionPointId")[0].value = collectionPointId;
}

function OnchangeDropdownAddress() {
    var selOption = $('#selectApiAddress :selected').val().split(",");
    var PostCode = document.getElementById("postCode").value;

    if (selOption.length > 0) {
        var addressLine1 = "";

        for (var i = 0; i <= 1; i++) {
            addressLine1 += (selOption[i] != " " ? (addressLine1 != "" ? ', ' : '') + selOption[i] : '')
        }

        var addressLine2 = "";

        for (var i = 2; i <= 4; i++) {
            addressLine2 += (selOption[i] != " " ? (addressLine2 != "" ? ', ' : '') + selOption[i] : '')
        }

        var addressLine3 = "";

        for (var i = 5; i <= 6; i++) {
            addressLine3 += (selOption[i] != " " ? (addressLine3 != "" ? ', ' : '') + selOption[i] : '')
        }

        $(".AddressLine1").val(addressLine1);
        $(".AddressLine2").val(addressLine2);
        $(".AddressLine3").val(addressLine3);
        $(".PostCode").val(PostCode);
    }
}

function SearchProductCategory() {
    var searchstring = $(".text-search").val();
    var valuesparam = $("#valuesParameter").val();
    window.location.href = basePath + "/Products/list?search=" + searchstring + "&values=" + valuesparam;
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
                data: { searchkey: request.term },
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
                var $li = $("<li>");
                var $link = $("<a>", {
                    href: basePath + "/Products/list?search=" + $('.text-search').val(),
                    class: "see-all"
                }).html("See All Results").appendTo($li);
                $li.appendTo($('.ui-autocomplete'));
            }
        },
        focus: updateTextBox,
        select: updateTextBox
    }).autocomplete('instance')._renderItem = function (ul, item) {
        return $('<li class="search-item">').append("")
            .append("<a href=" + basePath + "/Products/list?search=" + item.Name + "><img style='width: 46px; height:46px;' src=" + item.Path + " alt=" + item.Name + "/>" + item.Name + "</a>").appendTo(ul);
    };
});

function updateTextBox(event, ui) {
    $(this).val(ui.item.Name);
    return false;
}
function getTopCategoryProducts(ProductnavigationId) {
    $.ajax({
        type: "GET",
        url: basePath + "/Home/_TopCategoryProductsPartial/",
        data: { NavigationId: ProductnavigationId },
        dataType: 'html',
        success: function (data) {
            $('#top_category_products').empty();
            $('#top_category_products').html(data);
            $('li a').removeClass("blue");
            $("#catID_" + ProductnavigationId).addClass("blue");
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });
}

//--------add update remove cartitem----------------
function AddToCart(ProductId) {
    var quantity = $(".input-number").val() ?? 1;
    $.ajax({
        type: "GET",
        url: basePath + "/Products/AddCartItem/",
        data: { ProductId: ProductId, quantity: quantity },
        dataType: 'json',
        success: function (data) {
            var cardItemsValue = parseInt($("#cart-total").text());
            $("#cart-total").text(cardItemsValue + 1);
            GetCartitems(data)
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });
}
function RemoveCartItem(id) {
    $.ajax({
        type: "GET",
        url: basePath + "/Products/RemoveCartItem/",
        data: { cartId: id },
        dataType: 'json',
        success: function (data) {
            var cardItemsValue = parseInt($("#cart-total").val());
            $("#cart-total").val(cardItemsValue - 1);
            GetCartitems(null);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });
}
function UpdateCartItem(id, event) {
    var quantity = event.value;
    if (quantity > 0) {

        $.ajax({
            type: "GET",
            url: basePath + "/Products/EditCartItem/",
            data: { cartId: id, quantity: quantity },
            dataType: 'json',
            success: function (data) {

                GetCartitems(null)

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                alert('Error' + textStatus + "/" + errorThrown);
            }
        });

    }
}
function GetCartitems(cartId) {
    $.ajax({
        type: "GET",
        url: basePath + "/Products/_CartItemsPartial/",
        data: { cartId: cartId },
        dataType: 'html',
        success: function (data) {

            if (cartId != null) {
                $('.modal-body').empty();
                $('.modal-body').html(data);
                $('#myModal').modal('show');
            }
            else {
                $('#updateCart').empty();
                $('#updateCart').html(data)

            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });



}

//-----------------------------------------------------------



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
    if (!paymentMethodId) {
        alert("Select payment type before placing order");
        return;
    }
    if (!ShippingTypeId) {
        alert("Select shipping type before placing order");
        return;
    }

    window.location.href = basePath + "/Orders/ConfirmOrder/?accountAddressId=" + AccountAddressIds + "&paymentTypeId=" + paymentMethodId + "&deliveryMethodId=" + ShippingTypeId;
}
function updateQueryStringParameter(uri, key, value) {
    var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
    var separator = uri.indexOf('?') !== -1 ? "&" : "?";
    if (uri.match(re)) {
        return uri.replace(re, '$1' + key + "=" + value + '$2');
    }
    else {
        return uri + separator + key + "=" + value;
    }
}
function updateQueryAndGoToStep(step, key, value) {
    var uri = location.href;

    uri = updateQueryStringParameter(uri, "step", step);

    if (!!key && !!value) {
        uri = updateQueryStringParameter(uri, key, value);
    }

    location.href = uri;
}

function getUrlVariables() {
    var variables = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        variables.push(hash[0]);
        variables[hash[0]] = hash[1];
    }
    return variables;
}

function chooseDeliveryMethod() {
    var deliveryMethodId = $("input[name='delivery-method-radio']:checked").val();
    if (!deliveryMethodId) {
        alert("Please select shipping method.");
        return;
    }
    var nextStep = (deliveryMethodId === '2' ? 2 : 3);
    var checkoutViewModels =
    {
        "DeliveryMethodId": deliveryMethodId,
        "CurrentStep": nextStep,
        "ParentStep":1
    }
    ChangeUrlParameterValue(nextStep);
    GetDataForNextStep(checkoutViewModels, nextStep);
}

function chooseCollectionPoint(nextStep) {
    var collectionPointId = $("#collectionPointId")[0].value;
    if (!collectionPointId) {
        alert("Please select collection point.");
        return;
    }
    var checkoutViewModels =
    {
        "CollectionPointId": collectionPointId,
        "CurrentStep": nextStep,
        "ParentStep": 3
    }
    ChangeUrlParameterValue(nextStep);
    GetDataForNextStep(checkoutViewModels, nextStep);
}

function goToStep(step) {
    updateQueryAndGoToStep(step, null, null);
}

function chooseShippingRule(nextStep) {
    var shipmentRuleId = $("input[name='shipment-rule-radio']:checked").val();
    if (!shipmentRuleId) {
        alert("Please select shipping rule.");
        return;
    }
    var checkoutViewModels =
    {
        "ShipmentRuleId": shipmentRuleId,
        "CurrentStep": nextStep,
        "ParentStep": 4
    }
    ChangeUrlParameterValue(nextStep);
    GetDataForNextStep(checkoutViewModels,nextStep);
}

function choosePaymentMethod() {
    var paymenttypeId = $("input[name='payment-method-radio']:checked").val();
    if (!paymenttypeId) {
        alert("Please select payment method.");
        return;
    }

    //var variables = getUrlVariables();


    window.location.href = basePath + "/Orders/ConfirmOrder?paymentTypeId=" + paymenttypeId;
    //    "&paymentTypeId=" + paymenttypeId +
    //    "&deliveryMethodId=" + variables["deliveryMethodId"] +
    //    (!!variables["shipmentRuleId"] ? "&shipmentRuleId=" + variables["shipmentRuleId"] : "") +
    //    (!!variables["shippingAddressId"] ? "&shippingAddressId=" + variables["shippingAddressId"] : "") +
    //    (!!variables["collectionPointId"] ? "&collectionPointId=" + variables["collectionPointId"] : "");

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
    //getTopCategoryProducts(id);
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
    var arr = [];
    var data = "";
    var str = $(location).attr('href');
    var pagerep = new RegExp("&page=\\d+");
    str = str.replace(pagerep, '');
    str = removeURLParameter(str, "page");
    str = removeURLParameter(str, "values");
    var parameter = "&values=";
    str = str + parameter;
    $(":checkbox").each(function () {

        if (this.checked) {
            if (str.indexOf($(this).val()) < 0) {
                arr.push($(this).val());
                data = this.id;
                if (str.indexOf(data) < 0) {
                    var indexof = str.lastIndexOf("=");
                    var checkpreviousString = str.substr(indexof - 6, 6);
                    var found = str.charAt(indexof + 1);
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
function GetLoggedIn(placeholder, topheader) {

    $.ajax({
        type: "GET",
        url: basePath + "/User/Login/",
        data: { PlaceOrder: placeholder },
        dataType: 'Html',
        success: function (data) {
            $(".login-model-body").html("");
            $(".login-model-body").html(data);
            if (placeholder !== "") {
                $("#Placecheck").val(placeholder);
            }
            $('#signupPopup').modal('show');
            $(".registration_form_sec").removeClass("show");
            $(".login_form_sec").addClass("show");
            $(".error").remove();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });
    if (!topheader) {
        $('#top-header').load(basePath + "/Home/_TopHeaderPartial");
    }

};
function LoggedIn() {
    var UserName = $("#UserName").val();
    var UserPassword = $("#UserPassword").val();
    var placecheck = $("#Placecheck").val();
    $(".error").remove();
    if (UserName.length < 1) {
        $('#UserName').after('<span class="error">This field is required</span>');
        return;
    }
    else {

        if (!ValidEmail(UserName)) {

            $('#UserName').after('<span class="error">Enter a valid email</span>');

            return;
        }
    }
    if (UserPassword.length < 1) {

        $('#UserPassword').after('<span class="error">This field is required</span>');

        return;
    }
    var PlaceOrders;
    if (placecheck !== "" && placecheck !== undefined && placecheck != null) {
        PlaceOrders = true;
    }
    $.ajax({
        type: "GET",
        url: basePath + "/User/LoginUsers/",
        data: { UserName: UserName, UserPassword: UserPassword, PlaceOrder: PlaceOrders, Popup: placecheck },
        dataType: 'json',
        success: function (data) {
            if (!data.status) {

                $(".alert-message-login").html("User name or password is not correct").show().delay(2000).fadeOut();

            }
            else if (PlaceOrders) {
                $('#signupPopup').modal('hide');
                location.href = "/Orders/Checkout?accountId=" + data.AccountId;
            }
            else {
                if (data.status) {
                    $(".alert-message-login").html("Successfully logged in.").show().delay(5000).fadeOut();
                    $('#signupPopup').modal('hide');
                    $('#AccountNameHref').removeAttr("data-target");
                    $(".temproryShow").prop('title', data.Name);
                    location.reload();
                }



            }


        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".alert-message-login").html('Error' + textStatus + "/" + errorThrown).show().delay(5000).fadeOut();

        }
    });
}
function CreateUsers() {
    if (!$('#userRegister').valid()) { return; }

    var placecheck = $("#Placecheck").val();
    var PlaceOrders;
    if (placecheck !== "" && placecheck !== undefined && placecheck != null) {
        PlaceOrders = true;
    }
    $.ajax({
        type: "POST",
        url: basePath + "/User/CreateUser/",
        data: $("#userRegister").serialize(),
        success: function (data) {
            if (data) {
                $(".registration_form_sec").removeClass("show");
                $(".success-message-reg").html("Registration successful, please check email and activate your account").show();

                setTimeout(function () {
                    $(".success-message-reg").html("");
                    $('#signupPopup').fadeOut();
                    $('#signupPopup').modal('hide');
                }, 5000);
            }
            else {
                $(".alert-message-reg").html("Unable to complete operation, please contact support").show().delay(2000).fadeOut();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $(".alert-message-reg").html('Error' + textStatus + " /" + errorThrown).show().delay(5000).fadeOut();
        }
    });
}

function AddWishListItem(ProductId) {
    if (userStatus === "Login") {
        GetLoggedIn(false);
    }

    else {
        $.confirm({
            title: 'Confirm!',
            content: 'Would you like to be notified about this product!',
            buttons: {
                Yes: function () {
                    AddToWishList(ProductId, true);
                },
                No: function () {
                    AddToWishList(ProductId, false);
                }
            }
        });
    }
}

function AddToWishList(productId, notification) {
    var CurrentWisId = $("#wish_" + productId);
    $.ajax({
        type: "GET",
        url: basePath + "/Products/AddWishListItem/",
        data: { ProductId: productId, isNotfication: notification },
        dataType: 'json',
        success: function (data) {
            var ids = "#" + CurrentWisId[0].id;
            $(ids).find(".list-icon").css({ "color": "red" });
            $("#" + CurrentWisId[0].id).removeAttr("onclick", null);
            $("#" + CurrentWisId[0].id).attr("onclick", "RemoveWishListPopUp(" + productId + ")");
            var cardItemsValue = parseInt($("#WishList-total").text());

            $("#WishList-total").text(cardItemsValue + 1);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });

}


function RemoveWishListPopUp(RemoveProductId) {
    var CurrentId = $("#wish_" + RemoveProductId);
    $.confirm({
        title: 'Confirm!',
        content: 'Are you sure to delete this product from wishlist!',
        buttons: {
            Yes: function () {

                $.ajax({
                    type: "GET",
                    url: basePath + "/Products/RemoveWishList/",
                    data: { ProductId: RemoveProductId },
                    dataType: 'json',
                    success: function (data) {
                        var ids = "#" + CurrentId[0].id;
                        $(ids).find(".list-icon").css({ "color": "black" });
                        $("#" + CurrentId[0].id).removeAttr("onclick", null);
                        $("#" + CurrentId[0].id).attr("onclick", "AddWishListItem(" + RemoveProductId + ")");
                        var cardItemsValue = parseInt($("#WishList-total").text());
                        $("#WishList-total").text(data);
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert('Error' + textStatus + "/" + errorThrown);
                    }
                });
            },
            No: function () {

            }
        }
    });
}
function Logout() {
    $.confirm({
        title: 'Confirm!',
        content: 'Do you really want to logout?',
        buttons: {
            Yes: function () {
                window.location.href = basePath + "/User/logout";
            },
            No: function () {

            }
        }
    });
}
function RemoveWishItem(id) {
    $.ajax({
        type: "GET",
        url: basePath + "/Products/_wishlistItems/",
        data: { ProductId: id },
        dataType: 'html',
        success: function (data) {

            var cardItemsValue = parseInt($("#WishList-total").text());
            $("#WishList-total").val(cardItemsValue - 1);
            $('#UpdateWishList').empty();
            $('#UpdateWishList').html(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });


}

function UpdateUser() {
    var UserId = $("#AuthUser_UserId").val();
    var FirstName = $(".firstname").val();
    var LastName = $(".lastname").val();
    $(".error").remove();
    if (FirstName.length < 1) {
        $('.firstname').after('<span class="error">This field is required</span>');
        return;
    }
    if (LastName.length < 1) {
        $('.lastname').after('<span class="error">This field is required</span>');
        return;
    }
    $.ajax({
        type: "GET",
        url: basePath + "/User/UpdateUser/",
        data: { UserId: UserId, FirstName: FirstName, LastName: LastName },
        dataType: 'json',
        success: function (data) {

            if (data) {
                $(".alert-message").html("Contact information updated").show().delay(5000).fadeOut();
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });

}

$("#showRegForm").click(function () {
    var placeholder = false;
    $.ajax({
        type: "GET",
        url: basePath + "/User/Create/",
        data: { PlaceOrder: placeholder },
        dataType: 'Html',
        success: function (data) {
            $(".register-model-body").html("");
            $(".register-model-body").html(data);
            $(".registration_form_sec").addClass("show");
            $(".login_form_sec").removeClass("show");
            $(".error").remove();
            $.validator.unobtrusive.parse("#userRegister");

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });

});
$("#showLoginForm").click(function () {
    placeholder = false;
    $.ajax({
        type: "GET",
        url: basePath + "/User/Login/",
        data: { PlaceOrder: placeholder },
        dataType: 'Html',
        success: function (data) {
            $(".login-model-body").html("");
            $(".login-model-body").html(data);
            $(".registration_form_sec").removeClass("show");
            $(".login_form_sec").addClass("show");
            $(".error").remove();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });

});
