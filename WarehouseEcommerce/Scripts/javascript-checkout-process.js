$(function () {




});
function moveToNextStep(e) {
    var checkoutViewModels =
    {
        "AccountId": e.dataset.accountid,
        "AccountAddressId": e.dataset.accountaddressid,
        "BillingAddressId": e.dataset.billingaddressid,
        "ShippingAddressId": e.dataset.shippingaddressid,
        "DeliveryMethodId": e.dataset.deliverymethodid,
        "CollectionPointId": e.dataset.collectionpointid,
        "CurrentStep": e.dataset.currentstep,
        "ShipmentRuleId": e.dataset.shipmentruleid

    };

    ChangeUrlParameterValue(e.dataset.currentstep);
    GetDataForNextStep(checkoutViewModels);
}
function GetDataForNextStep(checkoutViewModels, step) {
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

function ChangeUrlParameterValue(currentStep) {

    var uri = location.href;
    var uriData = updateQueryStringParameter(uri, "step", currentStep);
    window.history.pushState("object or string", "", uriData);


}