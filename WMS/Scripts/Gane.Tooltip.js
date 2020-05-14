jQuery(document).ready(function () {

    var tooltipsDetail = [];

    var getTooltipsDetail = function () {

        var tooltipElements = $('[tooltipKey]');

        var tooltipKeys = [];

        for (var i = 0; i < tooltipElements.length; i++) {
            tooltipKeys.push($(tooltipElements[i]).attr('tooltipKey'))
        }


        var inputElements = $('input, label');
        for (var i = 0; i < inputElements.length; i++) {
            tooltipKeys.push(document.location.pathname + '/' + $(inputElements[i]).attr('id'))
        }

        tooltipKeys = jQuery.unique(tooltipKeys);

        $.ajax({
            type: "post",
            url: "/Tooltips/GetTooltipsDetailByKey",
            data: JSON.stringify({ 'keys': tooltipKeys }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {

                tooltipsDetail = data.reduce(function (p, c) { p[c.Key] = { 'Title': c.Title, 'Description': c.Description }; return p; }, {});

                showAvailableTooltipsIcon();
            },
            error: function (xhr, status, error) {
                console.log(error);
            }

        });
    }

    getTooltipsDetail();

    var showAvailableTooltipsIcon = function () {
        $('.tooltip-icon').each(function (index) {

            var tooltipKey = $(this).attr('tooltipKey');

            if (!!tooltipKey && !!tooltipsDetail[tooltipKey]) {
                $(this).show();
            }
            else {
                $(this).remove();
            }
        });
    }

    var addTooltipElement = function (targetElement, tooltipKey) {
        if (!!tooltipKey) {
            var container = $("<i class='tooltip-icon fa fa-info-circle' tooltipKey='" + tooltipKey + "'/>").hide();

            if (targetElement.is("label")) {
                targetElement.append(container);
            }
            else {
                targetElement.after(container);
            }
        }
    }

    $('[tooltipKey]').each(function (index) {

        var tooltipKey = $(this).attr('tooltipKey');

        addTooltipElement($(this), tooltipKey);
    });

    $('input, label').each(function (index) {

        var tooltipKey = document.location.pathname + '/' + $(this).attr('id');

        addTooltipElement($(this), tooltipKey);
    });

    $(".tooltip-icon").click(function (e) {
        e.stopPropagation();
        e.preventDefault();

        closeCurrentTooltip();

        showTooltip($(this));
    });

    var closeCurrentTooltip = function () {
        $(".tooltip-icon").not(this).popover('hide');

        if ($("#tooltipPanel").dialog('isOpen') === true) {
            $("#tooltipPanel").not(this).dialog('close');
        }
    }

    var showTooltip = function (tooltipIcon) {

        var tooltipKey = tooltipIcon.attr('tooltipKey');

        $("#tooltipPanel p").html(tooltipsDetail[tooltipKey].Description);

        if (!!tooltipsDetail[tooltipKey].Title) {
            $("span.ui-dialog-title").text(tooltipsDetail[tooltipKey].Title);
        }
        else {
            $(".ui-dialog-titlebar").hide();
        }

        $("#tooltipPanel").dialog("option", "position", {
            my: "left top",
            at: "left top",
            of: tooltipIcon
        }).dialog("open");
    }
    
    $("#tooltipPanel").dialog({
        show: {
            effect: "fade",
            duration: 300
        },
        hide: {
            effect: "fade",
            duration: 100
        },
        autoOpen: false,
        resizable: false,
        closeOnEscape: true,
        modal: false,
        draggable: false,
        minHeight: "15px",
        dialogClass: "tooltip-panel",
        open: function (event, ui) {
            $(".ui-dialog-titlebar-close").hide();
        }
    });

    $(document).bind('click',
        function (e) {
            if ( // Check if clicked object is not tool-tip dialog
                jQuery('#tooltipPanel').dialog('isOpen')
                && !jQuery(e.target).is('.ui-dialog, a')
                && !jQuery(e.target).closest('.ui-dialog').length
            ) {
                jQuery('#tooltipPanel').dialog('close');
            }
        }
    );
});
