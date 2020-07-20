







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
                    '<div class="billigAddrWrap collection-address-div" onclick="selectCollectionPointId(' + item.WarehouseId + ", '" + item.WarehouseName + "', '" + item.PostalCode + "', '" + item.Address + "', " + 'this)">' +
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

function selectCollectionPointId(id, name, postcode, address, element) {
    $(element).addClass("collection-point-selected");

    if ($("#collectionPointId").length > 0) {
        $("#collectionPointId")[0].value = id;
    }

    if ($("#collectionPointName").length > 0) {
        $("#collectionPointName")[0].value = name;
    }

    if ($("#collectionPointAddress").length > 0) {
        $("#collectionPointAddress")[0].value = address;
    }

    if ($("#collectionPointPostCode").length > 0) {
        $("#collectionPointPostCode")[0].value = postcode;
    }

    if (!!onCollectionPointSelected && typeof onCollectionPointSelected === "function") {
        onCollectionPointSelected(id, name, postcode, address)
    }
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
    var valuesparam = $("#valuesParameter").val() == undefined ? "" : $("#valuesParameter").val();
    var path = basePath + "/Products/list?search=" + searchstring + "&values=" + valuesparam;
    window.location.href = path;
}




//$('.text-search').on('input', function() {}
var searchvalues;
var seeall = true;
$(function () {
    $(".text-search").autocomplete({
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
                                Name: 'No Search Results, please try another search ',
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
                var $li = $('<li class="see-all-li">');
                var $link = $("<a>", {
                    href: basePath + "/Products/list?search=" + searchvalues.replace(" ", "_"),
                    class: "see-all"
                }).html("See All Results").appendTo($li);
                $li.appendTo($('.ui-autocomplete'));
            }
        },
        focus: updateTextBox,
        select: updateTextBox
    }).autocomplete('instance')._renderItem = function (ul, item) {
        if (seeall) {
            return $('<li class="search-item">').append("")
                .append("<a href=" + basePath + "/Products/list?search=" + item.Name + "><img style='width: 46px; height:46px;' src=" + encodeURI(item.DefaultImage) + " alt=" + item.Name + "/>" + item.Name + "</a>").appendTo(ul);
        }
        else {
            return $('<li class="search-item">').append("")
                .append('<p class="no-serach-results">' + item.Name + "</p>").appendTo(ul);
        }

    };
    $(".text-search").bind('keypress', function (e) {
        if (e.keyCode === 13) {
            $('#serchBtnico').trigger('click');
        }
    });

   
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
function addToCart(ProductId, quantity) {

    if (!quantity) {
        quantity = 1;
    }

    startLoading();
    $.ajax({
        type: "GET",
        url: basePath + "/Products/AddCartItem/",
        data: { ProductId: ProductId, quantity: quantity },
        dataType: 'json',
        success: function (data) {
            stopLoading();
            var cardItemsValue = parseInt($("#cart-total").text());
            $("#cart-total").text(cardItemsValue + 1);
            getCartitems(data);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            stopLoading();
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });
}

function removeCartItem(id) {
    $.confirm({
        title: 'Confirm!',
        content: 'Are you sure to delete this item from cart?',
        buttons: {
            Yes: function () {
                $.ajax({
                    type: "GET",
                    url: basePath + "/Products/RemoveCartItem/",
                    data: { cartId: id },
                    dataType: 'json',
                    success: function (data) {
                        var cardItemsValue = parseInt($("#cart-total").text());
                        $("#cart-total").text(cardItemsValue - 1);
                        getCartitems(null);
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert('Error' + textStatus + "/" + errorThrown);
                    }
                });
            },
            No: function () {
                this.close();
            }
        }
    });
}

function updateCartItem(id, quantity) {
    startLoading();
    if (!!quantity && quantity > 0) {
        $.ajax({
            type: "GET",
            url: basePath + "/Products/EditCartItem/",
            data: { cartId: id, quantity: quantity },
            dataType: 'json',
            success: function (data) {
                stopLoading();
                getCartitems(null);

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                stopLoading();
                alert('Error' + textStatus + "/" + errorThrown);
            }
        });
    }
}

function getCartitems(cartId) {
    startLoading();
    $.ajax({
        type: "GET",
        url: basePath + "/Products/_CartItemsPartial/",
        data: { cartId: cartId },
        dataType: 'html',
        success: function (data) {
            stopLoading();
            if (cartId != null) {
                $('.cart-item-data').html("");
                $('.cart-item-data').html(data);
                $('#myModal').modal('show');
            }
            else {
                $('#updateCart').html("");
                $('#updateCart').html(data);

            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            stopLoading();
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
    startLoading();
    $.ajax({
        type: "GET",
        url: basePath + "/User/LoginUsers/",
        data: { UserName: UserName, UserPassword: UserPassword, PlaceOrder: placecheck },
        dataType: 'json',
        success: function (data) {
            stopLoading();
            if (!data.status) {

                $(".alert-message-login").html("User name or password is not correct").show().delay(2000).fadeOut();

            }
            else if (placecheck == "true") {
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
            stopLoading();
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
    startLoading();
    $.ajax({
        type: "POST",
        url: basePath + "/User/CreateUser/",
        data: $("#userRegister").serialize(),
        success: function (data) {
            stopLoading();
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
            stopLoading();
            $(".alert-message-reg").html('Error' + textStatus + " /" + errorThrown).show().delay(5000).fadeOut();
        }
    });
}

function AddWishListItemStockNotAvailable(productId) {
    if (userStatus === "Login") {
        GetLoggedIn(false);
    }

    else {
        $.confirm({
            title: 'Confirm!',
            content: 'Would you like to be notified when this item is back in stock?',
            buttons: {
                Yes: function () {
                    addToNotify(productId, true);
                },
                No: function () {

                }
            }
        });
    }
}

function AddToWishList(productId, notification) {
    if (userStatus === "Login") {
        GetLoggedIn(false);
        return;
    }
    var currentWishId = $("#wish_" + productId);
    var wishListBtnId = $("#btnwish_" + productId);
    $.ajax({
        type: "GET",
        url: basePath + "/Products/AddWishListItem/",
        data: { ProductId: productId, isNotfication: notification },
        dataType: 'json',
        success: function (data) {
            var iconId = currentWishId[0] == null ? "" : "#" + currentWishId[0].id;
            $(iconId).find(".list-icon").css({ "color": "red" });
            $(iconId).removeAttr("onclick", null);
            $(iconId).attr("onclick", "RemoveWishListPopUp(" + productId + ",false)");
            var cardItemsValue = parseInt($("#WishList-total").text());
            $(wishListBtnId).css("cssText", "background-color: red !important;");
            $(wishListBtnId).removeAttr("onclick", null);
            $(wishListBtnId).attr("onclick", "RemoveWishListPopUp(" + productId + ",false)");
            $("#WishList-total").text(cardItemsValue + 1);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });

}


function addToNotify(productId, notification) {
    if (userStatus === "Login") {
        GetLoggedIn(false);
        return;
    }
    var currentClass = $(".notification-bell_" + productId);
    var notifyBtnClass = $(".btn_notification-bell_" + productId);
    $.ajax({
        type: "GET",
        url: basePath + "/Products/AddWishListItem/",
        data: { ProductId: productId, isNotfication: notification },
        dataType: 'json',
        success: function (data) {

            $(currentClass).find(".list-icon").css({ "color": "red" });
            $(currentClass).removeAttr("onclick", null);
            $(currentClass).attr("onclick", "removeNotifyProduct(" + productId + ",'true')");
            $(notifyBtnClass).css({ "background-color": "red" });
            $(notifyBtnClass).removeAttr("onclick", null);
            $(notifyBtnClass).attr("onclick", "removeNotifyProduct(" + productId + ",'true')");


        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });


}

function RemoveWishListPopUp(removeProductId, notification) {
    if (userStatus === "Login") {
        GetLoggedIn(false);
        return;
    }
    var currentWishId = $("#wish_" + removeProductId);
    var wisListBtnId = $("#btnwish_" + removeProductId);


    $.confirm({
        title: 'Confirm!',
        content: 'This item is already on your wishlist, would you like to remove it?',
        buttons: {
            Yes: function () {

                $.ajax({
                    type: "GET",
                    url: basePath + "/Products/RemoveWishList/",
                    data: { ProductId: removeProductId, notification: notification },
                    dataType: 'json',
                    success: function (data) {
                        var iconId = currentWishId[0] == null ? "" : "#" + currentWishId[0].id;
                        $(iconId).find(".list-icon").css({ "color": "black" });
                        $(iconId).removeAttr("onclick", null);
                        $(iconId).attr("onclick", "AddToWishList(" + removeProductId + ",false)");
                        $(wisListBtnId).css({ "background-color": "black" });
                        $(wisListBtnId).removeAttr("onclick", null);
                        $(wisListBtnId).attr("onclick", "AddToWishList(" + removeProductId + ",false)");


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
function RemoveWishItem(id) {
    if (userStatus === "Login") {
        GetLoggedIn(false);
        return;
    }
    $.confirm({
        title: 'Confirm!',
        content: 'This item is already on your wishlist, would you like to remove it?',
        buttons: {
            Yes: function () {
                $.ajax({
                    type: "GET",
                    url: basePath + "/Products/_wishlistItems/",
                    data: { ProductId: id },
                    dataType: 'html',
                    success: function (data) {

                        var cardItemsValue = parseInt($("#WishList-total").text());
                        $("#WishList-total").text((cardItemsValue-1));
                        $('#UpdateWishList').empty();
                        $('#UpdateWishList').html(data);
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

function removeNotifyProduct(removeProductId, notification) {
    if (userStatus === "Login") {
        GetLoggedIn(false);
        return;
    }
    var currentClass = $(".notification-bell_" + removeProductId);
    var notifyBtnClass = $(".btn_notification-bell_" + removeProductId);
    $.confirm({
        title: 'Confirm!',
        content: 'Would you like to remove notification for this product?',
        buttons: {
            Yes: function () {

                $.ajax({
                    type: "GET",
                    url: basePath + "/Products/RemoveWishList/",
                    data: { ProductId: removeProductId, notification: notification },
                    dataType: 'json',
                    success: function (data) {
                        $(currentClass).find(".list-icon").css({ "color": "black" });
                        $(currentClass).removeAttr("onclick", null);
                        $(currentClass).attr("onclick", "AddWishListItemStockNotAvailable(" + removeProductId + ",true)");
                        $(notifyBtnClass).css({ "background-color": "black" });
                        $(notifyBtnClass).removeAttr("onclick", null);
                        $(notifyBtnClass).attr("onclick", "AddWishListItemStockNotAvailable(" + removeProductId + ",true)");

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


function UpdateUser() {
    var userId = $("#AuthUser_UserId").val();
    var firstName = $(".firstname").val();
    var lastName = $(".lastname").val();
    $(".error").remove();
    if (firstName.length < 1) {
        $('.firstname').after('<span class="error">This field is required</span>');
        return;
    }
    if (lastName.length < 1) {
        $('.lastname').after('<span class="error">This field is required</span>');
        return;
    }
    $.ajax({
        type: "GET",
        url: basePath + "/User/UpdateUser/",
        data: { UserId: userId, FirstName: firstName, LastName: lastName },
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
function OnEnter(e) {
    if (e.keyCode === 13) {
        LoggedIn();
    }
}
function ChangeWishListStatus(productId, notification) {

    $.ajax({
        type: "GET",
        url: basePath + "/Products/ChangeWishListStatus/",
        data: { productId: productId, notification: notification },
        dataType: 'json',
        success: function (data) {
           
            $.ajax({
                type: "GET",
                url: basePath + "/Products/_wishlistItems/",
                dataType: 'html',
                success: function (data) {

                    var cardItemsValue = parseInt($("#WishList-total").text());
                    $("#WishList-total").text((cardItemsValue - 1));
                    $('#UpdateWishList').empty();
                    $('#UpdateWishList').html(data);
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    alert('Error' + textStatus + "/" + errorThrown);
                }
            });
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
$(".notification-switch").change(function (e) {
    var productId = $(this).val();
    var notification = $(this).is(":checked");
    ChangeWishListStatus(productId, notification);


});
