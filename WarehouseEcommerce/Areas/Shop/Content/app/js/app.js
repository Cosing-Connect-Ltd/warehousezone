/* page loader */
$(document).ready(function () {
    var loader = $('#page-preloader');
    if (loader.length > 0) {
        //$(window).on('load', function () {
        $('#page-preloader').removeClass('visible');
        //});
    }
});

//scroll to top
$(document).ready(function () {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            $('#scroll').fadeIn();
        } else {
            $('#scroll').fadeOut();
        }
    });
    $('#scroll').click(function () {
        $("html, body").animate({
            scrollTop: 0
        }, 600);
        return false;
    });

    $("#btn_show_pass").click(function () {
        if ($("#input-password").is(":text") == false) {
            $('#input-password').attr('type', 'text');
            $('#btn_show_pass').attr('value', 'Hide');
        } else {
            $('#input-password').attr('type', 'password');
            $('#btn_show_pass').attr('value', 'Show');
        }
    });
});


/* responsive menu */
function openNav() {
    $('body, #mobileMenu').addClass("active");
}

function closeNav() {
    $('body, #mobileMenu').removeClass("active");
}


// fixed header
$(document).ready(function () {
    $(window).scroll(function () {
        if ($(document).width() > 767) {
            if ($(this).scrollTop() > 100) {
                $('header').addClass('fixed fadeInDown animated container');
            } else {
                $('header').removeClass('fixed fadeInDown animated container');
            }
        }
    });
})


function openSearch() {
    $('body').addClass("active-search");
    document.getElementById("search").style.height = "auto";
    $('#search').addClass("sideb");
    $('.search_query').attr('autofocus', 'autofocus').focus();
}

function closeSearch() {
    $('body').removeClass("active-search");
    document.getElementById("search").style.height = "0";
    $('#search').addClass("siden");
    $('.search_query').attr('autofocus', 'autofocus').focus();
}


$(document).ready(function () {
    if ($(window).width() >= 768) {
        var count_block = $('.ui-mega-menu-level').length;
        var number_blocks = 8;
        //console.log(count_block);
        if (count_block < number_blocks) {
            return false;
        } else {
            $('.ui-mega-menu-level').each(function (i, n) {
                $('ui-mega-menu-level').addClass(i);
                if (i == number_blocks) {
                    $(this).append('<li class="view_more my-menu"><i class="fa fa-plus thumb_img"></i><a class="dropdown-toggle">More Categories</a></li>');
                }
                if (i > number_blocks) {
                    $(this).addClass('wr_hide_menu').hide();
                }
            });
            $('.view_more').click(function () {
                $(this).toggleClass('active');
                $('.wr_hide_menu').slideToggle();
            });
        }
    }
});

/* left column responsive */
function ct_filters() {

    if ($(window).width() <= 767) {
        $('#column-left').appendTo('#content');
        $('.filterp').appendTo('.xs-filter');
    } else {
        $('#column-left').appendTo('#column-left');
    }
}
$(document).ready(function () {
    ct_filters();
});
$(window).resize(function () {
    ct_filters();
});

$(document).ready(function () {

    // Menu
    $('#menu .dropdown-menu').each(function () {
        var menu = $('#menu').offset();
        var dropdown = $(this).parent().offset();

        var i = (dropdown.left + $(this).outerWidth()) - (menu.left + $('#menu').outerWidth());

        if (i > 0) {
            $(this).css('margin-left', '-' + (i + 10) + 'px');
        }
    });

    // Product List
    $('#list-view').click(function () {
        $('#content .product-grid > .clearfix').remove();

        $('#content .row > .product-grid').attr('class', 'product-layout product-list col-xs-12');
        $('#grid-view').removeClass('active');
        $('#list-view').addClass('active');

        localStorage.setItem('display', 'list');
    });

    // Product Grid
    $('#grid-view').click(function () {
        // What a shame bootstrap does not take into account dynamically loaded columns
        var cols = $('#column-right, #column-left').length;

        if (cols == 2) {
            $('#content .product-list').attr('class', 'product-layout product-grid col-lg-6 col-md-6 col-sm-12 col-xs-12');
        } else if (cols == 1) {
            $('#content .product-list').attr('class', 'product-layout product-grid col-lg-3 col-md-4 col-sm-6 col-xs-12');
        } else {
            $('#content .product-list').attr('class', 'product-layout product-grid col-lg-3 col-md-3 col-sm-6 col-xs-12');
        }

        $('#list-view').removeClass('active');
        $('#grid-view').addClass('active');

        localStorage.setItem('display', 'grid');
    });

    if (localStorage.getItem('display') == 'list') {
        $('#list-view').trigger('click');
        $('#list-view').addClass('active');
    } else {
        $('#grid-view').trigger('click');
        $('#grid-view').addClass('active');
    }

    // Checkout
    $(document).on('keydown', '#collapse-checkout-option input[name=\'email\'], #collapse-checkout-option input[name=\'password\']', function (e) {
        if (e.keyCode == 13) {
            $('#collapse-checkout-option #button-login').trigger('click');
        }
    });

    // tooltips on hover
    $('[data-toggle=\'tooltip\']').tooltip({
        container: 'body'
    });

    // Makes tooltips work on ajax generated content
    $(document).ajaxStop(function () {
        $('[data-toggle=\'tooltip\']').tooltip({
            container: 'body'
        });
    });
});


/* Header Menu */
function main_menu() {
    if (jQuery(window).width() < 992) {
        jQuery('ul.nav li.dropdown a.header-menu').attr("data-toggle", "dropdown");
    } else {
        jQuery('ul.nav li.dropdown a.header-menu').attr("data-toggle", "");
    }
}
$(document).ready(function () {
    main_menu();
});
jQuery(window).resize(function () {
    main_menu();
});
jQuery(window).scroll(function () {
    main_menu();
});
/* Header Menu */


$(document).ready(function () {
    var flag = false;
    var ajax_search_enable = $('#ajax-search-enable').val();

    var current_cate_value = $('ul.cate-items li.selected').data('value');
    var current_cate_text = $('ul.cate-items li.selected').html();

    $('.cate-selected').attr('data-value', current_cate_value);
    $('.cate-selected').html(current_cate_text);

    $('.hover-cate p').click(function () {
        $(".cate-items").slideToggle("slow");
        $(".cate-items").addClass("sblock");
    });
 

    $('.ajax-result-container').hover(
        function () {
            flag = true;
        },
        function () {
            flag = false;
        }
    );

    $('.hover-cate').hover(
        function () {
            flag = true;
        },
        function () {
            flag = false;
        }
    );

});


$(document).ready(function () {
    if ($("#owl-testi").length > 1) {
        $("#owl-testi").owlCarousel({
            itemsCustom: [
                [0, 1]
            ],
            autoPlay: 7000,
            navigationText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
            navigation: false,
            pagination: true
        });
    }
    if ($("#bestseller").length > 0) {
        $("#bestseller").owlCarousel({
            itemsCustom: [
                [0, 1],
                [600, 2],
                [768, 1]
            ],
            // autoPlay: 1000,
            navigationText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
            navigation: true,
            pagination: false
        });
    }
    if ($("#special").length > 0) {
        $("#special").owlCarousel({
            itemsCustom: [
                [0, 1],
                [600, 2],
                [768, 1]
            ],
            // autoPlay: 1000,
            navigationText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
            navigation: true,
            pagination: false
        });
    }

    if ($("#slideshow0").length > 0) {
        $("#slideshow0").owlCarousel({
            itemsCustom: [
                [0, 1]
            ],
            autoPlay: 2500,
            animateIn: 'fadeIn',
            animateOut: 'fadeOut',
            loop: true,
            navigationText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
            navigation: false,
            pagination: true
        });
    }
    if ($("#feature").length > 0) {
        $("#feature").owlCarousel({
            itemsCustom: [
                [0, 1],
                [375, 2],
                [600, 3],
                [768, 2],
                [992, 3],
                [1200, 3],
                [1410, 4]
            ],
            // autoPlay: 1000,
            navigationText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
            navigation: true,
            pagination: false
        });
    }

    if ($("#latest").length > 0) {
        $("#latest").owlCarousel({
            itemsCustom: [
                [0, 1],
                [375, 2],
                [600, 3],
                [768, 2],
                [992, 3],
                [1200, 3],
                [1410, 4]
            ],
            // autoPlay: 1000,
            navigationText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
            navigation: true,
            pagination: false
        });
    }
    if ($("#onsale").length > 0) {
        $("#onsale").owlCarousel({
            itemsCustom: [
                [0, 1],
                [768, 1],
                [992, 2]
            ],
            // autoPlay: 1000,
            navigationText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
            navigation: true,
            pagination: false
        });
    }
    if ($(".tab-content .tab-pane #cattab").length > 0) {
        $(".tab-content .tab-pane #cattab").owlCarousel({
            itemsCustom: [
                [0, 1],
                [375, 2],
                [600, 1],
                [992, 2],
                [1200, 2],
                [1410, 3]
            ],
            // autoPlay: 1000,
            navigationText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
            navigation: true,
            pagination: false
        });
    }
    if ($("#blog").length > 0) {
        $("#blog").owlCarousel({
            itemsCustom: [
                [0, 1],
                [600, 2],
                [768, 2],
                [992, 3],
                [1200, 3],
                [1850, 3]
            ],
            // autoPlay: 1000,
            navigationText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
            navigation: true,
            pagination: false
        });
    }
    if ($("#carousel0").length > 0) {
        $("#carousel0").owlCarousel({
            itemsCustom: [
                [0, 2],
                [600, 4],
                [768, 5],
            ],
            autoPlay: 2500,
            loop: true,
            navigationText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
            navigation: false,
            pagination: false
        });
    }

    if ($('#gallery_01').length > 0) {
        $('#gallery_01').owlCarousel({
            itemsCustom: [
                [0, 2],
                [412, 3],
                [600, 4],
                [768, 4],
                [992, 3],
                [1200, 3],
                [1380, 4]
            ],
            autoPlay: 1000,
            autoPlay: true,
            navigation: false,
            navigationText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
            pagination: false
        });
    }
    if ($("#zoom_03").length > 0) {
        if (jQuery(window).width() > 991) {
            //initiate the plugin and pass the id of the div containing gallery images
            $("#zoom_03").elevateZoom({
                gallery: 'gallery_01',
                cursor: 'pointer',
                galleryActiveClass: 'active',
                imageCrossfade: true,
                loadingIcon: ''
            });
            //pass the images to Fancybox
            $("#zoom_03").bind("click", function (e) {
                var ez = $('#zoom_03').data('elevateZoom');
                $.fancybox(ez.getGalleryList());
                return false;
            });
        }
    }
    if ($("#related").length > 0) {
        $("#related").owlCarousel({
            itemsCustom: [
                [0, 1],
                [375, 2],
                [600, 3],
                [768, 2],
                [992, 3],
                [1200, 3],
                [1410, 4]
            ],
            // autoPlay: 1000,
            navigationText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
            navigation: true,
            pagination: false
        });
    }
    if ($('.thumbnails').length > 0) {
        $('.thumbnails').magnificPopup({
            type: 'image',
            delegate: 'a',
            gallery: {
                enabled: true
            }
        });
    }


    //plugin bootstrap minus and plus
    $(document).ready(function () {
        $('.btn-number').click(function (e) {
            e.preventDefault();
            var fieldName = $(this).attr('data-field');
            var type = $(this).attr('data-type');
            var input = $("input[name='" + fieldName + "']");
            var currentVal = parseInt(input.val());
            if (!isNaN(currentVal)) {
                if (type == 'minus') {
                    var minValue = parseInt(input.attr('min'));
                    if (!minValue) minValue = 1;
                    if (currentVal > minValue) {
                        input.val(currentVal - 1).change();
                    }
                    if (parseInt(input.val()) == minValue) {
                        $(this).attr('disabled', true);
                    }

                } else if (type == 'plus') {
                    var maxValue = parseInt(input.attr('max'));
                    if (!maxValue) maxValue = 999;
                    if (currentVal < maxValue) {
                        input.val(currentVal + 1).change();
                    }
                    if (parseInt(input.val()) == maxValue) {
                        $(this).attr('disabled', true);
                    }

                }
            } else {
                input.val(0);
            }
        });
        $('.input-number').focusin(function () {
            $(this).data('oldValue', $(this).val());
        });
        $('.input-number').change(function () {

            var minValue = parseInt($(this).attr('min'));
            var maxValue = parseInt($(this).attr('max'));
            if (!minValue) minValue = 1;
            if (!maxValue) maxValue = 999;
            var valueCurrent = parseInt($(this).val());
            var name = $(this).attr('name');
            if (valueCurrent >= minValue) {
                $(".btn-number[data-type='minus'][data-field='" + name + "']").removeAttr('disabled')
            } else {
                alert('Sorry, the minimum value was reached');
                $(this).val($(this).data('oldValue'));
            }
            if (valueCurrent <= maxValue) {
                $(".btn-number[data-type='plus'][data-field='" + name + "']").removeAttr('disabled')
            } else {
                alert('Sorry, the maximum value was reached');
                $(this).val($(this).data('oldValue'));
            }
        });
        $(".input-number").keydown(function (e) {
            // Allow: backspace, delete, tab, escape, enter and .
            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 190]) !== -1 ||
                // Allow: Ctrl+A
                (e.keyCode == 65 && e.ctrlKey === true) ||
                // Allow: home, end, left, right
                (e.keyCode >= 35 && e.keyCode <= 39)) {
                // let it happen, don't do anything
                return;
            }
            // Ensure that it is a number and stop the keypress
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                e.preventDefault();
            }
        });
    });


});