//// function Fires when DOM is parsed, equlaent to document.ready ///////
$(function () {




    // Validate Chosen dropdowns
    //$.validator.setDefaults({ ignore: ":hidden:not(.chosen-select)" });

    $(document).bind('keydown', 'shift+c', function () {
        $(".profiler-results").remove();
    });

    $('#dvloan').hide();
    $('.graccount').hide();
    $('.data-datepicker').datepicker({ dateFormat: 'dd/mm/yy' });
    $('#tbl-users,#tbl-AGroup,.tsort').tablesorter();
    $("#drpPD").on("change", function () {
        var departmentId = $("#drpPD").val();
        if (departmentId === null || departmentId === "" || departmentId === 0) { return; }
        LoadingPanel.Show();

        //var pid = $("#prdid option:selected").val();
        $('#drpPG').empty();
        $("#drpPG").trigger("chosen:updated");
        $.ajax({
            type: "GET",
            url: "/PurchaseOrders/_GetProductGroup/",
            data: { DepartmentId: departmentId },


            success: function (data) {
                var options = "<option value='0'>Select Group</option>";
                $.each(data, function (i, item) {
                    options += "<option value='" + item.ProductGroupId + "'>" + item.ProductGroup + "</option>";
                });
                $("#drpPG").html(options);
                $("#drpPG").trigger("chosen:updated");
                $("drpPD").val(departmentId);
                LoadingPanel.Hide();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                LoadingPanel.Hide();
                alert('Error' + textStatus + "/" + errorThrown);

            }

        });

    });

    $("#drpPG").on("change", function () {
        var groupId = $("#drpPG").val();
        if (groupId === null || groupId === "" || groupId === 0) { return; }
        LoadingPanel.Show();
        
        //var pid = $("#prdid option:selected").val();
        $('#drpPC').empty();
        $("#drpPC").trigger("chosen:updated");
        $.ajax({
            type: "GET",
            url: "/PurchaseOrders/_GetProductCategory/",
            data: { GroupId: groupId },
            
            success: function (data) {
                var options = "<option value='0'>Select Category</option>";
                $.each(data, function (i, item) {
                    options += "<option value='" + item.ProductCategoryId + "'>" + item.ProductCategoryName + "</option>";
                });
                $("#drpPC").html(options);
                $("#drpPC").trigger("chosen:updated");
                $("drpPG").val(groupId);
                LoadingPanel.Hide();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                LoadingPanel.Hide();
                alert('Error' + textStatus + "/" + errorThrown);

            }

        });

    });

    $('#grProducts').prop('disabled', true).trigger("chosen:updated");
    $('.deletepg').click(function (e) {
        var r = confirm("Are You Sure You Want to Delete?");

        if (r == false) {
            e.preventDefault();
        }

    });
    $('.perror').hide();
    $('.close').click(function (e) {

        $('.mdl').hide();
        $('.mdl2').hide();

    });


    $('.Caction').click(function (e) {
        var id = e.target.id;
        if (id === "lnkDetails") {
            var Pid = $("#selkey").val();
            location.href = '/PProperties/Details/' + Pid;
        }

        var value;
        if (id === "lnksaveAddress") {
            value = $('#selkeysaveaddresses').val();
        }
        else if (id === "lnkContacts") {
            value = $('#selkeysaveaccounts').val();
        }
        else if (id === "lnkCredLimit") {
            value = $('#selkeysavecredlimit').val();
        }
        else if (id === "lnkProductUpdate") {
            value = $('#selkeyProductList').val();
        }
        else if (id === "lnksavePrdSCCCode") {
            value = $('#selkeyPrdSccCodes').val();
        }
        else if (id === "lnksavePrdAccountCode") {
            value = $('#selkeyPrdAccountCodes').val();
        }
        else if (id === "lnksavePrdAttributes") {
            value = $('#selkeyPrdAttributes').val();
        }
        else if (id === "lnksavePrdCategories") {
            value = $('#selkeyPrdCategories').val();
        }
        else if (id === "lnksavePrdInvStock") {
            value = $('#selkeyPrdInvStock').val();
        }
        else if (id === "lnksavePrdGroup") {
            value = $('#selkeyPrdGroup').val();
        }
        else if (id === "lnksavePrdInvTrans") {
            value = $('#selkeyPrdInvTrans').val();
        }
        else if (id === "lnksavePrdInvTrans") {
            value = $('#selkeyPrdInvTrans').val();
        }
        else if (id === "lnksavePrdKitItems") {
            value = $('#selkeyPrdKitItems').val();
        }
        else if (id === "lnksavePrdLocation") {
            value = $('#selkeyPrdLocation').val();
        }
        else if (id === "lnksavePrdPrice") {
            value = $('#selkeyPrdPrice').val();
        }
        else if (id === "lnksavePrdSer") {
            value = $('#selkeyPrdSer').val();
        }
        else if (id === "lnksavePrdStSnap") {
            value = $('#selkeyPrdStSnap').val();
        }
        else if (id === "lnkOrderUpdate") {
            value = $('#selkeyOrderList').val();
        }
        else if (id === "lnkOrderCompleteUpdate") {
            value = $('#selkeyOrderCompleteList').val();
        }
        else if (id === "lnkLocationUpdate") {
            value = $('#locationList').val();
        }
        else if (id === "lnkLocationDelete") {
            value = $('#locationList').val();
        }
        else if (id === "lnkSalesOrderUpdate") {
            value = $('#selkeySalesOrderList').val();
        }
        else if (id === "lnkSalesOrderCompleteUpdate") {
            value = $('#selkeySalesOrderCompleteList').val();
        }
        else if (id === "lnkmodifydirectSalesOrder") {
            value = $("#selkeySalesOrderList").val();
        }
        else if (id === "lnkDirectSalesOrderPrint") {
            value = $('#selkeyDirectSalesOrderId').val();
        }
        else if (id === "lnkSalesOrderAuthorise") {
            value = $('#selkeySalesOrderList').val();
            if (value === null || value < 1) return;
            var data = { OrderID: value, AuthorisedNotes: "TODO:" };
            if (confirm("Are you sure to authorise this sales order?")) {
                $.post("/Order/AuthoriseOrder/",
                    data,
                    function (result) {
                        AwaitingAuthGridview.Refresh();
                    });
                return;
            }
            return;
        }
        else if (id === "lnkApproveHolidayRequest") {
            value = $('#selkey').val();
            $.ajax({
                type: "GET",
                url: '/ResourceRequests/WarningBeforeApprove/',
                data: { ResourceHolidayRequestId: value, Reason: "", IsApproved: false },
                dataType: 'json',
                success: function (result) {
                    var contents = "";
                    if (result === false) { return; }
                    if (result.WarningMessage !== null) {
                        contents = result.WarningMessage + " " + "Do you want to approve this request or decline?";
                    }
                    value = $('#selkey').val();
                    var approved = $("#ResourceRequestApproved" + value).val() == "True";
                    var approvedDetails = $("#ApprovedDetails" + value).val();
                    var resourceName = "<div class='panel-heading mb-2'>Requested By: " + $("#divResourceName" + value).text() + "</div>";
                    var message = "<div class='row mb-2'><div class='col-md-12'><strong>Notes</strong><br/>" + $("#divRequestNotes" + value).text() + "</div></div>";
                    var requestPeriod = "<div class='row mb-2'><div class='col-md-12'><strong>Period</strong><br/>" + $("#divRequestPeriod" + value).text() + "</div></div>";
                    var $popupMessage = $("<div class='panel panel-primary'>" + resourceName + "<div class='alert alert-warning'>" + contents + "</div><div class='panel-body'>" + message + requestPeriod + "</div></div>");

                    if (!approved) {
                        $popupMessage.find(".panel-body")
                            .append(
                                "<div class='col-md-12 p-0 pt-1'><label class='checkbox-inline'><input type='checkbox' onclick='updateDeclinePopup(this)' id='chkDecline'>Decline</label></div>");
                        $popupMessage.find(".panel-body").append(
                            "<div class='col-md-12 pt-1 div-decline-reason' id='divDeclineReason' style='display:none'><textarea rows='4' cols='50' id='txtDeclineReason' placeholder='Decline Reason'>" +
                            $("#divCancelledReason" + value).text() +
                            "</textarea></div>");

                        var $confirmButton = $("<input type='button' onclick='confirmApproveHolidays(" +
                            value +
                            ")' id='btnApproveConfirm' value='Confirm' class='btn btn-primary'/>");
                        var $footer = $("<div class='panel-footer text-right'></div>");
                        $footer.append($confirmButton);
                        $popupMessage.append($footer);
                    } else {
                        var $appMsg = $("<div class='bg-success col-md-12'><b>" + approvedDetails + "</b></div>");
                        $popupMessage.append($appMsg);
                    }

                    Gane.Helpers.ShowPopupMessage("Approve or Decline Request", $popupMessage, PopupTypes.Warning);
                },

                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    // alert(xhr.status);
                    alert('Error' + textStatus + "/" + errorThrown);

                }
            });

        }
        else if (id === "lnkTransferOrderUpdate") {
            value = $('#selkeyTransferOrderList').val();
        }
        else if (id === "lnkTransferInOrderdelete") {
            value = $('#selkeyTransferOrderList').val();
        }
        else if (id === "lnkTransferInOrderCreate") {
            value = 3;
        }
        else if (id === "lnkTransferOutOrderCreate") {
            value = 4;
        }
        else if (id === "lnkTransferInOrderUpdate") {
            value = $('#selkeyTransferInOrderList').val();
        }
        else if (id === "lnkTransferInOrderDelete") {
            var target = $(this).data("target");
            var orderId = $("" + target).val();
            var type = $(this).data("type");
            DeleteTranserOrders(orderId, type);
            return;

        }

        else if (id === "lnkTransferOutOrderUpdate") {
            value = $('#selkeyTransferOutOrderList').val();
        }
        else if (id === "lnkWorksOrderUpdate") {
            value = $('#selkeyWorksOrderListGridView').val();
        }
        else if (id === "bsDelete") {
            value = $('#selkeyWorksOrderListGridView').val();
        }
        else if (id === "lnkjobTypeEdit") {

            value = $('#selkeyJobTypeList').val();
        }

        else if (id === "WoUrlFragment") {
            if (!confirm('Are you sure you want to complete the order?')) {
                return false;
            }
            value = $('#selkeyWorksOrderListGridView').val() + '/' + location.hash.substr(1, 2);
        }
        else if (id === "lnkresourceEdit") {
            value = $('#selkeyResourceList').val();
        }
        else if (id === "lnkWorkOrderCompleteUpdate") {
            value = $('#selkeyWorksOrderCompletedListGridView').val();
        }

        else if (id === "lnkPalletGridFocusedRowValue") {
            var hash = window.location.hash.substr(1);
            if (hash === "AD" || hash === "") {
                value = $('#DeliveryGridFocusedRowValue').val();
                location.href = '/pallets/orderProcess/' + value;
                return;
            }
            value = $('#PalletGridFocusedRowValue').val();
            location.href = '/pallets/account/pallet/' + value;
            return;
        }

        else {
            value = $('#selkey').val();
        }
        //check value
        if (value === null || value === "") {
            e.preventDefault();
            //do nothing
        }
        else if (value > 0 || value !== null) {

            $(this).attr('href',
                function () {

                    var res = this.href.split("/");
                    var lastindex = res.pop();

                    var res2 = lastindex.split("?");
                    var lastIndexNumber = res2[0];

                    if (!isNaN(lastIndexNumber)) {
                        this.href = res.join("/");
                    }

                    var hash = window.location.hash.substr(1);
                    if (hash !== null && hash !== "") {
                        value = value + "?fragment=" + hash;
                    }

                    return this.href + '/' + value;

                });
        } else {
            e.preventDefault();
        }

    });






    ////////////

    ////////////////////////////////////////////////Locations ////////////////////
    $('#OrderTypeID').change(function (e) {
        var a = $("#OrderTypeID option:selected").text();
        if (a == "Loan") {
            $('#dvloan').show();


        }
        else {
            $('#dvloan').hide();
        }

    });
    $('#Warehouse').change(function (e) {
        var a = $(this).val();
        $.ajax({
            type: "GET",
            url: '/ProductLocations/JsongetLocations',
            data: { "id": a },
            dataType: 'json',
            success: function (data) {
                var result = $("#locTemplate").tmpl(data);
                $("#Locations").empty().append(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                alert('Error' + textStatus + "/" + errorThrown);

            }
        });
    });

    $('#SearchLoc').click(function (e) {
        var a = $('#Warehouse').val();
        var pid = $('#ProductId').val();
        var q = '';
        $.ajax({
            type: "GET",
            url: '/ProductLocations/JsongetLocationlist',
            data: { "ProductId": pid, "id": a, "q": q },
            dataType: 'json',
            success: function (data) {
                var result = $("#locsearchTemplate").tmpl(data);
                $("#loclist").empty().append(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                alert('Error' + textStatus + "/" + errorThrown);

            }
        });

        $('#divLoc').show();
        e.preventDefault();
    });


    $("#LocsearchForm").submit(function () {
        var a = $('#Warehouse').val();
        var pid = $('#ProductId').val();
        var q = $('#q').val();
        $.ajax({
            type: "GET",
            url: '/ProductLocations/JsongetLocationlist',
            data: { "ProductId": pid, "id": a, "q": q },
            dataType: 'json',
            success: function (data) {
                var result = $("#locsearchTemplate").tmpl(data);
                $("#loclist").empty().append(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                alert('Error' + textStatus + "/" + errorThrown);
            }
        });
        return false;
    });

    /////////////////////////////////////////////////Product Attributes


    $('#SearchValue').click(function (e) {

        var a = $('#Attributes').val();
        var pid = $('#ProductId').val();
        var q = '';
        getAttributes(pid, a, q);
        $('#divValue').show();
        e.preventDefault();
    });

    $("#pasearch").submit(function () {

        var a = $('#Attributes').val();
        var pid = $('#ProductId').val();
        var q = $('#pasearch #q').val();
        getAttributes(pid, a, q);
        return false;
    });

    $('#Attributes').change(function (e) {
        var a = $('#Attributes').val();
        $.ajax({
            type: "GET",
            url: '/ProductAttributes/jsongetValues',
            data: { "id": a },
            dataType: 'json',
            success: function (data) {
                var result = $("#attrvalTemplate").tmpl(data);
                $("#Values").empty().append(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                alert('Error' + textStatus + "/" + errorThrown);
            }
        });
    });


    //////////////////////////////////////////////////////////product categories  

    $('#SearchCG').click(function (e) {
        $('#divPG').hide();
        var y = 30;
        $('#divCG').css('display', 'block').animate({ top: y });
        $('#q').val("");
        var pid = $('#ProductId').val();
        Get_PCat(pid, '');
        e.preventDefault();
    });

    $("#CgsearchForm").submit(function () {
        var pid = $('#ProductId').val();
        var q = $('#CgsearchForm #q').val();
        Get_PCat(pid, q);
        return false;
    });

    //// /////////////////////////////////////////////////////////////////productGroup 
    $('#SearchPG').click(function (e) {
        $('#divCG').hide();
        var y = 150;
        $('#divPG').css('display', 'block').animate({ top: y });
        $('#q').val("");
        var pid = $('#ProductId').val();
        Get_PGroups(pid, '');
        e.preventDefault();
    });

    //////////////////////////////  On page search submit ///////////////////////////////
    $("#pgsearchForm").submit(function () {

        var pid = $('#ProductId').val();
        var q = $('#pgsearchForm #q').val();
        Get_PGroups(pid, q);
        return false;
    });

    /////////////// add Asteric on the required data fields in views using jquery validation attribute value///////////////////
    $('input:not([type="checkbox"])').each(function () {
        var req = $(this).attr('data-val-required');
        if (undefined != req) {
            var label = $('label[for="' + $(this).attr('id') + '"]');
            var text = label.text();
            if (text.length > 0) {
                label.append('<span style="color:red"> *</span>');
            }
        }
    });

    /////////////////////////// save accordian active tab state and restore ////////////////////////////
    // enable UI TABS when loading finishes
    $('#right-nav').show("slow");
    var act = 0;
    $("#right-nav").accordion({
        create: function (event, ui) {
            //get index in cookie on accordion create event
            if ($.cookie('saved_index') != null) {
                act = $.cookie('saved_index');
                //alert($.cookie('saved_index'));
            }
        },
        activate: function (event, ui) {
            //set cookie for current index on change event
            $.cookie('saved_index', null, { path: '/' });
            var index = jQuery(this).find("h3").index(ui.newHeader[0]);
            $.cookie('saved_index', index, { path: '/' });
            //alert($.cookie('saved_index'));
        },
        active: parseInt($.cookie('saved_index')),
        heightStyle: "content"
    });

    // left navigation toggle settings 

    $("#toggle-button").css("z-index", "100000");

    //call toggleview to check wether nav view is narrow or wide
    var status = parseInt($.cookie('toggle_index'));
    if (status == 1) {
        navToggleNarrow();
    }
    else {
        navToggleWide();
    }

    // open navigation when clicked on any item
    $("#right-nav").click(function () {
        var status = parseInt($.cookie('toggle_index'));
        if (status == 1) {
            navToggleWide();
        }

    });


    /////////////////// Initialize Chosen plugin for select boxes ///////////////////////
    var config = {
        '.chosen-select': { search_contains: true },
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-width': { width: "95%" }
    };

    resizeChosen();
    jQuery(window).on('resize', resizeChosen);

    for (var selector in config) {
        $(selector).chosen(config[selector]);
    }

    ////////////////// Ajax function to get Products in search /////////////////
    function getProduts(url, q, page, ppitems) {
        $.ajax({
            type: "GET",
            url: baseurl + url,
            data: { "q": a, "page": page, "ppitems": ppitems },
            dataType: 'json',
            success: function (data) {
                var result = $("#plistTemplate").tmpl(data);
                $("#tbl-Products tbody").empty().append(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                alert('Error' + textStatus + "/" + errorThrown);
            }
        });
    }

    ////////////////// Ajax function to get Attributes /////////////////
    function getAttributes(pid, a, q) {
        $.ajax({
            type: "GET",
            url: '/ProductAttributes/JsongetValuelist',
            data: { "ProductId": pid, "id": a, "q": q },
            dataType: 'json',
            success: function (data) {
                var result = $("#valTemplate").tmpl(data);
                $("#vlist").empty().append(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                alert('Error' + textStatus + "/" + errorThrown);
            }
        });
    }

    ////////////////// Ajax function to get Product categories /////////////////
    function Get_PCat(p, q) {

        $.ajax({
            type: "GET",
            url: '/ProductCGMap/JsongetPCategories',
            data: { "ProductId": p, "q": q },
            dataType: 'json',
            success: function (data) {
                var result = $("#pcsearchTemplate").tmpl(data);
                $("#pclist").empty().append(result);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                alert('Error' + textStatus + "/" + errorThrown);
            }
        });

    } //  END OF  Get_PCat(p,q)

    ////////////////// Ajax function to get Product groups /////////////////
    function Get_PGroups(p, q) {

        $.ajax({
            type: "GET",
            url: '/ProductCGMap/JsongetPG',
            data: { "ProductId": p, "q": q },
            dataType: 'json',
            success: function (data) {
                var result = $("#pgsearchTemplate").tmpl(data);
                $("#pglist").empty().append(result);


            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                alert('Error' + textStatus + "/" + errorThrown);

            }
        });


    } //  END OF  Get_PGroups(p,q)


    function updatesorter() {
        $(".tsort").tablesorter();
    }

    function DisplayError(xhr) {
        // var msg = JSON.parse(xhr.responseText);
        var msg = xhr.responseText;
        alert(msg.Message);
    }


    ////////// Function to disable submit buttons after submit /////////////////
    $(document).on('novalidate', 'form', function () {
        var button = $(this).find('input[type="submit"]');
        setTimeout(function () {
            button.prop("disabled", false);
        }, 1);
    });

    $(document).on('submit', 'form', function () {
        var button = $(this).find('input[type="submit"]');
        setTimeout(function () {
            button.prop("disabled", true);
        }, 0);
    });

    //////////////////////////////// auto submit function on change value for warehouse dropdown list 
    $('select[name="wareh-select"]').change(function () {
        $('#wareh-submit').click();
    });

    ///////////////////////////// loader, shows while page loads ////////////////////////////////
    // Wait for window load

    // Animate loader off screen
    $('.se-pre-con').fadeOut("slow");

    $(window).on('beforeunload', function () {
        $('.se-pre-con').show();
    });


    /////////////////////// jquery ui tabs ////////////////////////////////////

    $("#import-tabs").css('visibility', 'visible');
    $("#import-tabs").tabs({
        activate: function (event, ui) {
            var selectedTabTitle = $(ui.newTab).text();
            if (selectedTabTitle === "Products") {
                $("#DataImportUploadForm #ImportType").val("Products");
                $("#data-import-results").html("<li>Choose Products CSV File</li>");
            }
        }
    });

    $("#JobTabs").tabs({ active: $.cookie('ActiveJobTabsId') });
    $("#JobTabs").on("tabsactivate", function (event, ui) { $.cookie('ActiveJobTabsId', ui.newTab.index(), { path: '/' }); });

    // enable tabs display
    $("#tabs").css('visibility', 'visible');
    $("#tabs").tabs({
        activate: function (event, ui) {
            window.location.hash = ui.newPanel.attr('id');
        }
    });

    //activate tab when hash fregment gets changed in URI
    function locationHashChanged() {
        $('#tabs > ul > li > a').each(function (index, a) {
            if ($(a).attr('href') == location.hash) {
                $('#tabs').tabs('option', 'active', index);
            }
        });
    }

    // call onhashchange function
    window.onhashchange = locationHashChanged;

    // code for file uplaoding 
    var vhdFiles = $('#hdPFiles').val();
    if (vhdFiles == null || vhdFiles == '')
        $("#dvfiles").hide();
    else
        $("#dvfiles").show();
    if ($("#chkdis").is(":checked")) {
        $("#dvdisc").show();
    }
    else {
        $("#dvdisc").hide();
    }

    if ($("#chkkit").is(":checked")) {
        $("#dvkit").show();
    }
    else {
        $("#dvkit").hide();
    }


    $('#chkdis').change(function () {

        if ($(this).is(":checked")) {
            $("#dvdisc").show();
        }
        else {
            $("#dvdisc").hide();
        }
    });

    $('#chkkit').change(function () {

        if ($(this).is(":checked")) {
            $("#dvkit").show();
        }
        else {
            $("#dvkit").hide();
        }
    });
    $('#LRemove').click(function (e) {
    });


    $('.loc').click(function (e) {
        var id = e.target.id;
        var value = $('#locationList').val();
        if (id === 'lnkLocationCreate') {
            setValue(0);
        }
        else if (id === 'OrderDetailUpdate' && value !== 0) {
            setValue(value);
        }
        else if (id === 'OrderDetailDelete' && value !== 0) {
            removeDetail(value);
        }
    });
    $('#TaxID').change(function (e) {

    });
    $('.serial').bind("keypress", function (e) {
        if (e.keyCode === 13) {

            // Cancel the default action on keypress event
            e.preventDefault();
            alert("Enter Pressed");
        }
    });

    $('.oAction').click(function (e) {

        var Req_url = window.location.href;
        if (Req_url.indexOf("PurchaseOrders/Create") !== -1 || Req_url.indexOf("PurchaseOrders/Edit") !== -1) {

            var value = $('#radioShipToWarehouse').prop("checked");
            if (!value) {

                var res = confirm("Are you sure to deilvery other than warehouse address?");
                if (!res) {

                    e.preventDefault();
                    return;

                }


            }
        }

        else if ($("#gridViewOrdDet").length < 1) return;

        if (gridViewOrdDet.cpRowCount <= 0) {
            alert('Please add atleast one product!');
            e.preventDefault();
        }
    });


    function GetTodayDate() {
        var tdate = new Date();
        var dd = tdate.getDate(); //yields day
        var MM = tdate.getMonth(); //yields month
        var yyyy = tdate.getFullYear(); //yields year
        var xxx = dd + "/" + (MM + 1) + "/" + yyyy;

        return xxx;
    }

    // Draggable initialize Start
    $(".draggable").draggable({ helper: 'clone', appendTo: 'body', zIndex: 100, cursor: "pointer" });
    $('.droppable').droppable({
        drop: function (ev, ui) {
            // Calculate an active time cell
            var cell = Scheduler.CalcHitTest(ev).cell;
            var EndTime = ui.draggable[0].getAttribute('data-duration');
            var OrderId = ui.draggable[0].getAttribute('data-orderid');
            var joblabel = ui.draggable[0].getAttribute('data-label');
            var tid = ui.draggable[0].getAttribute('data-tid');
            var dispatchId = ui.draggable[0].getAttribute('data-dpId');
            if (isNaN(parseInt(EndTime))) { EndTime = 0; }
            // Initiate a scheduler callback to create an appointment based on a cell interval
            if (cell !== null) {
                Scheduler.getCellInfoProvider().initializeCell(cell);
                Scheduler.PerformCallback({ start: cell.interval.start.getTime(), end: cell.interval.start.getTime() + parseInt(EndTime), subject: ui.draggable[0].textContent, resourceId: cell.resource, orderId: OrderId, jobLabel: joblabel, tenantid: tid, dispatchId: dispatchId });
                //ui.draggable[0].remove();
            } else
                alert('Drop the dragged item on a specific time cell.');

            // Additional logic goes here...
        }
    });
    // end Draggable

    $('.orderactcnts').change(function (e) {
        var id = $("#AccountID option:selected").val();
        if (id === null || id === "" || id === 0) { return; }
        LoadingPanel.Show();

        //var pid = $("#prdid option:selected").val();
        $('#AccountContactId').empty();
        $("#AccountContactId").trigger("chosen:updated");
        $.ajax({
            type: "GET",
            url: "/Order/_GetAccountContacts/",
            data: { Id: id },


            success: function (data) {
                LoadSOAccountAddresses();
                $.each(data, function (i, item) {
                    $('#AccountContactId').append($('<option></option>').val(item.AccountContactId).html(item.ContactName));
                });
                $("#AccountContactId").trigger("chosen:updated");
                fillAccountAddress(id);
                //SetContactEmail();
                fillProductDepartment(id);
                LoadingPanel.Hide();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                LoadingPanel.Hide();
                alert('Error' + textStatus + "/" + errorThrown);

            }

        });
    });

    function fillAccountAddress(accountId) {
        $('#emailWithaccount').empty();
        $("#emailWithaccount").trigger("chosen:updated");
        $.ajax({
            type: "GET",
            url: "/PurchaseOrders/_GetAccountAddress",
            data: { accountId: accountId, orderId: 0 },

            success: function (data) {
                var j = 0;
                var selectedId = [];
                $.each(data, function (i, item) {
                    if (item.Selected) {

                        $('#emailWithaccount').append('<option value=' + item.Value + ' selected>' + item.Text + '</option>');
                        selectedId.push(item.Value);

                    }
                    else {
                        $('#emailWithaccount').append('<option value=' + item.Value + '>' + item.Text + '</option>');

                    }


                });
                $("#emailWithaccount").trigger("chosen:updated");
                //for (var i = 0; i < selectedId.length; i++) {
                //    $('#emailWithaccount').val(selectedId[i]);
                //}
                //$("#emailWithaccount").trigger("chosen:updated");
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                // alert(xhr.status);
                LoadingPanel.Hide();
                alert('Error' + textStatus + "/" + errorThrown);

            }

        });

    }

    $('#btngrDrp').on("click", function (e) {

        var id = prdid.GetValue();
        var type = $("#type").val();
        if (type === 12 || type === 13) {
            //    $("#grProducts option:selected").val();
            //if (id === null || id === undefined || id === "") {

            //id = $("#wrProducts option:selected").val();
        }
        ModelGoodsReturn.Show();

    });


    $('.bsDrp').change(function (e) {
        var id = $("#products option:selected").val();

        ModelBS.Show();

    });

    $('#grOrderNumber').bind("keyup", function (e) {
        prdid.SetEnabled(false);
        //$('#grProducts').prop('disabled', true).trigger("chosen:updated");
        $('.graccount').hide();

        $('.graccounttext').val("");
        $('#grorderid').val("");
        $('#wgrorderid').val("");
        if (e.keyCode === 13 && $('#grOrderNumber').length > 0) {
            e.preventDefault();
            var orderid = $('#grOrderNumber').val();

            LoadingPanel.Show();
            var rst;
            $.ajax({
                type: "GET",
                url: "/InventoryTransaction/_IsOrderValid/",
                data: { order: orderid },


                success: function (data) {
                    LoadingPanel.Hide();
                    if (!data) {
                        $.confirm({
                            title: 'Information!',
                            content: "Not valid order number. Do you still want to return?",
                            buttons: {
                                cancel: function () {

                                },
                                Return: function () {
                                    prdid.SetEnabled(true);
                                    $("#btngrDrp").removeAttr("disabled");
                                }
                            }
                        });
                        //alert("Not valid order number");
                        // prdid.SetEnabled(false);

                    }
                    else {
                        prdid.SetEnabled(true);
                        //$('#grProducts').prop('disabled', false).trigger("chosen:updated");
                        $('.graccount').show();
                        $('.graccounttext').text(data.accountname);
                        $('#grorderid').val(data.orderid);
                        $('#wgrorderid').val(data.orderid);
                        if (data.Products !== null) {
                            var options = "";
                            prdid.PerformCallback();
                            //$.each(data.Products, function (i, v) {
                            //    var opt = "<option value='" + v.Id + "'>" + v.Name + "</option>";
                            //    options += opt;
                            //});
                            //$("#grProducts").html(options);
                            //$("#grProducts").trigger("chosen:updated");
                            $("#btngrDrp").removeAttr("disabled");
                        }

                        if (data.accountId !== null) {
                            $('#lblgrAccount').val(data.accountId);
                        }
                    }

                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    // alert(xhr.status);
                    LoadingPanel.Hide();
                    alert('Error' + textStatus + "/" + errorThrown);

                }

            });


        }
    });

    $('.woPropertyvalidation').click(function (e) {
        var pid = $("#PPropertyId option:selected").val();
        if (pid == undefined) {
            $('.perror').show();
            e.preventDefault();
        }
    });
    $('.cComplete').click(function (e) {

        if (confirm("Are you sure want to complete this order ?")) {
            return;
        }
        e.preventDefault();

    });


    // draggable-route initialize Start
    $(".draggable-route").draggable({
        helper: 'clone',
        appendTo: 'body',
        zIndex: 100,
        cursor: "pointer",
        start: function (event, ui) {
            $(ui.helper).addClass("route-ui-helper");
        }
    });
    $('.droppable-route').droppable({
        drop: function (ev, ui) {
            // Calculate an active time cell
            var cell = RouteScheduler.CalcHitTest(ev).cell;
            var endTime = ui.draggable[0].getAttribute('data-duration');
            var routeId = ui.draggable[0].getAttribute('data-routeid');
            var tid = ui.draggable[0].getAttribute('data-tid');
            // Initiate a scheduler callback to create an appointment based on a cell interval
            if (cell != null) {
                RouteScheduler.getCellInfoProvider().initializeCell(cell);
                RouteScheduler.PerformCallback({
                    start: cell.interval.start.getTime(), end: cell.interval.start.getTime() + parseInt(endTime),
                    mobileLocationId: cell.resource, routeId: routeId, tenantid: tid
                });
                //ui.draggable[0].remove();
            } else
                alert('Drop the dragged item on a specific time cell.');

            // Additional logic goes here...
        }
    });

    $('.orderSaveAndComplete').click(function (e) {

        var Req_url = window.location.href;
        if (Req_url.indexOf("PurchaseOrders/Create") !== -1 || Req_url.indexOf("PurchaseOrders/Edit") !== -1) {

            var value = $('#radioShipToWarehouse').prop("checked");
            if (!value) {

                var res = confirm("Are you sure to deilvery other than warehouse address?");
                if (!res) {

                    e.preventDefault();
                    return;
                }
            }
        }
        $("input[name='orderSaveAndComplete'").val('1');
    });

    $('.orderSaveAndProcess').click(function (e) {

        $("input[name='orderSaveAndProcess'").val('1');
    });

    // end draggable-route


    //only single checkbox allowed wihtin parent div
    $('.single-check-select .checkbox').click(function () {
        var checkedState = $(this).prop("checked");
        $(this)
            .parents('.single-check-select')
            .find('.checkbox:checked')
            .prop("checked", false);

        $(this).prop("checked", checkedState);
    });


}); //// end of main function which fires upon DOM parse completion


function resizeChosen() {
    $(".chosen-container").each(function () {
        $(this).attr('style', 'width: 100%');
    });
}


$(document).ajaxSuccess(
    function (event, xhr, settings) {
        $(".chosen-select").chosen();
    }
);


var GetDevexControlByName = function (name) {
    var controls = ASPxClientControl.GetControlCollection();
    return controls.GetByName(name);
};

var generateNextProductCode = function (textBoxId) {
    var productCode = "";
    $.get("/Products/GetNextProductCode/", function (data) {
        productCode = data;
        $("#" + textBoxId).val(productCode);
    });
    return productCode;
};

generateNextOrderNumber = function (txtOrderNumberId) {
    var orderNumber = "";
    var orderNumberPrefix = '';

    if (location.href.indexOf('PurchaseOrders') >= 0) orderNumberPrefix = 'PO';
    if (location.href.indexOf('SalesOrders') >= 0) orderNumberPrefix = 'SO';
    if (location.href.indexOf('TransferOrders') >= 0) orderNumberPrefix = 'TO';
    if (location.href.indexOf('WorksOrders') >= 0) orderNumberPrefix = 'WO';
    $.get("/Order/GetNextOrderNumber/" + orderNumberPrefix, function (data) {
        orderNumber = data;
        $("#" + txtOrderNumberId).val(orderNumber);
    });
    return orderNumber;
};


function DeleteTranserOrders(orderId, type) {
    if (orderId == null || orderId < 1) return;

    var confirmedAction = function () {
        $.get("/TransferOrders/DeleteTransferOrder/" + orderId,
            function (data) {
                if (!data) {
                    alert("You don’t have permission to perform this action!");
                }
                else {
                    switch (type) {
                        case 'TI':
                            _TransferInOrderListGridView.Refresh(); break;

                        default:
                        case 'TO':
                            _TransferOutOrderListGridView.Refresh();
                            break;
                    }
                }
            });
    };
    if (confirm("This action cannot be reversed, are you sure you want to delete this order?")) {
        confirmedAction();
    }
}

function GetRowKeyByDxGridName(GridName) {
    return GridName.GetRowKey(GridName.GetFocusedRowIndex());
}

// navigation toggle betwwen narrow and wide (functions)
function navToggleView() {

    if ($("#btn-toggleApptView").hasClass('fa-caret-left')) {

        navToggleNarrow();
    }
    else {
        navToggleWide();
    }
}

function navToggleNarrow() {
    $(".main-left").animate({ width: "97%" }, 400);
    $(".main-right").animate({ width: "3%" }, 400);
    $("#btn-toggleApptView").attr('class', 'fa fa-list');
    $(".ui-accordion-content").css("display", "none");
    $(".ui-accordion-header span").css("display", "none");
    $("#right-nav").accordion("disable");
    $(".ui-accordion-header").css("text-align", "center");
    $(".ui-accordion-header").css("font-size", "10px");
    // update toggle index in cookie
    toggleIndex = 1;
    $.cookie('toggle_index', toggleIndex, { path: '/' });
}

function navToggleWide() {
    $(".main-left").animate({ width: "85%" }, 400);
    $(".main-right").animate({ width: "15%" }, 400);
    $("#btn-toggleApptView").attr('class', 'fa fa-caret-left');
    $(".ui-accordion-content").css("display", "block");
    $(".ui-accordion-header span").css("display", "inline-block");
    $("#right-nav").accordion("enable");
    $("#right-nav").accordion("refresh");
    $(".ui-accordion-header").css("text-align", "left");
    $(".ui-accordion-header").css("font-size", "14px");
    // update toggle index in cookie
    toggleIndex = 0;
    $.cookie('toggle_index', toggleIndex, { path: '/' });
}


String.prototype.replaceAll = function (search, replacement) {
    if (search == null || search.length == 0) return '';
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};



function AccountContactChange(e) {
    SetContactEmail();
}

function SetContactEmail() {
    var id = $("#AccountContactId :selected").val();
    if (id > 0 && id !== undefined) {
        $('#emailWithaccount > option :selected').each(function () {
            $(this).attr("selected", false);
        });
        $("#emailWithaccount").val(id);
        $("#emailWithaccount").trigger("chosen:updated");
    }
    else {
        $("#emailWithaccount").val($("#emailWithaccount option:first").val());
        $("#emailWithaccount").trigger("chosen:updated");
    }
}
function EditAccount() {
    var value = $('#selkey').val();
    var MarketId = parseInt($("#MarketId").val());
    window.location = "/Account/edit?id=" + value + "&MarketId=" + MarketId;
}



//funtions to create GUID in Javascript
function guid() {
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}

function s4() {
    return Math.floor((1 + Math.random()) * 0x10000)
        .toString(16)
        .substring(1);
}


function fillProductDepartment(id) {

    var id = $("#AccountID option:selected").val();
    if (id === null || id === "" || id === 0) { return; }
    LoadingPanel.Show();

    //var pid = $("#prdid option:selected").val();
    $('#drpPD').empty();
    $("#drpPD").trigger("chosen:updated");
    $.ajax({
        type: "GET",
        url: "/PurchaseOrders/_GetProductDepartment/",
        data: { accountId: id },


        success: function (data) {
            var options = "";
            $.each(data, function (i, item) {
                options += "<option value='" + item.DepartmentId + "'>" + item.DepartmentName + "</option>";
            });
            $("#drpPD").html(options);
            $("#drpPD").trigger("chosen:updated");

            LoadingPanel.Hide();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            // alert(xhr.status);
            LoadingPanel.Hide();
            alert('Error' + textStatus + "/" + errorThrown);
        }

    });
}

function DirectSalesOrderPrint() {
    var id = $("#selkeyDirectSalesOrderId").val();
    window.open("/Reports/SalesOrderPrint?id=" + id + "&directsales=true");
}
function ReturnsOrderPrint() {
    var id = $("#selkeyDirectSalesOrderId").val();
    window.open("/Reports/SalesOrderPrint?id=" + id + "&returns=true");
}