function WFF_makeGroupDroppable(field,node_id){
    var tmp = $('#'+field.id).find('.WFF_static_group_box');
    tmp.droppable({
                accept: '.WFF_added_widget',
                greedy: true,
                drop: function(event, ui) {
                    WFF_createObject(ui,tmp,"#tab-"+node_id);
                    //WFF_makeResizable();
                }
    });
}

function WFF_makeResizable() {
    $('.WFF_static_group_box').resizable({
        stop: function(event,ui){
        
            //Recovering node
            var node = WFG_getNode(WFE_currentNodeSelectedID);
        
            //Recovering group field
            var field_id = "WFF_added_element_"+($(this).attr('id')).split('WFF_static_group')[1];
            var field = node.getFieldFromId(field_id);
            
            //Saving width & height to field object
            field.height = $(this).height();
            field.width = $(this).width();
        }
    });
}