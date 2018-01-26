
function getResultRetResult(val) {
    spinnerStop();
    var resultRet = document.getElementById("ctl00_resultRetrieve");
    $(document.getElementById("allResult")).text(val);
    var results = val.split("\\|//");
    if (results.length == 1) {
        $(resultRet).text(val.split("\\|)//")[0]);
    }
    else {
        fillResultList(results);
    }
}

function fillResultList(results) {

    if(navigator.appName == "Netscape")
    {
        for (var i = 0; i < results.length; i++) {
            var newspan = document.createElement("div");
            newspan.setAttribute("id", i + "");
            var list = results[i].split("\\|)//");

            $(newspan).text(list[0]);
            var brek = document.createElement("p");
            brek.appendChild(newspan);
            brek.setAttribute("style", "margin:5px 10px 0px 10px;");
            newspan.setAttribute("onmouseover", "mousO(this)");
            newspan.setAttribute("onmouseout", "mouseOu(this)");
            newspan.setAttribute("onclick", "resultChoose(this, " + i + ")");
            document.getElementById("ctl00_resultRetrieve").appendChild(brek);
        }
    }
    else
    {

            var select = document.getElementById("ctl00_resultRetrieve");
            select.setAttribute("onclick", "resultChooseForIE(this)");
        
            for (var i = 0; i < results.length; i++) {
                var elem = results[i].split("\\|)//");
                var brek = document.createElement("option");
                brek.setAttribute("value", i);
                brek.setAttribute("id", i + "");
                brek.setAttribute("style", "margin:5px 10px 0px 10px;");
                $(brek).text(elem[0]);

                select.appendChild(brek);
            }
    }
}

function resultChooseForIE(select) {
    
    var index = select.options[select.selectedIndex].value;
    var val = select.options[select.selectedIndex];
//for (i = 0; i < document.getElementById("ctl00_resultRetrieve").childNodes.length; i++) {
//var figlio = document.getElementById("ctl00_resultRetrieve").childNodes[i];
//figlio.firstChild.setAttribute("style", "border: none;");
//}
//$(val).css({ color: "white", backgroundColor: "#828AD0" });


    var desc = ((($(document.getElementById("allResult")).text()).split("\\|//"))[index]).split("\\|)//");
    var spanDesc = document.getElementById("ctl00_resultDescription");
    spanDesc.value = desc[1];
    enableButton("exportResultButton", "ExportCall(" + val.id + ",'')");
}

function resultChoose(val, index) {
    for (i = 0; i < document.getElementById("ctl00_resultRetrieve").childNodes.length; i++) {
        var figlio = document.getElementById("ctl00_resultRetrieve").childNodes[i];
        figlio.firstChild.setAttribute("style", "border: none;");
    }
    $(val).css({ color: "white", backgroundColor: "#828AD0" });
    var desc = ((($(document.getElementById("allResult")).text()).split("\\|//"))[index]).split("\\|)//");
    var spanDesc = document.getElementById("ctl00_resultDescription");
    spanDesc.value = desc[1];
    enableButton("exportResultButton", "ExportCall(" + val.id + ",'')");
}

function ExportResult(val) {
    if (val == "no") {
        alert("Error con creating xml, contact an administrator");
    } else {
        var newWindow = window.open("../FormFillier/resultXml.aspx", '_blank');
        newWindow.focus();
    }
}


function searchResult(str) {
    var all = ($(document.getElementById("allResult")).text()).split("\\|//");
    var results = new Array();
    var j = 0;
    str2 = str.toLowerCase();
    for (i = 0; i < all.length; i++) {
        if ((all[i].split("\\|)//")[0].toLowerCase()).match(str2) != null) {
            results[j] = all[i];
            j++;
        }
    }
    if (results.length == 0) {
        $(document.getElementById("ctl00_resultRetrieve")).text("No results for " + str);
    } else {
        cleanDialog(document.getElementById("ctl00_resultRetrieve"));
        cleanDialog(document.getElementById("ctl00_resultDescription"));
        fillResultList(results);
    }
}