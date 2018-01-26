
/*
    Controlli client-side da effettuare sul file:
    
    - dimenzione massima?
    - estenzioni permesse
    - post-server-request
        - mostrare errore / andato a buon fine
        - se e' un immagine e whereDisplay non e' vuoto mostrare l'immagine
*/

(function($) {
    $.fn.uploader = function(options) {

        this.each(function() {
            /* Caching this */
            var $this = $(this);

            $this.change(function(e) {
                $this = $(this);
                var defaults = { allowedExt: 'JPG|jpg|png|gif|jpeg', submit: 'redirect.php', whereDisplay: '', maxFileSize: '5120' }
                var settings = $.extend({}, defaults, options);


                if ($this.attr('value') != '' && $('#' + $this.attr('id') + '-upload').length == 0) {
                    $('<input id="' + $this.attr('id') + '-upload" type="button" value="Upload Image"/>')
                    .insertAfter('#' + $this.attr('id'));



                    $('#' + $this.attr('id') + '-upload').click(function() {

                        var fileName = $this.attr('value');
                        alert(fileName);
                        if (checkExtension(fileName.substr(fileName.lastIndexOf('.') + 1, fileName.length))) {
                            alert('Extension allowed');
                            var JSONobj = { 'fileName': fileName };


                            $.getJSON(settings['submit'], JSONobj, function(result) {
                                // parsare il result
                                // gestire eventuale errore

                                //Se whereDisplay non e' vuoto e il file uppato e' un immagine devo mostrarlo li.

                            });
                        } else {
                            alert('Error: invalid file extension');
                        }




                    });
                }

                /* Function to check if the file extension is allowed */
                function checkExtension(ext) {

                    if (!(ext && new RegExp('^(' + settings['allowedExt'] + ')$').test(ext)))
                        return false;
                    else
                        return true;
                }
            });







        });
        // returns the jQuery object
        return this;
    }

})(jQuery);

function sendData(id) {
    alert($('#' + id).test);
}
