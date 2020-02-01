using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Helpers
{
    public class GaneOrderNotesSessionHelper
    {
        public static Dictionary<string, List<OrderNotes>> OrderNotes { get; set; }

        public static OrderNotes UpdateOrderNotesSession(string pageToken, OrderNotes model)
        {
            if (!string.IsNullOrEmpty(pageToken))
            {
                var dList = HttpContext.Current.Session["OrderNotesSession"] as Dictionary<string, List<OrderNotes>>;

                if (dList == null)
                {
                    dList = new Dictionary<string, List<OrderNotes>>();
                }

                if (!dList.ContainsKey(pageToken))
                {
                    dList.Add(pageToken, new List<OrderNotes>(){  });
                }

                var nList = dList[pageToken];

                var addRequired = model.OrderNoteId == 0;
                if (model.OrderID > 0)
                {
                    var prevNoteId = model.OrderNoteId;
                    if (model.OrderNoteId > 0)
                    {
                        var item = nList.FirstOrDefault(m => m.OrderNoteId == prevNoteId);
                        if (item == null)
                        {
                            nList.Add(model);
                        }
                        else
                        {
                            var index = nList.IndexOf(item);
                            nList[index].OrderNoteId = model.OrderNoteId;
                            nList[index].Notes = model.Notes;
                            nList[index].CreatedBy = model.CreatedBy;
                            nList[index].DateCreated = model.DateCreated;
                        }
                    }
                }
                else
                {
                    var item = nList.FirstOrDefault(m => m.OrderNoteId == model.OrderNoteId);

                    if (addRequired || item == null)
                    {
                        model.OrderNoteId = !nList.Any() ? -1 : nList.Min(m => m.OrderNoteId) - 1;
                        if (model.OrderNoteId > 0)
                        {
                            model.OrderNoteId = -1;
                        }
                        nList.Add(model);
                    }
                    else
                    {
                        var index = nList.IndexOf(item);
                        nList[index].Notes = model.Notes;
                        nList[index].CreatedBy = model.CreatedBy;
                        nList[index].DateCreated = model.DateCreated;
                    }
                }
                HttpContext.Current.Session["OrderNotesSession"] = dList;
            }

            return new OrderNotes() { Notes = model.Notes, OrderNoteId = model.OrderNoteId, OrderID = model.OrderID };
        }

        public static void SetOrderNotesSessions(string pageToken, List<OrderNotes> orderNotes)
        {
            if (!string.IsNullOrEmpty(pageToken))
            {
                var orderNotesDictionary = HttpContext.Current.Session["OrderNotesSession"] as Dictionary<string, List<OrderNotes>> ?? new Dictionary<string, List<OrderNotes>>();
                if (orderNotesDictionary.ContainsKey(pageToken))
                {
                    var existingDetailsList = orderNotesDictionary[pageToken];
                    if (existingDetailsList != null)
                    {
                        orderNotesDictionary[pageToken] = orderNotes;
                    }
                    else
                    {
                        orderNotesDictionary.Add(pageToken, orderNotes);
                    }
                }
                else
                {
                    orderNotesDictionary.Add(pageToken, orderNotes);
                }
                HttpContext.Current.Session["OrderNotesSession"] = orderNotesDictionary;
            }
        }

        public static void RemoveOrderNotesSession(string pageToken, int orderNotesId = 0)
        {
            if (!string.IsNullOrEmpty(pageToken))
            {
                var orderNotesDictionary = HttpContext.Current.Session["OrderNotesSession"] as Dictionary<string, List<OrderNotes>> ?? new Dictionary<string, List<OrderNotes>>();

                if (orderNotesId == 0)
                {
                    orderNotesDictionary.Remove(pageToken);
                    return;
                }

                var existingDetailsList = orderNotesDictionary[pageToken];
                if (existingDetailsList != null)
                {
                    existingDetailsList.RemoveAll(m => m.OrderNoteId == orderNotesId);
                    orderNotesDictionary[pageToken] = existingDetailsList;
                }
                HttpContext.Current.Session["OrderNotesSession"] = orderNotesDictionary;
            }
        }

        public static List<OrderNotes> GetOrderNotesSession(string pageToken)
        {
            if (!string.IsNullOrEmpty(pageToken))
            {
                var orderNotesDictionary = HttpContext.Current.Session["OrderNotesSession"] as Dictionary<string, List<OrderNotes>> ?? new Dictionary<string, List<OrderNotes>>();

                if (!orderNotesDictionary.ContainsKey(pageToken)) return new List<OrderNotes>();

                return orderNotesDictionary[pageToken];
            }

            return new List<OrderNotes>();
        }

        public static void ClearSessionTokenData(string pageSessionToken)
        {
            if (!string.IsNullOrEmpty(pageSessionToken))
            {
                var orderNotesDictionary =
                    HttpContext.Current.Session["OrderNotesSession"] as Dictionary<string, List<OrderNotes>> ??
                    new Dictionary<string, List<OrderNotes>>();

                orderNotesDictionary.Remove(pageSessionToken);

                HttpContext.Current.Session["OrderNotesSession"] = orderNotesDictionary;
            }

        }
    }
}