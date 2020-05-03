$(function () {
  var firstNavList = $(".nb_mainnav_list").html();
  $(".nb_firstNav_res_lst_wrap").append(
    "<ul class='nb_firstNav_res_lst'>" + firstNavList + "</ul>"
  );

  var secNavList = $(".nb_sec_navlist").html();
  $(".res_navWrapper").append(
    "<ul class='nb_sec_nav_res'>" + secNavList + "</ul>"
  );

  $(".nb_res_nav_closeTrigger").click(function () {
    $(".res_navWrapper").removeClass("show");
  });
  $("#nb_res_navTrigger").click(function () {
    $(".res_navWrapper").addClass("show");
  });

  // banner slider
  $(".nb_hm_bnr_carrousle").carousel({
    interval: 5000,
    touch: true,
    pause: false,
    slide: true,
  });

  var totalItems = $(".item").length;
  var currentIndex = $("div.active").index() + 1;
  $(".nb_bnr_count.rgt").html("0" + totalItems);
  $(".nb_bnr_count.lft").html("0" + currentIndex);
  $(".nb_hm_bnr_carrousle").bind("slid.bs.carousel", function () {
    currentIndex = $("div.active").index() + 1;
    // $(".num").html("" + currentIndex + "/" + totalItems + "");
    $(".nb_bnr_count.lft").html("0" + currentIndex);
    // $(".nb_bnr_count.rgt").html("0" + totalItems);
  });

  // feautered product slider
  $(".nb_featuredPrdCarrousle").owlCarousel({
    loop: true,
    margin: 10,
    nav: true,
    responsive: {
      0: {
        items: 1,
      },
      365: {
        items: 2,
      },
      768: {
        items: 2,
      },
      1000: {
        items: 4,
      },
    },
  });

  // image_zoomer_btmslider
  $(".nb_imageZoomCarrousle").owlCarousel({
    loop: false,
    margin: 10,
    nav: false,
    autoplay: false,
    repeat: false,
    responsive: {
      0: {
        items: 3,
      },
      365: {
        items: 4,
      },
      768: {
        items: 4,
      },
      1000: {
        items: 4,
      },
    },
  });
  let imgInViewarea = $(".img_view_area img");
  // $(".nb_imageZoomCarrousle div.item img").click(function(e) {
  //   let activeImgsrc = e.target.src;
  //   imgInViewarea.attr("src", activeImgsrc);
  // });

  $("body").on("click", ".nb_imageZoomCarrousle div.item img", function (e) {
    let activeImgsrc = e.target.src;
    imgInViewarea.attr("src", activeImgsrc);
    imgInViewarea.attr("data-zoom-image", activeImgsrc);
    $(".ZoomViewWrapper .container .img_view_area img").attr(
      "src",
      activeImgsrc
    );
    $(".ZoomViewWrapper .container .img_view_area img").attr(
      "data-zoom-image",
      activeImgsrc
    );
  });

  $("#zoomViewTrigger").click(function (e) {
    let aaa = $("#Prod_carr_viewer").html();
    $(".ZoomViewWrapper .container").append(aaa);
    $(".ZoomViewWrapper").addClass("show");
    $("body").addClass("ZoomerActive");
  });

  $("body").on("click", "#closeZoomViewer", function () {
    $(".ZoomViewWrapper").removeClass("show");
    $(".ZoomViewWrapper .container").html("");
    $("body").removeClass("ZoomerActive");
  });

  // basket table counter

  $(".basketTable #incre_count").click(function () {
    let aaa = $(this).closest("#counter").val();
    alert(aaa);
    let currCoutVal = $(".basketTable #counter").val();
    currCoutVal = parseInt(currCoutVal);
    let increVal = currCoutVal + 1;
    $(".basketTable #counter").val(increVal);
  });
  $(".basketTable #decre_count").click(function () {
    let currCoutVal = $(".basketTable #counter").val();
    currCoutVal = parseInt(currCoutVal);
    if (currCoutVal > 1) {
      let increVal = currCoutVal - 1;
      $(".basketTable #counter").val(increVal);
    }
  });
  // show filter in responsive view
  $("#res_filterBox_trigger").click(function () {
    $(".nb_prd_filterwrap").toggleClass("show");
  });

  // showLoginForm
  // showRegForm
  $("#showRegForm").click(function () {
    $(".registration_form_sec").addClass("show");
    $(".login_form_sec").removeClass("show");
  });
  $("#showLoginForm").click(function () {
    $(".registration_form_sec").removeClass("show");
    $(".login_form_sec").addClass("show");
  });
  $("#triggerSearch").click(function (e) {
    e.preventDefault();
    $(".search_wrap_main").addClass("show");
    $("#searchinput").focus();
  });
  $("#serchBtnico").click(function (e) {
    e.preventDefault();
    $(".search_wrap_main").removeClass("show");
  });
});
function getVoidData() {
  return false;
}
