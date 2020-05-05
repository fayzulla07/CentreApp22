using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CustomerController : Controller
    {
        ISqlData data;
        public CustomerController(ISqlData data)
        {
            this.data = data;
        }
        // GET: Unit
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult Insert([FromBody]ICRUDModel<Customers> value)
        {
            int result = data.Add<Customers>(value.value);
            return Json(value.value);
        }
        [HttpPost]
        public ActionResult InsertCustom(Customers value)
        {
            int result = data.Add<Customers>(value);
            return Json(value);
        }
        public ActionResult Update([FromBody]ICRUDModel<Customers> entity)
        {
            int result = data.Update<Customers>(entity.value);
            return Json(entity.value);
        }

        public ActionResult Delete([FromBody]ICRUDModel<Customers> entity)
        {
            int result = data.Remove<Customers>(Convert.ToInt32(entity.key));
            return Json(entity.value);
        }
        public IActionResult ForDropDown([FromBody]DataManagerRequest dm)
        {
            IEnumerable<Customers> DataSource = data.GetAll<Customers>();
            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
            }
            int count = DataSource.Cast<Customers>().Count();
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        public IActionResult Setting([FromBody]DataManagerRequest dm)
        {
            IEnumerable<Customers> DataSource = data.GetAll<Customers>();
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
            int count = DataSource.Cast<Customers>().Count();
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