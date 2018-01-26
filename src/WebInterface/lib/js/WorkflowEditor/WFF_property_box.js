/**
 *  Property Box (east panel)
 **/

var WFF_constraintsCurrField = null;

var WFF_property_fields_array = new Array();
var WFF_constraints;

var WFF_itemSelected;
var WFF_arrConstr;

var WFF_fieldId;
var WFF_fieldType;

//String representing the selected constraints to be saved
var WFF_constraintsToBeSaved;

//var to see if the user has (de)checked one or more checkbox in the constraint dialog box
var WFF_dechecked;

var WFF_sel;
var WFF_descName;

WFF_makeSelectable = function(currentTab) {
    $(document).ready(function() {
            var close = true;
        $(currentTab).selectable({
            distance: 20,
            cancel: ':.WFF_element_block',
            start: function(event, ui) {
                close = true;
                WFF_deselectAll();
                $(".WFF_added_element").blur();
            },
            selected: function(event, ui) {
                $(ui).addClass('WFF_selected_field');
                
                

                var elemSelez = ui.selected;
                if (elemSelez.className.substring(0, 17) == "WFF_element_block") {
                    var inputObj = $(elemSelez).find(".WFF_added_element").find("input")[0];

                    if (inputObj == null || typeof inputObj == 'undefined') return;
                    var WFF_id = elemSelez.id;
                    WFF_setProperty(WFF_id);
                }
                //show property panel
                WFE_layout.open('east');
                close = false;
            },
            stop: function(event, ui) {
                WFF_showProperties();
                if(close)
                    WFE_layout.close('east');
            }
        });
    });
}

WFF_openPropertyPanelOnDoubleClick = function(id_field){
    WFF_deselectAll();
    //show property panel
    if (WFE_layout.state.east.isClosed)
        WFE_layout.open('east');

    WFF_setProperty(id_field);
    WFF_showProperties();
}

WFF_removeOldConstraints = function(){
$("#dialog_constraints").find("#selectConstraints").remove();
$("#dialog_constraints").find("#param").remove();
}

WFF_arrayContains = function(array, item){
    for(var i = 0; i< array.length; i++)
        if(array[i] == item) return true;
    return false;
}

WFF_removeParamsFromConstr = function(array){
    var ret = new Array();
    for(var i = 0; i< array.length; i++)
        ret[i] = array[i].split("#")[0];
    return ret;
}

/**
 *  Function used to add or remove a constraint when checking or
 *  dechecking the checkbox related to the constraint
 **/
WFF_addRemoveSelConstr = function(constr, checked){
    if(checked)
    {
        WFF_constraintsToBeSaved += ((WFF_constraintsToBeSaved != "") ? "@" : "") + constr;
        WFF_dechecked--;
    }
    else
    {
        var arrConstr = WFF_constraintsToBeSaved.split("@");
        var newConstr = "";
        for(var i = 0; i < arrConstr.length; i++)
        {
            if(arrConstr[i] != constr)
                newConstr += ((newConstr != "") ? "@" : "") +arrConstr[i];
        }
        WFF_constraintsToBeSaved = newConstr;
        WFF_dechecked++;
    }

}
/*
* Function to create the dialog box to add constraints
*/
WFF_callbackFunctConstr = function() {
    WFF_removeOldConstraints();
    WFF_removeOldParamsAndButton();

    var node = WFG_getNode(WFE_currentNodeSelectedID);
    var field = node.getFieldFromId(WFF_fieldId);
    var selectedConstr = WFF_findSelectedConstraints(field);
    var selectedConstName;
    $("#dialog_constraints").find("#selectedConstr").remove();
    //if there are already added constraints they are retrieved and shown with the respective parameters
    if (selectedConstr != null) {
        selectedConstName = WFF_removeParamsFromConstr(selectedConstr);
        $("#dialog_constraints").append("<div id='selectedConstr'>Added constraints:<br></div>");

        for (var i = 0; i < selectedConstr.length; i++) {
            if (field.specialType != WFE_special_type.RADIOBUTTON || (field.specialType == WFE_special_type.RADIOBUTTON && (selectedConstr[i].substring(0, 11) != "setOptions#" && selectedConstr[i].substring(0, 10) != "addOption#"))) {
                var cAndP = selectedConstr[i].split("#");
                var nodeObj = WFG_getNode(WFE_currentNodeSelectedID);
                var fieldObj = nodeObj.getFieldFromId(WFF_fieldId);
                if (fieldObj.specialType != WFE_special_type.GRCHOICE && fieldObj.specialType != WFE_special_type.GRSEQUENCE) {
                    var nonSoPiuCheNomeDiVariabileInventarmi = WFF_constraints.split("|");
                    var nomeConstr = "";
                    for (var k = 0; k < nonSoPiuCheNomeDiVariabileInventarmi.length; k++) {
                        if (nonSoPiuCheNomeDiVariabileInventarmi[k].split("#")[0].split("&")[2] == cAndP[0]) {
                            nomeConstr = nonSoPiuCheNomeDiVariabileInventarmi[k].split("#")[0].split("&")[0];
                            break;
                        }
                    }
                    $("#selectedConstr").append(nomeConstr);
                }
                //if the field is a groupbox
                else {
                    if (cAndP[0] == "minOccur")
                        $("#selectedConstr").append("Minimum number of occurences: ");
                    else if (cAndP[0] == "maxOccur")
                        $("#selectedConstr").append("Maximum number of occurences: ");
                }

                if (cAndP.length > 1) {
                    var paramsSel = cAndP[1].split("$");
                    for (var j = 0; j < paramsSel.length; j++)
                        $("#selectedConstr").append(" " + paramsSel[j]);
                }
                $("#selectedConstr").append("<input type='checkbox' checked='checked' onChange='WFF_addRemoveSelConstr(\"" + selectedConstr[i] + "\", this.checked)'/><br>");
            }
        }
    }
    $("#dialog_constraints").append("<div id='status_error_constr'></div>");

    //creating the drop down menu with the possible applicable constraints for the selected field
    if (field.specialType != WFE_special_type.GRCHOICE && field.specialType != WFE_special_type.GRSEQUENCE) {
        WFF_constraintsCurrField = field;
        var constraints = WFF_constraints;
        WFF_arrConstr = WFF_constraints.split("|");
        $("#dialog_constraints").append("<select id='selectConstraints' onChange='WFF_createParamsSelect(\"" + field.baseType + "\",this.selectedIndex-1)'></select>");

        $("#selectConstraints").append("<option id='empty' value='empty'>---</option>");
        var arrParams;
        var newArrConstr = new Array();
        var nameAndDescr;
        var description;
        var nameConstr;
        var realConstrName;
        for (var constr = 0; constr < WFF_arrConstr.length; constr++) {
            nameAndDescr = WFF_arrConstr[constr].split("#")[0];
            if (WFF_arrConstr[constr] != "" && nameAndDescr != "") {
                nameConstr = nameAndDescr.split("&")[0];
                description = nameAndDescr.split("&")[1];
                realConstrName = nameAndDescr.split("&")[2];
                if (field.specialType != WFE_special_type.RADIOBUTTON || (field.specialType == WFE_special_type.RADIOBUTTON && realConstrName != "setOptions" && realConstrName != "addOption")) {
                    newArrConstr.push(WFF_arrConstr[constr]);
                    $("#selectConstraints").append("<option id='" + realConstrName + "' value='" + realConstrName + "' ONMOUSEOVER='this.parentNode.title = \"" + description + "\"'>" + nameConstr + "</option>");
                }
            }
        }
        if (selectedConstr != null && typeof selectedConstr != 'undefined')
            WFF_arrConstr = newArrConstr;
    }
    //if the field is a groupbox
    else {
        $("#dialog_constraints").append("<div align='left' id='repeat'>Repeatable: <input type='radio' name='repeteable' value='y' onclick='WFF_showMinMaxOccur()' " + ((field.constraints != "") ? "checked" : "") + ">Yes<input type='radio' name='repeteable' value='n' onclick='WFF_hideMinMaxOccur()' " + ((field.constraints == "") ? "checked" : "") + "> No<br></div>");
        if ($("input:radio")[0].checked)
            $("#dialog_constraints").append("<div align='right' id='minMaxOccur'><div align='right'>Minimum number of occurrences: <input type='text' id='minOccur' size='5'/></div><div align='right'>Maximum number of occurrences: <input type='text' id='maxOccur' size='5'/></div></div>")
    }
    WFF_openDialogConstraints();
}

WFF_showMinMaxOccur = function(){
    $("#dialog_constraints").append("<div align='right' id='minMaxOccur'><div align='right'>Minimum number of occurrences: <input type='text' id='minOccur' size='5'/></div><div align='right'>Maximum number of occurrences: <input type='text' id='maxOccur' size='5'/></div></div>")
    WFF_dechecked = 0;
}

WFF_hideMinMaxOccur = function(){
    var nodeObj = WFG_getNode(WFE_currentNodeSelectedID);
    var fieldObj = nodeObj.getFieldFromId(WFF_fieldId);
    WFF_constraintsToBeSaved = "";
    WFF_dechecked = 100;
    $("#dialog_constraints").find("#selectedConstr").remove();
    $("#dialog_constraints").find("#minMaxOccur").remove();
}

WFF_findSelectedConstraints = function(field){
   var constr = WFF_constraintsToBeSaved = field.constraints;
   if(constr != "")
   {
        var constrArr = constr.split("@");
        return constrArr;
   }
   return null;
}

/**
 *  Function used when pressing the Add button to test if the added constraint is
 *  applyable to the field with the inserted parameters
 **/
WFF_addRow = function(){
    var inputs = $("#dialog_constraints").find("#para");
    var al = false;
    if (inputs.length > 0)
        for (var i = 0; i < inputs.length; i++)
            if (inputs[i].value == "")
                al = true;
    if(al)
       WFE_printErrorMessage("Insert parameters' values", null, "status_error_constr");
    else
    {
        var descName = $("#selectConstraints")[0].options[$("#selectConstraints")[0].selectedIndex].text.split(" ---")[0];
        var sel = $("#selectConstraints").val()+"#";
            for(var j=0;j<inputs.length;j++)
                sel += inputs[j].value + ((j < inputs.length -1)?"$":"");
       var nodeObj = WFG_getNode(WFE_currentNodeSelectedID);
       var fieldObj = nodeObj.getFieldFromId(WFF_fieldId);
       WFF_sel = sel;
       WFF_descName = descName;
       WFF_saveConstraints(fieldObj.serialize().split("|")[0] + "|" + sel);
    }
}
/*
* Function to show the added constraint in the dialog box if it is correct
*/
WFF_addConstraint = function(){
   var sel = WFF_sel;
   var descName = WFF_descName;
   WFF_constraintsToBeSaved += ((WFF_constraintsToBeSaved != "") ? "@" : "") + sel ;
   var cAndP = sel.split("#");
   var htmlOfSelect = $("#selectConstraints").html();
   WFF_removeOldConstraints();
   WFF_removeOldParamsAndButton();
   
   //creating the selected constraint with params values and checkbox       
   if($("#dialog_constraints").find("#selectedConstr") == null || $("#dialog_constraints").find("#selectedConstr").length == 0)
        $("#dialog_constraints").append("<div id='selectedConstr'>Added constraints:<br></div>"); 
   $("#selectedConstr").append(descName);
   if(cAndP.length > 1)
   {
        var paramsSel = cAndP[1].split("$");
        for(var j = 0; j < paramsSel.length; j++)
            $("#selectedConstr").append(" "+paramsSel[j]);
   }
   $("#selectedConstr").append("<input type='checkbox' checked='checked' onChange='WFF_addRemoveSelConstr(\""+sel+"\", this.checked)'/><br>");
   WFF_dechecked--;

   //recreating the select menu
   $("#dialog_constraints").append("<select id='selectConstraints' onChange='WFF_createParamsSelect(\"" + WFF_constraintsCurrField.baseType + "\",this.selectedIndex-1)'></select>");  
   $("#selectConstraints").html(htmlOfSelect);

}


WFF_removeOldParamsAndButton = function(){
$("#dialog_constraints").find("#param").remove();
$("#dialog_constraints").find("#addButton").remove();
$("#dialog_constraints").find("#minMaxOccur").remove();
$("#dialog_constraints").find("#repeat").remove();
}

WFF_createParamsSelect = function(fieldBaseType,paraIndex){
WFF_removeOldParamsAndButton();
if(paraIndex >= 0)
    {
        var arrPara = WFF_arrConstr[paraIndex].split("#");
        $("#dialog_constraints").append("<div align='left' id='param'></div>");
        for(var i = 1; i < arrPara.length; i++)
            {
                var paraName = arrPara[i];
                WFF_loadWebControlConstraintValue(paraName, fieldBaseType)
       
            }
        $("#dialog_constraints").append("<div align='left' id='addButton'><input type='button' value='Add' onclick='WFF_addRow()'/></div>");
    }
}

/*
 * Function used to show the dialog box to add constraints
 */
WFF_openDialogConstraints = function() {
    $("#dialog_constraints").dialog('open');
    WFF_dechecked = 0;
    
    $("#dialog_constraints").dialog({
        bgiframe: true,
        resizable: false,
        height: 300,
        width: 600,
        modal: true,
        show: 'slide',
        hide: 'slide',
        overlay: {
            backgroundColor: '#000',
            opacity: 0.5
        },
        buttons: {
            'Save constraints': function() {
            WFB_status_modified = true;
            WFB_removeAllCheckImage();

            var inputs = $(this).find("#para");
            var nodeObj = WFG_getNode(WFE_currentNodeSelectedID);
            var fieldObj = nodeObj.getFieldFromId(WFF_fieldId);
            if(fieldObj.specialType != WFE_special_type.GRCHOICE && fieldObj.specialType != WFE_special_type.GRSEQUENCE)
            {
                if (fieldObj.constraints != WFF_constraintsToBeSaved) {
                       //saving in struct and calling updates on status functions
                        fieldObj.constraints = WFF_constraintsToBeSaved;
                        WFS_AddFields(WFG_workflow.getId(), nodeObj);
                        WFF_removeOldParamsAndButton();
                        $(this).dialog('close');
                    }
                else WFE_printErrorMessage("Select and add a constraint before saving!", null, "status_error_constr");
            }
            //if the field is a groupbox
            else
            {
                if($("#minOccur").val() == "" && $("#maxOccur").val() == "" && WFF_dechecked == 0)
                    WFE_printErrorMessage("Insert parameter(s) value(s)", null, "status_error_constr");
                else
                {
                    if($("#minOccur").val() == undefined && WFF_dechecked == 0)
                        WFE_printErrorMessage("Select Yes and insert the parameter(s) value(s) before saving", null, "status_error_constr");
                    else if(($("#minOccur").val() != undefined && isNaN(parseInt($("#minOccur").val())) && $("#minOccur").val() != "")
                     || ($("#maxOccur").val() != undefined && isNaN(parseInt($("#maxOccur").val())) && $("#maxOccur").val() != ""))
                        WFE_printErrorMessage("Parameters must be numbers!", null, "status_error_constr");
                    else if(parseInt($("#minOccur").val()) < 0)
                        WFE_printErrorMessage("Minimum occurrences cannot be negative!", null, "status_error_constr");
                    else if(parseInt($("#maxOccur").val()) < 1)
                        WFE_printErrorMessage("Maximum occurrences cannot have value less than one!", null, "status_error_constr");
                    else if(parseInt($("#minOccur").val()) > parseInt($("#maxOccur").val()))
                            WFE_printErrorMessage("The minimum number of occurrences cannot be greater than the maximum!", null, "status_error_constr");
                        else
                        {
                            if(WFF_dechecked > 0)
                            {
                                fieldObj.constraints = WFF_constraintsToBeSaved;
                                if(fieldObj.constraints != "")
                                {
                                    var indexOfMin = fieldObj.constraints.indexOf("minOccur");
                                    if(indexOfMin == -1)
                                    {
                                        fieldObj.constraints = "minOccur#1" + ((fieldObj.constraints == "") ? "" : "@") + fieldObj.constraints;
                                    }
                                    var indexOfMax = fieldObj.constraints.indexOf("maxOccur");
                                    if(indexOfMax == -1)
                                    {
                                        fieldObj.constraints += ((fieldObj.constraints == "") ? "" : "@") + "maxOccur#unbounded";
                                    }
                                }
                            }
                            else 
                            {
                                if($("#minOccur").val() != "")
                                {
                                    var indexOf = fieldObj.constraints.indexOf("minOccur");
                                    if(indexOf != -1)
                                    {
                                        var lastIndexOf = fieldObj.constraints.indexOf("@");
                                        var pippo = new RegExp(fieldObj.constraints.substring(indexOf, ((lastIndexOf != -1 && lastIndexOf > indexOf) ? (lastIndexOf - indexOf) : fieldObj.constraints.length)));
                                        fieldObj.constraints = fieldObj.constraints.replace(pippo, "minOccur#" + $("#minOccur").val());
                                    }
                                    else
                                        fieldObj.constraints = "minOccur#" + $("#minOccur").val() + ((fieldObj.constraints == "") ? "" : "@") + fieldObj.constraints;
                                }
                                else 
                                {
                                    var indexOf = fieldObj.constraints.indexOf("minOccur");
                                    if(indexOf != -1)
                                    {
                                        var lastIndexOf = fieldObj.constraints.indexOf("@");
                                        fieldObj.constraints = fieldObj.constraints.replace(new RegExp(fieldObj.constraints.substring(indexOf, ((lastIndexOf != -1 && lastIndexOf > indexOf) ? (lastIndexOf - indexOf) : fieldObj.constraints.length))), "minOccur#1");
                                    }
                                    else fieldObj.constraints = "minOccur#1" + ((fieldObj.constraints == "") ? "" : "@") + fieldObj.constraints;
                                }
                                if($("#maxOccur").val() != "")
                                {
                                    var indexOf = fieldObj.constraints.indexOf("maxOccur");
                                    if(indexOf != -1)
                                    {
                                        var lastIndexOf = fieldObj.constraints.indexOf("@");
                                        fieldObj.constraints = fieldObj.constraints.replace(new RegExp(fieldObj.constraints.substring(indexOf, ((lastIndexOf != -1 && lastIndexOf > indexOf) ? (lastIndexOf - indexOf) : fieldObj.constraints.length))), "maxOccur#" + $("#maxOccur").val());
                                    }
                                    else
                                        fieldObj.constraints += ((fieldObj.constraints == "") ? "" : "@") + "maxOccur#" + $("#maxOccur").val();
                                }
                                else
                                {
                                    var indexOf = fieldObj.constraints.indexOf("maxOccur");
                                    if(indexOf != -1)
                                    {
                                        var lastIndexOf = fieldObj.constraints.indexOf("@");
                                        fieldObj.constraints = fieldObj.constraints.replace(new RegExp(fieldObj.constraints.substring(indexOf, ((lastIndexOf != -1 && lastIndexOf > indexOf) ? (lastIndexOf - indexOf) : fieldObj.constraints.length))), "maxOccur#unbounded");
                                    }
                                    else fieldObj.constraints += ((fieldObj.constraints == "") ? "" : "@") + "maxOccur#unbounded";
                                }
                            }
                            WFS_AddFields(WFG_workflow.getId(), nodeObj);
                            $(this).dialog('close');
                        }
                }
             //}
            }
//                else WFE_printErrorMessage("Select and add a constraint before saving!", null, "status_error_constr");
            },
            'Cancel': function() {
                $(this).dialog('close');
            }
        }
    });
}

WFF_deselectAll = function(){
    $(document).ready(function() {
        $(document).find('.WFF_selected_field').removeClass('WFF_selected_field');
        WFF_property_fields_array = new Array();
        $("#WFF_property_box").empty();
        //WFE_layout.close('east');
    });
}
/*
* Function that shows the property box with the treeview of the selected field's properties
*/
WFF_showProperties = function() {

    var htmlCode;
    var currField;


    //Removing unselectable fields (like TEXT, HTMLCODE, IMAGE, etc..)
    var tmpFieldsArray = new Array();

    for (var i = 0; i < WFF_property_fields_array.length; i++)
        tmpFieldsArray.push(WFF_property_fields_array[i]);

    for (var i = tmpFieldsArray.length - 1; i >= 0; i--) {
        currField = tmpFieldsArray[i];
        
        if (currField.specialType == WFE_special_type.TEXT || currField.specialType == WFE_special_type.HTMLCODE || currField.specialType == WFE_special_type.IMAGE)
            WFF_property_fields_array.splice(i, 1);
    }
    //Finish Removing unselectable fields   


    for (var i = 0; i < WFF_property_fields_array.length; i++) {
        currField = WFF_property_fields_array[i];

        //loading parameters to check double names of field
        var id_node = WFE_currentNodeSelectedID;
        var id_field = currField.id;
        var label_name = currField.getLabel();
        var label_id = id_field.split("WFF_added_element_")[1];

        /*  setta i valori anche nella struttura fields !!!!!! */

        htmlCode = "<input type='hidden' id='fieldId_" + i + "' value='" + currField.id + "'/>" +
        "<input type='hidden' id='fieldType_" + i + "' value='" + currField.baseType + "'/>" +
        "<li class='expandable" + ((i == WFF_property_fields_array.length - 1) ? " lastExpandable" : "") + "'>" +
        "<div class='hitarea expandable-hitarea" + ((i == WFF_property_fields_array.length - 1) ? " lastExpandable-hitarea" : "") + "'></div>" +
        "Field Name:<br /><input class='WFF_property_box_input' type='text' id='nameField' value='" + currField.getLabel() + "' onkeyup=\"WFF_change_name('" + id_field + "',this.value);\"" +

        //check for duplicate names                                           
        "onblur='var value = $(\"#nameField\").attr(\"value\"); WFF_onChangeFieldLabel(\"" + id_node + "\",\"" + id_field + "\",value,\"" + label_name.split(label_id)[0] + "\",\"" + label_id + "\");'/>" +

        "<ul style='display: none;'><li>Field type:<br /><strong>" + currField.baseType + "</strong></li>" +
        "<li alt=\"This is the parameter shown as label in the form filling phase\" title=\"This is the parameter shown as label in the form filling phase\">Rendered label:<br /><textarea class='WFF_property_box_input' cols=\"15\" id=\"renderedLabel\" onkeyup=\"WFF_change_renderedLabel('" + id_field + "',this.value);\">" + (currField.useRenderedLabel ? currField.renderedLabel : currField.getLabel()) + "</textarea></li>" +
        "<li>Description:<br /><textarea class='WFF_property_box_input' cols=\"15\" id=\"description\" value='" + currField.description + "' onkeyup=\"WFF_change_description('" + id_field + "',this.value);\">" + currField.description + "</textarea></li>" +
        "<li class='last'><input type='button' class='WFE_button' value='Constraints' onclick=\"WFF_getConstr('" + currField.id + "','" + currField.baseType + "');\" style=\"margin-left: 0px;\" /></li></ul></li>";

        $(document).find('#WFF_property_box').append(htmlCode);
        htmlCode = "";
    }

    $(document).ready(function() {
        $("#WFF_property_box").treeview();
    });
}

WFF_change_renderedLabel=function(id_field,val)
{
    WFB_status_modified = true;
    WFB_removeAllCheckImage();
    var node=WFG_getNode(WFE_currentNodeSelectedID);
    var field=field=node.getFieldFromId(id_field);
    
    field.renderedLabel=val;
    field.useRenderedLabel=true;
}

WFF_change_description=function(id_field,val)
{
    WFB_status_modified = true;
    WFB_removeAllCheckImage();
    var node=WFG_getNode(WFE_currentNodeSelectedID);
    var field=field=node.getFieldFromId(id_field);
    field.description=val;
}

WFF_change_name = function(id_field,val)
{
    var node=WFG_getNode(WFE_currentNodeSelectedID);
    var field=field=node.getFieldFromId(id_field);
    
    field.setLabel(val);
}

WFF_changeProp = function(fieldId, propType, newProp){
    if(propType == "name")
    {
        $(document).find("#"+fieldId).find(".WFF_label_box").val(newProp);
    }
}

WFF_setIniFin = function(iniFin){
    if(iniFin == "initial")
        WFG_setInitialNode(WFF_itemSelected);
    else if(iniFin == "final")
        WFG_setFinalNode(WFF_itemSelected);
    else if(iniFin == "normal")
        WFG_setNormalNode(WFF_itemSelected);
}

WFF_getConstr = function(fieldId, fieldType){
    WFF_fieldId = fieldId;
    WFF_fieldType = fieldType;
    WFF_getConstraints(fieldType);
}

WFF_setProperty = function(id_field){
    var node=WFG_getNode(WFE_currentNodeSelectedID);
    var field=null;
    if(node!=null)
    {
        field = node.getFieldFromId(id_field);
        if(field!=null)
        {
            WFF_property_fields_array.push(field);
        } else {
            WFE_printErrorMessage("DEBUG: WFF_setProperty("+id_field+"): field is null","verbose");
        }
    }
}

/**
*  Load webcontrols to be used in the dialog to set new preconditions with values
*/
WFF_loadWebControlConstraintValue = function(paraName, fieldBaseType) {
    var renderValueControl = paraName + " <input size=\"15\" type=\"text\" id='para' />";

    switch (fieldBaseType) {
        case 'IntBox':  //needed a restricted textBox
            renderValueControl = paraName + " <input size=\"15\" type=\"text\" id='para' onkeyup=\"this.value = this.value.replace(/[^0123456789,-]/g, '');\"/>";
            $("#param").append("<div class='webcontrol_value'>" + renderValueControl + "</div>");
            break;

        case 'Rating': //needed a restricted textBox
            renderValueControl = paraName + " <input size=\"15\" type=\"text\" maxLength=\"1\" id='para' onkeyup=\"this.value = this.value.replace(/[^012345]/g, '');\"/>";
            $("#param").append("<div class='webcontrol_value'>" + renderValueControl + "</div>");
            break;

        case 'VEMap': //needed a latitude and longitude textBoxes
            renderValueControl = paraName + " <input size=\"10\" type=\"text\" id='para' onkeyup=\"this.value = this.value.replace(/[^0123456789,-]/g, '');\"/>";
            $("#param").append("<div class='webcontrol_value'>" + renderValueControl + "</div>");
            break;

        case 'Calendar':    //needed a DatePicker
            renderValueControl = paraName + " <input size=\"15\" type=\"text\" id='para' class='datePickerConstraints' />";
            $("#param").append("<div class='webcontrol_value'>" + renderValueControl + "</div>");
            $(".datePickerConstraints").css('z-index', 1800);
            $(".datePickerConstraints").datepicker({ dateFormat: 'yy-mm-dd' });  //Setting the datepicker
            break;

        default:    //needed a simple textBox
            $("#param").append("<div class='webcontrol_value'>" + renderValueControl + "</div>");
            break;
    }

    //    var webcontrol = "<p>No webcontrol associated</p>";
    //    
    //    $('.precondition_webcontrol_preview').each(function (index){
    //        var name = $(this).parent().attr('id').split('precondition_')[1];
    //        
    //        if(webcontrol_name == name){
    //            webcontrol = $(this).parent().html();
    //            return webcontrol;
    //        }
    //    });
    //    return webcontrol;
}