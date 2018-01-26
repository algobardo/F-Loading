/* 
    Name: jQuery.tooltips.js 
    Version: 0.1
    

    jQuery plugin used to create tooltips on the right side of the input when focused.
    When the input loses the focus the tooltip will be removed.
    In the tooltip will be displayed the text that is writed in the 'title' attribute of the input.
       
    To be decided: 
    - Diplay the tooltip on mouse over and follow 
    it will follow the mouse until the mouse will go out of the input field

    To add:
    - Finish the tooltip.css
    - Other settings
    
    
    USAGE ( maybe will change ): 
    
    index.aspx:
        - include tooltips.css
        - include jQuery.tooltips.js
        - add the title attribute to each input that you want to display a tooltip
        
        - add this part of code at the end of the body ( maybe will change )
        
               <script type="text/javascript">
                    $('input').tooltips();      
               </script>
               
        - enjoy :)
    
*/


(function($) {
    $.fn.tooltips = function() {

        
        this.each(function() {
            /* Caching this */
            var $this = $(this);
            var title = this.title;
            /* Checking if it's an input and the attribute title is not empty */
            if ($this.is('input') && $this.attr('title') != '') {

                /* Removing the title from the input */

                this.title = '';

                /* When the input element is on focus it will display a tooltip on it's right side */
                
                $this.focus(function(e) {
                    $('<div class="tooltip-rounded" id="tooltip" />')
                    .insertAfter('#' + $this.attr('id'))
                    .text(title)
                    .hide()
                    .css({
                        top: e.pageY + 10,
                        left: e.pageX + 20
                    })
                    .fadeIn(200);
                });

                /* When the input loses the focus the tooltip is removed */
                $this.blur(function() {
                    //$('#tooltip').fadeOut(400);
                    $('#tooltip').remove();
                });
            }
        });
        // returns the jQuery object
        return this;
    }
})(jQuery);
