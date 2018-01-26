
//var __width = 690;
//var __height = 450;
var __width = 685;
var __height = 585;

var WFG_top = 0;
var WFG_left = 0;

var WFG_workflowList; // workflow list
var WFG_workflow; // current workflow ref

var WFG_eventDoNode = null;
var WFG_eventDoArc = null;


//////////////////////////////////////////////

/////// events activable on node or arc //////

function WFG_activateEventOnNode(eventToDo) {
    WFG_eventDoNode = eventToDo;
}

function WFG_activateEventOnArc(eventTodo) {
    WFG_eventDoArc = eventTodo;
}

var WFG_remember = null;
WFG_addEdgeEvent = function(node) {
    if (WFG_remember == null) WFG_remember = node;
    else if (WFG_remember != node) {
        WFG_addEdge(WFG_remember, node);
        WFG_resetEventNode();
    }
}

WFG_removeNode = function(node) {
    WFB_status_modified = true;
    WFB_removeAllCheckImage();
    node.remove();
    WFG_resetEventNode();
}

WFG_removeArc = function(arc) {
    WFB_status_modified = true;
    WFB_removeAllCheckImage();
    arc.remove();
    WFG_resetEventArc();
}

WFG_resetEventArc = function() {
    WFG_eventDoArc = null;
}

WFG_resetEventNode = function() {
    WFG_eventDoNode = null;
    WFG_remember = null;
}

///////////////////////////////////////////////

/////// canvas mouse event ////////////////////

function WFG_onMouseMove(evt) {
    if (WFG_workflow.elementSelected != null) {
        WFG_workflow.elementSelected.move(evt);
    }
}

function WFG_rightClickMenuArcHide()
{
    var menu = document.getElementById("rightClickArc");
    menu.style.visibility="hidden";
}

function WFG_rightClickMenuNodeHide()
{
    var menu = document.getElementById("rightClickNode");
    menu.style.visibility="hidden";
}

function WFG_onMouseUp(evt) {
    WFG_rightClickMenuArcHide();
    WFG_rightClickMenuNodeHide();
    WFG_unselect(false);

    if (WFG_eventDoArc != null) {
        var selected = WFG_workflow.edgePickCorrelation(evt);
        if (selected != null) {
            WFG_eventDoArc(selected);
        }
    }
//    else {
//        //modify node's xml rendering attributes
//        var nodetomodify = wfg_workflow.nodepickcorrelation(evt)
//        if (nodetomodify != null) {
//            var nodeid = nodetomodify.getid();
//            var nodename = nodetomodify.getname();
//            var point = nodetomodify.getpoint();
//            var width = nodetomodify.getwidth();
//            var height = nodetomodify.getheight();
//            wfs_modifienoderenderingattributes(wfg_workflow.getid(), nodeid, nodename, point['x'], point['y'], width, height );
//        }
//    }    
}

function WFG_ondblclick(evt) {
    var selected = WFG_workflow.edgePickCorrelation(evt);
    if (selected != null) {
        WFG_select(selected);
        WFE_openDialog(selected);
    }
}

function WFG_onRightClick(evt)
{
    var selected=WFG_workflow.edgePickCorrelation(evt);
    if(selected!=null)
    {
        WFG_select(selected);
        var menu = document.getElementById("rightClickArc");
        menu.style.top = evt.layerY+"px";
        menu.style.left = (evt.screenX - 100) + "px";
        menu.style.visibility = "Visible";
        menu.style.zIndex=100;
    } else 
    {
        selected=WFG_workflow.nodePickCorrelation(evt);
        if(selected!=null)
        {
            var menu = document.getElementById("rightClickNode");
            menu.style.top = evt.layerY+"px";
            menu.style.left = (evt.screenX - 100) + "px";
            menu.style.visibility = "Visible";
            menu.style.zIndex=100;
        }
    }
    return false;
}

///////////////////////////////////////////////

//////////////// utility //////////////////////

// convert document coordinate in canvas coordinate
function WFG_canvasCoordinate(element, evt) {
    var evt = window.event || evt; // cross browser compatibility

    el = this;
    
    if ($.browser.msie) 
    {
        el.clientY = evt.offsetY ;
        el.clientX = evt.offsetX ;
    }else
    {
        el.clientY = evt.layerY - element.offsetTop;
        el.clientX = evt.layerX - element.offsetLeft;
    }
}

function WFG_addNode(top, left) {
    WFB_status_modified = true;
    WFB_removeAllCheckImage();
    var nodeName = "";
    WFG_top = top;
    WFG_left = left;    

    $("#dialog_node_name").dialog('open'); 
}

function WFG_addEdge(nodeFrom, nodeTo) {
    WFB_status_modified = true;
    WFB_removeAllCheckImage();
    var control = nodeFrom.getEdge();
    var eye = true;
    for (var i = 0; eye && i < control.length; i++) if (control[i].to.isEqual(nodeTo)) eye = false;

    control = nodeTo.getEdge();
    for (var i = 0; eye && i < control.length; i++) if (control[i].to.isEqual(nodeFrom)) eye = false;

    if (eye) {
        edgeToInsert = new WFG_edge(nodeFrom, nodeTo);
        edgeToInsert.follow();
        nodeFrom.addedge(edgeToInsert);
        nodeTo.addedge(edgeToInsert);

        WFG_workflow.edgeCounter++;
        
        WFS_AddEdge(WFG_workflow.getId(), nodeFrom.getId(), nodeTo.getId());    //Adding the edge to the WF's Status
    }
}

function WFG_getNode(id) {
    return WFG_workflow.getNode(id);
}

function WFG_getAllNode() {
    return WFG_workflow.element;
}

function WFG_setSizeCanvas(width, height) {
    WFG_workflow.setSizeCanvas(width, height);
}

function WFG_select(item) {
    WFG_unselect(true);
    WFG_workflow.elementSelected = item;
    item.select();
    WFF_deselectAll();
}

function WFG_unselect(forced) {
    if (WFG_workflow.elementSelected != null) WFG_workflow.elementSelected.unselect(forced);
}

////////////////////////////////////////////////

/////////// workflow ///////////////////////////
function WFG_addWorkflow() {    
    //Getting the WF' Name
    $(function() {
        $('#dialog_to_set_WF_name').dialog('open');
    });
}

function WFG_ShowTutorial(){
    $("#tutorial_dialog").removeClass('hiddenpanel').dialog('open');;
    //open("/FormFillier/TutorialWFE.aspx","","",false);
    //location.href = "/FormFillier/TutorialWFE.aspx";
}

function WFG_AddWorkflowCanvasTab(wfID, wfName, idElement) {
    $('#tabs').append('<div id="status_error"><div id="tab-' + wfID + '"><div id="' + idElement + '" class="WFG_Canvas WFE_workflow" style="background-color: #FFFFFF;"></div></div></div>');
    $("#tabs").tabs('add', "#tab-" + wfID, wfName, WFE_openedTabs++).addClass('tabFont').addClass('WFE_form');
}

function WFG_workflow(wfID, idElement,wfName) {
    var el = this;
    el.idElement = idElement; 
    el.wfID = wfID;
    el.paper = Raphael(idElement, __width, __height);    
    el.elementSelected = null;
    el.element = new Array();
    el.name = wfName;
    el.nodeCounter = 0;
    el.edgeCounter = 0;

    // print grid
    var dim = 20;
    for (var i = 0; i <= __height; i += dim)
        el.paper.path({ stroke: "#C0C0C0", opacity: 1 }).moveTo(0, i).lineTo(1000, i);
    for (var i = 0; i <= 1000; i += dim)
        el.paper.path({ stroke: "#C0C0C0", opacity: 1 }).moveTo(i, 0).lineTo(i, __height);


    var elementDom = document.getElementById(el.idElement);
    elementDom.onmousemove = function(evt) { WFG_onMouseMove(evt); }
    elementDom.onmouseup = function(evt) { WFG_onMouseUp(evt); }
    elementDom.ondblclick = function(evt) { WFG_ondblclick(evt); }
    elementDom.oncontextmenu=function(evt){ return WFG_onRightClick(evt);}

    el.getId = function() {
        return wfID;
    }
    
    el.getName = function() {
        return el.name;
    }

    el.getNode = function(id) {
        for (var i = 0; i < el.element.length; i++) {
            if (el.element[i].getId() == id) return el.element[i];
        }
        return null;
    }

    el.getNodeByName = function(name) {
        for (var i = 0; i < el.element.length; i++) {
            if (el.element[i].getName() == name) return el.element[i];
        }
        return null;
    }

    el.getCanvasOffset = function() {
        return $(WFG_workflow.getId() + "_Canvas").offset();
    }

    el.setSizeCanvas = function(width, height) {
        el.paper.setSize(width, height);
    }

    el.getNodeNumber = function() {
        return el.element.length;
    }

    el.getNewNodeID = function() {
        return wfID + WFE_nodeID + (++(el.nodeCounter));
    }

    el.addNode = function(nodeName, top, left) {    
        var idNode = el.getNewNodeID();

        //Fixing coordinates
        var canvasPos = el.getCanvasOffset();
        var x = left - WFE_layout.state.west.size + 30 - canvasPos.left;

        if (WFE_layout.state.west.isSliding)
            x += WFE_layout.state.west.size;

        var y = top - 15 - canvasPos.top;

        var nodeNew = new WFG_graphNode(nodeName, idNode, el, x, y);
        el.element.push(nodeNew);
        WFG_resetEventNode();

        WFS_AddNode(el.getId(), idNode, nodeName, x, y, nodeNew.getWidth(), nodeNew.getHeight());    //Adding the node to the WF Status
    }

    el.addExistingNode = function(newNode) {
        el.element.push(newNode);
        WFG_resetEventNode();
    }

    el.removeNode = function(node) {
        for (var i = 0; i < el.element.length; i++) {
            if (el.element[i].isEqual(node)) {
                el.element.splice(i, 1);
                break;
            }
        }
        WFE_removeFormTab(node.getName(), node.getId());
    }

    el.edgePickCorrelation = function(evt) {
        var selected = null;
        var offset = new WFG_canvasCoordinate(document.getElementById(el.idElement), evt);
        for (var aggiungi = 0; aggiungi < el.element.length && selected == null; aggiungi++) {
            // return null se non trova, altrimenti l'edge giusto
            selected = el.element[aggiungi].edgePickCorrelation(offset);
        }
        return selected;
    }

    el.nodePickCorrelation = function(evt) {
        var toReturn = null;
        var offset = new WFG_canvasCoordinate(document.getElementById(el.idElement), evt);
        for (var i = 0; i < el.element.length && toReturn == null; i++) {
            if (el.element[i].contains(offset)) toReturn = el.element[i];
        }        
        return toReturn;
    }
    
    el.getCanvasWidth = function(){
        return el.paper.width;
    }

    el.getCanvasHeight = function(){
        return el.paper.height;
    }
}

////////////////////////////////////////////////

/////////// edge ///////////////////////////////

function WFG_edge(nodeFrom, nodeTo) {
    var el = this;

    el.from = nodeFrom;
    el.to = nodeTo;
    el.precondition = "<PRECONDITIONS></PRECONDITIONS>"
    var elRaphael = WFG_workflow.paper.path({ stroke: "black" }).moveTo(el.from["x"], el.from["y"]).lineTo(el.to["x"], el.to["y"]);
    var elRaphaelArrow = WFG_workflow.paper.path({ stroke: "black" }).moveTo(el.from["x"], el.from["y"]).lineTo(el.to["x"], el.to["y"]);

    el.predicateList=new Array();
    el.predicateHtml=null;

    elRaphael.toBack();

    el.getPredicate=function(id)
    {
        for(var i=0;i<el.predicateList.length;i++)
        {
            if(el.predicateList[i].id==id)
            {
                return el.predicateList[i];
            }
        }
        return null;
    }
    
    el.isLastPredicate=function(predicate)
    {
        if(el.predicateList.length>0) return el.predicateList[el.predicateList.length-1].id==predicate.id;
        return false;
    }
    
    el.removePredicateAfter=function(predicate)
    {
        for(var i=0;i<el.predicateList.length;i++)
        {
            if(el.predicateList[i].id==predicate.id)
            {
                for(var j=i+1;j<el.predicateList.length;j++) WFE_remove_row(el.predicateList[j].id);
                el.predicateList.splice(i+1,el.predicateList.length-i+1);
                break;
            }    
        }
    }

    el.follow = function(evt) {
        var fromPoint = el.from.getNearestPoint(el.to.getPoint(), 0);
        var toPoint = el.to.getNearestPoint(el.from.getPoint(), 1);

        var arrowType = ["M " + toPoint["x"] + " " + (toPoint["y"] + 10) + " L " + (toPoint["x"] - 10) + " " + (toPoint["y"]) + " M " + (toPoint["x"] - 10) + " " + (toPoint["y"]) + " L " + (toPoint["x"] + 10) + " " + (toPoint["y"]) + " M " + (toPoint["x"] + 10) + " " + (toPoint["y"]) + " L " + toPoint["x"] + " " + (toPoint["y"] + 10) + "",
        "M " + (toPoint["x"] - 10) + " " + toPoint["y"] + " L " + (toPoint["x"]) + " " + (toPoint["y"] - 10) + " M " + (toPoint["x"]) + " " + (toPoint["y"] - 10) + " L " + (toPoint["x"]) + " " + (toPoint["y"] + 10) + " M " + (toPoint["x"]) + " " + (toPoint["y"] + 10) + " L " + (toPoint["x"] - 10) + " " + toPoint["y"] + "",
        "M " + toPoint["x"] + " " + (toPoint["y"] - 10) + " L " + (toPoint["x"] + 10) + " " + (toPoint["y"]) + " M " + (toPoint["x"] + 10) + " " + (toPoint["y"]) + " L " + (toPoint["x"] - 10) + " " + (toPoint["y"]) + " M " + (toPoint["x"] - 10) + " " + (toPoint["y"]) + " L " + toPoint["x"] + " " + (toPoint["y"] - 10) + "",
        "M " + (toPoint["x"] + 10) + " " + toPoint["y"] + " L " + (toPoint["x"]) + " " + (toPoint["y"] - 10) + " M " + (toPoint["x"]) + " " + (toPoint["y"] - 10) + " L " + (toPoint["x"]) + " " + (toPoint["y"] + 10) + " M " + (toPoint["x"]) + " " + (toPoint["y"] + 10) + " L " + (toPoint["x"] + 10) + " " + toPoint["y"] + ""];
        elRaphaelArrow.attr({ path: arrowType[toPoint["type"]] });

        elRaphael.attr({ path: "M " + fromPoint["x"] + " " + fromPoint["y"] + " L " + toPoint["x"] + " " + toPoint["y"] });
    }

    el.isEqual = function(edge) {
        return el.from.isEqual(edge.from) && el.to.isEqual(edge.to);
    }

    el.setPrecondition = function(val) {
        el.precondition = val;
    }

    el.move = function(evt) {
    }

    /*  return: boolean
    true: if mouse event is nearer than tolerance from the arc's line    */
    el.isNear = function(evt) {
        var p0 = el.from.getNearestPoint(el.to.getPoint(), 0);
        var p1 = el.to.getNearestPoint(el.from.getPoint(), 1);
        var tolerance = 10;

        if (evt.clientX < (Math.max(p0["x"], p1["x"]) + tolerance) && evt.clientX > (Math.min(p0["x"], p1["x"]) - tolerance) && evt.clientY > (Math.min(p0["y"], p1["y"]) - tolerance) && evt.clientY < (Math.max(p0["y"], p1["y"]) + tolerance)) {
            var m = (p1["y"] - p0["y"]) / (p1["x"] - p0["x"]);
            var q = p0["y"] - m * p0["x"];

            var distance = Math.abs(-m * evt.clientX + evt.clientY - q) / Math.sqrt(Math.pow(m, 2) + 1);
            return distance < tolerance;
        } else
            return false;
    }

    el.select = function() {
        elRaphael.attr({ stroke: "blue" });
        elRaphaelArrow.attr({ stroke: "blue" });
    }

    el.unselect = function(forced) {
        if (forced) {
            elRaphael.attr({ stroke: "black" });
            elRaphaelArrow.attr({ stroke: "black" });
        }
    }

    el.remove = function() {
        elRaphael.remove();
        elRaphaelArrow.remove();
        el.from.removeEdge(el);
        el.to.removeEdge(el);

        WFG_workflow.edgeCounter--;

        WFS_RemoveEdge(el.from.workflowRelative.getId(), el.from.getId(), el.to.getId()); //Removing the edge from the WF's status
    }
}

////////////////////////////////////////////////

/////////// node handle ////////////////////////

function WFG_nodeHandle(graphNodeRelative, type) {
    var el = this;
    var nodeRelative = graphNodeRelative;
    var type = type;

    var nodePoint = nodeRelative.getPoint();
        var handleType = [{ "x": -15, "y": -15 }, { "x": nodeRelative.getWidth() + 5, "y": -15 }, { "x": -15, "y": nodeRelative.getHeight() + 5 }, { "x": nodeRelative.getWidth() + 5, "y": nodeRelative.getHeight() + 5}];
        el.elRaphael = WFG_workflow.paper.rect(nodePoint["x"] + handleType[type]["x"], nodePoint["y"] + handleType[type]["y"], 10, 10);
        el.elRaphael.attr({ fill: "white", opacity: 0 });
        el.elRaphael.node.onmousedown = function(evt) { onMouseDown(evt); }
        el.elRaphael.node.onmouseup = function(evt) { onMouseUp(evt); }

    el.moving = false;
    el.m_x;
    el.m_y;

    var point = new Array();
    point["x"] = el.elRaphael.attrs.x;
    point["y"] = el.elRaphael.attrs.y;

    function onMouseDown(evt) {
        var evt = window.event || evt; // cross browser compatibility
        el.moving = true;
        el.m_x = evt.clientX;
        el.m_y = evt.clientY;
        WFG_select(el);
    }

    function onMouseUp(evt) {
//        //If we are here, the node size has been modified, so we have to modify node's xml rendering attributes
//        var nodeID = nodeRelative.getId();
//        var nodeName = nodeRelative.getName();
//        var point = nodeRelative.getPoint();
//        var width = nodeRelative.getWidth();
//        var height = nodeRelative.getHeight();
//        WFS_ModifieNodeRenderingAttributes(WFG_workflow.getId(), nodeID, nodeName, point['x'], point['y'], width, height);
        
        el.moving = false;
    }

    el.hide = function() {
        el.elRaphael.attr({ opacity: 0 });
    }

    el.show = function() {
        el.elRaphael.attr({ opacity: 1 });
    }

    el.follow = function(evt) {
        var handleType = [{ "x": -15, "y": -15 }, { "x": nodeRelative.getWidth() + 5, "y": -15 }, { "x": -15, "y": nodeRelative.getHeight() + 5 }, { "x": nodeRelative.getWidth() + 5, "y": nodeRelative.getHeight() + 5}];
        var nodePoint = nodeRelative.getPoint();
        point["x"] = nodePoint["x"] + handleType[type]["x"];
        point["y"] = nodePoint["y"] + handleType[type]["y"];
        el.print();        
    }

    el.move = function(evt) {        
        if (el.moving) {
            var evt = window.event || evt; // cross browser compatibility
            var nodePoint = nodeRelative.getPoint();

            // mmmmm
            var controlHeight=true;
            var controlWidth=true;
            
            if (type == 3 || type == 1) controlWidth=controlWidth&&(nodeRelative.setWidth(nodeRelative.getWidth() + evt.clientX - el.m_x));
            if (type == 3 || type == 2) controlHeight=controlHeight&&(nodeRelative.setHeight(nodeRelative.getHeight() + evt.clientY - el.m_y));
            if ((type == 1 || type == 0) && (evt.clientY - el.m_y) < 0) controlHeight=controlHeight&&(nodeRelative.setHeight(nodeRelative.getHeight() + Math.abs(evt.clientY - el.m_y)));
            if ((type == 1 || type == 0) && (evt.clientY - el.m_y) > 0) controlHeight=controlHeight&&(nodeRelative.setHeight(nodeRelative.getHeight() - Math.abs(evt.clientY - el.m_y)));
            if ((type == 0 || type == 2) && (evt.clientX - el.m_x) < 0) controlWidth=controlWidth&&(nodeRelative.setWidth(nodeRelative.getWidth() + Math.abs(evt.clientX - el.m_x)));
            if ((type == 0 || type == 2) && (evt.clientX - el.m_x) > 0) controlWidth=controlWidth&&(nodeRelative.setWidth(nodeRelative.getWidth() - Math.abs(evt.clientX - el.m_x)));
            if ((type == 0 || type == 1)&& controlHeight) nodePoint["y"] += evt.clientY - el.m_y;
            if ((type == 0 || type == 2)&& controlWidth) nodePoint["x"] += evt.clientX - el.m_x;
            nodeRelative.setPoint(nodePoint["x"], nodePoint["y"]);

            // non serve più perchè adesso si blocca quando nodo raggiunge minWidth o minHeight però lascio...
            if (nodeRelative.getHeight() < 0) {
                nodeRelative.setPoint(nodePoint["x"] + nodeRelative.getHeight(), nodePoint["y"]);
                nodeRelative.setHeight(Math.abs(nodeRelative.getHeight()));
                nodeRelative.handleSwitchHeight();
            }
            if (nodeRelative.getWidth() < 0) {
                nodeRelative.setPoint(nodePoint["x"], nodePoint["y"] + nodeRelative.getWidth());
                nodeRelative.setWidth(Math.abs(nodeRelative.getWidth()));
                nodeRelative.handleSwitchWidth();
            }
            ////////

            el.m_x = evt.clientX;
            el.m_y = evt.clientY;
            nodeRelative.print();
        }
    }

    el.print = function() {
        el.elRaphael.attr({ x: point["x"], y: point["y"] });
    }

    el.getPoint = function() {
        return point;
    }

    el.select = function() {
        nodeRelative.show();
    }

    el.unselect = function(forced) {
        if (forced) nodeRelative.hide();
        else WFG_workflow.elementSelected = null;
    }

    /* Switch handle's type 0->2 1->3 2->0 3->1 following user vertical resizing */
    el.switchTypeHeight = function() {
        type = (type + 2) % 4;
    }

    /* Switch handle's type 0->1 1->0 2->3 3->2 following user horizontal resizing */
    el.switchTypeWidth = function() {
        if (type % 2 == 0) type += 1;
        else type = (type + 3) % 4;
    }

    el.remove = function() {
        el.elRaphael.remove();
    }
}

////////////////////////////////////////////////

/////////// arc button /////////////////////////

function WFG_arcButton(nodeRelative, point, width, height) {
    var el = this;
    el.type = "arc";
    el.elRaphael = WFG_workflow.paper.rect(point["x"] + width - 9, point["y"] + height - 9, 10, 10);
    el.elRaphael.attr({ fill: "gray", "stroke-width": "0px" });

    el.nodeRelative = nodeRelative;
    el.selected = false;

    el.line = WFG_workflow.paper.path({ stroke: "black" }).moveTo(point["x"], point["y"]).lineTo(point["x"], point["y"]);
    el.line.attr({ opacity: 0 });
    el.line.toBack();

    el.circle = null;
    el.circleNodeRelative = null;

    el.elRaphael.node.onmousedown = function(evt) { onMouseDown(evt); }

    function onMouseDown(evt) {
        WFG_select(el);
    }

    el.select = function() {
        el.selected = true;
        el.line.attr({ opacity: 1 });
    }

    el.unselect = function(forced) {
        el.selected = false;
        el.line.attr({ opacity: 0 });
        if (el.circle != null) {
            el.circle.remove();
            el.circle = null;
            if (el.circleNodeRelative != null) {
                WFG_addEdge(el.nodeRelative, el.circleNodeRelative);
            }
        }
    }

    el.move = function(evt) {
        if (el.selected) { 
            var nodeHover = el.nodeRelative.workflowRelative.nodePickCorrelation(evt);
            if (nodeHover != null && !nodeHover.isEqual(el.nodeRelative)) {
                if (el.circle == null) {
                    var nodeHoverPoint = nodeHover.getPoint();
                    var radius = Math.sqrt(Math.pow(nodeHover.getWidth(), 2) + Math.pow(nodeHover.getHeight(), 2)) / 2;
                    el.circle = el.nodeRelative.workflowRelative.paper.circle(nodeHoverPoint["x"] + nodeHover.getWidth() / 2, nodeHoverPoint["y"] + nodeHover.getHeight() / 2, radius);
                    el.circle.attr({ "stroke": "red", "stroke-width": "2px", "stroke-opacity": 0.5 });
                    el.circleNodeRelative = nodeHover;
                }
            } else if (el.circle != null) {
                el.circle.remove();
                el.circle = null;
                el.circleNodeRelative = null;
            }

            var offset = new WFG_canvasCoordinate(document.getElementById(el.nodeRelative.workflowRelative.idElement), evt);
            var nodePoint=el.nodeRelative.getNearestPoint({"x": offset.clientX,"y": offset.clientY},0);
            //var nodePoint = el.nodeRelative.getPoint();
            //el.line.attr({ path: "M " + (nodePoint["x"] + (nodeRelative.getWidth() / 2)) + " " + (nodePoint["y"] + (nodeRelative.getHeight() / 2)) + " L " + offset.clientX + " " + offset.clientY });
            el.line.attr({ path: "M " + (nodePoint["x"]) + " " + (nodePoint["y"]) + " L " + offset.clientX + " " + offset.clientY });
        }
    }

    el.follow = function(evt) {
        var nodePoint = el.nodeRelative.getPoint();
        el.point = { "x": nodePoint["x"] + nodeRelative.getWidth() - 9, "y": nodePoint["y"] + nodeRelative.getHeight() - 9 };
        el.print();
    }

    el.remove = function() {
        el.elRaphael.remove();
        el.line.remove();
        if (el.circle != null) el.circle.remove();
    }

    el.print = function() {
        el.elRaphael.attr({ x: el.point["x"], y: el.point["y"] });
    }
}

////////////////////////////////////////////////

/////////// node ///////////////////////////////

function WFG_graphNode(name, idNode, workflowRel, x, y) {
    var el = this;
    el.type = "node";

    var nameLength = name.length < 10 ? 10 : name.length;
    
    el.minHeight=50;
    el.minWidth=7 * nameLength;
    //var elRaphael = workflowRel.paper.image("../../css/WorkflowEditor/img/WFE_nodeDraggableImage.png", x, y, 10 * nameLength, 10 * nameLength);
    var elRaphael = workflowRel.paper.rect(x, y, el.minWidth, 7*nameLength, 10);
    elRaphael.attr({ "stroke-width": "1px" , "opacity":"0.6"});

    var id = idNode;
    el.workflowRelative = workflowRel;

    el.m_x;
    el.m_y;
    var moving = false;
    var status = changeStatus(0); // 0 not selected, 1 selected, 2 hover

    var handle = null;
    var edgeArray = new Array();

    var point = { "x": elRaphael.attrs.x, "y": elRaphael.attrs.y };

    //Calculating the name's padding
    var p = (nameLength * 3.5);
    var text = workflowRel.paper.text(point["x"] + p, point["y"] + p, name);
    text.attr({ "font-size": "8pt" });

    el.arcButton = new WFG_arcButton(el, point, elRaphael.attrs.width, elRaphael.attrs.height);

    /* node's field manager */

    el.field = new Array();
    
    /****/
    el.getFieldFromLabel = function(label) {
        var tmp = label.replace(/\'/g, "&apos;");
        label = tmp.replace(/"/g, "&quot;");        
        for (var i = 0; i < el.field.length; i++) {
            var tmp = el.field[i].getFromLabel(label);
            if (tmp != null) return tmp;
        }
        return null;
    }
    
    el.getFieldFromId=function(id)
    {
        for(var i=0;i<el.field.length;i++)
        {
            var tmp=el.field[i].getFromId(id);
            if(tmp!=null) return tmp;
        }
        return null; 
    }

    /******/
    el.addField = function(val) {
        el.field.push(val);
    }

    /*****/
    el.removeField = function(id) {
        for (var i = 0; i < el.field.length; i++) {
            if (el.field[i].id == id) {
                el.field.splice(i, 1);
                break;
            }
        }
    }

    //////////////////////////


    /*****/
    el.getFieldLabel = function() {
        var toReturn = new Array();
        for (var i = 0; i < el.field.length; i++) {
            if (el.field[i].specialType != WFE_special_type.TEXT && el.field[i].specialType != WFE_special_type.IMAGE && el.field[i].specialType != WFE_special_type.HTMLCODE)
                toReturn=toReturn.concat(el.field[i].getLabelArray());
            //toReturn.push(el.field[i].getLabel());
        }
        return toReturn;
    }

    /*****/
    // return lista label
    el.getFieldOfType = function(type) {
        var toReturn = new Array();
        for (var i = 0; i < el.field.length; i++) {
            toReturn=toReturn.concat(el.field[i].getOfType(type));
        }
        if (toReturn.length > 0) return toReturn;
        else return null;
    }

    /* end node's field manager */

    elRaphael.node.onmousedown = function(evt) { savePos(evt); }
    //elRaphael.node.onmouseup = function(evt){mouseUp(evt);}
    //elRaphael.node.onmouseout = function(evt){mouseOut(evt);}
    elRaphael.node.ondblclick = function(evt) { WFG_resetEventNode(); WFE_openFormTab(name, id); } 

    function createHandle() {
        for (var i = 0; i < 4; i++) {
            handle.push(new WFG_nodeHandle(el, i));
        }
    }

    function savePos(evt) {
        // sposta inizializzazione handle in altro posto
        if (handle == null) {
            handle = new Array();
            createHandle();
        }
        WFG_select(el);
        var evt = window.event || evt; // cross browser compatibility
        el.m_x = evt.clientX;
        el.m_y = evt.clientY;
        el.moving = true;
        changeStatus(1);
    }

    function unsavePos(evt) {
        el.moving = false;
    }

    function mouseOut(evt) {
        //unsavePos(evt);
        changeStatus(0);
    }

    function mouseUp(evt) {
        unsavePos(evt);
        changeStatus(2);
    }

    function mouseMove(evt) {
        if (el.status == 0) changeStatus(2);
        if (el.moving) {
            var evt = window.event || evt; // cross browser compatibility
            var cc = el.getPoint();

            ///// griglia magnetica ///////
            //var offset=new WFG_canvasCoordinate(document.getElementById(el.workflowRelative.idElement),evt);
            //el.setPoint(offset.clientX-(offset.clientX%20),offset.clientY-(offset.clientY%20));
            ///////////////////////////////

            ///// griglia normale /////////
            el.setPoint(cc["x"] + evt.clientX - el.m_x, cc["y"] + evt.clientY - el.m_y);
            ///////////////////////////////
            el.print(evt);
            el.m_x = evt.clientX;
            el.m_y = evt.clientY;
        }
    }

    function changeStatus(val) {
        el.status = val;
        if (val == 0) elRaphael.animate({ fill: "white" }, 500);
        else {
            elRaphael.attr({ fill: "#FFFFCC" });
        }
    }

    el.contains = function(evt) {
        var p = el.getPoint();
        return (evt.clientX > p["x"] && evt.clientX < (p["x"] + el.getWidth()) && evt.clientY > p["y"] && evt.clientY < (p["y"] + el.getHeight()))
    }

    el.addedge = function(line) {
        var eye = true;
        for (var i = 0; i < edgeArray.length && eye; i++) {
            if (line.isEqual(edgeArray[i])) {
                eye = false;
            }
        }
        if (eye) edgeArray.push(line);
    }

    el.removeEdge = function(arc) {
        for (var i = 0; i < edgeArray.length; i++) {
            if (arc.isEqual(edgeArray[i])) {
                edgeArray.splice(i, 1);
                return;
            }
        }
    }

    el.remove = function() {      
        var tmpArray = new Array();

        for (var i = 0; i < edgeArray.length; i++)
            tmpArray.push(edgeArray[i]);

        for (var i = 0; i < tmpArray.length; i++) {
            tmpArray[i].remove();
        }
        for (var i = 0; i < handle.length; i++) {
            handle[i].remove();
        }
        el.arcButton.remove();
        elRaphael.remove();
        text.remove();
        WFS_RemoveNode(el.workflowRelative.getId(), el);

        el.workflowRelative.removeNode(el);
    }

    el.getEdge = function() {
        return edgeArray;
    }

    el.handleSwitchHeight = function() {
        for (var i = 0; i < handle.length; i++) {
            handle[i].switchTypeHeight();
        }
    }

    el.handleSwitchWidth = function() {
        for (var i = 0; i < handle.length; i++) {
            handle[i].switchTypeWidth();
        }
    }

    el.isEqual = function(node) {
        return node.getId() == el.getId();
    }

    el.isInitialNode = function() {
        return elRaphael.attrs.stroke == "green";
    }

    el.isFinalNode = function() {
        return elRaphael.attrs.stroke == "red";
    }

    el.edgePickCorrelation = function(evt) {
        var eye = false;
        for (var i = 0; i < edgeArray.length; i++) {
            if (edgeArray[i].from.isEqual(el)) {
                eye = edgeArray[i].isNear(evt);
                if (eye) return edgeArray[i];
            }
        }
        return null;
    }

    el.hide = function() {
        for (var aggiungi = 0; aggiungi < handle.length; aggiungi++) handle[aggiungi].hide();
    }

    el.show = function() {
        for (var aggiungi = 0; aggiungi < handle.length; aggiungi++) handle[aggiungi].show();
    }

    el.getId = function() {
        return id;
    }

    el.getName = function() {
        return name;
    }

    el.getWidth = function() {
        return elRaphael.attrs.width;
    }

    el.setWidth = function(val) {
        if(val>el.minWidth)
        {
            elRaphael.attr({ width: val });
            return true;
        } else return false;
    }

    el.getHeight = function() {
        return elRaphael.attrs.height;
    }

    el.setHeight = function(val) {
        if(val>el.minHeight)    
        {
            elRaphael.attr({ height: val });
            return true;
        } else return false;
    }

    el.getPoint = function() {
        return point;
    }

    el.setPoint = function(x, y) {
        point["x"] = x;
        point["y"] = y;
    }

    el.isMoving = function() {
        return el.moving;
    }

    el.move = function(evt) {
        mouseMove(evt);
    }

    el.select = function() {
        el.show();
        if (WFG_eventDoNode != null) WFG_eventDoNode(el);
    }

    el.unselect = function(forced) {
        if (forced) el.hide();
        unsavePos();
    }

    // isTo=0 se el nodo from, altrimenti 1
    el.getNearestPoint = function(val, isTo) {
        var point = el.getPoint();
        var tmp = [{ "x": point["x"] + (el.getWidth() / 2), "y": point["y"], "type": 0 },
            { "x": point["x"] + el.getWidth(), "y": point["y"] + (el.getHeight() / 2), "type": 1 },
            { "x": point["x"] + (el.getWidth() / 2), "y": point["y"] + el.getHeight(), "type": 2 },
            { "x": point["x"], "y": point["y"] + (el.getHeight() / 2), "type": 3 },
            { "x": point["x"] + (el.getWidth() / 2), "y": point["y"] - 10, "type": 0 },
            { "x": point["x"] + el.getWidth() + 10, "y": point["y"] + (el.getHeight() / 2), "type": 1 },
            { "x": point["x"] + (el.getWidth() / 2), "y": point["y"] + el.getHeight() + 10, "type": 2 },
            { "x": point["x"] - 10, "y": point["y"] + (el.getHeight() / 2), "type": 3}];
        var better = null;
        var betterIndex = -1;

        for (var i = 0; i < 4; i++) {
            var distance = Math.sqrt(Math.pow(val["x"] - tmp[i]["x"], 2) + Math.pow(val["y"] - tmp[i]["y"], 2));
            if (better == null || better > distance) {
                better = distance;
                betterIndex = i;
            }
        }
        return tmp[betterIndex + isTo * 4];
    }

    el.print = function(evt) {
        for (var aggiungi = 0; aggiungi < handle.length; aggiungi++) handle[aggiungi].follow(evt);
        for (var aggiungi = 0; aggiungi < edgeArray.length; aggiungi++) edgeArray[aggiungi].follow(evt);
        el.arcButton.follow(evt);

        elRaphael.attr({ x: point["x"], y: point["y"] });
        var pointText = new Array();
        pointText["x"] = point["x"] + el.getWidth() / 2;
        pointText["y"] = point["y"] + el.getHeight() / 2;
        text.attr({ x: pointText["x"], y: pointText["y"] });
    }
}

////////////////////////////////////////////////

/////////// initialization /////////////////////

function WFG_initialize() {
    WFG_workflowList = new Array();
}
