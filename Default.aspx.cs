using System;
using System.Web.UI;

namespace MachineKarte.Web
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Monitoring.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
