﻿function namespace(namespaceString) {
    var parts = namespaceString.split('.'),
        parent = window,
        currentPart;
    for (var i = 0, length = parts.length; i < length; i++) {
        currentPart = parts[i];
        parent[currentPart] = parent[currentPart] || {};
        parent = parent[currentPart];
    }
    return parent;
}

namespace("Gane");

var RowCount = 0;
var AllCount = 0;
var TotalCount = 0;

(function ($, gane) {
    var showDispatch = true;
    gane.PalletHelpers = function () {
        var onBeginCallbackPalletItem = function (s, e) {
            e.customArgs["orderDetailID"] = $("#divPalletingInformation #CurrentOrderDetailID").val();
            e.customArgs["processedQuantity"] = $("#divPalletingInformation #CurrentProcessedQuantity").val();
            e.customArgs["productID"] = $("#divPalletingInformation #CurrentProductID").val();
            e.customArgs["productName"] = $("#divPalletingInformation #CurrentProductName").val();
            e.customArgs["currentPalletNumber"] = $("#divPalletingInformation #NextPalletNumber").val();
        };
        var updatePalletGenerator = function () {
            $("#divPalletGenerator").slideDown();
            LoadingPanel.Show();

            var data = { SelectedPalletID: $("#SelectedPalletID").val(), SelectedOrderProcessId: $("#SelectedOrderProcessId").val() };
            $.post('/Pallets/_GetPalletDetails',
                data,
                function (result) {
                    $("#divPalletGenerator").html(result);
                    Gane.Pallets.PalletEditorInit();
                    palletOrderItemsLoaded();
                    LoadingPanel.Hide();
                });
        };
        var confirmLoadToPallets = function () {
            showDispatch = true;
            RowCount++;
            var palletQuantity = SpinPalletQuantity.GetValue();
            var data = { PalletQuantity: palletQuantity, CurrentPalletID: $("#SelectedPalletID").val(), OrderProcessDetailID: $("#OrderDetailID").val(), ProductID: $("#ProductID").val() };
            $.post('/Pallets/AddProcessedProductsToPallet',
                data,
                function (result) {
                    ModalPopupAddPalletItem.Hide();
                    updatePalletGenerator();
                }).fail(function (xhr, status, error) {
                    ModalPopupAddPalletItem.Hide();
                    updatePalletGenerator();
                });
        };
        var palletOrderItemsLoaded = function () {
            var dataCount = 1;
            var addToPallet = function (oDetilId, pqty, productId, productname) {
                $("#divPalletingInformation #CurrentOrderDetailID").val(oDetilId);
                $("#divPalletingInformation #CurrentProcessedQuantity").val(pqty);
                $("#divPalletingInformation #CurrentProductID").val(productId);
                $("#divPalletingInformation #CurrentProductName").val(productname);
                ModalPopupAddPalletItem.Show();
            };

            $(".sales-detail-actions").each(function () {
                var orderDetailId = $(this).data('id');
                var processedQty = $(this).data('pqty');
                var actualQty = $(this).data('qty');
                var productId = $(this).data('productid');
                var productname = $(this).data('productname');
                var $basketLink = $("<a class='fa fa-cart-plus sales-detail-pallet-basket' title='Load to Pallet'></a>");
                $basketLink.on("click", function () {
                    addToPallet(orderDetailId, processedQty, productId, productname);
                });
                if ($("#SelectedPalletID").val() > 0) {
                    dataCount = 0;
                    if ($("#IsPalletCompleted").val() !== "True") {
                        if (parseFloat(actualQty) > parseFloat(processedQty)) {
                            $(this).html($basketLink);
                        }

                    }
                    else {

                        if ($("#divPalletingInformation .pallet-number .pallet-dispatch-success").length < 1) {
                            $("<small class='pallet-dispatch-success'>Completed : " + $("#PalletCompletedDate").val() + "</small>").appendTo("#divPalletingInformation .pallet-number");
                        }
                    }
                }
            });

        };


        var quickDispatch = function () {
            $("#btnQuickDispatchPallets").attr("disabled", true);
            var deliveryMethod = $('#orderDeliveryMethod').val();
            var networkCode = $('#orderDeliveryNetworkCode').val();
            LoadingPanel.Show();

            if (!!deliveryMethod && (deliveryMethod !== '3' || (deliveryMethod === '3' && !!networkCode))) {
                LoadingPanel.Show();
                $("#frmPalletsEditor").submit();
            }
            else {
                LoadingPanel.Hide();
                $("#btnQuickDispatchPallets").attr("disabled", false);
                LoadingPanel.Show();
                var data = { palletId: $("#SelectedPalletID").val(), orderProcessId: $("#SelectedOrderProcessId").val() };
                $.post('/Pallets/AddAllProcessedProductsToPallet',
                    data,
                    function (result) {
                        if (result != null) {
                            $(".pallet-number").html("<label>Pallet Number :" + result.NextPalletNumber + "</label>");
                            Gane.Helpers.LoadListItemsToDropdown('SelectedPalletID', result.AllCurrentPallets);
                            $("#SelectedPalletID").val(result.SelectedPalletID);
                            $("#SelectedPalletID").trigger("chosen:updated");
                        }

                        updatePalletGenerator();
                        openDispatchPopup();
                    }).fail(function (xhr, status, error) {
                        updatePalletGenerator();
                        LoadingPanel.Hide();
                        $("#btnQuickDispatchPallets").attr("disabled", false);
                    });
            }
        };


        var palletEditorInit = function () {
            $("#SelectedPalletID").on("change", function () {
                if ($("#SelectedPalletID option:selected") && $("#SelectedPalletID option:selected").text().length > 0) {
                    $(".pallet-number").html("<label>" + $("#SelectedPalletID option:selected").text() + "</label>");
                }
                updatePalletGenerator();
            });

            $("#btnNewPallet").unbind("click").on("click", function () {
                LoadingPanel.Show();
                var data = { SelectedOrderProcessId: $("#SelectedOrderProcessId").val() };
                $.post('/Pallets/_GetNewPallet',
                    data,
                    function (result) {
                        $(".pallet-number").html("<label>Pallet Number :" + result.NextPalletNumber + "</label>");
                        Gane.Helpers.LoadListItemsToDropdown('SelectedPalletID', result.AllCurrentPallets);
                        $("#SelectedPalletID").val(result.SelectedPalletID);
                        $("#SelectedPalletID").trigger("chosen:updated");

                        updatePalletGenerator();

                        LoadingPanel.Hide();
                    });
            });

            $("#btnQuickDispatchPallets").unbind("click").on("click", function () {
                quickDispatch();
            });
            $("#editpallets").unbind("click").on("click", function () {
                LoadingPanel.Show();
                var data = { SelectedOrderProcessId: $("#SelectedOrderProcessId").val() };
                $.post('/Pallets/_EditPallet',
                    data,
                    function (result) {
                        Gane.Helpers.LoadListItemsToDropdown('SelectedPalletID', result.AllCurrentPallets);
                        $("#SelectedPalletID").trigger("chosen:updated");

                        updatePalletGenerator();

                        LoadingPanel.Hide();
                    });
            });
        };

        return {

            PalletEditorInit: function () {
                return palletEditorInit();
            },
            UpdatePalletGenerator: function () {
                return updatePalletGenerator();
            },
            PalletOrderItemsLoaded: function () {
                return palletOrderItemsLoaded();
            },
            OnBeginCallbackPalletItem: function (s, e) {
                return onBeginCallbackPalletItem(s, e);
            },
            ConfirmLoadToPallets: function () { return confirmLoadToPallets(); }
        };
    };

    gane.Pallets = new gane.PalletHelpers();

})(jQuery, Gane);

function OnGridTotalCount(s, e) {
    TotalCount = s.cpVisibleRowCount;

}

function OnGridFocusedRowChanged(s, e) {
    $('.selkey').val(s.GetRowKey(s.GetFocusedRowIndex()));
}
function OnPalletGridFocusedRowChanged(s, e) {
    $('.selkey').val(s.GetRowKey(s.GetFocusedRowIndex()));
}

function OnPalletOrderDetailsCallback(s, e) {
    Gane.Pallets.PalletOrderItemsLoaded();
    ShowPalletDispatchButton();
}


function ShowPalletDispatchButton(s, e) {
    LoadingPanel.Show();
    var data = { OrderProcessId: $("#SelectedOrderProcessId").val() };
    $.post('/Pallets/PalletDispatchCheck', data, function (result) {
        LoadingPanel.Hide();
        if (result) {
            $("#btnDispatchPallets").slideDown();
        }
        else {
            $("#btnDispatchPallets").slideUp();
        }


    });

}

function EditPalletDispatch(palletDispatchId) {
    var data = { PalletsDispatchID: palletDispatchId };
    LoadingPanel.Show();
    $.post('/Pallets/EditDispatchPallets', data, function (result) {

        LoadingPanel.Hide();
        Gane.Helpers.ShowPopupMessage('Dispatch Pallets', result, PopupTypes.Warning);
        MVCxClientUtils.FinalizeCallback();

    }).fail(function (xhr, status, error) {
        LoadingPanel.Hide();
        Gane.Helpers.ShowPopupMessage('Dispatch Pallets Error', xhr.responseText, PopupTypes.Warning);
    });
}

function SaveEditPallets() {
    if (UploadControl.GetSelectedFiles() == null || UploadControl.GetSelectedFiles().length < 1) {
        $("#frmDispatchPallets").submit();
    }
    else {
        LoadingPanel.SetText('Dispatching Pallet. Please wait...');
        LoadingPanel.Show();
        UploadControl.UploadFile();
    }
}

function OnPalletSelected(s, e) {
    var selectedPallets = PalletsListGridView1.GetSelectedKeysOnPage();
    if (selectedPallets.length > 0) {
        $("#btnDispatchPallets").slideDown();
    } else {
        $("#btnDispatchPallets").slideUp();
    }
    $("#GridSelectedPallets").val(selectedPallets.join());

    s.GetSelectedFieldValues("PalletNumber", GetSelectedFieldValuesCallback);
}

function GetSelectedFieldValuesCallback(values) {
    var numbers = [];
    for (var i = 0; i < values.length; i++) {
        numbers.push(values[i]);
    }
    $("#GridSelectedPalletNumbers").val(numbers.join());
}

$(document).ready(function () {
    $("#btnDispatchPallets").on("click", function () {
        openDispatchPopup()
    });
});

function openDispatchPopup() {
    var dispatchId = $("#dispatchId").val();
    if (dispatchId > 0 && dispatchId !== null && dispatchId !== undefined && dispatchId !== "") {
        EditPalletDispatch(dispatchId);
    }
    else {
        var orderProcessId = $("#SelectedOrderProcessId").val();
        var deliveryNumber = $('#deliveryRefrenceNumber').val();
        var deliveryMethod = $('#orderDeliveryMethod').val();
        var networkCode = $('#orderDeliveryNetworkCode').val();
        var data = { OrderProcessId: orderProcessId, DeliveryMethod: deliveryMethod, networkCode: networkCode };
        LoadingPanel.Show();
        $.post('/Pallets/DispatchPallets',
            data,
            function (result) {
                LoadingPanel.Hide();
                Gane.Helpers.ShowPopupMessage('Dispatch Pallets', '<div class="pallet-number"><b>DELIVERY REFERENCE NUMBER : </b>' + deliveryNumber + "</div><br/>" + result, PopupTypes.Warning);
                MVCxClientUtils.FinalizeCallback();
            }).fail(function (xhr, status, error) {
                LoadingPanel.Hide();
                Gane.Helpers.ShowPopupMessage('Dispatch Pallets Error', xhr.responseText, PopupTypes.Warning);
            });
    }
}

function OnSubmitFormvalidate() {
    var deliveryMethod = $("#DeliveryMethod").val();
    if (deliveryMethod === "3") {
        var serviceId = $("#NetworkCode").val();
        if (serviceId <= 0 || serviceId === null || serviceId === "") {
            alert("Select services first!")
            return false;
        }
    }


}

function DeliveryMethodChange(e) {
    prepareTenantDeliveryServicesDropDown();
    if ($("#DeliveryMethod :selected").val() == "3") {
        $(".data-detail").hide();
    }
    else {
        $(".data-detail").show();
    }
}

function prepareTenantDeliveryServicesDropDown() {
    var deliveryMethod = $("#DeliveryMethod option:selected").val();
    if (deliveryMethod === null || deliveryMethod === "" || deliveryMethod === 0) { return; }
    LoadingPanel.Show();

    //var pid = $("#prdid option:selected").val();
    $('#NetworkCode').empty();
    $("#NetworkCode").trigger("chosen:updated");
    $.ajax({
        type: "GET",
        url: "/Order/_GetTenantDeliveryServices/",
        data: { deliveryMethod: deliveryMethod },
        success: function (data) {
            if (!!data && data.length > 0) {
                $("#delivery-service__selector").show();
                $.each(data, function (i, item) {
                    $('#NetworkCode').append($('<option></option>').val(item.NetworkCode).html(item.NetworkDescription));
                });
                $("#NetworkCode").trigger("chosen:updated");
            }
            else {
                $("#delivery-service__selector").hide();
            }

            LoadingPanel.Hide();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            // alert(xhr.status);
            LoadingPanel.Hide();
            alert('Error' + textStatus + "/" + errorThrown);
        }
    });
}

function DeletePalletProduct(PalletProductId) {
    if (confirm("Are you sure want to remove this product from pallet?")) {
        LoadingPanel.Show();
        $.ajax({
            type: "GET",
            url: "/Pallets/DeletePalletProduct/",
            data: { PalletProductId: PalletProductId },
            dataType: 'json',
            success: function (data) {
                LoadingPanel.Hide();
                if (data) {
                    Gane.Pallets.UpdatePalletGenerator();

                }
            }
        });
    }

}


