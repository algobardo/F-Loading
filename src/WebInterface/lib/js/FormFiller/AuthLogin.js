//$(document).ready(function() {
//    //dialog to choose service login
//    var https = document.getElementById("httpsService");
//    $(https).addClass('dialogHttps').dialog({
//        autoOpen: false,
//        bgiframe: true,
//        modal: true,
//        draggable: false,
//        resizable: false,
//        width: 260,
//        height: 240,
//        zIndex: 1000,
//        position: ['center', 300],
//        buttons: {
//            'Ok': function() {
//                httpsLoginCall(document.getElementById("ctl00_contentHome_Username").value + "§" + document.getElementById("ctl00_contentHome_Password").value, '');
//            },
//            'Cancel': function() {
//                $(this).dialog('close');
//            }
//        },
//        open: function() {

//        }, close: function() {
//            //httpsLoginCall("_§_", '');
//            location.href = "../FormFillier/index.aspx";
//        }

//    });
//});

function openFakeMessage() {
    var https = document.getElementById("httpsService");
    $(https).addClass('dialogHttps').dialog({
        autoOpen: true,
        bgiframe: true,
        modal: true,
        draggable: false,
        resizable: false,
        width: 260,
        height: 240,
        zIndex: 1000,
        position: ['center', 300],
        buttons: {
            'Ok': function() {
                httpsLoginCall(document.getElementById("ctl00_contentHome_Username").value + "§" + document.getElementById("ctl00_contentHome_Password").value, '');
            },
            'Cancel': function() {
                $(this).dialog('close');
            }
        },
        open: function() {

        }, close: function() {
            //httpsLoginCall("_§_", '');
            location.href = "../FormFillier/index.aspx";
        }

    });
 }

//Callback response
function httpsLoginResult(arg, context) {

    var temp = arg.split("|");
    if (temp[0] == "ok") {
        location.href = temp[1];
    } else {
        alert(temp[0]);
    }

}