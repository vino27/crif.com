$(document).ready(function () {
    $("#loader").hide();
    $(".no-items").hide();
    var dateChange = false;
    var marketChange = false;
    var eventChange = false;

    $("#market-needs").change(function () {
        if (!marketChange) {
            $('select option:contains("market")').text('All');
            marketChange = true;
        }
        var market = $(this).val();        
        var newsId = $('#txtNodId').val();
        var data = { market: market, newsId: newsId};
        callAjax(data);
    }); 
    addClickForPagination();
    function addClickForPagination() {
        $('.with-arrow.disabled').css('pointer-events', 'none');
        $(".page-link.page-number").click(function (e) {
            var pageNo = $(this).text();
            var market = $("#market-needs").val();
            var newsId = $('#txtNodId').val();
            var data = { market: market, newsId: newsId, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addNextClickForPagination();
    function addNextClickForPagination() {
        $(".next-page").click(function (e) {
            var pageNo = parseInt($('#txtPage').val()) + 1;
            var market = $("#market-needs").val();
            var newsId = $('#txtNodId').val();
            var data = { market: market, newsId: newsId, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addPreviousClickForPagination();
    function addPreviousClickForPagination() {
        $(".previous-page").click(function (e) {
            var pageNo = parseInt($('#txtPage').val()) - 1;
            var market = $("#market-needs").val();
            var newsId = $('#txtNodId').val();
            var data = { market: market, newsId: newsId, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addClickForLastPagePagination();
    function addClickForLastPagePagination() {
        $(".last-page").click(function (e) {
            var pageNo = parseInt($('#txtTotlaPage').val());
            var market = $("#market-needs").val();
            var newsId = $('#txtNodId').val();
            var data = { market: market, newsId: newsId, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addClickForFirstPagePagination();
    function addClickForFirstPagePagination() {
        $(".first-page").click(function (e) {
            var pageNo = 1;
            var market = $("#market-needs").val();
            var newsId = $('#txtNodId').val();
            var data = { market: market, page: pageNo, newsId: newsId };
            callAjax(data);
            e.preventDefault();
        });
    }


    function callAjax(data) {
        $("#loader").show();
        $(".no-items").hide(); 
        $.ajax({
            url: "/SuccessStoryAjaxHandler/",
            type: "GET",
            data: data
        }).success(function (msg) {
          
            if (msg != null && msg.length > 1) { 
                $('.success-ajax-section').empty();
                $('.success-ajax-section').append(msg);
                addClickForPagination();
                addNextClickForPagination();
                addPreviousClickForPagination();
                addClickForLastPagePagination();
                addClickForFirstPagePagination();
            }
            else { 
                $(".no-items").show();
                $('.success-ajax-section').empty();

            }
        }).complete(function () {
          
            $("#loader").hide();
        });
    }
});