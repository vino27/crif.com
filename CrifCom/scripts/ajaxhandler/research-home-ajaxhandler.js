$(document).ready(function () {
    $("#loader").hide();
    $(".no-items").hide();
    var marketChange = false;
    var topicChange = false;

    $("#market").change(function () {        
        if (!marketChange) {
            $('select option:contains("market")').text('All');
            marketChange = true;
        }
        var market = $("#market").val();
        var topic = $("#topic").val();       
        var researchId = $('#txtNodId').val();
        var data = { market: market, researchId: researchId, topics: topic };
        callAjax(data);
    });
    
        $("#topic").change(function () {
          
            if (!topicChange) {
                $('select option:contains("topic")').text('All');
                topicChange = true;
            }
            var market = $("#market").val();
            var topic = $("#topic").val();
            var researchId = $('#txtNodId').val();
            var data = { market: market, researchId: researchId, topics: topic };
            callAjax(data);
        });  

    addClickForPagination();
    function addClickForPagination() {
        $('.with-arrow.disabled').css('pointer-events', 'none');
        $(".page-link.page-number").click(function (e) {
            var pageNo = $(this).text();
            var market = $("#market").val();
            var topic = $("#topic").val();
            var researchId = $('#txtNodId').val();
            var data = { market: market, topics: topic, researchId: researchId, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addNextClickForPagination();
    function addNextClickForPagination() {
        $(".next-page").click(function (e) {
            var pageNo = parseInt($('#txtPage').val()) + 1;
            var market = $("#market").val();
            var topic = $("#topic").val();
            var researchId = $('#txtNodId').val();
            var data = { market: market, topics: topic, researchId: researchId, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addPreviousClickForPagination();
    function addPreviousClickForPagination(e) {
        $(".previous-page").click(function (e) {
            var pageNo = parseInt($('#txtPage').val()) - 1;
            var market = $("#market").val();
            var topic = $("#topic").val();
            var researchId = $('#txtNodId').val();
            var data = { market: market, topics: topic, researchId: researchId, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addClickForLastPagePagination();
    function addClickForLastPagePagination() {        
        $(".last-page").click(function (e) {
            var pageNo = parseInt($('#txtTotlaPage').val());
            var market = $("#market").val();
            var topic = $("#topic").val();
            var researchId = $('#txtNodId').val();
            var data = { market: market, topics: topic, researchId: researchId, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    addClickForFirstPagePagination();
    function addClickForFirstPagePagination() {
        $(".first-page").click(function (e) {
            var pageNo = 1;
            var market = $("#market").val();
            var topic = $("#topic").val();
            var researchId = $('#txtNodId').val();
            var data = { market: market, topics: topic, researchId: researchId, page: pageNo };
            callAjax(data);
            e.preventDefault();
        });
    }

    function callAjax(data) {
        $("#loader").show();
        $(".no-items").hide();
        $.ajax({
            url: "/ResearchHomeAjaxLHandler/",
            type: "GET",
            data: data
        }).success(function (msg) {                       
            if (msg != null && msg.length > 1) {
                $(".no-items").hide();
                $('.research-ajax-section').empty();
                $('.research-ajax-section').append(msg);                
                addClickForPagination();
                addNextClickForPagination();
                addPreviousClickForPagination();
                addClickForLastPagePagination();
                addClickForFirstPagePagination();
            }
            else {                
                $(".no-items").show();
                $('.research-ajax-section').empty();

            }
        }).complete(function () {
            $("#loader").hide();
        });
    }
});