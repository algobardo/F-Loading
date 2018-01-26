
var WFC_control=true;
var WFC_controlList = new Array();

var WFC_ServerCommunicationEnabled = true;

function WFC_controlExecute()
{
    if(WFC_controlList.length>0)
    {
        var tmp=WFC_controlList[0];
        WFC_controlList.splice(0,1);
        tmp.call();
    }
}

function WFC_callable(functionName,parameters)
{
    var el=this;
    el.functionName=functionName;
    el.parameters=parameters;
    
    el.call=function()
    {
        el.functionName(parameters,"");
    }
}

function pageLoad() {
}

function WFC_createWorkflow(workflowId) {
    if(WFC_control)
    {
        WFC_control=false;
        WFC_createWorkflowCall(workflowId, "");
    } else
    {
        WFC_controlList.push(new WFC_callable(WFC_createWorkflowCall,workflowId));
    }
}

function WFC_createWorkflowResponse(val) {
    //debug
    if (VERBOSE_MODE)
        WFE_printErrorMessage(val, "verbose");

    WFC_control=true;
    WFC_controlExecute();
    if(val!="") WFE_printErrorMessage(val);
}

function WFC_removeArc(idWorkflow, fromNameCodified, toNameCodified) {
    if(WFC_control)
    {
        WFC_control=false;
        WFC_removeArcCall(idWorkflow + "&" + fromNameCodified + "&" + toNameCodified, "");
    } else
    {
        WFC_controlList.push(new WFC_callable(WFC_removeArcCall, idWorkflow + "&" + fromNameCodified + "&" + toNameCodified));
    }
}

function WFC_removeArcResponse(result) {
    //debug
    if (VERBOSE_MODE)
        WFE_printErrorMessage(result, "verbose");

    var args = result.split('|');
    if (args[0] == 'OK')
        ; //TODO
    else {
        //alert(args[0]);
        WFE_printErrorMessage(args[0]);
    }
    WFC_control=true;
    WFC_controlExecute();
}

function WFC_removeNode(idWorkflow,idNode,nodeNameCodified) {
    if(WFC_control)
    {
        WFC_control=false;
        WFC_removeNodeCall(idWorkflow + "&" + idNode + "&" + nodeNameCodified, "");
    } else
    {
        WFC_controlList.push(new WFC_callable(WFC_removeNodeCall, idWorkflow + "&" + idNode + "&" + nodeNameCodified));
    }
}

function WFC_removeNodeResponse(result) {
    //debug
    if (VERBOSE_MODE)
        WFE_printErrorMessage(result, "verbose");

    var args = result.split('|');
    if (args[0] == 'OK')
        ; //TODO
    else {
        //alert(args[0]);
        WFE_printErrorMessage(args[0]);
    }
    WFC_control=true;
    WFC_controlExecute();
}

function WFC_LoadXmlDocFromSession(wfID) {
    WFS_restoreInProgress = true;
    if (WFC_control) {
        WFC_control = false;  
        WFC_LoadXmlDocFromSessionCall(wfID, "");
    } else {
        WFC_controlList.push(new WFC_callable(WFC_LoadXmlDocFromSessionCall, wfID));
    }    
}

function WFC_LoadXmlDocFromSessionResponse(result) {
    //debug    
    if (VERBOSE_MODE)
        WFE_printErrorMessage(result, "verbose");

    var args = result.split('|');
    if (args[0] == 'OK') {
        WFS_RestoreWorkflow(args[1], args[2]);
        WFE_printErrorMessage("Workflow restored","verbose");
    } else {
        //alert(args[0]);
        //WFE_printErrorMessage(args[0]);
        WFE_removeWFcookie();   //Removing the wf's cookie
    }
    spinnerStop();
    WFS_restoreInProgress = false;

    WFC_control = true;
    WFC_controlExecute();
}

function WFC_saveWorkflow(args) {
    if (WFC_control) {
        spinnerStart(document.documentElement);
        var wfID = args.split('|')[0];
        WFC_control=false;
        $.cookie(wfID, null, { path: '/' });
        WFC_saveWorkflowCall(args, "");
    } else
    {
        WFC_controlList.push(new WFC_callable(WFC_saveWorkflowCall,args));
    }
}

function WFC_saveWorkflowResponse(result) {
    //debug
    if (VERBOSE_MODE)
        WFE_printErrorMessage(result, "verbose");


    spinnerStop();
    $("#dialog_saving").dialog('close'); 

    var args = result.split('|');

    switch (args[0]){
        case 'OK':
            if ($(document).find('#ThemeEditorLink').length == 0) {
            
                //cleaning menu
                WFB_cleanMenu();
                
                //adding draggable node
                WFB_addDraggableNodeButton();
                
                //adding Save Button
                WFB_addSaveButton();
                WFB_addCheckImageToButton('Save');
                WFB_status_modified = false;
                
                //adding publish buttons
                WFB_addPublishButton();

                WFB_addTutorialWorkflowButton();
                            
                //hide the new buttons and prepare to run effects on them
                WFB_prepareButtonsEffect(0);
                //run effects on buttons
                WFB_addingButtonsEffect(0);

                //alert(args[1]);
                
                //Setting a cookie to save that the wf has been saved
                $.cookie(args[2], 'saved', { path: '/' });
            }
            WFE_printErrorMessage(args[1],'success');
            break;
        case "UNREGISTERED":
            location.href = "/FormFillier/Registration.aspx";
            break;
        default: 
            //alert(args[0]);
            WFE_printErrorMessage("[SaveFormError] "+args[0]);
            break;
    }
    WFC_control=true;
    WFC_controlExecute();
}

function WFC_publishWorkflow(wfID, wfType, wfDescription, wfExpirationDate, wfCheckOption, wfSelectedServices,usingStaticFields) {
    var args = wfID+'|'+wfType+'|'+wfDescription+'|'+wfExpirationDate+'|'+wfCheckOption+'|'+wfSelectedServices+'|'+usingStaticFields;
    if (WFC_control) {
        WFE_unsetEditedThemeCookie();
        spinnerStart(document.documentElement);
    
        if(usingStaticFields == "true") WFE_setUsingStaticFieldCookie("true");
        else WFE_setUsingStaticFieldCookie("false");
        
        WFC_control = false;
        WFC_publishWorkflowCall(args, "");
    } else {
        WFC_controlList.push(new WFC_callable(WFC_publishWorkflowCall, args));
    }
}

function WFC_publishWorkflowResponse(result) {
    //debug
    if (VERBOSE_MODE)
        WFE_printErrorMessage(result, "verbose");

    spinnerStop();
    $("#dialog_publishing").dialog('close');

    var args = result.split('|');   

     if (args[0] == 'OK') {
        //cleaning menu
        WFB_cleanMenu();

        //adding draggable node
        WFB_addDraggableNodeButton();

        //adding Save Button
        WFB_addSaveButton();
        WFB_addCheckImageToButton('Save');

        //adding publish buttons
        //TODO: add warning if workflow was already published => must rename workflow
        WFB_addPublishButton();
        WFB_addCheckImageToButton('Publish');

        //adding link to theme editor
        WFB_addThemeEditorButton();

        //adding link to send to contacts
        if( args[1] == 'private' )
            WFB_addSendToContactsButton();

        WFB_addTutorialWorkflowButton();

        //hide the new buttons and prepare to run effects on them
        WFB_prepareButtonsEffect(1);
        //run effects on buttons
        WFB_addingButtonsEffect(1);

        //Setting a cookie to save that wf has been published
        $.cookie(args[3], args[1], { path: '/' });
        
        WFE_printErrorMessage(args[2], 'success');
        if (WFPublishGlobalTypeToSendEmail == "private") {
            //alert("ciao");
            getUserGCall(-200, '');
            clickContactUser();
        }
    }
    else {
        //alert(args[0]);
        WFE_printErrorMessage("[PublishFormError] "+args[0]);
    }
    WFC_control = true;
    WFC_controlExecute();
}

function WFC_addNode(nodeID, nodeNameCodified, nodeXml) {
    var idWorkflow = WFG_workflow.getId();
    
    if(WFC_control)
    {
        WFC_control=false;
        WFC_addNodeCall(idWorkflow + "&" + nodeID + "&" + nodeNameCodified + "&" + nodeXml, "");
    } else
    {
        WFC_controlList.push(new WFC_callable(WFC_addNodeCall,idWorkflow + "&" + nodeID + "&" + nodeNameCodified + "&" + nodeXml));
    }
}

function WFC_addNodeResponse(result) {
    //debug
    if (VERBOSE_MODE)
        WFE_printErrorMessage(result, "verbose");

    var args = result.split('|'); 
    
    if (args[0] == 'OK')
        ;//WFE_printErrorMessage("Node added","verbose");//TODO
    else {
        //alert(args[0]);
        //WFE_printErrorMessage("[AddStepError] "+args[0]);
    }

    WFC_control=true;
    WFC_controlExecute();
}

function WFC_syncNode(nodeID, nodeNameCodified, nodeXml) {
    var idWorkflow = WFG_workflow.getId();
    
    if(WFC_control)
    {
        WFC_control = false;
        WFC_syncNodeCall(idWorkflow + "&" + nodeID + "&" + nodeNameCodified +"&" + nodeXml, "");
    } else
    {
        WFC_controlList.push(new WFC_callable(WFC_syncNodeCall,idWorkflow + "&" + nodeID + "&" + nodeNameCodified +"&" + nodeXml));
    }
}

function WFC_syncNodeResponse(result) {
    //debug
    if (VERBOSE_MODE)
        WFE_printErrorMessage(result, "verbose");

    var args = result.split('|'); 
    
    if (args[0] == 'OK')
        ; //TODO
    else {
        //alert(args[0]);
        WFE_printErrorMessage("[SyncStepError] "+args[0]);
    }

    WFC_control=true;
    WFC_controlExecute();
}

// eventArgument = workflowId from client & node id from & node id to & xml predicate
function WFC_addArc(fromNameCodified, toNameCodified, edgeXML) {
    var idWorkflow = WFG_workflow.getId();
    
    if(WFC_control)
    {
        WFC_control=false;
        WFC_addArcCall(idWorkflow + "&" + fromNameCodified + "&" + toNameCodified + "&" + edgeXML, "");
    } else
    {
        WFC_controlList.push(new WFC_callable(WFC_addArcCall, idWorkflow + "&" + fromNameCodified + "&" + toNameCodified + "&" + edgeXML));
    }
}

function WFC_addArcResponse(result) {
    //debug
    if (VERBOSE_MODE)
        WFE_printErrorMessage(result, "verbose");

    var args = result.split('|'); 
    
    if (args[0] == 'OK')
        ; //TODO
    else{
        //alert(args[0]);

        WFE_printErrorMessage(args[0]);
        
//        if( args[0] == "Workflow contains cycle" )
//            edgeToInsert.remove();
    }

    WFC_control=true;
    WFC_controlExecute();
}

//TO-CHANGE-ADDFIELD
function WFC_addField(field_list) {    
    if(WFC_control)
    {
        WFC_control=false;
        WFC_addFieldCall(field_list, "");
    } else
    {
        WFC_controlList.push(new WFC_callable(WFC_addFieldCall,field_list));
    }
}

function WFC_addFieldResponse(fieldTypes) {
    //debug
    if (VERBOSE_MODE)
        WFE_printErrorMessage("[addField] "+fieldTypes, "verbose");

    WFC_control = true;
    WFC_controlExecute();
    
    var error_message = fieldTypes.split('|');
    if(error_message[0]=="error")WFE_printErrorMessage("[AddFieldError] "+error_message[1]);
    
    WFS_AddFieldTypes(WFG_workflow.getId(), fieldTypes);
}

/*
function WFC_findPredicate(fromId, toId) {
    var idWorkflow = WFG_workflow.getId();
    WFC_findPredicateCall(idWorkflow + "&" + fromId + "&" + toId, "");
}

function WFC_findPredicateResponse(val) {
    //debug
    if (VERBOSE_MODE) 
        WFE_printErrorMessage(val, "verbose");

    WFC_control=true;
    WFC_controlExecute();
    
    WFE_pred = val;
    //alert("Predicato: "+WFE_pred);
    var predArr = (WFE_pred + "").split("|");
    //alert("WFE_openDialog: "+WFE_pred);
    $("#field1").find("#" + predArr[0]).attr("selected", "selected");
    $("#operation").find("#" + predArr[1]).attr("selected", "selected");
    $("#field2").find("#" + predArr[2]).attr("selected", "selected");
    $("#comparison").find("#" + predArr[3]).attr("selected", "selected");
    $("#comparisonVal").attr("value", predArr[4]);
}
*/

/*
function WFE_retrievePredicates(type) {
    WFE_retrievePredicatesCall(type);
}

function WFE_retrievePredicatesResponse(val) {
    //debug
    if (VERBOSE_MODE) 
        WFE_printErrorMessage(val, "verbose");

    WFC_control=true;
    WFC_controlExecute();
    
    WFE_Ope = val;
    //alert("WFE_Ope: " + WFE_Ope);
    WFE_printErrorMessage("WFE_Ope: " + WFE_Ope);
}
*/

function WFC_getPredicates(type) {    
    if(WFC_control)
    {
        WFC_control=false;
        WFC_getPredicatesCall(type,"");
    } else
    {
        WFC_controlList.push(new WFC_callable(WFC_getPredicatesCall,type));
    }
}

function WFC_getPredicatesResponse(predicates) 
{
    //debug
    if (VERBOSE_MODE)
        WFE_printErrorMessage(predicates, "verbose");

    WFF_receivePredicate(predicates);   
    
    WFC_control=true;
    WFC_controlExecute();
}

function WFC_getOperations(type)
{
    WFC_getOperationsCall(type,"");
   
    if(WFC_control)
    {
        WFC_control=false;
        WFC_getOperationsCall(type,"");
    } else
    {
        WFC_controlList.push(new WFC_callable(WFC_getOperationsCall,type));
    }
}

function WFC_getOperationsResponse(result)
{
    //debug
    if (VERBOSE_MODE)
        WFE_printErrorMessage(result, "verbose");

    WFC_control = true;
    WFC_controlExecute();
    
    WFF_receiveOperation(result);    
}

/*
//args !??!?
function WFC_setProperties()
{
    if(WFC_control)
    {
        WFC_control=false;
        WFC_setPropertiesCall("","");
    } else
    {
        WFC_controlList.push(new WFC_callable(WFC_setPropertiesCall,""));
    }
}

function WFC_setPropertiesResponse(result)
{   
    //debug
    if (VERBOSE_MODE) 
        WFE_printErrorMessage(result, "verbose");

    WFC_control=true;
    WFC_controlExecute();
}
*/

function WFF_getConstraints(type)
{
    if(WFC_control)
    {
        WFC_control=false;
        WFF_getConstraintsCall(type,"");
    } else
    {
        WFC_controlList.push(new WFC_callable(WFF_getConstraintsCall,type));
    }
}

function WFF_getConstraintsResponse(result)
{
    //debug
    if (VERBOSE_MODE)
        WFE_printErrorMessage(result, "verbose");

    WFF_constraints = result;
    WFF_callbackFunctConstr(); 
    
    WFC_control=true;
    WFC_controlExecute();
}

function WFF_saveConstraints(constr)
{
    if(WFC_control)
    {
        WFC_control=false;
        WFF_saveConstraintsCall(constr);
    } else
    {
        WFC_controlList.push(new WFC_callable(WFF_saveConstraintsCall,constr));
    }
    
}

function WFF_saveConstraintsResponse(result)
{
    //debug
    if (VERBOSE_MODE)
        WFE_printErrorMessage(result, "verbose");

    if (result != "") {
        WFE_printErrorMessage(result, null, "status_error_constr");
        //alert(result);
    } else {
        WFF_addConstraint();
    }
    
    WFC_control=true;
    WFC_controlExecute();
}

WFC_getAuthServiceList = function() {
    if(WFC_control)
    {
        WFC_control=false;
        WFC_getAuthServiceListCall("");
    } else
    {
        WFC_controlList.push(new WFC_callable(WFC_getAuthServiceListCall,""));
    }
}

WFC_getAuthServiceListResponse = function(result) {
    //debug
    if (VERBOSE_MODE)
        WFE_printErrorMessage(result, "verbose");
    
    var status = result.split('|');
    if (status[0] == "ERROR") {
        WFE_printErrorMessage("[ServicesListError] "+status[1]);
        //alert(result);
    } else {
        WFE_buildServiceList(status[1]);
    }
    
    WFC_control=true;
    WFC_controlExecute();
}