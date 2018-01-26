using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;

namespace TestSuite
{
    /// <summary>
    /// StorageTest class for testing storage method
    /// </summary>
    [TestClass]
    public class StorageTest
    {
        private StorageManager storageManager;

        public StorageTest()
        {
            //
            // TODO: aggiungere qui la logica del costruttore
            //
            storageManager = new StorageManager();    
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Ottiene o imposta il contesto del test che fornisce
        ///le informazioni e le funzionalità per l'esecuzione del test corrente.
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

        #region Attributi di test aggiuntivi
        //
        // È possibile utilizzare i seguenti attributi aggiuntivi per la scrittura dei test:
        //
        // Utilizzare ClassInitialize per eseguire il codice prima di eseguire il primo test della classe
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Utilizzare ClassCleanup per eseguire il codice dopo l'esecuzione di tutti i test della classe
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Utilizzare TestInitialize per eseguire il codice prima di eseguire ciascun test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Utilizzare TestCleanup per eseguire il codice dopo l'esecuzione di ciascun test
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        /// <summary>
        /// Test for the method moveFromToGroup
        /// </summary>
        [TestMethod]
        public void moveFromToGroupTest()
        {
            string g1 = "old", g2 = "new";
            List<string> listg = new List<string>();
            listg.Add(g1);
            listg.Add(g2);
            List<Group> gr = storageManager.addGroups(listg, 1);
            
            List<Contact> listold = new List<Contact>();
            listold.Add(storageManager.getEntityByID<Contact>(2));
            listold.Add(storageManager.getEntityByID<Contact>(3));
            listold.Add(storageManager.getEntityByID<Contact>(4));
            storageManager.addContactsToGroup(listold, gr[0]);

            List<Contact> listnew = new List<Contact>();
            listnew.Add(storageManager.getEntityByID<Contact>(4));
            listnew.Add(storageManager.getEntityByID<Contact>(5));
            listnew.Add(storageManager.getEntityByID<Contact>(6));
            storageManager.addContactsToGroup(listnew, gr[1]);

            List<Contact> movelist = new List<Contact>();
            movelist.Add(listold.ElementAt(1));
            movelist.Add(listold.ElementAt(2));
            //storageManager.moveFromToGroup(1, movelist, g1, g2);            
        }


        [TestMethod]
        public void publishTest()
        {
            //storageManager.publish(1030, DateTime.Today, DateTime.Today, "cwlih", null, true, true, true, 1, "nlvune", "cniufhdbre");
            storageManager.removeEntity<User>(11);
        }


        [TestMethod]
        public void getResultsByPublicationIDTest()
        {            
            XElement xe = XElement.Parse("<catalog><book id=\"bk101\"><author>Gambardella, Matthew</author><title>XML Developer's Guide</title><genre>Computer</genre>text1</book><!--Comment1--><book id=\"bk102\"><!--Comment2--><author>Ralls, Kim</author><title>Midnight Rain</title><genre>Fantasy</genre></book></catalog>");
            List<Result> listResultToRemove = new List<Result>();
            CompilationRequest s = storageManager.getEntityByID<CompilationRequest>(11);
            if (s != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    Result r = (Result) storageManager.addResult(11, xe);
                    listResultToRemove.Add(r);
                }
            }

            List<Result> lr = storageManager.getResultsByPublicationID(s.publicationID.Value);

            if(listResultToRemove.Count > 0)
                storageManager.removeEntity<Result>(listResultToRemove);
        }

        [TestMethod]
        public void addContactTest()
        {
            Contact c; //= storageManager.addContact("mkdsmfia91391", 1, "gavino");
            c = storageManager.addContact("eaoifjaofme", 4, "geronimo");
        }


    }
}
