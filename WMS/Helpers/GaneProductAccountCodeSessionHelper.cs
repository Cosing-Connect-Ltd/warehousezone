using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Helpers
{
    public class GaneProductAccountCodeSessionHelper
    {
        public static Dictionary<string, List<ProductAccountCodes>> ProductAccountCode { get; set; }

        public static ProductAccountCodes UpdateProductAccountCodesSession(string pageToken, ProductAccountCodes model)
        {
            if (!string.IsNullOrEmpty(pageToken))
            {
                var dList = HttpContext.Current.Session["ProductAccountCodesSession"] as Dictionary<string, List<ProductAccountCodes>>;

                if (dList == null)
                {
                    dList = new Dictionary<string, List<ProductAccountCodes>>();
                }

                if (!dList.ContainsKey(pageToken))
                {
                    dList.Add(pageToken, new List<ProductAccountCodes>(){  });
                }

                var nList = dList[pageToken];

                var addRequired = model.AccountID == 0;
                if (model.AccountID > 0)
                {
                    var prevAccountId = model.AccountID;
                    if (model.AccountID > 0)
                    {
                        var item = nList.FirstOrDefault(m => m.AccountID == prevAccountId);
                        if (item == null)
                        {
                            nList.Add(model);
                        }
                        else
                        {
                            var index = nList.IndexOf(item);
                            nList[index].AccountID = model.AccountID;
                            nList[index].ProdAccCode = model.ProdAccCode;
                            nList[index].ProdDeliveryType = model.ProdDeliveryType;
                            nList[index].ProdOrderingNotes = model.ProdOrderingNotes;
                            nList[index].CreatedBy = model.CreatedBy;
                            nList[index].DateCreated = model.DateCreated;
                        }
                    }
                }
                else
                {
                    var item = nList.FirstOrDefault(m => m.AccountID == model.AccountID);

                    if (addRequired || item == null)
                    {
                        model.AccountID = !nList.Any() ? -1 : nList.Min(m => m.AccountID) - 1;
                        if (model.AccountID > 0)
                        {
                            model.AccountID = -1;
                        }
                        nList.Add(model);
                    }
                    else
                    {
                        var index = nList.IndexOf(item);
                        nList[index].ProdAccCode = model.ProdAccCode;
                        nList[index].ProdDeliveryType = model.ProdDeliveryType;
                        nList[index].ProdOrderingNotes = model.ProdOrderingNotes;
                        nList[index].CreatedBy = model.CreatedBy;
                        nList[index].DateCreated = model.DateCreated;
                    }
                }
                HttpContext.Current.Session["ProductAccountCodesSession"] = dList;
            }

            return new ProductAccountCodes() { ProdAccCode = model.ProdAccCode, AccountID = model.AccountID};
        }

        public static void SetProductAccountCodesSession(string pageToken, List<ProductAccountCodes> ProductAccountCode)
        {
            if (!string.IsNullOrEmpty(pageToken))
            {
                var ProductAccountCodesDictionary = HttpContext.Current.Session["ProductAccountCodesSession"] as Dictionary<string, List<ProductAccountCodes>> ?? new Dictionary<string, List<ProductAccountCodes>>();
                if (ProductAccountCodesDictionary.ContainsKey(pageToken))
                {
                    var existingDetailsList = ProductAccountCodesDictionary[pageToken];
                    if (existingDetailsList != null)
                    {
                        ProductAccountCodesDictionary[pageToken] = ProductAccountCode;
                    }
                    else
                    {
                        ProductAccountCodesDictionary.Add(pageToken, ProductAccountCode);
                    }
                }
                else
                {
                    ProductAccountCodesDictionary.Add(pageToken, ProductAccountCode);
                }
                HttpContext.Current.Session["ProductAccountCodesSession"] = ProductAccountCodesDictionary;
            }
        }

        public static void RemoveProductAccountCodesSession(string pageToken, int AccountId = 0)
        {
            if (!string.IsNullOrEmpty(pageToken))
            {
                var ProductAccountCodesDictionary = HttpContext.Current.Session["ProductAccountCodesSession"] as Dictionary<string, List<ProductAccountCodes>> ?? new Dictionary<string, List<ProductAccountCodes>>();

                if (AccountId == 0)
                {
                    ProductAccountCodesDictionary.Remove(pageToken);
                    return;
                }

                var existingDetailsList = ProductAccountCodesDictionary[pageToken];
                if (existingDetailsList != null)
                {
                    existingDetailsList.RemoveAll(m => m.AccountID == AccountId);
                    ProductAccountCodesDictionary[pageToken] = existingDetailsList;
                }
                HttpContext.Current.Session["ProductAccountCodesSession"] = ProductAccountCodesDictionary;
            }
        }

        public static List<ProductAccountCodes> GetProductAccountCodesSession(string pageToken)
        {
            if (!string.IsNullOrEmpty(pageToken))
            {
                var ProductAccountCodesDictionary = HttpContext.Current.Session["ProductAccountCodesSession"] as Dictionary<string, List<ProductAccountCodes>> ?? new Dictionary<string, List<ProductAccountCodes>>();

                if (!ProductAccountCodesDictionary.ContainsKey(pageToken)) return new List<ProductAccountCodes>();

                return ProductAccountCodesDictionary[pageToken];
            }

            return new List<ProductAccountCodes>();
        }

        public static void ClearSessionTokenData(string pageSessionToken)
        {
            if (!string.IsNullOrEmpty(pageSessionToken))
            {
                var ProductAccountCodesDictionary =
                    HttpContext.Current.Session["ProductAccountCodesSession"] as Dictionary<string, List<ProductAccountCodes>> ??
                    new Dictionary<string, List<ProductAccountCodes>>();

                ProductAccountCodesDictionary.Remove(pageSessionToken);

                HttpContext.Current.Session["ProductAccountCodesSession"] = ProductAccountCodesDictionary;
            }

        }
    }
}