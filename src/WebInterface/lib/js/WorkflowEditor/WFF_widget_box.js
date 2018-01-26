/**
 *  WidgetBox javascript library
 **/ var WFF_rendered_box_id = 0; 
var WFF_preconditionArray=new Array();

var WFF_CurrentImgField = null; //To support image field creation

var WFF_CurrentHtmlCodeField = null; //To support embedded html code object creation


function WFF_preconditionArray_length()
{
    var count=0;
    for(var i in WFF_preconditionArray) count++;
    return count;
}

WFF_checkDoubleNames = function(name,label_id,node,field){
    var id_node = node.getId();
    var label_name = name.split(label_id)[0];
    
    var fieldArray = null;
    
    (field.fieldRelative != null) ? fieldArray = field.fieldRelative.children : fieldArray = node.field;
    if(fieldArray.length>1)
    {
        for (var i = 0; i < fieldArray.length; i++) 
        {
            if (fieldArray[i].specialType!=WFE_special_type.TEXT && fieldArray[i].id != field.id && fieldArray[i].getLabel() == (label_name+label_id))
            {
                label_id++;
                i=0;
            }
        }
    }
    field.name = label_name + label_id;
}

function WFF_onChangeFieldLabel(id_node,id_field,value,label_name,label_id) {
    //WFB_status_modified = true;
    //WFB_removeAllCheckImage();
    var node = WFG_workflow.getNode(id_node);
    var field = node.getFieldFromId(id_field);
    if(field==null) alert('DEBUG: [WFF_onChangeFieldLabel] field is null');
    if (field.specialType != WFE_special_type.TEXT && field.specialType != WFE_special_type.IMAGE) {            
        field.setLabel(value);

        for (var i = 0; i < node.field.length; i++) {
            if (node.field[i].specialType != WFE_special_type.TEXT && !node.field[i].isEquals(field)) {
                if (node.field[i].getLabel() == value) {
                    WFF_checkDoubleNames(label_name, label_id, node, field);
                    field.setLabel(field.name);
                }
            }

        }
        //Security controls
        //SEC_validate_field_labels(field, label_name);
    }
    else
        field.renderedLabel = value;
    
}

function WFF_setStaticLabelValue(value,field_id,node_id){
    var field = WFG_getNode(node_id).getFieldFromId(field_id);
    field.renderedLabel = value; 
}

function SEC_validate_field_labels(field, label_name) 
{
    //Quando lo ricommitti.. commenta la tua chiamata se fa cose che rompono come questa di ora!
    //security: sanitizing input
    var toSanitize = label_name;
    toSanitize.replace(/\W/g, " ");

    label_name = toSanitize;
    field.setLabel(toSanitize);
    
}

/**
 *  Return the rendering of the label for the current rendered item
 **/
function WFF_label_render(label_name, label_id, node, field) {
    var hasConstraints = (field.specialType != WFE_special_type.TEXT && field.specialType != WFE_special_type.HTMLCODE && field.specialType != WFE_special_type.IMAGE);
    var ondblclick = hasConstraints ? "$(this).blur();WFF_openPropertyPanelOnDoubleClick('" + field.id + "')" : "";
    
    return "<div class=\"WFF_added_element\">" +
				"<input type=\"text\" onDblClick=\""+ondblclick+"\" onClick=\"$(this).focus();\" onkeyup=\"WFF_expandLabelOnChange('#WFF_label" + label_id+"');\" onBlur=\"WFF_onChangeFieldLabel('" + node.getId() + "','" + field.id + "',this.value,'" + label_name +"','"+ label_id + "')\" id=\"WFF_label" + label_id + "\" class=\"WFF_label_box\" value=\"" + field.name + "\" size=\""+(4+field.name.length)+"\"/><script type=\"text/javascript\">$(\"#WFF_label" + label_id + "\").focus();</script>" +
			"</div>";

}

/**
 *  Render the close_button for the current rendered item
 **/
function WFF_close_button_render(type_to_close,element_to_remove,current_node){
    return 	/*'<img class="WFF_drag-button" src="../lib/css/WorkflowEditor/WFF_img/dragicon.png" alt="Drag me" title="Drag me" />*/'<div class="WFF_added_element WFF_close-button" title="remove this ' + type_to_close + '" onClick="WFF_remove_element(\'WFF_ui_added_' + type_to_close + '\',' + element_to_remove + ',\'' + current_node + '\'); WFF_deselectAll();" onMouseOver="this.focus();">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;';				
}


/**
 *  Initialize draggable widgets
 **/
function WFF_enable_draggable_widgets(){
    $(function() {
	    $('.WFF_added_widget').draggable({
	        opacity: 0.7 
	        , helper: 'clone'
	        , revert: 'invalid'
	        , scroll: false
	        //, containment: '#content-border'
	    });
    });
}

/**
 *  Remove element and his cookie
 **/
function WFF_remove_element(what_to_remove,element_to_remove,node_edited){
    WFB_status_modified = true;
    WFB_removeAllCheckImage();
    var wfnodepath = node_edited.split('-')[1];
    //what_to_remove is a WFF_ui_added_IntBox like string
    var fieldtype = what_to_remove.substr(13);

	/* :s */
	//remove relative object in the WFG structure
	var node = WFG_getNode(wfnodepath);
	var field = node.getFieldFromId("WFF_added_element_" + element_to_remove);
	var deltaheight = $("#"+field.id).height();
	
	if(field.fieldRelative != null){ //field belongs to a group and is not top-level
	    for (var toremove = 0; toremove < field.fieldRelative.children.length; toremove++) {
            if (field.fieldRelative.children[toremove].id == field.id) {
                field.fieldRelative.children.splice(toremove, 1);
                
                WFF_adjustParentHeight(field,'rem',deltaheight);
                
                $(node_edited).find('#WFF_added_element_'+element_to_remove).hide('scale',500,'$(this).remove()');
                
                var parent = $("#"+field.fieldRelative.id);
                if(parent.find('li').length == 0)
                    parent.find('.WFF_static_group_box').removeClass('WFF_ui_added_content').html('<li id=\"WFF_empty\" style=\"margin-top: 13px;\"><span>Group is empty.. Move here the elements you want to add..</span></li>');
                break;
            }
        }
	} else {
	    $(node_edited).find('#WFF_added_element_'+element_to_remove).hide('scale',500,'$(this).remove()');
	    if($(node_edited).find('li').length == 0) $(node_edited).removeClass('WFF_ui_added_content').html('<li id=\"WFF_empty\" style=\"margin-top: 13px;\"><span>EMPTY:<br />Move here the elements you want to add..</span></li>');
        node.removeField("WFF_added_element_" + element_to_remove);
    }
}

/**
 *  Creates the dynamic widget list, retrieving widgets rapresentation from the UpdatePanel
 **/
WFF_create_widget_list = function(){
    $('.WFF_widget_in_uppanel').each(function (index){
        var icon = $(this).html();
        $('#WFF_widget_list').append('<li id="WFF_widgets_'+this.id+
            '" class="WFF_widget_generic_button WFF_'+this.id+
            '-button WFF_added_widget" title="'+this.id+
            '" alt="'+this.id+'" onmouseover="$(this).addClass(\'WFF_widget_mouseover\');"'+
            ' onmouseout="$(this).removeClass(\'WFF_widget_mouseover\');" >'+icon+'</li>'
        );
    });
    
    //STATIC WIDGETS (special types, must be threated in different way! Loaded from fields aswell but not IFields)
    $('.WFF_widget_in_upspecial').each(function (index){
        var icon = $(this).html();
        $('#WFF_widget_list_static').append('<li id="WFF_widgets_'+this.id+
            '" class="WFF_widget_generic_button WFF_'+this.id+
            '-button WFF_added_widget" title="'+this.id+
            '" alt="'+this.id+'" onmouseover="$(this).addClass(\'WFF_widget_mouseover\');"'+
            ' onmouseout="$(this).removeClass(\'WFF_widget_mouseover\');" >'+icon+'</li>'
        );
    });
    
    //$('#WFF_widget_list').append('<li id="WFF_widgets_static-HaltRow" class="WFF_button WFF_HaltRow-button WFF_added_widget" title="StaticHaltRow" alt="StaticHaltRow">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Static Separation</li>');
    
    $('#WFF_accordion_widgets').accordion({
		event: "click",
		collapsible: true,
		fillSpace: true,			
		change: function(event,ui){
		    ui.newHeader.addClass('WFF_accordion_widgets_active');
		    ui.oldHeader.removeClass('WFF_accordion_widgets_active');
		}
	});

}

/**
 *  Enable droppable area and event handling on drop events
 **/
function WFF_enable_droppable(currentWF_ID,WFE_nodeID){
    $(function() {
        var tmp = $(WFF_droppable_id_name);
        tmp.droppable({
            accept: '.WFF_added_widget',
            greedy: true,
            drop: function(event, ui) {
                WFB_status_modified = true;
                WFB_removeAllCheckImage();
                
                //select the current tab
                var active_tab = '#tab-' + WFE_currentNodeSelectedID;

                WFF_createObject(ui, tmp, active_tab);
            }
        });

    });
}

/**
 *  Load all the predicates and operations applicable to chosen type into array (ASYNC CALL!)
 **/
function WFF_loadType(idType)
{
    if(WFF_preconditionArray[idType]==null || typeof WFF_preconditionArray[idType] == 'undefined')
    {
        WFC_getPredicates(idType);
    }
}
/* sistemare!!!!!!!!!!! */
/**
 *  Retriev predicates from server
 **/
function WFF_receivePredicate(val)
{
    var tmp=val.split("&");
    WFF_preconditionArray[tmp[0]]=new WFF_containerFunction(tmp[0]);
    for(var i=1;i<tmp.length;i++)
    {
        var tmpType=tmp[i].split("|");
        var functionName=tmpType[0];
        tmpType.splice(0,1);
        WFF_preconditionArray[tmp[0]].addFunction(functionName,tmpType,"predicate");
    }  
    //WFC_getOperations(tmp[0]);
}

/**
 *  Retrieve operations from server
 **/
function WFF_receiveOperation(val)
{
    var tmp=val.split("&");
    //WFF_preconditionArray[tmp[0]]=new WFF_containerFunction(tmp[0]);
    for(var i=1;i<tmp.length;i++)
    {
        var tmpType=tmp[i].split("|");
        var functionName=tmpType[0];
        tmpType.splice(0,1);
        WFF_preconditionArray[tmp[0]].addFunction(functionName,tmpType,"operation");
    }  
}

function WFF_containerSingleFunction(name,typeArray,type)
    {
        var el=this;
        el.name=name;
        el.typeArray=typeArray;
        el.type=type;
        
        el.getParameters=function()
        {
            return el.typeArray;
        }
        
        el.isPredicate=function()
        {
            if(el.type=="predicate") return true;
                else return false;
        }
        
        el.isOperation=function()
        {
            if(el.type=="operation") return true;
                else return false;
        }
        
        el.isUnary=function()
        {
            if(el.isOperation()) return !(el.typeArray.length>2);
            else return !(el.typeArray.length>1);
        }
    }

function WFF_containerFunction(id)
{
    var el=this;
    el.typeId=id;
    el.name=new Array();

    el.addFunction=function(functionName,typeArray,type)
    {
        el.name[functionName]=new WFF_containerSingleFunction(functionName,typeArray,type);
    }
    
    el.getParameters=function(functionName)
    {
        return el.name[functionName].getParameters();
    }
    
    el.isPredicate=function(functionName)
    {
        return el.name[functionName].isPredicate();
    }
    
    el.isOperation=function(functionName)
    {
        return el.name[functionName].isOperation();
    }
    
    el.getArray=function()
    {
        var toReturn=new Array();
        for(var i in el.name)
        {
            toReturn.push(i);
        }
        return toReturn;
    }
    
    el.getPredicateArray=function()
    {
        var toReturn=new Array();
        for(var i in el.name)
        {
            if(el.name[i].isPredicate())
                toReturn.push(i);
        }
        return toReturn;
    }
}

WFF_getPositionX = function(obj){
    return obj.offsetParent.offsetLeft;
}

WFF_getPositionY = function(obj){
    return obj.offsetParent.offsetTop;
}

WFF_createObject = function(ui, tmp, active_tab) {
    var current_id = WFF_rendered_box_id++;
    var render_control = "";

    var type_dragged = (ui.draggable.attr('id')).split("_")[2];

    WFF_findLastObj(tmp, current_id, type_dragged);

    var node = WFG_getNode(active_tab.split("-")[1]);
    var field = null;

    switch (type_dragged) {

        //case 'static-LabelBox':   
        case 'StaticText':
            field = new WFE_field("WFF_added_element_" + current_id, "StaticLabel", "StaticLabel", node);
            //position are deprecated now (ariel)
            /*
            field.posx = posx;
            field.posy = posy;
            */
            field.setAsStaticLabel("Label");

            WFF_addObjectToParent(tmp, active_tab, field, node);

            WFF_drawStaticLabel(field, current_id, tmp, active_tab, node);
            break;

        //case 'static-GroupBoxSequence':   
        case 'Sequence':
            field = new WFE_field("WFF_added_element_" + current_id, "GroupSequence", "GroupSequence" + current_id, node);
            field.name = "GroupSequence" + current_id;
            field.height = 40;
            WFF_checkDoubleNames(field.name, current_id, node, field);
            field.renderedLabel = field.name;
            field.setAsSequenceGroup();

            WFF_addObjectToParent(tmp, active_tab, field, node);

            WFF_drawGroupBox(field, current_id, tmp, active_tab, node);
            break;
        //case 'static-GroupBoxChoice':   
        case 'Choice':
            field = new WFE_field("WFF_added_element_" + current_id, "GroupChoice", "GroupChoice" + current_id, node);
            field.name = "GroupChoice" + current_id;

            //field.width = 200;
            field.height = 40;
            WFF_checkDoubleNames(field.name, current_id, node, field);
            field.renderedLabel = field.name;
            field.setAsChoiceGroup();

            WFF_addObjectToParent(tmp, active_tab, field, node);

            WFF_drawGroupBox(field, current_id, tmp, active_tab, node);
            break;

        //case 'static-ImageBox':   
        case 'StaticImage':
            //            var img_src = prompt("Insert url of the img you want to load","");
            //            
            //            if( img_src == null || typeof img_src == 'undefined' || img_src == "" )
            //                return;            
            var img_src = "";
            field = new WFE_field("WFF_added_element_" + current_id, "StaticImage", "StaticImage", node);
            field.setAsStaticImage("", img_src);

            WFF_CurrentImgField = field;

            $("#dialog_image_src").dialog('open');

            WFF_addObjectToParent(tmp, active_tab, field, node);
            WFF_drawStaticImage(field, current_id, tmp, active_tab, node);
            break;

        //case 'static-ImageBox':   
        case 'StaticHtmlCode':
            var code_src = "";
            field = new WFE_field("WFF_added_element_" + current_id, "StaticHtmlCode", "StaticHtmlCode", node);
            field.setAsStaticHtmlCode("EmbeddedHtmlCode", code_src);

            WFF_CurrentHtmlCodeField = field;

            $("#dialog_html_src").html("");
            $("#dialog_html_src").dialog('open');

            WFF_addObjectToParent(tmp, active_tab, field, node);
            WFF_drawStaticHtmlCode(field, current_id, tmp, active_tab, node);
            break;

        /*case 'static-HaltRow':
        render_control = '<div class="WFF_added_element"><hr></div>';
        tmp.find('#WFF_added_element_' + current_id).css({'position' : 'absolute' , 'top' : posy , 'left' : posx}).html('<div id="'+type_dragged+'" class="WFF_added_element_container">' + render_control + WFF_close_button_render(type_dragged, current_id,active_tab)+'</div>').draggable({grid: [25,25]});
        break;
        */ 

        case 'RadioButtonList':
            field = new WFE_field("WFF_added_element_" + current_id, type_dragged, type_dragged + current_id, node);

            WFF_addObjectToParent(tmp, active_tab, field, node);

            field.width = 200;
            field.height = 40;
            field.constraints = "";
            field.name = "Radio" + current_id;
            WFF_checkDoubleNames(field.name, current_id, node, field);
            field.renderedLabel = field.name;
            field.setAsRadioButton();

            field.children.push("Label");

            //Drawing the radio button
            WFF_drawRadioButton(field, current_id, tmp, active_tab, node);
            //WFF_makeResizable();

            break;

        default:    //is a 'normal' specialType
            field = new WFE_field("WFF_added_element_" + current_id, type_dragged, type_dragged + current_id, node);

            WFF_addObjectToParent(tmp, active_tab, field, node);

            field.width = 150;
            field.height = 40;
            field.constraints = "";
            field.name = "Label" + current_id;
            WFF_checkDoubleNames(field.name, current_id, node, field);
            field.renderedLabel = field.name;

            //Drawing the field
            WFF_drawField(field, current_id, tmp, active_tab, node);
            break;
    }


    var deltaheight = $("#" + field.id).height();

    //Check if widget was dragged either in a Group or not and adjust the parent height
    WFF_adjustParentHeight(field, 'add', deltaheight);

    //Makes all the dropped controls selectable
    WFF_makeSelectable(active_tab);
    WFF_makeSortable(active_tab);



    //Setting the width
    var width = tmp.find('#WFF_added_element_' + current_id).find('div.WFF_added_element_container').width();
    tmp.find('#WFF_added_element_' + current_id).width(width);
}

/**
* Finds last object created into the tab identified by the tmp object
*/
WFF_findLastObj = function(tmp, current_id, dragged_type) {
    if (!tmp.hasClass('WFF_ui_added_content'))
    {
        tmp.addClass('WFF_ui_added_content');
        tmp.html("");   
    }
    var first_id = "WFF_added_element_" + current_id;
    tmp.append('<li id="' + first_id + '" class="WFF_element_block"></li>');
        
    WFF_makeSelectable(tmp);
    WFF_makeSortable(tmp);
    
}

/**
* Draws a static-label into its html parent element
*/
WFF_drawStaticLabel = function(field, current_id, tmp, active_tab, node) {
    var type_dragged = 'static-LabelBox';
    var render_control = '<div class="WFF_added_element"><input type="text" id="WFF_static_label' + current_id + '" class="WFF_static_label_box" value="'+field.renderedLabel+'" onkeyup="WFF_expandLabelOnChange(\'#WFF_static_label'+current_id+'\');" onClick="$(this).focus();" onblur="WFF_setStaticLabelValue($(\'#WFF_static_label'+current_id+'\').attr(\'value\'),\''+field.id+'\',\''+node.getId()+'\');" /><script type="text/javascript">$("#WFF_static_label' + current_id + '").focus();</script></div>';

    tmp.find('#WFF_added_element_' + current_id).html('<div id="' + type_dragged + '" class="WFF_added_element_container">' + render_control + WFF_close_button_render(type_dragged, current_id, active_tab) + '</div>').find('.WFF_static_label_box').addClass('WFF_ui_added_' + type_dragged);
}

/**
* Draws a static-image into its html parent element
*/
WFF_drawStaticImage = function(field, current_id, tmp, active_tab, node) {
    //var type_dragged = 'static-ImageBox';
    //type???????
    //var render_control = '<div class="WFF_added_element"><input type="text" id="WFF_static_label' + current_id + '" class="WFF_static_label_box" value="'+field.renderedLabel+'" onkeyup="WFF_expandLabelOnChange(\'#WFF_static_label'+current_id+'\');" onClick="$(this).focus();" onblur="WFF_setStaticLabelValue($(\'#WFF_static_label'+current_id+'\').attr(\'value\'),\''+field.id+'\',\''+node.getId()+'\');" /><script type="text/javascript">$("#WFF_static_label' + current_id + '").focus();</script><img src="'+field.imgSrc+'" class="WFF_static_image_resized" /></div>';
    var render_control = '<div class="WFF_added_element"><img src="'+field.imgSrc+'" class="WFF_static_image_resized" /></div>';
    
    var preview_rendering = field.imgSrc;
    WFF_drawObject(field, current_id, tmp, active_tab, node, render_control, preview_rendering);
}

/**
* Draws a static-html embedded code into its html parent element
*/
WFF_drawStaticHtmlCode = function(field, current_id, tmp, active_tab, node) {
    //var render_control = '<div class="WFF_added_element"><input type="text" id="WFF_static_label' + current_id + '" class="WFF_static_label_box" value="'+field.renderedLabel+'" onkeyup="WFF_expandLabelOnChange(\'#WFF_static_label'+current_id+'\');" onClick="$(this).focus();" onblur="WFF_setStaticLabelValue($(\'#WFF_static_label'+current_id+'\').attr(\'value\'),\''+field.id+'\',\''+node.getId()+'\');" /><script type="text/javascript">$("#WFF_static_label' + current_id + '").focus();</script><img src="'+field.imgSrc+'" class="WFF_static_image_resized" /></div>';
    var render_control = '<div class="WFF_added_element">CODE EMBEDDED</div>';
    
    //var preview_rendering = field.imgSrc;
    //WFF_drawObject(field, current_id, tmp, active_tab, node, render_control, preview_rendering);
    WFF_drawObject(field, current_id, tmp, active_tab, node, render_control);
}


/**
* Draws a field into its html parent element
*/
WFF_drawField = function(field, current_id, tmp, active_tab, node) {
    var render_control = $('#uppanel').find('#' + field.baseType).html();
    
    //get the src of the preview rendering image to show on mouse over
    var preview_rendering = $('#renderingpanel').find('#' + field.baseType).find('img').attr('src');    

    WFF_drawObject(field, current_id, tmp, active_tab, node, render_control, preview_rendering);

    //Load all the predicates and operations relative to the current dragged element type in Arrays 
    WFF_loadType(field.baseType);
}

/**
* Draws a radioButtonList into its html parent element
*/
WFF_drawRadioButton = function(field, current_id, tmp, active_tab, node) {
    render_control = '<div class="WFF_added_element"><div id="WFF_radio_button_list' + current_id + '" class="WFF_radio_button_list"></div></div>';
    
    //Load all the predicates and operations relative to the current dragged element type in Arrays 
    WFF_loadType(field.baseType);

    WFF_drawObject(field, current_id, tmp, active_tab, node, render_control);
    
    //adjust height
    $('#WFF_radio_button_list'+current_id).height(field.height);
    
    //Adding + & - buttons
    $("#WFF_radio_button_list" + current_id).append('<input class=\"addradiobutton WFF_radio_control_plus\" type=\"button\" value="" onclick=\"WFF_addRadioButton(\'' + current_id + '\',\'' + field.id + '\',\'' + node.getId() + '\');\" /><input class=\"removeradiobutton WFF_radio_control_minus\" type=\"button\" value="" onclick=\"WFF_removeLastRadioButton(\'' + current_id + '\',\'' + field.id + '\',\'' + node.getId() + '\');\" /><br />');
    
    //Drawing radio button children
    for (var i = 0; i < field.children.length; i++) {
        $("#WFF_radio_button_list" + current_id).append('<img src="../lib/css/WorkflowEditor/WFF_img/RadioButton.png" height="10px"/>' + "<input id=\"WFF_radio_button_value_" + i + "\" type=\"text\" class=\"WFF_label_box\" value=\"" + field.children[i] + "\" onchange=\"var value = $(this).attr('value');WFF_changedRadioValue('" + field.id + "','" + node.getId() + "',value,'" + i + "');WFF_addConstraintsToRadioButton('" + current_id + "','" + field.id + "','" + node.getId() + "');\" onblur=\"WFF_addConstraintsToRadioButton('" + current_id + "','" + field.id + "','" + node.getId() + "');\" onClick=\"$(this).focus();\" />" + '<br />');
    }

    WFF_addConstraintsToRadioButton(current_id, field.id, node.getId());

    //Load all the predicates and operations relative to the current dragged element type in Arrays 
    WFF_loadType(field.baseType);

    WFF_makeSelectable(active_tab);
    WFF_makeSortable(active_tab);
}

/**
* Draws a groupBox into its html parent element
*/
WFF_drawGroupBox = function(field, current_id, tmp, active_tab, node) {
    //TO-CHANGE: width / height
    render_control = '<div class="WFF_added_element"><ul id="WFF_static_group' + current_id + '" class="WFF_static_group_box"></ul></div>';

    WFF_drawObject(field, current_id, tmp, active_tab, node, render_control);
    $('#'+field.id).addClass('WFF_adjusted_margin_group');
    $('#WFF_static_group' + current_id).html("<li id=\"WFF_empty\" style=\"margin-top: 13px;\"><span>Group is empty.. Move here the elements you want to add..</span></li>");

    WFF_makeGroupDroppable(field, node.getId());

    //Groups now automatically resized when objects are dragged into them
    //Resizing Group to saved size:

    field.height = 30;
    for (var h = 0; h < field.children.length; h++) field.height += field.children[h].height;
    $("#" + field.id).find(".WFF_static_group_box").height(field.height);

    //$("#WFF_static_group"+current_id).width(field.width);

    //WFF_makeResizable();
    ///WFF_makeSortable($("#" + field.id).find(".WFF_static_group_box"));
}

/**
 * Draws an object into its html parent element (it works for all types, static-label included)
 */
WFF_drawObject = function(field, current_id, tmp, active_tab, node, render_control,preview_rendering) {
    
    //modify last added container inserting: Label + ElementBox + CloseButton
    tmp.find("#" + field.id).html('<div id="' + field.baseType + '" class="WFF_added_element_container">' + WFF_label_render(field.renderedLabel.split(current_id)[0], current_id, node, field) + render_control + WFF_close_button_render(field.baseType, current_id, active_tab) + '</div>').find('.WFF_label_box').addClass('WFF_ui_added_' + field.baseType);
    $('#'+field.id).find('img').addClass('WFF_preview_image');
    if(preview_rendering != null){
        /*
        var old_src = $('#'+field.id).find('.WFF_preview_image').attr('src');
        $('#'+field.id).find('.WFF_preview_image').attr('onmouseover',
            "$(this).attr('src','"+preview_rendering+"');"
        );
        $('#'+field.id).find('.WFF_preview_image').attr('onmouseout',
            "$(this).attr('src','"+old_src+"');"
        );
        */
        var old_src = $('#'+field.id).find('.WFF_preview_image').attr('src');
        $('#'+field.id).find('.WFF_preview_image').tooltip({
            bodyHandler: function() {
                return $('<img/>').attr('src', preview_rendering);
            },
            showURL: false,
            top: -30,
            track: true
        });

    }
    $('#'+field.id).find('input').addClass('WFF_added_element');
    field.height = tmp.find("#" + field.id).height();

}

WFF_addObjectToParent = function( tmp, active_tab, field, node ) {
    if (('#' + tmp.attr('id')) == active_tab)
    {
        node.addField(field);
        return;
    } 
    else 
    {
        var parentID = tmp.parent().parent().parent().attr('id');
        var parentField = node.getFieldFromId(parentID);
        field.fieldRelative = parentField;
        parentField.children.push(field);
    }
}

function WFF_expandLabelOnChange(label_id){
    var size = $(label_id).attr('value').length+1;
    if(size<10)size=10;
    $(label_id).attr('size', size);
}

WFF_addRadioButton = function(current_id,field_id,node_id){
    var field = WFG_getNode(node_id).getFieldFromId(field_id);
    var deltaheight = $("#"+field.id).height();
    field.children.push("Label");
    var last  = field.children.length-1;

    $("#WFF_radio_button_list" + current_id).append('<img src="../lib/css/WorkflowEditor/WFF_img/RadioButton.png" height="10px"/>' + "<input id=\"WFF_radio_button_value_"+last+"\" type=\"text\" class=\"WFF_label_box\" value=\"" + field.children[last] + "\" onchange=\"var value = $(this).attr('value');WFF_changedRadioValue('"+field_id+"','"+node_id+"',value,'"+last+"');WFF_addConstraintsToRadioButton('"+current_id+"','"+field_id+"','"+node_id+"');\" onBlur=\"WFF_addConstraintsToRadioButton('"+current_id+"','"+field_id+"','"+node_id+"');\" onClick=\"$(this).focus();\" />" + '<br />');
    var height = $('#WFF_radio_button_list'+current_id).height()+40;
    $('#WFF_radio_button_list'+current_id).height(height);
    //Saving height in the field
    field.height = height;
    deltaheight = $("#"+field.id).height() - deltaheight;
    WFF_adjustParentHeight(field,'add',deltaheight);
    WFF_addConstraintsToRadioButton(current_id,field_id,node_id);
}

WFF_removeLastRadioButton = function(current_id,field_id,node_id){
    var field = WFG_getNode(node_id).getFieldFromId(field_id);
    if(field.children.length>1){
        var deltaheight = $("#"+field.id).height();
        var removed = field.children.pop();
        var index = field.children.length;
        $("#" + field_id).find("br").eq(index).nextAll().remove();
        var height = $('#WFF_radio_button_list'+current_id).height()-40;
        $('#WFF_radio_button_list'+current_id).height(height);
        //Saving height in the field
        field.height = height;
        deltaheight = deltaheight - $("#"+field.id).height();
        WFF_adjustParentHeight(field,'rem',deltaheight);
    }
}

WFF_changedRadioValue = function(field_id,node_id,value,radioid){
    var field = WFG_getNode(node_id).getFieldFromId(field_id);
    field.children[radioid]=value;
}

WFF_addConstraintsToRadioButton = function(current_id,field_id,node_id) {
    var field = WFG_getNode(node_id).getFieldFromId(field_id);
    field.constraints = "";
    var value = "";
    for(var i=0;i<field.children.length;i++){
        //value = WFS_EncodeName($("#WFF_radio_button_list" + current_id).find("#WFF_radio_button_value_"+i).attr('value'));
        value = WFS_XmlEncode($("#WFF_radio_button_list" + current_id).find("#WFF_radio_button_value_"+i).attr('value'));
        field.constraints += "addOption#"+value;
        if(i<(field.children.length-1)) field.constraints += "@";
    }
    //alert(field.constraints);
}

function WFF_makeSortable(active_div) {
    $(active_div).sortable({
        revert: true,
        update: function(event, ui) {            
            var sorted_item = ui.item.attr('id');
            var node = WFG_getNode(WFE_GetCurrentNodeSelectedID());
            var field = node.getFieldFromId(sorted_item);
            if(field==null) alert('DEBUG: [WFF_makeSortable] field is null');
            var array_to_sort = (field.fieldRelative == null) ? node.field : field.fieldRelative.children;
            var array_sorted_to_copy = (field.fieldRelative == null) ? $("#tab-" + node.getId()).sortable('toArray') : $('#' + field.fieldRelative.id).find('.WFF_static_group_box').sortable('toArray');
            WFF_sort_fields(array_to_sort, array_sorted_to_copy);
        },
        stop: function(event,ui){
            var sorted_item = ui.item.attr('id');
            $("#"+sorted_item).find(".WFF_label_box").focus();
        },
        containment: '#content-border',
        distance: 40,
        axis: 'y',
        cancel: '.WFF_close-button,.WFF_label_box,.addradiobutton,.removeradiobutton'
    });
}

function WFF_adjustParentHeight(field,op,deltaheight){
    if(field.fieldRelative != null){
        //alert("Field: "+field.name+" deltaheight: "+deltaheight);
        switch(op){
            case 'add':
                field.fieldRelative.height += (deltaheight+10); //field.height
                break;
            case 'rem':
                field.fieldRelative.height -= (deltaheight+10);
                break;
        }
        field.fieldRelative.setGroupHeight(field.fieldRelative.height);
        WFF_adjustParentHeight(field.fieldRelative,op,deltaheight);       
        //field.setHeightForContainerGroup(height);
        //$('#'+field.fieldRelative.id).find(".WFF_static_group_box").eq(0).height(field.fieldRelative.height);
    }
}

//Sorting all fields in nodes and group
WFF_sort_fields = function(array_to_sort,array_sorted_to_copy){

    function sortNumber(a, b)
    {
        return parseInt(a.sortedIndex.split('WFF_added_element_')[1]) - parseInt(b.sortedIndex.split('WFF_added_element_')[1]);
    }

    for(var ind=0;ind<array_sorted_to_copy.length;ind++){
        for(var j=0;j<array_to_sort.length;j++){
            if(array_to_sort[j].id == array_sorted_to_copy[ind]){
                array_to_sort[j].sortedIndex = "WFF_added_element_"+ind;
                break;
            }
        }
    }
    
    array_to_sort.sort(sortNumber);
}