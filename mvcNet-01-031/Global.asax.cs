using AppLogService;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace mvcNet_01_031
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //public static string AppInfoVersion = typeof(mvcNet_01_031.MvcApplication).Assembly.GetName().Name;
        //public static string AppTimeStamp = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;

        //public static System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        //public static FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                //string fileVersion = fvi.ProductVersion;
                //ViewBag.FileVersion = fvi.ProductVersion; ;

        private static System.Diagnostics.FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
        public static string AppName = fvi.ProductName.ToString();
        public static string AppVersion = fvi.ProductVersion.ToString();

        protected void Application_Start()
        {
            var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var version = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyLocation).ProductVersion;
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AppLog.LogStatus($"Info|{AppName} v.{AppVersion} Application START");

            //***TC AppScan suggested remediation of vulnerabilities
            if (ServicePointManager.SecurityProtocol.HasFlag(SecurityProtocolType.Tls12) == false)
            {
                ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
            }
#if DEBUG
            AppLog.LogStatus("Trace|Test1 Trace Log Entry");   //text-secondary
            AppLog.LogStatus("Debug|Test2 Debug Log Entry");   //text-white bg-info
            AppLog.LogStatus("Info|Test3 Info Log Entry");     //text-info
            AppLog.LogStatus("Warn|Test4 Warning Log Entry");  //text-warning
            AppLog.LogStatus("Error|Test5 Error Log Entry");   //text-danger
            AppLog.LogStatus("Fatal|Test6 Fatal Log Entry");   //text-white bg-dark
#endif
        }

        protected void Application_End()
        {
            AppLog.LogStatus("Info|${AppName} Application END");
            AppLog.ShutDown();
        }

        protected void Application_Error()
        {
            AppLog.LogStatus("Error|Application Error");

            var error = Server.GetLastError();
            if (error.Message.Length > 0)
                AppLog.LogStatus($"Info|Last Server Error [{error.Message}] [{(null == error.InnerException ? "no inner exception" : error.InnerException.Message)}]");

            var code = (error is HttpException) ? (error as HttpException).GetHttpCode() : 500;

            if (code != 404)
            {
                AppLog.LogStatus("Info|Http Error, code [{code}]");
            }
        }

        protected void Session_Start()
        {
            AppLog.LogStatus("Trace|Session_Start");

            //***TC AppScan cookie secure
            //SessionStateSection sessionState = (SessionStateSection)ConfigurationManager.GetSection("system.web/sessionState");
            //string sidCookieName = sessionState.CookieName;
            //if (Request.Cookies[sidCookieName] != null)
            //{
            //    HttpCookie sidCookie = Response.Cookies[sidCookieName];
            //    sidCookie.Value = Session.SessionID;
            //    sidCookie.HttpOnly = true;
            //    sidCookie.Secure = true;
            //    sidCookie.Path = "/";
            //}

#if DEBUG
            SetOrUpdateSessionVar("CurrentUserName", "TestUserName");
            SetOrUpdateSessionVar("CurrentUserIP", 1);
#endif
            //***TC RFU not SEQ: AddBlankLineBetweenLogEntries value is overridden by WebConfig setting in App_Services.LogService
            SetOrUpdateSessionVar((string)"AddBlankLineBetweenLogEntries", (bool)false);
            SetOrUpdateSessionVar("CurrentUserIP", string.Empty);
            SetOrUpdateSessionVar("CurrentUserName", string.Empty);
        }

        protected void Session_End()
        {
            AppLog.LogStatus("Trace|Session_End");
        }

        /// <summary>
        /// CreateSessionVar generic, creates a new session var of the type passed, removing any existing var with passed name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SessionVarKey"></param>
        /// <param name="Value"></param>
        protected void CreateSessionVar<T>(string SessionVarKey, T Value)
        {
            if (null != Session[SessionVarKey])
                Session.Remove(SessionVarKey);
            Session.Add(SessionVarKey, Value);
        }

        /// <summary>
        /// SetOrUpdateSessionVar generic to set or update session var with passed name and value
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="sessionVarName"></param>
        /// <param name="Value"></param>
        private void SetOrUpdateSessionVar<TValue>(string sessionVarName, TValue Value)
        {
            if (null == Session[sessionVarName])
                Session.Add(sessionVarName, Value);
            else if (!Session[sessionVarName].Equals(Value))
                Session[sessionVarName] = Value;
        }
    }
    /// <summary>
    /// Information about the executing assembly.
    /// </summary>
    public static class AssemblyInfo
            {
        private static DateTime? _Date = null;
        /// <summary>
        /// Gets the linker date from the assembly header.
        /// </summary>
        public static DateTime Date
        {
            get {
                if (_Date == null)
                {
                    _Date = GetLinkerTime(Assembly.GetExecutingAssembly());
                }
                return _Date.Value;
            }
        }

        /// <summary>
        /// Gets the linker date of the assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        /// <remarks>https://blog.codinghorror.com/determining-build-date-the-hard-way/>
        private static DateTime GetLinkerTime(Assembly assembly)
        {
            var filePath                      = assembly.Location;
            const int c_PeHeaderOffset        = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset           = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch            = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var linkTimeUtc      = epoch.AddSeconds(secondsSince1970);
            return linkTimeUtc.ToLocalTime();
        }
    }
}
