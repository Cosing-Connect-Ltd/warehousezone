using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ganedata.Core.Entities.Domain;

namespace WMS.Helpers
{
    public class GaneOrderDetailsSessionHelper
    {
        public static void UpdateOrderDetailSession(string pageToken, OrderDetailSessionViewModel orderDetail, bool isGroupProducts = false, bool isAddForTransferOrder = false)
        {
            if (!string.IsNullOrEmpty(pageToken))
            {
                var orderDetailsDictionary = HttpContext.Current.Session["OrderDetailsSession"] as Dictionary<string, List<OrderDetailSessionViewModel>> ?? new Dictionary<string, List<OrderDetailSessionViewModel>>();
                var hasKey = orderDetailsDictionary.ContainsKey(pageToken);
                var existingDetailsList = new List<OrderDetailSessionViewModel>();
                if (hasKey)
                {
                    existingDetailsList = orderDetailsDictionary[pageToken];
                    var existingOrderDetail = existingDetailsList.FirstOrDefault(m => m.OrderDetailID == orderDetail.OrderDetailID);
                    if (existingOrderDetail == null && isGroupProducts)
                    {
                        existingOrderDetail = existingDetailsList.FirstOrDefault(m => m.ProductId == orderDetail.ProductId);
                    }
                    if (existingOrderDetail != null)
                    {
                        if (isGroupProducts)
                        {
                            if (orderDetail.OrderDetailID > 0)
                            {
                                existingOrderDetail.Qty = orderDetail.Qty;
                            }
                            else
                            {
                                if (existingOrderDetail != null && isAddForTransferOrder)
                                {
                                    existingOrderDetail.Qty += orderDetail.Qty;
                                }
                                else
                                {
                                    existingOrderDetail.Qty = orderDetail.Qty;
                                }
                            }
                        }
                        else
                        {
                            existingOrderDetail.Qty = orderDetail.Qty;
                            existingOrderDetail.ProductMaster.SKUCode = orderDetail.ProductMaster.SKUCode;
                            existingOrderDetail.CaseQuantity = orderDetail.CaseQuantity;
                            existingOrderDetail.Price = orderDetail.Price;
                            existingOrderDetail.ProductId = orderDetail.ProductId;
                            existingOrderDetail.AccountCode = orderDetail.AccountCode;
                            existingOrderDetail.Notes = orderDetail.Notes;
                            existingOrderDetail.TotalAmount = orderDetail.TotalAmount;
                            existingOrderDetail.TaxAmount = orderDetail.TaxAmount;
                            existingOrderDetail.TaxID = orderDetail.TaxID;
                            existingOrderDetail.WarrantyID = orderDetail.WarrantyID;
                            existingOrderDetail.WarrantyAmount = orderDetail.WarrantyAmount;
                            existingOrderDetail.ProductMaster.Name = orderDetail.ProductMaster.Name;
                            existingOrderDetail.TaxName = orderDetail.TaxName;


                        }
                        var index = existingDetailsList.IndexOf(existingOrderDetail);
                        existingDetailsList[index] = existingOrderDetail;
                    }
                    else
                    {
                        existingDetailsList.Add(orderDetail);
                    }

                    orderDetailsDictionary[pageToken] = existingDetailsList;
                }
                else
                {
                    existingDetailsList = new List<OrderDetailSessionViewModel> { orderDetail };
                    orderDetailsDictionary.Add(pageToken, existingDetailsList);
                }
                HttpContext.Current.Session["OrderDetailsSession"] = orderDetailsDictionary;
            }
        }

        public static void SetOrderDetailSessions(string pageToken, List<OrderDetailSessionViewModel> orderDetails)
        {
            if (!string.IsNullOrEmpty(pageToken))
            {
                var orderDetailsDictionary = HttpContext.Current.Session["OrderDetailsSession"] as Dictionary<string, List<OrderDetailSessionViewModel>> ?? new Dictionary<string, List<OrderDetailSessionViewModel>>();




                if (orderDetailsDictionary.ContainsKey(pageToken))
                {
                    var existingDetailsList = orderDetailsDictionary[pageToken];
                    if (existingDetailsList != null)
                    {
                        orderDetailsDictionary[pageToken] = orderDetails;
                    }
                    else
                    {
                        orderDetailsDictionary.Add(pageToken, orderDetails);
                    }
                }
                else
                {
                    orderDetailsDictionary.Add(pageToken, orderDetails);
                }
                HttpContext.Current.Session["OrderDetailsSession"] = orderDetailsDictionary;
            }
        }

        public static void RemoveOrderDetailSession(string pageToken, int orderDetailId = 0)
        {
            if (!string.IsNullOrEmpty(pageToken))
            {
                var orderDetailsDictionary = HttpContext.Current.Session["OrderDetailsSession"] as Dictionary<string, List<OrderDetailSessionViewModel>> ?? new Dictionary<string, List<OrderDetailSessionViewModel>>();

                if (orderDetailId == 0)
                {
                    orderDetailsDictionary.Remove(pageToken);
                    return;
                }

                var existingDetailsList = orderDetailsDictionary[pageToken];
                if (existingDetailsList != null)
                {
                    existingDetailsList.RemoveAll(m => m.OrderDetailID == orderDetailId);
                    orderDetailsDictionary[pageToken] = existingDetailsList;
                }
                HttpContext.Current.Session["OrderDetailsSession"] = orderDetailsDictionary;
            }
        }

        public static List<OrderDetailSessionViewModel> GetOrderDetailSession(string pageToken)
        {
            if (!string.IsNullOrEmpty(pageToken))
            {
                pageToken = pageToken.Substring(0, 36);

                var orderDetailsDictionary = HttpContext.Current.Session["OrderDetailsSession"] as Dictionary<string, List<OrderDetailSessionViewModel>> ?? new Dictionary<string, List<OrderDetailSessionViewModel>>();

                if (!orderDetailsDictionary.ContainsKey(pageToken)) return new List<OrderDetailSessionViewModel>();

                return orderDetailsDictionary[pageToken];
            }

            return new List<OrderDetailSessionViewModel>();
        }

        public static void ClearSessionTokenData(string pageSessionToken)
        {
            if (!string.IsNullOrEmpty(pageSessionToken))
            {
                var orderDetailsDictionary = HttpContext.Current.Session["OrderDetailsSession"] as Dictionary<string, List<OrderDetailSessionViewModel>> ?? new Dictionary<string, List<OrderDetailSessionViewModel>>();

                if (!orderDetailsDictionary.ContainsKey(pageSessionToken)) return;

                orderDetailsDictionary.Remove(pageSessionToken);

                HttpContext.Current.Session["OrderDetailsSession"] = orderDetailsDictionary;
            }

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