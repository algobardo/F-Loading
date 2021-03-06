﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.CompactFramework.Design.Data, Version 2.0.50727.3053.
// 
namespace Mobile.floading {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="RemoteAccessPointSoap", Namespace="http://tempuri.org/")]
    public partial class RemoteAccessPoint : System.Web.Services.Protocols.SoapHttpClientProtocol {

        /// <remarks/>
        public RemoteAccessPoint(String host, int port)
        {/*
            XmlSettings settings = new XmlSettings(@"\Program Files\Mobile\" + Mobile.Properties.Resources.FormDataSetFolder + @"\config.xml");
            String host = null;
            String port = null;
            settings.ReadSettings();
            host = settings.Hostname;
            port = settings.Port;*/

            this.Url = "http://" + host + ":" + port + "/RemoteAccessPoint.asmx";
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetWorkflow", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public WorkflowInformations GetWorkflow(int compilationRequestId, string username, string service, string token) {
            object[] results = this.Invoke("GetWorkflow", new object[] {
                        compilationRequestId,
                        username,
                        service,
                        token});
            return ((WorkflowInformations)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetWorkflow(int compilationRequestId, string username, string service, string token, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetWorkflow", new object[] {
                        compilationRequestId,
                        username,
                        service,
                        token}, callback, asyncState);
        }
        
        /// <remarks/>
        public WorkflowInformations EndGetWorkflow(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((WorkflowInformations)(results[0]));
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/SendFilledDocument", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool SendFilledDocument(int compilationRequestId, string username, string service, string token, string dataStr) {
            object[] results = this.Invoke("SendFilledDocument", new object[] {
                        compilationRequestId,
                        username,
                        service,
                        token,
                        dataStr});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginSendFilledDocument(int compilationRequestId, string username, string service, string token, string dataStr, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("SendFilledDocument", new object[] {
                        compilationRequestId,
                        username,
                        service,
                        token,
                        dataStr}, callback, asyncState);
        }
        
        /// <remarks/>
        public bool EndSendFilledDocument(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((bool)(results[0]));
        }
    }
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class WorkflowInformations {
        
        private ResultStatus statusField;
        
        private WorkflowDescription descriptionField;
        
        private string publicationDescriptionField;
        
        /// <remarks/>
        public ResultStatus status {
            get {
                return this.statusField;
            }
            set {
                this.statusField = value;
            }
        }
        
        /// <remarks/>
        public WorkflowDescription description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
        
        /// <remarks/>
        public string publicationDescription {
            get {
                return this.publicationDescriptionField;
            }
            set {
                this.publicationDescriptionField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public enum ResultStatus {
        
        /// <remarks/>
        OK,
        
        /// <remarks/>
        WRONG_COMPILATION_REQUEST_ID,
        
        /// <remarks/>
        WRONG_USERNAME_OR_SERVICE,
        
        /// <remarks/>
        WRONG_TOKEN,
        
        /// <remarks/>
        ALREADY_COMPILED,
    }
    
    /// <remarks/>
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public partial class WorkflowDescription {
        
        private string nodesSchemaField;
        
        private string extendedTypesSchemaField;
        
        private string edgesDescriptionField;
        
        private string renderingDocumentField;
        
        /// <remarks/>
        public string NodesSchema {
            get {
                return this.nodesSchemaField;
            }
            set {
                this.nodesSchemaField = value;
            }
        }
        
        /// <remarks/>
        public string ExtendedTypesSchema {
            get {
                return this.extendedTypesSchemaField;
            }
            set {
                this.extendedTypesSchemaField = value;
            }
        }
        
        /// <remarks/>
        public string EdgesDescription {
            get {
                return this.edgesDescriptionField;
            }
            set {
                this.edgesDescriptionField = value;
            }
        }
        
        /// <remarks/>
        public string RenderingDocument {
            get {
                return this.renderingDocumentField;
            }
            set {
                this.renderingDocumentField = value;
            }
        }
    }
}
