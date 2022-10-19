using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WMS.Reports;
using WMS.Reports.Designs;

namespace WMS.Controllers
{
    public class BaseReportsController : BaseController
    {
        private readonly IAppointmentsService _appointmentsService;
        protected IGaneConfigurationsHelper GaneConfigurationsHelper;
        private readonly IEmailServices _emailServices;
        private readonly ITenantLocationServices _tenantLocationservices;
        protected ITenantsServices _tenantServices;
        readonly IAccountServices _accountServices;


        public BaseReportsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IAppointmentsService appointmentsService,
            IGaneConfigurationsHelper ganeConfigurationsHelper, IEmailServices emailServices, ITenantLocationServices tenantLocationservices, ITenantsServices tenantsServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _appointmentsService = appointmentsService;
            GaneConfigurationsHelper = ganeConfigurationsHelper;
            _emailServices = emailServices;
            _tenantLocationservices = tenantLocationservices;
            _tenantServices = tenantsServices;
            _accountServices = accountServices;
        }

        public PurchaseOrderPrint CreatePurchaseOrderPrint(int id = 0)
        {
            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);

            var report = new PurchaseOrderPrint();
            report.paramOrderId.Value = id;
            if (!config.ShowDecimalPoint)
            {
                report.lblQuantity.TextFormatString = "{0:0.##}";
            }
            report.RequestParameters = false;
            report.PoPictureBox.BeforePrint += PoPictureBox_BeforePrint;
            report.FooterMsg1.Text = config.PoReportFooterMsg1;
            report.FooterMsg2.Text = config.PoReportFooterMsg2;
            return report;
        }
        public POCollectionNotePrint CreatePurchaseOrderPrints(int id = 0)
        {
            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);

            var report = new POCollectionNotePrint();
            report.paramOrderId.Value = id;
            if (!config.ShowDecimalPoint)
            {
                report.lblQuantity.TextFormatString = "{0:0.##}";
            }
            report.RequestParameters = false;
            report.PoPictureBox.BeforePrint += PoPictureBox_BeforePrint;
            return report;
        }

        private void PoPictureBox_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var picture = (XRPictureBox)sender;
            picture.ImageUrl = ReportLogoPath("po-logo.png");
        }

        public SalesOrderPrint CreateSalesOrderPrint(int id = 0, bool directSales = false, bool returns = false)
        {
            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);

            var report = new SalesOrderPrint();
            report.paramOrderId.Value = id;
            if (!config.ShowDecimalPoint)
            {
                report.lblQuantity.TextFormatString = "{0:0.##}";
            }

            report.xrInvoiceBankHeader.Visible = false;
            report.xrInvoiceBankHeaderAcName.Visible = false;
            report.xrInvoiceBankHeaderAcNo.Visible = false;
            report.xrInvoiceBankHeaderAcSort.Visible = false;
            report.xrInvoiceBankHeaderAcNameValue.Visible = false;
            report.xrInvoiceBankHeaderAcNoValue.Visible = false;
            report.xrInvoiceBankHeaderAcSortValue.Visible = false;

            if (directSales)
            {
                report.lblheading.Text = "INVOICE";
                report.lblheading.WidthF = 750;
                report.xrLabel70.Visible = false;

                report.xrInvoiceBankHeader.Visible = true;
                report.xrInvoiceBankHeaderAcName.Visible = true;
                report.xrInvoiceBankHeaderAcNo.Visible = true;
                report.xrInvoiceBankHeaderAcSort.Visible = true;
                report.xrInvoiceBankHeaderAcNameValue.Visible = true;
                report.xrInvoiceBankHeaderAcNoValue.Visible = true;
                report.xrInvoiceBankHeaderAcSortValue.Visible = true;
            }
            else if (returns)
            {
                report.lblheading.Text = "CREDIT NOTE";
                report.lblheading.WidthF = 750;
                report.xrLabel70.Visible = false;
            }
            else
            {
                report.lblheading.Text = "SALES ORDER";
            }
            report.RequestParameters = false;
            report.SoPictureBox.BeforePrint += SoPictureBox_BeforePrint;
            report.FooterMsg1.Text = config.SoReportFooterMsg1;
            report.FooterMsg2.Text = config.SoReportFooterMsg2;
            return report;
        }

        private void SoPictureBox_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var picture = (XRPictureBox)sender;
            picture.ImageUrl = ReportLogoPath("so-logo.png");
        }

        public TransferOrderPrint CreateTransferOrderPrint(int id = 0)
        {
            var report = new TransferOrderPrint();
            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);
            if (!config.ShowDecimalPoint)
            {
                report.lblQuantity.TextFormatString = "{0:0.##}";
            }
            report.paramOrderId.Value = id;
            report.RequestParameters = false;
            report.ToPictureBox.BeforePrint += ToPictureBox_BeforePrint;
            return report;
        }

        private void ToPictureBox_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var picture = (XRPictureBox)sender;
            picture.ImageUrl = ReportLogoPath("to-logo.png");
        }

        public WorksOrderPrint CreateWorksOrderPrint(int id = 0)
        {
            var report = new WorksOrderPrint();
            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);
            if (!config.ShowDecimalPoint)
            {
                report.lblQuantity.TextFormatString = "{0:0.##}";
            }
            report.paramOrderId.Value = id;
            report.RequestParameters = false;
            return report;
        }

        private MemoryStream ExportReportToStream(XtraReport xReport)
        {
            var str = new MemoryStream();
            xReport.ExportToPdf(str);
            return str;
        }

        public WorksOrderPrint CreateWorksOrderDayPrint()
        {
            var resources = _appointmentsService.GetAllResources(CurrentTenantId).ToList();
            var combinedReport = new WorksOrderPrint { FilterString = "[WorksAppointmentResourcesId] In (?paramResourceIds)" };
            combinedReport.paramScheduleDate.Value = DateTime.Today;

            StaticListLookUpSettings paramResourceIdsSettings = (StaticListLookUpSettings)combinedReport.paramResourceIds.LookUpSettings;
            paramResourceIdsSettings.LookUpValues.AddRange(resources.Select(m => new LookUpValue(m.ResourceId, m.Name)));
            combinedReport.ParametersRequestBeforeShow += CombinedReport_ParametersRequestBeforeShow;

            return combinedReport;
        }

        private void CombinedReport_ParametersRequestBeforeShow(object sender, ParametersRequestEventArgs e)
        {

            List<int> resourcesIdsList = new List<int>();

            var resources = _appointmentsService.GetAllResources(CurrentTenantId).Where(x => x.InternalStaff == true).ToList();

            foreach (var res in resources)
            {
                resourcesIdsList.Add(res.ResourceId);
            }

            e.ParametersInformation[0].Parameter.Value = resourcesIdsList;

        }

        private Parameter GetOrderIdParam(int orderId)
        {
            return new Parameter
            {
                Name = "OrderId",
                Type = typeof(int),
                Visible = false,
                Value = orderId
            };
        }

        public DeliveryNotePrint CreateDeliveryNotePrint(int id = 0, int[] OrderprocessId = null)
        {
            //creta and extra report instence 
            var report = new DeliveryNotePrint();
            report.xrLabel16.Text = "Delivery Note";
            report.xrLabel17.Text = "Delivery Address:";
            if (OrderprocessId != null)
            {
                report.paramOrderProcessId.Value = OrderprocessId;
            }
            else
            {
                int[] orderprocessID = new int[] { id };
                report.paramOrderProcessId.Value = orderprocessID;
            }
            // requesting the parameter value from end-users.
            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);
            if (!config.ShowDecimalPoint)
            {
                report.lblQuantity.TextFormatString = "{0:0.##}";
            }

            report.PoPictureBox.BeforePrint += DnPictureBox_BeforePrint;
            return report;
        }
        public DileveryReportPrintGroup CreateDeliveryNotePrintByGroup(int id = 0)
        {
            //creta and extra report instence 
            var report = new DileveryReportPrintGroup();
            report.xrLabel16.Text = "Delivery Note";
            report.xrLabel17.Text = "Delivery Address:";
            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);
            if (!config.ShowDecimalPoint)
            {
                report.lblQuantity.TextFormatString = "{0:0.##}";
            }
            report.OrderProcessId.Value = id;
            report.RequestParameters = false;
            report.PoPictureBox.BeforePrint += DnPictureBox_BeforePrint;
            return report;
        }

        private void DnPictureBox_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var picture = (XRPictureBox)sender;
            picture.ImageUrl = ReportLogoPath("dn-logo.png");
        }


        public PalleteDispatchesReport CreatePalleteReport(int id = 0)
        {
            //creta and extra report instence 
            var report = new PalleteDispatchesReport();
            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);
            if (!config.ShowDecimalPoint)
            {
                report.lblQuantity.TextFormatString = "{0:0.##}";
            }
            report.dispatchId.Value = id;
            // requesting the parameter value from end-users.
            report.RequestParameters = false;
            return report;
        }

        public GoodsReceiveNotePrint CreateGoodsReceiveNotePrint(Guid id)
        {
            //creta and extra report instence 
            var report = new GoodsReceiveNotePrint();
            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);
            if (!config.ShowDecimalPoint)
            {
                report.lblQuantity.TextFormatString = "{0:0.##}";
            }
            report.paramGoodsReceiveId.Value = id;
            // requesting the parameter value from end-users.
            report.RequestParameters = false;
            report.PoPictureBox.BeforePrint += GrnPictureBox_BeforePrint;
            return report;
        }

        private void GrnPictureBox_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var picture = (XRPictureBox)sender;
            picture.ImageUrl = ReportLogoPath("grn-logo.png");
        }

        public MarketRoutePrint CreateMarketRoutePrint(int id)
        {
            //creta and extra report instence 
            var report = new MarketRoutePrint();

            report.paramRouteId.Value = id;
            // requesting the parameter value from end-users.
            report.RequestParameters = false;
            report.PoPictureBox.BeforePrint += MrpPictureBox_BeforePrint;
            return report;
        }
        public MarketCustomerReport CreateMarketCustomerPrint(int id)
        {
            //creta and extra report instence 
            var report = new MarketCustomerReport();

            report.MarketId.Value = id;
            report.TenantID.Value = CurrentTenantId;
            // requesting the parameter value from end-users.
            report.RequestParameters = false;
            report.PoPictureBox.BeforePrint += MrpPictureBox_BeforePrint;
            return report;
        }

        private void MrpPictureBox_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var picture = (XRPictureBox)sender;
            picture.ImageUrl = ReportLogoPath("mrp-logo.png");
        }

        public FinancialTransactionReport CreateFinancialTransactionReport(int? accountId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            FinancialTransactionReport financialreport = new FinancialTransactionReport();
            if (accountId.HasValue && startDate.HasValue && endDate.HasValue)
            {
                financialreport.AccountId.Value = accountId.Value;
                financialreport.StartDate.Value = startDate.Value;
                var enddate = endDate.Value;
                financialreport.EndDate.Value = enddate;
            }
            else
            {
                financialreport.StartDate.Value = DateTime.Today.AddMonths(-1);
                financialreport.EndDate.Value = DateTime.Today;
                StaticListLookUpSettings accountsSettings = (StaticListLookUpSettings)financialreport.AccountId.LookUpSettings;
                var accounts = _accountServices.GetAllValidAccounts(CurrentTenantId).OrderBy(c=>c.AccountCode).ToList();
                accountsSettings.LookUpValues.AddRange(accounts.Select(m => new LookUpValue(m.AccountID, (m.AccountCode + " - " + m.CompanyName))));
            }

            financialreport.FrPictureBox.BeforePrint += FrPictureBox_BeforePrint;
            return financialreport;
        }

        private void FrPictureBox_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var picture = (XRPictureBox)sender;
            picture.ImageUrl = ReportLogoPath("fr-logo.png");
        }

        protected string ReportLogoPath(string logoPath)
        {

            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            var imagesUrl = "Content/images/";
            var relativePath = "~/" + imagesUrl + logoPath;
            var returnPath = baseUrl + imagesUrl + "logo.png";

            if (System.IO.File.Exists(Server.MapPath(relativePath)))
            {
                returnPath = baseUrl + imagesUrl + logoPath;
            }

            return returnPath;
        }

        public InvoicePrint CreateInvoicePrint(int id = 0, int[] InvoiceIds = null)
        {
            var report = new InvoicePrint();
            report.xrPictureBox1.BeforePrint += InvoicePrintPictureBox_BeforePrint;
            TenantConfig config = _tenantServices.GetTenantConfigById(CurrentTenantId);
            if (!config.ShowDecimalPoint)
            {
                report.lblQuantity.TextFormatString = "{0:0.##}";
                report.xrLabel2.TextFormatString = "{0:0.##}";
            }
            if (InvoiceIds == null)
            {
                int[] ids = new int[] { id };
                report.invoiceMasterId.Value = ids;
            }
            else
            {
                report.invoiceMasterId.Value = InvoiceIds;
            }

            report.RequestParameters = false;
            return report;
        }
        private void InvoicePrintPictureBox_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var picture = (XRPictureBox)sender;
            picture.ImageUrl = ReportLogoPath("po-logo.png");
        }

    }
}