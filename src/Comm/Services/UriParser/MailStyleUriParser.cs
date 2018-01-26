
using System;
using Comm.Util;

namespace Comm
{

    /// <summary>
    /// Custom parser for mail uri
    /// the correct format is mailto:user@password?parameters=...
    /// </summary>
	public class MailStyleUriParser : GenericUriParser
	{		
		int defaultPort = 25;
		
		public MailStyleUriParser() : base(GenericUriParserOptions.NoFragment|GenericUriParserOptions.GenericAuthority|GenericUriParserOptions.Default)
		{
		}

        protected override void InitializeAndValidate(Uri uri, out UriFormatException parsingError)
        {
            parsingError = null;
        }

		protected override void OnRegister(string schemeName, int defaultPort)
        {
            if (schemeName.Equals("mailto")) 
				this.defaultPort = defaultPort;
        }

		protected override string GetComponents(Uri uri, UriComponents components, UriFormat format)
		{
        	String strUri = uri.OriginalString;
			if(strUri.IndexOf(':') == -1)
				throw new UriFormatException("Mail uri must adhere to format mailto:user@password?parameters=...");
			string [] data = strUri.Split(':');
			string parameters = "";
			string email = data[1];
			if(data[1].IndexOf('?') != -1) {
				string [] tokens = data[1].Split('?');
				parameters = tokens[1];
				email = tokens[0];
			} 
			
			if(!Validator.IsEmail(email)) 
				throw new UriFormatException("Invalid mail format");
		
			string ret = "";
			switch (components)
			{
				case UriComponents.UserInfo:
					ret = email;
					break;
				case UriComponents.Query:
					ret = parameters;
					break;
				case UriComponents.Scheme:
					ret = "mailto";
					break;
			}
			
			return ret;
		}
	}
}
