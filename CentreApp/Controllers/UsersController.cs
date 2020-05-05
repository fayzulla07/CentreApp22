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
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        ISqlData data;
        public UsersController(ISqlData data)
        {
            this.data = data;
        }
        // GET: Unit
        public ActionResult Index()
        {
            return View();
        }
      
        public ActionResult Insert([FromBody]ICRUDModel<Users> value)
        {
            int result = data.Add<Users>(value.value);
            return Json(value.value);
        }
       
        public ActionResult Update([FromBody]ICRUDModel<Users> entity)
        {
            int result = data.Update<Users>(entity.value);
            return Json(entity.value);
        }
       

        public ActionResult Delete([FromBody]ICRUDModel<Users> entity)
        {
            int result = data.Remove<Users>(Convert.ToInt32(entity.key));
            return Json(entity.value);
        }
        public ActionResult GetRole()
        {
            return Json(data.GetAll<Roles>());
        }
        public IActionResult ForDropDown([FromBody]DataManagerRequest dm)
        {
            IEnumerable<Users> DataSource = data.GetAll<Users>();

            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  // Search
            }
            if (dm.Where != null && dm.Where.Count > 0) // Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<Users>().Count();
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        public IActionResult Setting([FromBody]DataManagerRequest dm)
        {
            IEnumerable<Users> DataSource = data.GetAll<Users>();
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
            int count = DataSource.Cast<Users>().Count();
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