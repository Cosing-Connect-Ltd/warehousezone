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
        "ParentStep": e.dataset.parentstep,
        "ShipmentRuleId": e.dataset.shipmentruleid
        
    }


    ChangeUrlParameterValue(e.dataset.currentstep);
    GetDataForNextStep(checkoutViewModels);
}
function GetDataForNextStep(checkoutViewModels, checkoutStep) {
   
    $.ajax({
        url: basePath + '/Orders/_CheckoutProcessPartial',
        method: 'post',
        data: { checkoutViewModel: checkoutViewModels, checkoutStep: checkoutStep},
        dataType: 'html',
        success: function (data) {
            debugger;

            $(".checkout-main-div").html("").html(data);

        }
    });

}

function ChangeUrlParameterValue(currentStep) {

    var uri = location.href;
    var uriData = updateQueryStringParameter(uri, "checkoutStep", currentStep);
    window.history.pushState("object or string", "", uriData);


}