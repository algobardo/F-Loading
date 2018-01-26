using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Security;
using Core.WF;

namespace WebInterface
{
    public partial class ResetSession : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Threading.ThreadPool.SetMinThreads(20,100);
            foreach(var wf in Session){
                if(Session[(string)wf] is Workflow)
                Literal2.Text+=wf+"--";
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Application.Clear();    
        }

        Workflow wf;
        Token t;
        protected void Button2_Click(object sender, EventArgs e)
        {    
            wf = Session[TextBox1.Text] as Workflow;
            
            t = (Token)Page.Session["Token"];         
            for (int i = 0; i < 200; i++)
            {
               System.Threading.ThreadPool.QueueUserWorkItem(procNewOne);
            }
            
        }

        private void procNewOne(Object state)
        {
            try
            {
                if (t != null && t.Authenticated)
                {
                    User currentUser = t.GetCurrentUser();
                    wf.WorkflowName = "TESTDBFLOADING";
                    WorkflowReference wfRef = currentUser.AddNewWorkFlow(wf, "--");
                    if (wfRef == null)
                    {

                    }
                    ComputableWorkflowReference computebleWfRef = null;
                    computebleWfRef = wfRef.MakeComputable(FormType.PUBLIC_WITH_REPLICATION, "--","");
                }
            }
            catch
            {
            }
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            Token t = (Token)Page.Session["Token"];
            if (t != null && t.Authenticated)
            {
                User currentUser = t.GetCurrentUser();
                
                var cwfs = currentUser.GetComputableWorkflows();
                var cwfs2 = currentUser.GetEditableWorkflows();
                foreach (var wf in cwfs)
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(rmv,wf);
                }                
                foreach (var wf2 in cwfs2)
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(rmv, wf2);
                }
            }
        }

        private void rmv(object state)
        {
            try
            {
                if (state is WorkflowReference)
                    (state as WorkflowReference).Remove();
                else
                    (state as ComputableWorkflowReference).Remove();
            }
            catch
            {
            }
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            Fields.FieldsManager.LoadTypes();
        }

    }
}
