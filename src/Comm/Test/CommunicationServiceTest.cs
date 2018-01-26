using Comm;
using System;
using Storage;
using System.Xml.Linq;
using System.Data.Linq;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace TestSuite
{
    
    
    /// <summary>
    ///Classe di test per CommunicationServiceTest.
    ///Creata per contenere tutti gli unit test CommunicationServiceTest
    ///</summary>
    [TestFixture]
    public class CommunicationServiceTest
    {

        private CommunicationService serviceManager;
        private StorageManager storageManager;

        /// <summary>
        /// costruttore di default
        /// </summary>
        public CommunicationServiceTest()
        {
            serviceManager = CommunicationService.Instance;
            storageManager = new StorageManager();   
        }

        /// <summary>
        ///Test per send
        ///</summary>
        [Test]
        public void SendTest()
        {
            Publication pub = storageManager.getEntityByID<Publication>(11);
            Assert.IsNotNull(pub);

            UriParser.Register(new GoogleDocsUriParser(), "gdocs", -1);
            
            Assert.IsTrue(serviceManager.Send(pub));
        }

        /// <summary>
        /// Get services test
        /// </summary>
        [Test]
        public void GetServicesTest()
        {
            List<string> listServices = serviceManager.GetServices();
            Assert.IsNotNull(listServices);
            foreach (string s in listServices)
                Console.WriteLine(s);
        }

        /// <summary>
        /// GetMaxFieldCount method test
        /// </summary>
        [Test]
        public void GetMaxFieldsCountTest()
        {
            Assert.IsTrue(serviceManager.GetMaxInputFieldsCount() > 0);
        }

        /// <summary>
        /// GetInputFields method test
        /// </summary>
        [Test]
        public void GetInputFieldsTest()
        {
            List<string> listServices = serviceManager.GetServices();
            Assert.IsNotNull(listServices);
            foreach (string s in listServices)
            {
                Console.WriteLine("Service: "  + s);
                Dictionary<string, string> fields = serviceManager.GetInputFieldsForService(s);
                Assert.IsNotNull(fields);
                foreach (string key in fields.Keys) 
                    Console.WriteLine("Input Field " + key + " adhere to regexp " + fields[key]);
            }
        }

        /// <summary>
        /// BuildUri Method test
        /// </summary>
        [Test]
        public void BuildUriTest()
        {
            List<string> listServices = serviceManager.GetServices();
            Assert.IsNotNull(listServices);
            foreach (string s in listServices)
            {
                Console.WriteLine("Service: " + s);
                Dictionary<string, string> fields = serviceManager.GetInputFieldsForService(s);
                Dictionary<string, string> data = new Dictionary<string, string>();
                Assert.IsNotNull(fields);
                foreach (string key in fields.Keys)
                {
                    Console.WriteLine("Input Field " + key + " adhere to regexp " + fields[key]);
                    data.Add(key, "valid_uri");
                }

                Assert.IsNotNull(serviceManager.BuildUri(s, data));
            }
        }

        /// <summary>
        /// GetRegexpValidator method test
        /// </summary>
        [Test]
        public void GetRegexpValidatorTest()
        {
            List<string> listServices = serviceManager.GetServices();
            Assert.IsNotNull(listServices);
            foreach (string s in listServices)
            {
                string regexp = serviceManager.GetRegexpValidator(s);
                Console.WriteLine("Service: " + s + " uri regexp: " + regexp);
            }
        }

        //test per communicationResultTest
        [Test]
        public void CommunicationGetResultTest()
        {
            int publicationId = 606;
            //List<XmlDocument> listResult = Comm.PublicationResult.getResults(publicationId);                       
        }
    }
}
