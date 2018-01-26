using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Threading;
using Core.WF;

namespace Floading
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load()
        {
            Session["contactReturn"] = "/FormFillier/index.aspx";
        }
    }
}
