using System;
using System.Collections.Generic;
using System.Text;
using Google.GData.Documents;
using Google.GData.Client;
using Google.GData.Extensions;

namespace Communication
{
    class GoogleDocsService
    {

        static public void send(String email, String password)
        {
            DocumentsService service = new DocumentsService("servizioEsempio");
            service.setUserCredentials(email, password);
            DocumentsListQuery query = new DocumentsListQuery();
            DocumentsFeed feed = service.Query(query);

            foreach (DocumentEntry entry in feed.Entries)
            {
                Console.WriteLine(entry.Title.Text);
            }
            System.Console.ReadLine();

            // DocumentEntry newEntry = service.UploadDocument("C:\\Users\\gianpy\\Desktop\\specifiche.txt", "New Document Title.txt");
            // Console.WriteLine("Now accessible at: " + newEntry.AlternateUri.ToString());

        }

        public static void tMain()
        {
            send("xxx", "xxx");
        }

    }
}
