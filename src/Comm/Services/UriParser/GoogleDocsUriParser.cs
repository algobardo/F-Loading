
using System;

namespace Comm
{

    /// <summary>
    /// Custom parser for gdocs uri
    /// the correct format is gdocs:user:password
    /// </summary>
	public class GoogleDocsUriParser : GenericUriParser
	{		
		int defaultPort = 182;
		
		public GoogleDocsUriParser() : base(GenericUriParserOptions.NoFragment|GenericUriParserOptions.GenericAuthority|GenericUriParserOptions.Default)
		{
		}

        protected override void InitializeAndValidate(Uri uri, out UriFormatException parsingError)
        {
            parsingError = null;
        }

        protected override void OnRegister(string schemeName, int defaultPort)
        {
            if (schemeName.Equals("gdocs")) 
				this.defaultPort = defaultPort;
        }

		protected override string GetComponents(Uri uri, UriComponents components, UriFormat format)
		{
        	String strUri = uri.OriginalString;
            int c = strUri.IndexOf(':');
			if(c == -1 || c == strUri.Length)
				throw new UriFormatException("Gdocs uri must adhere to format gdocs:user:password");
            string user = strUri.Substring(c+1);
			
			string ret = "";
			switch (components)
			{
				case UriComponents.UserInfo:
                    ret = user;
					break;
				case UriComponents.Scheme:
					ret = "gdocs";
					break;
			}
			
			return ret;
		}
	}
}