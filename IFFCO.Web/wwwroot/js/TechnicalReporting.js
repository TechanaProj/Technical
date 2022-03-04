function CommonReportGenerateJS(Url, data) {
    debugger;

    if (Url[0] == "/") {
        Url = readCookie("U") + Url;
    } 
    $(".modalLoader").css("display", "block");
    $.ajax({
        type: "POST",
        url: Url,
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

function createCookie(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}

function isNull(val, returnVal) {
    debugger;
    if (val == undefined || val == null || isNaN(val) || val=="") {
        return returnVal;
    }
    return val;
}
