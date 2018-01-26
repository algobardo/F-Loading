using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;


namespace Mobile.Language
{
    public class ExceptionManager
    {
        public static String Error
        {
            get
            {
                return ResourceManager.Instance.GetString("Error");
            }
            
        }
        public static String Success
        {
            get
            {
                return ResourceManager.Instance.GetString("Success");
            }
            
        }
        public static String Warning
        {
            get
            {
                return ResourceManager.Instance.GetString("Warning");
            }

        }
        public static String CompilationRequestIdInteger
        {
            get
            {
                return ResourceManager.Instance.GetString("CompilationRequestIdInteger");
            }
            
        }
        public static String InformationSpecified
        {
            get
            {
                return ResourceManager.Instance.GetString("InformationSpecified");
            }
            
        }
         public static String FormRequestedSubmitted
        {
            get
            {
                return ResourceManager.Instance.GetString("FormRequestedSubmitted");
            }
            
        }
          public static String WrongInformationAuthentication
        {
            get
            {
                return ResourceManager.Instance.GetString("WrongInformationAuthentication");
            }
            
        }
          public static String WebServiceWrongConnection

          {
              get
              {
                  return ResourceManager.Instance.GetString("WebServiceWrongConnection");
              }

          }
          public static String StoringForm
          {
              get
              {
                  return ResourceManager.Instance.GetString("StoringForm");
              }

          }
          public static String LoadingForm
          {
              get
              {
                  return ResourceManager.Instance.GetString("LoadingForm");
              }

          }
          public static String ContentFormat
          {
              get
              {
                  return ResourceManager.Instance.GetString("ContentFormat");
              }

          }
          public static String IntegerValue
          {
              get
              {
                  return ResourceManager.Instance.GetString("IntegerValue");
              }

          }
          public static String InformationSubmitted

          {
              get
              {
                  return ResourceManager.Instance.GetString("InformationSubmitted");
              }

          }
          public static String FormSubmitted
          {
              get
              {
                  return ResourceManager.Instance.GetString("FormSubmitted");
              }

          }
          public static String FormNotSubmitted

          {
              get
              {
                  return ResourceManager.Instance.GetString("FormNotSubmitted");
              }

          }
          public static String FormSaved
          {
              get
              {
                  return ResourceManager.Instance.GetString("FormSaved");
              }

          }

    }
}
