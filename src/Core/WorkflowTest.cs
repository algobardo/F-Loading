//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using Core.WF;
//using System.Xml.Schema;
//using System.Xml;
//using System.IO;
//using System.Diagnostics;
//using System.Runtime.Serialization.Formatters.Binary;
//using Fields;
//using System.Xml.XPath;

//namespace Core
//{    
//    [TestFixture]
//    public class WorkflowTest
//    {
//        public static void ValidationEventHandler(Object sender, ValidationEventArgs e)
//        {
//            Assert.Ignore(e.ToString());
//        }

//        [Test]
//        public void testBrowserSync()
//        {
//            string nd = TestResources.nodeRenderingExample;

//            var nodo = BrowserWithServerSync.CreateServerNode(nd);

//            string[] scset = Utils.WriteSchemaSet(nodo.GetNodeSchemas());

//            foreach (string s in scset)
//            {
//                Debug.WriteLine(s);
//            }

//            Debug.WriteLine(nodo.GetRenderingDocument().OuterXml);
//        }


//        [Test]
//        //test if the precondition edge is corrected "test fail" = "precondition are not correct"
//        public void eagertest()
//        {

//            XmlSchema nd1SchemaType = XmlSchema.Read(new StringReader(TestResources.Node1Types), ValidationEventHandler);
//            XmlSchema nd1Schema = XmlSchema.Read(new StringReader(TestResources.Node1), ValidationEventHandler);
//            XmlSchemaSet schemaSet1 = new XmlSchemaSet();
//            schemaSet1.Add(nd1Schema);
//            schemaSet1.Add(nd1SchemaType);
//            schemaSet1.Add(FieldsManager.FieldTypesXSD);

//            XmlDocument edgexml = new XmlDocument();
//            edgexml.InnerXml = TestResources.EdgePreconditions;
//            schemaSet1.Compile();
//            bool res = EdgeLabelInterpreter.VerifyPreconditionsCorrectness(schemaSet1, "Node1", edgexml.FirstChild);
//            Assert.IsTrue(res);

            
//            edgexml.InnerXml = "<PRECONDITIONS></PRECONDITIONS>";
//            schemaSet1.Compile();
//            res = EdgeLabelInterpreter.VerifyPreconditionsCorrectness(schemaSet1, "Node1", edgexml.FirstChild);
//            Assert.IsTrue(res);

//        }



       

//        [Test]
//        /*
//         *  test works only on satisfyed precondition fixme
//         *  
//            Not any edges precondition satisfied
//            Node1
//            Node2
//            TestCase 'Core.WorkflowTest.CompleteWorkflowTest' failed: 
//            Expected: <Node2>
//            But was:  <Node1>
//            E:\floading\src\Core\WorkflowTest.cs(69,0): at Core.WorkflowTest.CompleteWorkflowTest()


//         */
//        public void CompleteWorkflowTest()
//        {
//            XmlSchema nd1SchemaType = XmlSchema.Read(new StringReader(TestResources.Node1Types), ValidationEventHandler);
//            XmlSchema nd2SchemaType = XmlSchema.Read(new StringReader(TestResources.Node2Types), ValidationEventHandler);
//            XmlSchema nd1Schema = XmlSchema.Read(new StringReader(TestResources.Node1), ValidationEventHandler);
//            XmlSchema nd2Schema = XmlSchema.Read(new StringReader(TestResources.Node2), ValidationEventHandler);
//            XmlSchemaSet schemaSet1 = new XmlSchemaSet();
//            schemaSet1.Add(nd1Schema);
//            schemaSet1.Add(nd1SchemaType);
//            XmlSchemaSet schemaSet2 = new XmlSchemaSet();
//            schemaSet2.Add(nd2Schema);
//            schemaSet2.Add(nd2SchemaType);

//            XmlDocument edgexml = new XmlDocument();
//            edgexml.InnerXml = TestResources.EdgePreconditions;

//            XmlDocument nd1filled = new XmlDocument();
//            nd1filled.InnerXml = TestResources.Node1Compiled;

//            XmlDocument nd2filled = new XmlDocument();
//            nd2filled.InnerXml = TestResources.Node2Compiled;


//            WFnode nd1 = new WFnode(schemaSet1, (XmlSchemaComplexType)nd1Schema.Items[0]);

//            //nd1filled 
//            WFnode nd2 = new WFnode(schemaSet2, (XmlSchemaComplexType)nd2Schema.Items[0]);
//            WFedgeLabel edge = new WFedgeLabel(edgexml, null);

//            Workflow wf = new Workflow("PIPPO");

//             wf.AddNode(nd1);
//             wf.AddNode(nd2);
//            wf.AddEdge(edge, nd1, nd2);

//            wf.MarkAsInitialNode(nd1);
//            wf.MarkAsFinalNode(nd2);

//            var wf2 = wf.Save();

//            Assert.AreEqual(wf2.GetState(), nd1);

            
//            wf2.ComputeNewStatus(WFeventType.TRYGOON, nd1filled, null);

//            Debug.WriteLine(wf2.GetState());
//            Debug.WriteLine(nd2);

//            Assert.AreEqual(nd2, wf2.GetState());


//            /*Getting final document*/
//            Debug.WriteLine(wf2.GetCollectedDocument().InnerXml);

//            /*Mobile documents*/
//            XmlDocument alledges = wf2.GetEdges();
//            XmlSchemaSet sset = wf2.GetNodesSchemas();

//            //Debug.WriteLine(alledges.InnerXml);

//            StringBuilder sb = new StringBuilder();
//            foreach (XmlSchema sch in sset.Schemas())
//            {
//                sch.Write(XmlWriter.Create(sb));
//                sb.AppendLine();
//                sb.AppendLine();
//            }

//            //Debug.WriteLine(sb);
//            wf2.ComputeNewStatus(WFeventType.TRYGOON, nd2filled, null);

//            wf2.GetCollectedDocument();
//            wf2.GetCollectedDocumentSchemas();
            
//        }
    
//        [Test]
//        public void InvalidWorkflowTest()
//        {
//            XmlSchema nd1SchemaType = XmlSchema.Read(new StringReader(TestResources.Node1Types),ValidationEventHandler);
//            XmlSchema nd2SchemaType = XmlSchema.Read(new StringReader(TestResources.Node2Types),ValidationEventHandler);
//            XmlSchema nd1Schema = XmlSchema.Read(new StringReader(TestResources.Node1),ValidationEventHandler);
//            XmlSchema nd2Schema = XmlSchema.Read(new StringReader(TestResources.Node2),ValidationEventHandler);
//            XmlSchemaSet schemaSet1 = new XmlSchemaSet ();
//            schemaSet1.Add(nd1Schema);
//            schemaSet1.Add(nd1SchemaType);
//            XmlSchemaSet schemaSet2 = new XmlSchemaSet();
//            schemaSet2.Add(nd2Schema);
//            schemaSet2.Add(nd2SchemaType);

//            XmlDocument edgexml = new XmlDocument();
//            edgexml.InnerXml = TestResources.EdgePreconditions;

//            XmlDocument nd1filled = new XmlDocument();
//            nd1filled.InnerXml = TestResources.NotValidNode1;
                                          
//            WFnode nd1 = new WFnode(schemaSet1 , (XmlSchemaComplexType)nd1Schema.Items[0]);

//            //nd1filled 
//            WFnode nd2 = new WFnode(schemaSet2 , (XmlSchemaComplexType)nd2Schema.Items[0]);
//            WFedgeLabel edge = new WFedgeLabel(edgexml,null);

//            Workflow wf = new Workflow("PIPPO");

//             wf.AddNode(nd1);
//             wf.AddNode(nd2);
//             wf.AddEdge(edge, nd1, nd2);

//            wf.MarkAsInitialNode(nd1);
//            wf.MarkAsFinalNode(nd2);

//            var wf2 = wf.Save();

//            Assert.AreEqual(wf2.GetState(),nd1);                  

//            /*TODO: inserire xml al costruttore di WFNodeInstance*/
//            nd1.Value=nd1filled;
//            WFnode nd1Instance = nd1;
//            nd1Instance.Validate(null);

//            /*controllo la validazione del nodeInstance*/
//            Assert.IsFalse(nd1Instance.Validate(null));
//        }
//        [Test]
//        public void FieldsTest()
//        {
//            XmlSchema nd1SchemaType = XmlSchema.Read(new StringReader(TestResources.Node1Types), ValidationEventHandler);
//            XmlSchema nd2SchemaType = XmlSchema.Read(new StringReader(TestResources.Node2Types), ValidationEventHandler);
//            XmlSchema nd1Schema = XmlSchema.Read(new StringReader(TestResources.Node1), ValidationEventHandler);
//            XmlSchema nd2Schema = XmlSchema.Read(new StringReader(TestResources.Node2), ValidationEventHandler);
//            XmlSchemaSet schemaSet1 = new XmlSchemaSet();
//            schemaSet1.Add(nd1Schema);
//            schemaSet1.Add(nd1SchemaType);
//            XmlSchemaSet schemaSet2 = new XmlSchemaSet();
            
//            schemaSet2.Add(nd2Schema);
//            schemaSet2.Add(nd2SchemaType);

//            XmlDocument edgexml = new XmlDocument();
//            edgexml.InnerXml = TestResources.EdgePreconditions;

//            XmlDocument nd1filled = new XmlDocument();
//            nd1filled.InnerXml = TestResources.Node1Compiled;

//            WFnode nd1 = new WFnode(schemaSet1, (XmlSchemaComplexType)nd1Schema.Items[0]);

//            //nd1filled 
//            WFnode nd2 = new WFnode(schemaSet2, (XmlSchemaComplexType)nd2Schema.Items[0]);
//            WFedgeLabel edge = new WFedgeLabel(edgexml, null);

//            Workflow wf = new Workflow("PIPPO");

//            wf.AddNode(nd1);
//             wf.AddNode(nd2);
//             wf.AddEdge(edge, nd1, nd2);

//            wf.MarkAsInitialNode(nd1);
//            wf.MarkAsFinalNode(nd2);

//            var wf2 = wf.Save();

//            wf2.ComputeNewStatus(WFeventType.TRYGOON, nd1filled, null);

//            var nestate = wf2.GetState();
//            var scs = nestate.GetNodeSchemas();
//            var rs = (XmlSchemaComplexType)scs.GlobalTypes[new XmlQualifiedName(nestate.NodeTypeName)];
//            var cons =
//                  ((XmlSchemaElement)
					  
							
//                                ((XmlSchemaSequence)
//                                    rs.Particle)
							
					
//                     .Items[0]);
//            var tipo = FieldsManager.GetInstance(scs,cons);

//            Assert.IsInstanceOfType(typeof(IField), tipo);

//        }
//        [Test]
//        public void InterpreterInstantiationTest(){
//            XmlSchema nd1SchemaType = XmlSchema.Read(new StringReader(TestResources.Node1Types), ValidationEventHandler);
//            XmlSchema nd2SchemaType = XmlSchema.Read(new StringReader(TestResources.Node2Types), ValidationEventHandler);
//            XmlSchema nd1Schema = XmlSchema.Read(new StringReader(TestResources.Node1), ValidationEventHandler);
//            XmlSchema nd2Schema = XmlSchema.Read(new StringReader(TestResources.Node2), ValidationEventHandler);
//            XmlSchemaSet schemaSet1 = new XmlSchemaSet();
//            schemaSet1.Add(nd1Schema);
//            schemaSet1.Add(nd1SchemaType);
//            XmlSchemaSet schemaSet2 = new XmlSchemaSet();

//            schemaSet2.Add(nd2Schema);
//            schemaSet2.Add(nd2SchemaType);

//            XmlDocument edgexml = new XmlDocument();
//            edgexml.InnerXml = TestResources.EdgePreconditions;

//            XmlDocument nd1filled = new XmlDocument();
//            nd1filled.InnerXml = TestResources.Node1Compiled;

//            WFnode nd1 = new WFnode(schemaSet1, (XmlSchemaComplexType)nd1Schema.Items[0]);

//            //nd1filled 
//            WFnode nd2 = new WFnode(schemaSet2, (XmlSchemaComplexType)nd2Schema.Items[0]);
//            WFedgeLabel edge = new WFedgeLabel(edgexml, null);

//            nd1.Value=nd1filled.DocumentElement;
//            WFnode wfni = nd1;
//            wfni.Validate(null);

//            var typo = FieldsManager.GetType(wfni.Value.FirstChild);

//        }
        

//        [Test]
//        public void SameSchemaDifferentName()
//        {
//            XmlSchema nd1SchemaType = XmlSchema.Read(new StringReader(TestResources.Node1Types), ValidationEventHandler);

//            XmlSchema nd1Schema = XmlSchema.Read(new StringReader(TestResources.Node1), ValidationEventHandler);

//            XmlSchemaSet schemaSet1 = new XmlSchemaSet();
//            schemaSet1.Add(nd1Schema);
//            schemaSet1.Add(nd1SchemaType);

//            var bd = Fields.FieldsManager.FieldTypesXSD;
//            schemaSet1.Add(bd);
//            schemaSet1.Compile();

//            schemaSet1.Add(bd);
//            schemaSet1.Compile();

//            XmlSchema.Read(new StringReader(TestResources.BaseTypes), ValidationEventHandler);
//            try
//            {
//                schemaSet1.Compile();
//            }
//            catch { }

//            /*This Example demonstrate how adding 2 times the same schema instance is good..and how much is not good to add the same schema with another instance*/


//            XmlSchemaElement el = new XmlSchemaElement();
//            el.Name = "Ciao";
//            el.SchemaType = XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String);
//            XmlSchema s1 = new XmlSchema();
//            s1.Items.Add(el);
//            XmlSchema s2 = new XmlSchema();
//            s2.Items.Add(el);
//            XmlSchemaSet ss1 = new XmlSchemaSet();
//            ss1.Add(s1);
//            XmlSchemaSet ss2 = new XmlSchemaSet();
//            ss2.Add(s2);

//            try
//            {
//                ss1.Compile();
//            }
//            catch(Exception er)
//            {
//                Debug.WriteLine(er.Message);
//            }

//            try
//            {
//                ss2.Compile();
//            }
//            catch (Exception er)
//            {
//                Debug.WriteLine(er.Message);
//            }
//        }

//        [Test]
//        public void SerializationTest()
//        {
//            XmlSchema nd1SchemaType = XmlSchema.Read(new StringReader(TestResources.Node1Types), ValidationEventHandler);
//            XmlSchema nd1Schema = XmlSchema.Read(new StringReader(TestResources.Node1), ValidationEventHandler);
//            XmlSchemaSet schemaSet1 = new XmlSchemaSet();
//            schemaSet1.Add(nd1Schema);
//            schemaSet1.Add(nd1SchemaType);

//            XmlDocument edgexml = new XmlDocument();
//            edgexml.InnerXml = TestResources.EdgePreconditions;

//            XmlDocument nd1filled = new XmlDocument();
//            nd1filled.InnerXml = TestResources.Node1Compiled;

//            WFnode nd1 = new WFnode(schemaSet1, (XmlSchemaComplexType)nd1Schema.Items[0]);

//            BinaryFormatter bf = new BinaryFormatter();
//            MemoryStream ms = new MemoryStream();

//            bf.Serialize(ms, nd1);
//            //WFnode nd1Deser;
//            //nd1Deser = (WFnode )bf.Deserialize(ms);
//            //Assert.AreEqual(nd1, nd1Deser);
//        }
//        [Test]
//        public void InterpreterSchemaTest()
//        {
//            var sch = EdgeLabelInterpreter.GetSchema();
//            XmlDocument xd = new XmlDocument();
//            xd.LoadXml(TestResources.IntboxTest);
//            xd.Schemas = sch;
//            xd.Validate(null);
//        }

//        [Test]
//        public void WorkflowSerializationTest()
//        {
//            XmlSchema nd1SchemaType = XmlSchema.Read(new StringReader(TestResources.Node1Types), ValidationEventHandler);
//            XmlSchema nd2SchemaType = XmlSchema.Read(new StringReader(TestResources.Node2Types), ValidationEventHandler);
//            XmlSchema nd1Schema = XmlSchema.Read(new StringReader(TestResources.Node1), ValidationEventHandler);
//            XmlSchema nd2Schema = XmlSchema.Read(new StringReader(TestResources.Node2), ValidationEventHandler);
//            XmlSchemaSet schemaSet1 = new XmlSchemaSet();
//            schemaSet1.Add(nd1Schema);
//            schemaSet1.Add(nd1SchemaType);
//            XmlSchemaSet schemaSet2 = new XmlSchemaSet();
//            schemaSet2.Add(nd2Schema);
//            schemaSet2.Add(nd2SchemaType);

//            XmlDocument edgexml = new XmlDocument();
//            edgexml.InnerXml = TestResources.EdgePreconditions;

//            XmlDocument nd1filled = new XmlDocument();
//            nd1filled.InnerXml = TestResources.Node1Compiled;

//            WFnode nd1 = new WFnode(schemaSet1, (XmlSchemaComplexType)nd1Schema.Items[0]);

//            //nd1filled 
//            WFnode nd2 = new WFnode(schemaSet2, (XmlSchemaComplexType)nd2Schema.Items[0]);
//            WFedgeLabel edge = new WFedgeLabel(edgexml, null);

//            Workflow wf = new Workflow("PIPPO");

//            wf.AddNode(nd1);
//            wf.AddNode(nd2);
//            wf.AddEdge(edge, nd1, nd2);

//            wf.MarkAsInitialNode(nd1);
//            wf.MarkAsFinalNode(nd2);

//            var wf2 = wf.Save();

//            MemoryStream m1 = new MemoryStream();
//            MemoryStream m2 = new MemoryStream();
//            BinaryFormatter bf = new BinaryFormatter();
//            bf.Serialize(m1,wf);
//            bf.Serialize(m2,wf2);
//            m1.Seek(0, SeekOrigin.Begin);
//            m2.Seek(0, SeekOrigin.Begin);
//            var vd1 = (Workflow)bf.Deserialize(m1);
//            var vd2 = (IComputableWorkflow)bf.Deserialize(m2);


//        }

//        [Test]
//        public void SerializationTest2()
//        {
//            XmlSchema nd1SchemaType = XmlSchema.Read(new StringReader(TestResources.Node1Types), ValidationEventHandler);
//            XmlSchema nd2SchemaType = XmlSchema.Read(new StringReader(TestResources.Node2Types), ValidationEventHandler);
//            XmlSchema nd1Schema = XmlSchema.Read(new StringReader(TestResources.Node1), ValidationEventHandler);
//            XmlSchema nd2Schema = XmlSchema.Read(new StringReader(TestResources.Node2), ValidationEventHandler);
//            XmlSchemaSet schemaSet1 = new XmlSchemaSet();
//            schemaSet1.Add(nd1Schema);
//            schemaSet1.Add(nd1SchemaType);
//            XmlSchemaSet schemaSet2 = new XmlSchemaSet();
//            schemaSet2.Add(nd2Schema);
//            schemaSet2.Add(nd2SchemaType);

//            XmlDocument edgexml = new XmlDocument();
//            edgexml.InnerXml = TestResources.EdgePreconditions;

//            XmlDocument nd1filled = new XmlDocument();
//            nd1filled.InnerXml = TestResources.Node1Compiled;

//            WFnode nd1 = new WFnode(schemaSet1, (XmlSchemaComplexType)nd1Schema.Items[0]);

//            //nd1filled 
//            WFnode nd2 = new WFnode(schemaSet2, (XmlSchemaComplexType)nd2Schema.Items[0]);
//            WFedgeLabel edge = new WFedgeLabel(edgexml, null);

//            Workflow wf = new Workflow("PIPPO");

//            wf.AddNode(nd1);
//            wf.AddNode(nd2);
//            wf.AddEdge(edge, nd1, nd2);

//            wf.MarkAsInitialNode(nd1);
//            wf.MarkAsFinalNode(nd2);

//            wf.WorkflowInvalidationEvent += new EventHandler<WorkflowValidationEventArgs>(wf_WorkflowValidityStatusChanged);
//            BinaryFormatter bs = new BinaryFormatter();
//            MemoryStream m = new MemoryStream();
//            bs.Serialize(m, wf);
//            m.Seek(0, SeekOrigin.Begin);
//            var wf2 = (Workflow)bs.Deserialize(m);
          
            
//        }
//        [Test]
//        //there is a rollback, but after rollback the user return at the same state.
//        //Result: When the state return back, the node saved in the "rollback" variable.
//        // When the action TRYGOON return on the same edge used previously, the data saved in "rollback" is loaded.
//        public void RollbackTest()
//        {
//            XmlSchema nd1SchemaType = XmlSchema.Read(new StringReader(TestResources.Node1Types), ValidationEventHandler);
//            XmlSchema nd2SchemaType = XmlSchema.Read(new StringReader(TestResources.Node2Types), ValidationEventHandler);
//            XmlSchema nd1Schema = XmlSchema.Read(new StringReader(TestResources.Node1), ValidationEventHandler);
//            XmlSchema nd2Schema = XmlSchema.Read(new StringReader(TestResources.Node2), ValidationEventHandler);
//            XmlSchemaSet schemaSet1 = new XmlSchemaSet();
//            schemaSet1.Add(nd1Schema);
//            schemaSet1.Add(nd1SchemaType);
//            XmlSchemaSet schemaSet2 = new XmlSchemaSet();
//            schemaSet2.Add(nd2Schema);
//            schemaSet2.Add(nd2SchemaType);

//            XmlDocument edgexml = new XmlDocument();
//            edgexml.InnerXml = TestResources.EdgePreconditions;

//            XmlDocument nd1filled = new XmlDocument();
//            nd1filled.InnerXml = TestResources.Node1Compiled;

//            XmlDocument nd2filled = new XmlDocument();
//            nd2filled.InnerXml = TestResources.Node2Compiled;

//            WFnode nd1 = new WFnode(schemaSet1, (XmlSchemaComplexType)nd1Schema.Items[0]);

//            //nd1filled 
//            WFnode nd2 = new WFnode(schemaSet2, (XmlSchemaComplexType)nd2Schema.Items[0]);
//            WFedgeLabel edge = new WFedgeLabel(edgexml, null);

//            Workflow wf = new Workflow("PIPPO");

//            wf.AddNode(nd1);
//            wf.AddNode(nd2);
//            wf.AddEdge(edge, nd1, nd2);

//            wf.MarkAsInitialNode(nd1);
//            wf.MarkAsFinalNode(nd2);

//            var wf2 = wf.Save();

//            Assert.AreEqual(wf2.GetState(), nd1);

            
//            wf2.ComputeNewStatus(WFeventType.TRYGOON, nd1filled, null);
//            wf2.ComputeNewStatus(WFeventType.ROLLBACK, nd2filled, null);
//            wf2.ComputeNewStatus(WFeventType.TRYGOON, nd1filled, null);
//        }

        

//        [Test]
//        //there is a rollback; after it the user choose the new branch of workflow.
//        //Result: When the state return back, the node saved in the "rollback" variable.
//        // When the action TRYGOON choose new edge, the "rollback" variable is cleaned and the state compute in new status.
        
//        public void RollbackTest2()
//        {
//            XmlSchema nd1SchemaType = XmlSchema.Read(new StringReader(TestResources.Node1Types), ValidationEventHandler);
//            XmlSchema nd2SchemaType = XmlSchema.Read(new StringReader(TestResources.Node2Types), ValidationEventHandler);
//            XmlSchema nd1Schema = XmlSchema.Read(new StringReader(TestResources.Node1), ValidationEventHandler);
//            XmlSchema nd2Schema = XmlSchema.Read(new StringReader(TestResources.Node2), ValidationEventHandler);
//            XmlSchema nd3Schema = XmlSchema.Read(new StringReader(TestResources.Node3), ValidationEventHandler);
//            XmlSchemaSet schemaSet1 = new XmlSchemaSet();
//            schemaSet1.Add(nd1Schema);
//            schemaSet1.Add(nd1SchemaType);
//            XmlSchemaSet schemaSet2 = new XmlSchemaSet();
//            schemaSet2.Add(nd2Schema);
//            schemaSet2.Add(nd2SchemaType);
//            XmlSchemaSet schemaSet3 = new XmlSchemaSet();
//            schemaSet3.Add(nd3Schema);
//            schemaSet3.Add(nd2SchemaType);

//            XmlDocument edgexml = new XmlDocument();
//            edgexml.InnerXml = TestResources.EdgePreconditions;

//            XmlDocument nd1filled = new XmlDocument();
//            nd1filled.InnerXml = TestResources.Node1Compiled;

//            XmlDocument nd2filled = new XmlDocument();
//            nd2filled.InnerXml = TestResources.Node2Compiled;

//            XmlDocument nd3filled = new XmlDocument();
//            nd3filled.InnerXml = TestResources.Node3Compiled;

//            WFnode nd1 = new WFnode(schemaSet1, (XmlSchemaComplexType)nd1Schema.Items[0]);

//            //nd1filled 
//            WFnode nd2 = new WFnode(schemaSet2, (XmlSchemaComplexType)nd2Schema.Items[0]);
//            WFnode nd3 = new WFnode(schemaSet3, (XmlSchemaComplexType)nd3Schema.Items[0]);

//            WFedgeLabel edge = new WFedgeLabel(edgexml, null);
//            WFedgeLabel edge1 = new WFedgeLabel(edgexml, null);

//            Workflow wf = new Workflow("PIPPO");

//            wf.AddNode(nd1);
//            wf.AddNode(nd2);
//            wf.AddNode(nd3);
//            wf.AddEdge(edge, nd1, nd2);
//            wf.AddEdge(edge1, nd1, nd3);

//            wf.MarkAsInitialNode(nd1);
//            wf.MarkAsFinalNode(nd3);

//            var wf2 = wf.Save();

//            Assert.AreEqual(wf2.GetState(), nd1);
//            wf2.ComputeNewStatus(WFeventType.TRYGOON, nd1filled, null);
//            wf2.ComputeNewStatus(WFeventType.ROLLBACK, nd2filled, null);
//            wf2.ComputeNewStatus(WFeventType.TRYGOON, nd3filled, null);
//        }


//        [Test]
//        //there is a rollback; after it the user choose the new branch of workflow.
//        //Result: When the state return back, the node saved in the "rollback" variable.
//        // When the action TRYGOON choose new edge, the "rollback" variable is cleaned and the state compute in new status.
//        // Node {a,b,c,d,e,f}
//        // Edge {ab,ae,ac,be,cc,cd,db,df,ed,ef}
//        public void WorkflowTest1()
//        {
//            XmlSchema nd1SchemaType = XmlSchema.Read(new StringReader(TestResources.Node1Types), ValidationEventHandler);
//            XmlSchema nd2SchemaType = XmlSchema.Read(new StringReader(TestResources.Node2Types), ValidationEventHandler);
//            XmlSchema nd1Schema = XmlSchema.Read(new StringReader(TestResources.Node1), ValidationEventHandler);
//            XmlSchema nd2Schema = XmlSchema.Read(new StringReader(TestResources.Node2), ValidationEventHandler);
//            XmlSchema nd3Schema = XmlSchema.Read(new StringReader(TestResources.Node3), ValidationEventHandler);
//            XmlSchema nd4Schema = XmlSchema.Read(new StringReader(TestResources.Node4), ValidationEventHandler);
//            XmlSchema nd5Schema = XmlSchema.Read(new StringReader(TestResources.Node5), ValidationEventHandler);
//            XmlSchema nd6Schema = XmlSchema.Read(new StringReader(TestResources.Node6), ValidationEventHandler);

//            XmlSchemaSet schemaSet1 = new XmlSchemaSet();
//            schemaSet1.Add(nd1Schema);
//            schemaSet1.Add(nd1SchemaType);
//            XmlSchemaSet schemaSet2 = new XmlSchemaSet();
//            schemaSet2.Add(nd2Schema);
//            schemaSet2.Add(nd2SchemaType);
//            XmlSchemaSet schemaSet3 = new XmlSchemaSet();
//            schemaSet3.Add(nd3Schema);
//            schemaSet3.Add(nd2SchemaType);
//            XmlSchemaSet schemaSet4 = new XmlSchemaSet();
//            schemaSet4.Add(nd4Schema);
//            schemaSet4.Add(nd1SchemaType);
//            XmlSchemaSet schemaSet5 = new XmlSchemaSet();
//            schemaSet5.Add(nd5Schema);
//            schemaSet5.Add(nd1SchemaType);
//            XmlSchemaSet schemaSet6 = new XmlSchemaSet();
//            schemaSet6.Add(nd6Schema);
//            schemaSet6.Add(nd1SchemaType);

//            XmlDocument emptypre = new XmlDocument();
//            emptypre.InnerXml = TestResources.EdgePreconditions3;

//            XmlDocument nd1filled = new XmlDocument();
//            nd1filled.InnerXml = TestResources.Node1Compiled;
//            XmlDocument nd2filled = new XmlDocument();
//            nd2filled.InnerXml = TestResources.Node2Compiled;
//            XmlDocument nd3filled = new XmlDocument();
//            nd3filled.InnerXml = TestResources.Node3Compiled;
//            XmlDocument nd4filled = new XmlDocument();
//            nd4filled.InnerXml = TestResources.Node4Compiled;
//            XmlDocument nd5filled = new XmlDocument();
//            nd5filled.InnerXml = TestResources.Node5Compiled;
//            XmlDocument nd6filled = new XmlDocument();
//            nd6filled.InnerXml = TestResources.Node6Compiled;
            
       
//            //nd1filled 
//            WFnode a = new WFnode(schemaSet1, (XmlSchemaComplexType)nd1Schema.Items[0]);
//            WFnode b = new WFnode(schemaSet2, (XmlSchemaComplexType)nd2Schema.Items[0]);
//            WFnode c = new WFnode(schemaSet3, (XmlSchemaComplexType)nd3Schema.Items[0]);
//            WFnode d = new WFnode(schemaSet4, (XmlSchemaComplexType)nd4Schema.Items[0]);
//            WFnode e = new WFnode(schemaSet5, (XmlSchemaComplexType)nd5Schema.Items[0]);
//            WFnode f = new WFnode(schemaSet6, (XmlSchemaComplexType)nd6Schema.Items[0]);


//            Workflow wf = new Workflow("PIPPO");
//            WFedgeLabel edge = new WFedgeLabel(emptypre, null);
//            WFedgeLabel edge1 = new WFedgeLabel(emptypre, null);
//            WFedgeLabel edge2 = new WFedgeLabel(emptypre, null);
//            WFedgeLabel edge3 = new WFedgeLabel(emptypre, null);
//            WFedgeLabel edge4 = new WFedgeLabel(emptypre, null);
//            WFedgeLabel edge5 = new WFedgeLabel(emptypre, null);
//            WFedgeLabel edge6 = new WFedgeLabel(emptypre, null);
//            WFedgeLabel edge7 = new WFedgeLabel(emptypre, null);
//            WFedgeLabel edge8 = new WFedgeLabel(emptypre, null);
//            WFedgeLabel edge9 = new WFedgeLabel(emptypre, null);

//            wf.AddNode(a);
//            wf.AddNode(b);
//            wf.AddNode(c);
//            wf.AddNode(d);
//            wf.AddNode(e);
//            wf.AddNode(f);
//            wf.AddEdge(edge, a, b);
//            wf.AddEdge(edge1, a, c);
//            wf.AddEdge(edge2, a, e);
//            wf.AddEdge(edge4, c, f);
//            wf.AddEdge(edge3, b, e);
//            wf.AddEdge(edge5, c, d);
//            wf.AddEdge(edge6, d, b);            
//            wf.AddEdge(edge9, e, f);


//            var vw = wf.Save();

//            vw.ComputeNewStatus(WFeventType.TRYGOON, nd1filled, null);
//            vw.ComputeNewStatus(WFeventType.TRYGOON, nd3filled, null);
//            vw.ComputeNewStatus(WFeventType.TRYGOON, nd4filled, null);
//            vw.ComputeNewStatus(WFeventType.TRYGOON, nd2filled, null);
//            vw.ComputeNewStatus(WFeventType.TRYGOON, nd5filled, null);
//             Assert.IsTrue(vw.IsFinalState());
//            vw.ComputeNewStatus(WFeventType.TRYGOON, nd6filled, null);

//            var dc = vw.GetCollectedDocument();
//            var sc = vw.GetCollectedDocumentSchemas();

//            dc.Schemas = sc;
//            dc.Validate(null);



//        }

//        void wf_WorkflowValidityStatusChanged(object sender, WorkflowValidationEventArgs e)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}

