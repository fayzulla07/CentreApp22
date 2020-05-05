using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using CentreApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlData.SqlGenerator;
using Syncfusion.EJ2.Base;

namespace CentreApp.Controllers
{
    [Authorize(Roles = "admin, seller")] // История продажи
    public class ProductReturnsController : Controller
    {
        ISqlData data;
        public ProductReturnsController(ISqlData data)
        {
            this.data = data;
        }
        // GET: Unit
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost] // возвращаем товар
        public ActionResult Insert(HistorySaleView entity)
        {
            try
            {
                double sales = data.SqlQuery<double>("select SaleCost from ProductSales where Id = @id", new { id = entity.Id }).FirstOrDefault();
                if (sales != 0)
                {
                    if (entity.SaleCost > sales)
                    {
                        return Ok("-1");
                    }
                }
                else
                {
                    return Ok("-1");
                }


                int usrid = 0;
                if (User.FindFirstValue("UserId") != null)
                    usrid = Convert.ToInt32(User.FindFirstValue("UserId"));
                int result = data.SqlExecuteProc("SP_AddProductReturn", new
                {
                    Id = 0,
                    Amount = entity.Amount,
                    Comments = entity.Comments,
                    ProductSaleId = entity.Id,
                    UserId = usrid,
                    RegDT = TimeZoneInfo.ConvertTime(DateTime.Now,
                     TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time"))
                });
                if (result > 0)
                    return Ok("1");
                else
                    return Ok("-1");
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
            
        }

        public ActionResult Delete([FromBody]ICRUDModel<HistorySaleView> entity)
        {
            if (User.IsInRole("admin"))
            {
                int result = data.SqlExecuteProc("SP_DeleteProductSale", new { Id = Convert.ToInt32(entity.key) });
                return Json(entity.value);
            }
            else
            {
                if (entity.key != null)
                {
                    var result = data.SqlQuery<HistorySaleView>("select * from HistorySaleView where Id = @myId", new { myId = entity.key }).FirstOrDefault();
                    if(result != null)
                    if (result.RegDt.Value.Date == DateTime.Now.Date)
                    {
                        data.SqlExecuteProc("SP_DeleteProductSale", new { Id = Convert.ToInt32(entity.key) });
                        return Json(entity.value);
                    }
                }
                
                
                return StatusCode(406, "Вы не можете удалить вчерашний запись");
            }
        }

        public IActionResult GetAll([FromBody]DataManagerRequest dm)
        {
            IEnumerable<HistorySaleView> DataSource = data.GetAll<HistorySaleView>();
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
            int count = DataSource.Cast<HistorySaleView>().Count();
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
        public IActionResult Setting([FromBody]DataManagerRequest dm)
        {
            IEnumerable<ProductReturns> DataSource = data.GetAll<ProductReturns>();
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
            int count = DataSource.Cast<ProductReturns>().Count();
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