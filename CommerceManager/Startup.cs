using EPiServer.Commerce.Security;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;

[assembly: OwinStartup(typeof(Startup))]

namespace EPiServer.Commerce.Security
{
    /// <summary>
    /// Sample Startup file to be use EPiServer.Commerce.Security
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configuration method used by Microsoft.Owin to initialize owin process.
        /// </summary>
        /// <param name="app">The application.</param>
        public void Configuration(IAppBuilder app)
        {
            //Enable cookie authentication, used to store the claims between requests
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                AuthenticationMode = AuthenticationMode.Active,
                LoginPath = new PathString("/Login"),
                LogoutPath = new PathString("/Logout")
            });
        }
    }
}





