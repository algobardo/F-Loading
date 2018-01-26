/// <reference path="../jshashtable.js" />
/// <reference path="../../lib/js/jquery-1.3.2.min.js" />
/// <reference path="../../lib/js/jquery-ui-1.7.1.custom.min.js" />
/// <reference path="../../../css/FormFiller/contentContacts.css" />

var sessionhash = new Hashtable();

/* --------------------------------- SERVICES --------------------------------- */
$(function() {
	$("#dialog").dialog({
		bgiframe: true,
		autoOpen: false,
		height: 175,
		modal: true,
		buttons: {
			'Choose': function() {
                        ImportContacts();
                        $(this).dialog('close');                   
			},
			Cancel: function() {
			$(this).dialog('close');
			}
		},close: function() {
		}
	});
			
	$('#download_contacts').click(function() {
		GetServices();
		$('#dialog').dialog('open');
	})
	.hover(
		function(){ 
			$(this).addClass("ui-state-hover"); 
		},
		function(){ 
			$(this).removeClass("ui-state-hover"); 
		}
	).mousedown(function(){
		$(this).addClass("ui-state-active"); 
	})
	.mouseup(function(){
			$(this).removeClass("ui-state-active");
	});

});

/* --------------------------------- SAVE CONTACTS ON DB --------------------------------- */	

$(function() {
	$("#contactfirst").dialog({
		bgiframe: true,
		autoOpen: false,
		height: 300,
		modal: true,
		buttons: {
			'Import contacts': function() {
                    AddContactsInGroup();                      
                    $(this).dialog('close');
			},
			Cancel: function() {
				$(this).dialog('close');
			}
		},close: function() {
		}
	});		
});
	
/* --------------------------------- REMOVE GROUP --------------------------------- */	
	
$(function() {	    
	$("#choiseGroup").dialog({
		bgiframe: true,
		autoOpen: false,
		height: 220,
		modal: true,
		buttons: {
			'Yes': function() {
                    var remObject = document.getElementById('remObject');
                    var ch = document.getElementById('rem_perm');
                    var toremove = remObject.title + "β";
                    if (ch.checked) toremove += "true";
                    else toremove += "false";
                    RemoveGroup(toremove);                      
                    $(this).dialog('close');
			},
			Cancel: function() {
				$(this).dialog('close');
			}
		},close: function() {
		}
	});		
	
});	
	
/* --------------------------------- CREATE GROUP --------------------------------- */	

$(function() {
	$("#selectGroups").dialog({
		bgiframe: true,
		autoOpen: false,
		height: 330,
		modal: true,
		buttons: {
			'Create': function() {
			        CreateGroup();
                    $(this).dialog('close');
			},
			Cancel: function() {
				$(this).dialog('close');
			}
		},close: function() {
		}
	});	
	
	$('#create_group').click(function() {
        allContacts();
		$('#selectGroups').dialog('open');
	})
	.hover(
		function(){ 
			$(this).addClass("ui-state-hover"); 
		},
		function(){ 
			$(this).removeClass("ui-state-hover"); 
		}
	).mousedown(function(){
		$(this).addClass("ui-state-active"); 
	})
	.mouseup(function(){
			$(this).removeClass("ui-state-active");
	});	
});

$(function() {
	$("#renameGroup").dialog({
		bgiframe: true,
		autoOpen: false,
		height: 220,
		modal: true,
		buttons: {
			'Rename': function() {
			           var s = document.getElementById('renGroup');
                       var text = document.getElementById('newnametext');
                       if (s.title == "OtherContacts" || text.value == "OtherContacts"){
                            $(this).dialog('close'); 
                            errorF("Impossible rename OtherContacts");
                       } else if (s.title == text.value || text.value == ""){
                            $(this).dialog('close'); 
                            errorF("Invalid group name");
                       } else {
                            RenameGroup(s.title + 'β' + text.value); 
                            $(this).dialog('close'); 
                       }
			},
			Cancel: function() {
				$(this).dialog('close');
			}
		},close: function() {
		}
	});			
});
	
	/* --------------------------------- REMOVE CONTACTS --------------------------------- */	
	
$(function() {
	$("#removeContacts").dialog({
		bgiframe: true,
		autoOpen: false,
		height: 300,
		modal: true,
		buttons: {
			'Remove': function() {
                    removeContacts();
                  $(this).dialog('close');
			},
			Cancel: function() {
				$(this).dialog('close');
			}
		},close: function() {
		}
	});		
});
	
/* --------------------------------- MOVE CONTACTS --------------------------------- */	

$(function() {
	$("#moveContacts").dialog({
		bgiframe: true,
		autoOpen: false,
		height: 330,
		modal: true,
		buttons: {
			'Move': function() {
                  moveContacts();
                  $(this).dialog('close');
			},
			Cancel: function() {
				$(this).dialog('close');
			}
		},close: function() {
		}
	});			
});

/* --------------------------------- ERRORDIALOG --------------------------------- */	

$(function() {
    $("#error").dialog({
        bgiframe: true,
        show: "clip",
        modal: true,
        zIndex: 1800,
        position: ['center', 'center'],
        autoOpen: false,
        height: 170,
        modal: true,
        buttons: {
            'Ok': function() {
                spinnerStop(false);
                $(this).dialog('close');
            },
			Cancel: function() {
				$(this).dialog('close');
			}
		},close: function() {
		}
	});		
});

/* --------------------------------- RenameGroup functions --------------------------------- */	

function RenameGroup(oldnew)
{
    var content = document.getElementById('content-border');
    spinnerStart(content,"spinnerBackgroundCustom");
    RenameGroupCall(oldnew,"");
}
                   
function RenameGroupResponse(val)
{
    var r = val.split('β');
    if (r[0]=="true"){
        var x = sessionhash.get(r[1]);
        if (x!=null){
            sessionhash.remove(r[1]);
            sessionhash.put(r[2],x);
        } 
    }
    else if (r[0]=="unr"){
        errorF("You are not registered");
    }
    else
        errorF("Operation failed");
    readSession();
}

/* --------------------------------- ImportContacts functions --------------------------------- */

function ImportContacts()
{
    var s = document.getElementById('selserv');
    s.setAttribute("style","visibility:visible;");
    var logneded = s.options[s.selectedIndex].value;
    var service = s.options[s.selectedIndex].title;
    if (logneded == "False"){
        ImportContactsCall(service,"");
        $('#contactfirst').dialog('open');
    } else {
        errorF("You need to login before importing contacts from " + service);
        $('#dialog').dialog('close');
    }
}
                   
function ImportContactsResponse(val)
{     
      if (val != "false" && val != "unr"){
          var contacts = new Array();
          contacts = val.split('β');
          var contatti = document.getElementById('tuttiicontatti');
         
          var imageLoading1 = document.getElementById('imageLoading1');
          contatti.removeChild(imageLoading1);
         
          if (contacts[0] == "true")  {
            if (contacts.length > 1){
                 for (var i=1; i<contacts.length; i++) {
                      var con = contacts[i].split('γ');
                      var check = document.createElement("input");
                      var label = document.createElement("label");
                      var br = document.createElement("br");
                      check.setAttribute("type", "checkbox");   
                      check.setAttribute("value", con[1]+"γ"+con[2]);
                      check.setAttribute("name", con[0]);  
                      check.setAttribute("checked","checked");                  
                      label.innerHTML = con[0];
                      contatti.appendChild(check);
                      contatti.appendChild(label);
                      contatti.appendChild(br);
                 }
             } 
          } 
    }else { 
        if (val == "unr"){
            errorF("You are not registered");
        } else {
            errorF("No contacts found");
            $('#contactfirst').dialog('close');
        }
    }
}    

/* --------------------------------- AddContactsInGroup functions --------------------------------- */

function AddContactsInGroup(){
    var content = document.getElementById('content-border');
    spinnerStart(content,"spinnerBackgroundCustom");
    var contacts = "";
    var f = document.getElementById('tuttiicontatti');
    var ser = f.childNodes;

    for (var i=2; i<ser.length; i+=3){
           var check = ser.item(i);
           if (i == 2){        
                var getid = (check.value).split('γ');
                contacts = getid[1];
           }
             if (check.checked) {
                var label = ser.item(i+1);
                contacts += "β" + $(label).text() + 'γ' + check.value;
                } 
    }
    AddContactsInGroupCall(contacts,"");
}
                   
function AddContactsInGroupResponse(val){
    if (val == "false") 
        errorF("Operation failed");
    if (val == "unr") 
        errorF("You are not registered");
        spinnerStop(false);
    GetGroupsAndContacts();
}    

/* -------------------------------- CreateGroup functions --------------------------------- */

function CreateGroup(){   
    var content = document.getElementById('content-border');
    spinnerStart(content,"spinnerBackgroundCustom");
    var val = document.getElementById('newNameGroup');
    if (val != null && val.value != ""  && val.value != "OtherContacts" && !sessionhash.containsKey(val.value)){
            var contacts = val.value;
            var f = document.getElementById('allconts');
            var ser = f.childNodes;
            for (var i=1; i<ser.length; i+=3){
               var check = ser.item(i-1);
               if (check.checked) {
                    var label = ser.item(i);
                    contacts += "β" + $(label).text() + 'γ' + check.value;                        
               } 
            }
            CreateGroupCall(contacts,"");
    } 
    else 
        errorF("The group name is not valid");
}
                   
function CreateGroupResponse(val){
    var splitt = val.split('β');
    if (splitt != null && splitt.length > 0){         
        if (splitt[0] == "true"){
            var hs = new Hashtable();        
            var o = sessionhash.get("OtherContacts");
            var group = splitt[1];
            for (var i = 2; i < splitt.length; i++){
                 var co = splitt[i].split('γ');  
                 if (!isOrphan(co[1]+"γ"+co[2])){
                      o.remove(co[1]+"γ"+co[2]);
                 }
                 hs.put(co[1]+"γ"+co[2],co[0]);
            }
           sessionhash.put(group,hs);
           readSession();   
        } 
        else if (splitt[0] == "unr") 
            errorF("You are not registered");
        else 
            errorF ("Operation failed");
    }
}    

/* --------------------------------- GetServices functions --------------------------------- */

function GetServices()
{
    GetServicesCall("","");
}
                   
function GetServicesResponse(val)
{   
    
    var servizi = document.getElementById('tuttiiservizi');
    var imageLoading = document.getElementById('imageLoading');
    servizi.removeChild(imageLoading);
    services = val.split('β');
    
    var selections = document.getElementById('selserv');
    selections.setAttribute("style","visibility:visible;");
    if (services[0] == "true"){
        for (var i = 1; i < services.length; i++){
        var ser = services[i].split('γ');
            var row = document.createElement("option");
            row.setAttribute("label",ser[0]);
            row.setAttribute("title",ser[0]);
            row.setAttribute("value",ser[1]);
            row.innerHTML = ser[0] + "<br />";
            selections.appendChild(row);
        }
    } 
    else if (services[0] == "unr") 
        errorF("You are not registered");
    else
        errorF ("Operation failed");   
}

        function returnBack(){
            UrlRet();
        }

        function UrlRet()
        {
            UrlRetCall("","");
        }
                           
        function UrlRetResponse(val)
        {   
            location.href = val;
        }


/* --------------------------------- GetGroupsAndContacts functions --------------------------------- */

    function GetGroupsAndContacts()
    {
        var content = document.getElementById('content-border');
        spinnerStart(content,"spinnerBackgroundCustom");
        GetGroupsAndContactsCall("","");
    }

    function GetGroupsAndContactsResponse(val)
    {
        writeSession(val);
    }

/* --------------------------------- session functions --------------------------------- */

function writeSession(val)
{
    if (val!=null && val!="false"){
        var groups = val.split('α');
        if (groups[0] == "true"){
            for (var i = 1; i < groups.length; i++){
                var contacts = groups[i].split('β');
                var groupname = contacts[0]; 

                var toinsert = new Hashtable();
                for (var j = 1; j < contacts.length; j++) {
                    var splitted = contacts[j].split('γ')                 
                    toinsert.put(splitted[1]+"γ"+splitted[2],splitted[0]);
                }

                if (sessionhash != null)  {
                    sessionhash.put(groupname,toinsert);
                } else errorF("Operation failed");
            }    
            readSession();
        }
        if (groups[0] == "unr") {
                spinnerStop(false);
                errorF("You are not registered","true");
            }
        if (groups[0] == "false") {
                spinnerStop(false);
                GetServices();
	            $('#dialog').dialog('open');
            }
    }
}

//Method to create accordion and contacts download from session
function readSession()
{
    var slider = document.getElementById('slider');
    var groups = sessionhash.keys();
    var acco = document.getElementById('accordion');
    acco.innerHTML = "";
   
    for (var i = 0; i < groups.length; i++){
       var contactkeysbis = sessionhash.get(groups[i]);
       var keysbis = contactkeysbis.keys();
       var h3 = document.createElement("h3");
       h3.setAttribute("onclick","setHideGroup('"+groups[i]+"')");
       
       var rename = document.createElement("input");
       rename.setAttribute("type","button");
       rename.setAttribute("class","accordion_button_rename");
       rename.setAttribute("title","Rename "+groups[i]);
       rename.setAttribute("id",groups[i]+"Rename");
       rename.setAttribute("onclick","openDialogRename('"+groups[i]+"')");
       
       var removeC = document.createElement("input");
       removeC.setAttribute("type","button");
       removeC.setAttribute("class","accordion_button_removeContacts");
       removeC.setAttribute("title","Remove selected contacts");
       removeC.setAttribute("id",groups[i]+"RemoveC");
       removeC.setAttribute("onclick","openDialogRemove()");
       
       var moveC = document.createElement("input");
       moveC.setAttribute("type","button");
       moveC.setAttribute("class","accordion_button_moveContacts");
       moveC.setAttribute("title","Move selected contacts");
       moveC.setAttribute("id",groups[i]+"MoveC");
       moveC.setAttribute("onclick","openDialogMove()");

       var input = document.createElement("input");
       input.setAttribute("type","button");
       input.setAttribute("id",groups[i]+"Remove");
       input.setAttribute("class","accordion_button");
       input.setAttribute("title","Remove "+groups[i]+" group");
       input.setAttribute("onclick","openDialog('"+groups[i]+"')");
       
       var checkAll = document.createElement("input");
       checkAll.setAttribute("type","button");
       checkAll.setAttribute("id",groups[i]+"Check");
       checkAll.setAttribute("class","accordion_button_checkYes");
       checkAll.setAttribute("title","Select All");
       checkAll.setAttribute("name","false");
       checkAll.setAttribute("onclick","selectAllContacts(1)");
       
       var checkAllNo = document.createElement("input");
       checkAllNo.setAttribute("type","button");
       checkAllNo.setAttribute("id",groups[i]+"CheckNo");
       checkAllNo.setAttribute("class","accordion_button_checkNo");
       checkAllNo.setAttribute("title","Deselect All");
              
       var a = document.createElement("a");
       a.setAttribute("style","font-size:11px;");
       
       var div = document.createElement("div");
       div.setAttribute("id",groups[i]);       
       div.setAttribute("style","height:60px; overflow:hidden;");
       div.setAttribute("onmouseover","scrollGroup1()");
       div.setAttribute("onmouseout","scrollGroup()");
       a.appendChild(document.createTextNode(groups[i] + " ("+keysbis.length+")"));
       
       a.appendChild(input); 
       a.appendChild(rename);
       a.appendChild(removeC);
       a.appendChild(moveC);
       a.appendChild(checkAllNo);
       a.appendChild(checkAll);         
       
       h3.setAttribute("onMouseOver","selectOption('"+groups[i]+"')");
       h3.setAttribute("onMouseOut","selectOptionOut('"+groups[i]+"')");
       h3.appendChild(a);
         if (contactkeysbis != null){
            if (keysbis != null){
                 for (var j = 0; j < keysbis.length; j++){ 
                        var check2 = document.createElement("input");
                        var label2 = document.createElement("label");
                        var br2 = document.createElement("br");
                
                        check2.setAttribute("type", "checkbox");   
                        check2.setAttribute("value", keysbis[j]);
                        check2.setAttribute("name", contactkeysbis.get(keysbis[j]));                  
                        label2.innerHTML = contactkeysbis.get(keysbis[j]);

                        div.appendChild(check2);
                        div.appendChild(label2);
                        div.appendChild(br2);
                 }
             }
         }
         acco.appendChild(h3);
         acco.appendChild(div);
         if (i == 0)  setHideGroup(groups[i]);
        }  

        $("#accordion").accordion('destroy').accordion({ });

        if (groups.length > 7) {
            slider.setAttribute("style","visibility:visible;");
        } else {
            slider.setAttribute("style","visibility:hidden;");
            $("#accordion").animate({ scrollTop:0 }, 1000);
        }
        
        spinnerStop(false);
}

/* --------------------------------- RemoveGroup functions --------------------------------- */

function RemoveGroup(val)
{
    var content = document.getElementById('content-border');
    spinnerStart(content,"spinnerBackgroundCustom");
    var spl = val.split('β');
    if (val != null && val != "" && spl[0] != "OtherContacts" )
        RemoveGroupCall(val,"");
    else
        errorF("Impossible remove OtherContacts");
}

function RemoveGroupResponse(val)
{
    var spl = val.split('β');
    if (spl[0] == "true"){
        if (spl[1] == "true"){
            sessionhash.remove(spl[2]);
        }else{
            var upd = sessionhash.get("OtherContacts");
            var toremove = sessionhash.get(spl[2]);
            var keys = toremove.keys();
            for (i = 0; i < keys.length; i++){
                var cont = toremove.get(keys[i]);
                upd.put(keys[i],cont);
            }
            sessionhash.remove(spl[2]);
            sessionhash.put("OtherContacts",upd);
        }
    }
    else if (spl[0] == "unr")
        errorF("You are not registered");
    else 
        errorF("Operation failed");
    readSession();
}

/* ------------------------------------ RemoveContacts functions ---------------------------------------------- */

function removeContacts(){
    var content = document.getElementById('content-border');
    spinnerStart(content,"spinnerBackgroundCustom");
    var torem = document.getElementById('remConts');
    var ser = torem.childNodes;   
    var group = document.getElementById('hidegrouprem');
    var removable = group.value;
    
    for (var i=1; i < ser.length; i+=3){
        var check = ser.item(i-1);
        if (check.checked) {                               
            removable += "β" + check.name + "γ" + check.value;
        }
    }
    RemoveContacts(removable); 
}

function RemoveContacts(contacts){

    RemoveContactsCall(contacts,"");
}
                   
function RemoveContactsResponse(val){

    var r = val.split('β');
    if (r[0]=="true"){
        var g = sessionhash.get(r[1]);
        var o = sessionhash.get("OtherContacts");
            for (var i = 2; i < r.length; i++){
                var con = r[i].split("γ");
                if (r[1]!="OtherContacts"){
                    g.remove(con[1]+"γ"+con[2]);
                    if (isOrphan(con[1]+"γ"+con[2])){
                        o.put(con[1]+"γ"+con[2],con[0]);
                    }
                  } else {
                    g.remove(con[1]+"γ"+con[2]);
                  }
            }
               sessionhash.put(r[1],g);
               sessionhash.put("OtherContacts",o);
    }
    else if (r[0]=="unr") 
        errorF("You are not registered");
    else 
        errorF("Operation failed");
    readSession();
}
    
/* ---------------------------------------------- MoveContacts functions --------------------------------- */

function MoveContacts(val)
{
     MoveContactsCall(val,"");
}

function MoveContactsResponse(val)
{
    var r = val.split('β');
    if (r[0]=="true"){
        var inn = sessionhash.get(r[1]);
        var out = sessionhash.get(r[2]);
        
        for (var i = 3; i < r.length; i++){
            var con = r[i].split("γ");
            out.remove(con[1]+"γ"+con[2]);
            if (r[1] != "OtherContacts"){
                inn.put(con[1]+"γ"+con[2],con[0]);
            } else {
            //othercontacts
                if(isOrphan(con[1]+"γ"+con[2])){
                    inn.put(con[1]+"γ"+con[2],con[0]);
                }
            }
        }
        sessionhash.put(r[1],inn);
        sessionhash.put(r[2],out);
    } 
    else if (r[0]=="unr")
        errorF("You are not registered");
    else 
        errorF("Operation failed");
    readSession();
}

function moveContacts(){
    var content = document.getElementById('content-border');
    spinnerStart(content,"spinnerBackgroundCustom");
    var torem = document.getElementById('movConts');
    var ser = torem.childNodes;   
    var group = document.getElementById('hidegroupmov');
    var drop = document.getElementById('seltomove');
    var newgroup = drop.options[drop.selectedIndex].value;  
    var movable = newgroup + "β" + group.value;
    
    for (var i=1; i<ser.length; i+=3){
        var check = ser.item(i-1);
        if (check.checked) {                               
            movable += "β" + check.name + "γ" + check.value;
        }
    }
    MoveContacts(movable);
}

/* ---------------------------------------------- utilities --------------------------------- */

//method for error message
function errorF(error,param){
        spinnerStop(false);
        var er = document.getElementById("error");
        er.innerHTML = ""; 
        if (param == "true")
        {
            var p = document.createElement("p");
            var a = document.createElement("a");
            a.setAttribute("href","Registration.aspx");
            a.appendChild(document.createTextNode("here"));
            p.appendChild(document.createTextNode("You need to register "));
            p.appendChild(a);
            p.setAttribute("style","top:50%;")
            er.appendChild(p);
        } else {
            var p = document.createElement("p");
            p.setAttribute("style","top:50%;");
            p.appendChild(document.createTextNode(error));
            er.appendChild(p);
        }
        $('#error').dialog('open'); 
    }

//check if someone belongs to a group other OtherContacts
function isOrphan(cont){
    
    var groups = sessionhash.keys();
    var i = 0;
    var contacts;
    while (i < groups.length){
        contacts = sessionhash.get(groups[i]);
        if (contacts.containsKey(cont)) return false;
        i++;
    }
    return true;
}

//fill a <select> of groups value - deprecated 
function fillgroup(val){
        if (val == "renGroup") {
         var te = document.getElementById('newnametext');
         te.value = "";
    }
        
    var check = document.getElementById('rem_perm');
    check.checked = "";
    var generic = document.getElementById(val);
    generic.innerHTML = "";
    var groups = sessionhash.keys();
    for (var i = 0; i < groups.length; i++){
        var row = document.createElement("option");
        row.setAttribute("label",groups[i]);
        row.setAttribute("title",groups[i]);
        row.setAttribute("value",groups[i]);
        row.innerHTML = groups[i] + "<br />";
        generic.appendChild(row);
    }
}

//select group value - deprecated 
function selectByGroup(){

    var groupcontrol = document.getElementById('groupies');
    var group = groupcontrol.options[groupcontrol.selectedIndex].value;       
    var contacts = sessionhash.get(group);

    var contactcontrol = document.getElementById('contactsList');
    contactcontrol.innerHTML = "";

    if (contacts != null && !contacts.isEmpty()){

        var check2 = document.createElement("input");
        var label2 = document.createElement("label");
        var br2 = document.createElement("br");
        
        var contactkeys = contacts.keys(); 
        
        for (var j = 0; j < contactkeys.length; j++){  
           
            var check2 = document.createElement("input");
            var label2 = document.createElement("label");
            var br2 = document.createElement("br");

            check2.setAttribute("type", "checkbox");   
            check2.setAttribute("value", contactkeys[j]);
            check2.setAttribute("name", contacts.get(contactkeys[j]));                  
            label2.innerHTML =  contacts.get(contactkeys[j]);
            
            contactcontrol.appendChild(check2);
            contactcontrol.appendChild(label2);
            contactcontrol.appendChild(br2);
        }
    }
}

// display all contacts on dialog called
function allContacts(){
    var temhash = new Hashtable();    
    var groupcontrol = document.getElementById('allconts');
    var tex = document.getElementById('newNameGroup');
    var child;
    while(child=groupcontrol.firstChild)
    groupcontrol.removeChild(child);
    tex.value = "";
    var groups = sessionhash.keys();    
    for (var i = 0; i < groups.length; i++){
        var contacts = sessionhash.get(groups[i]);
        if (contacts != null && !contacts.isEmpty()){
            var contactkeys = contacts.keys(); 
            for (var j = 0; j < contactkeys.length; j++){                  
                if (!temhash.containsKey(contactkeys[j])){
                    temhash.put(contactkeys[j],contacts.get(contactkeys[j]));               
                    var check2 = document.createElement("input");
                    var label2 = document.createElement("label");
                    var br2 = document.createElement("br");                
                    check2.setAttribute("type", "checkbox");   
                    check2.setAttribute("value", contactkeys[j]);
                    check2.setAttribute("name", contacts.get(contactkeys[j]));                  
                    label2.innerHTML =  contacts.get(contactkeys[j]);
                    groupcontrol.appendChild(check2);
                    groupcontrol.appendChild(label2);
                    groupcontrol.appendChild(br2);
                }
            }
        }   
     }
 }

//select contacts for remove first open dialog
function selectContacts(div,lab)
{   
    var justone = false;
    var torem = document.getElementById(div);
    var hg = document.getElementById(lab);
    var group = document.getElementById(hg.value);
    if (group.hasChildNodes){
        var ser = group.childNodes;
        torem.innerHTML = "";
        for (var i=0; i<ser.length; i+=3){
            var check = ser.item(i);
            if (check.checked) {
                justone = true;
                var label = ser.item(i+1);               
                var check2 = document.createElement("input");
                check2.setAttribute("type", "checkbox");   
                check2.setAttribute("value", check.value);
                check2.setAttribute("name", check.name); 
                check2.setAttribute("checked","checked");    
                var label2 = document.createElement("label");
                var br = document.createElement("br");
                label2.innerHTML = check.name;
                torem.appendChild(check2);
                torem.appendChild(label2);
                torem.appendChild(br);
            }
        }
    }
    return justone;
}

//select all contacts inside accordion
function selectAllContacts(val){

        var scroll = document.getElementById('hidegroupmov');
        var checkAll = document.getElementById(scroll.value+"Check");
        checkAll.setAttribute("name","true");
        var vis = document.getElementById(scroll.value);
        var ch = vis.childNodes;        
        
        for (var i=1; i<ch.length; i+=3){
           var check = ch.item(i-1);
            if (val == '1'){
                if (!check.checked) check.checked = "checked";
                } else if (val == '0') {
                        if (check.checked) check.checked = "";
            }
       }         
}

//refresh contacts cheched in ccordion
function setHideGroup(val){
    var checkAll = document.getElementById(val+"Check");
    var move = document.getElementById('hidegroupmov');
    var rem = document.getElementById('hidegrouprem');
    var divtoclear = document.getElementById(val);
    if (checkAll.getAttribute("name") == "false" ){
        var ser = divtoclear.childNodes;
                for (var i=0; i<ser.length; i+=3){
                    var check = ser[i];
                    check.checked = "";
                   }
    }
    move.value = val;
    rem.value = val; 
    checkAll.setAttribute("name","false");
       
}

//display all options on accordion
function selectOption(group){
        var rename = document.getElementById(group+"Rename");
        var remove = document.getElementById(group+"Remove");
        var move = document.getElementById(group+"MoveC");
        var removeC = document.getElementById(group+"RemoveC");
        var check = document.getElementById(group+"Check");
        var checkNo = document.getElementById(group+"CheckNo");
        rename.setAttribute("style","visibility:visible;");
        remove.setAttribute("style","visibility:visible;");
        check.setAttribute("style","visibility:visible;");
        checkNo.setAttribute("style","visibility:visible;");
        removeC.setAttribute("style","visibility:visible;");
        move.setAttribute("style","visibility:visible;");
}

//hidden all options on accordion
function selectOptionOut(group){
        var rename = document.getElementById(group+"Rename");
        var remove = document.getElementById(group+"Remove");
        var move = document.getElementById(group+"MoveC");
        var removeC = document.getElementById(group+"RemoveC");
        var check = document.getElementById(group+"Check");
        var checkNo = document.getElementById(group+"CheckNo");
        rename.setAttribute("style","visibility:hidden;");
        remove.setAttribute("style","visibility:hidden;");
        check.setAttribute("style","visibility:hidden;");
        checkNo.setAttribute("style","visibility:hidden;");
        removeC.setAttribute("style","visibility:hidden;");
        move.setAttribute("style","visibility:hidden;");
}

//method for opend dialog to choise group
function openDialog(val){
        var check = document.getElementById('rem_perm');
        var remObject = document.getElementById('remObject');
        check.checked = "";
         remObject.innerHTML = "";
         remObject.appendChild(document.createTextNode(val));
         remObject.setAttribute("title",val);
        $('#choiseGroup').dialog('open');
}

//method for opend dialog to rename group
function openDialogRename(val){
        var renGroup = document.getElementById('renGroup');
        var text = document.getElementById('newnametext');
         text.value = val;
         renGroup.innerHTML = "";
         renGroup.appendChild(document.createTextNode(val));
         renGroup.setAttribute("title",val);
        $('#renameGroup').dialog('open');
}

//method for opend dialog to remove group
function openDialogRemove(){
        if (selectContacts("remConts","hidegrouprem"))
	    $('#removeContacts').dialog('open');
	    else {
	        errorF("No contacts selected");
	        $('#removeContacts').dialog('close');
	    }
}
//method for opend dialog to move contacts
function openDialogMove(){
       if(selectContacts("movConts","hidegroupmov")){
		    fillgroup("seltomove");
	        $('#moveContacts').dialog('open');
	    }
	    else {
	        errorF("No contacts selected");
	        $('#moveContacts').dialog('close');
	    }
}

/********************************METHODS FOR SCROLL*******************************/
$(function() {
    $("#slider").slider(
        {   change: handleChange,
            slide: handleSlide,
            orientation: "vertical",
            min: -100,
            max: 0 });
});

function handleChange(e, ui) {
    var maxScroll = $("#accordion").attr("scrollHeight") - $("#accordion").height();
    $("#accordion").animate({ scrollTop: -ui.value * (maxScroll / 100) }, 1000);
}

function handleSlide(e, ui) {
    var maxScroll = $("#accordion").attr("scrollHeight") - $("#accordion").height();
    $("#accordion").attr({ scrollTop: -ui.value * (maxScroll / 100)});
}

function scrollGroup(){
        var scroll = document.getElementById('hidegroupmov');
        var vis = document.getElementById(scroll.value);
        vis.setAttribute("style","height:80px; overflow:hidden;");
}

function scrollGroup1(){
        var scroll = document.getElementById('hidegroupmov');
        var vis = document.getElementById(scroll.value);
        vis.setAttribute("style","height:80px; overflow:auto;");
}
/***************************************************************************************/