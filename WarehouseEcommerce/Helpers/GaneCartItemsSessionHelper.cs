using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ganedata.Core.Entities.Domain;

namespace WarehouseEcommerce.Helpers
{
    public class GaneCartItemsSessionHelper
    {
        public static void UpdateCartItemsSession(string pageToken, OrderDetailSessionViewModel orderDetail, bool isGroupProducts = false, bool isAddForTransferOrder = false)
        {
            var orderDetailsDictionary = HttpContext.Current.Session["CartItemsSession"] as List<OrderDetailSessionViewModel> ?? new List<OrderDetailSessionViewModel>();
            var hasKey = orderDetailsDictionary.Where(u=>u.ProductId==orderDetail.ProductId).ToList();
            var existingDetailsList = new List<OrderDetailSessionViewModel>();
            if (hasKey.Count>0)
            {
                existingDetailsList = hasKey;
                isGroupProducts = true;
                var existingOrderDetail = existingDetailsList.FirstOrDefault(m => m.ProductId == orderDetail.ProductId);
                if (existingOrderDetail == null && isGroupProducts)
                {
                    existingOrderDetail = existingDetailsList.FirstOrDefault(m => m.ProductId == orderDetail.ProductId);
                }
                if (existingOrderDetail != null)
                {
                    if (isGroupProducts)
                    {
                        if (orderDetail.ProductId > 0)
                        {
                            existingOrderDetail.Qty = orderDetail.Qty;
                            existingOrderDetail.TotalAmount = Math.Round((existingOrderDetail.Qty * existingOrderDetail.Price),2);
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

                //orderDetailsDictionary.AddRange(existingDetailsList);
            }
            else
            {
                orderDetail.TotalAmount = (orderDetail.Qty * orderDetail.Price);
                existingDetailsList = new List<OrderDetailSessionViewModel> { orderDetail };
                orderDetailsDictionary.AddRange(existingDetailsList);
            }
            HttpContext.Current.Session["CartItemsSession"] = orderDetailsDictionary;
        }

        public static void RemoveCartItemSession(int productId, int orderDetailId = 0)
        {
                var orderDetailsDictionary = HttpContext.Current.Session["CartItemsSession"] as List<OrderDetailSessionViewModel> ?? new List<OrderDetailSessionViewModel>();
                var existingDetailsList = orderDetailsDictionary.Where(u=>u.ProductId==productId).ToList();
                if (existingDetailsList != null)
                {
                 orderDetailsDictionary.RemoveAll(m => m.ProductId == productId);
                    
                }
                HttpContext.Current.Session["CartItemsSession"] = orderDetailsDictionary;
            
        }

        public static List<OrderDetailSessionViewModel> GetCartItemsSession()
        {
            var orderDetailsDictionary = HttpContext.Current.Session["CartItemsSession"] as  List<OrderDetailSessionViewModel> ?? new List<OrderDetailSessionViewModel>();
        if (orderDetailsDictionary.Count < 0)
        {
            return new List<OrderDetailSessionViewModel>();
        }
        else
        {
            return orderDetailsDictionary;
        }

        }

    }
    public class GaneWishListItemsSessionHelper
    {
        public static void UpdateWishListItemsSession(string pageToken, OrderDetailSessionViewModel orderDetail, bool isGroupProducts = false, bool isAddForTransferOrder = false)
        {
            var orderDetailsDictionary = HttpContext.Current.Session["WishListItemsSession"] as List<OrderDetailSessionViewModel> ?? new List<OrderDetailSessionViewModel>();
            var hasKey = orderDetailsDictionary.Where(u => u.ProductId == orderDetail.ProductId).ToList();
            var existingDetailsList = new List<OrderDetailSessionViewModel>();
            if (hasKey.Count > 0)
            {
                existingDetailsList = hasKey;
                isGroupProducts = true;
                var existingOrderDetail = existingDetailsList.FirstOrDefault(m => m.ProductId == orderDetail.ProductId);
                if (existingOrderDetail == null && isGroupProducts)
                {
                    existingOrderDetail = existingDetailsList.FirstOrDefault(m => m.ProductId == orderDetail.ProductId);
                }
                if (existingOrderDetail != null)
                {
                    if (isGroupProducts)
                    {
                        if (orderDetail.ProductId > 0)
                        {
                            existingOrderDetail.Qty = orderDetail.Qty;
                            existingOrderDetail.TotalAmount = Math.Round((existingOrderDetail.Qty * existingOrderDetail.Price), 2);
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

                //orderDetailsDictionary.AddRange(existingDetailsList);
            }
            else
            {
                orderDetail.TotalAmount = (orderDetail.Qty * orderDetail.Price);
                existingDetailsList = new List<OrderDetailSessionViewModel> { orderDetail };
                orderDetailsDictionary.AddRange(existingDetailsList);
            }
            HttpContext.Current.Session["WishListItemsSession"] = orderDetailsDictionary;
        }
        public static void RemoveWishListSession(int productId)
        {
            var orderDetailsDictionary = HttpContext.Current.Session["WishListItemsSession"] as List<OrderDetailSessionViewModel> ?? new List<OrderDetailSessionViewModel>();
            var existingDetailsList = orderDetailsDictionary.Where(u => u.ProductId == productId).ToList();
            if (existingDetailsList != null)
            {
                orderDetailsDictionary.RemoveAll(m => m.ProductId == productId);

            }
            HttpContext.Current.Session["WishListItemsSession"] = orderDetailsDictionary;

        }

        public static List<OrderDetailSessionViewModel> GetWishListItemsSession()
        {
            var orderDetailsDictionary = HttpContext.Current.Session["WishListItemsSession"] as List<OrderDetailSessionViewModel> ?? new List<OrderDetailSessionViewModel>();
            if (orderDetailsDictionary.Count < 0)
            {
                return new List<OrderDetailSessionViewModel>();
            }
            else
            {
                return orderDetailsDictionary;
            }

        }
    }
}