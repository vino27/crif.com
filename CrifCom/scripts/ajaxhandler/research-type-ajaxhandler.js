$(document).ready(function () {
    $("#loader").hide();
    $(".listing-type-two").show();
    addClickForPagination();
    function addClickForPagination() {
        $('.with-arrow.disabled').css('pointer-events', 'none');
        $(".page-link.page-number").click(function (e) {
            var pageNo = $(this).text();            
            var newsId = $('#txtNodId').val();
            var data = {newsId: newsId, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addNextClickForPagination();
    function addNextClickForPagination() {
        $(".next-page").click(function (e) {
            var pageNo = parseInt($('#txtPage').val()) + 1;
            var newsId = $('#txtNodId').val();
            var data = { newsId: newsId, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addPreviousClickForPagination();
    function addPreviousClickForPagination() {
        $(".previous-page").click(function (e) {
            var pageNo = parseInt($('#txtPage').val()) - 1;
            var newsId = $('#txtNodId').val();
            var data = { newsId: newsId, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addClickForLastPagePagination();
    function addClickForLastPagePagination() {
        $(".last-page").click(function (e) {
            var pageNo = parseInt($('#txtTotlaPage').val());
            var newsId = $('#txtNodId').val();
            var data = { newsId: newsId, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addClickForFirstPagePagination();
    function addClickForFirstPagePagination() {
        $(".first-page").click(function (e) {
            var pageNo = 1;
            var newsId = $('#txtNodId').val();
            var data = { newsId: newsId, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    function callAjax(data) {
        $("#loader").show();        
        $.ajax({
            url: "/ResearchtypeAjaxHandler/",
            type: "GET",
            data: data
        }).success(function (msg) {
            if (msg != null && msg.length > 1) {
                $('.reseatch-type-home-ajax-section').empty();
                $('.reseatch-type-home-ajax-section').append(msg);
                if ($('#txtPage').val() != 1)
                {
                    $(".listing-type-two").hide();
                }
                else
                {
                    $(".listing-type-two").show();
                }
                addClickForPagination();
                addNextClickForPagination();
                addPreviousClickForPagination();
                addClickForLastPagePagination();
                addClickForFirstPagePagination();
            }
        }).complete(function () {
            $("#loader").hide();
        });
    }

});