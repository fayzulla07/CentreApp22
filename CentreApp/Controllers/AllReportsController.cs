using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FastReport.Web;
using Microsoft.AspNetCore.Authorization;

namespace CentreApp.Controllers
{

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
        [Authorize(Roles = "admin, seller")]
        public IActionResult Index()
        {
            WebReport.Report.Load(@"Reports/Remains.frx");
            return View(WebReport);
        }
        [Authorize(Roles = "admin, seller")]
        [HttpPost]
        public IActionResult Report(string sval)
        {
            if (sval == "1") // остаток
            {
                WebReport.Report.Load(@"Reports/Remains.frx");
                return PartialView("Remains", WebReport); // pass the report to View
            }
            else if (sval == "2" && User.IsInRole("admin")) // приход
            {
                ViewBag.da1 ="";
                ViewBag.da2 = "";
                WebReport.Report.Load(@"Reports/Incomes.frx");
                return PartialView("Incomes", WebReport); // pass the report to View
            }
            else if (sval == "3")   // продажа
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
            if (sval == "4") // За сегодня
            {
                WebReport.Report.Load(@"Reports/RepToday.frx");
                return PartialView("RepToday", WebReport); // pass the report to View
            }
            if (sval == "5" && User.IsInRole("admin")) // Аналитика
            {
                WebReport.Report.Load(@"Reports/Analitic.frx");
                return PartialView("Analitic", WebReport); // pass the report to View
            }
            else
            {
                WebReport.Report.Load(@"Reports/Remains.frx");
                return PartialView("Remains", WebReport); // pass the report to View
            }
        }
        // ------------------------------------------------------------------------------------------------------------- //
        // ------------------------------------------------------------------------------------------------------------- //
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Incomes(DateTime? da1 = null, DateTime? da2 = null)
        {
            WebReport.Report.Load(@"Reports/Incomes.frx");
            if (da1 == null || da2 == null)
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
        [Authorize(Roles = "admin, seller")]
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