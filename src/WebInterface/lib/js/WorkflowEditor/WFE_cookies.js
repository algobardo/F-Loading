/**
 * USAGE:
 *
 * $.cookie('name');    //get cookie
 * $.cookie('name', 'value');   //set cookie
 * $.cookie('name', null);  //delete cookie
 *
 * **/

var THEME_EDITOR_RETURN_URL_FROM_WFE = 'http://floading.di.unipi.it/WorkflowEditor/WorkflowEditor.aspx';
var THEME_EDITOR_RETURN_URL_FROM_GUI = 'http://floading.di.unipi.it/FormFillier/index.aspx';

//var THEME_EDITOR_RETURN_URL_FROM_WFE = 'http://loa.cli.di.unipi.it:49746/WorkflowEditor/WorkflowEditor.aspx';
//var THEME_EDITOR_RETURN_URL_FROM_GUI = 'http://loa.cli.di.unipi.it:49746/FormFillier/index.aspx';

WFE_cookiesInitialize = function() {
    jQuery.cookie = function(name, value, options) {
        // name and value given, set cookie
        if (typeof value != 'undefined') {
            options = options || {};

            if (value === null) {
                value = '';
                options = $.extend({}, options); // clone object since it's unexpected behavior if the expired property were changed
                options.expires = -1;
            }

            var expires = '';

            if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
                var date;

                if (typeof options.expires == 'number') {
                    date = new Date();
                    date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
                } else {
                    date = options.expires;
                }
                expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
            }
            var path = options.path ? '; path=' + (options.path) : '';
            var domain = options.domain ? '; domain=' + (options.domain) : '';
            var secure = options.secure ? '; secure' : '';

            document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');

        } else {
            // only name given, get cookie
            var cookieValue = null;

            if (document.cookie && document.cookie != '') {
                var cookies = document.cookie.split(';');

                for (var i = 0; i < cookies.length; i++) {
                    var cookie = jQuery.trim(cookies[i]);

                    // Does this cookie string begin with the name we want?
                    if (cookie.substring(0, name.length + 1) == (name + '=')) {
                        cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                        break;
                    }
                }
            }
            return cookieValue;
        }
    }
}

WFE_removeWFcookie = function() {
    var activeWorkflowList = $.cookie("WFE_ActiveWorkflowList");

    $.cookie("WFE_ActiveWorkflowList", null, { path: '/' });

    if (activeWorkflowList == null) return;

    var ids = activeWorkflowList.split('|');

    for (var i = 0; i < ids.length; i++) {
        $.cookie(ids[i], null, { path: '/' });
    }
    $.cookie("theme_edited", null, { path: '/' });
}

WFE_setWFcookie = function(wfID,status_to_set) {
    if(!status_to_set)status_to_set = 'saved';
    var activeWorkflowList =  $.cookie("WFE_ActiveWorkflowList");
    var cookie_to_set = wfID;
//    if(activeWorkflowList != null){
//        cookie_to_set = activeWorkflowList + '|' + wfID;
//    } else {
//        cookie_to_set = wfID;
//    }
    //TOCHANGE: Edit more workflow at time!
    cookie_to_set = wfID;
    
    $.cookie("WFE_ActiveWorkflowList", cookie_to_set, { path: '/' }); //setting the edited wfid in the cookies
    $.cookie(wfID, status_to_set, { path: '/' });
}

WFE_setEditedThemeCookie = function(){
    $.cookie("theme_edited", "true", { path: '/' });
}

WFE_unsetEditedThemeCookie = function(){
    $.cookie("theme_edited", null, { path: '/' });
}

WFE_checkIfThemeWasEdited = function(){
    if($.cookie("theme_edited") == "true") return true;
    return false;
}

//Set the cookie to save the html representing the edge between from_name and to_name
WFE_setPredicatesHtmlCookie = function(html,from_name,to_name){
    $.cookie(from_name+"|"+to_name, html, {path: '/'});
}

//Get the cookie to save the html representing the edge between from_name and to_name
WFE_getPredicatesHtmlCookie = function(from_name,to_name){
    $.cookie(from_name+"|"+to_name);
}

WFE_setUsingStaticFieldCookie = function(bool){
    $.cookie("UsingStaticField", bool, {path: '/'});
}

WFE_getUsingStaticFieldCookie = function(){
    return $.cookie("UsingStaticField");
}

WFE_setReturnUrlCookie = function(url) {
    $.cookie("ReturnUrl", url, { path: '/' });
}

WFE_getReturnUrlCookie = function() {
    return $.cookie("ReturnUrl");
}