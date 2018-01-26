using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Security;

namespace LoginServices
{
    /// <summary>
    /// This class is used to retrieve some informations on a Live account.
    /// The informations are in XML format and then is used an XMLDocument.
    /// Contains methods that visit the XMLDocument and search the informations.
    /// To return the informations that have been found is used an object of an 
    /// inner class UserLiveContacts.
    /// </summary>
    public class LiveContactsParser
    {
        /// <summary>
        /// The username of the user.
        /// </summary>
        private String _username = null;
        public String Username { get { return _username; } }

        /// <summary>
        /// The list of User's Friends
        /// </summary>
        private List<Contact> _contacts = new List<Contact>();
        public List<Contact> Contacts { get { return _contacts; } }

        private static Security.Service _service;
        private static string serviceName = "Live";

        public LiveContactsParser()
        {
            List<Security.Service> extServices = Security.ExternalService.List();
            foreach (Security.Service service in extServices)
            {
                if (service.ServiceName.Equals(serviceName))
                    _service = service;
            }
        }

        /// <summary>
        /// This Method is used to parse an XML string or a file which 
        /// contains XML Code or an XML Stream.
        /// The informations are the name of Account's Owner, the name and the 
        /// mail of Owner's friends.
        /// </summary>
        /// <param name="XML"> This Object rapresents an XML strings or a name of a file that contains XML code or an XML Stream. </param>
        public void parse(Object XML)
        {
            XmlDocument doc = new XmlDocument();
            if (XML is String)
            {
                try
                {
                    doc.LoadXml((String)XML);
                }
                catch (Exception)
                {
                    try
                    {
                        doc.Load((String)XML);
                    }
                    catch (Exception)
                    {
                        doc = null;
                    }
                }
            }
            else if (XML is StreamReader)
            {
                try
                {
                    doc.Load((StreamReader)XML);
                }
                catch (Exception)
                {
                    doc = null;
                }
            }
            else doc = null;

            if (doc == null) return;
            recursiveParse(doc.DocumentElement);
        }

        /// <summary>
        /// Recursively search the informations.
        /// </summary>
        /// <param name="node"> The current XMLNode. </param>
        private void recursiveParse(XmlNode node)
        {
            if (node == null) return;
            if (node.NodeType != XmlNodeType.Element) return;

            if (((XmlElement)node).Name == "Owner") retriveOwner(node);
            else if (((XmlElement)node).Name == "Contact") retriveContact(node);
            else foreach (XmlNode n in node.ChildNodes) recursiveParse(n);
        }

        /// <summary>
        /// Retrives the name of owner from the XMLNode passed as parameter.
        /// </summary>
        /// <param name="node"> The XMLNode that contains the Owner's name. </param>
        private void retriveOwner(XmlNode node)
        {
            if (node == null) return;
            if (node.NodeType == XmlNodeType.Element)
            {
                XmlNodeList nodes = ((XmlElement)node).GetElementsByTagName("WindowsLiveID");
                if (nodes.Count == 0)
                {
                    nodes = ((XmlElement)node).GetElementsByTagName("DisplayName");
                    if (nodes.Count == 0) return;
                    else _username = nodes[0].FirstChild.Value;
                }
                else _username = nodes[0].FirstChild.Value;
            }
            else return;
        }

        /// <summary>
        /// Retrive the contact's informations from the XMLNode passed as parameter.
        /// </summary>
        /// <param name="node"> The XMLNode that contains the contact's informations. </param>
        /// <param name="tempname"> A temporary String that contains the name of contact. </param>
        /// <param name="tempmail"> A temporary String that contains the mail of contact. </param>
        private void retriveContact(XmlNode node)
        {
            if (node == null) return;
            if (node.NodeType == XmlNodeType.Element)
            {
                String tempname = "";
                XmlNodeList nodes = ((XmlElement)node).GetElementsByTagName("NickName");
                if (nodes.Count == 0)
                {
                    nodes = ((XmlElement)node).GetElementsByTagName("DisplayName");
                    if (nodes.Count == 0)
                    {
                        nodes = ((XmlElement)node).GetElementsByTagName("UniqueName");
                        if (nodes.Count == 0)
                        {
                            nodes = ((XmlElement)node).GetElementsByTagName("SortName");
                            if (nodes.Count == 0) return;
                            else tempname = nodes[0].FirstChild.Value;
                        }
                        else tempname = nodes[0].FirstChild.Value;
                    }
                    else tempname = nodes[0].FirstChild.Value;
                }
                else tempname = nodes[0].FirstChild.Value;

                String tempmail = "";
                nodes = ((XmlElement)node).GetElementsByTagName("WindowsLiveID");
                if (nodes.Count == 0)
                {
                    nodes = ((XmlElement)node).GetElementsByTagName("Address");
                    if (nodes.Count == 0) return;
                    else tempmail = nodes[0].FirstChild.Value;
                }
                else tempmail = nodes[0].FirstChild.Value;

                _contacts.Add(new Contact(tempname, tempmail, _service));
            }
            else return;
        }
    }
}