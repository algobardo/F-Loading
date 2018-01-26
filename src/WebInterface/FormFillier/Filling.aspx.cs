using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using Core.WF;
using Core;
using System.IO;
using Fields;
using System.Diagnostics;
using WebInterface.FormFillier.action;


namespace WebInterface.FormFillier
{
    public class OverridingNamespacePlaceHolder : PlaceHolder, INamingContainer
    {
        public void ResetStates()
        {
            //ClearChildState();
        }
    }

    public partial class Filling : System.Web.UI.Page, ICallbackEventHandler
    {


        #region Variable

        IComputableWorkflow workflow;
        OverridingNamespacePlaceHolder nodePlaceHolder = new OverridingNamespacePlaceHolder();
        string fromClient;

        #endregion

        #region Handler error
        private void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            String csname1 = "PopupScript";

            Type cstype = this.GetType();

            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager cs = Page.ClientScript;

            // Check to see if the startup script is already registered.
            if (!cs.IsStartupScriptRegistered(cstype, csname1))
            {
                String cstext1 = "alert(\"Form non valida " + e.Message.FromNotEncodedRender2XmlValue() + "\");";

                cs.RegisterStartupScript(cstype, csname1, cstext1, true);
            }
        }
        #endregion

        #region TEST WF
        public WFnode testNode1()
        {
            XmlDocument d = new XmlDocument();
            d.LoadXml(Core.TestResources.NodeWithComplexTypeRendering2);

            XmlSchemaSet ss = new XmlSchemaSet();
            ss.Add(Utils.ReadSchema(Core.TestResources.NodeWithComplexTypeSchema2));

            var nd = new WFnode("pippO");
            nd.ModifyNode(ss, d, "Nodo1");
            return nd;
        }
        public WFnode testNode2()
        {
            XmlDocument d = new XmlDocument();
            d.LoadXml(Core.TestResources.NodeWithComplexTypeRendering2.Replace("Nodo1", "Nodo2"));

            XmlSchemaSet ss = new XmlSchemaSet();
            ss.Add(Utils.ReadSchema(Core.TestResources.NodeWithComplexTypeSchema2.Replace("Nodo1", "Nodo2").Replace("StringBox0", "StringBox7")));

            var nd = new WFnode("pippO");
            nd.ModifyNode(ss, d, "Nodo2");
            return nd;
        }

        public IComputableWorkflow testValidationGUI()
        {
            WFnode nd1 = testNode1();
            WFnode nd2 = testNode2();

            WFedgeLabel edge = new WFedgeLabel();
            XmlDocument d = new XmlDocument();
            d.LoadXml("<PRECONDITIONS></PRECONDITIONS>");
            edge.ModifyPrecondition(d);

            Workflow wf = new Workflow("PIPPO");

            wf.AddNode(nd1);
            wf.AddNode(nd2);
            wf.AddEdge(edge, nd1, nd2);

            return wf.Save();

        }
        #endregion

        #region Rendering


        private void renderNode(Core.WF.WFnode state)
        {
            Trace.Warn("Begin render node");
            //nodePlaceHolder.Controls.Clear();
            presenPanel.Controls.Clear();

            var cmplexRenering = state.GetRenderingDocument().DocumentElement.FirstChild;
            presenPanel.Controls.Add(state.GetWebControl(Page.Server, cmplexRenering));
            //nodePlaceHolder.Controls.Add(state.GetWebControl(Page.Server, cmplexRenering));
            Trace.Warn("End render node");
        }

        private void renderFinal(Core.WF.WFnode state, XmlNode nodeV)
        {
            //presenPanel3.Visible = false;
            clear.Visible = false;

            Panel pf = new Panel();
            pf.Attributes.Add("class", "header");
            HyperLink h = new HyperLink();
            h.Attributes.Add("href", "#");
            h.Text = state.Name;

            pf.Controls.Add(h);

            presenPanel2.Controls.Add(pf);


            state.Value = nodeV;
            var cmplexRenering = state.GetRenderingDocument().DocumentElement.FirstChild;
            Control wc = state.GetWebControl(Page.Server, cmplexRenering);

            Panel p = new Panel();
            Panel pControls = new Panel();
            Panel pModify = new Panel();

            pControls.Enabled = false;
            pControls.Controls.Add(wc);

            for (int i = 0; i < presenPanel3.Controls.Count; i++)
            {
                if (presenPanel3.Controls[i] != null)
                {
                    if (((ImageButton)presenPanel3.Controls[i]).ToolTip == state.Name)
                    {
                        ((ImageButton)presenPanel3.Controls[i]).Visible = true;
                        ((ImageButton)presenPanel3.Controls[i]).ImageUrl = "../lib/image/Modify32.PNG";
                        ((ImageButton)presenPanel3.Controls[i]).Enabled = true;
                        ((ImageButton)presenPanel3.Controls[i]).ToolTip = "Modify";

                        pModify.Controls.Add(presenPanel3.Controls[i]);
                    }
                }
            }// TO DO : clear presenpanel3

            pModify.CssClass = "pModify";
            p.Controls.Add(pModify);
            p.Controls.Add(pControls);
            presenPanel2.Controls.Add(p);
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            List<Control> listControl = new List<Control>();
            string cbReference = this.ClientScript.GetCallbackEventReference(this, "arg", "ReceiveServerData", "");
            string callBackScript = @"function CallServer(arg, context){" + cbReference + ";}";
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "CallServer", callBackScript, true);

            listControl.Add(new ServiceFakeLog());
            List<Control>.Enumerator it = listControl.GetEnumerator();
            while (it.MoveNext())
            {
                callFill.Controls.Add(it.Current);

                IControlFormFiller t = (IControlFormFiller)it.Current;
                cbReference = Page.ClientScript.GetCallbackEventReference(it.Current, "arg", t.getNameFunctionServerResponse(), "context");
                callBackScript = @"function " + t.getNameFunctionServerCall() + "(arg, context)" + "{ " + cbReference + ";}";
                Page.ClientScript.RegisterClientScriptBlock(it.Current.GetType(), t.getNameFunctionServerCall(), callBackScript, true);


            }


            if (((IComputableWorkflow)Session["wf"]) != null)
            {
                workflow = (IComputableWorkflow)Session["wf"]; //RIGA DI FIX PER NULL??!!!
                presenPanel.Attributes.Add("style", "position:relative;left:800px;width:0px;display:none;");

                /////////////////////////////////HISTORY//////////////////////////////////////////////////////////////////

                IEnumerable<WFnode> nl = workflow.getNodeList();
                List<string> nodeNames = new List<string>();
                foreach (WFnode n in nl)
                {
                    nodeNames.Add(n.Name);
                }

                List<string> ns = new List<string>();
                ns = workflow.GetThroughPath();

                if (ns.Count > 0)
                {
                    string initNode = "";
                    int initNodeIndex = 0;

                    for (int i = 0; i < nodeNames.Count; i++)
                    {
                        if (nodeNames[i] == ns[ns.Count - 1])
                        {
                            initNode = nodeNames[i];
                            initNodeIndex = i;
                        }
                    }
                    string tmp = nodeNames[0];

                    nodeNames.RemoveAt(0);
                    nodeNames.Insert(0, initNode);
                    nodeNames.RemoveAt(initNodeIndex);
                    nodeNames.Insert(initNodeIndex, tmp);

                    int h = 1;
                    for (int i = ns.Count - 2; i >= 0; i--)
                    {
                        nodeNames.RemoveAt(h);
                        nodeNames.Insert(h, ns[i]);
                        h++;
                    }
                    //if (ns.Count < nodeNames.Count)
                    //{
                    //    nodeNames.RemoveAt(ns.Count);
                    //    nodeNames.Insert(ns.Count, workflow.GetState().Name);
                    //}

                }
                else
                {
                    string initNode = "";
                    int initNodeIndex = 0;

                    for (int i = 0; i < nodeNames.Count; i++)
                    {
                        if (nodeNames[i] == workflow.GetState().Name)
                        {
                            initNode = nodeNames[i];
                            initNodeIndex = i;
                        }
                    }
                    string tmp = nodeNames[0];

                    nodeNames.RemoveAt(0);
                    nodeNames.Insert(0, initNode);
                    nodeNames.RemoveAt(initNodeIndex);
                    nodeNames.Insert(initNodeIndex, tmp);
                }

                int W = ((nodeNames.Count() - 1) * 16) + 32 + ((nodeNames.Count()) * 5);
                int L = 480 - W / 2;
                presenPanel3.Attributes.Add("style", "position:relative;width:" + W + "px;margin:4px 0px 10px 0px;left:" + L + "px;");//"style", "position:relative;width:100%;margin:4px 0px 10px 0px;left:0px;"

                WFnode actual = workflow.GetState();
                int j = 0;
                presenPanel3.Controls.Clear();
                foreach (string n in nodeNames)
                {
                    ImageButton l = new ImageButton();
                    l.ImageUrl = "../lib/image/backNode.PNG";
                    l.CssClass = "HistoryLabel";
                    //if (j == 0)
                    //    l.ToolTip = nodeNames[0];
                    l.ToolTip = nodeNames[j];
                    if (!IsPostBack)
                    {
                        if (j == 0)
                        {
                            l.ImageUrl = "../lib/image/currentNode.PNG";
                            l.Attributes.Add("style", "margin-left:5px;");

                        }
                        else
                        {
                            l.Enabled = false;
                            l.ImageUrl = "../lib/image/forwardNode.PNG";
                            l.Attributes.Add("style", "margin-left:5px;margin-bottom:8px;");
                        }
                    }
                    l.ID = j.ToString();
                    //l.OnClientClick = "slideRightToLeft();";
                    l.Click += new ImageClickEventHandler(l_Click);
                    l.CausesValidation = false;
                    presenPanel3.Controls.Add(l);
                    j++;

                }


                /////////////////////////////////HISTORY//////////////////////////////////////////////////////////////////
            }
        }

        protected void forward_Click(object sender, EventArgs e)
        {

            bool final = workflow.IsFinalState();
            WFnode actual = workflow.GetState();
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(actual.GetValue(/*nodePlaceHolder.Controls[0]*/presenPanel.Controls[0], actual.GetRenderingDocument().DocumentElement.FirstChild));
            ActionResult ar = workflow.ComputeNewStatus(WFeventType.TRYGOON, doc, ValidationEventHandler);
            if (ar.OK && !final)
            {
                //nodePlaceHolder.ResetStates();
                //nodePlaceHolder.Controls.Clear();
                presenPanel.Controls.Clear();
                WFnode currentNode = workflow.GetState();
                renderNode(currentNode);

                /////////////////////////////////HISTORY//////////////////////////////////////////////////////////////////
                UpdateHistory(currentNode, ar);
                /////////////////////////////////HISTORY//////////////////////////////////////////////////////////////////
            }
            else if (ar.OK && final)
            {
                //nodePlaceHolder.ResetStates();
                //nodePlaceHolder.Controls.Clear();
                presenPanel.Controls.Clear();
                //nodePlaceHolder.Controls.Add(new Panel());//To not handle generic error dialog
                presenPanel.Controls.Add(new Panel());
                presenPanel2.Visible = true;
                forward.Visible = false;
                save.Visible = true;

                XmlDocument xd = workflow.GetCollectedDocument();
                xd.Schemas = workflow.GetCollectedDocumentSchemas();
                xd.Validate(null);//bug
                XmlNodeList ln = xd.ChildNodes[1].ChildNodes;
                IEnumerable<WFnode> nodeList = workflow.getNodeList();

                foreach (XmlNode xnd in ln)
                {
                    foreach (WFnode wfn in nodeList)
                    {
                        if (wfn.Name == XmlConvert.DecodeName(xnd.Name))
                            renderFinal(wfn, xnd);
                    }
                }
                presenPanel3.Controls.Clear();

            }
            else
            {
                /*It remains here...but someone should say him that there is no edge or his input doesn't validate*/
                WFnode currentNode = workflow.GetState();
                UpdateHistory(currentNode, ar);
            }

        }

        private void errHandler(object src, System.Xml.Schema.ValidationEventArgs e)
        {
            //Page.Controls.Add(new LiteralControl("<script>alert('"+e.Message+"')</script>"));
        }


        protected void back_Click(object sender, EventArgs e)
        {
            //need to check wmldoc = null!!!!
            ActionResult ar;
            if (save.Visible == true)
            {
                presenPanel3.Visible = true;
                clear.Visible = true;
                ar = workflow.ComputeNewStatus(WFeventType.ROLLBACK, null, ValidationEventHandler);
                forward.Text = "Forward";
                forward.Visible = true;
                save.Visible = false;
                //nodePlaceHolder.ResetStates();
                presenPanel2.Controls.Clear();
                presenPanel2.Visible = false;
                renderNode(workflow.GetState());
            }
            else
            {
                ar = workflow.ComputeNewStatus(WFeventType.ROLLBACK, null, ValidationEventHandler);
                //nodePlaceHolder.ResetStates();
                presenPanel2.Controls.Clear();
                presenPanel2.Visible = false;
                renderNode(workflow.GetState());
            }

            /////////////////////////////////HISTORY//////////////////////////////////////////////////////////////////
            WFnode currentNode = workflow.GetState();
            UpdateHistory(currentNode, ar);
            /////////////////////////////////HISTORY//////////////////////////////////////////////////////////////////
        }

        protected void restart_Click(object sender, EventArgs e)
        {
            presenPanel3.Visible = true;
            clear.Visible = true;
            forward.Text = "Forward";
            workflow.ResetComputation();
            //nodePlaceHolder.ResetStates();
            if (save.Visible == true)
            {
                presenPanel3.Visible = true;
                clear.Visible = true;
                forward.Text = "Forward";
                forward.Visible = true;
                save.Visible = false;
                //nodePlaceHolder.ResetStates();
                presenPanel2.Controls.Clear();
                presenPanel2.Visible = false;
            }

            WFnode currentNode = workflow.GetState();

            renderNode(currentNode);
            UpdateHistory(currentNode, new ActionResult(true));
        }

        protected void clear_Click(object sender, EventArgs e)
        {
            //nodePlaceHolder.ResetStates();//Value are still in controls

            WFnode currentNode = workflow.GetState();

            renderNode(currentNode);
            UpdateHistory(currentNode, new ActionResult(true));

        }

        protected void Page_Init(object sender, EventArgs e)
        {

            //presenPanel.Controls.Add(nodePlaceHolder);
            //presenPanel.Controls.Add();
            if (!Page.IsPostBack)
            {
                Trace.Warn("Begin retrieving env variabiles from db");
                Page.Controls.Add(new LiteralControl("<script>headerDivRed('none');</script>"));

                String crId = Request.Params.Get("CompilationRequestID");
                String wfId = Request.Params.Get("WorkflowID");
                String user = Request.Params.Get("Username");
                String serv = Request.Params.Get("Service");
                String tokMail = Request.Params.Get("Token");
                Trace.Warn("End retrieving env variabiles from db");

                if (crId != null)
                {
                    if (!crId.Equals("-1"))
                    {//Abbiamo tutto 

                        loadWf(wfId, crId, user, serv, tokMail);
                    }
                    else
                    {//Manca qualcosa crID ==-1
                        if (Session["LoginContactUserID"] == null)
                        {//E' la prima volta che viene richieta la pagina con crId == -1
                            int serviceid = -1;
                            try
                            {
                                serviceid = int.Parse(serv);
                            }
                            catch (Exception e2)
                            {
                                serviceid = -1;
                            }
                            if (serviceid == -1)
                            {//Va fatto scegliere il servizio
                                contentAll.Style.Add("background", "#A7A7A7 url('../lib/image/lock2.png') no-repeat center");
                                contentAll.Style.Add("-moz-opacity", "0.35");
                                contentAll.Visible = true;
                                List<Security.Service> listaServizi = Security.ExternalService.List();
                                string temp = "";
                                foreach (Security.Service s in listaServizi)
                                {
                                    temp = temp + s.ServiceName + "|";
                                }


                                Page.Controls.Add(new LiteralControl("<script>setType(-1, '" + temp + "');</script>"));

                            }
                            //Si presume che serv sia valido o scelto dall'utente
                            Session["LoginContact"] = true;
                            Session["LoginContactServiceID"] = serviceid;
                            Session["LoginContactReturnURL"] = "http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + Request.Url.AbsolutePath + Request.Url.Query;
                            if (serviceid != -1)
                                Response.Redirect("http://" + Security.EnvironmentManagement.getEnvValue("webServerAddress") + "/Auth/login.aspx");
                        }
                        else
                        {//Vuol dire che Session["LoginContactUserID"] != null, e' la seconda volta e ho tutte le info
                            // Si ma le hai nella sessione!
                            serv = "" + (int)Session["LoginContactServiceID"];
                            user = (string)Session["LoginContactUserID"];
                            // Pulisco la sessione, nn mi servono piu'
                            Session["LoginContactServiceID"] = null;
                            Session["LoginContactUserID"] = null;
                            loadWf(wfId, crId, user, serv, tokMail);
                        }
                    }
                }
                else
                {
                    workflow = (IComputableWorkflow)Session["wf"];
                    WFnode currentNode = workflow.GetState();

                    renderNode(currentNode);
                    UpdateHistory(currentNode, new ActionResult(true));
                }
            }
            else
            {
                Page.Controls.Add(new LiteralControl("<script>headerDivRed('filling');</script>"));

                workflow = ((IComputableWorkflow)Session["wf"]);
                if (workflow != null)
                {
                    if (!workflow.IsEndComputationState)
                        renderNode(workflow.GetState());

                    Security.Theme theme = ((Security.ComputableWorkflowReference)Session["wfr"]).GetTheme();
                    Page.Header.Controls.Add(new LiteralControl(@"<style type='text/css'>" + theme.CSS + "</style" + ">"));
                    if (theme.Title == "")
                    {
                        formTitle.InnerText = "NO TITLE";

                        formTitle.Style.Add("font-family", "Verdana");
                        formTitle.Style.Add("font-size", "30px");
                    }
                    else
                    {
                        formTitle.InnerText = theme.Title;

                    }
                }


            }

        }

        private void loadWf(string wfId, string crId, string user, string serv, string tokMail)
        {
            Trace.Warn("Begin load workflow");
            Security.ComputableWorkflowReference cref = Security.Token.GetWorkflow(wfId, crId, user, serv, tokMail);
            if (cref == null)
            {
                contentAll.Style.Add("background", "#A7A7A7 url('../lib/image/lock2.png') no-repeat center");
                contentAll.Style.Add("-moz-opacity", "0.35");
                contentAll.Visible = true;
                Page.Controls.Add(new LiteralControl("<script>setType(-2);</script>"));
                Session["wf"] = null;
            }
            else
            {
                Session["wfr"] = cref;
                workflow = cref.GetWorkflow();
                Session["wf"] = workflow;
                //si fa la prima visualizzazione
                renderNode(workflow.GetState());
                #region caricamento del tema 2° versione pro

                Security.Theme theme = ((Security.ComputableWorkflowReference)Session["wfr"]).GetTheme();
                Page.Header.Controls.Add(new LiteralControl(@"<style type='text/css'>" + theme.CSS + "</style" + ">"));
                if (theme.Title == "")
                {
                    formTitle.InnerText = "Testo di prova";
                }
                else
                {
                    formTitle.InnerText = theme.Title;
                    formTitle.Style.Add("font-family", "Verdana");
                    formTitle.Style.Add("font-size", "30px");
                }
                #endregion
            }
            Trace.Warn("End load workflow");
        }

        public void RaiseCallbackEvent(string eventArgument)
        {

            this.fromClient = eventArgument;//(IComputableWorkflow)Session["wf"];

            Security.ComputableWorkflowReference cref = (Security.ComputableWorkflowReference)Session["wfr"];
            if (cref != null)
            {
                cref.SaveFilling();
            }
        }

        public string GetCallbackResult()
        {
            return this.fromClient;
        }


        /////////////////////////////////HISTORY//////////////////////////////////////////////////////////////////
        protected void l_Click(object sender, EventArgs e)
        {

            int idClicked = Convert.ToInt32(((WebControl)sender).ID);
            IEnumerable<WFnode> nodel = workflow.getNodeList();
            WFnode current = workflow.GetState();


            if (current == null)//workflow.IsEndComputationState
            {
                presenPanel3.Visible = true;
                clear.Visible = true;
                forward.Text = "Forward";
                forward.Visible = true;
                save.Visible = false;
                presenPanel2.Visible = false;
                //current = nodel.ToList()[nodel.Count() - 1];
            }


            try
            {
                if (((ImageButton)presenPanel3.Controls[idClicked]).ToolTip != workflow.GetState().Name)
                {
                    string currentN;
                    do
                    {
                        workflow.ComputeNewStatus(WFeventType.ROLLBACK, null, ValidationEventHandler);
                        currentN = workflow.GetState().Name;
                    } while (((ImageButton)presenPanel3.Controls[idClicked]).ToolTip != currentN);
                }
            }
            catch (NullReferenceException actualNodeNull)
            {

                if (workflow.IsEndComputationState)
                {
                    string currentN;
                    do
                    {
                        workflow.ComputeNewStatus(WFeventType.ROLLBACK, null, ValidationEventHandler);
                        currentN = workflow.GetState().Name;
                    } while (((ImageButton)presenPanel3.Controls[idClicked]).ToolTip != currentN);
                }
            }
            nodePlaceHolder.ResetStates();
            nodePlaceHolder.Controls.Clear();


            WFnode currentNode = workflow.GetState();
            //presenPanel.Attributes.Add("style", "position:relative;left:0px;width:100%;display:block;");
            renderNode(currentNode);

            UpdateHistory(currentNode, new ActionResult(true));
        }

        //Metodo barbaro per calcolare i nodi coinvolti nel percorso anche in caso di biforcazioni
        private List<string> BackTrace(IComputableWorkflow WF, string checkPointNode)
        {
            List<string> backNodes = new List<string>();
            while (!WF.IsInitialState())//backtrace dal nodo corrente
            {
                backNodes.Add(WF.GetState().Name);
                WF.ComputeNewStatus(WFeventType.ROLLBACK, null, null);

                //nodePlaceHolder.ResetStates();
                presenPanel2.Controls.Clear();
                presenPanel2.Visible = false;
                //renderNode(workflow.GetState());

                //nodePlaceHolder.ResetStates();
                //nodePlaceHolder.Controls.Clear();
                presenPanel.Controls.Clear();
                renderNode(workflow.GetState());


            }
            backNodes.Add(WF.GetState().Name);

            WFnode curr = WF.GetState();
            while (curr.Name != checkPointNode)//riporta il workflow allo stato corrente
            {
                WFnode actual = workflow.GetState();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(actual.GetValue(nodePlaceHolder.Controls[0], actual.GetRenderingDocument().DocumentElement.FirstChild));
                ActionResult ar = workflow.ComputeNewStatus(WFeventType.TRYGOON, doc, ValidationEventHandler);
                curr = WF.GetState();
                //nodePlaceHolder.ResetStates();
                //nodePlaceHolder.Controls.Clear();
                presenPanel.Controls.Clear();
                renderNode(workflow.GetState());
            }
            return backNodes;
        }

        private void UpdateHistory(WFnode currentNode, ActionResult ar)
        {
            IEnumerable<WFnode> nodel = workflow.getNodeList();
            //List<string> BackNodes = BackTrace(workflow, currentNode.Name);
            List<string> BackNodes = workflow.GetThroughPath();
            List<string> nodeNames = new List<string>();
            foreach (WFnode n in nodel)
            {
                nodeNames.Add(n.Name);
            }

            List<string> ns = new List<string>();
            ns = workflow.GetThroughPath();

            if (ns.Count > 0 && ar.OK)
            {
                string initNode = "";
                int initNodeIndex = 0;

                for (int i = 0; i < nodeNames.Count; i++)
                {
                    if (nodeNames[i] == ns[ns.Count - 1])
                    {
                        initNode = nodeNames[i];
                        initNodeIndex = i;
                    }
                }
                string tmp = nodeNames[0];

                nodeNames.RemoveAt(0);
                nodeNames.Insert(0, initNode);
                nodeNames.RemoveAt(initNodeIndex);
                nodeNames.Insert(initNodeIndex, tmp);

                int h = 1;
                for (int i = ns.Count - 2; i >= 0; i--)
                {
                    nodeNames.RemoveAt(h);
                    nodeNames.Insert(h, ns[i]);
                    h++;
                }
                if (ns.Count < nodeNames.Count)
                {
                    nodeNames.RemoveAt(ns.Count);
                    nodeNames.Insert(ns.Count, workflow.GetState().Name);
                }

                for (int i = ns.Count + 1; i < nodeNames.Count; i++)
                {
                    nodeNames[i] = "";
                }

                for (int l = 0; l < nodeNames.Count; l++)
                {
                    ((ImageButton)presenPanel3.Controls[l]).ToolTip = nodeNames[l];
                }

            }
            if (ns.Count == 0 && ar.OK)
            {
                string initNode = "";
                int initNodeIndex = 0;

                for (int i = 0; i < nodeNames.Count; i++)
                {
                    if (nodeNames[i] == workflow.GetState().Name)
                    {
                        initNode = nodeNames[i];
                        initNodeIndex = i;
                    }
                }
                string tmp = nodeNames[0];

                nodeNames.RemoveAt(0);
                nodeNames.Insert(0, initNode);
                nodeNames.RemoveAt(initNodeIndex);
                nodeNames.Insert(initNodeIndex, tmp);

                for (int i = ns.Count + 1; i < nodeNames.Count; i++)
                {
                    nodeNames[i] = "";
                }

                for (int l = 0; l < nodeNames.Count; l++)
                {
                    ((ImageButton)presenPanel3.Controls[l]).ToolTip = nodeNames[l];
                }
            }



            foreach (Control c in presenPanel3.Controls)
            {
                c.Visible = true;
            }
            if (workflow.IsFinalState())
            {
                foreach (Control c in presenPanel3.Controls)
                {
                    if (((ImageButton)c).ToolTip == "")
                        c.Visible = false;
                }
            }

            int k = 0;
            foreach (string n in nodeNames)
            {
                if (BackNodes.Contains(((ImageButton)presenPanel3.Controls[k]).ToolTip))
                {
                    ((ImageButton)presenPanel3.Controls[k]).Enabled = true;
                    ((ImageButton)presenPanel3.Controls[k]).ImageUrl = "../lib/image/backNode.PNG";
                    if (k == 0)
                        ((ImageButton)presenPanel3.Controls[k]).Attributes.Add("style", "margin-left:5px;margin-bottom:8px;");
                    else
                        ((ImageButton)presenPanel3.Controls[k]).Attributes.Add("style", "margin-bottom:8px;");

                }
                else
                {
                    ((ImageButton)presenPanel3.Controls[k]).Enabled = false;
                    ((ImageButton)presenPanel3.Controls[k]).ImageUrl = "../lib/image/forwardNode.PNG";
                    ((ImageButton)presenPanel3.Controls[k]).Attributes.Add("style", "margin-bottom:8px;");

                }

                if (((ImageButton)presenPanel3.Controls[k]).ToolTip == currentNode.Name)
                {
                    ((ImageButton)presenPanel3.Controls[k]).ImageUrl = "../lib/image/currentNode.PNG";
                    ((ImageButton)presenPanel3.Controls[k]).Enabled = true;
                    if (k == 0)
                        ((ImageButton)presenPanel3.Controls[k]).Attributes.Add("style", "margin-left:5px;margin-bottom:0px;");
                    else
                        ((ImageButton)presenPanel3.Controls[k]).Attributes.Add("style", "margin-left:5px;margin-bottom:0px;");
                }

                k++;
            }

        }
        /////////////////////////////////HISTORY//////////////////////////////////////////////////////////////////




    }


}
