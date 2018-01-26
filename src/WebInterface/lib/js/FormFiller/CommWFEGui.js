//var about form's type need by wfeditor and Gui
var WFPublishGlobalTypeToSendEmail = "";

//Response of callback about comunication information services
function getCommListResult(args, context) {
    WFE_howToPublishWorkflow(args);
    spinnerStop();
}

//method to choose service from drop down list
function choosen(arg) {
    var dialogPublishing = document.getElementById("dialog_publishing");
    getServiceFieldCall(arg, '');
    spinnerStart(dialogPublishing,"dialogPublishWorkflowCustom");
}

//Response of callback about field information retrieve from a service
function getServiceFieldResult(args, context) {
    var mail = document.getElementById("pEmail");
    var user = document.getElementById("pUser");
    var pass = document.getElementById("pPass");
    var host = document.getElementById("pHost");
    var dir = document.getElementById("pDir");
    mail.style.display = "none";
    user.style.display = "none";
    pass.style.display = "none";
    host.style.display = "none";
    dir.style.display = "none";
    for (i = 0; i < Page_Validators.length; i++) {
        Page_Validators[i].isvalid = true;
        ValidatorUpdateDisplay(Page_Validators[i]);

    }
    var ser = args.split("&");
     switch (ser[0]) {
        case "RSS Feed":
            {
                mail.style.display = "block";
                break;
            };
        case "Email":
            {
                mail.style.display = "block";
                break;
               }
            case "Google Docs":
            {
                user.style.display = "block";
                pass.style.display = "block";
                break;
             }
            
        case "Ftp Server":
            {
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

//Method to restore button in dialog
function restoreButton() {
    var id = WFG_workflow.getId();
    var dialogPublishing = document.getElementById("dialog_publishing");
    $("#dialog_publishing").dialog(
    'option',
    'buttons',
    { 'Go ->': function() {
        var formType = $(this).find("#sele_saveWF").val();

        var descr = $(this).find("#wfDescription").val();
       
        var expDate = $(this).find("#datepicker").val();
        var selectedServices = null;
        if (formType == "public") {
            selectedServices = $(this).find('#seleEnabledServices').val();
        }
        checkOption = $(this).find('#mode').attr('checked');
        WFS_SetWorkflowDescription(id, descr);
        getCommListCall('', '');
        spinnerStart(dialogPublishing, "dialogPublishWorkflowCustom");
        WFPublishGlobalTypeToSendEmail = formType;
        transitionPublish(formType, descr, expDate, checkOption, selectedServices);
        
    },
        'Cancel': function() {
            $('#form_typePW').html("");
            $(this).dialog('close');
        }
    });
}

//Transition in dialog publish 
function transitionPublish(formType, desc, expirationDate, checkOption, selectedServices) {

    var id = WFG_workflow.getId();
    var buttons = $("#dialog_publishing").dialog('option', 'buttons');
    var box = document.getElementById("box");
    var boxSx = document.getElementById("boxSx");
    var boxDx = document.getElementById("boxDx");
    $(function() {
        $(box).animate({ opacity: "toggle" }, 1000, function() {
            boxSx.style.display = "none";
            boxDx.style.display = "block";
            $(box).animate({ opacity: "toggle" }, 1000);
        });
    });

    $("#dialog_publishing").dialog(
    'option',
    'buttons',
    { 'Publish': function() {
        if (Page_ClientValidate()) {

            var wfType = formType;
            var wfDescription = desc;
            var wfExpirationDate = expirationDate;
            var wfCheckOption = checkOption;
            var wfSelectedServices = selectedServices;
            
            WFPublishGlobalTypeToSendEmail = wfType;
            retrieveResult();

        } else
            alert("You must insert all boxes");
    },
        'Cancel': function() {
            $(this).dialog('close');
            restoreButton();

        },
        'Back': function() {
            backTransitionPublish();
            resetPublish();
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
            ValidatorUpdateDisplay(Page_Validators[i]);
        }
    }
    ValidatorUpdateIsValid();
    ValidationSummaryOnSubmit(validationGroup);
    Page_BlockSubmit = !Page_IsValid;
    return Page_IsValid;
};

//transition in dialog publish, on back button
function backTransitionPublish() {
    var box = document.getElementById("box");
    var boxSx = document.getElementById("boxSx");
    var boxDx = document.getElementById("boxDx");

    $(function() {
        $(box).animate({ opacity: "toggle" }, 1000, function() {
            boxSx.style.display = "block";
            boxDx.style.display = "none";
            $(box).animate({ opacity: "toggle" }, 1000);
        });
    });
    restoreButton();

}

//Restore initial setting of dialog
function resetPublish() {
    restoreButton();
    $(this).find("#sele_saveWF").val("Public");
    
    
    var mail = document.getElementById("pEmail");
    var user = document.getElementById("pUser");
    var pass = document.getElementById("pPass");
    var host = document.getElementById("pHost");
    var dir = document.getElementById("pDir");
    mail.style.display = "none";
    user.style.display = "none";
    pass.style.display = "none";
    host.style.display = "none";
    dir.style.display = "none";
}

//Restore intial visualization setting of dialog
function resetBox() {
    var boxSx = document.getElementById("boxSx");
    var boxDx = document.getElementById("boxDx");
    boxSx.style.display = "block";
    boxDx.style.display = "none";
}

//method to invoke uri creation
function retrieveResult() {

    var ser = $('#serviceComm').val();
    var request = "";
    if (ser != "none") {
        request = ser + "$";

        var mail = document.getElementById("pEmail");
        var user = document.getElementById("pUser");
        var pass = document.getElementById("pPass");
        var host = document.getElementById("pHost");
        var dir = document.getElementById("pDir");

        if (mail.style.display != "none") {
            request = request + "email|" + $('#ctl00_contentHome_email').val() + "&";
        }
        if (user.style.display != "none") {
            request = request + "username|" + $('#ctl00_contentHome_user').val() + "&";
        } if (pass.style.display != "none") {
        request = request + "password|" + $('#ctl00_contentHome_pass').val() + "&";
        } if (host.style.display != "none") {
        request = request + "host|" + $('#ctl00_contentHome_host').val() + "&";
        } if (dir.style.display != "none") {
        request = request + "directory|" + $('#ctl00_contentHome_dir').val() + "&";
        }
    }
    CreateUriCall(request, '');
}

//Responde about uri creation
function CreateUriResult(args, context) {
    if (args == "ok") {
        
        var wfType = $('#sele_saveWF').val();
        var wfDescription = $("#wfDescription").val();
        var wfExpirationDate = $("#datepicker").val();
        var wfSelectedServices = null;
        if (wfType == "public") {
            wfSelectedServices = $('#seleEnabledServices').val();
        }
        wfCheckOption = $('#mode').attr('checked');
        var id = WFG_workflow.getId();
        WFC_publishWorkflow(id, wfType, wfDescription, wfExpirationDate, wfCheckOption, wfSelectedServices, WFE_getUsingStaticFieldCookie());    
            
        restoreButton();
    } else
        alert(args);
}