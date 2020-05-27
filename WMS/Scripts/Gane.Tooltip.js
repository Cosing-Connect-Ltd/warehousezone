$(document).ready(function () {

    var pathParts = document.location.pathname.split('/');

    var tooltipKeyPrefix = pathParts[1] + "/" + pathParts[2] + "/";

    var getTooltipsDetail = function () {

        var tooltipElements = $('[tooltipKey]');

        var tooltipKeys = [];

        for (var i = 0; i < tooltipElements.length; i++) {
            tooltipKeys.push($(tooltipElements[i]).attr('tooltipKey'));
        }


        var inputElements = $('input, label');

        for (var i = 0; i < inputElements.length; i++) {
            tooltipKeys.push(tooltipKeyPrefix + $(inputElements[i]).attr('id'));
        }

        tooltipKeys = jQuery.unique(tooltipKeys);

        $.ajax({
            type: "POST",
            url: "/Tooltips/GetTooltipsByKey",
            data: JSON.stringify({ 'keys': tooltipKeys }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {

                var tooltipsDetail = data.reduce(function (p, c) { p[c.Key.toLowerCase()] = { 'Title': c.Title, 'Description': c.Description }; return p; }, {});

                showAvailableTooltipsIcon(tooltipsDetail);
            },
            error: function (xhr, status, error) {
                console.log(error);
            }

        });
    }

    getTooltipsDetail();

    var showAvailableTooltipsIcon = function (tooltipsDetail) {
        $('.tooltip-icon').each(function (index) {

            var tooltipKey = $(this).attr('tooltipKey').toLowerCase();

            if (!!tooltipKey && !!tooltipsDetail[tooltipKey]) {
                $(this).qtip({
                    content: {
                        text: tooltipsDetail[tooltipKey].Description,
                        title: tooltipsDetail[tooltipKey].Title
                    },
                    style: {
                        classes: 'qtip-bootstrap',
                        tip: {
                            width: 10,
                            height: 10
                        }
                    },
                    show: {
                        event: 'click',
                        solo: true
                    },
                    position: {
                        my: 'top left',
                        at: 'botton right',
                        effect: false,
                        viewport: $('.main-left'),
                        adjust: {
                            method: 'shift shift',
                            x: -6,
                            y: 15
                        }
                    },
                    hide: 'unfocus'
                });

                $(this).show();
            }
            else {
                $(this).remove();
            }
        });
    }

    var addTooltipElement = function (targetElement, tooltipKey) {
        if (!!tooltipKey) {
            var container = $("<i class='tooltip-icon fa fa-info-circle' tooltipKey='" + tooltipKey.toLowerCase() + "'/>").hide();

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
        var tooltipKey = tooltipKeyPrefix + $(this).attr('id');

        addTooltipElement($(this), tooltipKey);
    });
});
