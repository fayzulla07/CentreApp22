using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FastReport.Web;
using FastReport.Data;
using FastReport.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using CentreApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace CentreApp.Controllers
{
    [Authorize(Roles = "admin, seller")]
    public class AllReportsController : Controller
    {
        WebReport WebReport;
        public AllReportsController()
        {
            WebReport = new WebReport();
            WebReport.ShowToolbar = true;
            WebReport.Width = "100%";
            WebReport.Mode = WebReportMode.Preview;

        }
        public IActionResult Index()
        {
            WebReport.Report.Load(@"Reports/Remains.frx");
            return View(WebReport);
        }

        [HttpPost]
        public IActionResult Report(string sval)
        {
            if (sval == "1")
            {
                WebReport.Report.Load(@"Reports/Remains.frx");
                return PartialView("Remains", WebReport); // pass the report to View
            }
            else if (sval == "2")
            {
                ViewBag.da1 ="";
                ViewBag.da2 = "";
                WebReport.Report.Load(@"Reports/Incomes.frx");
                return PartialView("Incomes", WebReport); // pass the report to View
            }
            else if (sval == "3")
            {
                ViewBag.da1 = "";
                ViewBag.da2 = "";
                WebReport.Report.Load(@"Reports/SalesRep.frx");

                WebReport.Report.SetParameterValue("da1", DateTime.Now.AddYears(-3));
                WebReport.Report.SetParameterValue("da2", DateTime.Now.AddYears(-3));
                WebReport.Report.SetParameterValue("da3", DateTime.Now.AddYears(-3));
                WebReport.Report.SetParameterValue("da4", DateTime.Now.AddYears(-3));

                return PartialView("SalesRep", WebReport); // pass the report to View
            }
            else
            {
                WebReport.Report.Load(@"Reports/Remains.frx");
                return PartialView("Remains", WebReport); // pass the report to View
            }
        }
        // ------------------------------------------------------------------------------------------------------------- //
        // ------------------------------------------------------------------------------------------------------------- //
        [HttpPost]
        public IActionResult Incomes(DateTime? da1 = null, DateTime? da2 = null)
        {
            WebReport.Report.Load(@"Reports/Incomes.frx");
            if(da1 == null || da2 == null)
            {
                ViewBag.da1 = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                ViewBag.da2 = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                WebReport.Report.SetParameterValue("da1", DateTime.Now.AddDays(-1));
                WebReport.Report.SetParameterValue("da2", DateTime.Now.AddDays(1));
            }
            else
            {
                ViewBag.da1 = da1?.ToString("yyyy-MM-dd");
                ViewBag.da2 = da2?.ToString("yyyy-MM-dd");
                WebReport.Report.SetParameterValue("da1", da1);
                WebReport.Report.SetParameterValue("da2", da2);
            }
            
            WebReport.Report.SetParameterValue("da1", da1);
            WebReport.Report.SetParameterValue("da2", da2);
            return View(WebReport);
        }
        // ------------------------------------------------------------------------------------------------------------- //
        [HttpPost]
        public IActionResult Sales(DateTime? da1 = null, DateTime? da2 = null)
        {
            WebReport.Report.Load(@"Reports/SalesRep.frx");
            if (da1 == null || da2 == null)
            {
                ViewBag.da1 = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                ViewBag.da2 = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

                WebReport.Report.SetParameterValue("da1", DateTime.Now.AddDays(-1));
                WebReport.Report.SetParameterValue("da2", DateTime.Now.AddDays(1));
                WebReport.Report.SetParameterValue("da3", DateTime.Now.AddDays(-1));
                WebReport.Report.SetParameterValue("da4", DateTime.Now.AddDays(1));
            }
            else
            {
                ViewBag.da1 = da1?.ToString("yyyy-MM-dd");
                ViewBag.da2 = da2?.ToString("yyyy-MM-dd");
                WebReport.Report.SetParameterValue("da1", da1);
                WebReport.Report.SetParameterValue("da2", da2);
                WebReport.Report.SetParameterValue("da3", da1);
                WebReport.Report.SetParameterValue("da4", da2);
            }

            return View("SalesRep", WebReport);
        }
    }
}