
var validatePage = false;
var validateDialog = false;
var type;


$(document).ready(function() {
    //dialog for authentication during a filling
    $('#dialogLog').addClass('errorLog').dialog({
        autoOpen: false,
        bgiframe: true,
        hide: "fold",
        resizable: false,
        draggable: false,
        position: ['center', 200],
        modal: true,
        buttons: {
            'Ok': function() {
                var p = $(this).find('#selLog').val();
                fakeLogCall(p, '');
            },
            'Cancel': function() {
                $(this).dialog('close');
            }
        },
        close: function() {
            location.href = "/FormFillier/index.aspx";
        }
    });

//accordion for show result of filling
    $('#ctl00_contentHome_presenPanel2').accordion({ header: 'div.header', fillSpace: true }); //{header: 'h3'}
    $('#dialog').addClass('errorDialog').dialog({
        autoOpen: false,
        bgiframe: true,
        hide: "fold",
        minHeight: 10,
        resizable: false,
        position: [0, 0]
    });

    var cont = document.getElementById("panelFill");
    var dial = document.getElementById("dialogModalP");
    var posx = (cont.offsetWidth - dial.offsetWidth) / 2;
    var posy = (cont.offsetHeight - dial.offsetHeight) / 2

    $('#dialogModal').addClass('errorDialogModal').dialog({
        autoOpen: false,
        bgiframe: true,
        hide: "fold",
        resizable: false,
        draggable: false,
        position: ['center', 200],
        modal: true,
        close: function() { location.href = "/FormFillier/index.aspx"; }
    });

    document.getElementById("dialogModalP").innerHTML = "You haven't permisson to compile this form.<br/><br/> You will be redirect at home page";
    slideRightToLeft();


});

//choose the type of dialog login
function setType(t, list) {
    type = t;
    var service = list.split("|");
    var serviceOption = document.getElementById("selLog");
    
    for (j = 0; j < service.length - 1; j++) {
        var op = document.createElement("option");
        var opText = document.createTextNode(service[j]);
        op.appendChild(opText);
        op.value = service[j];
        serviceOption.appendChild(op);
    }

}

//response about dialog
function fakeLogResult(arg, context) {
    var t = arg.split("|");
    if (t[0] != "ok")
        alert(t[0]);
    location.href = t[1];


}
//animation scroll right to left
function slideRightToLeft() {
    var presenPanel = $("#ctl00_contentHome_presenPanel");
    var figli = presenPanel.children();
    if (figli.length == 0) {
        if (type == -1) {
            $("#dialogLog").dialog('open', {});
        } else {

            $(function() {
                $("#dialogModal").dialog(
                            'open',
                                {}
                            );
            });
        }
        type = 0;
    } else {
        if ((figli.length > 0)) {
            $(presenPanel).animate({ left: "0px", width: "100%", opacity: "toggle" }, 1000);
        }
    }
}

//Overwritten to read function is called when the validation of the page and submit before the callback
function Page_ClientValidate(validationGroup) {
    Page_InvalidControlToBeFocused = null;
    if (typeof (Page_Validators) == "undefined") {
        return true;
    }
    var i;
    validatePage = true;
    for (i = 0; i < Page_Validators.length; i++) {

        var controlID = Page_Validators[i].controltovalidate;
        if (!controlID.startsWith("ctl00_contentHome")) {

            var control = document.getElementById(controlID);
            var parent = control.parentNode;
            if ((parent.style.display != "none") & (parent.style.display != "")) {

                ValidatorValidate(Page_Validators[i], validationGroup, null);
            }
            else {
                Page_Validators[i].isvalid = true;
            }
        }
        else {
            ValidatorValidate(Page_Validators[i], validationGroup, null);
        }
    }
    validatePage = false;
    validateDialog = false;
    ValidatorUpdateIsValid();
    ValidationSummaryOnSubmit(validationGroup);
    Page_BlockSubmit = !Page_IsValid;
    return Page_IsValid;
};




function ValidatorUpdateDisplay(val) {

    var controlID = val.controltovalidate;
    if (!controlID.startsWith("ctl00_contentHome")) {
        ValidatorUpdateDisplayDefault(val);
    } else {
        if (!validateDialog) {
            var control = document.getElementById(controlID);
            var p = $(control.parentNode); //content-container");
            var position = p.offset();

            if ((typeof (control) != "undefined") && (control != null)) {
                if (val.isvalid == false) {
                    if (!validatePage) {

                        var posx = position.left + control.offsetLeft + control.offsetWidth + 10;
                        var posy = position.top + control.offsetTop;
                        document.getElementById("dialog").firstChild.nodeValue = val.errormessage;
                        $.popup.show('Error: ', val.errormessage, 'body', posx, posy);
                    } else {
                        var cWidth = document.documentElement.clientWidth;
                        var cHeight = document.documentElement.clientHeight;
                        $.popup.show('Error: ', 'One or more fields are not valid', 'body', (cWidth - 350) / 2, (cHeight + 100) / 2);
                        $(function() {
                            $(control).effect(
                        'pulsate',
                        { times: 5 },
                        300
                    );
                        });
                        val.style.visibility = "visible";
                    }
                } else {
                    val.style.visibility = "hidden";
                }
            }
        }
    }
};


//save the filled workflow
function save_Click() {

    var save = document.getElementById("saveDialog");
    $(save).dialog({
        bgiframe: true,
        resizable: false,
        height: 140,
        modal: true,
        draggable: false,
        show: 'slide',
        hide: 'slide',
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        buttons: {
            'SAVE': function() {
                CallServer('Calling From Button', null);
                $(save).dialog('close');
            },
            Cancel: function() {
                $(save).dialog('close');
            }
        }
    });
    $(save).dialog('open');

}


function ReceiveServerData(arg, context) {
    location.href = "/FormFillier/index.aspx";
}

startsWith = function(str) {
    return (this.match("^" + str) == str)
}