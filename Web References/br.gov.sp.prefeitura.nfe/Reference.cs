﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.42000.
// 
#pragma warning disable 1591

namespace ATIMO.br.gov.sp.prefeitura.nfe {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="LoteNFeSoap", Namespace="http://www.prefeitura.sp.gov.br/nfe")]
    public partial class LoteNFe : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback EnvioRPSOperationCompleted;
        
        private System.Threading.SendOrPostCallback EnvioLoteRPSOperationCompleted;
        
        private System.Threading.SendOrPostCallback TesteEnvioLoteRPSOperationCompleted;
        
        private System.Threading.SendOrPostCallback CancelamentoNFeOperationCompleted;
        
        private System.Threading.SendOrPostCallback ConsultaNFeOperationCompleted;
        
        private System.Threading.SendOrPostCallback ConsultaNFeRecebidasOperationCompleted;
        
        private System.Threading.SendOrPostCallback ConsultaNFeEmitidasOperationCompleted;
        
        private System.Threading.SendOrPostCallback ConsultaLoteOperationCompleted;
        
        private System.Threading.SendOrPostCallback ConsultaInformacoesLoteOperationCompleted;
        
        private System.Threading.SendOrPostCallback ConsultaCNPJOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public LoteNFe() {
            this.Url = global::ATIMO.Properties.Settings.Default.ATIMO_br_gov_sp_prefeitura_nfe_LoteNFe;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event EnvioRPSCompletedEventHandler EnvioRPSCompleted;
        
        /// <remarks/>
        public event EnvioLoteRPSCompletedEventHandler EnvioLoteRPSCompleted;
        
        /// <remarks/>
        public event TesteEnvioLoteRPSCompletedEventHandler TesteEnvioLoteRPSCompleted;
        
        /// <remarks/>
        public event CancelamentoNFeCompletedEventHandler CancelamentoNFeCompleted;
        
        /// <remarks/>
        public event ConsultaNFeCompletedEventHandler ConsultaNFeCompleted;
        
        /// <remarks/>
        public event ConsultaNFeRecebidasCompletedEventHandler ConsultaNFeRecebidasCompleted;
        
        /// <remarks/>
        public event ConsultaNFeEmitidasCompletedEventHandler ConsultaNFeEmitidasCompleted;
        
        /// <remarks/>
        public event ConsultaLoteCompletedEventHandler ConsultaLoteCompleted;
        
        /// <remarks/>
        public event ConsultaInformacoesLoteCompletedEventHandler ConsultaInformacoesLoteCompleted;
        
        /// <remarks/>
        public event ConsultaCNPJCompletedEventHandler ConsultaCNPJCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.prefeitura.sp.gov.br/nfe/ws/envioRPS", RequestElementName="EnvioRPSRequest", RequestNamespace="http://www.prefeitura.sp.gov.br/nfe", ResponseNamespace="http://www.prefeitura.sp.gov.br/nfe", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("RetornoXML")]
        public string EnvioRPS(int VersaoSchema, string MensagemXML) {
            object[] results = this.Invoke("EnvioRPS", new object[] {
                        VersaoSchema,
                        MensagemXML});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void EnvioRPSAsync(int VersaoSchema, string MensagemXML) {
            this.EnvioRPSAsync(VersaoSchema, MensagemXML, null);
        }
        
        /// <remarks/>
        public void EnvioRPSAsync(int VersaoSchema, string MensagemXML, object userState) {
            if ((this.EnvioRPSOperationCompleted == null)) {
                this.EnvioRPSOperationCompleted = new System.Threading.SendOrPostCallback(this.OnEnvioRPSOperationCompleted);
            }
            this.InvokeAsync("EnvioRPS", new object[] {
                        VersaoSchema,
                        MensagemXML}, this.EnvioRPSOperationCompleted, userState);
        }
        
        private void OnEnvioRPSOperationCompleted(object arg) {
            if ((this.EnvioRPSCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.EnvioRPSCompleted(this, new EnvioRPSCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.prefeitura.sp.gov.br/nfe/ws/envioLoteRPS", RequestElementName="EnvioLoteRPSRequest", RequestNamespace="http://www.prefeitura.sp.gov.br/nfe", ResponseNamespace="http://www.prefeitura.sp.gov.br/nfe", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("RetornoXML")]
        public string EnvioLoteRPS(int VersaoSchema, string MensagemXML) {
            object[] results = this.Invoke("EnvioLoteRPS", new object[] {
                        VersaoSchema,
                        MensagemXML});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void EnvioLoteRPSAsync(int VersaoSchema, string MensagemXML) {
            this.EnvioLoteRPSAsync(VersaoSchema, MensagemXML, null);
        }
        
        /// <remarks/>
        public void EnvioLoteRPSAsync(int VersaoSchema, string MensagemXML, object userState) {
            if ((this.EnvioLoteRPSOperationCompleted == null)) {
                this.EnvioLoteRPSOperationCompleted = new System.Threading.SendOrPostCallback(this.OnEnvioLoteRPSOperationCompleted);
            }
            this.InvokeAsync("EnvioLoteRPS", new object[] {
                        VersaoSchema,
                        MensagemXML}, this.EnvioLoteRPSOperationCompleted, userState);
        }
        
        private void OnEnvioLoteRPSOperationCompleted(object arg) {
            if ((this.EnvioLoteRPSCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.EnvioLoteRPSCompleted(this, new EnvioLoteRPSCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.prefeitura.sp.gov.br/nfe/ws/testeenvio", RequestElementName="TesteEnvioLoteRPSRequest", RequestNamespace="http://www.prefeitura.sp.gov.br/nfe", ResponseNamespace="http://www.prefeitura.sp.gov.br/nfe", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("RetornoXML")]
        public string TesteEnvioLoteRPS(int VersaoSchema, string MensagemXML) {
            object[] results = this.Invoke("TesteEnvioLoteRPS", new object[] {
                        VersaoSchema,
                        MensagemXML});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void TesteEnvioLoteRPSAsync(int VersaoSchema, string MensagemXML) {
            this.TesteEnvioLoteRPSAsync(VersaoSchema, MensagemXML, null);
        }
        
        /// <remarks/>
        public void TesteEnvioLoteRPSAsync(int VersaoSchema, string MensagemXML, object userState) {
            if ((this.TesteEnvioLoteRPSOperationCompleted == null)) {
                this.TesteEnvioLoteRPSOperationCompleted = new System.Threading.SendOrPostCallback(this.OnTesteEnvioLoteRPSOperationCompleted);
            }
            this.InvokeAsync("TesteEnvioLoteRPS", new object[] {
                        VersaoSchema,
                        MensagemXML}, this.TesteEnvioLoteRPSOperationCompleted, userState);
        }
        
        private void OnTesteEnvioLoteRPSOperationCompleted(object arg) {
            if ((this.TesteEnvioLoteRPSCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.TesteEnvioLoteRPSCompleted(this, new TesteEnvioLoteRPSCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.prefeitura.sp.gov.br/nfe/ws/cancelamentoNFe", RequestElementName="CancelamentoNFeRequest", RequestNamespace="http://www.prefeitura.sp.gov.br/nfe", ResponseNamespace="http://www.prefeitura.sp.gov.br/nfe", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("RetornoXML")]
        public string CancelamentoNFe(int VersaoSchema, string MensagemXML) {
            object[] results = this.Invoke("CancelamentoNFe", new object[] {
                        VersaoSchema,
                        MensagemXML});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void CancelamentoNFeAsync(int VersaoSchema, string MensagemXML) {
            this.CancelamentoNFeAsync(VersaoSchema, MensagemXML, null);
        }
        
        /// <remarks/>
        public void CancelamentoNFeAsync(int VersaoSchema, string MensagemXML, object userState) {
            if ((this.CancelamentoNFeOperationCompleted == null)) {
                this.CancelamentoNFeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCancelamentoNFeOperationCompleted);
            }
            this.InvokeAsync("CancelamentoNFe", new object[] {
                        VersaoSchema,
                        MensagemXML}, this.CancelamentoNFeOperationCompleted, userState);
        }
        
        private void OnCancelamentoNFeOperationCompleted(object arg) {
            if ((this.CancelamentoNFeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CancelamentoNFeCompleted(this, new CancelamentoNFeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFe", RequestElementName="ConsultaNFeRequest", RequestNamespace="http://www.prefeitura.sp.gov.br/nfe", ResponseNamespace="http://www.prefeitura.sp.gov.br/nfe", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("RetornoXML")]
        public string ConsultaNFe(int VersaoSchema, string MensagemXML) {
            object[] results = this.Invoke("ConsultaNFe", new object[] {
                        VersaoSchema,
                        MensagemXML});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ConsultaNFeAsync(int VersaoSchema, string MensagemXML) {
            this.ConsultaNFeAsync(VersaoSchema, MensagemXML, null);
        }
        
        /// <remarks/>
        public void ConsultaNFeAsync(int VersaoSchema, string MensagemXML, object userState) {
            if ((this.ConsultaNFeOperationCompleted == null)) {
                this.ConsultaNFeOperationCompleted = new System.Threading.SendOrPostCallback(this.OnConsultaNFeOperationCompleted);
            }
            this.InvokeAsync("ConsultaNFe", new object[] {
                        VersaoSchema,
                        MensagemXML}, this.ConsultaNFeOperationCompleted, userState);
        }
        
        private void OnConsultaNFeOperationCompleted(object arg) {
            if ((this.ConsultaNFeCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ConsultaNFeCompleted(this, new ConsultaNFeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFeRecebidas", RequestElementName="ConsultaNFeRecebidasRequest", RequestNamespace="http://www.prefeitura.sp.gov.br/nfe", ResponseNamespace="http://www.prefeitura.sp.gov.br/nfe", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("RetornoXML")]
        public string ConsultaNFeRecebidas(int VersaoSchema, string MensagemXML) {
            object[] results = this.Invoke("ConsultaNFeRecebidas", new object[] {
                        VersaoSchema,
                        MensagemXML});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ConsultaNFeRecebidasAsync(int VersaoSchema, string MensagemXML) {
            this.ConsultaNFeRecebidasAsync(VersaoSchema, MensagemXML, null);
        }
        
        /// <remarks/>
        public void ConsultaNFeRecebidasAsync(int VersaoSchema, string MensagemXML, object userState) {
            if ((this.ConsultaNFeRecebidasOperationCompleted == null)) {
                this.ConsultaNFeRecebidasOperationCompleted = new System.Threading.SendOrPostCallback(this.OnConsultaNFeRecebidasOperationCompleted);
            }
            this.InvokeAsync("ConsultaNFeRecebidas", new object[] {
                        VersaoSchema,
                        MensagemXML}, this.ConsultaNFeRecebidasOperationCompleted, userState);
        }
        
        private void OnConsultaNFeRecebidasOperationCompleted(object arg) {
            if ((this.ConsultaNFeRecebidasCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ConsultaNFeRecebidasCompleted(this, new ConsultaNFeRecebidasCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaNFeEmitidas", RequestElementName="ConsultaNFeEmitidasRequest", RequestNamespace="http://www.prefeitura.sp.gov.br/nfe", ResponseNamespace="http://www.prefeitura.sp.gov.br/nfe", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("RetornoXML")]
        public string ConsultaNFeEmitidas(int VersaoSchema, string MensagemXML) {
            object[] results = this.Invoke("ConsultaNFeEmitidas", new object[] {
                        VersaoSchema,
                        MensagemXML});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ConsultaNFeEmitidasAsync(int VersaoSchema, string MensagemXML) {
            this.ConsultaNFeEmitidasAsync(VersaoSchema, MensagemXML, null);
        }
        
        /// <remarks/>
        public void ConsultaNFeEmitidasAsync(int VersaoSchema, string MensagemXML, object userState) {
            if ((this.ConsultaNFeEmitidasOperationCompleted == null)) {
                this.ConsultaNFeEmitidasOperationCompleted = new System.Threading.SendOrPostCallback(this.OnConsultaNFeEmitidasOperationCompleted);
            }
            this.InvokeAsync("ConsultaNFeEmitidas", new object[] {
                        VersaoSchema,
                        MensagemXML}, this.ConsultaNFeEmitidasOperationCompleted, userState);
        }
        
        private void OnConsultaNFeEmitidasOperationCompleted(object arg) {
            if ((this.ConsultaNFeEmitidasCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ConsultaNFeEmitidasCompleted(this, new ConsultaNFeEmitidasCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaLote", RequestElementName="ConsultaLoteRequest", RequestNamespace="http://www.prefeitura.sp.gov.br/nfe", ResponseNamespace="http://www.prefeitura.sp.gov.br/nfe", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("RetornoXML")]
        public string ConsultaLote(int VersaoSchema, string MensagemXML) {
            object[] results = this.Invoke("ConsultaLote", new object[] {
                        VersaoSchema,
                        MensagemXML});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ConsultaLoteAsync(int VersaoSchema, string MensagemXML) {
            this.ConsultaLoteAsync(VersaoSchema, MensagemXML, null);
        }
        
        /// <remarks/>
        public void ConsultaLoteAsync(int VersaoSchema, string MensagemXML, object userState) {
            if ((this.ConsultaLoteOperationCompleted == null)) {
                this.ConsultaLoteOperationCompleted = new System.Threading.SendOrPostCallback(this.OnConsultaLoteOperationCompleted);
            }
            this.InvokeAsync("ConsultaLote", new object[] {
                        VersaoSchema,
                        MensagemXML}, this.ConsultaLoteOperationCompleted, userState);
        }
        
        private void OnConsultaLoteOperationCompleted(object arg) {
            if ((this.ConsultaLoteCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ConsultaLoteCompleted(this, new ConsultaLoteCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaInformacoesLote", RequestElementName="ConsultaInformacoesLoteRequest", RequestNamespace="http://www.prefeitura.sp.gov.br/nfe", ResponseNamespace="http://www.prefeitura.sp.gov.br/nfe", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("RetornoXML")]
        public string ConsultaInformacoesLote(int VersaoSchema, string MensagemXML) {
            object[] results = this.Invoke("ConsultaInformacoesLote", new object[] {
                        VersaoSchema,
                        MensagemXML});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ConsultaInformacoesLoteAsync(int VersaoSchema, string MensagemXML) {
            this.ConsultaInformacoesLoteAsync(VersaoSchema, MensagemXML, null);
        }
        
        /// <remarks/>
        public void ConsultaInformacoesLoteAsync(int VersaoSchema, string MensagemXML, object userState) {
            if ((this.ConsultaInformacoesLoteOperationCompleted == null)) {
                this.ConsultaInformacoesLoteOperationCompleted = new System.Threading.SendOrPostCallback(this.OnConsultaInformacoesLoteOperationCompleted);
            }
            this.InvokeAsync("ConsultaInformacoesLote", new object[] {
                        VersaoSchema,
                        MensagemXML}, this.ConsultaInformacoesLoteOperationCompleted, userState);
        }
        
        private void OnConsultaInformacoesLoteOperationCompleted(object arg) {
            if ((this.ConsultaInformacoesLoteCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ConsultaInformacoesLoteCompleted(this, new ConsultaInformacoesLoteCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.prefeitura.sp.gov.br/nfe/ws/consultaCNPJ", RequestElementName="ConsultaCNPJRequest", RequestNamespace="http://www.prefeitura.sp.gov.br/nfe", ResponseNamespace="http://www.prefeitura.sp.gov.br/nfe", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("RetornoXML")]
        public string ConsultaCNPJ(int VersaoSchema, string MensagemXML) {
            object[] results = this.Invoke("ConsultaCNPJ", new object[] {
                        VersaoSchema,
                        MensagemXML});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void ConsultaCNPJAsync(int VersaoSchema, string MensagemXML) {
            this.ConsultaCNPJAsync(VersaoSchema, MensagemXML, null);
        }
        
        /// <remarks/>
        public void ConsultaCNPJAsync(int VersaoSchema, string MensagemXML, object userState) {
            if ((this.ConsultaCNPJOperationCompleted == null)) {
                this.ConsultaCNPJOperationCompleted = new System.Threading.SendOrPostCallback(this.OnConsultaCNPJOperationCompleted);
            }
            this.InvokeAsync("ConsultaCNPJ", new object[] {
                        VersaoSchema,
                        MensagemXML}, this.ConsultaCNPJOperationCompleted, userState);
        }
        
        private void OnConsultaCNPJOperationCompleted(object arg) {
            if ((this.ConsultaCNPJCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ConsultaCNPJCompleted(this, new ConsultaCNPJCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    public delegate void EnvioRPSCompletedEventHandler(object sender, EnvioRPSCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class EnvioRPSCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal EnvioRPSCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    public delegate void EnvioLoteRPSCompletedEventHandler(object sender, EnvioLoteRPSCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class EnvioLoteRPSCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal EnvioLoteRPSCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    public delegate void TesteEnvioLoteRPSCompletedEventHandler(object sender, TesteEnvioLoteRPSCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class TesteEnvioLoteRPSCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal TesteEnvioLoteRPSCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    public delegate void CancelamentoNFeCompletedEventHandler(object sender, CancelamentoNFeCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CancelamentoNFeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CancelamentoNFeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    public delegate void ConsultaNFeCompletedEventHandler(object sender, ConsultaNFeCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ConsultaNFeCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ConsultaNFeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    public delegate void ConsultaNFeRecebidasCompletedEventHandler(object sender, ConsultaNFeRecebidasCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ConsultaNFeRecebidasCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ConsultaNFeRecebidasCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    public delegate void ConsultaNFeEmitidasCompletedEventHandler(object sender, ConsultaNFeEmitidasCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ConsultaNFeEmitidasCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ConsultaNFeEmitidasCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    public delegate void ConsultaLoteCompletedEventHandler(object sender, ConsultaLoteCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ConsultaLoteCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ConsultaLoteCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    public delegate void ConsultaInformacoesLoteCompletedEventHandler(object sender, ConsultaInformacoesLoteCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ConsultaInformacoesLoteCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ConsultaInformacoesLoteCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    public delegate void ConsultaCNPJCompletedEventHandler(object sender, ConsultaCNPJCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1590.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ConsultaCNPJCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ConsultaCNPJCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591