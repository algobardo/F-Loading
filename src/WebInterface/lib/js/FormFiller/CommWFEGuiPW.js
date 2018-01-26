//Response of callback about comunication information services
function getCommListPWResult(args, context) {
    var service = args;

    var list = service.split("|");

    $("#serviceCommPW").html("");
    var serviceOption = document.getElementById("serviceCommPW");

    var noneOp = document.createElement("option");
    var noneText = document.createTextNode("None");
    noneOp.appendChild(noneText);
    noneOp.value = "none";
    serviceOption.appendChild(noneOp);


    for (j = 0; j < list.length - 1; j++) {
        var op = document.createElement("option");
        var opText = document.createTextNode(list[j]);
        op.appendChild(opText);
        op.value = list[j];
        serviceOption.appendChild(op);
    }
    spinnerStop();
}

//method to choose service from drop down list
function choosen2(arg) {
    var dialogPublishWorkflow = document.getElementById("dialogPublishWorkflow");
    spinnerStart(dialogPublishWorkflow, "dialogPublishWorkflowCustom");
    getServiceFieldPWCall(arg, '');
}

//method to invoke callback publish
function PublishWfResult(arg, context) {
    alert(arg);
    spinnerStop();
    
    if (WFPublishGlobalTypeToSendEmail == "private") {
        $('#dialogWorkflowDiv').dialog('close');
        getUserGCall(-200,''); //take the WFE_CurrentWorkflow in session on server call
        clickContactUser();
    }
}

//Response of callback about field information retrieve from a service
function getServiceFieldPWResult(args, context) {

    var mail = document.getElementById("pEmailPW");
    var user = document.getElementById("pUserPW");
    var pass = document.getElementById("pPassPW");
    var host = document.getElementById("pHostPW");
    var dir = document.getElementById("pDirPW");
    mail.style.display = "none";
    user.style.display = "none";
    pass.style.display = "none";
    host.style.display = "none";
    dir.style.display = "none";
    for (i = 0; i < Page_Validators.length; i++) {
        Page_Validators[i].isvalid = true;
        ValidatorUpdateDisplayDefault(Page_Validators[i]);

    }
    var ser = args.split("&");
    switch (ser[0]) {
        case "RSS Feed":
            {
                //rss
                mail.style.display = "block";

                break;
            };
        case "Email":
            {
                //email
                mail.style.display = "block";
                break;
               }

              case "Google Docs":
              	{
              		//ftp
              		user.style.display = "block";
              		pass.style.display = "block";
              		break;
              	}
        case "Ftp Server":
            {
                //ftp
                user.style.display = "block";
                pass.style.display = "block";
                host.style.display = "block";
                dir.style.display = "block";
                break;
            }
        default:
            {
                break;
            };
    }
    spinnerStop();
}

//Method to validate validator
function ValidatorUpdateDisplayDefault(val) {
    validateDialog = true;

    if (typeof (val.display) == "string") {
        if (val.display == "None") {
            return;
        }
        if (val.display == "")
            return;
        if (val.display == "Dynamic") {
            val.style.display = val.isvalid ? "none" : "inline";
            return;
        }
    }
    if ((navigator.userAgent.indexOf("Mac") > -1) &&
        (navigator.userAgent.indexOf("MSIE") > -1)) {
        val.style.display = "inline";
    }
    val.style.visibility = val.isvalid ? "hidden" : "visible";

}

//Method to restore button in dialog
function restoreButtonPW() {
    var dialogPublishWorkflow = document.getElementById("dialogPublishWorkflow");
    $(dialogPublishWorkflow).dialog(
    'option',
    'buttons',
    { 'Go ->': function() {
        var wfType = $(this).find("#sele_saveWFPW").val();
        var descr = $(this).find("#wfDescriptionPW").val();
        var expDate = $(this).find("#datepickerPW").val();
        var selectedServices = null;
        if (wfType == "public") {
            selectedServices = $(this).find('#seleEnabledServicesPW').val();
        }
        checkOption = $(this).find('#modePW').attr('checked');

        getCommListPWCall('', '');
        spinnerStart(dialogPublishWorkflow, "dialogPublishWorkflowCustom");
        WFPublishGlobalTypeToSendEmail = wfType;
        transitionPublishPW(wfType, descr, expDate, checkOption, selectedServices);
        

    },
        'Cancel': function() {
            $('#form_typePW').html("");
            $(this).dialog('close');
        }
    });
}

//Transition in dialog publish 
function transitionPublishPW(formType, desc, expirationDate, checkOption, selectedServices) {

    var buttons = $("#dialogPublishWorkflow").dialog('option', 'buttons');
    var box = document.getElementById("boxPW");
    var boxSx = document.getElementById("boxSxPW");
    var boxDx = document.getElementById("boxDxPW");
    $(function() {
        $(box).animate({ opacity: "toggle" }, 1000, function() {
            boxSx.style.display = "none";
            boxDx.style.display = "block";
            $(box).animate({ opacity: "toggle" }, 1000);
        });
    });

    $("#dialogPublishWorkflow").dialog(
    'option',
    'buttons',
    { 'Publish': function() {
        if (Page_ClientValidate(10)) {
            var wfType = formType;
            var wfDescription = desc;
            var wfExpirationDate = expirationDate;
            var wfCheckOption = checkOption;
            var wfSelectedServices = selectedServices;
            //WFS_SetWorkflowDescription(id, desc); ------------->  ?!?!?!?!?!?

            WFPublishGlobalTypeToSendEmail = wfType;
            retrieveResultPW();
        } else {
            alert("You must insert all boxes");
        }
    },
        'Cancel': function() {
            $(this).dialog('close');
            restoreButtonPW();

        },
        'Back': function() {
            backTransitionPublishPW();
            resetPublishPW();
        }
    });
    spinnerStop();
}

//Overwritten to read function is called when the validation of the page and submit before the callback
function Page_ClientValidate(validationGroup) {
    Page_InvalidControlToBeFocused = null;
    if (typeof (Page_Validators) == "undefined") {
        return true;
    }
    var i;
    for (i = 0; i < Page_Validators.length; i++) {
        var controlID = Page_Validators[i].controltovalidate;
        var control = document.getElementById(controlID);
        var parent = control.parentNode;

        if ((parent.style.display != "none") & (parent.style.display != "")) {
            ValidatorValidate(Page_Validators[i], validationGroup, null);
        }
        else {

            Page_Validators[i].isvalid = true;
            ValidatorUpdateDisplayDefault(Page_Validators[i]);
        }
    }
    ValidatorUpdateIsValid();
    ValidationSummaryOnSubmit(validationGroup);
    Page_BlockSubmit = !Page_IsValid;
    return Page_IsValid;
};

//transition in dialog publish, on back button
function backTransitionPublishPW() {
    var box = document.getElementById("boxPW");
    var boxSx = document.getElementById("boxSxPW");
    var boxDx = document.getElementById("boxDxPW");

    $(function() {
        $(box).animate({ opacity: "toggle" }, 1000, function() {
            boxSx.style.display = "block";
            boxDx.style.display = "none";
            $(box).animate({ opacity: "toggle" }, 1000);
        });
    });
    restoreButtonPW();
}

//Restore initial setting of dialog
function resetPublishPW() {
    restoreButtonPW();
    $(this).find("#sele_saveWFPW").val("Public");
    $(this).find("#wfDescriptionPW").val("");

    $("#serviceCommPW").html("");
    
    var mail = document.getElementById("pEmailPW");
    var user = document.getElementById("pUserPW");
    var pass = document.getElementById("pPassPW");
    var host = document.getElementById("pHostPW");
    var dir = document.getElementById("pDirPW");
    mail.style.display = "none";
    user.style.display = "none";
    pass.style.display = "none";
    host.style.display = "none";
    dir.style.display = "none";
}

//Restore intial visualization setting of dialog
function resetBoxPW() {
    var boxSx = document.getElementById("boxSxPW");
    var boxDx = document.getElementById("boxDxPW");
    boxSx.style.display = "block";
    boxDx.style.display = "none";
}

//method to invoke uri creation
function retrieveResultPW() {

    var ser = $('#serviceCommPW').val();
    var request = "";
    if (ser != "none") {
        request = ser + "$";

        var mail = document.getElementById("pEmailPW");
        var user = document.getElementById("pUserPW");
        var pass = document.getElementById("pPassPW");
        var host = document.getElementById("pHostPW");
        var dir = document.getElementById("pDirPW");
        var url = document.getElementById("pUrlPW");

        if (mail.style.display != "none") {
            request = request + "email|" + $('#ctl00_emailPW').val() + "&";
        }
        if (user.style.display != "none") {
            request = request + "username|" + $('#ctl00_userPW').val() + "&";
        } if (pass.style.display != "none") {
            request = request + "password|" + $('#ctl00_passPW').val() + "&";
        } if (host.style.display != "none") {
            request = request + "host|" + $('#ctl00_hostPW').val() + "&";
        } if (dir.style.display != "none") {
            request = request + "directory|" + $('#ctl00_dirPW').val() + "&";
        }
    }
    CreateUriPWCall(request, '');
}

//Responde about uri creation
function CreateUriPWResult(args, context) {
    if (args == "ok") {

        var formType = $('#sele_saveWFPW').val();
        var descr = $("#wfDescriptionPW").val();
        var expDate = $("#datepickerPW").val();
        var selectedServices = null;
        if (formType == "public") {
            selectedServices = $('#seleEnabledServicesPW').val();
        }
        checkOption = $('#modePW').attr('checked');
        var param = formType + "|" + descr + "|" + expDate + "|" + checkOption + "|" + selectedServices + "|"+WFE_getUsingStaticFieldCookie();

        spinnerStart(document.documentElement); 
        PublishWfCall(param, '');
        
        $("#dialogPublishWorkflow").dialog('close');
        restoreButtonPW();
    } else
        alert(args);
}