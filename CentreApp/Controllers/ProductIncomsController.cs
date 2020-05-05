using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using CentreApp.Models;
using FastReport.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SqlData.SqlGenerator;
using Syncfusion.EJ2.Base;

namespace CentreApp.Controllers
{
    [Authorize(Roles = "admin")] // Приход товара и История прихода
    public class ProductIncomsController : Controller
    {

        ISqlData data;
        public ProductIncomsController(ISqlData data)
        {
            this.data = data;
        }
        // GET: Unit
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NewIncoms()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Get(string ProductCode, int PrId = 0)
        {
            if (ProductCode != null)
            {
                Products result;
                if (PrId > 0)
                    result = data.GetById<Products>(PrId);
                else
                    result = data.SqlQuery<Products>("select * from Products WHERE [Code] = @Code;", new { Code = ProductCode }).FirstOrDefault();
                if (result != null)
                {
                    return Json(new { ProductId = result.Id, Amount = 1, Volume = result.Volume, ProductName = result.Name });
                }
                else
                {
                    return Ok("Такого продукта нет!");
                }
            }
            return Ok("Введите штрих код!");
        }
        
         [HttpPost] // Для История прихода (старый) на гриде диалоговое окно
        public ActionResult InsertCosts(ProductIncomeCostModel entity)
        {
            try
            {
                int result = data.SqlExecuteProc("SP_AddCostByProductType", entity);
                return Ok("Успешно изменено!");
            }
            catch (Exception ex)
            {
                return StatusCode(123, ex.Message);
               
            }
           
        }
        [HttpPost] // Для История прихода (старый) на гриде диалоговое окно
        public ActionResult Insert(ProductIncoms entity)
        {
            if(User.FindFirstValue("UserId") != null)
            entity.UserId = Convert.ToInt32(User.FindFirstValue("UserId"));
            int result = data.SqlExecuteProc("SP_AddProductIncome", entity);
            return Json(entity);
        }
        [HttpPost]  // Для Прихода (новой)
        public ActionResult Insert2([FromBody]ProductIncoms[] entity)
        {
            if (entity == null)
            {
                return Json("null");
            }
            foreach (var item in entity)
            {
                if (User.FindFirstValue("UserId") != null)
                    item.UserId = Convert.ToInt32(User.FindFirstValue("UserId"));
            }
            List<object> idi = new List<object>();
            foreach (var item in entity)
            {
                int result = data.SqlExecuteProc("SP_AddProductIncome", item);
                if (result < 0)
                {
                    string Name = "";
                    Name = data.GetById<Products>(item.Id).Name;
                    idi.Add(new { Id = item.Id, Name });
                }
            }

            if (idi.Count == 0)
                return Json("Успешно сохранено!");
            else
            {
                return Json(idi);
            }

        }

        public ActionResult Update([FromBody]ICRUDModel<ProductIncoms> entity)
        {
            data.Update<ProductIncoms>(entity.value);
            return Json(entity.value);
        }
       

        public ActionResult Delete([FromBody]ICRUDModel<ProductIncoms> entity)
        {
            try
            {
               int result = data.SqlExecuteProc("SP_DeleteProductIncome", new { Id = Convert.ToInt32(entity.key) });
               return Json(entity.value);

            }
            catch (Exception ex)
            {
                return StatusCode(400, ex.Message);
            }
        }
        public IActionResult Setting([FromBody]DataManagerRequest dm)
        {
            IEnumerable<ProductIncoms> DataSource = data.SqlQuery<ProductIncoms>("select * from ProductIncoms order by Id desc");
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
            int count = DataSource.Cast<ProductIncoms>().Count();
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