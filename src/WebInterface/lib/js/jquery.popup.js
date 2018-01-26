/**
   * Show a pop-up with given title and message.
   */
jQuery.popup = {

    /*
    * options : optional settings, can contain following params:
    *          convertNLtoBR : if true, will convert new lines (\n) to <br/> in message
    *          main_class : class names to be added on main popup <div> tag
    *          xxx_id : id to use for popup elements, shouldn't need to modify
    */

    show: function(title, message, container, posx, posy, options) {

        // define defaults and override with options if available
        settings = jQuery.extend({
            convertNLtoBR: true,
            main_class: "",
            main_id: "popup",
            bg_id: "popup_bg",
            title_id: "popup_title",
            msg_id: "popup_message",
            close_id: "popup_close"
        }, options);

        if (!this.initialized) {
            // inject needed elements in DOM
            domElements = '<div id="' + settings.bg_id + '"></div>';
            domElements += '<div id="' + settings.main_id + '" class="' + settings.main_class + '">';
            domElements += '<span id="' + settings.title_id + '"></span><a id="' + settings.close_id + '"> </a>';
            domElements += '<div id="' + settings.msg_id + '"></div>';
            domElements += '</div>';
            //jQuery('#'+container).append(domElements);
            jQuery(container).append(domElements);
            // call given method after DOM has been altered (maybe user wants to attach to elements, or whatever)

            // setup event handlers
            // popup close by outer click
            jQuery('#' + settings.bg_id).click(function() { hidePopup(); });
            jQuery('#' + settings.close_id).click(function() { hidePopup(); });

            this.initialized = true;
        }

        // convert \n into <br/> if asked to (only in message param)
        if (settings.convertNLtoBR) {
            message = message.replace(/\n/g, "<br/>");
        }

        // prepare popup content
        jQuery('#' + settings.title_id).html(title);
        jQuery('#' + settings.msg_id).html(message);
        // display.. tadaaa!
        showPopup();

        // show popup
        function showPopup() {
            // loads popup only if it is disabled
            if (!this.showing) {
                positionPopup();
                jQuery('#' + settings.bg_id).css({ "opacity": "0.6" });
                jQuery('#' + settings.bg_id).fadeIn("slow");
                jQuery('#' + settings.main_id).fadeIn("slow");
                this.showing = true;
            }
        }

        // hide popup
        function hidePopup() {
            // disables popup only if it is enabled
            if (this.showing) {
                jQuery('#' + settings.bg_id).fadeOut("normal");
                jQuery('#' + settings.main_id).fadeOut("normal");
                this.showing = false;
            }
        }

        // center popup in viewport
        function positionPopup() {

            // get viewport dimensions
            var cWidth = document.documentElement.clientWidth;
            var cHeight = document.documentElement.clientHeight;
            var popupHeight = jQuery('#' + settings.main_id).height();
            var popupWidth = jQuery('#' + settings.main_id).width();

            // positionning

            jQuery('#' + settings.main_id).css({
                "top": posy, //cHeight/2-popupHeight/2, 
                "left": posx//cWidth/2-popupWidth/2
            });
            // IE6 
            jQuery(settings.bg_id).css({ "height": cHeight });
        }

        return jQuery;

    }, // end show function

    // jQuery.fn.name = function(..){...} => call $('selector').name
    // jQuery.name = function(..){...} => call $.name
    // jQuery.namespace = {name: function(..){...}, .. } => call $.namespace.name
    // inside plugin: use jQuery, not $ alias which might not exist

    // popup ready or not
    initialized: false,

    // false = disabled, true = enabled
    showing: false

};   // ';' required or will break if compressed