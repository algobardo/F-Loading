function MyVEMap(_mapname, _divname, _latname, _longname, _where, _image, _cimage, _viewonly) {
    
	this.divname = _divname;
	this.latname = _latname;
	this.longname = _longname;
	this.where = _where;
	this.mapname = _mapname;
	this.image = _image;
	this.cimage = _cimage;
	this.viewonly = _viewonly;

	this.maxdists = null;

	this.pushpin = null;
	this.moving = false;

    // draws a circle (used for max distance constraints)
	this.addCircle = function(latin, lonin, radius) {
	    var locs = new Array();
	    var lat1 = latin * Math.PI / 180.0;
	    var lon1 = lonin * Math.PI / 180.0;
	    var d = radius / 6371; //(km:6371, miles:3956)
	    var x;
	    for (x = 0; x <= 360; x += 10) // 36 points
	    {
	        var tc = (x / 90) * Math.PI / 2;
	        var lat = Math.asin(Math.sin(lat1) * Math.cos(d) + Math.cos(lat1) * Math.sin(d) * Math.cos(tc));
	        lat = 180.0 * lat / Math.PI;
	        var lon;
	        if (Math.cos(lat1) == 0) {
	            lon = lonin;
	        }
	        else {
	            lon = ((lon1 - Math.asin(Math.sin(tc) * Math.sin(d) / Math.cos(lat1)) + Math.PI) % (2 * Math.PI)) - Math.PI;
	        }
	        lon = 180.0 * lon / Math.PI;
	        var loc = new VELatLong(lat, lon);
	        locs.push(loc);
	    }

	    poly = new VEShape(VEShapeType.Polyline, locs);
	    poly.SetLineColor(new VEColor(0, 255, 0, 1.0));
	    poly.SetFillColor(new VEColor(0, 255, 0, 0.5));
	    poly.SetTitle("Selectable area");
	    poly.SetDescription("You should select a location inside the circle.");
	    poly.SetCustomIcon(this.cimage);
	    poly.SetLineWidth(2);
	    this.map.AddShape(poly);
	}

    // adds a new max distance constraint
	this.addMaxDist = function(latin, lonin, radius) {
	    if (this.maxdists == null)
	        this.maxdists = new Array();
	    var maxdist = { "latitude": parseFloat(latin.replace(",", ".")), "longitude": parseFloat(lonin.replace(",", ".")), "radius": parseFloat(radius.replace(",", ".")) };
	    this.maxdists.push(maxdist);
	    //this.addCircle(latin, lonin, radius); // added in setUpMap(), after the map creation
	}

    // return true if one of max distance contraints is satisfied
	this.checkMaxDists = function(_loc) {
	    if (this.maxdists == null)
	        return true;

	    var lat1 = _loc.Latitude;
	    var lon1 = _loc.Longitude;

	    for (i = 0; i < this.maxdists.length; i++) {
	        var maxdist = this.maxdists[i];
	        var lat2 = maxdist["latitude"];
	        var lon2 = maxdist["longitude"];
	        var R = 6371; // earth's radium (km)
	        var dLat = (lat2 - lat1) * Math.PI / 180; // toRad (degree * Math.PI / 180)
	        var dLon = (lon2 - lon1) * Math.PI / 180;
	        var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) + Math.cos(lat1 * Math.PI / 180) * Math.cos(lat2 * Math.PI / 180) * Math.sin(dLon / 2) * Math.sin(dLon / 2);
	        var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
	        var d = R * c;

	        if (d <= maxdist["radius"])
	            return true;
	    }
	    return false;
	}
	
	eval(
"	this.GetLocationInfo = function(_locations) \n" + 
"   { \n" + 
"		if(_locations != null) \n" + 
"			" + this.mapname + ".pushpin.SetDescription(_locations[0].Name); \n" + 
"	} "
	);

    // updates the pushpin on the map given a location
	this.updatePushPin = function(_loc) {

	    // check max distance constraints before to update pushpin
	    if (!this.checkMaxDists(_loc))
	        return;

	    if (this.pushpin == null) {
	        this.pushpin = new VEShape(VEShapeType.Pushpin, _loc);
	        this.pushpin.SetTitle('Selected location');
	        this.pushpin.SetDescription('no info available');
	        this.pushpin.SetCustomIcon(this.image);
	        this.map.AddShape(this.pushpin);
	    }
	    else {
	        this.pushpin.SetPoints(_loc);
	    }
	    this.map.FindLocations(_loc, this.GetLocationInfo);
	}

    // updates the latitude and longitude fields according to the pushpin position
	this.updateLatLong = function() {
	    if (this.pushpin == null) 
	        return;
	    var latlon = this.pushpin.GetPoints()[0];
	    var latvalue = '' + (Math.round(latlon.Latitude * 10000) / 10000);
	    var lonvalue = '' + (Math.round(latlon.Longitude * 10000) / 10000);
	    document.getElementById(this.latname).value = latvalue.replace(".", ",");
	    document.getElementById(this.longname).value = lonvalue.replace(".", ",");
	}

	// updates the map position according to the latitude and longitude fields
	this.updateMapLatLong = function() {
	    var loc = new VELatLong(parseFloat(document.getElementById(this.latname).value.replace(",", ".")), parseFloat(document.getElementById(this.longname).value.replace(",", ".")));
	    this.map.SetCenter(loc);
	    this.updatePushPin(loc);
	}

	// updates the map position finding the location described in the search field
	this.updateMapWhere = function(){
		if (document.getElementById(this.where).value != ''){
			try{this.map.Find(null, document.getElementById(this.where).value);} catch(e){alert(e.message);}
		}
	}

	// mouse handlers to move and set the pushpin
    // Note: eval is used to dynamically generate handlers (needed for more maps in a single page, each handler refer to its father map)
	eval(
"	this.mouseDownHandler = function(e) { \n" +
"		if (e.rightMouseButton){ \n" +
"			var loc = " + this.mapname + ".map.PixelToLatLong(new VEPixel(e.mapX, e.mapY)); \n" +
"			" + this.mapname + ".updatePushPin(loc); \n" +
"			" + this.mapname + ".moving = true; \n" +
"			" + this.mapname + ".map.vemapcontrol.EnableGeoCommunity(true); \n" +
"			document.getElementById(" + this.mapname + ".divname).style.cursor = 'Move'; \n" +
"			" + this.mapname + ".updateLatLong(); \n" +
"		} \n" +
"	} "
	);

	eval(
"	this.mouseMoveHandler = function(e) { \n" +
"		if (" + this.mapname + ".moving) { \n" +
"			" + this.mapname + ".map.HideInfoBox(" + this.mapname + ".pushpin); \n" +
"			var loc = " + this.mapname + ".map.PixelToLatLong(new VEPixel(e.mapX, e.mapY)); \n" +
"			" + this.mapname + ".updatePushPin(loc); \n" +
"			" + this.mapname + ".updateLatLong(); \n" +
"		} \n" +
"	} "
	);
	
	eval(
"	this.mouseUpHandler = function(e) { \n" +
"		if (" + this.mapname + ".moving && e.rightMouseButton) { \n" +
"			" + this.mapname + ".moving = false; \n" +
"			" + this.mapname + ".map.vemapcontrol.EnableGeoCommunity(false); \n" +
"			document.getElementById(" + this.mapname + ".divname).style.cursor = ''; \n" +
"			" + this.mapname + ".updateLatLong(); \n" +
"		} \n" +
"	} "
	);

    // Draws the map - to be called after the contructor and before to do everything else =)
	this.draw = function() {

	    try {
	        // map creation
	        this.map = new VEMap(this.divname);
	    } catch (err) {
	        setTimeout(this.mapname + ".draw()", 800);
	        return;
	    }
	    
	    this.map.SetDashboardSize(VEDashboardSize.Tiny);
	    this.map.LoadMap();
	    this.map.SetMapStyle(VEMapStyle.Hybrid);
	    this.map.SetMouseWheelZoomToCenter(false);

	    // adding maxDistance Circles
	    if (this.maxdists != null) {
	        for (i = 0; i < this.maxdists.length; i++) {
	            var maxdist = this.maxdists[i];
	            this.addCircle(maxdist["latitude"], maxdist["longitude"], maxdist["radius"]);
	        }
	    }

	    // Setting view-only mode when needed
	    if (document.getElementById(this.latname).disabled && document.getElementById(this.longname).disabled)
	        this.viewonly = true;

	    // adding pushpin handlers
	    if (!this.viewonly) {
	        this.map.AttachEvent("onmousedown", this.mouseDownHandler);
	        this.map.AttachEvent("onmousemove", this.mouseMoveHandler);
	        this.map.AttachEvent("onmouseup", this.mouseUpHandler);
	    }

	    // Check if a default location is set
	    if (document.getElementById(this.latname).value.length > 0 && document.getElementById(this.longname).value.length > 0) {
	        this.updateMapLatLong();
	    }
	    else {
	        /* this.map.SetCenter(new VELatLong(47.22, -122.44)); //Sets center by location */
	        /* try { this.map.Find(null, 'Pisa, Italia'); } catch (e) { alert(e.message); } // Sets center finding a location */
	        this.map.SetZoomLevel(1); // Just min zoom (all the earth) Zoom range: 1..19
	    }

	}  		
}
