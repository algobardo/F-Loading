var spinnerCountTimer = 0;
var father;
var timeoutSpinnerPlaceHolder;
var idSpin = "spinnerDefaultId";
function spinnerStart(object, newid) {
    father = object;
    var bg = document.createElement("div");
    bg.setAttribute("class","spinnerBackground");
    if (newid)
        idSpin = newid;
    bg.setAttribute("id", idSpin);
    
    bg.style.display = "block";
    bg.style.position = "absolute";
    
    bg.style.width = object.offsetWidth + "px";
    bg.style.height = object.offsetHeight + "px";
   
    var curleft = curtop = 0;
    if (object.offsetParent) {
        curleft = object.offsetLeft
        curtop = object.offsetTop
        while (object = object.offsetParent) {
            curleft += object.offsetLeft
            curtop += object.offsetTop
        }
    }
    bg.style.left = curleft;
    bg.style.top = curtop;
    
    var spinner = document.createElement("img");
    spinner.style.position = "absolute";
    spinner.setAttribute("src", "../lib/image/spinner.png");
    
    bg.appendChild(spinner);

    if (father != null && typeof father != 'undefined')
        father.appendChild(bg);
    
    posLeft = (bg.offsetWidth - spinner.offsetWidth) / 2;
    posTop = (bg.offsetHeight - spinner.offsetHeight) / 2;
    spinner.style.left = posLeft + "px";
    spinner.style.top = posTop + "px";
    
    startTimer(1);
}

function startTimer(sec) {
    if (spinnerCountTimer == 20) {
        spinnerCountTimer = 0;
        spinnerStop(true);
        alert("Not connect to server");

    } else {
        if (sec > 0) {
            spinnerCountTimer = spinnerCountTimer + 1;
            timeoutSpinnerPlaceHolder = setTimeout("startTimer(" + (sec - 1) + ");", 1000);
        }
    }
}

function spinnerStop(bool) {
    var bg = document.getElementById(idSpin);

    if (father != null && typeof father != 'undefined' && bg != null)
        father.removeChild(bg);

    if (bool) {
        $("#" + father.id).dialog('close');
    }
    clearTimeout(timeoutSpinnerPlaceHolder);
    idSpin = "spinnerBackground";
}

