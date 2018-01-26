using System;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.IO;
using System.Net;
using System.Timers;

namespace Communication
{
    class FtpService
    {
        
        public void send(String serverFtp, String folder, String username, String password, String pathFile)
        {

            //ex pathFile = ftp://nome_ftp//eventual_folder//filename.extension
            try
            {
                String completePathFileOnFtpServer = "ftp://" + serverFtp + "//" + folder + "//Report Floading - " + DateTime.Now.ToString("f") + ".txt";
                
                //create connession with ftp server
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(completePathFileOnFtpServer);
                ftpRequest.Credentials = new NetworkCredential(username, password);
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
               
                //create an array for upload in ftp server
                System.IO.FileStream fr = new System.IO.FileStream(pathFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                byte[] fileContents = new byte[fr.Length];
                fr.Read(fileContents, 0, Convert.ToInt32(fr.Length));
                fr.Close();
                
                //get ftp stream
                Stream writerFtp = ftpRequest.GetRequestStream();
                writerFtp.Write(fileContents, 0, fileContents.Length);
                writerFtp.Close();
                
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                System.Console.ReadLine();
            }

        }

        public static void Main()
        {
            FtpService ftpS = new FtpService();
            ftpS.send("ftp..com", ".com", "@aruba.it", "", "C:\\Users\\gianpy\\Desktop\\specifiche.txt");
        }
    }
}
