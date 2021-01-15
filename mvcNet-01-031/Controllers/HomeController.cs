using mvcNet_01_031;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace mvcNet_01_031.Controllers
{
    public class HomeController : _baseController
    {
        public ActionResult Index()
        {
            ViewBag.ReadMe = string.Empty;

            string filePath = Server.MapPath("~/readme.txt");
            string[] readme = new string[100];

            if (System.IO.File.Exists(filePath))
            {
                readme = System.IO.File.ReadAllLines(filePath);
            }
            else
            {
                readme = new[] { $"File [{filePath} NOT FOUND]" };
            }

            ViewBag.ReadMe = readme;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = $"{MvcApplication.AppName} description page.";

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi    = FileVersionInfo.GetVersionInfo(assembly.Location);
            ViewBag.ProductName    = fvi.ProductName; ;
            ViewBag.ProductVersion = fvi.ProductVersion;

            var assemblyLocation         = System.Reflection.Assembly.GetExecutingAssembly().Location;
            FileInfo fileInfo            = new FileInfo(assemblyLocation);
            ViewBag.ProductBuildDateTime = fileInfo.LastWriteTime.ToString("yyyy.MM.ddTHHmm");

            //string pathAboutText      = ConfigurationManager.AppSettings["ProductAboutText-FilePath"];
            //string[] productAboutText = AppAboutTextAtServerPath(pathAboutText);
            //ViewBag.ProductAboutText = productAboutText;
            ViewBag.ProductAboutText = ViewBagProductAboutText();

            return View();
        }

        private string[] AppAboutTextAtServerPath(string pathAboutText)
        {
            string filePath = Server.MapPath(pathAboutText);

            string[] productAboutText = new string[100];

            if (System.IO.File.Exists(filePath))
            {
                productAboutText = System.IO.File.ReadAllLines(filePath);
            }
            else
            {
                productAboutText = new[] { $"File [{filePath} NOT FOUND]" };
            }

            return productAboutText;
        }

        public ActionResult Contact()
        {
            ViewBag.Message = $"{MvcApplication.AppName} contact page.";

            ViewBag.ProductAboutText = ViewBagProductAboutText();

            return View();
        }

        private string[] ViewBagProductAboutText()
        {
            string pathAboutText = ConfigurationManager.AppSettings["ProductAboutText-FilePath"];
            string[] productAboutText = AppAboutTextAtServerPath(pathAboutText);
            //ViewBag.ProductAboutText = productAboutText;
            return productAboutText;
        }
    }
}