using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security
{
    /// <summary>
    /// class that models a contact of the workflow's creator
    /// </summary>
    public class Contact
    {

        private int _contactid;
        private string _name;
        private string _email;
        private Service _service;
        
        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="name">contact's name</param>
        /// <param name="email">contact's email address</param>
        /// <param name="serv">contact's reference service</param>
        public Contact(string name, string email, Service service)
        {
            if (email == null)
                throw new ArgumentNullException();
            this._contactid = -1;
            this._name = name;
            this._email = email;
            this._service = service;
        }

        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="contactid">contact's db unique id</param>
        /// <param name="name">contact's name</param>
        /// <param name="email">contact's email address</param>
        /// <param name="serv">contact's reference service</param>
        internal Contact(int contactid, string name, string email, Service serv)
        {
            if (email == null)
                throw new ArgumentNullException();
            this._contactid = contactid;
            this._name = name;
            this._email = email;
            this._service = serv;//new Service(serv.nameService, serv.serviceID);
        }

        /// <summary>
        /// return the contact's id
        /// </summary>
        internal int ContactID
        {
            get { return _contactid; }
        }

       /// <summary>
       /// return the contact's name
       /// </summary>
        public string Name{
            get { return _name; }
        }

        /// <summary>
        /// return the contact's email address
        /// </summary>
        public string Email
        {
            get { return _email; }
        }

        /// <summary>
        /// return the contact's reference service
        /// </summary>
        public Service Service
        {
            get { return _service; }
        }
    }
}
