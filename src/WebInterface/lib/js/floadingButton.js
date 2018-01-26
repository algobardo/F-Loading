FLOADING_createButton = function(parent, button_id, button_text, function_to_call_on_click){
    var div = document.createElement("div");
    div.setAttribute("id",button_id);
    var left = document.createElement("div");
    var center = document.createElement("div");
    var right = document.createElement("div");
    
    center.innerHTML = button_text;
    
    if(!function_to_call_on_click)
    {
        div.setAttribute("class","floading-button-disabled");    
        left.setAttribute("class","floading-button-disabled-left");    
        center.setAttribute("class","floading-button-disabled-center");    
        right.setAttribute("class","floading-button-disabled-right");
    }
    else
    {
        div.setAttribute("class","floading-button-normal");
        div.setAttribute("onclick",function_to_call_on_click);
        left.setAttribute("class","floading-button-left");
        left.setAttribute("onmouseover","buttonOver('"+button_id+"');");
        left.setAttribute("onmouseout","buttonOut('"+button_id+"');");
        left.setAttribute("onmousedown","buttonDown('"+button_id+"');");
        left.setAttribute("onmouseup","buttonUp('"+button_id+"');");
        center.setAttribute("class","floading-button-center");
        center.setAttribute("onmouseover","buttonOver('"+button_id+"');");
        center.setAttribute("onmouseout","buttonOut('"+button_id+"');");
        center.setAttribute("onmousedown","buttonDown('"+button_id+"');");
        center.setAttribute("onmouseup","buttonUp('"+button_id+"');");
        right.setAttribute("class","floading-button-right");
        right.setAttribute("onmouseover","buttonOver('"+button_id+"');");
        right.setAttribute("onmouseout","buttonOut('"+button_id+"');");
        right.setAttribute("onmousedown","buttonDown('"+button_id+"');");
        right.setAttribute("onmouseup","buttonUp('"+button_id+"');");
    }
    
    div.appendChild(left);
    div.appendChild(center);
    div.appendChild(right);
    	
	var par = document.getElementById(parent);
	par.appendChild(div);	
}

function disableButton(button_id)
{
    var div = document.getElementById(button_id);
    div.setAttribute("class","floading-button-disabled");
    div.setAttribute("onclick","");
    var child = div.childNodes;
    child[0].setAttribute("class","floading-button-disabled-left");    
    child[0].setAttribute("onmouseover","");
    child[0].setAttribute("onmouseout","");
    child[0].setAttribute("onmousedown","");
    child[0].setAttribute("onmouseup","");
    child[1].setAttribute("class","floading-button-disabled-center");    
    child[1].setAttribute("onmouseover","");
    child[1].setAttribute("onmouseout","");
    child[1].setAttribute("onmousedown","");
    child[1].setAttribute("onmouseup","");
    child[2].setAttribute("class","floading-button-disabled-right");
    child[2].setAttribute("onmouseover","");
    child[2].setAttribute("onmouseout","");
    child[2].setAttribute("onmousedown","");
    child[2].setAttribute("onmouseup","");    
}

function enableButton(button_id, function_to_call_on_click)
{   
    var div = document.getElementById(button_id);
    div.setAttribute("class","floading-button-normal");
    div.setAttribute("onclick",function_to_call_on_click);
    var child = div.childNodes;
    child[0].setAttribute("class","floading-button-left");
    child[0].setAttribute("onmouseover","buttonOver('"+button_id+"');");
    child[0].setAttribute("onmouseout","buttonOut('"+button_id+"');");
    child[0].setAttribute("onmousedown","buttonDown('"+button_id+"');");
    child[0].setAttribute("onmouseup","buttonUp('"+button_id+"');");
    child[1].setAttribute("class","floading-button-center");
    child[1].setAttribute("onmouseover","buttonOver('"+button_id+"');");
    child[1].setAttribute("onmouseout","buttonOut('"+button_id+"');");
    child[1].setAttribute("onmousedown","buttonDown('"+button_id+"');");
    child[1].setAttribute("onmouseup","buttonUp('"+button_id+"');");
    child[2].setAttribute("class","floading-button-right");
    child[2].setAttribute("onmouseover","buttonOver('"+button_id+"');");
    child[2].setAttribute("onmouseout","buttonOut('"+button_id+"');");
    child[2].setAttribute("onmousedown","buttonDown('"+button_id+"');");
    child[2].setAttribute("onmouseup","buttonUp('"+button_id+"');");
}

function changeButtonText(button_id, text)
{
    document.getElementById(button_id).childNodes[1].innerHTML = text;   
}

function changeButtonFunction(button_id, function_to_call_on_click)
{
    document.getElementById(button_id).setAttribute("onclick",function_to_call_on_click);    
}

function deleteButton(parent, button_id) {
    var par = document.getElementById(parent);
    var btn = document.getElementById(button_id);
    par.removeChild(btn);
}

function hideButton(button_id) {
    var btn = document.getElementById(button_id);
    btn.setAttribute("style","display: none");
}

function showButton(button_id) {
    var btn = document.getElementById(button_id);
    btn.setAttribute("style","display: block");
}

function buttonDown(id) {
    var btn = document.getElementById(id);
    var left = btn.firstChild;
    while(left.nodeType!="1"){left = left.nextSibling;}
    var center = left.nextSibling;
    while(center.nodeType!="1"){center = center.nextSibling;}
    var right = center.nextSibling;
    while(right.nodeType!="1"){right = right.nextSibling;}
    left.setAttribute('class','floading-button-clicked-left');
    center.setAttribute('class','floading-button-clicked-center');
    right.setAttribute('class','floading-button-clicked-right');
}

function buttonUp(id) {
    var btn = document.getElementById(id);
    var left = btn.firstChild;
    while(left.nodeType!="1"){left = left.nextSibling;}
    var center = left.nextSibling;
    while(center.nodeType!="1"){center = center.nextSibling;}
    var right = center.nextSibling;
    while(right.nodeType!="1"){right = right.nextSibling;}
    left.setAttribute('class','floading-button-left');
    center.setAttribute('class','floading-button-center');
    right.setAttribute('class','floading-button-right');
}

function buttonOut(id) {        
    var btn = document.getElementById(id);
    var left = btn.firstChild;
    while(left.nodeType!="1"){left = left.nextSibling;}
    var center = left.nextSibling;
    while(center.nodeType!="1"){center = center.nextSibling;}
    var right = center.nextSibling;
    while(right.nodeType!="1"){right = right.nextSibling;}
    left.setAttribute('class','floading-button-left');
    center.setAttribute('class','floading-button-center');
    right.setAttribute('class','floading-button-right');
}

function buttonOver(id) {        
    var btn = document.getElementById(id);
    var left = btn.firstChild;
    while(left.nodeType!="1"){left = left.nextSibling;}
    var center = left.nextSibling;
    while(center.nodeType!="1"){center = center.nextSibling;}
    var right = center.nextSibling;
    while(right.nodeType!="1"){right = right.nextSibling;}
    left.setAttribute('class','floading-button-over-left');
    center.setAttribute('class','floading-button-over-center');
    right.setAttribute('class','floading-button-over-right');
}