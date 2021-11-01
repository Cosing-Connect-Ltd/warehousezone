var selectAllTenants = function() {
    $("#PropertyTenantIds option").prop("selected", true);
    $("#PropertyTenantIds").trigger("chosen:updated");
    updateTenantsEmailAddress();
}
var updatePropertyTenantsList = function() {
    var propertyId = $("#PPropertyId").val();
    if (propertyId == null || propertyId < 1 || $("#PropertyTenantIds").length<1) return;
    var orderId = $("#OrderID").val();
    var data = {
        orderid: orderId,
        id: propertyId
    };

    $.post("/WorksOrders/GetPropertyTenants/", data, function (data) {
        $("#PropertyTenantIds").html("");
        $(data).each(function (s, v) {
            var $input = $("<option " + (v.Selected ? "selected='selected' ":"") + " value='"+v.Value+"'>"+ v.Text +"</option>");
            $("#PropertyTenantIds").append($input);
        });
        $("#PropertyTenantIds").trigger("chosen:updated");
        if (data.orderid < 1) {
            selectAllTenants();
        } else {
            updateTenantsEmailAddress();
        }
    });
}

var updateTenantsEmailAddress = function() {
    var emails = [];
	var selectedTenants = $("#PropertyTenantIds").val();
	if (selectedTenants === undefined || selectedTenants === null || selectedTenants.length < 1 || selectedTenants === "") {
        $("#LblCustomRecipients").text('Select Tenants...');
        return false;
    }

    var getTenantEmailById = function(tenantId) {
        var result = "";
        var orderId = $("#OrderID").val();
        var data = {
            orderid: orderId,
            TenantId: tenantId
        };
        if (tenantId < 1) {
            return '';
        }
        $.post("/WorksOrders/GeEmailForPTenant/", data, 
            function(email) {
                if (email.length > 0 && email.indexOf('@') > 0 && emails.indexOf(email)<0) {
                    emails.push(email);
                    $("#LblCustomRecipients").text(emails.join('; '));
                }
            });
        return result;
    }

    for (var i = 0; i < selectedTenants.length; i++) {
        var tenantId = selectedTenants[i];
        getTenantEmailById(tenantId);
    }
}

$(document).ready(function() {

    $("#PPropertyId").on("change", function() {
        $("#CustomRecipients").val('');
        $('#PropertyTenantIds').empty();
        $("#PropertyTenantIds").trigger("chosen:updated");
        updatePropertyTenantsList();
    });

    $("#WorksOrderCommandComplete").on("click", function() {
        if (confirm("Are you sure you want to complete the order?")) {
            return true;
        }
else return false;
    });
     

  /*  var updateJobSubtypeList = function() {
        var jobTypeId = $("#JobTypeId").val();
        $.get("/WorksOrders/GetJobSubtypes/" + jobTypeId, function(data) {
            $("#frmWorksOrderForm #JobSubTypeId").html("");
            $("#frmWorksOrderForm #JobSubTypeId_chosen .chosen-drop li").remove();
            $(data).each(function(s,v) {
                var $input = $("<option value='"+v.Value+"'>"+ v.Text +"</option>");
                $("#frmWorksOrderForm #JobSubTypeId").append($input);
            });
            $("#frmWorksOrderForm #JobSubTypeId").trigger("chosen:updated");
        });
    }

    $("#frmWorksOrderForm #JobTypeId").on("change", function() {
        updateJobSubtypeList();
    });
*/

    $("#PropertyTenantIds").on("change",
        function() {
            updateTenantsEmailAddress();
        });


    $("div.order-shipmentinfo-panes[style='visibility: hidden']").slideUp();

    setTimeout(function() {
        updateTenantsEmailAddress();
    }, 100);

});

function loadOrderDetailsEvents() {
    $("#PPropertyId").change();//Load property tenants on load

    if ($("#WorksOrderBulkCreatedListGridView").length > 0) {
        WorksOrderBulkCreatedListGridView.Refresh();
    }

    $("#btnSaveBulkOrderSingle").on("click", function() {
        var formData = $("#frmBulkWorksOrderForm").serialize();
        var childFormData = $("#frmWorksSingleOrderForm").serialize();
        formData = formData+childFormData;
        LoadingPanel.Show();
        $.post("/WorksOrders/SaveOrdersBulkSingle",
            formData,
            function (result) {
                if(result.success == true) 
                {
                    LoadingPanel.Hide();
                    ModelWorksOrderSingle.Hide();
                    WorksOrderBulkCreatedListGridView.Refresh();
                }
                else 
                {
                    LoadingPanel.Hide();
                    alert(result.message)
                }
               
            });
    });
    $("#btnSaveBulkOrderSingleEdit").on("click", function() {
        var formData = $("#frmBulkWorksOrderForm").serialize();
        var childFormData = $("#frmWorksSingleOrderFormEdit").serialize();
        formData = formData+childFormData;
        LoadingPanel.Show();
        $.post("/WorksOrders/UpdateOrdersBulkSingle",
            formData,
            function (result) {
                if(result.success == true) 
                {
                     LoadingPanel.Hide();
                     ModelWorksOrderSingleEdit.Hide();
                     WorksOrderBulkCreatedListGridView.Refresh();
                }
                else 
                {
                    LoadingPanel.Hide();
                    alert(result.message);
                }
            });
    });

    $("#btnWOUpdateOrder").on("click", function () {
        var orderId = GetRowKeyByDxGridName(WorksOrderBulkCreatedListGridView);

        if (orderId < 1) {
            alert("Please select an order to edit and try again.");
            return false;
        }
        ModelWorksOrderSingleEdit.Show();
    });
}

$(document).ready(function() {
    loadOrderDetailsEvents();
});


function OnBeginCallbackPicklistFilter(s, e) {
    
    e.customArgs["id"] = PickListDate.GetValue();
}