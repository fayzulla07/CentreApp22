using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CentreApp.Models
{
    public class Customers
    {
        [Key]
        public int Id { get; set; } // int
        public string Name { get; set; } // nvarchar(200)
        public string Address { get; set; } // nvarchar(300)
        public string Email { get; set; } // nvarchar(150)
        public double? GeoLatitude { get; set; } // float
        public double? GeoLongitude { get; set; } // float
        public double? GeoAltitude { get; set; } // float
        public string PhoneNumber { get; set; } // nvarchar(100)
        public int? CustomerTypeId { get; set; } // int
    }

    public class CustomerTypes
    {
        [Key]
        public int Id { get; set; } // int
        public string Name { get; set; } // nvarchar(200)
        public string Description { get; set; } // nvarchar(350)
    }

    public class MyCompany
    {
        [Key]
        public int Id { get; set; } // int
        public string Name { get; set; } // nvarchar(200)
        public string Description { get; set; } // nvarchar(300)
        public int? ParentId { get; set; } // int
        public int? Level { get; set; } // int
    }

    public class ProductIncoms
    {
        [Key]
        public int Id { get; set; } // int

        [Required (ErrorMessage = "Выберите продукт")] 
        public int ProductId { get; set; } // int

        [Required(ErrorMessage = "введите количество")]
        [Range(0, double.MaxValue, ErrorMessage = "введите количество корректно!")]
        public double Amount { get; set; } // float

        [Required(ErrorMessage = "Это поле обязательно!")]
        [Range(0, double.MaxValue, ErrorMessage = "введите цену корректно!")]
        public double IncomeCost { get; set; } // float

        [Required(ErrorMessage = "Это поле обязательно!")]
        [Range(0, double.MaxValue, ErrorMessage = "введите цену корректно!")]
        public double SaleCost { get; set; } // float

        [Required(ErrorMessage = "Это поле обязательно!")]
        [Range(0, double.MaxValue, ErrorMessage = "введите цену корректно!")]
        public double OptCost { get; set; } // float
        public DateTime? ProductionDt { get; set; } // datetime

        public int? SupplierId { get; set; } // int
        public DateTime? RegDt { get; set; } = TimeZoneInfo.ConvertTime(DateTime.Now,
                 TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time")); // datetime
        public int? UserId { get; set; } // int
        public string Comments { get; set; } // nvarchar(300)
        public double Kurs { get; set; } // nvarchar(300)
    }

    public class ProductReturns
    {
        [Key]
        public int Id { get; set; } // int
        public int ProductSaleId { get; set; } // int
        public double Amount { get; set; } // float
        public double ReturnCost { get; set; } // float
        public DateTime RegDt { get; set; } = TimeZoneInfo.ConvertTime(DateTime.Now,
                 TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time")); // datetime
        public int? UserId { get; set; } // int
        public string Comments { get; set; } // nvarchar(300)
    }

    public class Products
    {
        [Key]
        public int Id { get; set; } // int

        [Required(ErrorMessage = "Это поле обязательно!")]
        public string Name { get; set; } // nvarchar(200)
        public string Code { get; set; } // nvarchar(60)
        public string Description { get; set; } // nvarchar(350)
        public int? ProductTypeId { get; set; } // int
        public double RemainCount { get; set; } // float
        public int? UnitId { get; set; } // int
        public int? Limit { get; set; } // int

        public double? Volume { get; set; } 
        public double? Amount { get; set; } // расчетная поля

        public AvCurrentCosts costs { get; set; }
    }

    public class ProductSales
    {
        [Key]
        public int Id { get; set; } // int
        public int ProductId { get; set; } // int

        [Required(ErrorMessage = "Это поле обязательно!")]
        [Range(0, double.MaxValue, ErrorMessage = "Значение должно быть больше нуля")]
        public double Amount { get; set; } // float
        public double SaleCost { get; set; } // float
        public double IncomeCost { get; set; } // float
        public bool? IsOptCost { get; set; } // bit
        public int? CustomerId { get; set; } // int
        public DateTime? RegDt { get; set; } = TimeZoneInfo.ConvertTime(DateTime.Now,
                 TimeZoneInfo.FindSystemTimeZoneById("Central Asia Standard Time")); // datetime
        public int? UserId { get; set; } // int
        public string Comments { get; set; } // nvarchar(300)
        public int OrderNumber { get; set; } // nvarchar(300)
        public bool IsBank { get; set; } // nvarchar(300)
        public double Kurs { get; set; } // nvarchar(300)
    }

    public class ProductTypes
    {
        [Key]
        public int Id { get; set; } // int

        [Required(ErrorMessage = "Это поле обязательно!")]
        public string Name { get; set; } // nvarchar(200)
        public string Description { get; set; } // nvarchar(350)
    }

    public class Suppliers
    {
        [Key]
        public int Id { get; set; } // int

        [Required(ErrorMessage = "Это поле обязательно!")]
        public string Name { get; set; } // nvarchar(200)
        public string Description { get; set; } // nvarchar(300)
        public string PhoneNumber { get; set; } // nvarchar(100)
        public string Email { get; set; } // nvarchar(120)
        public double? HisDebt { get; set; } // float
        public double? MyDebt { get; set; } // float
    }

    public class Units
    {
        [Key]
        public int Id { get; set; } // int
        [Required(ErrorMessage = "Это поле обязательно!")]
        public string Name { get; set; } // nvarchar(200)
        public string Description { get; set; } // nvarchar(300)
    }

    public class Roles
    {
        [Key]
        public int Id { get; set; } // int
        public string Name { get; set; } // nvarchar(150)
        public string Description { get; set; } // nvarchar(250)
    }

    public class Users
    {
        [Key]
        public int Id { get; set; } // int
        public string Name { get; set; } // nvarchar(200)
        public string Description { get; set; } // nvarchar(300)

        [Required(ErrorMessage = "Это поле обязательно!")]
        public string LoginName { get; set; } // nvarchar(150)

        [Required(ErrorMessage = "Это поле обязательно!")]
        public string Password { get; set; } // nvarchar(350)

        public int? RoleId { get; set; } // int

        public Roles roles { get; set; }
    }
    // -------------------------------------Views----------------------------------------//
    public class AvCurrentCosts
    {
        [Key]
        public int ProductIdId { get; set; }
        public double IncomeCost { get; set; }
        public double OptCost { get; set; }
        public double SaleCost { get; set; }
    }
    public class HistorySaleView
    {
        [Key]
        public int Id { get; set; } // int
        public string Name { get; set; } // nvarchar(200)
        public double Amount { get; set; } // float
        public string AmountUnit { get; set; } // nvarchar(211)
        public double SaleCost { get; set; } // float
        public string Client { get; set; } // nvarchar(200)
        public string OptCost { get; set; } // varchar(3)
        public DateTime? RegDt { get; set; } // datetime
        public int? OrderNumber { get; set; } // int
        public string Comments { get; set; } // nvarchar(300)
        public bool IsBank { get; set; } // nvarchar(300)
        public string UserName { get; set; } // nvarchar(300)
    }
    public class ReturnView
    {
        [Key]
        public int Id { get; set; } // int
        public string Name { get; set; } // nvarchar(200)
        public double ReturnCost { get; set; } // float
        public string AmountUnit { get; set; } // nvarchar(226)
        public DateTime RegDt { get; set; } // datetime
        public string CustomerName { get; set; } // nvarchar(200)
        public string Comments { get; set; } // nvarchar(200)
    }
    public class SalesTotalByDayView
    {
        public DateTime RegDt { get; set; } // datetime
        public double IncomeCost { get; set; } // nvarchar(200)
        public double SaleTotal { get; set; } // nvarchar(200)
    }
    public class ReturnTotalByDayView
    {
        public DateTime RegDt { get; set; } // datetime
        public double ReturnCost { get; set; } // nvarchar(200)
    }

    //tempdata
    public class ProductIncomeCostModel
    {
        public double IncomeCost { get; set; } // nvarchar(200)
        public double SaleCost { get; set; } // nvarchar(200)
        public double OptCost { get; set; } // nvarchar(200)
        public int ProductTypeId { get; set; } // nvarchar(200)
    }
}
