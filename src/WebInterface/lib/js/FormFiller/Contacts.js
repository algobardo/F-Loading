var groupList = new Array();
var gruppoCorr = "";

//Respnse callback to get group of user
function getUserGResult(val) {

    var groups = val.split("\\|/");
    //if (groups[0] == "-200") {
    spinnerStop();
    //      }
    var dialogWorkflowBodyContentDiv = document.getElementById("dialogWorkflowBodyContentDiv");
    var contactdiv = document.getElementById("ctl00_listGroup");
    if (groups.length == 1) {
        $(contactdiv).text(val);
    }
    else {
        for (var i = 1; i < groups.length; i++) {
            var brek = document.createElement("p");
            var group = document.createElement("div");
            var arg = groups[0] + "|" + groups[i];
            group.setAttribute("onmouseover", "mousO(this)");
            group.setAttribute("onmouseout", "mouseOu(this)");
            group.setAttribute("id", groups[i]);
            group.setAttribute("onclick", "addContactGroup('" + arg + "','" + groups[i] + "',this)");
            $(group).text(groups[i]);
            brek.appendChild(group);
            contactdiv.appendChild(brek);
        }
    }
}

//method to add contact to a group
function addContactGroup(arg, group, elem) {
    var contatti = document.getElementById("ctl00_listContact");
    var gruppi = document.getElementById("ctl00_listGroup");
    var checks = contatti.childNodes;
    if ($(checks).text() != "") {
        for (var i = 1; i < checks.length; i++) {
            var checkbox = checks.item(i).childNodes.item(0);
            if (checkbox.checked) {
                var label = checks.item(i).childNodes.item(1);
                var ind = HasElement($(label).text() + "/false");
                if (ind != -2) {
                    for (var j = 0; j < ind.length; j++) {
                        groupList[ind[j]] = $(label).text() + "/true";
                    }
                }
            }
            if (!checkbox.checked) {
                var label = checks.item(i).childNodes.item(1);
                var ind = HasElement($(label).text() + "/true");
                if (ind != -2) {
                    for (var j = 0; j < ind.length; j++) {
                        groupList[ind[j]] = $(label).text() + "/false";
                    }
                }
            }
        }
    }

    var ind = HasElement(elem.id) + 1;
    for (var i = 0; i < gruppi.childNodes.length; i++) {
        var temp = gruppi.childNodes.item(i);
        if (temp != elem)
            $(temp.childNodes.item(0)).css({ color: "black", backgroundColor: "transparent" });
    }
    $(elem).css({ color: "white", backgroundColor: "#828AD0", opacity: "1.0" });

    gruppoCorr = elem.id;
    $(contatti).text("");
    if (HasElement(elem.id) == -2) {
        groupList.push(group);
        getContactsListCall(arg, "");
    }
    else {
        var tmp = HasElement(elem.id);
        var ind = tmp[0] + 1;
        var sottr = ind;
        while (groupList[ind] != "|") {
            var group = groupList[ind].split("/");
            var check = document.createElement("input");
            var label = document.createElement("label");
            check.setAttribute("id", "" + (ind - sottr));
            check.setAttribute("type", "checkbox");
            var group2 = HasElement(group[0] + "/true");
            if (group[1] == "true" || group2 != -2)
                check.checked = true;
            $(label).text(group[0]);
            var brek = document.createElement("p");
            brek.appendChild(check);
            brek.appendChild(label);
            contatti.appendChild(brek);
            ind++;
        }
        if ((ind - sottr) == 0) {
            $(contatti).text("There is not any contact");
        }
        enableButton("dialogRightButton", "send('" + arg.split("|")[0] + "|" + "')");
    }
}


function HasElement(elem) {
    var res = -2;
    var resarr = new Array();
    for (var i = 0; i < groupList.length; i++) {
        if (groupList[i] == elem) {
            res = i;
            resarr.push(i);
        }
    }
    if (res == -2) return res;
    else return resarr;
}

//response of callback about list of contact
function getContactsListResult(val) {
    var contatti = document.getElementById("ctl00_listContact");
    var contacts = val.split("\\|/");
    var contWdiv = document.getElementById("dialogWorkflowContactList");
    if (contacts.length == 1) {
        $(contatti).text(val);
        disableButton("dialogRightButton");
    }
    else {
        for (var i = 1; i < contacts.length - 1; i++) {
            var check = document.createElement("input");
            var label = document.createElement("label");
            check.setAttribute("id", "" + i);
            check.setAttribute("type", "checkbox");
            $(label).text(contacts[i]);
            if (HasElement(contacts[i] + "/true") != -2) {
                check.checked = true;
                groupList.push(contacts[i] + "/true");
            }
            else { groupList.push(contacts[i] + "/false"); }
            var brek = document.createElement("p");
            brek.appendChild(check);
            brek.appendChild(label);
            contatti.appendChild(brek);
        }
        enableButton("dialogRightButton", "send('" + contacts[0] + "|" + "')");
    }
    groupList.push("|");
}

//method to send mail
function send(val) {
    var result = "";
    var gruppo = "";
    var ind = 0;
    for (var i = 0; i < groupList.length; i++) {
        var group = groupList[i].split("/");
        if (group.length == 1) { gruppo = group; ind = 0; }
        if (group[1] == "true") result += (ind - 1) + "/" + gruppo + "|";
        ind++;
    }
    var contatti = document.getElementById("ctl00_listContact");
    var checks = contatti.childNodes;
    for (var i = 1; i < checks.length; i++) {
        var checkbox = checks.item(i).childNodes.item(0);
        if (checkbox.checked) {
            result += (i - 1) + "/" + gruppoCorr + "|";
        }
    }

    if (result == "") {
        alert("Select at least a contact");
    }
    else {

        spinnerStart(document.getElementById("dialogWorkflowBodyDiv"), "spinnerSendMail");
        SendCall(val + result, "");
    }
}

//show sending result
function SendResult(val) {

    if (val == "OK")
        showMailResponse("Operation succesful");
    else
        showMailResponse("Operation failed");
}

function showMailResponse(message) {
    alert(message);
    spinnerStop();
}