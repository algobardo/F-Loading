var logged;

$(document).ready(function() {
    helper();
});

var first = 0;


function costruisci(swtch) {
    var divStat = document.getElementById("StatC");
    if (divStat.hasChildNodes()) {
        divStat.innerHTML = "";
        first = 0;
    }
    if (parseInt(swtch) == 1 && first == 0) {
        var raiL = document.createElement("label");
        raiL.setAttribute("style", "font-size:24px");
        raiL.innerHTML = "Rai";
        var divRai = document.createElement("div");
        divRai.setAttribute("style", "height:20px;width:0px;background-color:#7FFFD4");
        var mediaL = document.createElement("label");
        mediaL.setAttribute("style", "font-size:24px");
        mediaL.innerHTML = "Mediaset";
        var divMedia = document.createElement("div");
        divMedia.setAttribute("style", "height:20px;width:0px;background-color:#5F9EA0");
        divStat.appendChild(raiL);
        divStat.appendChild(divRai);
        divStat.appendChild(mediaL);
        divStat.appendChild(divMedia);
        $(divRai).animate({ width: "200px" }, 1500);
        $(divMedia).animate({ width: "50px" }, 1500);
        first++;
    }
    if (parseInt(swtch) == 2 && first == 0) {
        var siL = document.createElement("label");
        siL.setAttribute("style", "font-size:24px");
        siL.innerHTML = "Si";
        var divSi = document.createElement("div");
        divSi.setAttribute("style", "height:20px;width:0px;background-color:#7FFFD4");
        var noL = document.createElement("label");
        noL.setAttribute("style", "font-size:24px");
        noL.innerHTML = "No";
        var divNo = document.createElement("div");
        divNo.setAttribute("style", "height:20px;width:0px;background-color:#5F9EA0");
        var forseL = document.createElement("label");
        forseL.setAttribute("style", "font-size:24px");
        forseL.innerHTML = "Forse";
        var forseDiv = document.createElement("div");
        forseDiv.setAttribute("style", "height:20px;width:0px;background-color:#008B8B");
        divStat.appendChild(siL);
        divStat.appendChild(divSi);
        divStat.appendChild(noL);
        divStat.appendChild(divNo);
        divStat.appendChild(forseL);
        divStat.appendChild(forseDiv);
        $(divSi).animate({ width: "100px" }, 1500);
        $(divNo).animate({ width: "90px" }, 1500);
        $(forseDiv).animate({ width: "60px" }, 1500);
        first++;
    }
}
var c = 0;
var flag = 0;
//var timer = 0;
function pausa() {
    var elem = document.getElementById("current_bt");
    
    var pp = document.getElementById("playPause");

    if (flag == 0) {
    pp.innerHTML = '';
    pp.innerHTML = '<img id="Img1" alt="Resume" src="../lib/image/play1.png" onclick="pausa()" runat="server" />';
        flag = 1;
    }
    else {
       pp.innerHTML = '';
    pp.innerHTML = '<img id="Img1" alt="Resume" src="../lib/image/pause2.png" onclick="pausa()" runat="server" />';
       // elem.setAttribute("value", "Pause");
        flag = 0;
        helper();
    }
}
function helper() {
    var cont = document.getElementById("ctl00_contentHome_HelpContent");
    var conth = document.getElementById("helpAlone");
    var height2 = 0;
    var height3 = 0
    var inhtml = "";
    if (c == 0) {
        height2 = "60px";
        height3 = "130px";
        inhtml = "This system allow you to fill or create forms. You can try it using Editor, just press on \"New\/Edit Form\" button on the menu.<br/> You don't need to register a new account to use our system, you can use one or more of yours.";
        //inhtml = "Per creare la tua form dovrai cliccare sul bottone \"New\/Edit Form\", accedendo all'editor.";
    }
    if (c == 1) {
        height2 = "84px";
        height3 = "150px";
        inhtml = "If you are authenticated you can use any feature. You could create new forms or see, modify, publish or delete them. You can also retrieve result about it.<br /> Using our \"step-by-step\" menu.";
        //inhtml = "Alla fine del processo di creazione sarà possibile inviare la form a chi si vuole, decidendo in base ai contatti associati al servizio nel quale ti sarai loggato.";
    }
    if (c == 2) {
        height2 = "84px";
        height3 = "130px";
        inhtml = "When you have created a form you could decide its 'form-type' and who can fill it (some contact, some group of contact or all contact). <br/> You could permit anonymous or single filling.";
        //inhtml = "Eseguire il login non è obbligatorio, ma se non ti autenticherai non potrai consultare le statistiche sulle form fatte da te.";
    }
    if (c == 3) {
        height2 = "60px";
        height3 = "130px";
        inhtml = "If you don't want to have an authentication on floading server, you can only try our editor.<br/> In both case you can fill all public forms with our \"filling-assistant\" that will help you about required field or more others validation checks.";

    }
    if (c == 4) {
        inhtml = "For a sample see the video above explaining you how to use the interface and it shows the creation of a simple form.<br/>For further information contact Loa-Group 2009 at floading2009@gmail.com.";
    }
    if (flag == 0) {
        $(cont).fadeOut(1000, function() {
            cont.innerHTML = inhtml;
        });
        c++;
        if (c > 4)
            c = 0;
        $(cont).fadeIn(1000, function() {helper();}).wait();
    }
}
$.fn.wait = function(time, type) {
    time = time || 6500;
    type = type || "fx";
    return this.queue(type, function() {
        var self = this;
        setTimeout(function() {
            $(self).dequeue();
        }, time);
    });
};

function wait(msecs) {
    var start = new Date().getTime();
    var cur = start
    while (cur - start < msecs) {
        cur = new Date().getTime();
    }
} 