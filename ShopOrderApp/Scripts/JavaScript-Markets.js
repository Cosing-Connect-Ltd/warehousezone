var adjustCustomersSelectionHeight = function () {

    $(".account-available-customers").removeAttr("style");
    $(".account-selected-customers").removeAttr("style");
    var newHeight = $(".account-available-customers").height() > $(".account-selected-customers").height() ? $(".account-available-customers").height() : $(".account-selected-customers").height();
    $(".account-selected-customers").height(newHeight);
    $(".account-available-customers").height(newHeight);

    $(".account-available-customers").append($(".account-available-customers .account-box").remove().sort(function (a, b) {
        var at = $(a).data("text"), bt = $(b).data("text");
        return (at > bt) ? 1 : ((at < bt) ? -1 : 0);
    }));
}

var reloadMarketRouteEvents = function () {
    $(".account-available-customers, .account-selected-customers").sortable({
        items: ".account-box",
        connectWith: ".account-customers",
        stop: function (item, container) {
            onRouteCustomerSelected(item, container);
        }
    }).disableSelection();

    $(".txt-search-market-customers").on("keydown", function (e) {

        if (e.keyCode == 13) {
            e.preventDefault();
        }

        var searchText = $(this).val().toLowerCase();
        $(".account-available-customers .account-box").each(function () {
            if ($(this).data("text").indexOf(searchText) >= 0) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });

    adjustCustomersSelectionHeight();
}

var getDateFromDevexControl = function (name) {
    var dateString = GetDevexControlByName(name).GetFormattedText();
    var parts = dateString.split("/");
    return new Date(parts[2], parts[1] - 1, parts[0], 0, 0);

}
var onRouteCustomerSelected = function (item, container) {
    var marketId = $("#frmSaveMarketRoute #MarketId").val();
    var marketRouteId = $("#frmSaveMarketRoute #Id").val();
    var order = 1;
    var accounts = [];
    $(".account-selected-customers .account-box").each(function () {
        var accountId = $(this).data("id");
        var duplicate = $(".account-selected-customers").find('[data-id=' + accountId + ']');
        if (duplicate.length > 1) {
            alert("Customer already exists for market");
            duplicate.last().remove();
        }
        else {
            var data = { AccountId: accountId, SortOrder: order, VisitFrequency: $(this).find("select option:checked").val(), MarketId: marketId, IsSkippable: $(this).find("input[type=checkbox]").prop("checked"), SkipFromDate: getDateFromDevexControl("SkipFromDate_" + accountId), SkipToDate: getDateFromDevexControl("SkipToDate_" + accountId) };
            accounts.push(JSON.stringify(data));
            order++;
        }

    });

    $("#MarketCustomerEntries").val("[" + accounts.join(',') + "]");
    adjustCustomersSelectionHeight();
}


//Route Markets

var reloadRouteMarketsEvents = function () {
    $(".route-available-markets, .route-selected-markets").sortable({
        items: ".route-box",
        connectWith: ".route-markets",
        stop: function (item, container) {
            onRouteMarketsSelected(item, container);
        }
    }).disableSelection();

    $(".txt-search-market-routes").on("keydown", function (e) {

        if (e.keyCode == 13) {
            e.preventDefault();
        }

        var searchText = $(this).val().toLowerCase();
        $(".route-available-markets .route-box").each(function () {
            if ($(this).data("text").indexOf(searchText) >= 0) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });
}

var onRouteMarketsSelected = function (item, container) {
    var routeId = $("#frmSaveRouteMarkets #RouteId").val();
    var order = 1;
    var markets = [];
    $(".route-selected-markets .route-box").each(function () {
        var marketId = $(this).data("id");
        var duplicate = $(".route-selected-markets").find('[data-id=' + marketId + ']');
        if (duplicate.length > 1) {
            alert("Market already exists in route");
            duplicate.last().remove();
        }
        else {
            var data = { RouteId: routeId, SortOrder: order, MarketId: marketId };
            markets.push(JSON.stringify(data));
            order++;
        }
    });

    $("#RouteMarketsEntries").val("[" + markets.join(',') + "]");
}


$(document).ready(function () {

    reloadMarketRouteEvents();

    reloadRouteMarketsEvents();


    $(".account-selected-customers input[type=checkbox]").on("click", function () {
        onRouteCustomerSelected();
    });

    $(".account-selected-customers select").change(function () {
        onRouteCustomerSelected();
    });

});