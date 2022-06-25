function CommonReportGenerateJS(Url, data) {
    debugger;

    //if (Url[0] == "/") {
    //    Url = readCookie("U") + Url;
    //}
    Url = readCookie("U") + Url;

    //Url = readCookie("U") + (Url[0] == "/") ? "" : "/" + Url;

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
            var path = window.location.href

            if (response.errorMessage != null && response.errorMessage != "") {
                CommonAlert(response.alert, response.errorMessage, null, null, "error");
            }
            $(".modalLoader").hide();
            if (response.report != null) {
                //var x = window.open(response.report, '_blank')
                //window.open(response.report, 'winname', 'directories=no,titlebar=no,toolbar=no,location=no,status=no,menubar=no,scrollbars=no,resizable=no,width=1000,height=1000');
                //$("#reportEmbed").attr("src", response.report)
                
                   //var win = window.open(response.report, '_blank');
                   // win.document.head.innerHTML = '<title>Hi</title></head>';                   
                   // var script = document.createElement('script');               
                   // script.innerHTML = "window.onload = function () {history.pushState(null, null, '');}"
                   // win.document.head.appendChild(script);


                //var contentId = "/" + response.areaName + "/" + response.selectedMenu + "/GenerateReport";
                //url = window.location.origin + contentId;
                //var x = window.open('_blank');
                //x.document.write('<iframe src="' + response.report + '" style="width:100%; height:1000px; " id="reportembedded" alt="pdf" title="Report" type="application/pdf" >');

                if (response.report.includes('aspx')) {
                    window.open(response.report, '_blank').focus();
                } else {
                    var x = window.open("", '_blank');
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
