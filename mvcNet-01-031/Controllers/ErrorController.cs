using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvcNet_01_031.Controllers
{
    [HandleError(View = "Error", ExceptionType = typeof(NullReferenceException))]

    //***TC CE from poc_CustomErrors

    public class ErrorController : Controller
    {
        [HandleError(ExceptionType = typeof(HttpRequest))]
        public ActionResult BadRequest()
        {
            var model = new System.Web.Mvc.HandleErrorInfo(new HttpUnhandledException(), "Error", "BadRequest");
            //return View("BadRequest");
            return View("Error", model);
        }

        [HandleError(ExceptionType = typeof(DivideByZeroException))]
        public ActionResult DivideByZero()
        {
            var model = new System.Web.Mvc.HandleErrorInfo(new DivideByZeroException(), "Error", "DivideByZero");
            return View("Error", model);
        }

        [HandleError(ExceptionType = typeof(HttpException))]
        public ActionResult HttpException()
        {
            var model = new System.Web.Mvc.HandleErrorInfo(new HttpException(), "Error", "HttpException");
            return View("Error", model);
        }

        [HandleError(ExceptionType = typeof(HttpNotFoundResult))]
        public ActionResult NotFound()
        {
            var model = new System.Web.Mvc.HandleErrorInfo(new HttpUnhandledException(), "Error", "NotFound");
            return View("Error", model);
        }

        [HandleError(ExceptionType = typeof(HttpServerUtility))]
        public ActionResult ServerError()
        {
            var model = new System.Web.Mvc.HandleErrorInfo(new HttpUnhandledException(), "Error", "ServerError");
            return View("Error", model);
        }
    }
}