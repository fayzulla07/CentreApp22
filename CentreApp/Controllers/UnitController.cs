using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using CentreApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlData.SqlGenerator;
using Syncfusion.EJ2.Base;

namespace CentreApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class UnitController : Controller
    {
        ISqlData data;
        public UnitController(ISqlData data)
        {
            this.data = data;
        }
        // GET: Unit
        public ActionResult Index()
        {
            return View();
        }
      
        public ActionResult Insert([FromBody]ICRUDModel<Units> value)
        {
            if (!ModelState.IsValid) // если проверка не удалась
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                BadRequestObjectResult badRequest = new BadRequestObjectResult(HttpStatusCode.BadRequest);
                badRequest.Value = "Ошибка добавление";

            }
            int result = data.Add<Units>(value.value);
            return Json(value.value);
        }
       
        public ActionResult Update([FromBody]ICRUDModel<Units> entity)
        {
            if (!ModelState.IsValid) // если проверка не удалась
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return new BadRequestObjectResult(HttpStatusCode.BadRequest);//message returns the exception content  

            }
            int result = data.Update<Units>(entity.value);
            return Json(entity.value);
        }
       

        public ActionResult Delete([FromBody]ICRUDModel<Units> entity)
        {
            if (!ModelState.IsValid) // если проверка не удалась
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return new BadRequestObjectResult(HttpStatusCode.BadRequest);//message returns the exception content  

            }
            int result = data.Remove<Units>(Convert.ToInt32(entity.key));
            return Json(entity.value);
        }
        public IActionResult ForDropDown([FromBody]DataManagerRequest dm)
        {
            IEnumerable<Units> DataSource = data.GetAll<Units>();
            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  // Search
            }
            if (dm.Where != null && dm.Where.Count > 0) // Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<Units>().Count();
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        public IActionResult Setting([FromBody]DataManagerRequest dm)
        {
            IEnumerable<Units> DataSource = data.GetAll<Units>();
            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0) //Sorting
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<Units>().Count();
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
    }
}