using AutoMapper;
using DevExpress.Web.Mvc;
using DocumentFormat.OpenXml.EMMA;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class ApiCredentialsController : BaseController
    {
        private readonly IApiCredentialServices _apiCredentialServices;
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly ITenantLocationServices _tenantLocationServices;
        private readonly IMapper _mapper;

        public ApiCredentialsController(ICoreOrderService orderService,
                                        IPropertyService propertyService,
                                        IAccountServices accountServices,
                                        ILookupServices lookupServices,
                                        IApiCredentialServices apiCredentialServices,
                                        ITenantLocationServices tenantLocationServices,
                                        ITenantWebsiteService tenantWebsiteService,
                                        IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _apiCredentialServices = apiCredentialServices;
            _tenantWebsiteService = tenantWebsiteService;
            _tenantLocationServices = tenantLocationServices;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        [HttpGet]
        public ActionResult Create()

        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.TenantWebsites = new SelectList(_tenantWebsiteService.GetAllValidTenantWebSite(CurrentTenantId), "SiteId", "SiteName");
            ViewBag.TenantLocations = new SelectList(_tenantLocationServices.GetAllTenantLocations(CurrentTenantId), "WarehouseId", "WarehouseName");

            var model = new ApiCredentials {
                ExpiryDate = DateTime.UtcNow
            };
            return View(model);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitCreate(ApiCredentials apiCredential)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }


            if (ModelState.IsValid)
            {
                apiCredential.TenantId = CurrentTenantId;
                _apiCredentialServices.Save(apiCredential, CurrentUserId);
                return RedirectToAction("Index");
            }
            return View(apiCredential);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var apiCredential = _apiCredentialServices.GetById(id.Value, CurrentTenantId);
            if (apiCredential == null)
            {
                return HttpNotFound();
            }

            return View(_mapper.Map<ApiCredentialsViewModel>(apiCredential));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitDelete(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            _apiCredentialServices.Delete(id, CurrentUserId);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var apiCredential = _apiCredentialServices.GetById((int)id, CurrentTenantId);

            if (apiCredential == null)
            {
                return HttpNotFound();
            }

            ViewBag.TenantWebsites = new SelectList(_tenantWebsiteService.GetAllValidTenantWebSite(CurrentTenantId), "SiteId", "SiteName");
            ViewBag.TenantLocations = new SelectList(_tenantLocationServices.GetAllTenantLocations(CurrentTenantId), "WarehouseId", "WarehouseName");

            return View(apiCredential);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitEdit(ApiCredentials apiCredential)

        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (ModelState.IsValid)
            {
                _apiCredentialServices.Save(apiCredential, CurrentUserId);
                return RedirectToAction("Index");
            }

            return View(apiCredential);
        }

        #region GridView Methods
        public ActionResult ApiCredentialList()
        {
            var viewModel = GridViewExtension.GetViewModel("ApiCredentialListGridView");

            if (viewModel == null)
                viewModel = ApiCredentialListCustomBinding.CreateApiCredentialGridViewModel();

            if (string.IsNullOrEmpty(viewModel.FilterExpression) && ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("ApiCredentialsList"))
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["ApiCredentialsList"];
                var decodedValue = HttpUtility.UrlDecode(cookie.Value);
                var filterParams = decodedValue
                    .Split('|')
                    .ToList();
                var lengthParam = filterParams.Where(x => x.StartsWith("filter")).SingleOrDefault();

                if (!string.IsNullOrEmpty(lengthParam))
                {
                    var index = filterParams.IndexOf(lengthParam);
                    var savedFilterExpression = filterParams[index + 1];
                    viewModel.FilterExpression = savedFilterExpression;
                }

                var pageNo = filterParams.Where(x => x.StartsWith("page")).SingleOrDefault();
                if (!string.IsNullOrEmpty(pageNo))
                {
                    var index = filterParams.IndexOf(pageNo);
                    var savedPageNo = filterParams[index];
                    var savedPageSize = filterParams[index + 1];
                    GridViewPagerState state = new GridViewPagerState();
                    state.PageIndex = Convert.ToInt32(string.Join("", savedPageNo.ToCharArray().Where(Char.IsDigit))) - 1;
                    state.PageSize = Convert.ToInt32(string.Join("", savedPageSize.ToCharArray().Where(Char.IsDigit)));
                    viewModel.Pager.Assign(state);
                }
            }

            return ApiCredentialGridActionCore(viewModel);
        }

        public ActionResult _ApiCredentialListPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("ApiCredentialListGridView");
            viewModel.Pager.Assign(pager);
            return ApiCredentialGridActionCore(viewModel);
        }

        public ActionResult _ApiCredentialsFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("ApiCredentialListGridView");
            viewModel.ApplyFilteringState(filteringState);
            return ApiCredentialGridActionCore(viewModel);
        }


        public ActionResult _ApiCredentialsGetDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("ApiCredentialListGridView");
            viewModel.ApplySortingState(column, reset);
            return ApiCredentialGridActionCore(viewModel);
        }

        public ActionResult ApiCredentialGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    ApiCredentialListCustomBinding.ApiCredentialGetDataRowCount(args, CurrentTenantId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        ApiCredentialListCustomBinding.ApiCredentialGetData(args, CurrentTenantId);
                    })
            );
            return PartialView("_ApiCredentialList", gridViewModel);
        }
        #endregion

    }
}