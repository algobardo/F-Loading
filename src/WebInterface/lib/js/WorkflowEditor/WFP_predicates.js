
var WFE_fiscalCodeRegExp = new RegExp("^[A-Za-z]{6}[0-9LMNPQRSTUV]{2}[A-Za-z]{1}[0-9LMNPQRSTUV]{2}[A-Za-z]{1}[0-9LMNPQRSTUV]{3}[A -Za-z]{1}$");

var WFE_currentNode = null; // Current node
var WFE_currentEdge = null; // Current edge

var WFE_row_index = 0;
var WFE_select_index = 0;

//////////////////////////////////////////////
/////// logic structure //////////////////////

// type= and | or | null
WFE_logic = function(logic_predicate, not, type, id) {
    var el = this;
    el.id=id;
    el.type = type;
    el.not = not;
    el.logicPredicate = logic_predicate;
    
    el.toXml=function(idNode)
    {
        var toReturn="";
        if(el.type!="null")
        {
            toReturn+="<"+el.type+">";
            if(el.not)
               toReturn+="<NOT>"; 
            
            toReturn+=el.logicPredicate.toXml(idNode);
            
            if(el.not)
               toReturn+="</NOT>"; 
            
            toReturn+="<placeholder>";
            
            toReturn+="</"+el.type+">";
        } else
        {
            if(el.not) toReturn+="<NOT>"+el.logicPredicate.toXml(idNode)+"</NOT>";
            else toReturn+=el.logicPredicate.toXml(idNode);
        }
        return toReturn;
    } 
}

buildPathForGroups = function(field){
if (field.fieldRelative != null) return buildPathForGroups(field.fieldRelative) + '/' + WFS_EncodeName(field.name);
    return WFS_EncodeName(field.name);
}

// predicateName si potrebbe recuperare da preconditionInfo !!!!
WFE_logic_predicate = function(preconditionInfo, fieldOne, fieldTwo, value, predicateName) {
    var el = this;
    el.preconditionInfo = preconditionInfo;
    el.fieldOne = fieldOne;
    el.fieldTwo = fieldTwo;
    el.value = value;
    el.predicateName = predicateName;

    el.setFieldOne = function(field) {
        el.fieldOne = field;
        el.fieldTwo = null;
        el.value = null;
    }

    el.setFieldTwo = function(field) {
        el.fieldTwo = field;
        el.value = null;
    }

    el.setValue = function(value) {
        el.value = value;
    }

    el.isValid = function() {
        return el.fieldOne != null && el.preconditionInfo != null && (el.preconditionInfo.isUnary() || el.fieldTwo != null || el.value != null);
    }

    el.setPredicate = function(preconditionInfo) {
        el.preconditionInfo = preconditionInfo;
        el.predicateName = el.preconditionInfo.name;
    }

    el.toXml = function(idNode) {
        if (el.fieldOne == "empty") return "";

        if (!el.isValid()) return "<error>";

        var toReturn = "";

        toReturn += "<" + el.predicateName + ">";
        //recursively build the path for groups
        toReturn += "<P" + WFS_predicateNumber + " fieldType=\"" + el.fieldOne.baseType + "\" type=\"path\">//" + WFS_EncodeName(idNode) + "/" + buildPathForGroups(el.fieldOne) + "</P" + WFS_predicateNumber + ">";
        WFS_predicateNumber++;
        if (!el.preconditionInfo.isUnary()) {
            if (el.value == null) {
                toReturn += "<P" + WFS_predicateNumber + " fieldType=\"" + el.fieldTwo.baseType + "\" type=\"path\">//" + WFS_EncodeName(idNode) + "/" + buildPathForGroups(el.fieldTwo) + "</P" + WFS_predicateNumber + ">";
                WFS_predicateNumber++;
            } else {

                if (el.preconditionInfo.getParameters()[1] == 'VEMap') {
                    var coordinates = el.value.split('|');
                    toReturn += "<P" + WFS_predicateNumber + " type=\"value\"><" + el.preconditionInfo.getParameters()[1] + "><Latitude>" + coordinates[0] + "</Latitude><Longitude>" + coordinates[1] + "</Longitude></" + el.preconditionInfo.getParameters()[1] + "></P" + WFS_predicateNumber + ">";
                }
                else {
                    var valueAttr = " ";

                    if (el.value != "" && (el.value[0] == " " || el.value[el.value.length - 1] == " "))
                        valueAttr = " xml:space=\"preserve\"";
                        
                        toReturn += "<P" + WFS_predicateNumber + " type=\"value\"><" + el.preconditionInfo.getParameters()[1] + "><Value" + valueAttr + ">" + el.value + "</Value></" + el.preconditionInfo.getParameters()[1] + "></P" + WFS_predicateNumber + ">";
                    WFS_predicateNumber++;
                }
            }
        }
        toReturn += "</" + el.predicateName + ">";
        return toReturn;
    }
}


//////////////////////////////////////////////
/////// precondition dialog //////////////////

WFE_add_row = function(id_row) {
        $("#dialog").append("<div id=\"row_" + id_row + "\"></div>");
        $("#row_" + id_row).append("Not<input onChange=\"WFE_onChangeNot("+id_row+",this.checked)\" id=\"" + id_row + "_not\" type=\"checkbox\" />");
        WFE_add_select(id_row, "WFE_onChange_label('" + id_row + "',this.selectedIndex-1,this.options[this.selectedIndex].value)", WFE_getLabelAndResult());
        $("#row_" + id_row).append("<br><span id=\"row_" + id_row + "_radio\">And<input onChange=\"WFE_onChangeRadioButton(" + id_row + ",this.value)\" type=\"radio\" name=\"" + id_row + "_logic\" value=\"AND\">&nbsp;Or<input onChange=\"WFE_onChangeRadioButton(" + id_row + ",this.value)\" type=\"radio\" name=\"" + id_row + "_logic\" value=\"OR\">&nbsp;Last<input style=\"margin-bottom:20px;\" onChange=\"WFE_onChangeRadioButton(" + id_row + ",this.value)\" type=\"radio\" name=\"" + id_row + "_logic\" value=\"null\" checked></span>");
        
        WFE_addPrecondition(id_row);
    }
    
    WFE_onChangeRadioButton=function(id_row,value)
    {
        var tmp=WFE_currentEdge.getPredicate(id_row);
        if(tmp!=null) tmp.type=value;
        
        if(value!="null" && tmp!=null && WFE_currentEdge.isLastPredicate(tmp)) WFE_add_row(WFE_row_index);
        if(value=="null" && tmp!=null) WFE_currentEdge.removePredicateAfter(tmp);

    }
    
    WFE_onChangeNot=function(id_row,value)
    {
        var tmp=WFE_currentEdge.getPredicate(id_row);
        if(tmp!=null) tmp.not=value;
    }

    WFE_onChangeValue = function(id, value) {
        var id_row = id.replace("row_","");

        //If the predicate contains a VEMap, we have to recover the latitude value, and concat it with the value received
        if ($("#row_" + id_row).find("#" + id_row + "_latitude").length > 0) {
            var v = $("#row_" + id_row).find("#" + id_row + "_latitude")[0].value;
            value = v + '|' + value;
        }

        var tmp = WFE_currentEdge.getPredicate(id_row);
        if (tmp != null) tmp.logicPredicate.setValue(value);
    }

    WFE_add_select = function(id_row, onChangeFunction, options) {
        if ($("#row_" + id_row).find("select").length > 0)
            $("#row_" + id_row).find("select:last").after("<select id=\"select_" + WFE_select_index + "\" onChange=\"" + onChangeFunction + "\"></select>&nbsp;");
        else $("#row_" + id_row).append("<select id=\"select_" + WFE_select_index + "\" onChange=\"" + onChangeFunction + "\"></select>&nbsp;");
        WFE_add_option(id_row, WFE_select_index, options);

        WFE_select_index++;
    }

    WFE_add_option = function(id_row, id_select, options) {
        $("#row_" + id_row).find("#select_" + id_select).append("<option id=\"empty\" value=\"empty\">---</option>");
        for (var i = 0; i < options.length; i++)
            $("#row_" + id_row).find("#select_" + id_select).append("<option id='" + options[i] + "' value='" + options[i] + "'>" + options[i] + "</option>");
    }

    WFE_onChange_label = function(id_row, selectedIndex, labelOne) {
        WFE_remove_select(id_row, 0);
        var tmp = WFE_currentNode.getFieldFromLabel(labelOne);

        if (tmp != null) {
            var type = tmp.baseType;

            WFE_add_select(id_row, "WFE_onChange_precondition('" + id_row + "',this.options[this.selectedIndex].value,'" + type + "','" + labelOne + "')", WFF_preconditionArray[type].getPredicateArray());
        }
        else
            tmp = "empty";

        var tmpPredicate = WFE_currentEdge.getPredicate(id_row);
        if (tmpPredicate != null) tmpPredicate.logicPredicate.setFieldOne(tmp);
    }

    WFE_onChange_precondition = function(id_row, precondition, type, fieldOne) {
        WFE_remove_select(id_row, 1);

        var preconditionInfo = WFF_preconditionArray[type].name[precondition];
        var functionType = preconditionInfo.getParameters();
        var parameters = null;

        if (preconditionInfo.isPredicate()) parameters = functionType.slice(1, functionType.length);
        else parameters = functionType.slice(2, functionType.length);

        // va solo con quelle binarie occhio!
        for (var i = 0; i < parameters.length; i++) {
            var option = WFE_getFieldOfType(parameters[i]);
                        
            if (option == null || typeof option == 'undefined')
                option = new Array();
                
            option.push("Value");
            WFE_add_select(id_row, "WFE_onChange_parameters('" + id_row + "','" + precondition + "','" + type + "','" + fieldOne + "',this.options[this.selectedIndex].value)", option);
        }

        var tmp = WFE_currentEdge.getPredicate(id_row);
        if (tmp != null) tmp.logicPredicate.setPredicate(preconditionInfo);
    }

    WFE_onChange_parameters = function(id_row, precondition, type, labelOne, labelTwo) {
        if (labelTwo == "Value") {
            //$("#row_" + id_row).find("select:last").after("<input size=\"15\" type=\"text\" id=\"" + id_row + "_value\" onChange=\"WFE_onChangeValue("+id_row+",this.value);\"/>");
            WFP_loadWebcontrolForPreconditions(type, id_row)
            $(".webcontrol_value").css('display', 'inline');

            $(".precondition_webcontrol_preview").change(function() {
                //alert(this.value);                
                WFE_onChangeValue(this.parentNode.parentNode.id, this.value);
            });
        } else {
            $("#" + id_row + "_value").remove();
            $("#" + id_row + "_latitude").remove();
            $("#" + id_row + "_latitudeLabel").remove();
            $("#" + id_row + "_longitude").remove();
            $("#" + id_row + "_longitudeLabel").remove();
        }
        var tmp = WFE_currentEdge.getPredicate(id_row);
        if (tmp != null) tmp.logicPredicate.setFieldTwo(WFE_currentNode.getFieldFromLabel(labelTwo));
    }
   
   //brutta adesso si può sistemare!!!
    WFE_addPrecondition = function(id_row) {
        $("#row_" + id_row + "_continue").remove();        
                
        var value = null;
        if ($("#row_" + id_row).find("#" + id_row + "_value").length > 0)
            value = $("#row_" + id_row).find("#" + id_row + "_value")[0].value;
        else if ($("#row_" + id_row).find("#" + id_row + "_latitude").length > 0)
            value = $("#row_" + id_row).find("#" + id_row + "_latitude")[0].value + '|' + $("#row_" + id_row).find("#" + id_row + "_longitude")[0].value;

        if (value != null) {
            var tmp = WFE_currentEdge.getPredicate(id_row);
            if (tmp != null) tmp.logicPredicate.setValue(value);
        }

        var labelArray = new Array();
        var select = $("#row_" + id_row).find("select").each(function(i) { labelArray.push(this.options[this.selectedIndex].value) });
        var notChecked = document.getElementById(id_row + "_not").checked;

        var logicSelected = null;
        var radioElement = document.getElementById("row_" + id_row + "_radio");
        for (var i = 0; i < radioElement.childNodes.length; i++) {
            if (radioElement.childNodes[i].checked == true) {
                logicSelected = radioElement.childNodes[i].value;
                break;
            }
        }
        var fieldOne = (labelArray[0] != null) ? WFE_currentNode.getFieldFromLabel(labelArray[0]) : null;
        var fieldTwo = (labelArray[2] != null) ? WFE_currentNode.getFieldFromLabel(labelArray[2]) : null;
        var preconditionInfo = (labelArray[1] != null) ? WFF_preconditionArray[fieldOne.baseType].name[labelArray[1]] : null;
        WFE_currentEdge.predicateList.push(new WFE_logic(new WFE_logic_predicate(preconditionInfo, fieldOne, labelArray.length > 1 ? (labelArray[2] == "Value" ? null : fieldTwo) : null, value, labelArray[1]), notChecked, logicSelected, id_row));
        WFE_row_index += 1;
        if (logicSelected == "OR" || logicSelected == "AND") WFE_add_row(WFE_row_index);
    }

    WFE_openDialog = function(edge) {
        $("#dialog").dialog('open');        

        WFP_save = false;
        WFE_currentNode = edge.from;
        WFE_currentEdge = edge;

        if (edge.predicateHtml == null) {
            WFE_row_index += 1;
            WFE_add_row(WFE_row_index);
        } else {
            var predicate = WFE_currentEdge.predicateList;
            // dialog precondition restoring //
            $("#dialog").append(WFE_currentEdge.predicateHtml);

            for (var i = 0; i < predicate.length; i++) {
                if (predicate[i].id > WFE_row_index)
                    WFE_row_index = predicate[i].id;

                $("#row_" + predicate[i].id).find("select").each(
                    function(index) {
                        var label = "";
                        if (index == 0) label = predicate[i].logicPredicate.fieldOne.getLabel();
                        else if (index == 1) label = predicate[i].logicPredicate.predicateName;
                        else label = (predicate[i].logicPredicate.fieldTwo == null) ? "Value" : predicate[i].logicPredicate.fieldTwo.getLabel();
                        for (var j = 0; j < this.options.length; j++)
                            if (this.options[j].value == label)
                            this.options[j].selected = true;
                    }
                    );

                var notChecked = document.getElementById(predicate[i].id + "_not");
                if (notChecked != null) notChecked.checked = predicate[i].not;

                $("#row_" + predicate[i].id + "_radio").find("input").each(
                    function(j) {
                        if (this.value == predicate[i].type)
                            this.checked = true;
                    }
                );

                if (predicate[i].logicPredicate.value != null) {                    
                    if (predicate[i].logicPredicate.preconditionInfo.getParameters()[1] == 'VEMap') {
                        var coordinates = predicate[i].logicPredicate.value.split('|');
                        $("#" + predicate[i].id + "_latitude").attr("value", coordinates[0]);
                        $("#" + predicate[i].id + "_longitude").attr("value", coordinates[1]);
                    }
                    else
                        $("#" + predicate[i].id + "_value").attr("value", predicate[i].logicPredicate.value);
                }
            }
            // end restoring //
            WFE_row_index++;
            $(".precondition_webcontrol_preview").change(function() {
                //alert(this.value);                
                WFE_onChangeValue(this.parentNode.parentNode.id, this.value);
            });
        }

        $("#dialog").dialog({
            bgiframe: true,
            resizable: false,
            height: 400,
            width: 800,
            modal: true,
            show: 'slide',
            hide: 'slide',
            close: function(event, ui) {
                WFE_remove_all_row();
                if (!WFP_save) {
                    var eye = WFS_EdgePrecondition(WFE_currentEdge, false);

                    if (!eye) {
                        WFE_currentEdge.predicateList = new Array();
                        WFE_currentEdge.predicateHtml = null;
                    }
                }
            },
            overlay: {
                backgroundColor: '#000',
                opacity: 0.5
            },
            buttons: {
                'Save preconditions': function() {
                    WFB_status_modified = true;
                    WFB_removeAllCheckImage();

                    // salva precondizioni                    
                    var eye = WFS_EdgePrecondition(WFE_currentEdge, true);
                    if (eye) {
                        WFE_currentEdge.predicateHtml = $("#dialog").html();
                        WFP_save = true;
                        $(this).dialog('close');
                    } else {
                        alert("Precondition error");
                    }

                },
                'Cancel': function() {
                    $(this).dialog('close');
                }
            }
        });
    }

    // unirle????
    WFE_getLabelAndResult = function() {
        // skip dei bool! facciamoci fare un bel field!
        var toReturn = WFE_currentNode.getFieldLabel();
        for (var i = 0; i < WFE_result.length; i++) {
            if (WFE_result[i] != "Bool") toReturn.push("Result " + i);
        }
        return toReturn;
    }

    WFE_getFieldOfType = function(type) {
        var toReturn = WFE_currentNode.getFieldOfType(type);
        for (var i = 0; i < WFE_result.length; i++) {
            if (WFE_result[i] == type) toReturn.push("Result " + i);
        }
        return toReturn;
    }
    ////////

    WFE_remove_select = function(id_row, start) {
        $("#row_" + id_row).find("select").each(function(i) {
            if (i > start) {
                $(this).remove();
            }
        });
        $("#" + id_row + "_value").remove();
        $("#" + id_row + "_latitude").remove();
        $("#" + id_row + "_latitudeLabel").remove();
        $("#" + id_row + "_longitude").remove();
        $("#" + id_row + "_longitudeLabel").remove();
    }

    WFE_remove_all_row = function() {
        $("#dialog").html("");
        WFE_row_index = 0;
    }
    
    WFE_remove_row=function(id_row)
    {
        $("#row_" + id_row).remove();
    }


//////////////////////////////////////////////
/////// old //////////////////////////////////

WFP_addPredicateFromXml = function(edge) {
    $(function() {
        WFE_currentEdge = edge;
        $('#dialog_predicates_from_xml').append("<p>Insert here complete XML for the predicate</p><select id=\"from\"><option id=\"empty\" value=\"empty\" selected=\"selected\">----</option>"
                                    + "</select>&nbsp;<select id=\"to\"><option id=\"empty\" value=\"empty\" selected=\"selected\">----</option></select><p><textarea id=\"dialog_xml_textarea\" cols=\"30\" rows=\"10\"></textarea></p>");

        var nodes = WFG_getAllNode();
        var ids = new Array();

        if (nodes != null && nodes != undefined) {
            for (var i = 0; i < nodes.length; i++) {
                ids.push(nodes[i].getName());
            }

            $("#dialog_predicates_from_xml").find('select').each(function(i) {
                for (var i = 0; i < ids.length; i++) {
                    $(this).append("<option id='" + i + "' value='" + ids[i] + "'>" + ids[i] + "</option>");
                }
            });
        }

        $('#dialog_predicates_from_xml').dialog({
            show: 'slide',
            width: 400,
            close: function(event,ui){
                $('#dialog_predicates_from_xml').html("");
                
                
            },
            buttons: {
                'Save preconditions': function() {
                    WFB_status_modified = true;
                    WFB_removeAllCheckImage();
                    
                    var xml = $("#dialog_xml_textarea")[0].value;
                    if(xml=="") $(this).dialog('close');
                    var from = $("#from")[0].value;
                    var to = $("#to")[0].value;
                    
                    var fromId = -1;
                    var toId = -1;
                    for(var nodeid=1;nodeid<WFG_workflow.element.length+1;nodeid++){
                        if (WFG_workflow.getNode(WFG_workflow.getId() + WFE_nodeID + nodeid).getName() == from) fromId = WFG_workflow.getId() + WFE_nodeID + nodeid;
                        if (WFG_workflow.getNode(WFG_workflow.getId() + WFE_nodeID + nodeid).getName() == to) toId = WFG_workflow.getId() + WFE_nodeID + nodeid;
                    }

                    //WFS_ModifyEdge(WFG_workflow.getId(), fromId, toId, xml);

                    //$('#dialog_predicates_from_xml').html("");

                    // changed for beta version
                    WFE_currentEdge.predicateList.push(xml);
                    var eye=WFP_EdgePrecondition(WFE_currentEdge,true);
                    if(eye)
                    {
                        WFE_currentEdge.predicateHtml=$("#dialog").html();
                        WFP_save=true;
                        $(this).dialog('close');
                    } else
                    {
                        alert("Precondition error");
                    }
                    
                    $(this).dialog('close');
                },
                'Cancel': function() {
                    //$('#dialog_predicates_from_xml').html("");
                
                    $(this).dialog('close');
                }
            }
        });

        $('#dialog_predicates_from_xml').dialog('open');
    });
}

function WFP_EdgePrecondition(edge, control) {

    for (var WFS_i = 0; WFS_i < edge.predicateList.length; WFS_i++) {
        //var tmp=edge.predicateList[WFS_i].toXml(edge.from.getId());
        var text = edge.predicateList[WFS_i];
    }
    if (control) {
        //alert(text);
        edge.setPrecondition(text);
        WFS_ModifyEdge(edge.from.workflowRelative.getId(), edge.from.getId(), edge.to.getId(), text);
    }
    return true;
}

/**
 *  Load webcontrols to be used in the dialog to set new preconditions with values
 */
WFP_loadWebcontrolForPreconditions = function(webcontrol_name, id_row) {
    var renderValueControl = "<input size=\"15\" type=\"text\" class='precondition_webcontrol_preview' id=\"" + id_row + "_value\" />";
    
    switch (webcontrol_name) {
        case 'IntBox':  //needed a restricted textBox
            renderValueControl = "<input size=\"15\" type=\"text\" class='precondition_webcontrol_preview' id=\"" + id_row + "_value\" onkeyup=\"this.value = this.value.replace(/[^0123456789,-]/g, '');\"/>";
            $("#row_" + id_row).find("select:last").after("<div class='webcontrol_value'>" + renderValueControl + "</div>");
            break;

        case 'Rating': //needed a restricted textBox
            renderValueControl = "<input size=\"15\" type=\"text\" maxLength=\"1\" class='precondition_webcontrol_preview' id=\"" + id_row + "_value\" onkeyup=\"this.value = this.value.replace(/[^012345]/g, '');\"/>";
            $("#row_" + id_row).find("select:last").after("<div class='webcontrol_value'>" + renderValueControl + "</div>");
            break;

        case 'VEMap': //needed a latitude and longitude textBoxes
            renderValueControl = "<label id=\"" + id_row + "_latitudeLabel\"> Latitude</label><input size=\"10\" type=\"text\" id=\"" + id_row + "_latitude\" onkeyup=\"this.value = this.value.replace(/[^0123456789,-]/g, '');\"/><label id=\"" + id_row + "_longitudeLabel\"> Longitude</label><input size=\"10\" type=\"text\" class='precondition_webcontrol_preview' id=\"" + id_row + "_longitude\" onkeyup=\"this.value = this.value.replace(/[^0123456789,-]/g, '');\"/>";
            $("#row_" + id_row).find("select:last").after("<div class='webcontrol_value'>" + renderValueControl + "</div>");
            break;

        case 'FiscalCode': //needed a restricted textBox
            renderValueControl = "<input size=\"15\" type=\"text\" class='precondition_webcontrol_preview' id=\"" + id_row + "_value\" onchange=\"if (!(WFE_fiscalCodeRegExp.test(this.value))) { alert(this.value + ' is not a correct italian fiscal code!'); this.value = ''; } \"/>";
            $("#row_" + id_row).find("select:last").after("<div class='webcontrol_value'>" + renderValueControl + "</div>");
            break;

        case 'Calendar':    //needed a DatePicker
            $("#row_" + id_row).find("select:last").after("<div class='webcontrol_value'>" + renderValueControl + "</div>");
            $("#" + id_row + "_value").css('z-index', 1800);
            $("#" + id_row + "_value").datepicker({ dateFormat: 'yy-mm-dd' });  //Setting the datepicker
            break;

        default:    //needed a simple textBox
            $("#row_" + id_row).find("select:last").after("<div class='webcontrol_value'>" + renderValueControl + "</div>");
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