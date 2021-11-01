function namespace(namespaceString) {
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

var PopupTypes = { Success: 1, Warning: 2, Danger: 3 };
(function ($, gane) {


    gane.UtilityHelpers = function () {

        var addValidationErrorToElement = function ($selector, message) {
            $selector.closest("form").find(".validation-summary-errors").remove();
            $("<div class='validation-summary-errors col-md-12'>" + message + "</div>")
                .insertAfter($selector);
            LoadingPanel.Hide();
            return false;
        };

        var showPopupMessage = function (title, htmlContent, type, callback) {
            PopupMessage.SetHeaderText(title);
            PopupMessage.SetContentHtml($("<div>").append(htmlContent).html());
            PopupMessage.Show();
            if (type !== null && type === PopupTypes.Danger) {
                $(".common-dxpopup-header .dxpc-header").css("background-color", "red");
            }
            else if (type !== null && type === PopupTypes.Warning) {
                $(".common-dxpopup-header .dxpc-header").css("background-color", "orange");
            }
            else if (type !== null && type === PopupTypes.Success) {
                $(".common-dxpopup-header .dxpc-header").css("background-color", "green");
            } else {
                $(".common-dxpopup-header .dxpc-header").css("background-color", "#3e9c97");
                $(".common-dxpopup-header .dxpc-header").css("color", "#fff");
            }
            if (callback !== null) {
                setTimeout(callback, 100);
            }
        };
       


         
        


        var loadPickLocationsReceiveProduct = function (id) {

            $.get('/order/GetProductLocations/' + id, function (data) {
                if (data.ProductDetails.length > 0 && !data.Serialised) {
                    var $table = $("<table class='table table-bordered table-striped' id='tblSOProductLocations'></table>");
                    //BATCH REQUIRED FLAG
                    if (data.ContainsBatches) {
                        var batchColumns = "";
                        if (data.ContainsExpiryDate) {
                            batchColumns = "<div class='col-md-4'>Batch Number</div><div class='col-md-2'>Expiry Date</div>";
                        } else {
                            batchColumns = "<div class='col-md-6'>Batch Number</div>";
                        }
                        $table.append("<thead><th>Location</th><th>" + batchColumns + "<div class='col-md-3'>In Stock</div><div class='col-md-3'>Pick Quantity</div></th></thead>");
                    } else {
                        $table.append("<thead><th>Location</th><th>In Stock</th><th>Pick Quantity</th></thead>");
                    }

                    for (var i = 0; i < data.ProductDetails.length; i++) {

                        var currentProductDetail = data.ProductDetails[i];
                        //BATCH REQUIRED FLAG
                        if (data.ContainsBatches) {

                            var batchItems = "<div>";

                            for (var j = 0; j < currentProductDetail.Batches.length; j++) {
                                var batchColumn = "";

                                //EXPIRY DATE REQUIRED FLAG
                                if (data.ContainsExpiryDate) {
                                    batchColumn = "<div class='col-md-4 product-batchrow-td'><span class='labels'>" + (currentProductDetail.Batches[j].BatchNumber != null ? currentProductDetail.Batches[j].BatchNumber : "") + "</span></div><div class='col-md-2 product-batchrow-td'><span class='labels'>" + (currentProductDetail.Batches[j].ExpiryDateString != null ? currentProductDetail.Batches[j].ExpiryDateString : '') + "</span></div>";
                                } else {
                                    batchColumn = "<div class='col-md-6 product-batchrow-td'><span class='labels'>" + currentProductDetail.Batches[j].BatchNumber + "</span></div>";
                                }


                                batchItems += "<div class='row process-product-batchrow'>" + batchColumn + "<div class='col-md-3 product-batchrow-td'><span class='labels'>" + currentProductDetail.Batches[j].Quantity +"</span></div><div class='col-md-3'><input class='form-control txt-pick-qty' placeholder='Pick Quantity' type='text' name='PickQuantity_" + currentProductDetail.Location + '_' + currentProductDetail.Batches[j].BatchNumber + "'/></div></div>";
                            }
                            var $row = $("<tr data-id='" + currentProductDetail.LocationCode + "' data-qty='" + currentProductDetail.Quantity + "'><td><input type='hidden' name='PickLocation" + i + "' value='" + currentProductDetail.LocationCode +
                                "'/>" + currentProductDetail.Location + "</td><td>" + batchItems + "</td></tr>");
                            $table.append($row);

                        } else {

                            var $prow = $("<tr data-id='" +
                                currentProductDetail.LocationCode +
                                "' data-batch='" +
                                currentProductDetail.LocationCode +
                                "' data-qty='" +
                                currentProductDetail.Quantity +
                                "'><td><input type='hidden' name='PickLocation" + i + "' value='" + currentProductDetail.LocationCode + "'/>" +
                                currentProductDetail.Location +
                                "</td><td>" +
                                currentProductDetail.Quantity +
                                "</td><td><input class='form-control txt-pick-qty' placeholder='Pick Quantity' type='text' name='PickQuantity" + i + "_" + currentProductDetail.Location + "'/></td></tr>");
                            $table.append($prow);
                        }
                    }
                    $("#divLocationsPicker").append($table);
                    $("#btnPickLocations").hide();
                    $("#divProcessButtons").show();
                }
                //SERIALISED FLAG
                if (data.Serialised) {
                    var $serialScanner = $("<div class='col-md-12 text-center'></div>");
                    var $serialScannerTextBox = $("<input type='text' id='SerialText' class='form-control' placeholder='Scan Serial'/>");
                    $serialScannerTextBox.on("change", function () {
                        $("#SerialText").on("keypress",
                            function (e) {
                                var data = {};

                                if (e.which == 13) {
                                    $.post('/api/product/serial-details/',
                                        data,
                                        function (result) {

                                            var $table = $("#divLocationsPicker #tblSOProductLocations");

                                            if ($table.length < 1) {
                                                $table = $("<table class='table table-bordered table-striped' id='tblSOProductLocations'></table>");
                                                $table.append("<thead><th>Product</th><th>Code</th><th>Serial</th><th>Actions</th></thead>");
                                                $("#divLocationsPicker").append($table);
                                            }

                                            if (result != null) {
                                                var $row = $("<tr><td>" + result.ProductName + "</td><td>" + result.ProductCode + "</td><td>" + result.SerialCode + "</td><td><a class='fa fa-trash'></a></td></tr>");
                                                $table.append($row);
                                            }
                                        });
                                }
                            });
                    });

                    $("#divLocationsPicker").append($serialScanner);
                }

                $("#tblSOProductLocations input[type=text].txt-pick-qty").ForceNumericOnly();

            });

        }

        var seriesObject = { SeriesText: '', SeriesNumber: 0 };

        function getSeriesObjectForString(text) {
            if (text == null) return '';
            var result = '0';
            var lastAlphaIndex = 0;
            for (var i = 0; i < text.length; i++) {
                if (parseInt(text[i]) > 0) {
                    result = result + text[i];
                } else {
                    result = '';
                    lastAlphaIndex = i;
                }
            }
            seriesObject.SeriesText = text.substring(0, lastAlphaIndex + 1);
            seriesObject.SeriesNumber = parseInt(result);
            return seriesObject;
        }

        function getCurrentUKDate() {

            var day = new Date().getDate() + 1;
            var month = new Date().getMonth() + 1;
            var year = new Date().getFullYear();

            if (day < 10) { day = "0" + day; } else day = day;
            if (month < 10) { month = "0" + month; } else month = month;

            return day + "/" + month + "/" + year;
        }

        var loadListItemsToDropdown = function (id, items, selValue) {
            $("#" + id).html("");
            for (var i = 0; i < items.length; i++) {
                $("#" + id).append("<option value='" + items[i].Value + "'>" + items[i].Text + "</option");
            }

            if ($("#" + id + "_chosen").length > 0) {
                $("#" + id).trigger("chosen:updated");
            }
            $("#" + id).val(selValue);
        }


        var ajaxPost = function (url, data, messageDiv, successCallback, showSuccess, successMessage, onErrorCallback) {
            $.ajax({
                url: url,
                type: "POST",
                data: data.contentType && data.contentType.indexOf("application/json") != -1 ? JSON.stringify(data) : data,
                contentType: data.contentType ? data.contentType : "application/x-www-form-urlencoded",
                traditional: true,
                success: function (result, textStatus, request) {

                    if (typeof (successCallback) === 'function') {
                        successCallback(result);
                    }

                    if (showSuccess) {
                        Gane.Helpers.ShowPopupMessage('Updated Successfully', successMessage !== null && successMessage !== undefined ? successMessage : "Update Successful", 2000);
                    }

                    if (messageDiv != null) {
                        $("#" + messageDiv).html(result);
                    }
                },
                error: function (request, status, error) {

                    if (onErrorCallback != null) {
                        return onErrorCallback();
                    }

                    if (error === "") {
                        Gane.Helpers.ShowPopupMessage('Error occurred', request);
                    } else {
                        Gane.Helpers.ShowPopupMessage('Error occurred', error);
                    }
                }

            });
        }

        return {
            AjaxPost: function (url, data, messageDiv, successCallback, showSuccess, successMessage, onErrorCallback) {
                return ajaxPost(url, data, messageDiv, successCallback, showSuccess, successMessage, onErrorCallback);
            },
            AddValidationErrorToElement: function ($selector, message) {
                return addValidationErrorToElement($selector, message);
            },
            LoadPickLocationsReceiveProduct: function (id) {
                return loadPickLocationsReceiveProduct(id);
            },
            ShowPopupMessage: function (title, htmlContent, type, callback) {
                return showPopupMessage(title, htmlContent, type, callback);
            },
            GetCurrentUKDate: function () {
                return getCurrentUKDate();
            },
            GetSeriesObjectForString: function (text) {
                return getSeriesObjectForString(text);
            },
            LoadListItemsToDropdown: function (id, items, selValue) {
                return loadListItemsToDropdown(id, items, selValue);
            },
           
        };
    }


    // Numeric only control handler
    jQuery.fn.ForceNumericOnly =
        function (allowNegative, noDecimalPoint, decimalPlaceLength, textLength, isInputTelphone) {

            return this.each(function () {
                $(this).keydown(function (e) {
                    var key = e.charCode || e.keyCode || 0;
                    // allow backspace, tab, delete, enter, arrows, numbers and keypad numbers ONLY
                    // home, end, period, and numpad decimal
                    if (e.shiftKey && (key == 9 || (isInputTelphone && key == 187 && $(this).val().length < 1))) {
                        return true;
                    }
                    if (e.shiftKey) {
                        return false;
                    }

                    if ((key == 110 || key == 190) && $(this).val().indexOf('.') > -1) {
                        return false;
                    }

                    if (((key >= 96 && key <= 105) || (key >= 48 && key <= 57)) && (textLength != undefined && $(this).val().length >= textLength)) {
                        if (e.target.selectionStart == e.target.selectionEnd)
                            return false;
                    }

                    if (((key >= 96 && key <= 105) || (key >= 48 && key <= 57)) && !noDecimalPoint && decimalPlaceLength != undefined && $(this).val().indexOf('.') > -1) {
                        var inputValue = $(this).val().split(".");
                        if (inputValue[1].length >= decimalPlaceLength && e.target.selectionStart == e.target.selectionEnd) {
                            return false;
                        }
                    }

                    if (!noDecimalPoint && (key == 110 || key == 190) && $(this).val().indexOf('.') < 0 && $(this).val().length < 1) {
                        $(this).val("0.");
                        return false;
                    }

                    return (((key == 109 || key == 189) && allowNegative && $(this).val().length < 1) ||
                        key == 8 ||
                        key == 9 ||
                        key == 13 ||
                        key == 46 ||
                        (!noDecimalPoint && (key == 110 || key == 190) && $(this).val().indexOf('.') < 0) ||
                        (key >= 35 && key <= 40) ||
                        (key >= 48 && key <= 57) ||
                        (key >= 96 && key <= 105) ||
                        (key == 107 && isInputTelphone && $(this).val().length < 1));
                });
            });
        };


    gane.Helpers = new gane.UtilityHelpers();

})(jQuery, Gane);