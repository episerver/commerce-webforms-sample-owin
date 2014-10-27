using System.Linq;
using System.Web;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Sample.BaseControls;
using EPiServer.Commerce.Security;
using EPiServer.Framework.Localization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Core;
using Mediachase.Commerce.Customers;
using Mediachase.Commerce.Customers.Profile;
using Mediachase.Commerce.Security;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Security.Claims;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;

namespace EPiServer.Commerce.Sample.Templates.Sample.Units.Security
{
    public partial class Login : RendererControlBase<EntryContentBase>
    {
        private readonly IRegistrar _registrar = ServiceLocator.Current.GetInstance<IRegistrar>();
        private readonly static string[] DefaultUserRoles = { AppRoles.RegisteredRole, AppRoles.EveryoneRole };

        /// <summary>
        /// Handles the click event of an existing user
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void loginExisting_Click(object sender, EventArgs e)
        {
            var username = EmailAddress_ExistingId.Value;
            var password = Password_ExistingId.Value;
            bool remember = !String.IsNullOrEmpty(Request.Form["RememberMe"]);

            if (username == null || !_registrar.ValidateUser(username, password))
            {
                SignInFailureText.Text = "Login failed. Please make sure username and password are correct.";
                return;
            }

            var profile = SecurityContext.Current.CurrentUserProfile as CustomerProfileWrapper;
            if (profile == null)
            {
                throw new NullReferenceException("profile");
            }
            var accountState = profile.State;

            if (accountState == 1 || accountState == 3)
            {
                SignInFailureText.Text = LocalizationService.Current.GetString("Sample/Validation/AccountLocked");
                return;
            }

            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut(WsFederationAuthenticationDefaults.AuthenticationType);
            var identity = _registrar.CreateIdentity(username);
            var properties = new AuthenticationProperties()
            {
                IsPersistent = remember
            };
            _registrar.SignIn(properties, identity);
            
            Context.RedirectFast(GetUrl(Settings.AccountPage));
        }

        /// <summary>
        /// Handles the click event of a new user
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void loginCreateNew_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            string firstName = FirstNameId.Value;
            string lastName = LastNameId.Value;
            string emailAddress = EmailAddressNewId.Value;
            string password = Password_NewId.Value;
            ClaimsPrincipal principal = null;
            try
            {
                principal = _registrar.CreateUser(emailAddress, password, emailAddress);
            }
            catch (Exception ex)
            {
                CreateFailureText.Text = ex.Message;
                return;
            }

            // Now create an account in the ECF 
            var customerContact = CustomerContact.CreateInstance(principal);
            customerContact.FirstName = firstName;
            customerContact.LastName = lastName;
            customerContact.RegistrationSource = String.Format("{0}, {1}", this.Request.Url.Host, SiteContext.Current);
            customerContact["Email"] = emailAddress;

            customerContact.SaveChanges();
            _registrar.SignIn
            (
            new AuthenticationProperties()
            {
                IsPersistent = false
            }, principal.Identity as ClaimsIdentity);

            Context.RedirectFast(GetUrl(Settings.AccountPage));
        }
    }
}