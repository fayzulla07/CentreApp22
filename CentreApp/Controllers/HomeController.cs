using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using CentreApp.Models;
using FastReport.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlData.SqlGenerator;

namespace CentreApp.Controllers
{
    [Authorize(Roles = "admin, seller")]
    public class HomeController : Controller
    {
        ISqlData data;
        public HomeController(ISqlData data)
        {
            this.data = data;
        }
        public IActionResult Index()
        {
           var result = data.GetAll<SalesTotalByDayView>().FirstOrDefault();
           var resultReturn = data.GetAll<ReturnTotalByDayView>().FirstOrDefault();
            if(result == null)
            {
                ViewBag.pribl = 0;
                ViewBag.totall = new SalesTotalByDayView() { RegDt = DateTime.Now.Date, IncomeCost = 0, SaleTotal = 0 };
            }
            else
            {
                ViewBag.pribl = result.SaleTotal - result.IncomeCost;
                ViewBag.totall = result;
            }
            if (resultReturn == null)
            {
                ViewBag.rtotall = new ReturnTotalByDayView() { RegDt = DateTime.Now.Date, ReturnCost = 0 };
            }
            else
            {
                ViewBag.rtotall = resultReturn;
            }
            return View();
        }
        public IActionResult GetTotal(DateTime? DT)
        {
            SalesTotalByDayView result = null;
            ReturnTotalByDayView resultReturn = null;
            if (DT == null)
            {
                DateTime date = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time"));
                result = data.SqlQuery<SalesTotalByDayView>("select * from SalesTotalByDayView where RegDt = @DT", new { DT = date.ToString("yyyy-MM-dd") }).FirstOrDefault();
                resultReturn = data.SqlQuery<ReturnTotalByDayView>("select * from ReturnTotalByDayView where RegDt = @DT", new { DT = date.ToString("yyyy-MM-dd") }).FirstOrDefault();
            }
            else
            {
                DateTime date = TimeZoneInfo.ConvertTime((DateTime)DT, TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time"));
                result = data.SqlQuery<SalesTotalByDayView>("select * from SalesTotalByDayView where RegDt = @DT", new { DT = date.ToString("yyyy-MM-dd") }).FirstOrDefault();
                resultReturn = data.SqlQuery<ReturnTotalByDayView>("select * from ReturnTotalByDayView where RegDt = @DT", new { DT = date.ToString("yyyy-MM-dd") }).FirstOrDefault();
            }
            if (result == null)
            {
                ViewBag.pribl = 0;
                ViewBag.totall = new SalesTotalByDayView() { RegDt = DT == null ? DateTime.Now : (DateTime)DT, IncomeCost = 0, SaleTotal = 0 };
            }
            else
            {
                ViewBag.pribl = result.SaleTotal - result.IncomeCost;
                ViewBag.totall = result;
            }
            if (resultReturn == null)
            {
                ViewBag.rtotall = new ReturnTotalByDayView() { RegDt = DT == null ? DateTime.Now : (DateTime)DT, ReturnCost = 0 };
            }
            else
            {
                ViewBag.rtotall = resultReturn;
            }
            return View("Index");
        }
    }

}