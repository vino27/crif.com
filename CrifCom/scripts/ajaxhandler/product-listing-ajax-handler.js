$(document).ready(function () {
    $(".loader").hide();
    $(".no-items").hide();
    var dateChange = false;
    var marketChange = false;
    var eventChange = false;

    $(".dropdown-product-listing").change(function () {
        if (!marketChange) {
            $('select option:contains("market")').text('All');
            marketChange = true;
        }
        var market = $(this).val();
        var sectionId = $(this).closest("section").attr("id");
        var data = { market: market, family: sectionId };
        callAjax(data, sectionId);
    }); 
    function callAjax(data, section) {
        var $divContainer = $("#" + section + "").find(".product-list-container");
        var $loder = $("#" + section + "").find(".loader");
        var $noitems = $("#" + section).find(".no-items"); 
        $loder.show();
        $noitems.hide();
        $.ajax({
            url: "/ProductListingAjaxHandler/",
            type: "GET",
            data: data
        }).success(function (msg) { 
            if (msg != null && msg.length > 1 && msg.trim()!="") {
                $divContainer.empty();
                $divContainer.append(msg);
                $noitems.hide();
            }
            else {
                 $noitems.show();
                 $divContainer.empty();

            }
        }).complete(function () {

            $loder.hide();
        });
    }
});