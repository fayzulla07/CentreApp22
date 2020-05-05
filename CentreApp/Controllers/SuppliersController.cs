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
    public class SuppliersController : Controller
    {
        ISqlData data;
        public SuppliersController(ISqlData data)
        {
            this.data = data;
        }
        // GET: Unit
        public ActionResult Index()
        {
            return View();
        }
      
        public ActionResult Insert([FromBody]ICRUDModel<Suppliers> value)
        {
            int result = data.Add<Suppliers>(value.value);
            return Json(value.value);
        }
       
        public ActionResult Update([FromBody]ICRUDModel<Suppliers> entity)
        {
            int result = data.Update<Suppliers>(entity.value);
            return Json(entity.value);
        }
       

        public ActionResult Delete([FromBody]ICRUDModel<Suppliers> entity)
        {
            int result = data.Remove<Suppliers>(Convert.ToInt32(entity.key));
            return Json(entity.value);
        }
        public IActionResult ForDropDown([FromBody]DataManagerRequest dm)
        {
            IEnumerable<Suppliers> DataSource = null;//= data.SqlQuery<Products>("select * from Products");
            DataOperations operation = new DataOperations();
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                if (dm.Where.FirstOrDefault().value != null)
                {
                    // DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
                    DataSource = data.SqlQuery<Suppliers>($"SELECT [Id], [Name] from Suppliers where " +
                        $"[Name] like CONCAT('%',@searching,'%') or " +
                        $"[Code] like CONCAT('%',@searching,'%') ORDER BY Id OFFSET 0 ROWS FETCH NEXT 100 ROWS ONLY", new { searching = dm.Where.FirstOrDefault().value });
                }
            }
            if (DataSource == null)
            {
                DataSource = data.SqlQuery<Suppliers>($"SELECT [Id], [Name] from Suppliers ORDER BY Id OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY");

            }
            int count = data.SqlQuery<int>("select count(*) from Suppliers").FirstOrDefault();

            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        public IActionResult Setting([FromBody]DataManagerRequest dm)
        {
            IEnumerable<Suppliers> DataSource = data.GetAll<Suppliers>();
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
            int count = DataSource.Cast<Suppliers>().Count();
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