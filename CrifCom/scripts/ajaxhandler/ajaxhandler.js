$(document).ready(function () {
    $("#loader").hide();
    $(".no-items").hide();
    var dateChange = false;
    var marketChange = false;
    var eventChange = false;   

    $("#market").change(function () {
        if (!marketChange) {
            $('select option:contains("market")').text('All');
            marketChange = true;
        }
        var market = $("#market").val();
        var eventsortraining = "";
        if ($("#event").length) {
           eventsortraining = $("#event").val();
        }
        var date = $("#date").val();
        var newsId = $('#txtNodId').val();
        var data = { market: market,newsId: newsId, e: eventsortraining, date: date};
        callAjax(data);
    });

    $("#date").change(function () {        
        var market = $("#market").val();
        var eventsortraining = "";
        if ($("#event").length) {
            eventsortraining = $("#event").val();
        }
        var date = $("#date").val();
        var newsId = $('#txtNodId').val();
        var data = { market: market, newsId: newsId, e: eventsortraining, date: date };
        callAjax(data);
    });

    $("#event").change(function () {
        var market = $("#market").val();
        var eventsortraining = "";
        if ($("#event").length) {
            eventsortraining = $("#event").val();
        }
        var date = $("#date").val();
        var newsId = $('#txtNodId').val();
        var data = { market: market,newsId: newsId, e: eventsortraining, date: date };       
        callAjax(data);
    });

    addClickForPagination();
    function addClickForPagination() {
        $('.with-arrow.disabled').css('pointer-events', 'none');
        $(".page-link.page-number").click(function (e) {
            var pageNo = $(this).text();
            var market = $("#market").val();
            var eventsortraining = "";
            if ($("#event").length) {
                eventsortraining = $("#event").val();
            }
            var date = $("#date").val();
            var newsId = $('#txtNodId').val();
            var data = { market: market, newsId: newsId, e: eventsortraining, date: date, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addNextClickForPagination();
    function addNextClickForPagination() {        
        $(".next-page").click(function (e) {            
            var pageNo = parseInt($('#txtPage').val()) + 1;
            var market = $("#market").val();
            var eventsortraining = "";
            if ($("#event").length) {
                eventsortraining = $("#event").val();
            }
            var date = $("#date").val();
            var newsId = $('#txtNodId').val();
            var data = { market: market, newsId: newsId, e: eventsortraining, date: date, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addPreviousClickForPagination();
    function addPreviousClickForPagination() {        
        $(".previous-page").click(function (e) {            
            var pageNo = parseInt($('#txtPage').val()) - 1;
            var market = $("#market").val();
            var eventsortraining = "";
            if ($("#event").length) {
                eventsortraining = $("#event").val();
            }
            var date = $("#date").val();
            var newsId = $('#txtNodId').val();
            var data = { market: market, newsId: newsId, e: eventsortraining, date: date, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addClickForLastPagePagination();
    function addClickForLastPagePagination() {        
        $(".last-page").click(function (e) {            
           var pageNo = parseInt($('#txtTotlaPage').val());
            var market = $("#market").val();
            var eventsortraining = "";
            if ($("#event").length) {
                eventsortraining = $("#event").val();
            }
            var date = $("#date").val();
            var newsId = $('#txtNodId').val();
            var data = { market: market, page: pageNo, newsId: newsId, e: eventsortraining, date: date };
            callAjax(data);
            e.preventDefault();
        });
    }

    addClickForFirstPagePagination();
    function addClickForFirstPagePagination() {
        $(".first-page").click(function (e) {
            var pageNo = 1;
            var market = $("#market").val();
            var eventsortraining = "";
            if ($("#event").length) {
                eventsortraining = $("#event").val();
            }
            var date = $("#date").val();
            var newsId = $('#txtNodId').val();
            var data = { market: market, page: pageNo, newsId: newsId, e: eventsortraining, date: date };
            callAjax(data);
            e.preventDefault();
        });
    }


    function callAjax(data) { 
	
        $("#loader").show();
        $(".no-items").hide();
        $.ajax({
            url: "/NewsAjaxHandler/",
            type: "GET",
            data: data
        }).success(function (msg) {            
            if (msg != null && msg.length > 1) {
                $('.news-ajax-section').empty();
                $('.news-ajax-section').append(msg);
                if ($('#txtPage').val() != 1) {
                    $(".listing-type-two").hide();
                }
                else {
                    $(".listing-type-two").show();
                }
                addClickForPagination();
                addNextClickForPagination();                
                addPreviousClickForPagination();
                addClickForLastPagePagination();
                addClickForFirstPagePagination();                
            }
            else {
                $(".no-items").show();
                $('.news-ajax-section').empty();
                
            }
        }).complete(function () {
            $("#loader").hide();
        });
    }
});