function dec2hex(textString) {
    return (textString + 0).toString(16).toUpperCase();
}
function getCPfromChar(textString) {
    // converts a character or sequence of characters to hex codepoint values
    // copes with supplementary characters
    // returned values include a space between each hex value and at the end
    var codepoint = "";
    var haut = 0;
    var n = 0;
    for (var i = 0; i < textString.length; i++) {
        var b = textString.charCodeAt(i);
        if (b < 0 || b > 0xFFFF) {
            codepoint += 'Error: Initial byte out of range in getCPfromChar: ' + dec2hex(b);
        }
        if (haut != 0) { // we should be dealing with the second part of a supplementary character
            if (0xDC00 <= b && b <= 0xDFFF) {
                codepoint += dec2hex(0x10000 + ((haut - 0xD800) << 10) + (b - 0xDC00)) + ' ';
                haut = 0;
                continue;
            }
            else {
                codepoint += 'Error: Second byte out of range in getCPfromChar: ' + dec2hex(haut);
                haut = 0;
            }
        }
        if (0xD800 <= b && b <= 0xDBFF) { //b is the first part of a supplementary character
            haut = b;
        }
        else { // this is not a supplementary character
            // codepoint += dec2hex(b);
            codepoint += b.toString(16).toUpperCase() + ' ';
        }
    }
    //alert('>'+codepoint+'<');
    return codepoint;
}
function IsInvalid(c, firstOnlyLetter) {
    if (c == ':')
        return true;

    if (firstOnlyLetter)
        return !IsFirstNameChar(c);
    else
        return !IsNameChar(c);
}

function IsFirstNameChar(ch) {
    if(ch == ':') return false;
    if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch == '_')) {
        return true;
    } else {
        return false;
    }   
}
function IsNameChar(ch) {
    if(ch == ':') return false;
    if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || (ch >= '0' && ch <= '9') ||(ch=='_')) {
        return true;
    }
    else {
        return false;
    }    
}

function WFS_EncodeName(name) {
    var sb = "";
    name = name + "";

    if (name.length == 0)
        return name;


    len = name.length;
    for (i = 0; i < len; i = i + 1) {       
        var c = name.charAt(i);
        if (IsInvalid(c, i == 0)) {
            if(c==':'){
                sb = sb + "_x003A_";
            } else {
                sb = sb + "_x00"+getCPfromChar(c).substring(0,2)+"_";
            }
        } else if (c == '_' && i + 6 < len && name.charAt(i + 1) == 'x' && name.charAt(i + 6) == '_')
            sb = sb + "_x005F_";
        else
            sb = sb + c;
    }
    return sb;
}

function convertCP2Char(textString) {
    var outputString = '';
    textString = textString.replace(/^\s+/, '');
    if (textString.length == 0) { return ""; }
    textString = textString.replace(/\s+/g, ' ');
    var listArray = textString.split(' ');
    for (var i = 0; i < listArray.length; i++) {
        var n = parseInt(listArray[i], 16);
        if (n <= 0xFFFF) {
            outputString += String.fromCharCode(n);
        } else if (n <= 0x10FFFF) {
            n -= 0x10000
            outputString += String.fromCharCode(0xD800 | (n >> 10)) + String.fromCharCode(0xDC00 | (n & 0x3FF));
        } else {
            outputString += 'convertCP2Char error: Code point out of range: ' + dec2hex(n);
        }
    }
    return (outputString);
}

function TryDecoding (s)
{
    if (s.length < 6)
    return s;

    c =  convertCP2Char( s.substring (3, 5) )
    if (s.length == 6)
    return c;
    return c + WFS_DecodeName (s.substring (6,s.length));
}

function WFS_DecodeName(name) {
    name += "";
    if (name.length == 0)
        return name;

    pos = name.indexOf('_');
    if (pos == -1 || pos + 6 >= name.length)
        return name;

    if ((name.charAt(pos + 1) != 'x') || name.charAt(pos + 6) != '_')
        return name.charAt(0) + WFS_DecodeName(name.substring(1, name.length));

    return name.substring(0, pos) + TryDecoding(name.substring(pos + 1, name.length));
}


/*************** XML ENCODING *****************************/

function WFS_XmlEncode(string) {
    if (string == null || typeof string == 'undefined')
        return "";
    if( string.replace )
        return string.replace(/&/g, '&' + 'amp;').replace(/</g, '&' + 'lt;')
                .replace(/>/g, '&' + 'gt;').replace(/\'/g, '&' + 'apos;').replace(/\"/g, '&' + 'quot;');
    return string;
                
}

function WFS_XmlDecode(string) {
    if (string == null || typeof string == 'undefined')
        return ""; 
    if (string.replace)
        return string.replace(/&amp;/g, '&').replace(/&lt;/g, '<')
            .replace(/&gt;/g, '>').replace(/&apos;/g, "'").replace(/&quot;/g, '\"');
    return string;
}

// TESTING ENCODING
/*
    //DEBUG
    var string_test = "stringa_con:e_una /";
    alert("TEST ENCODE: '"+string_test+"' : "+WFS_EncodeName(string_test));
*/