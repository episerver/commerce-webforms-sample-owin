using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using EPiServer.Commerce.Sample.BaseControls;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Security;
using EPiServer.ServiceLocation;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.WsFederation;

namespace EPiServer.Commerce.Sample.Templates.Sample.MasterPages.Controls
{
    public partial class LoginSelector : RendererControlBase<EntryContentBase>
    {
        private readonly IRegistrar _registrar = ServiceLocator.Current.GetInstance<IRegistrar>();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void logout_ButtonClick(object sender, EventArgs e)
        {
            //Response.Cookies.Clear();
            //Response.SetStatus(HttpStatusCode.SeeOther);
            //Context.GetOwinContext().Authentication.SignOut(WsFederationAuthenticationDefaults.AuthenticationType);
            Context.GetOwinContext().Authentication.SignOut(_registrar.AuthenticationType);
            Response.Redirect("/", false);
        }
        
        protected string GetLoginUrl()
        {
            return GetUrl(Settings.LoginPage);
        }
    }
}