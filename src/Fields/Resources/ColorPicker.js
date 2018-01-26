// Script adapted for fLOAding by Alfredo Cardigliano 
// (this version of the script is not the original)

// Copyright 2008 Sebo Zoltan <iamzoli@yahoo.com>
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject
// to the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

var COLORPICKER_HEIGHT_OF_OBJ=25;//color picker top or bottom
var COLORPICKER_WIDTH_OF_OBJ=150; //color picker left or right

// Some utility functions
var COLORPICKER_is_div_init=false;
function COLORPICKER_hexToRgb(hex_string, default_)
{
    if (default_ == undefined)
    {
        default_ = null;
    }

    if (hex_string.substr(0, 1) == '#')
    {
        hex_string = hex_string.substr(1);
    }
    
    var r;
    var g;
    var b;
    if (hex_string.length == 3)
    {
        r = hex_string.substr(0, 1);
        r += r;
        g = hex_string.substr(1, 1);
        g += g;
        b = hex_string.substr(2, 1);
        b += b;
    }
    else if (hex_string.length == 6)
    {
        r = hex_string.substr(0, 2);
        g = hex_string.substr(2, 2);
        b = hex_string.substr(4, 2);
    }
    else
    {
        return default_;
    }
    
    r = parseInt(r, 16);
    g = parseInt(g, 16);
    b = parseInt(b, 16);
    if (isNaN(r) || isNaN(g) || isNaN(b))
    {
        return default_;
    }
    else
    {
        return {r: r / 255, g: g / 255, b: b / 255};
    }
}

function COLORPICKER_rgbToHex(r, g, b, includeHash)
{
    r = Math.round(r * 255);
    g = Math.round(g * 255);
    b = Math.round(b * 255);
    if (includeHash == undefined)
    {
        includeHash = true;
    }
    
    r = r.toString(16);
    if (r.length == 1)
    {
        r = '0' + r;
    }
    g = g.toString(16);
    if (g.length == 1)
    {
        g = '0' + g;
    }
    b = b.toString(16);
    if (b.length == 1)
    {
        b = '0' + b;
    }
    return ((includeHash ? '#' : '') + r + g + b).toUpperCase();
}

var COLORPICKER_arVersion = navigator.appVersion.split("MSIE");
var COLORPICKER_version = parseFloat(COLORPICKER_arVersion[1]);

function COLORPICKER_fixPNG(myImage)
{
    if ((COLORPICKER_version >= 5.5) && (COLORPICKER_version < 7) && (document.body.filters)) 
    {
        var node = document.createElement('span');
        node.id = myImage.id;
        node.className = myImage.className;
        node.title = myImage.title;
        node.style.cssText = myImage.style.cssText;
        node.style.setAttribute('filter', "progid:DXImageTransform.Microsoft.AlphaImageLoader"
                                        + "(src=\'" + myImage.src + "\', sizingMethod='scale')");
        node.style.fontSize = '0';
        node.style.width = myImage.width.toString() + 'px';
        node.style.height = myImage.height.toString() + 'px';
        node.style.display = 'inline-block';
        return node;
    }
    else
    {
        return myImage.cloneNode(false);
    }
}

function COLORPICKER_trackDrag(node, handler)
{
    function fixCoords(x, y)
    {
        var nodePageCoords = COLORPICKER_pageCoords(node);
        x = (x - nodePageCoords.x) + document.documentElement.scrollLeft;
        y = (y - nodePageCoords.y) + document.documentElement.scrollTop;
        if (x < 0) x = 0;
        if (y < 0) y = 0;
        if (x > node.offsetWidth - 1) x = node.offsetWidth - 1;
        if (y > node.offsetHeight - 1) y = node.offsetHeight - 1;
        return {x: x, y: y};
    }
    function mouseDown(ev)
    {
        var coords = fixCoords(ev.clientX, ev.clientY);
        var lastX = coords.x;
        var lastY = coords.y;
        handler(coords.x, coords.y);

        function moveHandler(ev)
        {
            var coords = fixCoords(ev.clientX, ev.clientY);
            if (coords.x != lastX || coords.y != lastY)
            {
                lastX = coords.x;
                lastY = coords.y;
                handler(coords.x, coords.y);
            }
        }
        function upHandler(ev)
        {
            COLORPICKER_myRemoveEventListener(document, 'mouseup', upHandler);
            COLORPICKER_myRemoveEventListener(document, 'mousemove', moveHandler);
            COLORPICKER_myAddEventListener(node, 'mousedown', mouseDown);
        }
        COLORPICKER_myAddEventListener(document, 'mouseup', upHandler);
        COLORPICKER_myAddEventListener(document, 'mousemove', moveHandler);
        COLORPICKER_myRemoveEventListener(node, 'mousedown', mouseDown);
        if (ev.preventDefault) ev.preventDefault();
    }
    COLORPICKER_myAddEventListener(node, 'mousedown', mouseDown);
    node.onmousedown = function(e) { return false; };
    node.onselectstart = function(e) { return false; };
    node.ondragstart = function(e) { return false; };
}

var COLORPICKER_eventListeners = [];

function COLORPICKER_findEventListener(node, event, handler)
{
    var i;
    for (i in COLORPICKER_eventListeners)
    {
        if (COLORPICKER_eventListeners[i].node == node && COLORPICKER_eventListeners[i].event == event
         && COLORPICKER_eventListeners[i].handler == handler)
        {
            return i;
        }
    }
    return null;
}

function COLORPICKER_myAddEventListener(node, event, handler)
{
    if (COLORPICKER_findEventListener(node, event, handler) != null)
    {
        return;
    }

    if (!node.addEventListener)
    {
        node.attachEvent('on' + event, handler);
    }
    else
    {
        node.addEventListener(event, handler, false);
    }

    COLORPICKER_eventListeners.push({node: node, event: event, handler: handler});
}

function COLORPICKER_removeEventListenerIndex(index)
{
    var eventListener = COLORPICKER_eventListeners[index];
    delete COLORPICKER_eventListeners[index];
    
    if (!eventListener.node.removeEventListener)
    {
        eventListener.node.detachEvent('on' + eventListener.event,
                                       eventListener.handler);
    }
    else
    {
        eventListener.node.removeEventListener(eventListener.event,
                                               eventListener.handler, false);
    }
}

function COLORPICKER_myRemoveEventListener(node, event, handler)
{
    COLORPICKER_removeEventListenerIndex(COLORPICKER_findEventListener(node, event, handler));
}

function COLORPICKER_cleanupEventListeners()
{
    var i;
    for (i = COLORPICKER_eventListeners.length; i > 0; i--)
    {
        if (COLORPICKER_eventListeners[i] != undefined)
        {
            COLORPICKER_removeEventListenerIndex(i);
        }
    }
}
COLORPICKER_myAddEventListener(window, 'unload', COLORPICKER_cleanupEventListeners);

// This copyright statement applies to the following two functions,
// which are taken from MochiKit.
//
// Copyright 2005 Bob Ippolito <bob@redivi.com>
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject
// to the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

function COLORPICKER_hsvToRgb(hue, saturation, value)
{
    var red;
    var green;
    var blue;
    if (value == 0.0)
    {
        red = 0;
        green = 0;
        blue = 0;
    }
    else
    {
        var i = Math.floor(hue * 6);
        var f = (hue * 6) - i;
        var p = value * (1 - saturation);
        var q = value * (1 - (saturation * f));
        var t = value * (1 - (saturation * (1 - f)));
        switch (i)
        {
            case 1: red = q; green = value; blue = p; break;
            case 2: red = p; green = value; blue = t; break;
            case 3: red = p; green = q; blue = value; break;
            case 4: red = t; green = p; blue = value; break;
            case 5: red = value; green = p; blue = q; break;
            case 6: // fall through
            case 0: red = value; green = t; blue = p; break;
        }
    }
    return {r: red, g: green, b: blue};
}

function COLORPICKER_rgbToHsv(red, green, blue)
{
    var max = Math.max(Math.max(red, green), blue);
    var min = Math.min(Math.min(red, green), blue);
    var hue;
    var saturation;
    var value = max;
    if (min == max)
    {
        hue = 0;
        saturation = 0;
    }
    else
    {
        var delta = (max - min);
        saturation = delta / max;
        if (red == max)
        {
            hue = (green - blue) / delta;
        }
        else if (green == max)
        {
            hue = 2 + ((blue - red) / delta);
        }
        else
        {
            hue = 4 + ((red - green) / delta);
        }
        hue /= 6;
        if (hue < 0)
        {
            hue += 1;
        }
        if (hue > 1)
        {
            hue -= 1;
        }
    }
    return {
        h: hue,
        s: saturation,
        v: value
    };
}

function COLORPICKER_pageCoords(node)
{
    var x = node.offsetLeft;
    var y = node.offsetTop;
    var parent = node.offsetParent;
    while (parent != null)
    {
        x += parent.offsetLeft;
        y += parent.offsetTop;
        parent = parent.offsetParent;
    }
    return {x: x, y: y};
}

// The real code begins here.
var COLORPICKER_huePositionImg = document.createElement('img');
COLORPICKER_huePositionImg.galleryImg = false;
COLORPICKER_huePositionImg.width = 35;
COLORPICKER_huePositionImg.height = 11;
COLORPICKER_huePositionImg.src = COLORPICKER_HUE_SLIDER_ARROWS_LOCATION;
COLORPICKER_huePositionImg.style.position = 'absolute';

var COLORPICKER_hueSelectorImg = document.createElement('img');
COLORPICKER_hueSelectorImg.galleryImg = false;
COLORPICKER_hueSelectorImg.width = 35;
COLORPICKER_hueSelectorImg.height = 200;
COLORPICKER_hueSelectorImg.src = COLORPICKER_HUE_SLIDER_LOCATION;
COLORPICKER_hueSelectorImg.style.display = 'block';

var COLORPICKER_satValImg = document.createElement('img');
COLORPICKER_satValImg.galleryImg = false;
COLORPICKER_satValImg.width = 200;
COLORPICKER_satValImg.height = 200;
COLORPICKER_satValImg.src = COLORPICKER_SAT_VAL_SQUARE_LOCATION;
COLORPICKER_satValImg.style.display = 'block';

var COLORPICKER_crossHairsImg = document.createElement('img');
COLORPICKER_crossHairsImg.galleryImg = false;
COLORPICKER_crossHairsImg.width = 21;
COLORPICKER_crossHairsImg.height = 21;
COLORPICKER_crossHairsImg.src = COLORPICKER_CROSSHAIRS_LOCATION;
COLORPICKER_crossHairsImg.style.position = 'absolute';

function COLORPICKER_makeColorSelector(inputBox)
{
    var rgb, hsv
    
    function colorChanged()
    {
		COLORPICKER_is_div_init=false;
        var hex = COLORPICKER_rgbToHex(rgb.r, rgb.g, rgb.b);
		
        var hueRgb = COLORPICKER_hsvToRgb(hsv.h, 1, 1);
        var hueHex = COLORPICKER_rgbToHex(hueRgb.r, hueRgb.g, hueRgb.b);
		//alert(hex);
		inputBox.style.background = hex;
		inputBox.value =hex;
		// popox ideea
		if(((rgb.r*100+rgb.g*100+rgb.b*100)/3)<65) //change text color to white if the background color is to dark
		inputBox.style.color="#fff";
		else inputBox.style.color="#000";
			
		
        satValDiv.style.background = hueHex;
        crossHairs.style.left = ((hsv.v*199)-10).toString() + 'px';
       crossHairs.style.top = (((1-hsv.s)*199)-10).toString() + 'px';
       huePos.style.top = ((hsv.h*199)-5).toString() + 'px';
	   COLORPICKER_is_div_init=true;
    }
    function rgbChanged()
    {
        hsv = COLORPICKER_rgbToHsv(rgb.r, rgb.g, rgb.b);
        colorChanged();
    }
    function hsvChanged()
    {
        rgb = COLORPICKER_hsvToRgb(hsv.h, hsv.s, hsv.v);
        colorChanged();
    }
    
    var colorSelectorDiv = document.createElement('div');
    colorSelectorDiv.style.paddingLeft = '5px';
	colorSelectorDiv.style.paddingRight = '5px';
	colorSelectorDiv.style.paddingBottom = '5px';
    colorSelectorDiv.style.position = 'relative';
	colorSelectorDiv.style.diplay="inline";
	colorSelectorDiv.style.height = '204px';
    colorSelectorDiv.style.width = '210px';

    var satValDiv = document.createElement('div');
    satValDiv.style.position = 'relative';
	satValDiv.style.diplay="inline";
	satValDiv.style.top = '5px';
    satValDiv.style.width = '200px';
	satValDiv.style.height = '200px';
    var newSatValImg = COLORPICKER_fixPNG(COLORPICKER_satValImg);
    satValDiv.appendChild(newSatValImg);
    var crossHairs = COLORPICKER_crossHairsImg.cloneNode(false);
    satValDiv.appendChild(crossHairs);
    function satValDragged(x, y)
    {
        hsv.s = 1-(y/199);
        hsv.v = (x/199);
        hsvChanged();
    }
    COLORPICKER_trackDrag(satValDiv, satValDragged)
	  
    colorSelectorDiv.appendChild(satValDiv);

    var hueDiv = document.createElement('div');
    hueDiv.style.position = 'absolute';
	hueDiv.style.diplay="inline";
    hueDiv.style.left = '210px';
    hueDiv.style.top = '5px';
    hueDiv.style.width = '35px';
    hueDiv.style.height = '200px';
    var huePos = COLORPICKER_fixPNG(COLORPICKER_huePositionImg);
    hueDiv.appendChild(COLORPICKER_hueSelectorImg.cloneNode(false));
    hueDiv.appendChild(huePos);
    function hueDragged(x, y)
    {
		COLORPICKER_is_div_init=false;
        hsv.h = y/199;
        hsvChanged();
    }
    COLORPICKER_trackDrag(hueDiv, hueDragged);
    colorSelectorDiv.appendChild(hueDiv);

    function inputBoxChanged()
    {
        rgb = COLORPICKER_hexToRgb(inputBox.value, {r: 0, g: 0, b: 0});
        rgbChanged();
    }
    COLORPICKER_myAddEventListener(inputBox, 'change', inputBoxChanged);
  
    inputBoxChanged();
    
    return colorSelectorDiv;
}


function COLORPICKER_colorPickerGetTopPos(inputObj)
{
	var returnValue = inputObj.offsetTop;
	while((inputObj = inputObj.offsetParent) != null){
		returnValue += inputObj.offsetTop;
	}
	return returnValue-COLORPICKER_HEIGHT_OF_OBJ;
}
	
function COLORPICKER_colorPickerGetLeftPos(inputObj)
{
	var returnValue = inputObj.offsetLeft;
	while((inputObj = inputObj.offsetParent) != null)returnValue += inputObj.offsetLeft;
	return returnValue+COLORPICKER_WIDTH_OF_OBJ;
}
	
function COLORPICKER_start(inputObj)
{
	COLORPICKER_hide();
	var color_picker_div = document.createElement('DIV');
	color_picker_div.style.left = COLORPICKER_colorPickerGetLeftPos(inputObj) + 'px';
	color_picker_div.style.width='250px';
	color_picker_div.style.heigth='190px';
	   
	color_picker_div.style.position='absolute';
	color_picker_div.style.paddingBottom='1px';
	color_picker_div.style.backgroundColor='#FFF';
	color_picker_div.style.border='1px solid #317082';
	
	color_picker_div.style.top = COLORPICKER_colorPickerGetTopPos(inputObj) + inputObj.offsetHeight + 2 + 'px';
	color_picker_div.id = 'The_colorPicker';
	color_picker_div.style.display='block';
	color_picker_div.appendChild(COLORPICKER_makeColorSelector(inputObj));	
	document.body.appendChild(color_picker_div);
	COLORPICKER_is_div_init=true;
}

function COLORPICKER_hide()
{	
	if (COLORPICKER_is_div_init){
		 COLORPICKER_is_div_init=false;
		  document.body.removeChild(document.getElementById('The_colorPicker'));
	}
}

function COLORPICKER_maskedHex(input)
{
	var mask = '#[0-9a-fA-F]{7}';
	input.value=input.value.replace(mask,"");
}
