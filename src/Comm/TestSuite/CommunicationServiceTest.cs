using Comm;
using System;
using Storage;
using System.Xml.Linq;
using System.Data.Linq;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;
using System.Text.RegularExpressions;
using Comm.Util;
using Google.GData.Documents;
using Google.GData.Client;
using Google.GData.Extensions;

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

        [Test]
        public void SetupTest()
        {
            Publication pub = storageManager.getEntityByID<Publication>(86);
            if (pub != null)
            {
               Assert.IsTrue(serviceManager.Setup(pub));
            }
        }

        /// <summary>
        ///Test per send
        ///</summary>
        [Test]
        public void SendTestEmail()
        {
            Publication pub = storageManager.getEntityByID<Publication>(86);
            Assert.IsNotNull(pub);

            pub.URIUpload = "mailto:loa2009-comunicazione@cli.di.unipi.it";

            Assert.IsTrue(serviceManager.Send(pub));
        }

        /// <summary>
        ///Test per send
        ///</summary>
        [Test]
        public void SendTestEmailPartial()
        {
            Result res = storageManager.getEntityByID<Result>(10);
            Assert.IsNotNull(res);

            res.Publication.URIUpload = "mailto:loa2009-comunicazione@cli.di.unipi.it";

            Assert.IsTrue(serviceManager.Send(res));
        }

        /// <summary>
        ///Test per send
        ///</summary>
        [Test]
        public void SendTestFtp()
        {
            Publication pub = storageManager.getEntityByID<Publication>(86);
            Assert.IsNotNull(pub);

            pub.URIUpload = "ftp://loa:loauser@10.0.2.2/Result";

            Assert.IsTrue(serviceManager.Send(pub));
        }

        /// <summary>
        ///Test per send
        ///</summary>
        [Test]
        public void SendTestFtpPartial()
        {
            Result res = storageManager.getEntityByID<Result>(10);
            Assert.IsNotNull(res);

            res.Publication.URIUpload = "ftp://loa:loauser@10.0.2.2/Result";

            Assert.IsTrue(serviceManager.Send(res));
        }

        /// <summary>
        ///Test per send
        ///</summary>
        [Test]
        public void SendTestGdocs()
        {
            if(!UriParser.IsKnownScheme("gdocs")) 
                UriParser.Register(new GoogleDocsUriParser(), "gdocs", -1);
            Publication res2 = storageManager.getEntityByID<Publication>(485);
            Assert.IsNotNull(res2);

            res2.URIUpload = "gdocs:asfalto:oh";

            Assert.IsTrue(serviceManager.Send(res2));
       
        }

        /// <summary>
        ///Test per send
        ///</summary>
        [Test]
        public void SendTestGdocsPartial()
        {
            Result res = storageManager.getEntityByID<Result>(24);
            Assert.IsNotNull(res);

            res.Publication.URIUpload = "gdocs:asfalto:oh";

            Assert.IsTrue(serviceManager.Send(res));
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
                Console.WriteLine("Service: " + s);
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

        [Test]
        public void RegexpTest()
        {
            string password = "pass4_OKw.-04d?^*";
            string directory = "";
            Regex reg = new Regex(@"^(([a-zA-Z0-9]([-\.\w;\^\$\.\|\?\*])*){2,40})$");
            Assert.IsTrue(reg.IsMatch(password));
            reg = new Regex(@"([a-zA-Z0-9\-\.\?\,\'\/\\\%\$#_]*)?$");
            Assert.IsTrue(reg.IsMatch(directory));
        }

        //test per communicationResultTest
        [Test]
        public void CommunicationGetResultTest()
        {
            int publicationId = 606;
            //List<XmlDocument> listResult = Comm.PublicationResult.getResults(publicationId);                       
        }

        //test per GoogleDoc
        [Test]
        public void GoogleDocs()
        {
            int publicationId = 606;
            //Comm.Services.GoogleDocsService = new G
            string path = "C:\\Users\\gianpiero\\Desktop\\rating.txt";



            DocumentsService myService = new DocumentsService("exampleCo-exampleApp-1");
            myService.setUserCredentials("gianpieroradano", "");
            DocumentEntry newEntry = myService.UploadDocument(path, "New Document Title.txt");
            Console.WriteLine("Now accessible at: " + newEntry.AlternateUri.ToString());

        }


        //test per GoogleDoc
        [Test]
        public void GoogleDocsInside()
        {
            int publicationId = 606;
            //Comm.Services.GoogleDocsService = new G
           
            Comm.Services.GoogleDocsService gg = new  Comm.Services.GoogleDocsService();
            List<Result> rs =storageManager.getResultsByPublicationID(318);
            //gg.Send(rs.First);


            

        }


        //test per GoogleDoc
        [Test]
        public void GoogleDocsList()
        {
            DocumentsListQuery query = new DocumentsListQuery();
            DocumentsService myService = new DocumentsService("exampleCo-exampleApp-1");
            myService.setUserCredentials("gianpieroradano", "");
            DocumentsFeed feed = myService.Query(query);
            foreach (DocumentEntry entry in feed.Entries)
            {
                Console.WriteLine(entry.Title.Text);
            }
        }
    }
}

