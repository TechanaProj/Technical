function CommonAjax(Url, Type, Async, ContentType, Cache, fn, Id, content) {
    if (Url != "") {
        debugger;
        if (Url[0] == "/") {
            Url = readCookie("U") + Url;
        } 

        $.ajax({
            url: Url,
            type: Type,
            async: Async,
            data: content,
            contentType: ContentType,
            cache: Cache,
            success: function (data) {                
                CallBack(fn, data, Id);
            },
            error: function (response) {                
                CommonAlert("Error", response.statusText, null, null, "error");
                //alert(response.responseText);
            },
            failure: function (response) {                
                CommonAlert("Failure", response.statusText, null, null, "error");
                //alert(response.responseText);
            }
        });
    }
}
function CommonAjax1(Url, Type, Async, ContentType, Cache, fn, Id, content, headersData) {    
    if (Url != "") {
        debugger;
        if (Url[0] == "/") {
            Url = readCookie("U") + Url;
        } 

        $.ajax({
            url: Url,
            type: Type,
            async: Async,
            // data: content,
            contentType: ContentType,
            cache: Cache,
            headers: headersData,
            success: function (data) {

                CallBack(fn, data, Id);
            },
            error: function (response) {
                CommonAlert("Error", response.statusText, null, null, "error");
            },
            failure: function (response) {
                CommonAlert("Failure", response.statusText, null, null, "error");
            }
        });
    }
}

function CommonAjaxComboBox(Url, Type, Async, ContentType, Cache, fn, Id, content) {    
    if (Url != "") {
        $.ajax({
            url: Url,
            type: Type,
            async: Async,
            data: content,
            contentType: ContentType,
            cache: Cache,
            success: function (data) {

                CallBack(fn, data, Id);
                $(Id).select2({
                    Data: data
                });

            },
            error: function (response) {
                CommonAlert("Error", response.statusText, null, null, "error");
            },
            failure: function (response) {
                CommonAlert("Failure", response.statusText, null, null, "error");
            }
        });
    }
}


function CallBack(fn, data, Id) {    
    return fn(data, Id);
}
function BindEdit(Data, id) {
    var InputControlles = $("table[name='EditControll']").children("input");
    var KeysName = Object.keys(Data.gradeGroupMsts);
    $.each(KeysName, function (index, value) {

        $("input[name='" + value.charAt(0).toUpperCase() + value.slice(1) + "']").val(Data.gradeGroupMsts[value]);
    })
}
function PostEdit(Data, id) {
    var InputControlles = $("table[name='EditControll']").children("input");
    var KeysName = Object.keys(Data.gradeGroupMsts);
    $.each(KeysName, function (index, value) {

        $("input[name='" + value.charAt(0).toUpperCase() + value.slice(1) + "']").value(Data.gradeGroupMsts[value]);
    })
}
function CustomAutocomplete(txtId, hdnId, url, Type, DataType, ContentType) {
    $(txtId).autocomplete({
        source: function (request, response) {
            $.ajax({
                url: url + request.term,
                //data: "{ 'prefix': '" + request.term + "'}",
                dataType: DataType,
                type: Type,
                contentType: ContentType,
                success: function (data) {
                    response($.map(data, function (item) {

                        return item;
                    }))

                },
                error: function (response) {
                    alert(response.responseText);
                },
                failure: function (response) {
                    alert(response.responseText);
                }
            });
        },
        //select: function (e, i) {
        //    $(hdnId).val(i.item.val);
        //},
        minLength: 1
    });
}

//$(function () {
//    $("#txtSate").autocomplete({
//        source: [
//            { label: "India", value: "IND" },
//            { label: "Australia", value: "AUS" }
//        ]
//    });
//});

function BindDropDown(data, id) {    
    var districtHtml = "<option>---Select---</option>";

    $.each(data, function (index, District) {
        districtHtml += "<option value=" + District.value + ">" + District.text + "</option>";

    });
    $(id).html(districtHtml);
}
function BindDropDownAll(data, id) {
    var districtHtml = "<option value='---Select---'>All</option>";

    $.each(data, function (index, District) {
        districtHtml += "<option value=" + District.value + ">" + District.text + "</option>";

    });
    $(id).html(districtHtml);
}
function BindDropDownWithoutSelect(data, id) {
    var districtHtml = "";

    $.each(data, function (index, District) {
        districtHtml += "<option value=" + District.value + ">" + District.text + "</option>";

    });
    $(id).html(districtHtml);
}
function ConvertDDLtoCombo(data, id) {
    var districtHtml = "<option>---Select---</option>";

    $.each(data, function (index, fetchData) {
        if (fetchData.Selected) {
            districtHtml += "<option selected='selected' value=" + fetchData.value + ">" + fetchData.text + "</option>";
        }
        else {
            districtHtml += "<option value=" + fetchData.value + ">" + fetchData.text + "</option>";
        }

    });
    $(id).html(districtHtml);
    $(id).select2();
}

function accessrights(btnselect, btninsert, btnupdate, btndelete) {
    if (btnselect !== "N") {
        $('#Select').removeAttr('disabled');
    }
    else {
        $('#Select').attr('disabled', 'disabled');
    }
    if (btninsert !== "N") {
        $('#Insert').removeAttr('disabled');
    }
    else {
        $('#Insert').attr('disabled', 'disabled');
    }
    if (btnupdate !== "N") {
        $('.edit').show();
    }
    else {
        $('.edit').hide();
    }
    if (btndelete !== "N") {
        $('.delete').show();
    }
    else {
        $('.delete').hide();
    }
}
function SHOW_TR_Function(e, e1, id) {   
    var TRID = document.getElementById(e);
    var TRID1 = document.getElementById(e1);
    TRID.style.display = '';
    TRID1.style.display = "none";
    
}
function HIDE_TR_Function(e) {
    var TRID = document.getElementById(e);
    TRID.style.display = "none";
}

function BindGrid(ExcelColumns, PdfColumns) {    
    if ($('#HrmsGrid').DataTable().context.length>0) {
        var table = $('#HrmsGrid').DataTable();
        var tableArray = [
            {
                extend: 'excel',
                text: '<img src="../images/excel.png" style="width:20px; heigth:10px;">',
                exportOptions: {
                    columns: ExcelColumns
                }
            },
            {
                extend: 'pdfHtml5',
                text: '<img src="../images/pdf.png" style="width:20px; heigth:10px;">',
                exportOptions: {
                    columns: PdfColumns
                }
            }
        ]
        new $.fn.dataTable.Buttons(table, {
            buttons: tableArray
        });

        $("select[name='HrmsGrid_length']").css("height", "37");

        table.buttons(0, null).container().prependTo(
            table.table().container()
        )
    }
}

function BindGridWithId(ExcelColumns, PdfColumns,Id) {    
    if ($(Id).DataTable().context.length > 0) {
        var table = $(Id).DataTable();
        var tableArray = [
            {
                extend: 'excel',
                text: '<img src="../images/excel.png" style="width:20px; heigth:10px;">',
                exportOptions: {
                    columns: ExcelColumns
                }
            },
            {
                extend: 'pdfHtml5',
                text: '<img src="../images/pdf.png" style="width:20px; heigth:10px;">',
                exportOptions: {
                    columns: PdfColumns
                }
            }
        ]
        new $.fn.dataTable.Buttons(table, {
            buttons: tableArray
        });

        $("select[name='HrmsGrid_length']").css("height", "37");

        table.buttons(0, null).container().prependTo(
            table.table().container()
        )
    }
}

function BindGridWithIdAll(ExcelColumns, PdfColumns, Id) {
    var table = $(Id).DataTable({
        lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]]
    });
    
    var tableArray = [
        {
            extend: 'excel',
            text: '<img src="../images/excel.png" style="width:20px; heigth:10px;">',
            exportOptions: {
                columns: ExcelColumns
            }
        },
        {
            extend: 'pdfHtml5',
            text: '<img src="../images/pdf.png" style="width:20px; heigth:10px;">',
            exportOptions: {
                columns: PdfColumns
            }
        }
    ]
    new $.fn.dataTable.Buttons(table, {
        buttons: tableArray,
    });

    $("select[name='HrmsGrid_length']").css("height", "37");

    table.buttons(0, null).container().prependTo(
        table.table().container()
    )
}

function GenrateRow(rows, columns) {
    var strHtml = "";
    $.each(rows, function (rowIndex, rowValue) {
        strHtml += "<tr>";
        $.each(columns, function (colIndex, colValue) {
            strHtml += "<td>";
            strHtml += GenrateInputType(colValue.Type, colValue.Name, colValue.Value, colValue.ClassName, colValue.Id, colValue.fnName, colValue.AttrVal)
            strHtml += "</td>";
        });
        strHtml += "</tr>";
    });
    return strHtml;
}
function GenrateRowForTh(rows, columns) {
    var strHtml = "";
    $.each(rows, function (rowIndex, rowValue) {
        strHtml += "<tr>"
        $.each(columns, function (colIndex, colValue) {
            strHtml += "<th>";
            strHtml += GenrateInputType(colValue.Type, colValue.Name, colValue.Value, colValue.ClassName, colValue.Id, colValue.fnName)
            strHtml += "</th>";
        });
        strHtml += "</tr>"
    });
    return strHtml;
}
function GenrateInputType(Type, Name, Value, ClassName, Id, fnName, AttrVal) {
    var strHtml;
    switch (Type.toLowerCase()) {

        //case "decimalvalue":

        //    strHtml = "<input     onkeypress = 'return isDecimalKey(event)'  type='TextBox' class='" + ClassName + "' name='" + Name + "' id='" + Id + "' value='" + Value + "' />";
        //    break;


        case "textbox":
            if (AttrVal == undefined) {
                strHtml = "<input  type='TextBox' class='" + ClassName + "' name='" + Name + "' id='" + Id + "' value='" + Value + "' onchange='" + fnName + "' />";
            }
            else {
                strHtml = "<input  " + AttrVal + "  type='TextBox' class='" + ClassName + "' name='" + Name + "' id='" + Id + "' value='" + Value + "' onchange='" + fnName + "'  />";
            }
            break;
        case "select":
            if (AttrVal == undefined) {
                strHtml = "<select class='" + ClassName + "' name='" + Name + "' id='" + Id + "' value='" + Value + "' onchange='" + fnName + "'>";
                strHtml += "<option value='0'>--Select--</option>";
                strHtml += "</select>";
            } else {
                strHtml = "<select " + AttrVal + " class='" + ClassName + "' name='" + Name + "' id='" + Id + "' value='" + Value + "' onchange='" + fnName + "'>";
                strHtml += "<option value='0'>--Select--</option>";
                strHtml += "</select>";
            }
            break;
        case "textarea":
            strHtml = "<textarea class='" + ClassName + "' id='" + Id + "' rows='4' cols='50'></textarea>";
            break;
        case "button":
            strHtml = "<input id='" + Id + "' class='" + ClassName + "' onclick='" + fnName + "' type='button' value='" + Value + "'>";
            break;
        case "anchor":
            strHtml = "<a href='javascript:void(0)' class='" + ClassName + "' id='" + Id + "' onclick='" + fnName + "' >" + Value + "</a>";
            break;
        case "label":
            if (AttrVal == undefined) {
                strHtml = "<label class='" + ClassName + "' id='" + Id + "' name='" + Name + "' >" + Value + "</label>";
            }
            else {
                strHtml = "<label " + AttrVal + " class='" + ClassName + "' id='" + Id + "' name='" + Name + "' >" + Value + "</label>";
            }
            break;
        case "hidden":
            strHtml = " <input  type = 'hidden' class='" + ClassName + "' name = '" + Name + "' id = '" + Id + "' value = '" + Value + "' />";
            break;
        case "file":
            strHtml = " <input  type = 'file' class='" + ClassName + "' name = '" + Name + "' id = '" + Id + "' onchange='" + fnName + "' value = '" + Value + "' />";
            break;
        case "img":
            strHtml = " <img  class='" + ClassName + "' name = '" + Name + "' id = '" + Id + "' value = '" + Value + "' />";
            break;
        case "checkbox":
            if (AttrVal == undefined) {
                strHtml = "<input type='checkbox' class='" + ClassName + "'  name = '" + Name + "' id = '" + Id + "' value='" + Value + "'>"
            } else {
                strHtml = "<input type='checkbox' " + AttrVal + " class='" + ClassName + "'  name = '" + Name + "' id = '" + Id + "' value='" + Value + "'>"
            }
            break;
        case "number":
            if (AttrVal == undefined) {
                strHtml = "<input type='number' class='" + ClassName + "'  name = '" + Name + "' id = '" + Id + "' value='" + Value + "'>"
            } else {
                strHtml = "<input type='number' " + AttrVal + " class='" + ClassName + "'  name = '" + Name + "' id = '" + Id + "' value='" + Value + "'>"
            }
            break;
        default:
            strHtml = false;

    }
    return strHtml;
}

function GetTableJsonList(table) {
    var ArrayData = [];
    var DataObject = {};
    $.each(table[0].rows, function (rowIndex, rowValue) {
        DataObject = {};
        $.each(rowValue.cells, function (colIndex, colValue) {
            if (colValue.firstElementChild.localName == 'label') {
                if (colValue.firstElementChild.getAttribute("name") != "") {
                    DataObject[colValue.firstElementChild.getAttribute("name")] = colValue.firstElementChild.textContent;
                }
            } else {
                if (colValue.firstElementChild.type.toLowerCase() == "checkbox") {
                    DataObject[colValue.firstElementChild.name] = colValue.firstElementChild.checked;
                } else
                    if (colValue.firstElementChild.type.toLowerCase() != "button") {
                        DataObject[colValue.firstElementChild.name] = colValue.firstElementChild.value;
                    }
            }
        });
        ArrayData.push(DataObject);
    });
    return ArrayData;
}
//function TranslateEnglishToHindi() {
//    google.load("elements", "1", {
//        packages: "transliteration"
//    });
//    google.setOnLoadCallback(onLoad);
//}

//function onLoad() {

//    var options = {
//        sourceLanguage: google.elements.transliteration.LanguageCode.ENGLISH,
//        destinationLanguage: google.elements.transliteration.LanguageCode.HINDI, // available option English, Bengali, Marathi, Malayalam etc.
//        shortcutKey: 'ctrl+g',
//        transliterationEnabled: true
//    };

//    var control = new google.elements.transliteration.TransliterationControl(options);
//    control.makeTransliteratable(['txtHindiContent']);
//    var control = new google.elements.transliteration.TransliterationControl(options);
//    control.makeTransliteratable(['txtHindiContent']);
//}

$(document).ready(function () {
    $(".edit").removeAttr("data-toggle");
    $(".Details").removeAttr("data-toggle");

    $(".addnewitem").click(function (e) {        
        e.preventDefault();
        localStorage.setItem("currentUrl", $(this).attr('href'));
        document.getElementById("page-wrapper").innerHTML = "";
        $('#page-wrapper').load($(this).attr('href'));
    });
    //BindGrid();
    //$(".selected-ar > a").click(function (e) {        
    //    e.preventDefault();           
    //        document.getElementById("page-wrapper").innerHTML = "";
    //        $('#page-wrapper').load($(this).attr('href'));
    //        $(".top-head").text($(this).text());

    //});    
});


function CommonAlert(Title, Message, fn, data, type, msgType) {    
    //var data = JSON.stringify(data);
    switch (type.toLowerCase()) {
        case 'alert':
            AlertMessage(Title, Message, fn, data, msgType);
            break;
        case 'success':
            SuccessAlert(Title, Message, fn, data)
            break;
        case 'warning':
            WarningAlert(Title, Message, fn, data)
            break;
        case 'delete':
            DeleteAlert(Title, Message, fn, data)
            break;
        case 'error':
            WarningAlert(Title, Message, fn, data)
            break;
        default:
            break;
    }
}
function AlertMessage(Title, Message, fn, data, type) {
    Title = Title == "warning" ? "Warning" : Title;
    var strHtml = "";
    strHtml += "<div id='CreateModal' class='modal' style='display:block!important'>";
    strHtml += "<div class='modal-dialog'>";
    strHtml += "<div class='modal-content' style='MARGIN-TOP: 71PX'>";
    strHtml += "<div class='modal-header' style='background-color: #41ab34'>";
    Title = Title == "warning" ? "Warning" : Title;
    strHtml += "<h4 class='modal-title' style='font-weight:bold;color: white'>" + Title + "</h4>";
    strHtml += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true' onclick='divhideSuccess();'>×</button>";
    strHtml += "</div>";
    strHtml += "<div class='modal-body'>";
    
    if (type.toLowerCase() == "create") {
        strHtml += "<center><img style='    width: 60px;' src='images/Success-Round-Tick.png' alt=''></center>";
        strHtml += "<p style='font-weight:bold;padding-top: 12px;color: black;text-align: center;'><mediam>" + Message + " save successfully!</mediam></p>";
    } else if (type.toLowerCase() == "delete") {
        strHtml += "<center><img style='    width: 60px;' src='images/warning.png' alt=''></center>";
        strHtml += "<p style='font-weight:bold;padding-top: 12px;color: black;text-align: center;'><mediam>" + Message + " deleted successfully!</mediam></p>";
    } else if (type.toLowerCase() == "update") {
        strHtml += "<center><img style='    width: 60px;' src='images/Success-Round-Tick.png' alt=''></center>";
        strHtml += "<p style='font-weight:bold;padding-top: 12px;color: black;text-align: center;'><mediam>" + Message + " updated successfully!</mediam></p>";
    } else
        if (type.toLowerCase() == "confirmation") {
            strHtml += "<center><img style='    width: 60px;' src='images/warning.png' alt=''></center>";
            strHtml += "<p style='font-weight:bold;padding-top: 12px;color: black;text-align: center;'><mediam>" + Message + "</mediam></p>";
    } else {
        strHtml += "<p style='font-weight:bold;padding-top: 12px;color: black;text-align: center;'><mediam>" + Message + "</mediam></p>";
    }
    strHtml += "</div>";
    strHtml += "<div class='modal-footer'>";
    //strHtml += "<input type='button' class='btn btn-primary m-btn' onclick='PopupOk(" + fn + ",\"" + data + "\");' data-dismiss='modal' value='OK'>";
    strHtml += "<input type='button' class='btn btn-primary m-btn' onclick='PopupOk(" + fn + "," + JSON.stringify(data) + ");' data-dismiss='modal' value='OK'>";
    if (type.toLowerCase() == "confirmation") {
        strHtml += "<input type='button' class='btn btn-danger m-btn' onclick='PopupCancel();' data-dismiss='modal' value='Cancel'>";
    }
    strHtml += "</div></div></div></div>";
    $("#commonModalbinder").append(strHtml)
    $("#commonModalbinder").css("display", "block");
}
function SuccessAlert(Title, Message, fn, data) {
    Title = Title == "warning" ? "Warning" : Title;
    var strHtml = "";
    strHtml += "<div id='ErrorModal' class='modal' style='display:block'>";
    strHtml += "<div class='modal-dialog'>";
    strHtml += "<div class='modal-content' style='MARGIN-TOP: 71PX'>";
    strHtml += "<div class='modal-header' style='background-color: #41ab34'>";    
    strHtml += "<h4 class='modal-title' style='font-weight:bold;color: white'>" + Title + "</h4>";
    strHtml += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true' onclick='divhidError();'>×</button>";
    strHtml += "</div>";
    strHtml += "<div class='modal-body'>";
    strHtml += "<center><img style='    width: 60px;' src='images/Success-Round-Tick.png' alt=''></center>";
    strHtml += "<p style='font-weight:bold; text-align:center'><mediam>" + Message + "</mediam></p>";
    strHtml += "</div>";
    strHtml += "<div class='modal-footer'>";
    //strHtml += "<input type='button' class='btn btn-primary m-btn' onclick='PopupOk(" + fn + ",\"" + data + "\");' data-dismiss='modal' value='OK'>";
    strHtml += "<input type='button' class='btn btn-primary m-btn' onclick='PopupCancel();' data-dismiss='modal' value='Ok'>";
    strHtml += "</div></div></div></div>";
    $("#commonModalbinder").append(strHtml)
    $("#commonModalbinder").css("display", "block");
}
function WarningAlert(Title, Message, fn, data) {
    Title = Title == "warning" ? "Warning" : Title;
    var strHtml = "";
    strHtml += "<div id='ErrorModal' class='modal' style='display:block'>";
    strHtml += "<div class='modal-dialog'>";
    strHtml += "<div class='modal-content' style='MARGIN-TOP: 71PX'>";
    strHtml += "<div class='modal-header' style='background-color: #e24c4b'>";
    strHtml += "<h4 class='modal-title' style='font-weight:bold;color: white'>" + Title + "</h4>";
    strHtml += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true' onclick='divhidError();'>×</button>";
    strHtml += "</div>";
    strHtml += "<div class='modal-body'>";
    strHtml += "<center><img style='    width: 60px;' src='images/warning.png' alt=''></center>";
    strHtml += "<p style='font-weight:bold; text-align:center'><mediam>" + Message + "</mediam></p>";
    strHtml += "</div>";
    strHtml += "<div class='modal-footer'>";
    //strHtml += "<input type='button' class='btn btn-primary m-btn' onclick='PopupOk(" + fn + ",\"" + data + "\");' data-dismiss='modal' value='OK'>";
    strHtml += "<input type='button' class='btn btn-primary m-btn' onclick='PopupCancel();' data-dismiss='modal' value='Ok'>";
    strHtml += "</div></div></div></div>";
    $("#commonModalbinder").append(strHtml)
    $("#commonModalbinder").css("display", "block");
}
function DeleteAlert(Title, Message, fn, data) {
    Title = Title == "warning" ? "Warning" : Title;
    var strHtml = "";
    strHtml += "<div id='ErrorModal' class='modal' style='display:block'>";
    strHtml += "<div class='modal-dialog'>";
    strHtml += "<div class='modal-content' style='MARGIN-TOP: 71PX'>";
    strHtml += "<div class='modal-header'>";
    strHtml += "<h4 class='modal-title' style='font-weight:bold;color: black'>" + Title + "</h4>";
    strHtml += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true' onclick='divhidError();'>×</button>";
    strHtml += "</div>";
    strHtml += "<div class='modal-body'>";
    strHtml += "<center><img style='width: 60px;' src='images/warning.png' alt=''></center>";
    strHtml += "<p style='font-weight:bold;color: red;text-align: center;'><mediam>" + Message + "</mediam></p>";
    strHtml += "</div>";
    strHtml += "<div class='modal-footer'>";
    //strHtml += "<input type='button' class='btn btn-primary m-btn' onclick='PopupOk(" + fn + ",\"" + data + "\");' data-dismiss='modal' value='OK'>";
    strHtml += "<input type='button' class='btn btn-primary m-btn' onclick='PopupOk(" + fn + "," + JSON.stringify(data) + ");' data-dismiss='modal' value='OK'>";
    strHtml += "<input type='button' class='btn btn-primary m-btn' onclick='PopupCancel();' data-dismiss='modal' value='Cancel'>";
    strHtml += "</div></div></div></div>";
    $("#commonModalbinder").append(strHtml)
    $("#commonModalbinder").css("display", "block");
}
function PopupOk(fn, data) {
    CallBack(fn, data);
}
function PopupCancel(path) {
    $('#commonModalbinder').css("display", "none");
    $('#commonModalbinder').html("");
    $(".modalLoader").hide();
}
function divhideSuccess() {
    $('#commonModalbinder').css("display", "none");
    $('#commonModalbinder').html("");
    $(".modalLoader").hide();
}
function divhideError() {
    document.getElementById("ErrorModal").style.display = "none";
}

function CommonUpdateAlert(Title, Message, bgColor, img) {


    $("#ErrorModal .modal-title").text(Title);
    $("#ErrorModal .modal-body p").text(Message);
    $("#ErrorModal").css("display", "block");
    $("#ErrorModal .modal-header").css("background-color", bgColor);
    $("#ErrorModal .modal-body img").attr("src", img);
    //$(".modal-footer").children().attr("onclick", fn)
    //var strHtml = "";
    //strHtml += "<div id='ErrorModal' class='modal' style='display:block'>";
    //strHtml += "<div class='modal-dialog'>";
    //strHtml += "<div class='modal-content' style='MARGIN-TOP: 71PX'>";
    //strHtml += "<div class='modal-header' style='background-color: #e24c4b'>";
    //strHtml += "<h4 class='modal-title' style='font-weight:bold;color: white'>" + Title + "</h4>";
    //strHtml += "<button type='button' class='close' data-dismiss='modal' aria-hidden='true' onclick='divhidError();'>×</button>";
    //strHtml += "</div>";
    //strHtml += "<div class='modal-body'>";
    //strHtml += "<center><img style='    width: 60px;' src='~/images/warning.png' alt=''></center>";
    //strHtml += "<p style='font-weight:bold; text-align:center'><mediam>" + Massage +"</mediam></p>";
    //strHtml += "</div>";
    //strHtml += "<div class='modal-footer'>";
    //strHtml += "<input type='button' class='btn btn-primary m-btn' onclick='divhideError();' data-dismiss='modal' value='OK'>";
    //strHtml += "</div></div></div></div>";
    //return 
}

function BootStrapCalender(Id) {    
    var date_input = $(Id); //our date input has the name "date"
    var container = $('.bootstrap-iso form').length > 0 ? $('.bootstrap-iso form').parent() : "body";
    var options = {
        format: 'dd/mm/yyyy',
        container: container,
        todayHighlight: true,
        autoclose: true,
    };
    date_input.datepicker(options);
}
function BootStrapCalenderMMDDYY(Id) {
    var date_input = $(Id); //our date input has the name "date"
    var container = $('.bootstrap-iso form').length > 0 ? $('.bootstrap-iso form').parent() : "body";
    var options = {
        format: 'mm/dd/yyyy',
        container: container,
        todayHighlight: true,
        autoclose: true,
    };
    date_input.datepicker(options);
}
function DeleteRowFunction(btndel) {
    if (typeof (btndel) == "object") {
        $(btndel).closest("tr").remove();
    } else {
        return false;
    }
}
function FormTextbox(Type, Name, Value, ClassName, Id, fnName, FormClass, AttrVal) {
    var strHtml = "";
    strHtml += "<div class='" + FormClass + "'>";
    strHtml += "<div class='form-group'>";
    strHtml += GenrateInputType(Type, Name, Value, ClassName, Id, fnName, AttrVal);
    strHtml += "</div>";
    strHtml += "</div>";

    return strHtml;
}

function FormLabel(Type, Name, Value, ClassName, Id, fnName, FormClass, AttrVal) {
    var strHtml = "";
    strHtml += "<div class='" + FormClass + " text-right'>";
    strHtml += "<div class='form-group'>";
    strHtml += GenrateInputType(Type, Name, Value, ClassName, Id, fnName, AttrVal);//"<label id='lbl_"++"' class='lebel2'>" + labeltxt + "</label>";
    strHtml += "</div>";
    strHtml += "</div>";
    return strHtml;
}
function GenrateFrom(columns) {
    var strHtml = "";
    $.each(columns, function (colIndex, colValue) {
        if (colValue.Type.toLowerCase() == "label") {
            strHtml += FormLabel(colValue.Type, colValue.Name, colValue.Value, colValue.ClassName, colValue.Id, colValue.fnName, colValue.FormClass, colValue.AttrVal)
        }
        if (colValue.Type.toLowerCase() == "textbox" || colValue.Type.toLowerCase() == "textarea" || colValue.Type.toLowerCase() == "button" || colValue.Type.toLowerCase() == "select" || colValue.Type.toLowerCase() == "hidden" || colValue.Type.toLowerCase() == "file" || colValue.Type.toLowerCase() == "img" || colValue.Type.toLowerCase() == "checkbox" || colValue.Type.toLowerCase() == "number") {
            strHtml += FormTextbox(colValue.Type, colValue.Name, colValue.Value, colValue.ClassName, colValue.Id, colValue.fnName, colValue.FormClass, colValue.AttrVal)
        }
    });
    return strHtml;
}

function CustomConfrimBox(Title, Content, fn, data) {
    $.confirm({
        title: Title,
        content: Content,
        buttons: {
            confirm: function () {
                CallBack(fn, data)
            },
            cancel: function () {
                return true;
            }
            //,
            //somethingElse: {
            //    text: 'Something else',
            //    btnClass: 'btn-blue',
            //    keys: ['enter', 'shift'],
            //    action: function () {
            //        $.alert('Something else?');
            //    }
            //}
        }
    });
}

function Dateformate(selectedDate) {    
    var FormattedDate = "";
    if (selectedDate == undefined) {
        FormattedDate = "";
    }
    else if (selectedDate == "") {
        FormattedDate = "";
    }
    else {
        FormattedDate = selectedDate.split("/")[1] + '/' + selectedDate.split("/")[0] + '/' + selectedDate.split("/")[2];
    }

    return FormattedDate
}
function toValidDate(datestring) {
    return datestring.replace(/(\d{2})(\/)(\d{2})/, "$3$2$1");
}
function DateformateFromDate(selectedDate) {
    var FormattedDate = "";
    if (selectedDate != null) {
        var ddmmyyyy = new Date(selectedDate);
        FormattedDate = (ddmmyyyy.getDate().toString().length == 1 ? "0" + ddmmyyyy.getDate() : ddmmyyyy.getDate()) + '/' + (ddmmyyyy.getMonth().toString().length == 1 ? "0" + ddmmyyyy.getMonth() : ddmmyyyy.getMonth()) + '/' + ddmmyyyy.getFullYear();
    }

    return FormattedDate
}

function GetIndex(thisid) {
    var interval = null;
    $(".modalLoader").show();
    localStorage.setItem("currentUrl", thisid);
    document.getElementById("page-wrapper").innerHTML = "";
    $('#page-wrapper').load(thisid);   
    interval = setInterval(function () {      
        if ($(".plcholder-cn").html() != undefined) {
            $(".modalLoader").hide();
            clearInterval(interval);
        }        
    }, 1000);
    return false;
    //alert(window.location.origin + thisid);
}
function singleScript() {

    scriptLoader("js/canvasjs.min.js")
    scriptLoader("js/datatables.min.js");
    scriptLoader("js/datatables/jquery.dataTables.min.js");
    scriptLoader("js/datatables/dataTables.buttons.min.js");
    scriptLoader("js/datatables/jszip.min.js");
    scriptLoader("js/datatables/pdfmake.min.js");
    scriptLoader("js/datatables/vfs_fonts.js");
    scriptLoader("js/datatables/buttons.html5.min.js");
    scriptLoader("js/datatables/buttons.colVis.min.js");
    scriptLoader("js/jquery-confirm.min.js");
}
function scriptLoader(path, callback) {
    var script = document.createElement('script');
    script.type = "text/javascript";
    script.async = true;
    script.src = path;
    script.onload = function () {
        if (typeof (callback) == "function") {
            callback();
        }
    }
    try {
        var scriptOne = document.getElementsByTagName('script')[0];
        scriptOne.parentNode.insertBefore(script, scriptOne);
    }
    catch (e) {
        document.getElementsByTagName("head")[0].appendChild(script);
    }
}
var url;

$("input[type='submit']").click(function (e) {    
    if ($(this).hasClass("report")) {
        
        var valid = this.form.checkValidity();
        if (valid) {
            if (this.form.hasAttribute("onsubmit")) {
                valid = false;
                valid = CallBack(this.form.onsubmit)
            }
            if (valid) {
                $(".modalLoader").css("display", "block");
                e.preventDefault();
                var $form = $("input[type='submit']").parents('form');

                $.ajax({
                    type: "POST",
                    url: $form.attr('action'),
                    data: $form.serialize(),
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
                        //////////////////////      Commented in order to fix the report URL Issue  //////////////////////////////////

                        //var x = window.open('','_blank');
                        //x.document.write('<body></body>');
                        //var style = x.document.createElement('style');
                        //style.innerHTML = ".modalLoader { display:block; position:fixed; z-index:9999999; top:0; left:0; height:100%; width:100%; background:rgba(18, 92, 214, 0.25) url('../images/tenor.gif') 50% 50% no-repeat;} body.loading{overflow:hidden;}";
                        //x.document.head.appendChild(style);
                        //var div = x.document.createElement('div');
                        //div.className = "modalLoader";
                        //div.id = "modalLoaderId"
                        //x.document.body.appendChild(div);
                        //var iframe = x.document.createElement('iframe');
                        //iframe.id = 'reportFrame';
                        //iframe.src = response.report;
                        //iframe.style = "width:100%; height:100%;"
                        //x.document.body.appendChild(iframe);
                        //var script = x.document.createElement('script');
                        //script.type = "text/javascript";
                        //script.innerHTML = "document.getElementById('reportFrame').onload = function(){document.getElementById('modalLoaderId').style.display='none';}";
                        //x.document.head.appendChild(script);
                        //x.document.title = response.selectedMenu;

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
                });
                return false;
            }
        }
    } else {
        
        var valid = this.form.checkValidity();
        if (valid) {
            if (this.form.hasAttribute("onsubmit")) {
                valid = false;
                valid = CallBack(this.form.onsubmit)
            }
            if (valid) {
                e.preventDefault();
                $(".modalLoader").css("display", "block");

                //var $form = $("input[type='submit']").parents('form');
                var $form = $(this).parents("form");
                
                $.ajax({
                    type: "POST",
                    url: $form.attr('action'),
                    data: $form.serialize(),
                    error: function (xhr, response, error) {                        
                       CommonAlert("Error", response, null, null, "error");
                    },
                    success: function (response) {
                        //do something with response                         
                        var contentId;
                        if (response.selectedAction == null) {
                             contentId = "/" + response.areaName + "/" + response.selectedMenu + "/Index";
                        }
                        else {
                             contentId = "/" + response.areaName + "/" + response.selectedMenu + "/" + response.selectedAction;
                        }
                        //CommonAlert(response.alert, response.message, window.location.origin + contentId);
                        url = window.location.origin + contentId;
                        if (response.isAlertBox) {
                            if (response.errorMessage != "") {
                                CommonAlert(response.alert, response.message, SubmitPopup, url, "error");
                            } else {
                                CommonAlert(response.alert, response.message, SubmitPopup, url, "alert", "create");
                            }
                        } else {
                            url = window.location.origin + contentId;
                            $(".modalLoader").hide();
                            GetIndex(url);
                        }
                        

                    }
                });

                return false;
            }
        }
    }
});
function CommonSubmit(data, actionUrl, e) {
    
    e.preventDefault();

    $(".modalLoader").css("display", "block");
    var $form = $(this).parents("form");
    $.ajax({
        type: "POST",
        url: actionUrl,
        data: data,
        //processData: false,//Add for uploading content like pdf
        //contentType: false,//Add for uploading content like pdf
        error: function (xhr, response, error) {            
            CommonAlert("Error", response, null, null, "error");
        },
        success: function (response) {            
            var url = window.location.origin + "/" + response.areaName + "/" + response.selectedMenu + "/" + response.selectedAction;

            if (response.isAlertBox) {
                if (response.errorMessage != "") {
                    CommonAlert(response.alert, response.message, SubmitPopup, url, "error");
                } else {
                    CommonAlert(response.alert, response.message, SubmitPopup, url, "alert", "create");
                }
            } else {                
                $(".modalLoader").hide();
                GetIndex(url);
            }

        }
    });
}

function CommonSubmitUpload(data, actionUrl, e) {    
    e.preventDefault();

    $(".modalLoader").css("display", "block");
    var $form = $(this).parents("form");
    $.ajax({
        type: "POST",
        url: actionUrl,
        data: data,
        processData: false,//Add for uploading content like pdf
        contentType: false,//Add for uploading content like pdf
        error: function (xhr, response, error) {
            CommonAlert("Error", response, null, null, "error");
        },
        success: function (response) {
            var url = window.location.origin + "/" + response.areaName + "/" + response.selectedMenu + "/" + response.selectedAction;

            if (response.isAlertBox) {
                if (response.errorMessage != "") {
                    CommonAlert(response.alert, response.message, SubmitPopup, url, "error");
                } else {
                    CommonAlert(response.alert, response.message, SubmitPopup, url, "alert", "create");
                }
            } else {
                $(".modalLoader").hide();
                GetIndex(url);
            }

        }
    });
}
function SubmitPopup(data) {
    $("#page-wrapper").load(data);
    $(".modalLoader").hide();
}

function hideImage() {$(".modalLoader").hide();}



//function divShow(id) {
//    document.getElementById("hdnValue").value = id;
//    document.getElementById("deleteModal").style.display = "block";
//}
function tryParseFloat(a) {
    a = a || 0;
    return parseFloat(a);

    //if (isNaN(a)) {
    //    return 0;
    //}
    //return parseFloat(a);
}

function ResponseSave(response) {
    
    var contentId = "/" + response.areaName + "/" + response.selectedMenu + "/Index";
    url = window.location.origin + contentId;
    $("#commonModalbinder").css("display", "none");
    if (response.errorMessage != "") {
        CommonAlert(response.alert, response.errorMessage, AfterSuccess, url, "error");
    } else {
        CommonAlert(response.alert, response.message, AfterSuccess, url, "alert", "create");
    }
}

function DeleteConfirm(Data) {
    
    $(".modalLoader").css("display", "block");
    CommonAjax(Data.Url, "Get", false, "application/json", false, DeleteSuccess);
}
function DeleteSuccess(response) {
    
    var contentId;
    if (response.selectedAction == null) {
        contentId = "/" + response.areaName + "/" + response.selectedMenu + "/Index";
    }
    else {
        contentId = "/" + response.areaName + "/" + response.selectedMenu + "/" + response.selectedAction;
    }
     
    url = window.location.origin + contentId;
    $("#commonModalbinder").css("display", "none");    
    if (response.errorMessage != "") {
        CommonAlert(response.alert, response.errorMessage, AfterSuccess, url, "error");
    } else {
        CommonAlert(response.alert, response.message, AfterSuccess, url, "alert", "delete");
    }
}
function AfterSuccess(url) {
    document.getElementById("page-wrapper").innerHTML = "";
    $("#page-wrapper").load(url);
    $(".modalLoader").hide();

}
function isNumeric(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
function isDecimal(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
        && (charCode < 48 || charCode > 57))
        return false;

    return true;
}

function divShow(url) {
    
    CommonAlert('Delete', 'Do you want to delete?', DeleteConfirm, { 'Url': url }, 'delete');
}
function SetDateInHidden(thisVal, hiddenObj) {
    debugger;
    hiddenObj.val(Dateformate(thisVal.value));
}
//var hdnPlaceCode = document.getElementById('hdnValue').value
    //document.getElementById("deleteModal").style.display = "none";
    //var data = Data+"?id="
//window.location.href = "/ESTABLISHMENT/EMSC27/DeleteConfirmed?id=" + hdnPlaceCode;
function OpenPdf(url) {
    
    $(".modalLoader").show();
    var x = window.open('_blank');
    x.document.write('<body></body>');
    var style = x.document.createElement('style');
    style.innerHTML = ".modalLoader { display:block; position:fixed; z-index:9999999; top:0; left:0; height:100%; width:100%; background:rgba(18, 92, 214, 0.25) url('../images/tenor.gif') 50% 50% no-repeat;} body.loading{overflow:hidden;}";
    x.document.head.appendChild(style);
    var div = x.document.createElement('div');
    div.className = "modalLoader";
    div.id = "modalLoaderId"
    x.document.body.appendChild(div);
    var iframe = x.document.createElement('iframe');
    iframe.id = 'reportFrame';
    iframe.src = url;
    iframe.style = "width:100%; height:100%;"
    x.document.body.appendChild(iframe);
    var script = x.document.createElement('script');
    script.type = "text/javascript";
    script.innerHTML = "document.getElementById('reportFrame').onload = function(){document.getElementById('modalLoaderId').style.display='none';}";
    x.document.head.appendChild(script);   
    $(".modalLoader").hide();
}