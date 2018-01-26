//GLOBAL VARIABLES
var WFS_XmlDocs = new Array();  //List of the WFs's xmlDocs

var WFS_restoreInProgress = false;

WFS_predicateNumber = 0;


/**
********************************************** CREATE WF'S STATUS **********************************************
*/

function WFS_EdgePrecondition(edge, control) {
    var text = "<PRECONDITIONS><placeholder>";

    for (var WFS_i = 0; WFS_i < edge.predicateList.length; WFS_i++) {
        //var tmp=edge.predicateList[WFS_i].toXml(edge.from.getId());
        var tmp = edge.predicateList[WFS_i].toXml(edge.from.getName());
        text = text.replace(/<placeholder>/, tmp);
    }

    text += "</PRECONDITIONS>";

    if (text.indexOf("<placeholder>") == -1 && text.indexOf("<error>") == -1) {
        if (control) {
            //alert(text);
            edge.setPrecondition(text);
            WFS_ModifyEdge(edge.from.workflowRelative.getId(), edge.from.getId(), edge.to.getId(), text);
        }
        return true;
    } else {
        return false;
    }
}

/**
* The namespace resolver xpath utility
*/
WFS_NSresolver = function(prefix) {
    if (prefix == 'xs')
        return 'http://www.w3.org/2001/XMLSchema';
    else
        return null;
}


/**
*  Creates and returns an xml Document, containing the xml Data received
*/
function WFS_CreateXmlDocFromString(xmlData) {
    var xmlDoc = null;

    //Creating the Xml Document    
    if ($.browser.msie) { //for IE        
        xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
        xmlDoc.async = "false";
        xmlDoc.loadXML(xmlData);
    } else if (document.implementation && document.implementation.createDocument) { //for the others        
        parser = new DOMParser();
        xmlDoc = parser.parseFromString(xmlData, "text/xml");
    }
    return xmlDoc;
}

/**
*  Gets an xml Document from cookie
*/
function WFS_GetXmlDocFromCookie(cookieID) {
    var xmlData = $.cookie(cookieID);

    if (xmlData != null && typeof xmlData != 'undefined')
        return WFS_CreateXmlDocFromString(xmlData);
    return null;
}

/**
*  Saves an xml Document as a cookie
*/
function WFS_SaveXmlDocAsCookie(cookieID, xmlDoc) {    
    //Serializing the xmlDoc to string
    var xmlDocString = $.browser.msie ? xmlDoc.xml : (new XMLSerializer()).serializeToString(xmlDoc);

    xmlDocString = xmlDocString.replace(/\n/g, "");

    //it can save max 2600 chars
    ; //$.cookie(cookieID, xmlDocString, { path: '/' });   //Setting the cookie
}

/**
 * Returns a set of xml nodes evaluating the xpath expression received
 */
function WFS_GetElementNodesFromXpathExpr(xmlDoc, xpathExpr, parentNode) {
    //Microsoft Internet Explorer
    if ($.browser.msie) 
        return parentNode.selectNodes(xpathExpr);    

    //Others browser
    return xmlDoc.evaluate(xpathExpr, parentNode, WFS_NSresolver, XPathResult.ANY_TYPE, null);
}

/**
 * Iterates into xPathResult's children
 */
function WFS_IterateNext(xPathResult, counter) {
    //Microsoft Internet Explorer
    if ($.browser.msie)        
        if (counter < xPathResult.length) return xPathResult[counter]; else return null;   

    //Others browser
    return xPathResult.iterateNext();
}

/**
* Removes all children of the parent element received 
*/
function WFS_RemoveAllChildren(parent) {
    if (parent == null || parent.childNodes == null) return;

    var children = new Array();
    for (var i = 0; i < parent.childNodes.length; i++)
        children.push(parent.childNodes.item(i));
    for (var i = 0; i < children.length; i++)
        parent.removeChild(children[i]);
}

/**
*  Creates an xml Document that represents the WF identified by wfID and saves it in a cookie
*/
function WFS_CreateWorkflow(wfID, wfName) {
    //Creating the XmlDocument
    var xmlData = "<WORKFLOW></WORKFLOW>";
    var xmlDoc = WFS_CreateXmlDocFromString(xmlData);

    var wfNameCodified = WFS_EncodeName(wfName);
    
    //Adding the new WF's xmlDoc in the XmlDocsList
    WFS_XmlDocs[wfID] = xmlDoc;

    if (xmlDoc == null) return;

    var root = xmlDoc.documentElement;                        //Getting the root tag
    root.setAttribute('name', wfNameCodified);        //Adding the name's attribute

    root.appendChild(xmlDoc.createElement('EDGES'));          //Appending the edges's tag
    root.appendChild(xmlDoc.createElement('NODES'));          //Appending the nodes's tag

    //Saving the wf's xmlDoc as cookie
    WFS_SaveXmlDocAsCookie(wfID, xmlDoc);  //Setting the cookie

    //Adding the wf's ID in the active-workflow-list's cookie
    var activeWorkflowList = $.cookie("WFE_ActiveWorkflowList");

    if (activeWorkflowList == null) //we are creating the first WF
        activeWorkflowList = wfID;
    else
        activeWorkflowList += '|' + wfID;
    $.cookie("WFE_ActiveWorkflowList", activeWorkflowList, { path: '/' }); //Setting the cookie

    WFC_createWorkflow(wfID + '|' + wfNameCodified);       //Calling server for the WF's creation
}


/**
*  Saves the WF identified by wfID, if ok
*/
function WFS_SaveWorkflow() {
    var wfID = WFG_workflow.getId();
    var xmlDoc = WFS_XmlDocs[wfID];

    if (xmlDoc == null) return;

    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//NODES", xmlDoc);
    var parent = WFS_IterateNext(xPathResult,0);
    var nodes = parent.childNodes;

    if (nodes.length == 0) {
        WFE_printErrorMessage("Form is empty!");
        return;
    }

    //Checking empty nodes
    for (var i = 0; i < nodes.length; i++) {
        var rendering = nodes[i].getElementsByTagName('RENDERING').item(0); //the Rendering element
        var renderSequence = rendering.getElementsByTagName('xs_sequence').item(0);

        if (renderSequence.childNodes.length == 0) {  //if a node is empty it shows the errorMessage and return
            //alert(errorMessage);
            WFE_printErrorMessage("Form has an empty step!");
            return;
        }
    }
    WFE_saveWorkflow(wfID);
}

/**
 * Sets the wf's description
 */
function WFS_SetWorkflowDescription(wfID, wfDescr) {
    var xmlDoc = WFS_XmlDocs[wfID]; //Recovering the wf's xmlDoc
    var root = xmlDoc.documentElement;

    root.setAttribute('description', wfDescr);
}

/**
* Gets the wf's description
*/
function WFS_GetWorkflowDescription(wfID) {
    var xmlDoc = WFS_XmlDocs[wfID]; //Recovering the wf's xmlDoc
    var root = xmlDoc.documentElement;

    return root.getAttribute('description');
}

/**
*  Creates an xml element that represents the Node identified by nodeID and saves it into the WF's xmlDoc
*/
function WFS_AddNode(wfID, nodeID, nodeName, x, y, width, height) {
    var xmlDoc = WFS_XmlDocs[wfID]; //Recovering the wf's xmlDoc

    var nodeNameCodified = WFS_EncodeName(nodeName);

    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//NODES", xmlDoc);
    var parent = WFS_IterateNext(xPathResult, 0);
    //var element = xmlDoc.createElement(nodeID);
    var element = xmlDoc.createElement(nodeNameCodified);   //using real node names    

    //Creating the SCHEMA element
    var schema = xmlDoc.createElement('SCHEMA');
    schema.appendChild(xmlDoc.createElement('EXTENDEDTYPES'));
    var nodeType = xmlDoc.createElement('NODETYPE');
    var complexType = xmlDoc.createElement('xs:complexType');

    //complexType.setAttribute('name', nodeID);
    complexType.setAttribute('name', nodeNameCodified);   //using real node names

    complexType.setAttribute('xmlns:xs', 'http://www.w3.org/2001/XMLSchema');
    complexType.appendChild(xmlDoc.createElement('xs:sequence'));
    nodeType.appendChild(complexType);
    schema.appendChild(nodeType);

    //Creating the RENDERING element
    var rendering = xmlDoc.createElement('RENDERING');
    //rendering.setAttribute('name', nodeID);
    rendering.setAttribute('name', nodeNameCodified);   //using real node names
    rendering.setAttribute('x', WFS_XmlEncode(x));
    rendering.setAttribute('y', WFS_XmlEncode(y));
    rendering.setAttribute('width', WFS_XmlEncode(width));
    rendering.setAttribute('height', WFS_XmlEncode(height));
    var renderComplexType = xmlDoc.createElement('xs_complexType');
    var renderSequence = xmlDoc.createElement('xs_sequence');

    //renderComplexType.setAttribute('name', nodeID);
    renderComplexType.setAttribute('name', nodeNameCodified);   //using real node names

    renderComplexType.appendChild(renderSequence);

    rendering.appendChild(renderComplexType);

    element.appendChild(schema);
    element.appendChild(rendering);

    parent.appendChild(element);
    WFS_SaveXmlDocAsCookie(wfID, xmlDoc);

    //Creating a tmp document that represents the node
    //var nodeDoc = WFS_CreateXmlDocFromString("<" + nodeID + "></" + nodeID + ">");
    var nodeDoc = WFS_CreateXmlDocFromString("<" + nodeNameCodified + "></" + nodeNameCodified + ">");   //using real node names
    nodeDoc.documentElement.appendChild(schema.cloneNode(true));
    nodeDoc.documentElement.appendChild(rendering.cloneNode(true));
    var nodeXml = $.browser.msie ? nodeDoc.xml : (new XMLSerializer()).serializeToString(nodeDoc);

    WFC_addNode(nodeID, nodeNameCodified, nodeXml);    //Calling the server for the node's creation
}

///**
//*  Modifies the renderind attributes of the Node identified by nodeName
//*/
//function WFS_ModifieNodeRenderingAttributes(wfID, nodeID, nodeName, x, y, width, height) {
//    var xmlDoc = WFS_XmlDocs[wfID]; //Recovering the wf's xmlDoc
//    var nodeNameCodified = WFS_EncodeName(nodeName);

//    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//NODES/" + nodeNameCodified, xmlDoc);
//    var nodeEle = WFS_IterateNext(xPathResult,0);

//    var rendering = nodeEle.getElementsByTagName('RENDERING').item(0); //the Rendering element
//    rendering.setAttribute('name', nodeNameCodified);   //using real node names
//    rendering.setAttribute('x', WFS_XmlEncode(x));
//    rendering.setAttribute('y', WFS_XmlEncode(y));
//    rendering.setAttribute('width', WFS_XmlEncode(width));
//    rendering.setAttribute('height', WFS_XmlEncode(height));

//    WFS_SaveXmlDocAsCookie(wfID, xmlDoc);
//}

/**
*  Creates an xml element that represents the edge between the node with ID=fromID and 
*  the node with ID=toID, and saves it into the WF's xmlDoc
*/
function WFS_AddEdge(wfID, fromID, toID) {
    var xmlDoc = WFS_XmlDocs[wfID]; //Recovering the wf's xmlDoc

    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//EDGES", xmlDoc);
    var parent = WFS_IterateNext(xPathResult, 0);
    var element = xmlDoc.createElement('EDGE');
    //element.setAttribute('from', fromID);
    
    var formNameCodified = WFS_EncodeName(WFG_workflow.getNode(fromID).getName());
    var toNameCodified = WFS_EncodeName(WFG_workflow.getNode(toID).getName());
    
    element.setAttribute('from', formNameCodified);
    element.setAttribute('to', toNameCodified);

    element.appendChild(xmlDoc.createElement('PRECONDITIONS'));
    parent.appendChild(element);

    WFS_SaveXmlDocAsCookie(wfID, xmlDoc);

    WFC_addArc(formNameCodified, toNameCodified, "<PRECONDITIONS></PRECONDITIONS>");    //Calling the server for the edge's creation
}

/**
*  Modifies an xml element that represents the edge between the node with ID=fromID and 
*  the node with ID=toID, and saves it into the WF's xmlDoc
*/
function WFS_ModifyEdge(wfID, fromID, toID, xml) {
    var xmlDoc = WFS_XmlDocs[wfID]; //Recovering the wf's xmlDoc

    var formNameCodified = WFS_EncodeName(WFG_workflow.getNode(fromID).getName());
    var toNameCodified = WFS_EncodeName(WFG_workflow.getNode(toID).getName());

    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//EDGES/EDGE[(@from='" + formNameCodified + "') and (@to='" + toNameCodified + "')]", xmlDoc);

    var element = WFS_IterateNext(xPathResult, 0);

    //Removing old preconditions's xml value
    WFS_RemoveAllChildren(element);

    var tmpDoc = WFS_CreateXmlDocFromString(xml);

    element.appendChild(tmpDoc.documentElement.cloneNode(true));

    WFS_SaveXmlDocAsCookie(wfID, xmlDoc);

    WFC_addArc(formNameCodified, toNameCodified, xml);    //Calling the server to modify the edge
}

/**
*  Removes the node identified by the from and to ids
*/
function WFS_RemoveNode(wfID, node) {
    var xmlDoc = WFS_XmlDocs[wfID]; //Recovering the wf's xmlDoc

    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//NODES/"+WFS_EncodeName(node.getName()), xmlDoc);
    var elementToRemove = WFS_IterateNext(xPathResult, 0);

    xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//NODES", xmlDoc);
    var elementParent = WFS_IterateNext(xPathResult, 0);

    elementParent.removeChild(elementToRemove);

    WFS_SaveXmlDocAsCookie(wfID, xmlDoc);

    WFC_removeNode(wfID, node.getId(), WFS_EncodeName(node.getName()));
}

/**
*  Removes the edge identified by the from and to ids
*/
function WFS_RemoveEdge(wfID, fromID, toID) {
    var xmlDoc = WFS_XmlDocs[wfID]; //Recovering the wf's xmlDoc

    var fromNameCodified = WFS_EncodeName(WFG_workflow.getNode(fromID).getName());
    var toNameCodified = WFS_EncodeName(WFG_workflow.getNode(toID).getName());


    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//EDGES/EDGE[(@from='" + fromNameCodified + "') and (@to='" + toNameCodified + "')]", xmlDoc);
    var elementToRemove = WFS_IterateNext(xPathResult, 0);

    xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//EDGES", xmlDoc);
    var elementParent = WFS_IterateNext(xPathResult, 0);

    elementParent.removeChild(elementToRemove);

    WFS_SaveXmlDocAsCookie(wfID, xmlDoc);

    WFC_removeArc(wfID, fromNameCodified, toNameCodified);
}

/**
*  Modifies the node's xml element adding fields
*/
function WFS_AddFields(wfID, node) {
    var xmlDoc = WFS_XmlDocs[wfID]; //Recovering the wf's xmlDoc
    WFS_SaveXmlDocAsCookie("ciao", xmlDoc);
    var nodeID = node.getId();
    var nodeName = node.getName();
    var nodeNameCodified = WFS_EncodeName(nodeName);

    // NodeId & FieldType | FieldIdCounter | Label | Position-x | Position-y | Width | Height | RenderedLabel | Constraints
    var serialized_fields = nodeID + '&' + nodeName;    //This string will contain all fields's properties in the node

    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//NODES/" + nodeNameCodified, xmlDoc);
    var nodeEle = WFS_IterateNext(xPathResult, 0);

    var schema = nodeEle.getElementsByTagName('SCHEMA').item(0);       //the Schema element
    var nodeType = schema.getElementsByTagName('NODETYPE').item(0);
    var sequence = nodeType.getElementsByTagName('xs:sequence').item(0);

    if (sequence == null) sequence = nodeType.childNodes[0].childNodes[1];

    var rendering = nodeEle.getElementsByTagName('RENDERING').item(0); //the Rendering element
    var renderComplexType = rendering.getElementsByTagName('xs_complexType').item(0);
    var renderSequence = xmlDoc.createElement('xs_sequence');

    //Removing old information
    WFS_RemoveAllChildren(sequence);
    WFS_RemoveAllChildren(renderComplexType);

    //Adding the render squence to the complex type
    renderComplexType.appendChild(renderSequence);

    var usingStaticFields = "false";
    for (var i = 0; i < node.field.length; i++) {
        var field = node.field[i];

        //Adding the field in the serialized-fields string
        serialized_fields += '&' + field.serialize();

        switch (field.specialType) {
            case WFE_special_type.NORMAL:   //NORMAL FIELD
            case WFE_special_type.RADIOBUTTON:    //RADIO BUTTON
                WFS_AddFieldElementToDocument(field, xmlDoc, sequence, renderSequence);
                break;

            case WFE_special_type.GRSEQUENCE:   //GROUP SEQUENCE
            case WFE_special_type.GRCHOICE:   //GROUP CHOICE
                WFS_AddGroupElementToDocument(field, xmlDoc, sequence, renderSequence);
                break;

            case WFE_special_type.TEXT:     //FREE TEXT
                WFS_AddTextElementToDocument(field, xmlDoc, renderSequence);
                usingStaticFields = "true";
                break;
            case WFE_special_type.HTMLCODE: //STATIC EMBEDDED HTML CODE
                WFS_AddHtmlCodeElementToDocument(field, xmlDoc, renderSequence);
                usingStaticFields = "true";
                break;
            case WFE_special_type.IMAGE:    //STATIC IMAGE
                WFS_AddImageElementToDocument(field, xmlDoc, renderSequence);
                usingStaticFields = "true";
            default: break;
        }
    }
    WFE_setUsingStaticFieldCookie(usingStaticFields);
    WFC_addField(serialized_fields); //Calling the server to actualize the node's content
}

/**
* Adds a field element in the wf's document
*
* params: 
*        - field: the field object
*        - xmlDoc: the WF's xml Document
*        - parent: the xml parent element (usually it will be the node sequence)
*        - renderParent: the xml render-parent element (usually it will be the node render sequence)
*/
function WFS_AddFieldElementToDocument(field, xmlDoc, parent, renderParent) {
    //modifying the schema
    var element = xmlDoc.createElement('xs:element');   //Creating an element that represents the field

    var fieldNameCodified = WFS_EncodeName(field.name);

    element.setAttribute('name', fieldNameCodified);

    //Adding the field type (only when it's a NORMAL field)
    if (field.specialType == WFE_special_type.NORMAL || field.specialType == WFE_special_type.RADIOBUTTON)
        element.setAttribute('type', WFS_XmlEncode(field.type));

    parent.appendChild(element);    //Adding the field element in the parent element

    //modifying the rendering document
    var fieldEle = xmlDoc.createElement('xs_element');
    fieldEle.setAttribute('name', fieldNameCodified);
    fieldEle.setAttribute('renderedLabel', WFS_XmlEncode(field.renderedLabel));
    fieldEle.setAttribute('baseType', WFS_XmlEncode(field.baseType));
    fieldEle.setAttribute('type', WFS_XmlEncode(field.type));
    fieldEle.setAttribute('description', WFS_XmlEncode(field.description));
    fieldEle.setAttribute('specialType', WFS_XmlEncode(field.specialType));
    fieldEle.setAttribute('constraints', WFS_XmlEncode(field.constraints));

    //for Theme Editor (apply default themes)
    fieldEle.setAttribute('class', WFS_XmlEncode(field.baseType));
    fieldEle.setAttribute('rel', WFS_XmlEncode(field.baseType));

    //Adding the radio button height as fieldEle attribute
    if (field.specialType == WFE_special_type.RADIOBUTTON)
        fieldEle.setAttribute('height', WFS_XmlEncode(field.height));

    //style element is deprecated!!
    /*
    Creating the style
    var fieldStyle = xmlDoc.createElement('style');
    var fieldTop = xmlDoc.createElement('top');
    var fieldLeft = xmlDoc.createElement('left');
    var fieldWidth = xmlDoc.createElement('width');
    var fieldHeight = xmlDoc.createElement('height');

    //Setting coordinates
    var left = 0;
    left += parseInt(field.posx) + 200;
    var top = 0;
    top += parseInt(field.posy) - 15;
    fieldLeft.appendChild(xmlDoc.createTextNode(WFS_XmlEncode(left)));
    fieldTop.appendChild(xmlDoc.createTextNode(WFS_XmlEncode(top)));
    fieldWidth.appendChild(xmlDoc.createTextNode(WFS_XmlEncode(field.width)));
    fieldHeight.appendChild(xmlDoc.createTextNode(WFS_XmlEncode(field.height)));

    fieldStyle.setAttribute('class', WFS_XmlEncode(field.baseType));
    fieldStyle.appendChild(fieldTop);
    fieldStyle.appendChild(fieldLeft);
    fieldStyle.appendChild(fieldWidth);
    fieldStyle.appendChild(fieldHeight);
    fieldEle.appendChild(fieldStyle);
    */
    renderParent.appendChild(fieldEle); //Adding the field render element in the render parent element
}

/**
* Adds a group sequence or choice element in the wf's document
*
* params: 
*        - field: the field object
*        - xmlDoc: the WF's xml Document
*        - parent: the xml parent element (usually it will be the node sequence)
*        - renderParent: the xml render-parent element (usually it will be the node render sequence)
*/
function WFS_AddGroupElementToDocument(field, xmlDoc, parent, renderParent, grouptype) {
    //Adding the field (group sequence) in the parent element
    WFS_AddFieldElementToDocument(field, xmlDoc, parent, renderParent);

    var fieldNameCodified = WFS_EncodeName(field.name);

    //Recovering the group element and the render group element
    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//*[@name='" + fieldNameCodified + "']", parent);

    var groupEle = WFS_IterateNext(xPathResult, 0);
    var groupRenderEle = WFS_IterateNext(xPathResult, 1);

    //Recovering constraints
    if (field.constraints != "") {
        var constraints = field.constraints.split('@');
        var minOccurs = constraints[0].split('#')[1];
        var maxOccurs = constraints[1].split('#')[1];

        if (minOccurs != "") groupEle.setAttribute('minOccurs', minOccurs);
        if (maxOccurs != "") groupEle.setAttribute('maxOccurs', maxOccurs);
    }

    //Adding the group height as groupRenderEle attribute
    groupRenderEle.setAttribute('height', WFS_XmlEncode(field.height));

    //Creating the group-sequence-element's content
    var complexType = xmlDoc.createElement('xs:complexType');
    var sequenceOrChoice = field.specialType == WFE_special_type.GRSEQUENCE ? xmlDoc.createElement('xs:sequence') : xmlDoc.createElement('xs:choice');
    complexType.appendChild(sequenceOrChoice);

    var renderComplexType = xmlDoc.createElement('xs_complexType');
    var renderSequenceOrChoice = field.specialType == WFE_special_type.GRSEQUENCE ? xmlDoc.createElement('xs_sequence') : xmlDoc.createElement('xs_choice');
    renderComplexType.appendChild(renderSequenceOrChoice);

    //Adding the content created in the group-sequence-element
    groupEle.appendChild(complexType);
    groupRenderEle.appendChild(renderComplexType);

    if (field.children == null) return;    //if the group has not children, will return

    //Iterating into group children
    for (var i = 0; i < field.children.length; i++) {
        var f = field.children[i];

        switch (f.specialType) {
            case WFE_special_type.NORMAL:   //NORMAL FIELD
            case WFE_special_type.RADIOBUTTON:   //RADIOBUTTON
                WFS_AddFieldElementToDocument(f, xmlDoc, sequenceOrChoice, renderSequenceOrChoice);
                break;

            case WFE_special_type.GRSEQUENCE:   //GROUP SEQUENCE
            case WFE_special_type.GRCHOICE:     //GROUP CHOICE
                WFS_AddGroupElementToDocument(f, xmlDoc, sequenceOrChoice, renderSequenceOrChoice, f.specialType);
                break;

            case WFE_special_type.TEXT:     //FREE TEXT
                WFS_AddTextElementToDocument(f, xmlDoc, renderSequenceOrChoice);
                break;
            case WFE_special_type.HTMLCODE: //STATIC EMBEDDED HTML CODE
                WFS_AddHtmlCodeElementToDocument(field, xmlDoc, renderSequenceOrChoice);
                break;
            case WFE_special_type.IMAGE:    //STATIC IMAGE
                WFS_AddImageElementToDocument(field, xmlDoc, renderSequenceOrChoice);
                break;
            default: break;
        }
    }
}



/**
* Adds a text element in the wf's document
*
* params: 
*        - field: the field object
*        - xmlDoc: the WF's xml Document
*        - renderParent: the xml render-parent element (usually it will be the node render sequence)
*        - serialized_fields: the serialized fields string
*/
function WFS_AddTextElementToDocument(field, xmlDoc, renderParent) {
    //modifying the rendering document
    var text = xmlDoc.createElement('TEXT');
    text.setAttribute('value', WFS_XmlEncode(field.renderedLabel));


    //changed by Ariel
    /*
    //Creating the style
    var textValue = xmlDoc.createElement('Value');
    
    
    var textStyle = xmlDoc.createElement('style');
    var textTop = xmlDoc.createElement('top');
    var textLeft = xmlDoc.createElement('left');
    

    //Setting coordinates
    var left = 0;
    left += 200 + parseInt(field.posx);
    var top = 0;
    top += parseInt(field.posy) - 15;
    textLeft.appendChild(xmlDoc.createTextNode(WFS_XmlEncode(left)));
    textTop.appendChild(xmlDoc.createTextNode(WFS_XmlEncode(top)));

    textStyle.appendChild(textTop);
    textStyle.appendChild(textLeft);
    textValue.appendChild(xmlDoc.createTextNode(WFS_XmlEncode(field.renderedLabel)));
    text.appendChild(textStyle);
    text.appendChild(textValue);
    */
    renderParent.appendChild(text);
}

/**
* Adds a static image element in the wf's document
*
* params: 
*        - field: the field object
*        - xmlDoc: the WF's xml Document
*        - renderParent: the xml render-parent element (usually it will be the node render sequence)
*        - serialized_fields: the serialized fields string
*/
function WFS_AddImageElementToDocument(field, xmlDoc, renderParent) {
    //modifying the rendering document
    var image = xmlDoc.createElement('IMAGE');    
    image.setAttribute('caption', WFS_XmlEncode(field.renderedLabel));
    image.setAttribute('src', WFS_XmlEncode(field.imgSrc));
    image.setAttribute('renderedLabel', WFS_XmlEncode(field.renderedLabel));
    image.setAttribute('link', WFS_XmlEncode(field.link));

    renderParent.appendChild(image);
}

/**
* Adds a static embedded html code element in the wf's document
*
* params: 
*        - field: the field object
*        - xmlDoc: the WF's xml Document
*        - renderParent: the xml render-parent element (usually it will be the node render sequence)
*        - serialized_fields: the serialized fields string
*/
function WFS_AddHtmlCodeElementToDocument(field, xmlDoc, renderParent) {
    //modifying the rendering document
    var htmlCode = xmlDoc.createElement('HTMLCODE');
    htmlCode.setAttribute('renderedLabel', WFS_XmlEncode(field.renderedLabel));
    htmlCode.setAttribute('src', WFS_XmlEncode(field.htmlSrc));

    renderParent.appendChild(htmlCode);
}

/**
*  Modifies the node's xml element adding field types
*/
function WFS_AddFieldTypes(wfID, serialized_field_types) {
    var xmlDoc = WFS_XmlDocs[wfID]; //Recovering the wf's xmlDoc
    var args = serialized_field_types.split('&');
    var nodeID = args[0];
    var nodeName = WFG_workflow.getNode(nodeID).getName();
    var nodeNameCodified = WFS_EncodeName(nodeName);

    //Recovering the extendedtypes element reference
    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//NODES/" + nodeNameCodified + "/SCHEMA/EXTENDEDTYPES", xmlDoc);
    var extendedTypes = WFS_IterateNext(xPathResult, 0);

    //Removing old types
    WFS_RemoveAllChildren(extendedTypes);

    //Recovering the field types elements
    var tmpDoc = WFS_CreateXmlDocFromString(args[1]);
    var fieldTypes = tmpDoc.documentElement.childNodes;

    for (var i = 0; i < fieldTypes.length; i++) {
        if (fieldTypes[i].nodeType == 1)
            extendedTypes.appendChild(fieldTypes[i].cloneNode(true));
    }
    WFS_SaveXmlDocAsCookie(wfID, xmlDoc);

    //Recovering the schema and rendering elements of the node
    xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//NODES/" + nodeNameCodified + "/SCHEMA", xmlDoc);
    var schema = WFS_IterateNext(xPathResult, 0);

    xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//NODES/" + nodeNameCodified + "/RENDERING", xmlDoc);
    var rendering = WFS_IterateNext(xPathResult, 0);

    //Creating a tmp document that represents the node
    var nodeDoc = WFS_CreateXmlDocFromString("<" + nodeNameCodified + "></" + nodeNameCodified + ">");
    nodeDoc.documentElement.appendChild(schema.cloneNode(true));
    nodeDoc.documentElement.appendChild(rendering.cloneNode(true));
    var nodeXml = $.browser.msie ? nodeDoc.xml : (new XMLSerializer()).serializeToString(nodeDoc);

    WFC_syncNode(nodeID, nodeNameCodified, nodeXml);  //Calling the server to update the node status
}
/****************************************************************************************************************/




/**
********************************************** RESTORE WF'S STATUS **********************************************
*/

/**
*  Restores the active WFs using cookie's informations
*/
function WFS_RestoreAllWorkflows() {
    
    //Recovering the wfs's IDs from the active Workflow List
    var activeWorkflowList = $.cookie("WFE_ActiveWorkflowList");

    if (activeWorkflowList == null) { return; }  //No one wf exists

    var ids = activeWorkflowList.split('|');

    spinnerStart(document.getElementById("tabs"));

    for (var i = 0; i < ids.length; i++) {
        WFC_LoadXmlDocFromSession(ids[i]);        
        WFS_RestoreWorkflowEditorButtons(ids[i]);
    }
}

/**
* Restores all workflow editor buttons
*/
function WFS_RestoreWorkflowEditorButtons(wfID) {
    WFB_cleanMenu();

    //adding draggable nodes
    WFB_addDraggableNodeButton();

    //Adding Save Buttons
    WFB_addSaveButton();   

    var wfState = $.cookie(wfID);

    if (wfState == null) wfState = "";

    var theme_edited = WFE_checkIfThemeWasEdited();

    switch (wfState) {
        case 'saved':
            WFB_addPublishButton();
            WFB_addCheckImageToButton("Save");         
            break;
              
        case 'public':
            WFB_addPublishButton();
            WFB_addThemeEditorButton();
            WFB_addCheckImageToButton("Save");
            WFB_addCheckImageToButton("Publish");
            if(theme_edited) WFB_addCheckImageToButton("Theme Editor");
            break;

        case 'private':
            WFB_addPublishButton();
            WFB_addThemeEditorButton();
            WFB_addSendToContactsButton();
            WFB_addCheckImageToButton("Save");
            WFB_addCheckImageToButton("Publish");
            if(theme_edited) WFB_addCheckImageToButton("Theme Editor");
            break;
    }

    WFB_addTutorialWorkflowButton();

    WFB_prepareButtonsEffect();

    WFB_addingButtonsEffect();
}

/**
* Restores the WF identified by wfID, using its xmlDoc
*/
function WFS_RestoreWorkflow(wfID, xmlData) {
    //Disabling client-server communication
    WFC_ServerCommunicationEnabled = false;    
    //Recovering the xmlDoc from the cookie
    var xmlDoc = WFS_CreateXmlDocFromString(xmlData); //WFS_GetXmlDocFromCookie(wfID);

    //Adding the new WF's xmlDoc in the XmlDocsList
    WFS_XmlDocs[wfID] = xmlDoc;

    var root = xmlDoc.documentElement;                           //The root element of the xmlDoc
    var wfName = WFS_DecodeName(root.getAttribute('name'));      //The wf's Name
    var idElement = wfID + "_Canvas";                            //The canvas-div's id

    //Creating the WF's tab
    WFG_AddWorkflowCanvasTab(wfID, wfName, idElement);

    //Creating the WF in the graph editor
    var workflow = new WFG_workflow(wfID, idElement, wfName)
    WFG_workflowList.push(workflow);
    WFG_workflow = workflow;    //Setting the current wf   

    //Restoring the WF's nodes
    WFS_RestoreAllNodes(workflow, wfID);

    //Restoring the WF's edges
    WFS_RestoreAllEdges(workflow, wfID);

    //Setting node button draggability
    WFB_setNodeButtonDraggability();

    //Selecting the WF's tab
    $("#tabs").tabs('select', "#tab-" + wfID);

    //Enabling client-server communication
    WFC_ServerCommunicationEnabled = true;
}

/**
* Restores the WF's nodes
*/
function WFS_RestoreAllNodes(workflow, wfID) {    
    var xmlDoc = WFS_XmlDocs[wfID]; //The WF's xmlDoc

    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//NODES", xmlDoc);
    var nodesParent = WFS_IterateNext(xPathResult, 0);    //Recovering the nodes's parent element

    var nodes = nodesParent.childNodes;     //Recovering the nodes list

    for (var i = 0; i < nodes.length; i++)
        WFS_RestoreNode(nodes[i], workflow, xmlDoc);      //Restoring the node    
}


/**
* Restores the node identified by the node element
*/
function WFS_RestoreNode(nodeEle, workflow, xmlDoc) {
    var rendering = nodeEle.getElementsByTagName('RENDERING').item(0);

    var nodeName = WFS_DecodeName(rendering.getAttribute('name'));
    var idNode = workflow.getNewNodeID();
    var x = parseInt(WFS_XmlDecode(rendering.getAttribute('x')));
    var y = parseInt(WFS_XmlDecode(rendering.getAttribute('y')));
    var width = parseInt(WFS_XmlDecode(rendering.getAttribute('width')));
    var height = parseInt(WFS_XmlDecode(rendering.getAttribute('height')));

    //Creating the node
    var node = new WFG_graphNode(nodeName, idNode, workflow, x, y);
    node.setWidth(width);
    node.setHeight(height);

    //Restoring the node in the WF
    workflow.addExistingNode(node);

    //Recovering all node's fields
    WFS_RestoreAllNodeFields(rendering.getElementsByTagName('xs_sequence').item(0), node, xmlDoc);
}

/**
* Restores the WF's edges
*/
function WFS_RestoreAllEdges(workflow, wfID) {
    var xmlDoc = WFS_XmlDocs[wfID]; //The WF's xmlDoc

    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//EDGES", xmlDoc);
    var edgesParent = WFS_IterateNext(xPathResult, 0);    //Recovering the edges's parent element

    var edges = edgesParent.childNodes;     //Recovering the edges list

    for (var i = 0; i < edges.length; i++)
        WFS_RestoreEdge(edges[i], workflow);      //Restoring the edge
}

/**
* Restores the edge identified by the edge element
*/
function WFS_RestoreEdge(edgeEle, workflow) {
    var from = WFS_DecodeName(edgeEle.getAttribute('from'));
    var to = WFS_DecodeName(edgeEle.getAttribute('to'));

    if (from == null || to == null) return;

    //Recovering the edge's nodes references
    var fromNode = workflow.getNodeByName(from);
    var toNode = workflow.getNodeByName(to);

    //Creating the edge
    var edge = new WFG_edge(fromNode, toNode);
    edge.follow();
    fromNode.addedge(edge);
    toNode.addedge(edge);

    //Creating tmp doc containing the preconditions xml
    var precDoc = WFS_CreateXmlDocFromString('<PRECONDITIONS></PRECONDITIONS>');
    var precChildren = edgeEle.getElementsByTagName('PRECONDITIONS').item(0).childNodes;

    //Adding all preconditions to the precDoc
    for (var i = 0; i < precChildren.length; i++)
        precDoc.documentElement.appendChild(precChildren[i].cloneNode(true));

    //Creating the preconditions string
    var precondition = $.browser.msie ? precDoc.xml : (new XMLSerializer()).serializeToString(precDoc);
    edge.setPrecondition(precondition);     //Setting the edge's preconditions
}


/**
* Restores all node's fields contained in the nodeSequence
*/
function WFS_RestoreAllNodeFields(nodeSequence, node, xmlDoc) {
    if (typeof nodeSequence == 'undefined' || nodeSequence == null) return; //The node is empty!

    //Creating the node's tab
    WFE_openFormTab(node.getName(), node.getId());

    var node_tab = "#tab-" + node.getId();

    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "*", nodeSequence);
    var i = 0;
    var child = WFS_IterateNext(xPathResult, i);

    var usingStaticFields = "false";
    //Recovering all node's children
    while (child) {
        switch (child.tagName) {
            case 'xs_element':  //FIELD
                WFS_RestoreField(child, node, $(node_tab), xmlDoc);
                break;

            case 'TEXT':    //TEXT
                WFS_RestoreTextField(child, node, $(node_tab));
                usingStaticFields = "true";
                break;

            case 'HTMLCODE':   //IMAGE
                WFS_RestoreHtmlCodeField(child, node, $(node_tab));
                usingStaticFields = "true";
                break;

            case 'IMAGE':   //IMAGE
                WFS_RestoreImageField(child, node, $(node_tab));
                usingStaticFields = "true";
                break;
        }
        child = WFS_IterateNext(xPathResult, ++i);
    }
    WFE_setUsingStaticFieldCookie(usingStaticFields);
}

/**
* Restores the text field identified by the text element
*/
function WFS_RestoreTextField(textEle, node, jQueryParentObj) {
    //Recovering all attributes
    //    var valueEle = textEle.getElementsByTagName('Value').item(0);
    //    var value = WFS_XmlDecode(valueEle.firstChild.data);
    //    
    //    var topEle = textEle.getElementsByTagName('top').item(0);
    //    var leftEle = textEle.getElementsByTagName('left').item(0);
    //    //    var widthEle = textEle.getElementsByTagName('width').item(0);
    //    //    var heightEle = textEle.getElementsByTagName('height').item(0);    
    //    var top = parseInt(WFS_XmlDecode(topEle.firstChild.data)) + 15;
    //    var left = parseInt(WFS_XmlDecode(leftEle.firstChild.data)) - 200;
    //    var width = parseInt(WFS_XmlDecode(widthEle.firstChild.data));
    //    var height = parseInt(WFS_XmlDecode(heightEle.firstChild.data));

    var value = WFS_XmlDecode(textEle.getAttribute('value'));

    //Restoring the text in the editor
    var current_id = WFF_rendered_box_id++;
    var field = new WFE_field("WFF_added_element_" + current_id, "StaticImage", "StaticImage", node);

    //    field.posx = left;
    //    field.posy = top;
    field.setAsStaticLabel(value);

    //The node tab
    var node_tab = "#tab-" + node.getId();
    WFF_findLastObj(jQueryParentObj, current_id);

    //Adding the fieldTEXT in the parent (it could be the node-tab or a group-sequence for example)
    WFF_addObjectToParent(jQueryParentObj, node_tab, field, node);

    WFF_drawStaticLabel(field, current_id, jQueryParentObj, node_tab, node);
}

/**
* Restores the image field identified by the image element
*/
function WFS_RestoreImageField(imageEle, node, jQueryParentObj) {
    var img_src = WFS_XmlDecode(imageEle.getAttribute('src'));
    var description = WFS_XmlDecode(imageEle.getAttribute('caption'));
    var renderedLabel = WFS_XmlDecode(imageEle.getAttribute('renderedLabel'));
    var link = WFS_XmlDecode(imageEle.getAttribute('link'));

    //Restoring the image in the editor
    var current_id = WFF_rendered_box_id++;
    var field = new WFE_field("WFF_added_element_" + current_id, "", "", node);

    field.setAsStaticImage(renderedLabel, img_src);
    field.name = renderedLabel;
    field.description = renderedLabel;
    field.link = link;

    //The node tab
    var node_tab = "#tab-" + node.getId();
    WFF_findLastObj(jQueryParentObj, current_id);

    //Adding the fieldTEXT in the parent (it could be the node-tab or a group-sequence for example)
    WFF_addObjectToParent(jQueryParentObj, node_tab, field, node);

    WFF_drawStaticImage(field, current_id, jQueryParentObj, node_tab, node);
}

/**
* Restores the embedded html code field identified by the HTML element
*/
function WFS_RestoreHtmlCodeField(htmlCodeEle, node, jQueryParentObj) {
    var html_src = WFS_XmlDecode(htmlCodeEle.getAttribute('src'));
    var renderedLabel = WFS_XmlDecode(htmlCodeEle.getAttribute('renderedLabel'));

    //Restoring the html code field in the editor
    var current_id = WFF_rendered_box_id++;
    var field = new WFE_field("WFF_added_element_" + current_id, "StaticHtmlCode", "StaticHtmlCode", node);

    field.setAsStaticHtmlCode(renderedLabel, html_src);

    //The node tab
    var node_tab = "#tab-" + node.getId();
    WFF_findLastObj(jQueryParentObj, current_id);

    //Adding the fieldTEXT in the parent (it could be the node-tab or a group-sequence for example)
    WFF_addObjectToParent(jQueryParentObj, node_tab, field, node);

    WFF_drawStaticHtmlCode(field, current_id, jQueryParentObj, node_tab, node);
}

/**
* Restores the field identified by the field element
*/
function WFS_RestoreField(fieldEle, node, jQueryParentObj, xmlDoc) {
    var baseType = WFS_XmlDecode(fieldEle.getAttribute('baseType'));   //The field's base-type (ex: StringBox)
    var type = WFS_XmlDecode(fieldEle.getAttribute('type'));           //The field's type (ex: StringBox1)

    //Creating the WFE_field object
    var current_id = WFF_rendered_box_id++;
    var field = new WFE_field("WFF_added_element_" + current_id, baseType, type, node);

    //Recovering all attributes
    var name = WFS_DecodeName(fieldEle.getAttribute('name'));
    var renderedLabel = WFS_XmlDecode(fieldEle.getAttribute('renderedLabel'));
    var constraints = WFS_XmlDecode(fieldEle.getAttribute('constraints'));
    var description = WFS_XmlDecode(fieldEle.getAttribute('description'));
    var specialType = WFS_XmlDecode(fieldEle.getAttribute('specialType'));
    var useRenderedLabel = !(renderedLabel == name);
    //    var topEle = fieldEle.getElementsByTagName('top').item(0);
    //    var leftEle = fieldEle.getElementsByTagName('left').item(0);
    //    var widthEle = fieldEle.getElementsByTagName('width').item(0);
    //    var heightEle = fieldEle.getElementsByTagName('height').item(0);
    //    var top = parseInt(WFS_XmlDecode(topEle.firstChild.data)) + 15;
    //    var left = parseInt(WFS_XmlDecode(leftEle.firstChild.data)) - 200;
    //    var width = parseInt(WFS_XmlDecode(widthEle.firstChild.data));
    //    var height = parseInt(WFS_XmlDecode(heightEle.firstChild.data));

    //Setting all attributes
    field.name = name;
    field.renderedLabel = renderedLabel;
    field.description = description;
    field.useRenderedLabel = useRenderedLabel;
    field.type = type;
    field.baseType = baseType;
    //    field.posx = left;
    //    field.posy = top;
    //    field.width = width;
    //    field.height = height;
    field.constraints = constraints;

    //Restoring the field in the editor
    var node_tab = "#tab-" + node.getId();
    WFF_findLastObj(jQueryParentObj, current_id);

    //Adding the field in the parent (it could be the node-tab or a group-sequence for example)
    WFF_addObjectToParent(jQueryParentObj, node_tab, field, node);

    //Drawing the field 
    switch (specialType) {
        case WFE_special_type.NORMAL:   //NORMAL FIELD        
            WFF_drawField(field, current_id, jQueryParentObj, node_tab, node);
            WFF_loadType(field.baseType);
            break;

        case WFE_special_type.RADIOBUTTON:  //RADIO BUTTON LIST
            field.setAsRadioButton();

            WFS_RestoreRadioButton(fieldEle, field, current_id, jQueryParentObj, node_tab, node, xmlDoc);
            break;

        case WFE_special_type.GRSEQUENCE:   //GROUP SEQUENCE
            field.setAsSequenceGroup();

            WFS_RestoreGroup(fieldEle, field, current_id, jQueryParentObj, node_tab, node, xmlDoc);
            break;
        case WFE_special_type.GRCHOICE:     //GROUP CHOICE
            field.setAsChoiceGroup();

            WFS_RestoreGroup(fieldEle, field, current_id, jQueryParentObj, node_tab, node, xmlDoc);
            break;

        default: break;
    }
}

/**
* Restores the radioButton state
*/
function WFS_RestoreRadioButton(fieldEle, field, current_id, jQueryParentObj, node_tab, node, xmlDoc) {
    //Restoring all radio-button's children
    //Recovering the radioButtonComplexType element
    var type = fieldEle.getAttribute('type');
    var nodeName = node.getName();

    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "//" + WFS_EncodeName(nodeName) + "/SCHEMA/EXTENDEDTYPES/*[@name='" + type + "']//*[@value]", xmlDoc);

    var i = 0;
    var enumerationEle = WFS_IterateNext(xPathResult, i);

    while (enumerationEle) {
        //Adding the value string attribut as child of the radioButtonList
        field.children.push(WFS_XmlDecode(enumerationEle.getAttribute('value')));
        enumerationEle = WFS_IterateNext(xPathResult, ++i);
    }

    //Drawing the radioButtonList
    WFF_drawRadioButton(field, current_id, jQueryParentObj, node_tab, node);

    //Restoring the radio button height
    field.height = parseInt(WFS_XmlDecode(fieldEle.getAttribute('height')));
    $('#' + field.id).find(".WFF_radio_button_list").height(field.height);
}

/**
* Restores the group sequence or choice state
*/
function WFS_RestoreGroup(fieldEle, field, current_id, jQueryParentObj, node_tab, node, xmlDoc) {
    var sequenceOrChoice = "";

    if (field.specialType == WFE_special_type.GRSEQUENCE)
        sequenceOrChoice = 'xs_sequence';
    else
        sequenceOrChoice = 'xs_choice';

    //Drawing the group
    WFF_drawGroupBox(field, current_id, jQueryParentObj, node_tab, node);

    //Restoring the group height
    field.height = parseInt(WFS_XmlDecode(fieldEle.getAttribute('height')));
    $('#' + field.id).find(".WFF_static_group_box").height(field.height);

    //Restoring all group's children
    var xPathResult = WFS_GetElementNodesFromXpathExpr(xmlDoc, "xs_complexType/" + sequenceOrChoice + "/*", fieldEle);
    var i = 0;
    var child = WFS_IterateNext(xPathResult, i);
    var jqueryObj = $('#' + field.id).find('.WFF_static_group_box');

    while (child) {
        switch (child.tagName) {
            case 'xs_element':  //FIELD
                WFS_RestoreField(child, node, jqueryObj, xmlDoc);
                break;

            case 'TEXT':    //TEXT
                WFS_RestoreTextField(child, node, jqueryObj);
                break;

            case 'IMAGE':   //IMAGE
                WFS_RestoreImageField(child, node, jqueryObj);
                break;
        }
        child = WFS_IterateNext(xPathResult, ++i);
    }
}

/***************************************************************************************************************/