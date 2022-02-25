function CommonReportGenerateJS(url, data) {
    debugger;
    $(".modalLoader").css("display", "block");
    $.ajax({
        type: "POST",
        url: url,
        data: data,
        error: function (xhr, status, error) {

            CommonAlert(status, error, SubmitPopup, url, "error");
        },
        success: function (response) {
            debugger;
            var contentId = "/" + response.areaName + "/" + response.selectedMenu + "/GenerateReport";
            url = window.location.origin + contentId;

            if (response.errorMessage != null && response.errorMessage != "") {
                CommonAlert(response.alert, response.errorMessage, null, null, "error");
            }
            $(".modalLoader").hide();
            if (response.report != null) {
                var x = window.open('', '_blank');
                x.document.write('<body></body>');
                x.location.hash = response.selectedMenu;
                var embedtag = x.document.createElement('embed');
                embedtag.id = 'reportEmbed';
                embedtag.src = response.report;
                embedtag.style = "width:100%; height:100%;";
                embedtag.alt = "pdf";
                embedtag.title = "Report";
                embedtag.type = "application/pdf";
                embedtag.pluginspage = "http://www.adobe.com/products/acrobat/readstep2.html";
                x.document.body.appendChild(embedtag);
                x.document.title = response.selectedMenu;
            }


        }
    });
}