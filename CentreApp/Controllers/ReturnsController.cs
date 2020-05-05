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
    [Authorize(Roles = "admin, seller")]
    public class ReturnsController : Controller
    {
        ISqlData data;
        public ReturnsController(ISqlData data)
        {
            this.data = data;
        }
        // GET: Unit
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Delete([FromBody]ICRUDModel<ReturnView> entity)
        {
            if (!ModelState.IsValid) // если проверка не удалась
            {
                return new BadRequestObjectResult(HttpStatusCode.BadRequest);//message returns the exception content  

            }
            int result = data.SqlExecuteProc("SP_DeleteProductReturn", new { Id = entity.key });
            return Json(entity.value);
        }
     
        public IActionResult Setting([FromBody]DataManagerRequest dm)
        {
            IEnumerable<ReturnView> DataSource = data.GetAll<ReturnView>();
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
            int count = DataSource.Cast<ReturnView>().Count();
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