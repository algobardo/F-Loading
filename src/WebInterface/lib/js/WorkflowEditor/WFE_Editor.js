//GLOBAL VARIABLES:
var VERBOSE_MODE = false;
var WFE_currentNodeSelectedID; //Global var for the active (current) node (or workflow)

var WFE_authServices = null; //Global var for Authentication services list

var WFE_special_type = { NORMAL: 'NORMAL', TEXT: 'TEXT', RADIOBUTTON: 'RADIOBUTTON', GRSEQUENCE: 'GRSEQUENCE', GRCHOICE: 'GRCHOICE', IMAGE: 'IMAGE', HTMLCODE: 'HTMLCODE' };

var WFF_droppable_id_name;
var WFE_result = new Array();

var WFE_preconditionCounter = 1;

var WFE_pred;
var WFE_Ope;

var WFE_openedTabs = 0;     //Number of opened tabs

//SECURITY
//function SEC_xmlencode(string) {
//    return string.replace(/\&/g,'&'+'amp;').replace(/</g,'&'+'lt;')
//        .replace(/>/g,'&'+'gt;').replace(/\'/g,'&'+'apos;').replace(/\"/g,'&'+'quot;');
//}

WFE_printErrorMessage = function(error_message, type, id_div) {
    var currtheme = 'default';
    if (type == "verbose" && !VERBOSE_MODE) return;
    if (type != null) {
        currtheme = type + '_message';
    }
    if (id_div == null)
        id_div = "status_error";
    $('#' + id_div).jGrowl(error_message, {
        theme: currtheme,
        open: function(e, m, o) {
            $('#' + id_div).show();
            //alert(error_message);
        },
        close: function(e, m, o) {
            $('#' + id_div).hide();
        }
    });
}

function WFE_field(id, baseType, type, nodeRelative) {
    var el = this;
    el.id = id;             //id in the editor; example: WFF_added_element_1
    el.type = type;         //example: StringBox1
    el.baseType = baseType; //example: StringBox
    el.nodeRelative = nodeRelative;
    el.fieldRelative = null;  //null means the field is top-level, otherwise it represents the father of the field
    el.sortedIndex = id;    //used to sort the fields

    //el.posx = null;
    //el.posy = null;
    el.width = null;
    el.height = null;       //the height of the field in the editor (not in the rendering!)
    el.constraints = "";    //constraints on the type of the field
    el.layout = ""          //additional layout properties to be setted in the rendering document

    el.name = "";           //the label in the editor

    //properties for the rendering document
    el.renderedLabel = "";    //the label that will be displayed in the FormFilling page
    el.useRenderedLabel = false;
    el.description = "";      //used as tooltip in mobile

    //properties to manage static images (and links?)
    el.imgSrc = null;
    el.link = null;
    
    //properties to manage static html code objects
    el.htmlSrc = null;

    //properties to manage groups and radio buttons
    el.specialType = WFE_special_type.NORMAL;
    el.children = null;

    el.isEquals = function(val) {
        return val.id == el.id;
    }

    el.getLabel = function() {
        //        var pun = $('#tab-' + el.nodeRelative.getId()).find("#" + el.id);
        //        var ret = pun.find(".WFF_static_label_box");
        //        if(ret.length == 0)
        //        {
        //            ret = pun.find(".WFF_label_box");
        //            if(ret.length!=0)return ret[0].value;
        //            else return null;
        //        }
        //        return ret[0].value;
        return el.name;
    }

    el.getLabelArray = function() {
        var toReturn = new Array();
        if (!el.isAGroup()) {
            toReturn.push(el.getLabel());
        } else {
            for (var WFE_i = 0; WFE_i < el.children.length; WFE_i++) {
                toReturn = toReturn.concat(el.children[WFE_i].getLabelArray());
            }
        }
        return toReturn;
    }

    el.getFromId = function(id) {
        if (el.id == id) {
            return el;
        } else if (el.isAGroup()) {
            for (var WFE_i = 0; WFE_i < el.children.length && toReturn == null; WFE_i++) {
                var toReturn = el.children[WFE_i].getFromId(id);
                if (toReturn != null) return toReturn;
            }
        }
        return null;
    }

    el.getFromLabel = function(label) {
        if (el.name == label) {
            return el;
        } else if (el.isAGroup()) {
            for (var WFE_i = 0; WFE_i < el.children.length && toReturn == null; WFE_i++) {
                var toReturn = el.children[WFE_i].getFromLabel(label);
                if (toReturn != null) return toReturn;
            }
        }
        return null;
    }

    el.getOfType = function(type) {
        var toReturn = new Array();
        if (el.baseType == type) {
            toReturn.push(el.getLabel());
        }
        if (el.isAGroup()) {
            for (var WFE_i = 0; WFE_i < el.children.length; WFE_i++) {
                toReturn = toReturn.concat(el.children[WFE_i].getOfType(type));
            }
        }
        return toReturn;
    }

    el.isAGroup = function() {
        return el.specialType == WFE_special_type.GRSEQUENCE || el.specialType == WFE_special_type.GRCHOICE;
    }

    el.setLabel = function(value) {
        if (value == "") {
            el.renderedLabel = value;
            el.useRenderedLabel = true;
        }
        else {
            var tmp = value.replace(/\'/g, "&apos;");
            tmp = tmp.replace(/"/g, "&quot;");
            //el.name = value;
            el.name = tmp;
        }

        $('#tab-' + el.nodeRelative.getId()).find("#" + el.id).find(".WFF_label_box")[0].value = value;
        if (!el.useRenderedLabel) el.renderedLabel = value;
    }

    el.setPositionX = function(posx) {
        el.posx = posx;
        $(document).find("#" + el.id).css("left", parseInt(posx) - $(document).find("#" + el.id)[0].childNodes[0].offsetLeft + "px");
    }

    el.setPositionY = function(posy) {
        el.posy = posy;
        $(document).find("#" + el.id).css("top", parseInt(posy) - $(document).find("#" + el.id)[0].childNodes[0].offsetLeft + "px");
    }

    //Setting field as a static label
    el.setAsStaticLabel = function(text) {
        el.specialType = WFE_special_type.TEXT;
        el.renderedLabel = text;
        el.useRenderedLabel = true;
    }

    //Setting field as a static image
    el.setAsStaticImage = function(text, img_src) {
        el.specialType = WFE_special_type.IMAGE;
        el.renderedLabel = text;
        el.useRenderedLabel = true;
        el.imgSrc = img_src;
        el.link = "#";
    }
    
    //Setting field as a static html code
    el.setAsStaticHtmlCode = function(text, code_src) {
        el.specialType = WFE_special_type.HTMLCODE;
        el.renderedLabel = text;
        el.htmlSrc = code_src;
    }

    //Setting field as a Radio Button List
    el.setAsRadioButton = function() {
        el.specialType = WFE_special_type.RADIOBUTTON;
        el.children = new Array();
    }

    //Setting field as a Sequence Group
    el.setAsSequenceGroup = function() {
        el.specialType = WFE_special_type.GRSEQUENCE;
        el.children = new Array();
    }

    //Setting field as a Choice Group
    el.setAsChoiceGroup = function() {
        el.specialType = WFE_special_type.GRCHOICE;
        el.children = new Array();
    }

    el.setGroupHeight = function(newheight) {
        if (el.specialType != WFE_special_type.GRSEQUENCE && el.specialType != WFE_special_type.GRCHOICE) {
            alert("FALSE SETGROUPHEIGHT");
            return false;
        } else {
            $('#' + el.id).find(".WFF_static_group_box").eq(0).height(newheight);
            return true;
        }
    }

    /**
    * Serializes the field
    * ATTENTION: field.name & field.renderedLabel are ENCODED
    */
    el.serialize = function() {
        switch (el.specialType) {
            case WFE_special_type.GRSEQUENCE:
            case WFE_special_type.GRCHOICE:
                var serialized_children = "";   //String to return

                if (el.children == null || el.children.length == 0)
                    return "";

                for (var i = 0; i < el.children.length - 1; i++)
                    serialized_children += el.children[i].serialize() + '&';

                //The last child
                serialized_children += el.children[el.children.length - 1].serialize();
                return serialized_children;

            case WFE_special_type.NORMAL:
            case WFE_special_type.RADIOBUTTON:
                // FieldType | FieldIdCounter | Label | Position-x | Position-y | Width | Height | RenderedLabel | Constraints | Layout //not implemented yet!
                return el.baseType + '|' + el.type.split(el.baseType)[1] + '|' + WFS_EncodeName(el.name) + '|' + el.posx + '|' + el.posy + '|' + el.width + '|' + el.height + '|' + WFS_EncodeName(el.renderedLabel) + '|' + el.constraints + '|' + el.layout;

            case WFE_special_type.TEXT:
                return "";
        }
    }

}


//Layout settings
var WFE_layout;

function WFE_modifySizeCanvas() {
    var width = 900;

    if (WFE_layout.state.west.isClosed == false && WFE_layout.state.west.isHidden == false && !(WFE_layout.state.west.isSliding == true))
        width -= WFE_layout.state.west.size;
    else
        width -= 10;

    if (WFE_layout.state.east.isClosed == false && WFE_layout.state.east.isHidden == false && !(WFE_layout.state.east.isSliding == true))
        width -= WFE_layout.state.east.size;
    else
        width -= 10;

    WFG_setSizeCanvas(width, 585);
}

var WFE_layoutSettings = {
    applyDefaultStyles: true
    , name: "WFE_layout" // NO FUNCTIONAL USE, but could be used by custom code to 'identify' a layout
    // options.defaults apply to ALL PANES - but overridden by pane-specific settings
    , defaults: {
        size: "auto"
      , paneClass: "pane" 		// default = 'ui-layout-pane'
      , resizerClass: "resizer"	// default = 'ui-layout-resizer'
      , togglerClass: "toggler"	// default = 'ui-layout-toggler'
      , buttonClass: "button"	// default = 'ui-layout-button'
      , contentSelector: ".WFE_content"	// inner div to auto-size so only it scrolls, not the entire pane!
      , contentIgnoreSelector: ".ui-layout-ignore"		// 'paneSelector' for content to 'ignore' when measuring room for content      
      , hideTogglerOnSlide: true		// hide the toggler when pane is 'slid open'
      , togglerTip_open: "Close This Pane"
      , togglerTip_closed: "Open This Pane"
      , resizerTip: "Resize This Pane"
      , fxName: "slide"		// none, slide, drop, scale
      , fxSpeed: "slow"		// slow, normal, fast, 1000, nnn
    }
    , west: {
        size: 201
      , minSize: 201
      , spacing_closed: 21			// wider space when closed
      , togglerAlign_closed: "top"		// align to top of resizer
      , togglerLength_open: 0			// NONE - using custom togglers INSIDE west-pane
      , togglerTip_open: "Close West Pane"
      , togglerTip_closed: "Open West Pane"
      , resizerTip_open: "Resize West Pane"
      , slideTrigger_open: "mouseover" 	// default
      , showOverflowOnHover: true
      , initClosed: false
      , onclose_end: function() { WFE_modifySizeCanvas(); }
      , onopen_end: function() { WFE_modifySizeCanvas(); }
      , onresize_end: function() { WFE_modifySizeCanvas(); }
    }
    , east: {
        size: 200
      , minSize: 200
      , spacing_closed: 21			// wider space when closed
      , togglerAlign_closed: "top"		// align to top of resizer
      , togglerLength_open: 0 			// NONE - using custom togglers INSIDE east-pane
      , togglerTip_open: "Close East Pane"
      , togglerTip_closed: "Open East Pane"
      , resizerTip_open: "Resize East Pane"
      , slideTrigger_open: "mouseover"
      , initClosed: true
      , onclose_end: function() { WFE_modifySizeCanvas(); }
      , onopen_end: function() { WFE_modifySizeCanvas(); }
      , onresize_end: function() { WFE_modifySizeCanvas(); }
    }
};

/**
************************************* INITIALIZATION *************************************************   
**/

function WebForm_CallbackComplete_SyncFixed() {
    // SyncFix: the original version uses "i" as global thereby resulting in javascript errors when "i" is used elsewhere in consuming pages
    for (var i = 0; i < __pendingCallbacks.length; i++) {
        callbackObject = __pendingCallbacks[i];
        if (callbackObject && callbackObject.xmlRequest && (callbackObject.xmlRequest.readyState == 4)) {
            // the callback should be executed after releasing all resources
            // associated with this request.
            // Originally if the callback gets executed here and the callback
            // routine makes another ASP.NET ajax request then the pending slots and
            // pending callbacks array gets messed up since the slot is not released
            // before the next ASP.NET request comes.
            // FIX: This statement has been moved below
            // WebForm_ExecuteCallback(callbackObject);
            if (!__pendingCallbacks[i].async) {
                __synchronousCallBackIndex = -1;
            }
            __pendingCallbacks[i] = null;

            var callbackFrameID = "__CALLBACKFRAME" + i;
            var xmlRequestFrame = document.getElementById(callbackFrameID);
            if (xmlRequestFrame) {
                xmlRequestFrame.parentNode.removeChild(xmlRequestFrame);
            }

            // SyncFix: the following statement has been moved down from above;
            WebForm_ExecuteCallback(callbackObject);
        }
    }
}

WFE_initialize = function() {
    WFE_preloadImages();
    if (typeof (WebForm_CallbackComplete) == "function") {
        // set the original version with fixed version
        WebForm_CallbackComplete = WebForm_CallbackComplete_SyncFixed;
    }

    //Adding Creat Workflow button
    WFB_addCreateWorkflowButton();
    WFB_addTutorialWorkflowButton();

    WFE_wfID = "workflow_"; //Base signature for Workflows ID
    WFE_wfCounter = 0;      //Counter of WFs active in the editor

    var WFE_editorMode = 0;     //Editor mode can be (0->WF-Editor or 1->Form-Editor)

    WFE_nodeID = "_node_";  //Base signature for nodes ID    
    WFE_currentNodeSelectedID = ""; //ID of the current node active in the Form-Editor

    var WFE_TitlesAdded = false;
    var WFE_savedPredicate = false;

    var WFE_dropListCounter = null; //Number of dropList showed in the dialog Box

    WFF_droppable_id_name = 'empty'; //name of the tab of the current form editor

    //Setting Layout    
    WFE_layout = $("#WFE-body").layout(WFE_layoutSettings);

    // save selector strings to vars so we don't have to repeat it
    // must prefix paneClass with "body > " to target ONLY the WFE_layout panes
    var westSelector = "#WFE-body > .ui-layout-west"; // outer-west pane
    var eastSelector = "#WFE-body > .ui-layout-east"; // outer-east pane

    // CREATE SPANs for pin-buttons - using a generic class as identifiers
    $("<span></span>").addClass("pin-button").prependTo(westSelector);
    $("<span></span>").addClass("pin-button").prependTo(eastSelector);
    // BIND events to pin-buttons to make them functional
    WFE_layout.addPinBtn(westSelector + " .pin-button", "west");
    WFE_layout.addPinBtn(eastSelector + " .pin-button", "east");

    // CREATE SPANs for close-buttons - using unique IDs as identifiers
    $("<span></span>").attr("id", "west-closer").prependTo(westSelector);
    $("<span></span>").attr("id", "east-closer").prependTo(eastSelector);
    // BIND layout events to close-buttons to make them functional
    WFE_layout.addCloseBtn("#west-closer", "west");
    WFE_layout.addCloseBtn("#east-closer", "east");

    //Hiding west-form-editor section (due to we are by default in WF-Editor mode)
    $(".west-form-editor").hide();
    $(".west-form-editor").removeClass('hiddenpanel');

    //building the Auth Service List for future use in Publication dialog
    WFC_getAuthServiceList();

    //setting the tutorial video dialog
    $("#tutorial_dialog").dialog({
        title: "A simple tutorial",
        autoOpen: false,
        width: 450,
        height: 400,
        show: 'slide',
        hide: 'slide'
    });

    /**
    *************************************UTILIY METHODS*************************************************   
    **/

    //@return: ID of the new Workflow
    WFE_GetNewWF_ID = function() {
        return WFE_wfID + (++WFE_wfCounter);
    }

    //@return: ID of the current selected node
    WFE_GetCurrentNodeSelectedID = function() {
        return WFE_currentNodeSelectedID;
    }

    //Toggle Editor Mode (from WF-Editor to Form-Editor or viceversa)
    var toggleEditorMode = function() {
        if (WFE_editorMode == 1) {
            $(".west-form-editor").hide();
            $(".west-wf-editor").show();
            WFE_editorMode = 0;
        } else {
            $(".west-wf-editor").hide();
            $(".west-form-editor").show();
            WFE_editorMode = 1;
        }
    }
    /**************************************** END UTILITY METHODS **************************************/



    /**
    ************************************* TABS SECTION *************************************************   
    **/
    //Defining the tabs sortability 
    //uncomment if needed
    //$("#tabs").tabs().find(".ui-tabs-nav").sortable({ axis: 'x' }); 

    //Handling tab selection event
    $("#tabs").tabs({
        select: function(event, ui) {
            var node = WFG_getNode(WFE_currentNodeSelectedID);
            WFE_layout.close('east');
            //Updating the current node selected            
            WFE_currentNodeSelectedID = $('#tabs').find('a').eq(ui.index).attr('href').split("-")[1];

            if (WFE_editorMode != 0 && node != null && WFC_ServerCommunicationEnabled) {
                WFS_AddFields(WFG_workflow.getId(), node);
            }

            //If needed a switch editor mode
            if (WFE_editorMode == 0 || ui.index == 0) {
                toggleEditorMode();
                //removing all the properties from the property box
                WFF_deselectAll();
            }
        }
    });

    //Opening a form tab
    WFE_openFormTab = function(nodeName, nodeId) {
        if ($(document).find("#tab-" + nodeId).length == 0) {

            //SECURITY:
            //nodeId = SEC_xmlencode(nodeId);

            //Creating the new-form's tab with empty content
            $("#tabs").append("<ul id=\"tab-" + nodeId + "\"  class=\"WFF_empty_content WFF_widget_edit_area\"><li id=\"WFF_empty\" style=\"margin-top: 13px;\"><span>EMPTY:<br />Move here the elements you want to add..</span></li></ul>");
            $("#tabs").tabs('add', "#tab-" + nodeId, nodeName, WFE_openedTabs++).addClass('tabFont');

            WFF_droppable_id_name = "#tab-" + nodeId;
            //set the current tab as a droppable area to drag and drop widgets within
            WFF_enable_droppable(WFG_workflow.getId(), WFE_nodeID);
        }
        $("#tabs").tabs('select', "#tab-" + nodeId);

        //change the current Node ID
        WFE_currentNodeSelectedID = nodeId;
        WFF_droppable_id_name = "#tab-" + nodeId;
    }

    WFE_removeFormTab = function(nodeName, nodeId) {
        var tab_index = -1;
        $("#tabs").find('.WFF_widget_edit_area').each(function(i) {
            if ($(this).attr('id') == ('tab-' + nodeId)) tab_index = i + 1;
        });
        $("#tabs").find('#tab-' + nodeId).remove();
        if (tab_index > 0)
            $("#tabs").tabs('remove', tab_index);
    }
    /**************************************** END TABS SECTION **************************************/




    WFE_howToPublishWorkflow = function(service) {
        var wfID = WFG_workflow.getId();
        var description = WFS_GetWorkflowDescription(wfID);
        if (description == null || description == "") description = "...write a description here";
        $("#wfDescription").val(description);
        var list = service.split("|");

        $("#serviceComm").html("");
        var serviceOption = document.getElementById("serviceComm");

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

        /*   $("#dialog_publishing").append("<div id='box'>" +
        "<div id='boxSx'>" +
        "<p style='margin:5px 5px 5px 5px;'>Choose the forms' type</p>" +
        "<select id='sele_saveWF' style=' margin: 5px 0px 0px 30px;'>" +
        "<option id='public' value='public'>Public</option>" +
        "<option id='private' value='private'>Private</option>" +
        "</select><p><textarea id='wfDescription' rows='8' cols='28'style=' margin: 5px 0px 0px 30px;'>" + description +
        "</textarea></p>" +
        "</div>" +
        "<div id='boxDx'>" +
        "<p style='margin:5px 5px 5px 5px;'>Choose a way to retrieve result</p>" +
        "<select id='serviceComm' onchange='choosen(this.value)' style=' margin: 5px 0px 0px 30px;'>" + text +
        "</select>" +
        "<br/><br/><div class='field' id='pEmail' ><span>Email</span><br/><input type='text'/></div>" +
        "<div class='field' id='pUser'><span>Username</span><br/><input type='text'/></div>" +
        "<div class='field' id='pPass'><span>Password</span><br/><input type='text'/></div>" +
        "<div class='field' id='pHost'><span>Host</span><br/><input type='text'/></div>" +
        "<div class='field' id='pDir'><span>Directory</span><br/><input type='text'/></div>" +
        "<br/></div>" +
        "</div>"
        );
        */

        var dialog_publishing = document.getElementById("dialog_publishing");
        $("#dialog_publishing").dialog({
            bgiframe: true,
            autoOpen: false,
            resizable: false,
            height: 480,
            width: 350,
            modal: true,
            show: 'slide',
            hide: 'slide',
            overlay: {
                backgroundColor: '#000',
                opacity: 0.5
            },
            buttons: {
                'Go ->': function() {
                    var formType = $(this).find("#sele_saveWF").val();
                    var descr = $(this).find("#wfDescription").val();
                    var expDate = $(this).find("#datepicker").val();
                    var selectedServices = null;
                    if (formType == "public") {
                        selectedServices = $(this).find('#seleEnabledServices').val();
                    }
                    checkOption = $(this).find('#mode').attr('checked');

                    WFS_SetWorkflowDescription(wfID, descr);
                    spinnerStart(dialog_publishing, "dialogPublishWorkflowCustom");
                    transitionPublish(formType, descr, expDate, checkOption, selectedServices);

                },
                'Cancel': function() {
                    $('#form_type').html("");
                    $(this).dialog('close');
                }
            },
            open: function(event, ui) {
                //setting the conditional droplist and checkbox
                WFE_drawPublicTypeOptions();
                WFPublishGlobalTypeToSendEmail = "";
                //setting the datepicker for the publish dialog
                var date = new Date();
                var date_string = (date.getDate() + 1) + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
                $("#expiration_date").html('<input type="text" id="datepicker" value="' + date_string + '"/>');
                $("#datepicker").datepicker({
                    zIndex: 1800,
                    altFormat: 'dd/mm/yyyy',
                    dateFormat: 'dd/mm/yy',
                    minDate: '+1d',
                    beforeShow: function(input) {
                        $("#datepicker").datepicker('enable');
                    },
                    onClose: function(dateText, inst) {
                        //alert($("#datepicker").val());
                    }
                });

            },
            close: function(event, ui) {

                //$(this).html("");

                resetPublish();
                resetBox();
            },
            zIndex: 500
        });

        $("#dialog_publishing").dialog('open');
    }

    WFE_saveWorkflow = function(wfID) {
        var description = WFS_GetWorkflowDescription(wfID);
        if (description == null || description == "") description = "...write a description here";

        $("#dialog_saving").append("<textarea id='wfDescription' rows='8' cols='30'>" + description + "</textarea>");

        $("#dialog_saving").dialog('open');

        $("#dialog_saving").dialog({
            bgiframe: true,
            resizable: false,
            height: 300,
            width: 300,
            modal: true,
            show: 'slide',
            hide: 'slide',
            overlay: {
                backgroundColor: '#000',
                opacity: 0.5
            },
            buttons: {
                'Save': function() {
                    var wfDescription = $(this).find("#wfDescription").val(); ;
                    WFS_SetWorkflowDescription(wfID, wfDescription);
                    WFC_saveWorkflow(wfID + '|' + wfDescription);
                },
                'Cancel': function() {
                    $(this).dialog('close');
                }
            },
            close: function(event, ui) {
                $(this).html("");
            }
        });
    }


    /*************************  Create Workflow session     *****************************/
    var idElement = '_Canvas';

    $('#dialog_to_set_WF_name').dialog({
        autoOpen: false,
        show: 'slide',
        width: 280,
        open: function(event, ui) {
            var today = new Date();
            var day = today.getDate() + '/';
            var month = (today.getMonth() + 1) + '/';
            var year = today.getFullYear() + ' ';
            var hour = today.getHours() + ':';
            var minutes = today.getMinutes().toString() + ':';
            var seconds = today.getSeconds().toString();

            if (seconds.length == 1) seconds = '0' + seconds;
            if (minutes.length == 2) minutes = '0' + minutes;

            var date = day + month + year + hour + minutes + seconds;
            $('#dialog_to_set_WF_name').append('<p>Form\'s Name:<br /><i>(max 30 charachters)</i><p><input maxlength="30" size="30" id="wfNameInput" type="text" value="My Form - ' + date + '"float: left; style="width margin: 20px 30px 20px 20px;" /></p></p>');
            //$("#wfNameInput").focus();
        },
        zIndex: 1000,
        buttons: {
            'OK!': function() {
                var wfID = WFE_GetNewWF_ID();
                idElement = wfID + idElement;

                var wfName = $('#dialog_to_set_WF_name').find('#wfNameInput')[0].value;

                //security input validation
                wfName = wfName.replace(/&/g, "").replace(/</g, "").replace(/>/g, "").replace(/\'/g, "").replace(/\"/g, "");

                //-------------------
                //SECURITY CONTROLS 

                //wfName = wfName.replace(/[<>]/g, "");
                //wfName = wfName.replace(/&lt/g, "");

                //wfName = wfName.replace(/&gt/g, "");
                //wfName = wfName.replace(/&lt/g, "");

                //wfName = wfName.replace(/\W/g, " ");

                //wfName = escape(wfName);

                //END SECURITY CONTROLS

                if (wfName == "" || wfName[0] == '\\') wfName = wfID;
                //alert(wfName);

                //SECURITY:
                //wfID = SEC_xmlencode(wfID);
                //wfName = SEC_xmlencode(wfName);
                //alert(wfID);

                //Creating the tab
                WFG_AddWorkflowCanvasTab(wfID, wfName, idElement);

                //Creating the document and the WF into the server
                WFS_CreateWorkflow(wfID, wfName);

                //Adding buttons
                WFB_cleanMenu();

                //Adding add node button 
                WFB_addDraggableNodeButton();

                //adding Save Button
                WFB_addSaveButton();

                //adding Tutorial Button
                WFB_addTutorialWorkflowButton();

                //Creating the WF in the graph editor
                WFG_workflowList.push(new WFG_workflow(wfID, idElement, wfName));
                WFG_workflow = WFG_workflowList[0];

                $('#dialog_to_set_WF_name').html("");
                $(this).dialog('close');
            },
            'Cancel': function() {
                $('#dialog_to_set_WF_name').html("");
                $(this).dialog('close');
            }
        }
    });


    //Dialog for the node's name
    $('#dialog_node_name').dialog({
        autoOpen: false,
        show: 'slide',
        width: 230,
        open: function(event, ui) {
            $('#dialog_node_name').append('<p>Step\'s Name: <input maxlength="40" id="nodeNameInput" type="text" value="step_' + (WFG_workflow.nodeCounter + 1) + '" style="float: left; margin: 20px 30px 20px 20px;" /></p>');
            $("#nodeNameInput").focus();
        },
        buttons: {
            'OK!': function() {
                nodeName = $('#dialog_node_name').find('#nodeNameInput')[0].value;

                //security input validation        
                nodeName = nodeName.replace(/&/g, "").replace(/</g, "").replace(/>/g, "").replace(/\'/g, "").replace(/\"/g, "");

                var nameOK = true;

                for (var i = 0; i < WFG_workflow.element.length; i++) {
                    if (WFG_workflow.element[i].getName() == nodeName) {
                        nameOK = false;
                        alert("Name already used!");
                        break;
                    }
                }

                if (nodeName == null || nodeName == "") nameOK = false;

                if (nameOK) {
                    WFG_workflow.addNode(nodeName, WFG_top, WFG_left);

                    $('#dialog_node_name').html("");
                    $(this).dialog('close');
                }

            },
            'Cancel': function() {
                $('#dialog_node_name').html("");
                $(this).dialog('close');
            }
        }
    });


    //Dialog for the image's src
    $('#dialog_image_src').dialog({
        autoOpen: false,
        modal: true,
        show: 'slide',
        width: 330,
        open: function(event, ui) {
            $('#dialog_image_src').append('<p>Image\'s url: <input size="35" maxlength="1000" id="imageSrcInput" type="text" value="http://" style="float: left; margin: 20px 30px 20px 20px;" /></p>');
            $("#imageSrcInput").focus();
        },
        buttons: {
            'OK!': function() {
                var imgSrc = $('#dialog_image_src').find('#imageSrcInput')[0].value;

                WFF_CurrentImgField.setAsStaticImage("", imgSrc);
                $("#" + WFF_CurrentImgField.id).find(".WFF_static_image_resized").attr('src', imgSrc);

                $('#' + WFF_CurrentImgField.id).find('.WFF_preview_image').tooltip({
                    bodyHandler: function() {
                        return $('<img/>').attr('src', imgSrc);
                    },
                    showURL: false,
                    top: -30,
                    track: true
                });

                $('#dialog_image_src').html("");
                $(this).dialog('close');
            },
            'Cancel': function() {                
                WFF_remove_element('WFF_ui_added_' + WFF_CurrentImgField.baseType, WFF_CurrentImgField.id.replace("WFF_added_element_", ""), "#tab-"+WFE_currentNodeSelectedID);
                WFF_deselectAll();

                $('#dialog_image_src').html("");
                $(this).dialog('close');
            }
        }
    });

    //Dialog for the html object src
    $('#dialog_html_src').dialog({
        autoOpen: false,
        modal: true,
        show: 'slide',
        width: 330,
        open: function(event, ui) {
            $('#dialog_html_src').append('<p>Insert your code to embed here <textarea cols="35" rows="10"  id="htmlSrcInput" value="" style="float: left; margin: 20px 30px 20px 20px;" ></textarea></p>');
            $("#htmlSrcInput").focus();
        },
        buttons: {
            'OK!': function() {
                var htmlSrc = $('#dialog_html_src').find('#htmlSrcInput')[0].value;
                htmlSrc = htmlSrc.replace(/<\/object>/g, '<param name="wmode" value="transparent"></param></object>');
                htmlSrc = htmlSrc.replace(/<embed/, '<embed wmode="transparent"');

                WFF_CurrentHtmlCodeField.setAsStaticHtmlCode("EmbeddedHtmlCode", htmlSrc);

                //TO-DO: preview to be modified!
                /*
                $("#" + WFF_CurrentImgField.id).find(".WFF_static_image_resized").attr('src', imgSrc);

                $('#' + WFF_CurrentImgField.id).find('.WFF_preview_image').tooltip({
                bodyHandler: function() {
                return $('<img/>').attr('src', imgSrc);
                },
                showURL: false,
                top: -30,
                track: true
                });
                */

                $('#dialog_html_src').html("");
                $(this).dialog('close');
            },
            'Cancel': function() {
            WFF_remove_element('WFF_ui_added_' + WFF_CurrentHtmlCodeField.baseType, WFF_CurrentHtmlCodeField.id.replace("WFF_added_element_", ""), "#tab-" + WFE_currentNodeSelectedID);
                WFF_deselectAll();
                
                $('#dialog_html_src').html("");
                $(this).dialog('close');
            }
        }
    });

}

function WFE_drawEnabledServicesDropList() {
    if ($('#mode').attr('checked')) {
        var html = '<p class="p" id="services_droplist">Enabled services: <select id="seleEnabledServices" title="select the account that grants the users to compile the form">';
        html += '<option id="allservices" value="allservices" selected="selected">All services</option>';
        for (var s = 0; s < WFE_authServices.length(); s++) {
            var name = WFE_authServices.getServiceNameByIndex(s);
            var id = WFE_authServices.getServiceIdByIndex(s);
            html += '<option id="' + name + '" value="' + id + '">' + name + '</option>'
        }
        html += '</select></p>';
        $('#form_type').append(html);
    } else {
        $('#services_droplist').remove();
    }
}

//format of the string containing the services: ServiceName1@ServiceId1$ServiceName2@ServiceId2
WFE_buildServiceList = function(services_to_print) {
    WFE_authServices = new WFE_authServiceList();
    var services = services_to_print.split('$');
    for (var k = 0; k < services.length; k++) {
        WFE_authServices.addService(services[k].split('@')[0], services[k].split('@')[1]);
    }
}

WFE_authServiceList = function() {
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

    servList.length = function() { return servList.list.length; }
}

WFE_onChangePublicationType = function() {
    var pubType = $('#sele_saveWF').val();
    if (pubType == "public") {
        WFE_drawPublicTypeOptions();
    } else {
        WFE_drawPrivateTypeOptions();
    }
}

WFE_drawPublicTypeOptions = function() {
    //alert($('#form_type').html);
$('#form_type').html('Single filling mode <input type="checkbox" id="mode" onChange="WFE_drawEnabledServicesDropList();" title="if checked the form will be available only if the user accesses with one of the service selected "/><br />');
    //alert($('#form_type').html);
}

WFE_drawPrivateTypeOptions = function() {
$('#form_type').html('Anonymous filling mode <input type="checkbox" id="mode" title="if checked the form will be anonymous (user inserted data will be disassociate from the user)"/><br />');
}

WFE_preloadImages = function(){
    var images_preloaded = new Array();
    var images_src = new Array(
        "/lib/css/WorkflowEditor/WFF_img/Minus_button.png",
        "/lib/css/WorkflowEditor/WFF_img/Minus_button_disabled.png",
        "/lib/css/WorkflowEditor/WFF_img/Minus_button_hover.png",
        "/lib/css/WorkflowEditor/WFF_img/Plus_button.png",
        "/lib/css/WorkflowEditor/WFF_img/Plus_button_disabled.png",
        "/lib/css/WorkflowEditor/WFF_img/Plus_button_hover.png",
        "/lib/css/WorkflowEditor/WFF_img/Plus_button.png",
        "/lib/css/WorkflowEditor/WFF_img/RadioButton.png",
        "/lib/css/WorkflowEditor/WFF_img/video_tutorial.png",
        "/lib/css/WorkflowEditor/WFF_img/web.png",
        "/lib/css/WorkflowEditor/WFF_img/palette.png",
        "/lib/css/WorkflowEditor/WFF_img/icon_new.png",
        "/lib/css/WorkflowEditor/WFF_img/buttonbg.png",
        "/lib/css/WorkflowEditor/WFF_img/check.png",
        "/lib/css/WorkflowEditor/WFF_img/WFE_button.png",
        "/lib/css/WorkflowEditor/WFF_img/WFE_button_clicked.png",
        "/lib/css/WorkflowEditor/WFF_img/WFE_button_hover.png",
        "/lib/css/WorkflowEditor/WFF_img/WFE_button_opaque.png",
        "/lib/css/WorkflowEditor/WFF_img/WFE_bg_grid_square.gif",
        "/lib/css/WorkflowEditor/WFF_img/WFF_close.png",
        "/lib/css/WorkflowEditor/img/80ade5_40x100_textures_04_highlight_hard_100.png",
        "/lib/css/WorkflowEditor/img/d6d6d6_40x100_textures_02_glass_80.png",
        "/lib/css/WorkflowEditor/img/WFE_addNodeDraggableImage.png",
        "/lib/css/WorkflowEditor/img/resize-bar.png",
        "/lib/css/WorkflowEditor/img/D1E6FC_40x100_textures_10_dots_medium_90.png",
        "/lib/css/WorkflowEditor/img/WFE_button.png",
        "/lib/css/WorkflowEditor/img/resizable-w.gif",
        "/lib/css/WorkflowEditor/img/resizable-e.gif",
        "/lib/css/WorkflowEditor/img/pin-up-off.gif",
        "/lib/css/WorkflowEditor/img/pin-up-on.gif",
        "/lib/css/WorkflowEditor/img/pin-dn-off.gif",
        "/lib/css/WorkflowEditor/img/pin-dn-on.gif",
        "/lib/css/WorkflowEditor/img/go-lt-off.gif",
        "/lib/css/WorkflowEditor/img/go-lt-on.gif",
        "/lib/css/WorkflowEditor/img/go-rt-off.gif",
        "/lib/css/WorkflowEditor/img/go-rt-on.gif",
        "/lib/css/WorkflowEditor/img/go-rt-off.gif",
        "/lib/css/WorkflowEditor/img/go-rt-on.gif",
        "/lib/css/WorkflowEditor/img/go-lt-off.gif",
        "/lib/css/WorkflowEditor/img/go-lt-on.gif"
    );
    if (document.images){
        for(var i=0;i<images_src.length;i++){
            images_preloaded.push(new Image());
            images_preloaded[i].src = images_src[i];
        }
    }
}