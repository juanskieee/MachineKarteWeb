using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;

namespace MachineKarteWeb
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        void Application_Error(object sender, EventArgs e)
        {
            if (!IsDiagnosticsEnabled())
            {
                return;
            }

            try
            {
                Exception error = Server.GetLastError();

                if (error == null)
                {
                    return;
                }

                string folder = Server.MapPath("~/App_Data/Logs");
                Directory.CreateDirectory(folder);

                string fileName = "error-" +
                    DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) +
                    ".log";
                string path = Path.Combine(folder, fileName);

                var entry = new StringBuilder();
                entry.AppendLine("==== " +
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) +
                    " ====");
                entry.AppendLine("Url: " + Request.RawUrl);
                entry.AppendLine("UserHostAddress: " + Request.UserHostAddress);
                entry.AppendLine(error.ToString());
                entry.AppendLine();

                File.AppendAllText(path, entry.ToString(), Encoding.UTF8);
            }
            catch
            {
                // Logging must never mask the original error.
            }
        }

        private static bool IsDiagnosticsEnabled()
        {
            string setting = ConfigurationManager.AppSettings["DiagnosticsEnabled"];

            if (string.IsNullOrWhiteSpace(setting))
            {
                return true;
            }

            bool enabled;
            return !bool.TryParse(setting, out enabled) || enabled;
        }
    }
}
