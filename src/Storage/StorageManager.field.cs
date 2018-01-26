using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storage
{
    public partial class StorageManager
    {




        #region Field management

        /// <summary>
        /// Add a field
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="categoryID"></param>
        /// <param name="name"></param>
        /// <param name="xml"></param>
        /// <returns>Field reference or null if errors occur</returns>
        public Field addField(int userID, int categoryID, string name, System.Xml.Linq.XElement xml)
        {
            Field f = new Field()
            {
                userID = userID,
                categoryID = categoryID,
                nameField = name,
                xmlField = xml,
                dateCreation = DateTime.Now
            };

            return AddEntity(f);
        }

        /// <summary>
        /// Add a field to a category.
        /// </summary>
        /// <param name="fieldID"></param>
        /// <param name="categoryID"></param>
        /// <returns></returns>
        public Boolean addFieldToCategoryByID(int fieldID, int categoryID)
        {
          /*
            var f_tmp = from f in context.Field
                        where f.fieldID == fieldID
                        select f;
            Field field = f_tmp.First<Field>();
            field.categoryID = categoryID;
           */
            getEntityByID<Field>(fieldID).categoryID = categoryID; // TODO: gestire not found
            return commit();
        }

        /// <summary>
        /// Remove a field from a category by ID. Category will be null.
        /// </summary>
        /// <param name="fieldID"></param>
        /// <returns></returns>
        public Boolean removeFieldToCategoryByID(int fieldID)
        {
            /*var f_tmp = from f in context.Field
                        where f.fieldID == fieldID
                        select f;
            Field field = f_tmp.First<Field>();
            field.categoryID = null;
            */
            getEntityByID<Field>(fieldID).categoryID = null; // TODO: gestire not found
            return commit();
        }
        
        /// <summary>
        /// Change the category to a field.
        /// </summary>
        /// <param name="fieldID"></param>
        /// <param name="newcategoryID"></param>
        /// <returns></returns>
        public Boolean changeFieldToCategoryByID(int fieldID, int newcategoryID)
        {
            /*
            var f_tmp = from f in context.Field
                        where f.fieldID == fieldID
                        select f;
            Field field = f_tmp.First<Field>();
            field.categoryID = newcategoryID;
             */
            getEntityByID<Field>(fieldID).categoryID = newcategoryID; // TODO: gestire not found
            return commit();
        }
        #endregion


    }


}
