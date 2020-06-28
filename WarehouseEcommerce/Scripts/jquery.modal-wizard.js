(function ($) {
    $.fn.modalWizard = function () {
        return this
            .on('show.bs.modal', function () {
                // init the steps
                updateModalStep($(this));
            })
            .on('hide.bs.modal', function () {
                // some code for later
            })
            .on("navigate", function (e, navDir, stepNumber) {
                var $this = $(this);
                if (stepNumber) {
                    $this.attr("data-current-step", stepNumber);
                }
                else if (navDir === 'next') {
                    var Numbers = $("#kit-product").attr("data-current-step");
                    var totallength = $("#kit-product").find('[data-step]').length;
                    var qty;
                    var status = $("#fieldset_" + Numbers).find("#customCheck").is(":checked");
                    var same = $("#fieldset_" + Numbers).data("same");
                    if (status) {
                        status = false;
                        qty = $("#fieldset_" + Numbers).find("#customCheck").val();
                        if (qty > 1) {
                            for (i = 1; i < parseInt(qty); i++) {
                                var steps = parseInt(Numbers) + 1;
                                var nextsameData = $("#fieldset_" + steps).data("same");
                                if (nextsameData === same) {
                                    status = true;
                                    var htmls = $("#fieldset_" + Numbers).html();
                                    $("#fieldset_" + steps).html("");
                                    $("#fieldset_" + steps).html(htmls);
                                    Numbers = parseInt(Numbers) + 1;
                                }
                            }

                        }

                    }
                    $this.attr("data-current-step", status ? parseInt(parseInt(Numbers) >= parseInt(totallength) ? parseInt(totallength) : parseInt(Numbers + 1)) : +$this.attr("data-current-step") + 1);
                }
                else {
                    $this.attr("data-current-step", +$this.attr("data-current-step") - 1);
                }
                updateModalStep($this);
            })
            .on('update', function () {
                // some code for later
            })
            .on('reset', function () {
                // to reset the modal
                // check if it's a form and reset it
                var $this = $(this),
                    $form = $this.find('form');
                $this.attr('data-current-step', $this.data('current-step')); // $.fn.data only store the inital value
                if (this.reset) {
                    this.reset();
                } else if ($form.length) {
                    $form.get(0).reset();
                } else {
                    $this.find('input').val('');
                }
            })
            .on('click', '[data-submit], [type=submit]', function (e) {
                var kitProductCartItems = [];
                var pid = $("#main-kit-product").val();
                $("[id^=fieldset_]").each(function (e) {
                    var productid = $(this).find("#selected-productids").data("productid");
                    var qty = $(this).find("#customCheck").val();
                    if (productid !== undefined && productid !== null && productid !== "" && productid > 0) {

                        kitProductCartItems.push({ "SimpleProductId": productid, "Quantity": qty, "KitProductId": pid });
                    }

                });
                PostAttributeValues(pid, kitProductCartItems);

            })
            .on('click', '[data-step-to]', function (e) {
                var $this = $(e.target);
                var $modal = $(e.delegateTarget);
                $modal.trigger('navigate', [$this.data("step-to")]);
            });
    }

    function updateModalStep($modal) {
        var step = +$modal.attr("data-current-step"),
            stepsLength = $modal.find('[data-step]').length;
        if (step === 1 && step !== stepsLength) {
            $modal.find('[data-step-to=prev]').hide().end().find('[data-step-to=next]').show().end().find('[data-submit], [type=submit]').hide();
        } else if (step === stepsLength) {
            $modal.find('[data-step-to=next]').hide().end().find('[data-step-to=prev]').show().end().find('[data-submit], [type=submit]').show();
        } else if (step > stepsLength && step < 0) {
            return;
        } else {
            $modal.find('[data-step-to]').show().end().find('[data-submit], [type=submit]').hide();
        }
        $modal
            .find("[data-step=" + step + "]")
            .show()
            .siblings("[data-step]")
            .hide()
            .end()
            .trigger('update', [step]);
    }

    function checkValidate($modal) {
        var $step = $modal.find(':required:invalid').closest('[data-step]');
        if ($step.length) $modal.trigger('navigate', [null, $step.data('step')]);
    }
    function PostAttributeValues(pid, kititems) {

        $.ajax({
            url: basePath + '/Products/_CartItemsPartial',
            method: 'post',
            data: { ProductId: pid, kitProductCartItems: kititems },
            dataType: 'json',
            success: function (data) {
                window.location.href = basePath + '/Products/AddToCart';
                $("#kit-product").hide();
            },
            error: function (err) {
                window.location.href = basePath + '/Products/AddToCart';
                $("#kit-product").hide();
            }
        });

    }

})(jQuery);
