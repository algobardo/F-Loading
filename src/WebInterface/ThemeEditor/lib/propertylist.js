var PropertyList=function(){
    var StringBoxPropertyList = [
    { 
	    "group":"border",
	    "properties": [
		    {
			    "name":"border-width",
			    "type":"text",
			    "validator":"size",
			    "info": "Width of Field Border. Example: 2px"
		    },
		    {
			    "name":"border-color",
			    "type":"text",
			    "validator":"color",
			    "info": "Color of Field Border. Example: #ff00ff"
		    },
		    {
			    "name":"border-style",
			    "type":"text",
			    "validator":"none",
			    "info": "Type of Field Border. Example: solid"
		    }
	    ]                     
   },{ 
	    "group":"background",
	    "properties": [
		    {
			    "name":"background-color",
			    "type":"text",
			    "validator":"color",
			    "info": "Color of Field background. Example: #ff00ff"
		    }
	    ] 
		
   }] 
   return{
        getList:function( type ){
            if( eval( "typeof " + type + "PropertyList == 'undefined'" ) ) {
                type = "StringBox";
            }            
            return eval( type + "PropertyList");
        },
        validate:function( type, value ){
			if( type == "color" )
				return value.match( /#[a-fA-F0-9]{6}/ );
			if( type == "size" ) 
				return ( value.match( /\d+px/ ));
			if( type == "none" )
				return true;
			return false;            
        }
   };
}();