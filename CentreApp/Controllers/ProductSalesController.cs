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
    [Authorize(Roles = "admin, seller")]
    public class ProductSalesController : Controller
    {
        ISqlData data;
        public ProductSalesController(ISqlData data)
        {
            this.data = data;
        }
        // GET: Unit
        public ActionResult Index()
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
                    result.costs = data.SqlQuery<AvCurrentCosts>("select * from AvCurrentCosts where ProductId = @prodId", new { prodId = result.Id }).FirstOrDefault();
                    if(result.costs == null || result.RemainCount <= 0)
                    {
                        return Ok("Продукт не осталось!");
                    }
                    string unt = data.GetById<Models.Units>((int)result.UnitId).Name;
                    if(unt != null)
                    {
                        unt = result.RemainCount.ToString() + unt;
                    }
                    else
                    {
                        unt = result.RemainCount.ToString();
                    }
                    return Json(new { ProductId = result.Id, result.costs.OptCost, MySaleCost = result.costs.SaleCost, Amount = 1, 
                        result.costs.SaleCost, IsOptCost = false, RemainCount =  unt, 
                        RemainFloat = result.RemainCount, Volume = result.Volume, VolumeTotal = result.Volume * 1,
                        SaleCostTG = result.costs.SaleCost , ProductName = result.Name, OptCost2 = result.costs.OptCost
                    });
                }
                else
                {
                    return Ok("Такого продукта нет!");
                }
            }
            return Ok("Введите штрих код!");
        }
        [HttpPost]
        public ActionResult Insert([FromBody]ProductSales[] entity)
        {
            if(entity == null)
            {
                return Json("null");
            }
            int customerid;
            int lastorder = data.SqlQuery<int>("select * from LastOrderView").FirstOrDefault();
            if (lastorder <= 0)
                lastorder = 1;
            foreach (var item in entity)
            {
                if (User.FindFirstValue("UserId") != null)
                    item.UserId = Convert.ToInt32(User.FindFirstValue("UserId"));
                var dataa = data.GetById<Products>(item.Id);
                dataa.costs = data.SqlQuery<AvCurrentCosts>("select * from AvCurrentCosts where ProductId = @prodId", new { prodId = item.Id }).FirstOrDefault();
                if (dataa.costs != null)
                    item.IncomeCost = dataa.costs.IncomeCost;
                item.ProductId = item.Id;
                if(item.CustomerId == null || item.CustomerId <= 0)
                {
                    customerid = data.SqlQuery<int>("select TOP 1 Id from Customers").FirstOrDefault();
                    if (customerid > 0)
                        item.CustomerId = customerid;
                }
                item.OrderNumber = lastorder;
            }
            List<object> idi = new List<object> ();
            foreach (var item in entity)
            {
                int result = data.SqlExecuteProc("SP_AddProductSale", item);
                if (result < 0)
                {
                    string Name = "";
                    Name = data.GetById<Products>(item.Id).Name; 
                    idi.Add(new { Id= item.Id, Name });
                }
            }
            
            if(idi.Count == 0)
            return Json("Успешно сохранено!");
            else
            {
               return Json(idi);
            }

        }
        
        public ActionResult Update([FromBody]ICRUDModel<ProductSales> entity)
        {
            int result = data.Update<ProductSales>(entity.value);
            return Json(entity.value);
        }
        public ActionResult Delete([FromBody]ICRUDModel<ProductSales> entity)
        {
            int result = data.SqlExecuteProc("SP_DeleteProductSale", new { Id = Convert.ToInt32(entity.key) });
            return Json(entity.value);
        }
        public IActionResult ForDropDown([FromBody]DataManagerRequest dm)
        {
            IEnumerable<ProductSales> DataSource = data.GetAll<ProductSales>();

            DataOperations operation = new DataOperations();
            if (dm.Search != null && dm.Search.Count > 0)
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);  // Search
            }
            if (dm.Where != null && dm.Where.Count > 0) // Filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, dm.Where[0].Operator);
            }
            int count = DataSource.Cast<ProductSales>().Count();
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);
        }
        public IActionResult Setting([FromBody]DataManagerRequest dm)
        {
            IEnumerable<ProductSales> DataSource = data.GetAll<ProductSales>();
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
            int count = DataSource.Cast<ProductSales>().Count();
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