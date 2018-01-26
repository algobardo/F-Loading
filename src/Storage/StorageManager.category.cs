using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Storage
{
    public partial class StorageManager
    {
        
        /// <summary>
        /// Add new category.
        /// </summary>
        /// <param name="name">Category name</param>
        /// <returns>Category reference</returns>
        public Category addCategory(string name)
        {
            Category c = new Category()
            {
                nameCategory = name
            };
            return AddEntity(c);
        }
    }

   
}
