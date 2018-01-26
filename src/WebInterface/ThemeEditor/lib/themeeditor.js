/* Add edit buttons and toolbox placeholder div ( named "grid"+id )to all <input> nodes*/
//window.onload = initTE;



function RGBtoHex( tmp ) {
    tmp = tmp.replace(/rgb\(/i, '');
    tmp = tmp.replace(/\)/, '');
    tmp = tmp.split(',');
    return "#"+toHex(tmp[0]) + toHex(tmp[1]) + toHex(tmp[2]) 
}
function toHex(N) {
    if (N == null) return "00";
    N = parseInt(N); if (N == 0 || isNaN(N)) return "00";
    N = Math.max(0, N); N = Math.min(N, 255); N = Math.round(N);
    return "0123456789ABCDEF".charAt((N - N % 16) / 16)
      + "0123456789ABCDEF".charAt(N % 16);
}

function matchRGB(tmp) {
    return tmp.match(/rgb\([ ]*[0-9]{1,3}[ ]*,[ ]*[0-9]{1,3}[ ]*,[ ]*[0-9]{1,3}[ ]*\)/i);
}


function addEditButtons(){
    /*To-Do Filter for non editable fields on page */
   var tmp;   
   var btn;
   var i;

   $("#ctl00_contentHome_presenPanel [rel]").each(function() {
       tmp = document.createElement("div");
       tmp.setAttribute("id", "grid" + this.getAttribute("id"));
       
       tmp.setAttribute("class", "grid");


       editLink = document.createElement("div");
       editLink.setAttribute("class", "editLink");
       editLink.setAttribute('onclick', "toolbox.generatePropertyGrid('" + this.getAttribute("id") + "','" + this.getAttribute("rel") + "');");
       editLink.appendChild(document.createTextNode("Edit"));

       tmp.appendChild(editLink);
       
       
       // Append it right after <input> field
       this.parentNode.insertBefore(tmp, this.nextSibling);

   })
}

/*
    Initialize ThemeEditor
*/
function initTE( themeFormTitle ){
    // Generate edit buttons
    document.getElementById('titleEditBox').setAttribute('value', themeFormTitle);
    changeTitle(themeFormTitle);
    addEditButtons();
    //
    $('#titleSize').attr('value', $('#ctl00_contentHome_formTitle').css('font-size'));
    $('#color_formTitle').attr('value', RGBtoHex($('#ctl00_contentHome_formTitle').css('color')));
}
function changeTitle( value ){
    var titleNode = document.getElementById("ctl00_contentHome_formTitle");
    document.getElementById('generatedTitle').setAttribute('value', value);   
    titleNode.replaceChild( document.createTextNode( value ), titleNode.firstChild );

    document.title = value + " - Floading ";
}

function showCSS() {
    $.jGrowl(toolbox.generateCSS(), {
    theme: 'growlfloading',
        position: 'center',
        header: "CSS Generated",
        sticky: true,
        speed: 'slow'
    });
    
}

/*
    Populate two hidden field of the form to send the 
    generated CSS and the generated Title of the user's form
*/
function submitFormData() {

    var 
        hiddenCSS,     // The hidden TextArea where to put the generated CSS
        hiddenTitle;        // The hidden input type text where to put the generated Title

    // Retrive the two DOM element
    hiddenCSS = document.getElementById('generatedCss');
    hiddenTitle = document.getElementById('generatedTitle');

    // Settin their value
    hiddenCSS.appendChild(document.createTextNode(toolbox.generateCSS()));
    //hiddenTitle.setAttribute('value', document.getElementById('titleEditBox').innerHTML);

    // Some debug informations
   
    var dataString = "generatedTitle=" + hiddenTitle.getAttribute('value') + "&generatedCss=" + toolbox.generateCSS() + "&skipTheme=false";
    // Submitting the form


    $.ajax({
        type: "POST",
        url: $('#form2').attr('action'),
        data: dataString,
        success: function(msg) {
            var jsonResponse = eval('(' + msg + ')');
            if (jsonResponse.status == "STATUS_OK") {
                $.jGrowl("The new theme has been saved", {
                    theme: 'growlfloading',
                    position: 'center',
                    header: "Save Theme",
                    life: 2000,
                    speed: 'slow',
                    animateOpen: {
                        height: "show"
                    },
                    animateClose: {
                        height: "hide"
                    },
                    close: function(e, m, o) {
                        WFE_setEditedThemeCookie();
                        var url = WFE_getReturnUrlCookie();
                        
                        //WFE_setReturnUrlCookie(null);
                        window.location = url;
                    }
                });
            }
            else if (jsonResponse.status == "NO_TOKEN") {
                $.jGrowl("You need to be authed to save the theme! Please login and try again.", {
                    theme: 'growlfloading',
                    position: 'center',
                    header: "Error!",
                    sticky: true,
                    speed: 'slow',
                    animateOpen: {
                        height: "show"
                    },
                    animateClose: {
                        height: "hide"
                    }
                });
            }
            else {
                $.jGrowl("There are some error saving in the database", {
                    theme: 'growlfloading',
                    position: 'center',
                    header: "Error!",
                    sticky: true,
                    speed: 'slow',
                    animateOpen: {
                        height: "show"
                    },
                    animateClose: {
                        height: "hide"
                    }
                });
            }

        },
        error: function() {
            alert("ERRORE CONNESSIONE!");
        }
    });

       
}

function skipThemeEditor() {

    // Submitting the form skipping theme editor saving
    
    /*
    $.ajax({
        type: "POST",
        url: $('#form2').attr('action'),
        data: "skipTheme=true",
        success: function(msg) {
            var jsonResponse = eval('(' + msg + ')');
            if (jsonResponse.status == "STATUS_OK") {
                $.jGrowl("The theme edit has been skipped", {
                theme: 'growlfloading',
                    position: 'center',
                    header: "Skip theme",
                    life: 2000,
                    speed: 'slow',
                    animateOpen: {
                        height: "show"
                    },
                    animateClose: {
                        height: "hide"
                    },
                    close: function(e,m,o) {
                        window.location = '../WorkflowEditor/WorkflowEditor.aspx';
		            }                        
                });
            }
            else {
                $.jGrowl("There are some error saving in the database", {
                    theme: 'growlfloading',
                    position: 'center',
                    header: "Error!",
                    sticky: true,
                    speed: 'slow',
                    animateOpen: {
                        height: "show"
                    },
                    animateClose: {
                        height: "hide"
                    }                     
                });
            } 
                
        },
        error: function(XMLHttpRequest, textStatus, errorThrown) {
            alert(textStatus);
            alert(errorThrown);
        }

    });
    */
    $.jGrowl("The theme edit has been skipped", {
        theme: 'growlfloading',
        position: 'center',
        header: "Skip theme",
        life: 2000,
        speed: 'slow',
        animateOpen: {
            height: "show"
        },
        animateClose: {
            height: "hide"
        },
        close: function(e, m, o) {
            var url = WFE_getReturnUrlCookie();
            //WFE_setReturnUrlCookie(null);
            window.location = url;
        }
    });
    
       
}


function ShowUploadFrame(){
    $('#UploadLogoFrame').show();
}

function logoDelete()
     {
        $("#logo-delete-response").load("SaveLogo.aspx?deleteImg=1");
        document.getElementById("logo-div").removeChild(document.getElementById("logo-img"));
        document.getElementById("logo-delete").style.display="none";
     }
     
     
	function cssUpload()
	{
	    document.getElementById("css_upload_img").style.display="inline";
	    return true;
	}
	
	function cssUploadDone(msg, success) { 
	    //Hide ajax loading circle
	    document.getElementById("css_upload_img").style.display = 'none';
	    
	    if (success)
	    {
	    //Show OK arrow
	    document.getElementById('cssupIMG').setAttribute('src',"resources/check.png");
	    document.getElementById('cssupIMG').style.display="inline";
	    
	    //Show link to css file inline editor
	    document.getElementById("css-div").innerHTML = '<a href="'+"CssEdit.aspx?cssFile=themeCss.css"+'" target="_blank">view/edit CSS file</a>';
	    document.getElementById("css-div").attributes.visibility = 'visible';
	    }
	    else
	    {
	    document.getElementById("css-div").innerHTML = msg;
	    document.getElementById("css-div").attributes.visibility = 'visible';
	    
	    }
	    return true;
	}

	function startUpload() {
        document.getElementById("logo_upload_img").style.display = "inline";
        //document.getElementById("logo-div").removeChild(document.getElementById("logo-img"));
        
          return true;
    }

    function stopUpload(msg, success){
          var result = '';
          if (success) {
              if (document.getElementById("logo-img"))
                  document.getElementById("logo-div").removeChild(document.getElementById("logo-img"));
              document.getElementById("logo-delete").style.display = "none";

            var img = document.createElement('img');
            
            var now = new Date();
            img.setAttribute('src', msg + "?id="+ now.getTime());
            img.setAttribute('id', 'logo-img');
            
            document.getElementById("logo-div").appendChild(img);
            
            
            var del = document.createElement('input');
            del.setAttribute('type', 'button');
            del.setAttribute('value', 'Delete');
            del.setAttribute('id', 'logo-del');
            //document.getElementById("logo-div").appendChild(del);

            document.getElementById("logo-delete").style.display = "inline";
            //document.getElementById("imgfile").setAttribute('value', '');
            //uncaught exception: [Exception... "Component returned failure code: 0x80004003 (NS_ERROR_INVALID_POINTER) [nsIDOMHTMLDivElement.removeChild]" nsresult: "0x80004003 (NS_ERROR_INVALID_POINTER)" location: "JS frame :: http://loa.cli.di.unipi.it:49746/ThemeEditor/ThemeEditor.aspx :: startUpload :: line 99" data: no]
          }
          else {
              $.jGrowl(msg, {
                  theme: 'growlfloading',
                  position: 'center',
                  header: "Upload Logo Error!",
                  life: 2000,
                  speed: 'slow',
                  animateOpen: {
                      height: "show"
                  },
                  animateClose: {
                      height: "hide"
                  }
              });
              //document.getElementById('logo-div').innerHTML=msg;
              //document.getElementById("logo-div").style.display="inline";
          }
          
          //document.getElementById('logo_upload_form').style.display = 'inline';  
          document.getElementById("logo_upload_img").style.display="none"; 
          
          return true;
    }
    
    $("#accordion").tabs("#accordion div.pane", { 
    tabs: 'h2',  
    effect: 'slide',
     event: 'mouseover' 
});