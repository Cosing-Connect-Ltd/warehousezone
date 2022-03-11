using Ganedata.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public static class CCPECustomBindings
    {
        public static List<CCEPDataModel> GetDataForCCPE(DateTime startDate, DateTime endDate)
        {
            var db = DependencyResolver.Current.GetService<IApplicationContext>();
            var query = "select  case when[AccountSectorId] = 6 then 'CCEP' else 'Other' end 'Account_Type' ,[ProductCategoryName],case when (case when[IssueDate] is null AND c.[ExpectedDate] is null then month(a.[DateCreated]) when[IssueDate] is null then month(c.[ExpectedDate])" +
                " else month([IssueDate]) end) in (1, 2, 3) then 'Q1' when (case when[IssueDate] is null AND c.[ExpectedDate] is null then month(a.[DateCreated]) when[IssueDate] is null then month(c.[ExpectedDate]) else month([IssueDate]) end) in (4, 5, 6) then 'Q2' when (case when[IssueDate] is null AND c.[ExpectedDate] is null then month(a.[DateCreated]) " +
                " when[IssueDate] is null then month(c.[ExpectedDate]) else month([IssueDate]) end) in (7, 8, 9) then 'Q3'" +
                " when (case when[IssueDate] is null AND c.[ExpectedDate] is null then month(a.[DateCreated]) when[IssueDate] is null then month(c.[ExpectedDate]) else month([IssueDate]) end) in (10, 11, 12) then 'Q4' end 'Q' " +
                ",concat(case when[IssueDate] is null AND c.[ExpectedDate] is null then year(a.[DateCreated]) when[IssueDate] is null then year(c.[ExpectedDate]) else year([IssueDate]) end, ' - ' , case when[IssueDate] is null AND c.[ExpectedDate] is null then month(a.[DateCreated]) when[IssueDate] is null then month(c.[ExpectedDate]) else month([IssueDate]) end) " +
                "'Year_Month' ,concat(d.[AccountCode], ' - ', d.[CompanyName]) 'Customer_Name' ,d.AccountID 'Customer_AccNo' ,[ShipmentAddressLine1] 'Address1' ,[ShipmentAddressLine2] 'Address2' ,null 'City' ,[ShipmentAddressPostcode] 'Postcode' ,b.BarCode 'EAN' ,null 'Customer_Product_Code' " +
                ",b.[Name] 'Cusomter_Product_Description' ,null 'Brand' ,null 'Pack_Size' ,null 'Pack_Type' ,null 'Pack_Config' ,a.[Qty] 'Case_Volume' ,null 'Sales_Value' ,case when[IssueDate] is null AND c.[ExpectedDate] is null then a.[DateCreated] when[IssueDate] is null then c.[ExpectedDate] else[IssueDate] end 'Issue_Date',c.InvoiceNo  " +
                "from [OrderDetails] a with(nolock) left outer join [ProductMaster] b with(nolock) " +
                "on a.ProductId = b.ProductId left outer join [Orders] c with(nolock) on a.OrderID = c.OrderID left outer join [Account] d with(nolock) " +
                "on c.[AccountId] = d.AccountID and a.[IsDeleted] is null inner join [ProductCategories] e with(nolock) on b.ProductCategoryId = e.[ProductCategoryId] where a.[IsDeleted] is null and " +
                "left([OrderNumber],2) = 'SO' and[AccountSectorId] = 6 and c.InvoiceNo is not null and a.DateCreated >= '" + startDate.ToString("MM/dd/yyyy") + "' and a.DateCreated<='" + endDate.ToString("MM/dd/yyyy") + "'";
            var CCepData = db.Database.SqlQuery<CCEPDataModel>(query).ToList();
            return CCepData;



        }
    }

    public class CCEPDataModel
    {

        public string Account_Type { get; set; }

        public string ProductCategoryName { get; set; }

        public string Q { get; set; }

        public string Year_Month { get; set; }
        public string Customer_Name { get; set; }
        public int Customer_AccNo { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }

        public string Postcode { get; set; }

        public string EAN { get; set; }

        public string Customer_Product_Code { get; set; }

        public string Cusomter_Product_Description { get; set; }

        public string Brand { get; set; }

        public string Pack_Size { get; set; }

        public string Pack_Type { get; set; }

        public string Pack_Config { get; set; }

        public decimal Case_Volume { get; set; }
        public string Sales_Value { get; set; }
        public string InvoiceNo { get; set; }
        

        public DateTime Issue_Date { get; set; }


    }
}