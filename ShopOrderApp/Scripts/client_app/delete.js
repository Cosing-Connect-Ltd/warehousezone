﻿//on form modal delete
$(document).on('click', '#deleteBtn', function (e) {
    e.preventDefault();
    var selector = $('#modalDialog');
    var controller = $(this).data('controller');
    var controllerAction = $(this).attr('data-action');
    var kitsvalue = $(this).attr('data-kitType');
    selector.modal('show');
    $title = $(this).data('title');
    $message = $(this).data('message');
    $datavalueid = $(this).data('valueid');
    selector.find('.modal-header h4').text($title);
    selector.find('.modal-body p').text($message);
    selector.find('.modal-footer #deleteConfirm').attr('data-valueid', $datavalueid).attr('data-controller', controller).attr('data-action', controllerAction).attr('data-kitType', kitsvalue);
});

//on delete
$(document).on('click', '#modalDialog .modal-footer #deleteConfirm', function (e) {
    var selector = $('#modalDialog');
    $datavalueid = $(this).attr('data-valueid');
    var controller = $(this).attr('data-controller');
    var controllerAction = $(this).attr('data-action');
    var kitsvalue = $(this).attr('data-kitType');
    var form = $('#_deleteForm');
    var token = $('input[name="__RequestVerificationToken"]', form).val();
    console.log('delete: ' + controller);
    var model = { Id: $datavalueid, __RequestVerificationToken: token };

    if (controllerAction == "RemoveRecipeItemProduct" || controllerAction == "RemoveKitItemProduct" || controllerAction =="RemoveGroupedItemProduct") {

        controllerAction = "RemoveRecipeItemProduct";
        model = { Id: $(this).attr('data-valueid'), RecipeProductId: $(this).attr('data-valueid') }
        

    }

    $.ajax({
        url: '/' + controller + '/' + (controllerAction == null ? "Delete" : controllerAction),
        data: model,
        method: 'POST',
        success: function (response)
        {
            if (response.success) {
                selector.modal('hide');
                if ($("#gridMaster").length > 0) {
                    gridMaster.Refresh();
                }
                if ($("#ProductRecipeSelectedItemsGrid").length > 0) {
                    ProductRecipeSelectedItemsGrid.Refresh();
                }
                if (controllerAction == "RemoveRecipeItemProduct") {
                    var productid = $(".productIds").val();
                    if (kitsvalue=="1") {
                        GetDevexControlByName(productid + "productKitItems").Refresh();
                    }
                    else if (kitsvalue == "2")
                    {
                        GetDevexControlByName(productid + "productgroupedItems").Refresh();
                    }
                    else if (kitsvalue == "3") {
                        GetDevexControlByName(productid + "productRecipeItems").Refresh();
                    }
                }
                if (controllerAction == "RemoveKitItemProduct") {
                    var productid = $("#ProductId").val();
                    GetDevexControlByName(productid + "productKitItems").Refresh();
                }
                if (controllerAction == "DeleteSelectedKitItem") {
                    ProductKitSelectedItemsGrid.Refresh();
                }
                if (controllerAction == "Delete") {
                    MarketJobsGrid1.Refresh();
                    MarketJobsGrid6.Refresh();
                }
                if ($("#ShiftsGridView").length > 0) {
                    ShiftsGridView.Refresh();
                }
                //hide row
                $('#row_' + $datavalueid).hide();
            }
        }
    });
});