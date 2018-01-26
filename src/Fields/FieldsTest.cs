using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

#if false
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Web.UI.WebControls;

namespace Fields
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class FieldsTest
    {

        private List<IField> fieldList;
        
        public FieldsTest()
        {
            
            
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        public static void ValidationEventHandler(Object sender, ValidationEventArgs e)
        {
            
        }


        [TestMethod]
        public void FieldsManagerTest()
        {
            fieldList = new List<IField>();

            #region Reflection Test
            /*
             * Reflection test
             */
            FieldsManager.LoadTypes();
            List<Type> lt=FieldsManager.FieldTypes;
            Assert.IsTrue(lt.Count>0);

            

            List<MethodInfo> lm1=FieldsManager.GetConstraints(lt[0]);
            List<MethodInfo> lm2 = FieldsManager.GetConstraints(lt[1]);
            Assert.IsNotNull(lm1);
            Assert.IsNotNull(lm2);

            Console.WriteLine("--Report1 (reflection test)--");
            Console.WriteLine("Types found:");
            foreach (Type t in lt){Console.WriteLine(t.Name);}
            Console.WriteLine("Constraints for Field: " + lt[0].Name);
            foreach (MethodInfo m in lm1) { Console.WriteLine(m.Name); }
            Console.WriteLine("Constraints for Field: " + lt[1].Name);
            foreach (MethodInfo m in lm2) { Console.WriteLine(m.Name); }

            #endregion

            #region Instance Creation Test 1

            fieldList.Add(FieldsManager.GetInstance(lt[0]));
            fieldList.Add(FieldsManager.GetInstance(lt[0]));
            fieldList.Add(FieldsManager.GetInstance(lt[0]));
            fieldList.Add(FieldsManager.GetInstance(lt[0]));

            fieldList.Add(FieldsManager.GetInstance(lt[1]));
            fieldList.Add(FieldsManager.GetInstance(lt[1]));
            fieldList.Add(FieldsManager.GetInstance(lt[1]));
            fieldList.Add(FieldsManager.GetInstance(lt[1]));
 
            foreach (IField f in fieldList)
                Assert.IsNotNull(f);
            for (int i = 0; i < fieldList.Count; i++)
            {
                fieldList[i].TypeName = "type" + i;
            }

            Console.WriteLine("Fields schema");
            XmlSchema xsch=new XmlSchema();
            XmlWriterSettings xset=new XmlWriterSettings();
            xset.ConformanceLevel =ConformanceLevel.Auto;
            XmlWriter xwr=XmlWriter.Create(Console.Out,xset);
            foreach(IField f in fieldList)
                xsch.Items.Add(f.TypeSchema);
            
            xsch.Write(xwr);


            

            

            #endregion

            #region Instance Manipulation
            
            
            //Add Constraint to IntBox
           /* if (f1 is IntBox)
            {
                Assert.IsTrue(((IntBox)f1).AddGreaterThanConstraint(1));
            }
            Console.WriteLine("IntBox with Constraint");
            XmlSchemaComplexType xsd3 = f1.TypeSchema;
            xsch = new XmlSchema();
            xsch.Items.Add(xsd3);
            //prints new schema to output
            xsch.Write(xwr);
            */
            XmlSchemaSet xsdset = new XmlSchemaSet();
            XmlSchema xsd4 = XmlSchema.Read(new StringReader(TestResources.BaseTypes), ValidationEventHandler);
            xsdset.Add(xsd4);
            int count1 = 0, count2 = 0;
            foreach (IField f in fieldList)
            {

                xsch = new XmlSchema();
                if (f is IntBox)
                {
                    switch (count1)
                    {
                        case 0: 
                            {
                                Assert.IsTrue(((IntBox)f).AddGreaterThanConstraint(1));
                            } break;
                        case 1: 
                            {
                                Assert.IsTrue(((IntBox)f).AddIntervalConstraint(3, 100));
                            } break;
                        case 2: 
                            { 
                                Assert.IsTrue(((IntBox)f).AddLowerThanConstraint(9000));
                            } break;
                        case 3: 
                            {
                                Assert.IsTrue(((IntBox)f).AddLowerThanConstraint(0));
                                Assert.IsTrue(((IntBox)f).AddGreaterThanConstraint(-1000));
//                                Assert.IsFalse(((IntBox)f).AddLowerThanConstraint(0));
//                                Assert.IsFalse(((IntBox)f).AddLowerThanConstraint(0));
//                                Assert.IsFalse(((IntBox)f).AddGreaterThanConstraint(-1000));
                            } break;
                    }
                    count1++;
                    
                }
               if (f is StringBox)
                {
                    switch (count2)
                    {
                        case 0: 
                            {
                                Assert.IsTrue(((StringBox)f).AddMinLengthConstraint(2));
                                Assert.IsTrue(((StringBox)f).AddMaxLengthConstraint(10));
                            } break;
                        case 1: 
                            { 
                                Assert.IsTrue(((StringBox)f).AddPatternConstraint("[0-9]"));
                                Assert.IsTrue(((StringBox)f).AddMaxLengthConstraint(1));
                            } break;
                        case 2: 
                            {
                                Assert.IsTrue(((StringBox)f).AddMaxLengthConstraint(2));
 //                               Assert.IsFalse(((StringBox)f).AddMaxLengthConstraint(2));
 //                               Assert.IsFalse(((StringBox)f).AddMinLengthConstraint(-2));
                                Assert.IsTrue(((StringBox)f).AddMinLengthConstraint(1));
                            } break;
                        case 3: { } break;
                    }
                    count2++;
                }
                XmlSchemaComplexType xsd3 = f.TypeSchema;
                xsch.Items.Add(xsd3);
                xsdset.Add(xsch);
            }
            XmlSchema compiledSchema = null;
            xsdset.Compile();

            foreach (XmlSchema sch in xsdset.Schemas())
            {
                compiledSchema = sch;
                compiledSchema.Write(Console.Out);
                Console.Out.WriteLine();
            }
            
            #endregion 

            #region Instance Retrieving

            XmlSchema xsd5 = XmlSchema.Read(new StringReader(TestResources.BaseTypes), ValidationEventHandler);
            XmlSchema xsd6 = XmlSchema.Read(new StringReader(TestResources.Field1), ValidationEventHandler);
            XmlSchema xsd7 = XmlSchema.Read(new StringReader(TestResources.Field2), ValidationEventHandler);
            XmlSchema xsd8 = XmlSchema.Read(new StringReader(TestResources.Elem1), ValidationEventHandler);


            XmlSchemaSet xset2 = new XmlSchemaSet();
            xset2.Add(xsd5);
            xset2.Add(xsd6);
            xset2.Add(xsd7);
            xset2.Add(xsd8);

            xset2.Compile();

            List<IField> lf = new List<IField>();
            foreach (XmlSchemaElement elem in ((XmlSchemaSequence)((XmlSchemaComplexType)xsd8.Items[0]).ContentTypeParticle).Items)
            {
                Assert.IsNotNull(elem);
                IField felem = FieldsManager.GetInstance(xset2, elem);
                Assert.IsNotNull(felem);
                lf.Add(felem);
            }
            
            Assert.IsTrue(lf.Count > 0);
            
            //XmlSchemaElement elem1 =((XmlSchemaElement)((XmlSchemaSequence)compl1.ContentTypeParticle).Items[0]);
            //XmlSchemaElement elem2 = ((XmlSchemaElement)((XmlSchemaSequence)compl2.ContentTypeParticle).Items[0]);

            Console.Out.WriteLine("------Fields Retrieved-------");

            List<WebControl> wl = new List<WebControl>();
            foreach (IField l in lf)
            {
                Assert.IsNotNull(l.Name);
                Console.Out.WriteLine("Field name: "+l.Name);
                WebControl w=l.GetWebControl();
                Assert.IsNotNull(w);
                List<BaseValidator> vl = l.GetValidators();
                Assert.IsNotNull(vl);
                Assert.IsTrue(vl.Count > 0);
                XmlSchema xs=new XmlSchema();
                xs.Items.Add(l.TypeSchema);
                xs.Write(Console.Out);
                foreach (BaseValidator v in vl)
                    Console.Out.WriteLine("Validator:"+v.GetType().Name);
            }


            //WebControl w=l.GetWebControl();
            //    Assert.IsNotNull(w);
            //    w.
            
            
            

            #endregion

        }

        
        
    }
}

#endif