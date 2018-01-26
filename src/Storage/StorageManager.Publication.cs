using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Web;

namespace Storage
{
    public partial class StorageManager
    {
        /// <summary>
        /// Save a publication
        /// </summary>
        /// <param name="modelID">Model ID of the publication</param>
        /// <param name="date">Date of publication</param>
        /// <param name="expDate">Expiration Date</param>
        /// <param name="urlBase">Url of publication</param>
        /// <param name="workflow">Workflow of the pubblication</param>
        /// <param name="anonymResult">If the result is aonymous</param>
        /// <param name="isPublic">If everyone can fill the pubblication</param>
        /// <param name="isResultReplicated">If one person can compile the same pubblication more then one time</param>
        /// <param name="serviceCreditID">The service of the credential</param>
        /// <param name="URIUpload">URI for uploading (sending result out of Floading)</param>
        /// <param name="urlRSS">URL for RSS feed (sending result out of Floading)</param>
        /// <returns>The pubblication just created</returns>
        public Publication publish(int modelID, System.DateTime date, System.DateTime expDate, 
            string urlBase, Object workflow, bool anonymResult, bool isPublic, bool isResultReplicated,
            int externalServiceID, string URIUpload, string urlRSS, string descr)
        {
            Publication p = new Publication();

            var model = from m in context.Model
                        where m.modelID == modelID
                        select m;
            Model m1 = model.First<Model>();
            m1.isPublicated = true;

            p.namePublication = m1.nameModel;
            p.xml = object2ByteArray(workflow);            
            p.urlBase = urlBase;
            p.date = date;
            p.userID = m1.userID;
            p.themeID = m1.themeID;
            p.expirationDate = expDate;
            p.anonymResult = anonymResult;
            p.isPublic = isPublic;
            p.isResultReplicated = isResultReplicated;
            p.externalServiceID = externalServiceID;
            p.URIUpload = URIUpload;
            p.urlRSS = urlRSS;
            p.description = descr;
            
            return AddEntity(p);
        }

        # region Publications management

 
        public SortedList<int, String> getPublicationsExpired(int elapsedDays)
        {
            return new SortedList<int, String>(context.Publication.Where(p => p.expirationDate > DateTime.Now.AddDays(elapsedDays)).OrderBy(p=>p.publicationID).ToDictionary(p=>p.publicationID,p=>p.namePublication));
        }



        public List<PartialPublication> getPublicationsByUserID2(int userID)
        {
                //SELECT DATA
                var pub = from pb in context.Publication
                          where pb.userID == userID
                          orderby pb.date
                          select new
                          {
                              pb.publicationID,
                              pb.namePublication,
                              pb.userID,
                              pb.date,
                              pb.expirationDate,
                              pb.anonymResult,
                              pb.isPublic,
                              pb.isResultReplicated,
                              pb.externalServiceID,
                              pb.description
                          };

                //PREPARE LIST
                List<PartialPublication> list = new List<PartialPublication>(pub.Count());
                foreach (var t in pub)
                {
                    PartialPublication ppb = new PartialPublication();
                    ppb.publicationID = t.publicationID;
                    ppb.namePublication = t.namePublication;
                    ppb.userID = t.userID;
                    ppb.date = t.date;
                    ppb.expirationDate = t.expirationDate;
                    ppb.anonymResult = t.anonymResult;
                    ppb.isPublic = t.isPublic;
                    ppb.isResultReplicated = t.isResultReplicated;
                    ppb.externalServiceID = (int)t.externalServiceID;
                    ppb.descr = t.description;
                    list.Add(ppb);
                }

                return list;
        }




        public SortedList<int, Pair<String, String>> getPublicationsByUserID(int userID)
        {
            try
            {
                return new SortedList<int, Pair<String, String>>(context.Publication.Where(p=> p.userID == userID).OrderBy(p => p.date).ToDictionary(p => p.publicationID, v => new Pair<String, String>() { First = v.namePublication, Second = v.description }));
            }
            catch (Exception)
            {
                return null;    //No results
            }
        }

        //public SortedList<int, Pair<String, String>> getPublicationsPublic()
        //{
        //    try
        //    {
        //        return new SortedList<int, Pair<String, String>>(context.Publication.Where(p => p.isPublic).OrderBy(p => p.publicationID).ToDictionary(p => p.publicationID, v => new Pair<String, String>() { First = v.namePublication, Second = v.description }));
        //    }
        //    catch (Exception)
        //    {
        //        return null;    //No results
        //    }
        //}


        public SortedList<int, Pair<String,String> > getPublicationsByContactID(int contactID)
        {
            try
            {
                return new SortedList<int, Pair<String,String> >( context.CompilationRequest.Where(cr => cr.contactID == contactID).Join(context.Publication, cr => cr.publicationID, p => p.publicationID, (cr, p) => p).OrderBy(p=>p.date).ToDictionary(p => p.publicationID, v => new Pair<String, String>() { First = v.namePublication, Second = v.description }));
            }
            catch (Exception)
            {
                return null;    //No results
            }
            /*
            try
            {
                var y = from cc in context.CompilationRequest
                        join pp in context.Publication on cc.publicationID equals pp.publicationID
                        where cc.contactID == contactID
                        orderby pp.date
                        select new { pp.publicationID, pp.namePublication };
                return y.ToDictionary(p => p.publicationID, p => p.namePublication);
            }
            catch (Exception)
            {
                return null;    //No results
            }
            */
        }

        public List<Publication> getListPublicationsByContactID(int contactID)
        {
            var pub = from cr in context.CompilationRequest
                    join pb in context.Publication on cr.publicationID equals pb.publicationID
                    where cr.contactID == contactID
                    orderby pb.date
                    select pb;
            return pub.ToList<Publication>();
        }

        public List<PartialPublication> getPublicationsPublic()
        {
            var pub = from pb in context.Publication
                      where pb.isPublic
                      orderby pb.date
                      select new
                      {
                          pb.publicationID,
                          pb.namePublication,
                          pb.userID,
                          pb.date,
                          pb.expirationDate,
                          pb.anonymResult,
                          pb.isPublic,
                          pb.isResultReplicated,
                          pb.externalServiceID,
                          pb.description
                      };

            List<PartialPublication> list = new List<PartialPublication>(pub.Count());
            foreach (var t in pub)
            {
                PartialPublication ppb = new PartialPublication();
                ppb.publicationID = t.publicationID;
                ppb.namePublication = t.namePublication;
                ppb.userID = t.userID;
                ppb.date = t.date;
                ppb.expirationDate = t.expirationDate;
                ppb.anonymResult = t.anonymResult;
                ppb.isPublic = t.isPublic;
                ppb.isResultReplicated = t.isResultReplicated;
                ppb.externalServiceID = (int)t.externalServiceID;
                ppb.descr = t.description;
                list.Add(ppb);
            }

            return list;
        }


        
        public List<Pair<PartialPublication, Storage.CompilationRequest>> getPublicationsCompilationRequestByContactID(int contactID)
        {
            var tuple = from cr in context.CompilationRequest
                      join pb in context.Publication
                      on cr.publicationID equals pb.publicationID
                      where cr.contactID == contactID
                      orderby pb.date
                      select new { 
                          pb.publicationID,
                          pb.namePublication,
                          pb.userID,
                          pb.date,
                          pb.expirationDate,
                          pb.anonymResult,
                          pb.isPublic,
                          pb.isResultReplicated,
                          pb.externalServiceID,
                          pb.description,
                          cr
                      };

            List<Pair<PartialPublication, Storage.CompilationRequest>> list = new List<Pair<PartialPublication, CompilationRequest>>(tuple.Count());
            foreach (var t in tuple)
            {
                Pair<PartialPublication, Storage.CompilationRequest> p = new Pair<PartialPublication, CompilationRequest>();
                PartialPublication ppb = new PartialPublication();
                ppb.publicationID = t.publicationID;
                ppb.namePublication = t.namePublication;
                ppb.userID = t.userID;
                ppb.date = t.date;
                ppb.expirationDate = t.expirationDate;
                ppb.anonymResult = t.anonymResult;
                ppb.isPublic = t.isPublic;
                ppb.isResultReplicated = t.isResultReplicated;
                ppb.externalServiceID = (int)t.externalServiceID;
                ppb.descr = t.description;
                p.First = ppb;
                p.Second = t.cr;

                list.Add(p);
            }
            return list;
                
        }

        public Object getWorkflowByPublication(Publication pb)
        {
            return byteArray2Object(pb.xml.ToArray());
        }

        #endregion


        # region Service-Contact management

        

        #endregion

    }
}
