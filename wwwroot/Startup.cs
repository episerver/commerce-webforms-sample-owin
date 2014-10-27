using EPiServer.Commerce.Security;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using Owin;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;

[assembly: OwinStartup(typeof(EPiServer.Commerce.Sample.Startup))]

namespace EPiServer.Commerce.Sample
{
    /// <summary>
    /// Sample Startup file to be use EPiServer.Commerce.Security
    /// </summary>
    public class Startup
    {
        const string LogoutUrl = "/util/logout.aspx";
        private readonly IRegistrar _registrar = ServiceLocator.Current.GetInstance<IRegistrar>();

        /// <summary>
        /// Configuration method used by Microsoft.Owin to initialize owin process.
        /// </summary>
        /// <param name="app">The application.</param>
        public void Configuration(IAppBuilder app)
        {
            //Uncomment the line below if you have are only using the configure IRegistrar for authentication
            //app.SetDefaultSignInAsAuthenticationType(_registrar.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = _registrar.AuthenticationType
            });

            //Comment out the following if you are not using ADFS authenication down to
            app.SetDefaultSignInAsAuthenticationType(WsFederationAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = WsFederationAuthenticationDefaults.AuthenticationType
            });

            //Enable federated authentication
            app.UseWsFederationAuthentication(new WsFederationAuthenticationOptions()
            {
                //Trusted URL to federation server meta data
                MetadataAddress = "https://devdc.dev.test/federationmetadata/2007-06/federationmetadata.xml",
                //Value of Wtreal must *exactly* match what is configured in the federation server
                Wtrealm = "https://commercesample:44333/",
                Notifications = new WsFederationAuthenticationNotifications()
                {
                    RedirectToIdentityProvider = (ctx) =>
                    {
                        //To avoid a redirect loop to the federation server send 403 when user is authenticated but does not have access
                        if (ctx.OwinContext.Response.StatusCode == 401 && ctx.OwinContext.Authentication.User.Identity.IsAuthenticated)
                        {
                            ctx.OwinContext.Response.StatusCode = 403;
                            ctx.HandleResponse();
                        }
                        return Task.FromResult(0);
                    },
                    SecurityTokenValidated = (ctx) =>
                    {
                        //Ignore scheme/host name in redirect Uri to make sure a redirect to HTTPS does not redirect back to HTTP
                        var redirectUri = new Uri(ctx.AuthenticationTicket.Properties.RedirectUri, UriKind.RelativeOrAbsolute);
                        if (redirectUri.IsAbsoluteUri)
                        {
                            ctx.AuthenticationTicket.Properties.RedirectUri = redirectUri.PathAndQuery;
                        }
                        //Sync user and the roles to EPiServer in the background
                        ServiceLocator.Current.GetInstance<SynchronizingUserService>().SynchronizeAsync(ctx.AuthenticationTicket.Identity);
                        return Task.FromResult(0);
                    },
                    
                }
            });
            //End Comment out AFDS if you are not using
            
            //Always keep the following to properly setup virtual roles for principals, map the logout in backend function and create anti forgery key  
            app.UseStageMarker(PipelineStage.Authenticate);
            app.Map(LogoutUrl, map =>
            {
                map.Run(ctx =>
                {
                    ctx.Authentication.SignOut();
                    return Task.FromResult(0);
                });
            });
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
        }
    }
}





