var WFB_status_modified = false;

//Managing buttons:

//Cleaning buttons menu
WFB_cleanMenu = function(){
    $("#WFE_WidgetBoxButtons").html("");    //Deleting the Create New Form button
}
 
WFB_addSaveButton = function(){
    $("#WFE_WidgetBoxButtons").append('<p><input value="Save" type="button" class="ui-widget WFE_button" onclick="WFS_SaveWorkflow();" onmousedown="$(this).addClass(\'WFE_button_clicked\');" onmouseup="$(this).removeClass(\'WFE_button_clicked\');" alt="Save the model in the system" title="Save the model in the system" /><img src="/lib/image/managefiles.png" class="WFE_button_img" /></p>');
}

//Aggiungere call back
WFB_addPublishButton = function(){
    $("#WFE_WidgetBoxButtons").append('<p><input type="button" style="margin-left: 27px;" class="ui-widget WFE_button" value="Publish" onclick="getCommListCall(\'\',\'\')" onmousedown="$(this).addClass(\'WFE_button_clicked\');" onmouseup="$(this).removeClass(\'WFE_button_clicked\');" alt="Publish the model to let users fill it" title="Publish the model to let users fill it" /><img src="/lib/css/WorkflowEditor/WFF_img/web.png" class="WFE_button_img" /></p>');
}

WFB_addThemeEditorButton = function() {
$("#WFE_WidgetBoxButtons").append('<p><a class="WFE_menu_button_link" id="ThemeEditorLink" href="../ThemeEditor/ThemeEditor.aspx" style="color: White;"><input type="button" style="margin-left: 27px;" class="ui-widget WFE_button" value="Theme Editor" onmousedown="$(this).addClass(\'WFE_button_clicked\'); WFE_setReturnUrlCookie(THEME_EDITOR_RETURN_URL_FROM_WFE);" onmouseup="$(this).removeClass(\'WFE_button_clicked\');" alt="Edit the theme for the publication" title="Edit the theme for the publication" /><img src="/lib/css/WorkflowEditor/WFF_img/palette.png" class="WFE_button_img" /></a></p>');
}

WFB_addSendToContactsButton = function(){
    $("#WFE_WidgetBoxButtons").append('<p><input type="button" style="margin-left: 27px;" class="ui-widget WFE_button" value="Send To Contacts" onclick="getUserGCall(-200,\'\');clickContactUser();" onmousedown="$(this).addClass(\'WFE_button_clicked\');" onmouseup="$(this).removeClass(\'WFE_button_clicked\');" /></p>');
}

WFB_addCreateWorkflowButton = function(){
$("#WFE_WidgetBoxButtons").append('<p><input class="ui-widget WFE_button WFE_createWorkflowButton" type="button" value="Create Form"  onclick="WFG_addWorkflow();" onmousedown="$(this).addClass(\'WFE_button_clicked\');" onmouseup="$(this).removeClass(\'WFE_button_clicked\');" /><img src="/lib/css/WorkflowEditor/WFF_img/icon_new.png" class="WFE_button_img WFE_createWorkflowButton" /></p>');
}

WFB_addTutorialWorkflowButton = function(){
    $("#WFE_WidgetBoxButtons").append('<p><br /></p><p><input class="ui-widget WFE_button" type="button" value="Tutorial"  onclick="WFG_ShowTutorial();" onmousedown="$(this).addClass(\'WFE_button_clicked\');" onmouseup="$(this).removeClass(\'WFE_button_clicked\');" alt="Shows a brief tutorial that explains how to use the application" title="Shows a brief tutorial that explains how to use the application" /><img src="/lib/css/WorkflowEditor/WFF_img/video_tutorial.png" class="WFE_button_img" /></p>');
}

WFB_addDraggableNodeButton = function(){
    $("#WFE_WidgetBoxButtons").append('<p><div id="WFE_addNodeDraggable" class="WFE_addNodeDraggable" alt="Drag me to the grid to add a new step" title="Drag me to the grid to add a new step" ></div></p>');
    //Setting node button draggability
    WFB_setNodeButtonDraggability();
}

WFB_setNodeButtonDraggability = function() {
    //Setting node creation draggability
    $("#WFE_addNodeDraggable").draggable({ opacity: 0.7, helper: 'clone', containment: '#content-border' });
    $(".WFG_Canvas").droppable({
        accept: '#WFE_addNodeDraggable',
        drop: function(event, ui) {
            var pos = ui.position;
            WFG_addNode(pos.top, pos.left);
        }
    });
}

WFB_prepareButtonsEffect = function(start){
    if(start == null) start = 0;
    $("#WFE_WidgetBoxButtons").find("p").each(function(index){if(index>(start-1))$(this).hide();});
}

WFB_addingButtonsEffect = function(start){
    if(start == null) start = 0;
    $("#WFE_WidgetBoxButtons").find("p").each(function(index){
        if(index>(start-1)){
            $(this).fadeIn(1000*(index-start)); //show('drop',{},500,null);
        }
    });
}

WFB_addCheckImageToButton = function(button_name) {
    var html_to_append = '<img id="WFE_check_'+button_name+'" src="/lib/css/WorkflowEditor/WFF_img/check.png" class="WFE_checkop" />';
    var button_list = $("#WFE_WidgetBoxButtons").find('input');
    for(var i=0;i<button_list.length;i++){
        if(button_list[i].value==button_name){
            button_list.eq(i).parent().append(html_to_append);
        }
    }
}

WFB_removeCheckImageToButton = function(button_name) {
    $("#WFE_check_"+button_name).remove();
}

WFB_removeAllCheckImage = function() {
    if(WFB_status_modified && ! WFS_restoreInProgress){
        /*
        var buttons = $("#WFE_WidgetBoxButtons").find('input');
        buttons.each(function(index){
            WFB_removeCheckImageToButton(buttons[index].value);
        });
        */
        WFB_cleanMenu();
        WFB_addDraggableNodeButton();
        WFB_addSaveButton();
        WFB_addTutorialWorkflowButton();
    }
}