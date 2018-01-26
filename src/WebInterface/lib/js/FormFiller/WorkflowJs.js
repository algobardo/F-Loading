/////////////////////////////////////////////////////////
//////////////Publications - models methods//////////////
/////////////////////////////////////////////////////////

var titleUserWorkflow = "Choose a publication, a model or create a new one";
var titleSelectContact = "Select the contacts to send mail";

function getWorkFlowListResult(val) {
    spinnerStop();
    var workflowList = document.getElementById("ctl00_workflowRetrieved");
    var modelList = document.getElementById("ctl00_modelRetrieved");
    if (val == null) {
        //non dovrebbe mai succedere
        workflowList.innerHTML = "Error on server";
        modelList.innerHTML = "Error on server";
    }
    else {
        var list = val.split("\\||//");
        workflowList.innerHTML = "";
        modelList.innerHTML = "";

        $(document.getElementById("allWorkflow")).text(list[0]);
        $(document.getElementById("allModel")).text(list[1]);

        if (list.length == 1) {
            $(workflowList).text(val.split("\\|)//")[0]);
            $(modelList).text(val.split("\\|)//")[0]);
        } else {
            //publications
            var publication = list[0].split("\\|//");
            if (publication.length == 1) {
                $(workflowList).text(publication[0]);

            } else {
                fillPublicationList(publication)
            }
            //models
            var model = list[1].split("\\|//");
            if (model.length == 1) {
                $(modelList).text(publication[0]);
            } else {
                fillModelList(model);
            }
            document.getElementById("searchWorkflow").focus();
        }
    }
}
function fillPublicationList(publication) {

    if(navigator.appName == "Netscape")
    {
        for (i = 0; i < publication.length; i++) {
            var elem = publication[i].split("\\|)//");
            var newspan = document.createElement("div");
            newspan.setAttribute("id", "publication" + elem[2]);
            $(newspan).text(elem[0]);
            var brek = document.createElement("p");
            brek.appendChild(newspan);
            brek.setAttribute("style", "margin:5px 10px 0px 10px;");
            newspan.setAttribute("onmouseover", "mousO(this)");
            newspan.setAttribute("onmouseout", "mouseOu(this)");

            newspan.setAttribute("onclick", "workflowChoose(this, " + i + ")"); //,"+wfElem[1]+")");
            document.getElementById("ctl00_workflowRetrieved").appendChild(brek);
        }
    }
    else
    {
        var select = document.getElementById("ctl00_workflowRetrieved");
        select.setAttribute("onclick", "workflowChooseForIE(this)");
        
        for (i = 0; i < publication.length; i++) {
            var elem = publication[i].split("\\|)//");
            var newspan = document.createElement("div");
            //newspan.setAttribute("id", "publication" + elem[2]);
            //$(newspan).text(elem[0]);
            var brek = document.createElement("option");
            brek.setAttribute("value", i);
            brek.setAttribute("id", "publication" + elem[2]);
            $(brek).text(elem[0]);
            //brek.appendChild(newspan);
            brek.setAttribute("style", "margin:5px 10px 0px 10px;");
            //newspan.setAttribute("onmouseover", "mousO(this)");
            //newspan.setAttribute("onmouseout", "mouseOu(this)");

            
            select.appendChild(brek);
        }
    }
}
function fillModelList(model) {

    if(navigator.appName == "Netscape")
    {
        for (i = 0; i < model.length; i++) {
            var elem = model[i].split("\\|)//");
            var newspan = document.createElement("div");
            newspan.setAttribute("id", "model" + elem[2]);
            $(newspan).text(elem[0]);
            var brek = document.createElement("p");
            brek.appendChild(newspan);
            brek.setAttribute("style", "margin:5px 10px 0px 10px;");
            newspan.setAttribute("onmouseover", "mousO(this)");
            newspan.setAttribute("onmouseout", "mouseOu(this)");
            newspan.setAttribute("onclick", "modelChoose(this, " + i + ")"); //,"+wfElem[1]+")");
            document.getElementById("ctl00_modelRetrieved").appendChild(brek);
        }
    }
    else
    {
        var select = document.getElementById("ctl00_modelRetrieved");
        select.setAttribute("onclick", "modelChooseForIE(this)");
        
        for (i = 0; i < model.length; i++) {
            var elem = model[i].split("\\|)//");
            //var newspan = document.createElement("div");
            //newspan.setAttribute("id", "model" + elem[2]);
            //$(newspan).text(elem[0]);
            
            var brek = document.createElement("option");
            brek.setAttribute("value", i);
            brek.setAttribute("id", "model" + elem[2]);
            //brek.appendChild(newspan);
            brek.setAttribute("style", "margin:5px 10px 0px 10px;");
            $(brek).text(elem[0]);
            //newspan.setAttribute("onmouseover", "mousO(this)");
            //newspan.setAttribute("onmouseout", "mouseOu(this)");
            //newspan.setAttribute("onclick", "modelChoose(this, " + i + ")"); //,"+wfElem[1]+")");
            select.appendChild(brek);
        }
    }
}

function searchWorkflow(str) {
    var workflowList = document.getElementById("ctl00_workflowRetrieved");
    str2 = str.toLowerCase();
    //search on the publications
    var all = ($(document.getElementById("allWorkflow")).text()).split("\\|//");
    var result = new Array();
    var j = 0;
    for (i = 0; i < all.length; i++) {
        if ((all[i].split("\\|)//")[0].toLowerCase()).match(str2) != null) {
            result[j] = all[i];
            j++;
        }
    }
    if (result.length == 0) {
        $(document.getElementById("ctl00_workflowRetrieved")).text("No results for " + str);
    } else {
        cleanDialog(document.getElementById("ctl00_workflowRetrieved"));
        cleanDialog(document.getElementById("ctl00_workflowRetrievedDescription"));
        fillPublicationList(result);
    }
    //search on the models
    var all = ($(document.getElementById("allModel")).text()).split("\\|//");
    var result = new Array();
    var j = 0;
    for (i = 0; i < all.length; i++) {
        if ((all[i].split("\\|)//")[0].toLowerCase()).match(str2) != null) {
            result[j] = all[i];
            j++;
        }
    }
    if (result.length == 0) {
        $(document.getElementById("ctl00_modelRetrieved")).text("No results for " + str);
    } else {
        cleanDialog(document.getElementById("ctl00_modelRetrieved"));
        cleanDialog(document.getElementById("ctl00_workflowRetrievedDescription"));
        fillModelList(result);
    }
}

function workflowChooseForIE(select)
{
    var index = select.options[select.selectedIndex].value;
    var val = select.options[select.selectedIndex];
//for (i = 0; i < document.getElementById("ctl00_workflowRetrieved").childNodes.length; i++) {
//var figlio = document.getElementById("ctl00_workflowRetrieved").childNodes[i];
//figlio.firstChild.setAttribute("style", "border: none;");
//}

//$(val).css({ color: "white", backgroundColor: "#828AD0" });

    
    var publicationId = val.id.substring(11);
    var spanDesc = document.getElementById("ctl00_workflowRetrievedDescription");

    var list = ($(document.getElementById("allWorkflow")).text()).split("\\|//"); //lista degli elementi
    
    var date = "Expiration date: " + list[index].split("\\|)//")[3];
    var descIndex = ((list[index]).split("\\|)//"))[1];
    if ((descIndex == "") | (descIndex == "...write a description here")) {

        descIndex = "No description present";
    }
    spanDesc.value = date + "\n-----------------------------\n" + descIndex;
    enableButton("dialogCenterButton", "openThemeEditor('" + publicationId + "')");
    enableButton("dialogRightButton", "getUserGCall('" + publicationId + "','');transitionDialog();");
    var name = list[index].split("\\|)//")[0];
    enableButton("deleteWorkflow","removePublication('"+name+"','"+publicationId+"');");
}

function workflowChoose(val, index) {
    for (i = 0; i < document.getElementById("ctl00_workflowRetrieved").childNodes.length; i++) {
        var figlio = document.getElementById("ctl00_workflowRetrieved").childNodes[i];
        figlio.firstChild.setAttribute("style", "border: none;");
    }

    $(val).css({ color: "white", backgroundColor: "#828AD0" });

    var publicationId = val.id.substring(11);
    var spanDesc = document.getElementById("ctl00_workflowRetrievedDescription");

    var list = ($(document.getElementById("allWorkflow")).text()).split("\\|//"); //lista degli elementi
    
    var date = "Expiration date: " + list[index].split("\\|)//")[3];
    var descIndex = ((list[index]).split("\\|)//"))[1];
    if ((descIndex == "") | (descIndex == "...write a description here")) {

        descIndex = "No description present";
    }
    spanDesc.value = date + "\n-----------------------------\n" + descIndex;
    enableButton("dialogCenterButton", "openThemeEditor('" + publicationId + "')");
    enableButton("dialogRightButton", "getUserGCall('" + publicationId + "','');transitionDialog();");
    var name = list[index].split("\\|)//")[0];
    enableButton("deleteWorkflow","removePublication('"+name+"','"+publicationId+"');");
}

function removePublication(name,id) {
    var deleteDialog = document.getElementById("deleteDialog");
    $(deleteDialog).dialog('option','buttons',{ 
        'Yes': function() { removePublicationCall(''+id, null);},
        Cancel: function() { $(deleteDialog).dialog('close');}
    });
    $(deleteDialog).data('title.dialog', 'Remove '+name);
    $(deleteDialog).dialog('open');
}

function removePublicationResult(result) {
    if(result == "OK") {    
        getWorkFlowListCall('', '');
        $(document.getElementById("deleteDialog")).dialog('close');
    }
    else
        alert('Errore');
}

function openThemeEditor(publicationId) {
    openThemeEditorCall(publicationId, '');
}

function openThemeEditorResult(result) {
    if (result == 'OK')
        location.href = "/ThemeEditor/ThemeEditor.aspx";
    else
        alert(result);
}

function modelChooseForIE(select)
{
    
    var index = select.options[select.selectedIndex].value;
    
//for (i = 0; i < document.getElementById("ctl00_modelRetrieved").childNodes.length; i++) {
//var figlio = document.getElementById("ctl00_modelRetrieved").childNodes[i];
//figlio.firstChild.setAttribute("style", "border: none;");
//}

//$(select.options[select.selectedIndex]).css({ color: "white", backgroundColor: "#828AD0" });

    var spanDesc = document.getElementById("ctl00_workflowRetrievedDescription");

    var list = ($(document.getElementById("allModel")).text()).split("\\|//");
    var descIndex = (list[index]).split("\\|)//")[1];
    if ((descIndex == "") | (descIndex == "...write a description here")) {
        descIndex = "No description present";
    }
    spanDesc.value = descIndex;
    var modelId = select.options[select.selectedIndex].id.substring(5);
    enableButton("dialogCenterButton", "openWorkflowEditor(" + modelId + ");");
    enableButton("dialogRightButton", "openPublishDialog(" + modelId + ");");
    var name = (list[index]).split("\\|)//")[0];
    enableButton("deleteWorkflow","removeModel('"+name+"','"+modelId+"');");
}

function modelChoose(val, index) {
    for (i = 0; i < document.getElementById("ctl00_modelRetrieved").childNodes.length; i++) {
        var figlio = document.getElementById("ctl00_modelRetrieved").childNodes[i];
        figlio.firstChild.setAttribute("style", "border: none;");
    }

    $(val).css({ color: "white", backgroundColor: "#828AD0" });

    var spanDesc = document.getElementById("ctl00_workflowRetrievedDescription");

    var list = ($(document.getElementById("allModel")).text()).split("\\|//");
    var descIndex = (list[index]).split("\\|)//")[1];
    if ((descIndex == "") | (descIndex == "...write a description here")) {
        descIndex = "No description present";
    }
    spanDesc.value = descIndex;
    var modelId = val.id.substring(5);
    enableButton("dialogCenterButton", "openWorkflowEditor(" + modelId + ");");
    enableButton("dialogRightButton", "openPublishDialog(" + modelId + ");");
    var name = (list[index]).split("\\|)//")[0];
    enableButton("deleteWorkflow","removeModel('"+name+"','"+modelId+"');");
}

function removeModel(name,id) {
    var deleteDialog = document.getElementById("deleteDialog");    
    $(deleteDialog).dialog('option','buttons',{ 
        'Yes': function() { removeModelCall(''+id, null);},
        Cancel: function() { $(deleteDialog).dialog('close');}
    });
    $(deleteDialog).data('title.dialog', 'Remove '+name);
    $(deleteDialog).dialog('open');    
}

function removeModelResult(result) {
    if(result == "OK") {    
        getWorkFlowListCall('', '');
        $(document.getElementById("deleteDialog")).dialog('close');
    }
    else
        alert('Errore');
}

function openWorkflowEditor(modelId) {
    openWorkflowEditorCall(modelId, '');
}

function openWorkflowEditorResult(result) {
    var status = result.split('&');
    var modelName = status[1];
    if (status[0] == "OK" && modelName != "") {
        WFE_setWFcookie(modelName);
        location.href = "/WorkflowEditor/WorkflowEditor.aspx";
    } else {
        //do somethig....
        alert("Error: " + status[0]);
    }
}

//method to animate transition from list of publication and send to contact
function transitionDialog() {
    var dialogWorkflowList = document.getElementById("dialogWorkflowList");
    var dialogWorkflowContactList = document.getElementById("dialogWorkflowContactList");

    var listWork = document.getElementById("ctl00_workflowRetrieved");
    var labelWork = document.getElementById("ctl00_workflowRetrievedDescription");

    var listGroup = document.getElementById("ctl00_listGroup");
    var listContact = document.getElementById("ctl00_listContact");

    var footer = document.getElementById("dialogWorkflowFooterDiv");

    $(function() {
        $(footer).animate({ opacity: "toggle" }, 1000,
            function() {
                changeButtonText("dialogCenterButton", "Back");
                changeButtonFunction("dialogCenterButton", "backTransitionDialog();");
                changeButtonText("dialogRightButton", "Send request");
                disableButton("dialogRightButton");
                hideButton("deleteWorkflow");
            });
        $(listWork).animate({ opacity: "toggle" }, 1000);
        $(labelWork).animate({ width: "0px" }, 500);
    });

    $(function() {
        $(dialogWorkflowList).animate({ opacity: "toggle" }, 1000,
            function() {
                $(dialogWorkflowContactList).animate({ opacity: "toggle" }, 500,
                    function() {
                        listContact.setAttribute("style", "width:0px");
                        $(listGroup).animate({ opacity: "toggle" }, 1000);
                        $(listContact).animate({ width: "310px" }, 1000);
                        $(footer).animate({ opacity: "toggle" }, 1000);
                    });
            });
    });
    $(document.getElementById("dialogWorkflowDiv")).data('title.dialog', titleSelectContact);
}

//method to animate transition from send to contact to workflow list
function backTransitionDialog() {
    var dialogWorkflowList = document.getElementById("dialogWorkflowList");
    var listWork = document.getElementById("ctl00_workflowRetrieved");
    var labelWork = document.getElementById("ctl00_workflowRetrievedDescription");
    var dialogWorkflowContactList = document.getElementById("dialogWorkflowContactList");
    var listGroup = document.getElementById("ctl00_listGroup");
    var listContact = document.getElementById("ctl00_listContact");
    var footer = document.getElementById("dialogWorkflowFooterDiv");

    cleanDialog(listGroup);
    cleanDialog(listContact);
    cleanSelected(listWork);
    cleanDialog(labelWork);

    $(function() {
        $(footer).animate({ opacity: "toggle" }, 1000,
            function() {
                disableButton("dialogCenterButton");
                changeButtonText("dialogCenterButton", "Edit theme");
                disableButton("dialogRightButton");
                changeButtonText("dialogRightButton", "Send");
                showButton("deleteWorkflow");
                disableButton("deleteWorkflow");
            }
        );
        $(listGroup).animate({ opacity: "toggle" }, 1000);
        $(listContact).animate({ width: "0px" }, 500);
    });
    $(function() {
        $(dialogWorkflowContactList).animate({ opacity: "toggle" }, 1000,
            function() {
                $(dialogWorkflowList).animate(
                    { opacity: "toggle" }, 500,
                    function() {
                        labelWork.setAttribute("style", "width:0px");
                        $(listWork).animate({ opacity: "toggle" }, 1000);
                        $(labelWork).animate({ width: "315px" }, 1000);
                        $(footer).animate({ opacity: "toggle" }, 1000);
                    });
            });
    });
    $(document.getElementById("dialogWorkflowDiv")).data('title.dialog', titleUserWorkflow);
}

//method to choose tab in dialog
function tabClick(selectTab) {
    if (selectTab.getAttribute("class") != "openDialogTab") {
        var div = document.getElementById("handleTab");
        var tabs = div.childNodes;
        for (var i = 0; i < tabs.length; i++) {
            var otherTab = tabs[i];
            if (otherTab.nodeType == "1") {
                if ((otherTab.id == "modelTab") | (otherTab.id == "publicationTab"))
                    otherTab.setAttribute("class", "dialogTab");
            }
        }
        selectTab.setAttribute("class", "openDialogTab");
        switchView();
    }
}

//method to update list
function switchView() {
    var workflowList = document.getElementById("ctl00_workflowRetrieved");
    var modelList = document.getElementById("ctl00_modelRetrieved");
    var labelWork = document.getElementById("ctl00_workflowRetrievedDescription");
    if (workflowList.style.display == "block") {
        workflowList.style.display = "none";
        modelList.style.display = "block";
        disableButton("deleteWorkflow");
        disableButton("dialogCenterButton");
        changeButtonText("dialogCenterButton", "Edit model");
        disableButton("dialogRightButton");
        changeButtonText("dialogRightButton", "Publish");
        cleanSelected(modelList);

    } else {
        workflowList.style.display = "block";
        modelList.style.display = "none";
        disableButton("deleteWorkflow");
        disableButton("dialogCenterButton");
        changeButtonText("dialogCenterButton", "Edit theme");
        disableButton("dialogRightButton");
        changeButtonText("dialogRightButton", "Send");
        cleanSelected(workflowList);
    }
    cleanDialog(labelWork);
}

///////////////////////////////////////////////////
//////////////Public workflow methods//////////////
///////////////////////////////////////////////////

//get public form
function getFormPResult(val) {
    spinnerStop(false);
    var allWf = document.getElementById("allPublicWorkflow");
    $(allWf).text(val);

    var contWdiv = document.getElementById("ctl00_publicWorkflowRetrieved");
    var workflows = val.split("\\||//");    
        if (workflows.length > 1) {
        workflows.splice(workflows.length - 1, 1); //remove last element \\||//
        document.getElementById("searchPublicWorkflow").focus();
    }
    fillPublicWorkflowList(workflows);
    
}

function fillPublicWorkflowList(workflows) {

    if(navigator.appName == "Netscape")
    {
        if (workflows.length == 1 && workflows[0].split("\\|//").length == 1) {
        var newspan = document.createElement("div");
        $(newspan).text(workflows[0]);
        var brek = document.createElement("p");
        brek.setAttribute("style", "margin:5px 10px 0px 10px;");
        brek.appendChild(newspan);
        document.getElementById("ctl00_publicWorkflowRetrieved").appendChild(brek);
    } else {
        for (var i = 0; i < workflows.length; i++) {
            var pair = (workflows[i].split("\\|//"))[0];
            var name = pair.split("\\|)//")[0];
            var link = ((workflows[i].split("\\|//"))[1]);
            var newspan = document.createElement("div");
            newspan.setAttribute("id", i + "");
            $(newspan).text(name);
            var brek = document.createElement("p");

            brek.setAttribute("style", "margin:5px 10px 0px 10px;");
            newspan.setAttribute("title", link);
            newspan.setAttribute("onmouseover", "mousO(this)");
            newspan.setAttribute("onmouseout", "mouseOu(this)");
            newspan.setAttribute("onclick", "publicWorkflowChoose(this, " + i + ")");
            brek.appendChild(newspan);
            document.getElementById("ctl00_publicWorkflowRetrieved").appendChild(brek);
        }
    }
    }
    
    else{
    var select = document.getElementById("ctl00_publicWorkflowRetrieved");
    select.setAttribute("onclick", "publicWorkflowChooseForIE(this)");
    if (workflows.length == 1 && workflows[0].split("\\|//").length == 1) {
        var newspan = document.createElement("div");
        $(newspan).text(workflows[0]);
        var brek = document.createElement("option");
        brek.setAttribute("style", "margin:5px 10px 0px 10px;");
        brek.appendChild(newspan);
        document.getElementById("ctl00_publicWorkflowRetrieved").appendChild(brek);
    } else {
    
        for (var i = 0; i < workflows.length; i++) {
            var pair = (workflows[i].split("\\|//"))[0];
            var name = pair.split("\\|)//")[0];
            var link = ((workflows[i].split("\\|//"))[1]);

            var brek = document.createElement("option");
            brek.setAttribute("style", "margin:5px 10px 0px 10px;");
            brek.setAttribute("value", i);
            $(brek).text(name);
            brek.setAttribute("title", link);

            select.appendChild(brek);
        }
    }
    }
}

function searchPublic(str) {
    var all = ($(document.getElementById("allPublicWorkflow")).text()).split("\\||//");
    var result = new Array();
    var j = 0;
    str2 = str.toLowerCase();
    for (i = 0; i < all.length; i++) {
        if ((all[i].split("\\|)/")[0].toLowerCase()).match(str2) != null) {
            result[j] = all[i];
            j++;
        }
    }
    if (result.length == 0) {
        $(document.getElementById("ctl00_publicWorkflowRetrieved")).text("No results for " + str);
    } else {
        cleanDialog(document.getElementById("ctl00_publicWorkflowRetrieved"));
        cleanDialog(document.getElementById("ctl00_publicWorkflowRetrievedDescription"));
        fillPublicWorkflowList(result);
    }
}

function publicWorkflowChoose(val,index) {
    var list = document.getElementById("ctl00_publicWorkflowRetrieved");
        for (i = 0; i < list.childNodes.length; i++) {
            var figlio = list.childNodes[i];
            figlio.firstChild.setAttribute("style", "border: none;");
        }
        $(val).css({ color: "white", backgroundColor: "#828AD0" });
        var spanDesc = document.getElementById("ctl00_publicWorkflowRetrievedDescription");
        var list = document.getElementById("allPublicWorkflow");
        var desc = ((($(list).text()).split("\\||//")[index]).split("\\|//")[0]).split("\\|)//")[1];
        if ((desc == "") | (desc == "...write a description here")) {
            desc = "No description present";
        }
        spanDesc.value = desc;

        enableButton("publicWFCompileButton", "location.href='" + val.title + "';");
}

function publicWorkflowChooseForIE(select) {

        var index = select.options[select.selectedIndex].value;
        
        var list = document.getElementById("ctl00_publicWorkflowRetrieved");
    
    
        for (i = 0; i < list.childNodes.length; i++) {
            select.options[i].setAttribute("style", "border: none;");
        }
    
        $(select.options[select.selectedIndex]).css({ /*border: "2px solid #A7A7A7",*/color: "white", backgroundColor: "#828AD0" });
        var spanDesc = document.getElementById("ctl00_publicWorkflowRetrievedDescription");
        var list = document.getElementById("allPublicWorkflow");
        var desc = ((($(list).text()).split("\\||//")[index]).split("\\|//")[0]).split("\\|)//")[1];

        if ((desc == "") | (desc == "...write a description here")) {
            desc = "No description present";
        }
        spanDesc.value = desc;

        enableButton("publicWFCompileButton", "location.href='" + select.options[select.selectedIndex].title + "';");
    
}

////////////////////////////////////////////
//////////////Commons operation/////////////
////////////////////////////////////////////


function mouseOu(elem) {
    //
    if ($(elem).css("backgroundColor") == "rgb(167, 167, 167)") {
        $(elem).css({ backgroundColor: "transparent", opacity: "1.0" })
    }
}
function mousO(elem) {
    if ($(elem).css("backgroundColor") == "transparent") {
        $(elem).css({ backgroundColor: "#A7A7A7", opacity: "0.5" });
    }
}