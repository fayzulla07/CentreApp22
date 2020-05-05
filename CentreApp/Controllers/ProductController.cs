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

    public class ProductController : Controller
    {
        ISqlData data;
        public ProductController(ISqlData data)
        {
            this.data = data;
        }
        // GET: Unit
        [Authorize(Roles = "admin")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "admin")]
        public ActionResult Insert([FromBody]ICRUDModel<Products> entity)
        {
            if (!ModelState.IsValid) // если проверка не удалась
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                BadRequestObjectResult badRequest = new BadRequestObjectResult(HttpStatusCode.BadRequest);
                badRequest.Value = "Ошибка добавление модели";

            }
                double? Amount = entity.value.Amount;
                 entity.value.Amount = null;
            int result = data.Add(entity.value);
            entity.value.Amount = Amount;
            return Json(entity.value);
        }
        [Authorize(Roles = "admin")]
        public ActionResult Update([FromBody]ICRUDModel<Products> entity)
        {
            if (!ModelState.IsValid) // если проверка не удалась
            {
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                BadRequestObjectResult badRequest = new BadRequestObjectResult(HttpStatusCode.BadRequest);
                badRequest.Value = "Ошибка изменение модели";

            }
            double? Amount = entity.value.Amount;
            entity.value.Amount = null;
            int result = data.Update<Products>(entity.value);
            entity.value.Amount = Amount;
            return Json(entity.value);
        }
        [Authorize(Roles = "admin")]
        public ActionResult Delete([FromBody]ICRUDModel<Products> entity)
        {
            int result = data.Remove<Products>(Convert.ToInt32(entity.key));
            return Json(entity.value);
        }
        [Authorize(Roles = "admin, seller")]
        public IActionResult ForDropDown([FromBody]DataManagerRequest dm)
        {
            IEnumerable<Products> DataSource = data.SqlQuery<Products>("select p.Id, p.Name + '  ||  ' + cast(p.RemainCount as varchar(30)) + u.Name as [Name] from Products as p inner join Units as u on p.UnitId = u.Id");
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
            int count = DataSource.Cast<Products>().Count();

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        [Authorize(Roles = "admin, seller")]
        public IActionResult Setting([FromBody]DataManagerRequest dm)
        {
            IEnumerable<Products> DataSource = data.GetAll<Products>();
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
            int count = DataSource.Cast<Products>().Count();
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