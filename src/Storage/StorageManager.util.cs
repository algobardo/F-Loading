using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data.Linq.Mapping;

namespace Storage
{
    public partial class StorageManager
    {
        # region Util

        #region (De)Serialization

        public Object byteArray2Object(byte[] ar)
        {
            try
            {
                return new BinaryFormatter().Deserialize(new MemoryStream(ar));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public T byteArray2Object<T>(byte[] ar) where T : class
        {
            try
            {
                return (T)new BinaryFormatter().Deserialize(new MemoryStream(ar));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public byte[] object2ByteArray(Object o)
        {
            try
            {
                MemoryStream sb0k = new MemoryStream();
                new BinaryFormatter().Serialize(sb0k, o);
                return sb0k.ToArray();
            }
            catch (Exception)
            {
                return null;
            }

        }

        public void object2ByteArray(Object o, out byte[] ar)
        {
            ar = object2ByteArray(o);
        }

        #endregion

        #region Entity Management

        public class Pair<TFirst, TSecond>
        {
            public TFirst First;
            public TSecond Second;
        }

        public class Tris<TFirst, TSecond, TThird>
        {
            public TFirst First;
            public TSecond Second;
            public TThird Third;
        }


        public class PartialPublication
        {
            public int publicationID;
            public string namePublication;
            public System.DateTime date;
            public int userID;
            public System.DateTime expirationDate;
            public bool anonymResult;
            public bool isPublic;
            public bool isResultReplicated;
            public int externalServiceID;
            public string descr;
        }

        private static Dictionary<Type, Pair<String, String>> reflCache; //reflection cache

        private List<T> AddEntity<T>(List<T> entities) where T : class
        {
            context.GetTable<T>().InsertAllOnSubmit(entities);
            if (commit())
                return entities;
            context.GetTable<T>().DeleteAllOnSubmit(entities); //Undo
            return null;
        }

        private T AddEntity<T>(T entity) where T : class
        {
            context.GetTable<T>().InsertOnSubmit(entity);
            if (commit())
                return entity;
            context.GetTable<T>().DeleteOnSubmit(entity); //Undo
            return null;
        }

        private List<T> AddEntityAndUnion<T>(List<T> toInsert, List<T> alreadyIn) where T : class
        {
            return AddEntity(toInsert) == null ? null : alreadyIn == null ? toInsert : toInsert.Union(alreadyIn).ToList();
        }

        public T getEntityByID<T>(int ID) where T : class
        {
            var pair = getEntityPK<T>();
            return context.ExecuteQuery<T>(String.Format("SELECT * FROM \"{0}\" WHERE {1} = {2}",
                pair.First, pair.Second, ID.ToString())).FirstOrDefault();
            // Ok, it sucks!
        }
        
        public Boolean removeEntity<T>(int ID) where T : class
        {
            T e = getEntityByID<T>(ID);
            return (e != null) && removeEntity<T>(e);
        }

        public Boolean removeEntity<T>(T entity) where T : class
        {
            context.GetTable<T>().DeleteOnSubmit(entity);
            return commit();
        }

        public Boolean removeEntity<T>(List<T> entities) where T : class
        {
            context.GetTable<T>().DeleteAllOnSubmit(entities);
            return commit();
        }
        

        private Pair<String, String> getEntityPK<T>() where T : class
        {
            if (reflCache == null)
                reflCache = new Dictionary<Type, Pair<String, String>>();
            if (!reflCache.ContainsKey(typeof(T)))
                reflCache.Add(typeof(T), new Pair<String, String>()
                {
                    First = typeof(T).Name,
                    Second = context.Mapping.GetTable(typeof(T)).RowType.DataMembers.FirstOrDefault(k => k.IsPrimaryKey).Name
                });
            return reflCache[typeof(T)];
        }

        #endregion




        #endregion
    }
}
