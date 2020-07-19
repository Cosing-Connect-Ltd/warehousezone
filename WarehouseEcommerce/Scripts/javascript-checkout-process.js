$(function () {




});
function moveToNextStep(e) {
    var checkoutViewModels =
    {
        "AccountId": e.dataset.accountid,
        "AccountAddressId": e.dataset.accountaddressid,
        "ShippingAddressId": e.dataset.shippingaddressid,
        "DeliveryMethodId": e.dataset.deliverymethodid,
        "CollectionPointId": e.dataset.collectionpointid,
        "CurrentStep": e.dataset.currentstep,
        "ShipmentRuleId": e.dataset.shipmentruleid
    };

    changeUrlParameterValue(e.dataset.currentstep);
    getDataForNextStep(checkoutViewModels);
}

function getDataForNextStep(checkoutViewModels, step) {
    startLoading();
    $.ajax({
        url: basePath + '/Orders/_CheckoutProcessPartial',
        method: 'post',
        data: { checkoutViewModel: checkoutViewModels, step: step },
        dataType: 'html',
        success: function (data) {
            $(".checkout-main-div").html("").html(data);
            stopLoading();
            getAsteriskForRequiredFields();
        }
    });
}

function changeUrlParameterValue(currentStep) {
    var uri = location.href;
    var uriData = updateQueryStringParameter(uri, "step", currentStep);
    window.history.pushState("object or string", "", uriData);
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
                    var result = $.parseJSON(data[i]);
                    if (result !== null) {
                        $.dialog({
                            title: 'Information!',
                            content: result.Message
                        });
                        
                    }
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

function onAddressDropdownChange() {
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
    var nextStep = (deliveryMethodId === '2' ? 1 : 2);
    var checkoutViewModels =
    {
        "DeliveryMethodId": deliveryMethodId,
        "CurrentStep": nextStep,
        "ParentStep": 1
    }
    changeUrlParameterValue(nextStep);
    getDataForNextStep(checkoutViewModels, nextStep);
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
    changeUrlParameterValue(nextStep);
    getDataForNextStep(checkoutViewModels, nextStep);
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
    changeUrlParameterValue(nextStep);
    getDataForNextStep(checkoutViewModels, nextStep);
}

function goToOrderConfirmation() {
    var paymentMethodId = $("input[name='payment-method-radio']:checked").val();
    if (!paymentMethodId) {
        alert("Please select payment method.");
        return;
    }

    var isAddressSameForBilling = $("#IsAddressSameForBilling").val();
    var billingAddressId = $("#BillingAddressId").val();

    if (isAddressSameForBilling.toLowerCase() == "false" && !billingAddressId) {
        alert("Please select billing address.");
        return;
    }

    $.ajax({
        url: basePath + '/Orders/SetPaymentDetails',
        method: 'post',
        data: { paymentMethodId: paymentMethodId, isAddressSameForBilling: isAddressSameForBilling },
        dataType: 'json',
        success: function (data) {
            window.location.href = basePath + "/Orders/ConfirmOrder";
        }
    });
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
