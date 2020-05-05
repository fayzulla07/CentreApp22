using System;
using System.Collections.Generic;
using System.Linq;
using CentreApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlData.SqlGenerator;
using Syncfusion.EJ2.Base;

namespace CentreApp.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProductTypeController : Controller
    {
        ISqlData data;
        public ProductTypeController(ISqlData data)
        {
            this.data = data;
        }
        // GET: Unit
        public ActionResult Index()
        {
            return View();
        }
      
        public ActionResult Insert([FromBody]ICRUDModel<ProductTypes> value)
        {
            int result = data.Add<ProductTypes>(value.value);
            return Json(value.value);
        }
       
        public ActionResult Update([FromBody]ICRUDModel<ProductTypes> entity)
        {
            int result = data.Update<ProductTypes>(entity.value);
            return Json(entity.value);
        }
       

        public ActionResult Delete([FromBody]ICRUDModel<ProductTypes> entity)
        {
            int result = data.Remove<ProductTypes>(Convert.ToInt32(entity.key));
            return Json(entity.value);
        }
        public IActionResult ForDropDown([FromBody]DataManagerRequest dm)
        {
            IEnumerable<ProductTypes> DataSource = data.GetAll<ProductTypes>();
            DataSource.ToList().Insert(0, new ProductTypes { Name = "Выбрать" });
            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  //Search
            }
            if (dm.Where != null && dm.Where.Count > 0) //Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<ProductTypes>().Count();
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        public IActionResult Setting([FromBody]DataManagerRequest dm)
        {
            
            IEnumerable<ProductTypes> DataSource = data.GetAll<ProductTypes>();
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
            int count = DataSource.Cast<ProductTypes>().Count();
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