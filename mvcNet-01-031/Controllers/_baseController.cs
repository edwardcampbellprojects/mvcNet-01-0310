using AppLogService;
using iterDev.CS.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace mvcNet_01_031.Controllers
{
    public class _baseController : Controller
    {
        interface I_baseController
        {
            bool Using_LINQ();

            //string GetGlobalValue(string keyName);
            //void SetGlobalValue(string keyName, string keyValue);

            string GetPrompt();
            void SetPrompt(string prompt);

            string GetStatus();
            void SetStatus(string status);
        }

        //        public _baseController()
        //        {
        //#if _baseDEBUG
        //            AppLog.LogTrace("_baseController loaded");
        //#endif
        //        }
        #region constructors...

        //protected _Entities db { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="_baseController"/> class.
        /// </summary>
        public _baseController()
        {
            //db = new _Entities();
        }

        protected override void Dispose(bool disposing)
        {
            //db.Dispose();
            base.Dispose(disposing);
        }
        #endregion

        #region Global Event Handlers

        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            string controllerName = context.ActionDescriptor.ControllerDescriptor.ControllerName;
            string actionName = context.ActionDescriptor.ActionName;
            StringBuilder sbParameters = new StringBuilder(1);

            foreach (var parameter in context.ActionParameters)
            {
                var parameterKey = parameter.Key;
                var parameterValue = parameter.Value;
                sbParameters.Append($"{parameter.Key}={parameter.Value}, ");
            }
            string userIP = Request.ServerVariables["REMOTE_ADDR"].ToString();
            string userName = "PUBLIC";

            try
            {
                //Determine the logged in user name and save it
                //if (null != Request.Cookies["CurrentUser"])
                //    userName = Request.Cookies["CurrentUser"].Values["UserName"];
                //if (null != User.Identity)
                //    userName = User.Identity.GetUserName();

                //userName = string.IsNullOrEmpty($"{User.Identity.Name}") ? "UNKNOWN" : User.Identity.Name;
                userName = string.IsNullOrEmpty(User.Identity.Name) ? "UNKNOWN" : User.Identity.Name;
            }
            catch (Exception exc)
            {
                //string msg = $"Error|{AfmInteractiveDirectory.MvcApplication.AppName}.OnActionExecuting: Current User Name not found in ServerVariables[\"HTTP_COOKIE\"]: [{exc.Message}]";
                string msg = $"Error|{this}.OnActionExecuting: Current User Name not found: [{exc.Message}]";
                Session["Status"] = msg;
                AppLogService.AppLog.LogStatus(msg);
#if DEBUG
                throw new Exception(msg);
#endif
            }

            string parameters = sbParameters.ToString().Length > 2 ? sbParameters.ToString().RemoveLast(2) : sbParameters.ToString();

            AppLog.LogStatus($"Trace|[{userIP}.{userName}] {controllerName}.{actionName}({parameters})");
            base.OnActionExecuting(context);
        }

        //***TC EXC mvc https://www.c-sharpcorner.com/article/exception-handling-in-asp-net-mvc/
        /// <summary>
        /// _baseController OnException exception handler v 1
        /// </summary>
        /// <param name="exceptionContext"></param>
        protected override void OnException(ExceptionContext exceptionContext)
        {
            string controllerName = (string)exceptionContext.RouteData.Values["controller"];
            string actionName = (string)exceptionContext.RouteData.Values["action"];

            string excMsg = exceptionContext.Exception.Message;
            Exception custException = new Exception(excMsg);

            //***TC LOG
            //AppLog.LogError(exceptionContext.Exception.Message + " in " + controllerName);
            string innerExcMsg = exceptionContext.Exception.InnerException == null ? "no inner exception" : exceptionContext.Exception.InnerException.Message;
            AppLog.LogError($"EXCEPTION in {controllerName}.{actionName}: exceptionContext.Exception.Message [{innerExcMsg}]" );

            var model = new HandleErrorInfo(custException, controllerName, actionName);
            int httpStatusCode = exceptionContext.HttpContext.Response.StatusCode;

            switch (httpStatusCode.ToString())
            {
                case "200":
                    break;
                case "400":
                    exceptionContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/BadRequest.cshtml",
                        ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                        TempData = exceptionContext.Controller.TempData
                    };
                    break;
                case "404":
                    exceptionContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/Exception404.cshtml",
                        ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                        TempData = exceptionContext.Controller.TempData
                    };
                    break;
                case "500":
                    exceptionContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/Exception500.cshtml",
                        ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                        TempData = exceptionContext.Controller.TempData
                    };
                    break;
                default:
                    exceptionContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/Shared/Error.cshtml",
                        ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                        TempData = exceptionContext.Controller.TempData
                    };
                    break;
            }
        }

        #endregion

        #region App_UiGlobalHelpers
        // specific methods
        public virtual bool Using_LINQ() { return (bool)ConfigurationManager.AppSettings["Use_LINQ"].ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase); }

        public virtual string GetPrompt() { return GetGlobalValue("Prompt"); }
        public virtual void SetPrompt(string prompt) { SetGlobalValue("Prompt", prompt); }

        public virtual string GetStatus() { return GetGlobalValue("Status"); }
        public virtual void SetStatus(string status) { SetGlobalValue("Status", status); }

        /// <summary>
        /// Gets the global value.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <returns>System.String.</returns>
        public virtual string GetGlobalValue(string keyName)
        {
            if (!String.IsNullOrWhiteSpace((string)Session[keyName]))
                return Session[keyName].ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Sets the global value.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="keyValue">The key value.</param>
        public virtual void SetGlobalValue(string keyName, string keyValue)
        {
#if DEBUG
            //Can't use SetStatus() because it will recurse
            Session["Status"] = keyName + " set to " + keyValue;
#endif
            if (null == Session[keyName])
                Session.Add(keyName, keyValue);
            else
                Session[keyName] = keyValue;
        }

        #region StatusSet Methods

        public void StatusSet_Error(string StatusText, bool StatusPersist = false, bool StatusShown = false)
        {
            _SetStatus("alert-danger", StatusText, StatusPersist, StatusShown);
        }

        public void StatusSet_Info(string StatusText, bool StatusPersist = false, bool StatusShown = false)
        {
            _SetStatus("alert-info", StatusText, StatusPersist, StatusShown);
        }

        public void StatusSet_Success(string StatusText, bool StatusPersist = false, bool StatusShown = false)
        {
            _SetStatus("alert-success", StatusText, StatusPersist, StatusShown);
        }

        public void StatusSet_Warning(string StatusText, bool StatusPersist = false, bool StatusShown = false)
        {
            _SetStatus("alert-warning", StatusText, StatusPersist, StatusShown);
        }

        private void _SetStatus(string StatusClass, string StatusText, bool StatusPersist = false, bool StatusShown = false)
        {
            Session["StatusClass"]   = StatusClass;
            Session["Status"]        = StatusText;
            Session["StatusPersist"] = StatusPersist;
            Session["StatusShown"]   = StatusShown;
        }
        #endregion //StatusSet

        #endregion

    }
}