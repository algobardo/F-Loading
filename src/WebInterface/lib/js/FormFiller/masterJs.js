var headerOpened = true;

$(document).ready(function() {

    //dialog result    
    var dialogResultDiv = document.getElementById("dialogResultDiv");
    var resRet = document.getElementById("ctl00_resultRetrieve");
    $(dialogResultDiv).addClass('dialogDiv').dialog({
        autoOpen: false,
        bgiframe: true,
        show: "clip",
        modal: true,
        draggable: false,
        resizable: false,
        width: 700,
        height: 430,
        zIndex: 1800,
        position: ['center', 'center'],
        open: function() {
            var link = document.getElementById("resultExportLink");
            $(link).text("");
            link.setAttribute("href", "");
            cleanDialog(resRet);
            spinnerStart(document.getElementById("dialogResultBodyContentDiv"));
        },
        close: function() {
            cleanSelected(resRet);
            spinnerStop();
        }
    });    

    //dialog logout
    var dialogLoginError = document.getElementById("logoutDialog");
    $(dialogLoginError).addClass('logout').dialog({
        autoOpen: false,
        bgiframe: true,
        modal: true,
        draggable: false,
        resizable: false,
        width: 300,
        height: 250,
        zIndex: 1800,
        title: "Logout",
        position: ['center', 'center'],
        buttons: {
            'Ok': function() {
                LogoutCall('', '');
                $(this).dialog('close');
            },
            'Cancel': function() {
                $(this).dialog('close');
            }
        }
    });


    //dialog publish model
    var dialogPublishWorkflow = document.getElementById("dialogPublishWorkflow");
    $(dialogPublishWorkflow).addClass('dialogDiv').dialog({

        bgiframe: true,
        autoOpen: false,
        resizable: false,
        height: 480,
        width: 340,
        modal: true,
        position: ['center', 'center'],
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        buttons: {
            'Go ->': function() {
                var formType = $(this).find("#sele_saveWFPW").val();
                var descr = $(this).find("#wfDescriptionPW").val();
                var expDate = $(this).find("#datepickerPW").val();
                var selectedServices = null;
                if (formType == "public") {
                    selectedServices = $(this).find('#seleEnabledServicesPW').val();
                }
                checkOption = $(this).find('#modePW').attr('checked');

                getCommListPWCall('', '');
                spinnerStart(dialogPublishWorkflow, "dialogPublishWorkflowCustom");
                transitionPublishPW(formType, descr, expDate, checkOption, selectedServices);

            },
            'Cancel': function() {
                $('#form_typePW').html("");
                $(this).dialog('close');
            }
        },
        open: function(event, ui) {
            //setting the conditional droplist and checkbox
            var arg = $(this).data('arg');

            WFPublishGlobalTypeToSendEmail = "";
            var serv = arg.split("\\//");
            $(this).data('arg', serv[0]);
            buildServiceList(serv[1]);
            publicTypeOptions();
            $(this).find("#wfDescriptionPW").val("...write a description here");
            $(this).find("#sele_saveWFPW").val("Public");

            //setting the datapicker for the publish dialog
            var date = new Date();

            var date_string = (date.getDate() + 1) + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
            $("#expiration_datePW").html('<input type="text" id="datepickerPW" value="' + date_string + '"/>');
            $("#datepickerPW").datepicker({
                zIndex: 1800,
                altFormat: 'dd/mm/yyyy',
                dateFormat: 'dd/mm/yy',
                minDate: '+1d',
                beforeShow: function(input) {
                    $("#datepickerPW").datepicker('enable');
                },
                onClose: function(dateText, inst) {
                }
            });

        },
        close: function(event, ui) {
            resetPublishPW();
            resetBoxPW();
        },
        zIndex: 500
    });

    //dialog remove
    $(document.getElementById("deleteDialog")).dialog({
        autoOpen: false,
        bgiframe: true,
        resizable: false,
        height: 140,
        width: 400,
        modal: true,
        draggable: false,
        show: 'slide',
        hide: 'slide',
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        zIndex: 2000
    });

    //dialog user workflow
    var dialogWorkflowDiv = document.getElementById("dialogWorkflowDiv");
    var modelRet = document.getElementById("ctl00_modelRetrieved");
    var workRet = document.getElementById("ctl00_workflowRetrieved");
    var workRetDes = document.getElementById("ctl00_workflowRetrievedDescription");
    var listGroup = document.getElementById("ctl00_listGroup");
    var listContact = document.getElementById("ctl00_listContact");
    $(dialogWorkflowDiv).addClass('dialogDiv').dialog({
        autoOpen: false,
        bgiframe: true,
        show: "clip",
        modal: true,
        draggable: false,
        resizable: false,
        width: 700,
        height: 430,
        zIndex: 1800,
        position: ['center', 'center'],
        title: titleUserWorkflow,
        open: function() {
            cleanDialog(workRet);
            cleanDialog(modelRet);
            cleanDialog(workRetDes);
            cleanDialog(listGroup);
            cleanDialog(listContact);
            spinnerStart(document.getElementById("dialogWorkflowList"), "spinnerMyFormDialog");
        },
        close: function() {
            //if the dialog is closed to the contact list and then reopened it should be reset callback
            workRetDes.style.width = "325px";
            cleanSelected(workRet);
            spinnerStop();
        }
    });

    //dialog public workflow
    var dialogPublicWorkflowDiv = document.getElementById("dialogPublicWorkflowDiv");
    var workPublicRet = document.getElementById("ctl00_publicWorkflowRetrieved");
    var workPublicRetDes = document.getElementById("ctl00_publicWorkflowRetrievedDescription");
    $(dialogPublicWorkflowDiv).addClass('dialogDiv').dialog({
        autoOpen: false,
        bgiframe: true,
        show: "clip",
        modal: true,
        draggable: false,
        resizable: false,
        width: 700,
        height: 430,
        zIndex: 1800,
        position: ['center', 'center'],
        title: "Choose one workflow to compile",
        open: function() {
            disableButton("publicWFCompileButton");
            cleanDialog(workPublicRet);
            cleanDialog(workPublicRetDes);
            spinnerStart(document.getElementById("dialogPublicWorkflowBodyContentDiv"));
        },
        close: function() {
            cleanSelected(workRet);
            spinnerStop();
        }
    });
});

//method to close the header during a filling
function headerDivRed(clicked) {
    var headerDiv = document.getElementById("headerDiv");
    var freccia = document.getElementById("ctl00_freccia");
    var headerDivRight = document.getElementById("headerDiv-right");
    if (clicked == "none") {
        if (headerOpened) {
            if (navigator.appName == "Microsoft Internet Explorer")
                $(headerDivRight).css("display", "none");
        }
        $(headerDiv).slideToggle("slow", function() {
            if (headerOpened) {

                headerOpened = false;
                freccia.setAttribute("src", "../lib/image/arrowd.gif");
            } else {
                $(headerDivRight).css("display", "inline");
                headerOpened = true;
                freccia.setAttribute("src", "../lib/image/arrowu.gif");
            }
        });
    } else {
        if (clicked == "filling") {
            headerDiv.style.display = "none";
            headerOpened = false;
            freccia.setAttribute("src", "../lib/image/arrowd.gif");

        } else
            if (clicked == "login") {
            if (!headerOpened) {
                $(headerDiv).slideToggle("slow", function() {
                    headerOpened = true;
                    freccia.setAttribute("src", "../lib/image/arrowu.gif");
                });
            }
        }
    }
}

//opening the result dialog
function clickResult() {
    getResultRetCall("", "");
    disableButton("exportResultButton");
    var dialogResultDiv = document.getElementById("dialogResultDiv");
    $(dialogResultDiv).dialog('open', {});
}

//clean the code
function cleanSelected(val) {
    if(navigator.appName == "Netscape"){
        for (i = 0; i < val.childNodes.length; i++) {
            var figlio = val.childNodes[i];
            if (figlio.firstChild != null)
                figlio.firstChild.setAttribute("style", "border: none;");
        }
    }
}

//clean the dialog
function cleanDialog(dialog) {
    dialog.innerHTML = "";
    if ((dialog.value != null) & (dialog.value != ""))
        dialog.value = "";
}

//open login https
function openHttps() {
    $('#httpsService').dialog('open', {});
}

//is called by wf-editor after creation or from the dialog myform-> publish-> private-> send
function clickContactUser() {

    var dialogWorkflowDiv = document.getElementById("dialogWorkflowDiv");
    var dialogWorkflowList = document.getElementById("dialogWorkflowList");
    var listWork = document.getElementById("ctl00_workflowRetrieved");
    var labelWork = document.getElementById("ctl00_workflowRetrievedDescription");
    var dialogWorkflowContactList = document.getElementById("dialogWorkflowContactList");
    var listGroup = document.getElementById("ctl00_listGroup");
    var listContact = document.getElementById("ctl00_listContact");
    //fix for send->back->publish
    if ($(listContact).width() == 0) {
        $(listContact).animate({ width: "310px" }, 1000);
    }
    dialogWorkflowContactList.style.display = "block";
    listGroup.style.display = "block";
    listContact.style.display = "block";
    labelWork.style.display = "none";
    listWork.style.display = "none";
    dialogWorkflowList.style.display = "none";

    hideButton("newWorkflow");
    hideButton("dialogCenterButton");
    hideButton("deleteWorkflow");
    changeButtonText("dialogRightButton", "Send request");
    disableButton("dialogRightButton");

    $(dialogWorkflowDiv).data('title.dialog', titleSelectContact);
    $(dialogWorkflowDiv).dialog('open', {});
}

//method to open dialog about workflow with logged user
function clickWorkUser() {
    spinnerStop();
    var dialogWorkflowDiv = document.getElementById("dialogWorkflowDiv");
    var dialogWorkflowList = document.getElementById("dialogWorkflowList");
    var listWork = document.getElementById("ctl00_workflowRetrieved");
    var listModel = document.getElementById("ctl00_modelRetrieved");
    var labelWork = document.getElementById("ctl00_workflowRetrievedDescription");
    var dialogWorkflowContactList = document.getElementById("dialogWorkflowContactList");
    var listGroup = document.getElementById("ctl00_listGroup");
    var listContact = document.getElementById("ctl00_listContact");

    changeButtonText("dialogRightButton", "Publish");
    changeButtonText("dialogCenterButton", "Edit model");
    disableButton("dialogRightButton");
    disableButton("dialogCenterButton");
    disableButton("deleteWorkflow");
    showButton("newWorkflow");
    showButton("deleteWorkflow");
    showButton("dialogCenterButton");
    showButton("dialogRightButton");

    document.getElementById("modelTab").setAttribute("class", "openDialogTab");
    document.getElementById("publicationTab").setAttribute("class", "dialogTab");

    dialogWorkflowContactList.style.display = "none";
    listGroup.style.display = "none";
    listContact.style.display = "none";

    dialogWorkflowList.style.display = "block";

    labelWork.style.display = "block";
    listModel.style.display = "block";
    listWork.style.display = "none";

    $(dialogWorkflowDiv).data('title.dialog', titleUserWorkflow);
    $(dialogWorkflowDiv).dialog('open', {});
    spinnerStop();
    getWorkFlowListCall('', '');
}

//method to open dialog about public workflow
function clickWorkPublic() {
    getFormPCall('', '');
    var dialogPublicWorkflowDiv = document.getElementById("dialogPublicWorkflowDiv");
    $(dialogPublicWorkflowDiv).dialog('open', {});
}

//method to animate menu when login
function animeLogin() {

    var menu = document.getElementById('menu');
    var ServiceListContainer = document.getElementById('ServiceListContainer');
    var ServiceList = document.getElementById('ctl00_ServiceList');
    var pannello = document.getElementById("ctl00_updatePanel2");
    //This audit was made because it generated an error, further review by
    if (pannello != null) {
        menu.removeChild(pannello);
        if (navigator.appName == "Microsoft Internet Explorer") {
            $("#border-menu-top-left").css("position", "relative");
        }
        var up = document.createElement('a');
        var upImg = new Image();
        upImg.src = "../lib/image/menu.png";
        upImg.style.border = "none";
        upImg.style.height = "25px";
        upImg.style.margin = "5px 0px 0px 0px";
        var up2 = document.createTextNode("Menu");
        up.title = "Open menu";
        up.setAttribute('href', '#');
        up.setAttribute("style", "font-family:Comic Sans MS;");
        up.appendChild(upImg);
        menu.appendChild(up);
        up.onclick = function() {
            $(ServiceListContainer).animate({
                left: "-100px"
            },
        500,
        function() {
            if (navigator.appName == "Microsoft Internet Explorer") {
                $("#border-menu-top-left").css("position", "absolute");
            }
            $(ServiceListContainer).hide(
            'blind',
            500,
            function() {
                ServiceListContainer.style.display = "none";
                ServiceListContainer.setAttribute("style", "position:relative;float:right;top:0px;left:-100px;margin:0px;height:35px;width:auto;");
            })
            menu.appendChild(pannello);
            menu.removeChild(up);
            $(menu).animate({
                height: "150px",
                top: "0px"
            },
            500
            );
        });
        };
        ServiceListContainer.style.display = "block";
        $(menu).animate({
            height: "35px",
            top: "80px"
        },
    500);
        $(ServiceListContainer).show(
    'blind',
    500,
    function() {
        $(ServiceListContainer).animate({
            left: "0px"
        },
        300

        );
    });
    }
}

//FOR DIALOG PUBLISH

function openPublishDialog(idx) {

    var dialogWorkflowDiv = document.getElementById("dialogWorkflowDiv");
    spinnerStart(dialogWorkflowDiv, "dialogWorkflowDivCustom");
    getIdModelAuthServiceCall(idx, '');
    var dialogPublishWorkflow = document.getElementById("dialogPublishWorkflow");
    $(dialogPublishWorkflow).data('idx', idx);
}

//method to get authentication service
function getIdModelAuthServiceResult(arg, context) {
    spinnerStop();
    var dialogPublishWorkflow = document.getElementById("dialogPublishWorkflow");
    $(dialogPublishWorkflow).data('arg', arg);
    $(dialogPublishWorkflow).dialog('open');
}

function dropList() {
    if ($('#modePW').attr('checked')) {
        var html = '<p class="p" id="services_droplistPW">Enabled services: <select id="seleEnabledServicesPW" title="select the account that grants the users to compile the form">';
        html += '<option id="allservicesPW" value="allservices" selected="selected">All services</option>';
        for (var s = 0; s < authServices.length(); s++) {
            var name = authServices.getServiceNameByIndex(s);
            var id = authServices.getServiceIdByIndex(s);
            html += '<option id="' + name + 'PW" value="' + id + '">' + name + '</option>'
        }
        html += '</select></p>';
        $('#form_typePW').append(html);
    } else {
        $('#services_droplistPW').remove();
    }
}


function openLogout() {
    var dialogLoginError = document.getElementById("logoutDialog");
    $(dialogLoginError).dialog('open');


}

function LogoutResult(arg, context) {
    window.location = arg;
}

function ShowLoginError(message, returnUrl) {
    var dialogLoginError = document.getElementById("dialogLoginError");
    dialogLoginError.innerHTML = message;

    $(dialogLoginError).dialog({
        autoOpen: true,
        bgiframe: true,
        show: "fold",
        modal: true,
        draggable: false,
        resizable: false,
        width: 440,
        height: 150,
        zIndex: 30000,
        title: "Login Error",
        position: ['center', 100],
        buttons: {
            Close: function() {
                $(this).dialog('close');
            }
        },
        open: function() {
        },
        close: function() {
        }
    });
}


buildServiceList = function(services_to_print) {
    authServices = new authServiceList();
    var services = services_to_print.split('\\||//');
    for (var k = 0; k < services.length; k++) {
        authServices.addService(services[k].split('\\|//')[0], services[k].split('\\|//')[1]);
    }
}

authServiceList = function() {
    var servList = this;
    servList.list = new Array();

    service = function(name, id) {
        var serv = this;
        serv.name = name;
        serv.id = id;
    }

    servList.addService = function(name, id) {
        servList.list.push(new service(name, id));
    }

    servList.getServiceIdByName = function(name) {
        for (var i = 0; i < servList.list.length; i++) {
            if (servList.list[i].name == name) return servList.list[i].id;
        }
    }

    servList.getServiceNameById = function(id) {
        for (var i = 0; i < servList.list.length; i++) {
            if (servList.list[i].id == id) return servList.list[i].name;
        }
    }

    servList.getServiceNameByIndex = function(index) {
        if (index < servList.list.length) {
            return servList.list[index].name;
        }
        return null;
    }

    servList.getServiceIdByIndex = function(index) {
        if (index < servList.list.length) {
            return servList.list[index].id;
        }
        return null;
    }

    servList.length = function() {
        return servList.list.length;
    }
}

changePublicationType = function() {
    var pubType = $('#sele_saveWFPW').val();
    if (pubType == "public") {
        publicTypeOptions();
    } else {
        privateTypeOptions();
    }
}

publicTypeOptions = function() {
    $('#form_typePW').html('Single filling mode <input type="checkbox" id="modePW" onChange="dropList();" title="if checked the form will be available only if the user accesses with one of the service selected "/><br />');
}

privateTypeOptions = function() {
    $('#form_typePW').html('Anonymous filling mode <input type="checkbox" id="modePW" title="if checked the form will be anonymous (user inserted data will be disassociate from the user)"/><br />');
}

/**
*  Thi javascript disable the history rollback
*
* added by WorkflowEditor
**/
GUI_disableHistoryScript = function() {
    if (history.length > 0) {
        history.forward();
    }
}

/**
*  Script to capture events related to key pressed
**/
GUI_keyLogger = function(event) {
    var keynum;
    var keychar;
    var explorer = false;

    if (!event) return;

    if (window.event) // IE
    {
        keynum = event.keyCode;
        explorer = true;
    }
    else if (event.which) // Netscape/Firefox/Opera
    {
        keynum = event.which;
    }
    keychar = String.fromCharCode(keynum);
    if (keynum == 8) {
        if (explorer) {
            event.keyCode = 0;
            return event.keyCode;
        } else {
            event.wich = 0;
            return event.wich;
        }
    }
}

GUI_disableBackspace = function() {
    if (typeof window.event != 'undefined') {
        var src = null;
        document.onkeydown = function() {
            src = event.srcElement.tagName.toUpperCase();
            if (src != 'INPUT' && src != 'TEXTAREA')
                return (event.keyCode != 8);
        }
    } else {
        document.onkeypress = function(e) {
            src = e.target.nodeName.toUpperCase()
            if (src != 'INPUT' && src != 'TEXTAREA')
                return (e.keyCode != 8);
        }
    }
}

GUI_disableHistoryScript();

