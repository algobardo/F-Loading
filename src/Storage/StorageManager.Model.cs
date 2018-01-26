using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;


namespace Storage
{
    public partial class StorageManager
    {

        # region Model management

        /// <summary>
        /// Create a new (editable) Model
        /// </summary>
        /// <param name="userID">Model owner or creator</param>
        /// <param name="themeID">Model theme</param>
        /// <param name="modelName">Name or Title of the model</param>
        /// <param name="xmlDoc">Workflow object</param>
        /// <param name="description">Short description of the model (max 256 chars)</param>
        /// <returns>The newly created model or null when DB errors occour</returns>
        public Model addModel(int userID, int themeID, String modelName, Object workflowDoc, String description)
        {
            Model m = new Model()
            {
                nameModel = modelName,
                userID = userID,
                themeID = themeID,
                xml = object2ByteArray(workflowDoc),
                description=description
            };

            return AddEntity(m);
        }

        public Object getWorkflowByModel(Model md)
        {
            return byteArray2Object(md.xml.ToArray());
        }


        public List<Tris<int, String, String>> getModelsByUserID(int userID, bool descending)
        {
            return descending ? context.Model.Where(m => m.userID == userID).OrderByDescending(m => m.modelID).Select(m => new Tris<int, String, String>() { First = m.modelID, Second = m.nameModel, Third = m.description }).ToList() : context.Model.Where(m => m.userID == userID).OrderBy(m => m.modelID).Select(m => new Tris<int, String, String>() { First = m.modelID, Second = m.nameModel, Third = m.description }).ToList() ;
        }

        public SortedList<int, Pair<String,String>> getModelsByUserID(int userID)
        {
            try
            {
                return new SortedList<int, Pair<String,String>> (context.Model.Where(m => m.userID == userID).OrderBy(m=>m.modelID).ToDictionary(m => m.modelID, m => new Pair<String,String>(){First=m.nameModel,Second=m.description}));
            }
            catch (Exception)
            {
                var x = new SortedList<int,string>(8);
                var s =  x.Reverse();
                return null;    //No results
            }
        }

        public Dictionary<int, String> getOrdModelsByUserID(int userID)
        {
            try
            {
                var model = from m in context.Model
                            where m.userID == userID
                            orderby m.modelID
                            select m;
                return model.ToDictionary(m => m.modelID, m => m.nameModel);
            }
            catch (Exception)
            {
                return null;    
            }
        }
        #endregion      
    }
}
