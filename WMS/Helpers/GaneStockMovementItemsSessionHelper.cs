using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ganedata.Core.Entities.Domain;

namespace WarehouseEcommerce.Helpers
{
    public class GaneStockMovementItemsSessionHelper
    {
        public static void UpdateStockMovementItemsSession(StockMovementViewModel stockMovement, bool isGroupProducts = false, bool isAddForTransferOrder = false)
        {
            var stockmovementDictionary = HttpContext.Current.Session["StockMovementSession"] as List<StockMovementViewModel> ?? new List<StockMovementViewModel>();
            var hasKey = stockmovementDictionary.Where(u => u.ProductId == stockMovement.ProductId).ToList();
            var existingstockMovementList = new List<StockMovementViewModel>();
            if (hasKey.Count > 0)
            {
                existingstockMovementList = hasKey;
                isGroupProducts = true;
                var stockMovementDetail = existingstockMovementList.FirstOrDefault(m => m.ProductId == stockMovement.ProductId);
                if (stockMovementDetail == null && isGroupProducts)
                {
                    stockMovementDetail = existingstockMovementList.FirstOrDefault(m => m.ProductId == stockMovement.ProductId);
                }
                if (stockMovementDetail != null)
                {
                    if (isGroupProducts)
                    {
                        if (stockMovement.ProductId > 0)
                        {
                            stockMovementDetail.Qty = stockMovement.Qty;

                        }
                        else
                        {
                            if (stockMovementDetail != null && isAddForTransferOrder)
                            {
                                stockMovementDetail.Qty += stockMovement.Qty;
                            }
                            else
                            {
                                stockMovementDetail.Qty = stockMovement.Qty;
                            }
                        }
                    }
                    else
                    {
                        stockMovementDetail.Qty = stockMovement.Qty;
                        stockMovementDetail.ProductId = stockMovement.ProductId;
                        stockMovementDetail.FromLocation = stockMovement.FromLocation;
                        stockMovementDetail.ToLocation = stockMovement.ToLocation;
                        stockMovementDetail.WarehouseId = stockMovement.WarehouseId;
                        stockMovementDetail.TenentId = stockMovement.TenentId;
                        stockMovementDetail.UserId = stockMovement.UserId;
                        stockMovement.ToLocationName = stockMovement.ToLocationName;
                        stockMovement.ProductName = stockMovement.ProductName;
                        stockMovement.FromLocationName = stockMovement.FromLocationName;

                    }
                    var index = existingstockMovementList.IndexOf(stockMovementDetail);
                    existingstockMovementList[index] = stockMovementDetail;
                }
                else
                {
                    existingstockMovementList.Add(stockMovementDetail);
                }

                //orderDetailsDictionary.AddRange(existingDetailsList);
            }
            else
            {
                existingstockMovementList = new List<StockMovementViewModel> { stockMovement };
                stockmovementDictionary.AddRange(existingstockMovementList);
            }
            HttpContext.Current.Session["StockMovementSession"] = stockmovementDictionary;
        }

        public static void SetStockMovementSessions(string pageToken, List<StockMovementViewModel> stockMovements)
        {
            if (!string.IsNullOrEmpty(pageToken))
            {
                var stockMovementsDictionary = HttpContext.Current.Session["StockMovementSession"] as Dictionary<string, List<StockMovementViewModel>> ?? new Dictionary<string, List<StockMovementViewModel>>();

                if (stockMovementsDictionary.ContainsKey(pageToken))
                {
                    var existingStockMovementList = stockMovementsDictionary[pageToken];
                    if (existingStockMovementList != null)
                    {
                        stockMovementsDictionary[pageToken] = stockMovements;
                    }
                    else
                    {
                        stockMovementsDictionary.Add(pageToken, stockMovements);
                    }
                }
                else
                {
                    stockMovementsDictionary.Add(pageToken, stockMovements);
                }
                HttpContext.Current.Session["StockMovementSession"] = stockMovementsDictionary;
            }
        }

        public static void RemoveStockMovementItemSession(int productId, int orderDetailId = 0)
        {
            var stockmovementDictionary = HttpContext.Current.Session["StockMovementSession"] as List<StockMovementViewModel> ?? new List<StockMovementViewModel>();
            var existingDetailsList = stockmovementDictionary.Where(u => u.ProductId == productId).ToList();
            if (existingDetailsList != null)
            {
                stockmovementDictionary.RemoveAll(m => m.ProductId == productId);

            }
            HttpContext.Current.Session["StockMovementSession"] = stockmovementDictionary;

        }

        public static List<StockMovementViewModel> GetStockMovementsSession()
        {
            var stockmovementDictionary = HttpContext.Current.Session["StockMovementSession"] as List<StockMovementViewModel> ?? new List<StockMovementViewModel>();
            if (stockmovementDictionary.Count < 0)
            {
                return new List<StockMovementViewModel>();
            }
            else
            {
                return stockmovementDictionary;
            }

        }

        public static void ClearSessionStockMovement()
        {
            HttpContext.Current.Session["StockMovementSession"] = null;
        }

    

    public static bool IsDictionaryContainsKey(string pageSessionToken)
    {
        var result = false;

        var orderDetailsDictionary = HttpContext.Current.Session["OrderDetailsSession"] as Dictionary<string, List<OrderDetailSessionViewModel>> ?? new Dictionary<string, List<OrderDetailSessionViewModel>>();

        if (orderDetailsDictionary.ContainsKey(pageSessionToken)) result = true;


        return result;
    }

}
}