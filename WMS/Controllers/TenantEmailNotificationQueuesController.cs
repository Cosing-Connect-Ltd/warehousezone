using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class TenantEmailNotificationQueuesController : BaseController
    {

        // GET: TenantEmailNotificationQueues

        public TenantEmailNotificationQueuesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IActivityServices activityServices)
       : base(orderService, propertyService, accountServices, lookupServices)
        {
            
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult _NotificationQueue()
        {
            var viewModel = GridViewExtension.GetViewModel("NotificationQueueGridView");

            if (viewModel == null)
            {
                viewModel = TEmailNotifcationQueueCustomBinding.CreateNotificationQueueGridViewModel();
            }
            return NotificationQueueGridActionCore(viewModel);
        }
        public ActionResult _NotificationQueueListPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("NotificationQueueGridView");
            viewModel.Pager.Assign(pager);
            return NotificationQueueGridActionCore(viewModel);
        }

        public ActionResult _NotificationQueueFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("NotificationQueueGridView");
            viewModel.ApplyFilteringState(filteringState);
            return NotificationQueueGridActionCore(viewModel);
        }


        public ActionResult _NotificationQueueGetDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("NotificationQueueGridView");
            viewModel.ApplySortingState(column, reset);
            return NotificationQueueGridActionCore(viewModel);
        }



        public ActionResult NotificationQueueGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    TEmailNotifcationQueueCustomBinding.NotificationQueueGetDataRowCount(args, CurrentTenantId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        TEmailNotifcationQueueCustomBinding.NotificationQueueGetData(args, CurrentTenantId);
                    })
            );
            return PartialView("_NotificationQueueList", gridViewModel);
        }

        //// GET: TenantEmailNotificationQueues/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TenantEmailNotificationQueue tenantEmailNotificationQueue = db.TenantEmailNotificationQueues.Find(id);
        //    if (tenantEmailNotificationQueue == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tenantEmailNotificationQueue);
        //}

        //// GET: TenantEmailNotificationQueues/Create
        //public ActionResult Create()
        //{
        //    ViewBag.AppointmentId = new SelectList(db.Appointments, "AppointmentId", "Subject");
        //    ViewBag.OrderId = new SelectList(db.Order, "OrderID", "OrderNumber");
        //    ViewBag.TenantEmailTemplatesId = new SelectList(db.TenantEmailTemplates, "TemplateId", "EventName");
        //    return View();
        //}

        //// POST: TenantEmailNotificationQueues/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "TenantEmailNotificationQueueId,OrderId,EmailSubject,AttachmentVirtualPath,TenantEmailTemplatesId,AppointmentId,IsNotificationCancelled,ScheduledProcessing,ScheduledProcessingTime,ActualProcessingTime,WorkOrderStartTime,WorkOrderEndTime,WorksOrderResourceName,ProcessedImmediately,CustomRecipients,CustomCcRecipients,CustomBccRecipients,CustomEmailMessage,InvoiceMasterId")] TenantEmailNotificationQueue tenantEmailNotificationQueue)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.TenantEmailNotificationQueues.Add(tenantEmailNotificationQueue);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.AppointmentId = new SelectList(db.Appointments, "AppointmentId", "Subject", tenantEmailNotificationQueue.AppointmentId);
        //    ViewBag.OrderId = new SelectList(db.Order, "OrderID", "OrderNumber", tenantEmailNotificationQueue.OrderId);
        //    ViewBag.TenantEmailTemplatesId = new SelectList(db.TenantEmailTemplates, "TemplateId", "EventName", tenantEmailNotificationQueue.TenantEmailTemplatesId);
        //    return View(tenantEmailNotificationQueue);
        //}

        //// GET: TenantEmailNotificationQueues/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TenantEmailNotificationQueue tenantEmailNotificationQueue = db.TenantEmailNotificationQueues.Find(id);
        //    if (tenantEmailNotificationQueue == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.AppointmentId = new SelectList(db.Appointments, "AppointmentId", "Subject", tenantEmailNotificationQueue.AppointmentId);
        //    ViewBag.OrderId = new SelectList(db.Order, "OrderID", "OrderNumber", tenantEmailNotificationQueue.OrderId);
        //    ViewBag.TenantEmailTemplatesId = new SelectList(db.TenantEmailTemplates, "TemplateId", "EventName", tenantEmailNotificationQueue.TenantEmailTemplatesId);
        //    return View(tenantEmailNotificationQueue);
        //}

        //// POST: TenantEmailNotificationQueues/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "TenantEmailNotificationQueueId,OrderId,EmailSubject,AttachmentVirtualPath,TenantEmailTemplatesId,AppointmentId,IsNotificationCancelled,ScheduledProcessing,ScheduledProcessingTime,ActualProcessingTime,WorkOrderStartTime,WorkOrderEndTime,WorksOrderResourceName,ProcessedImmediately,CustomRecipients,CustomCcRecipients,CustomBccRecipients,CustomEmailMessage,InvoiceMasterId")] TenantEmailNotificationQueue tenantEmailNotificationQueue)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(tenantEmailNotificationQueue).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.AppointmentId = new SelectList(db.Appointments, "AppointmentId", "Subject", tenantEmailNotificationQueue.AppointmentId);
        //    ViewBag.OrderId = new SelectList(db.Order, "OrderID", "OrderNumber", tenantEmailNotificationQueue.OrderId);
        //    ViewBag.TenantEmailTemplatesId = new SelectList(db.TenantEmailTemplates, "TemplateId", "EventName", tenantEmailNotificationQueue.TenantEmailTemplatesId);
        //    return View(tenantEmailNotificationQueue);
        //}

        //// GET: TenantEmailNotificationQueues/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TenantEmailNotificationQueue tenantEmailNotificationQueue = db.TenantEmailNotificationQueues.Find(id);
        //    if (tenantEmailNotificationQueue == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(tenantEmailNotificationQueue);
        //}

        //// POST: TenantEmailNotificationQueues/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    TenantEmailNotificationQueue tenantEmailNotificationQueue = db.TenantEmailNotificationQueues.Find(id);
        //    db.TenantEmailNotificationQueues.Remove(tenantEmailNotificationQueue);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

    }
}
