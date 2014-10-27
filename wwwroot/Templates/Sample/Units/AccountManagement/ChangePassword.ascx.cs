using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Commerce.Sample.BaseControls;
using EPiServer.Commerce.Security;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using System;

namespace EPiServer.Commerce.Sample.Templates.Sample.Units.AccountManagement
{
	public partial class ChangePassword : RendererControlBase<CatalogContentBase>
	{
        private readonly IRegistrar _registrar = ServiceLocator.Current.GetInstance<IRegistrar>();

	    protected void Page_Load(object sender, EventArgs e)
		{

		}

        protected void savePassword_Click(object sender, EventArgs e)
        {
            if (!_registrar.ValidateUser(PrincipalInfo.CurrentPrincipal.Identity.Name, CurrentPassword.Text))
            {
                passwordError.Text = "Old Password is not valid, please fix and try again";
                return;
            }

            try
            {
                _registrar.ChangePassword(PrincipalInfo.CurrentPrincipal.Identity.Name, CurrentPassword.Text, NewPassword.Text);
                PasswordSuccessful.Text = "Password changed successfully!";
            }
            catch (Exception ex)
            {
                passwordError.Text = ex.Message;
            }
        }

        protected void cancel_Click(object sender, EventArgs e)
        {
            Context.RedirectFast(GetUrl(Settings.AccountPage));
        }
	}
}