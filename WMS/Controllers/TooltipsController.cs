using AutoMapper;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class TooltipsController : BaseController
    {
        private readonly ITooltipServices _tooltipServices;
        private readonly ITenantsServices _tenantsServices;
        private readonly IMapper _mapper;

        public TooltipsController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, ITooltipServices tooltipServices, ITenantsServices tenantsServices, IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _tooltipServices = tooltipServices;
            _tenantsServices = tenantsServices;
            _mapper = mapper;
        }

        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        public ActionResult Create()

        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.Tenants = new SelectList(_tenantsServices.GetAllTenants(), "TenantId", "TenantName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tooltip tooltip)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (CurrentUser.SuperUser != true && tooltip.TenantId != CurrentTenantId)
            {
                tooltip.TenantId = CurrentTenantId;
            }

            if (ModelState.IsValid)
            {
                _tooltipServices.Save(tooltip, CurrentUserId);
                return RedirectToAction("Index");
            }
            return View(tooltip);
        }

        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tooltip = _tooltipServices.GetById(id.Value, CurrentTenantId, CurrentUser.SuperUser == true);
            if (tooltip == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.SuperUser != true && tooltip.TenantId != CurrentTenantId)
            {
                return Redirect((string)Session["ErrorUrl"]);
            }

            return View(_mapper.Map<TooltipViewModel>(tooltip));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            _tooltipServices.Delete(id, CurrentUserId);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            Tooltip tooltip = _tooltipServices.GetById((int)id, CurrentTenantId, CurrentUser.SuperUser == true);

            if (tooltip == null)
            {
                return HttpNotFound();
            }

            if (CurrentUser.SuperUser != true && tooltip.TenantId != CurrentTenantId)
            {
                return Redirect((string)Session["ErrorUrl"]);
            }

            ViewBag.Tenants = new SelectList(_tenantsServices.GetAllTenants(), "TenantId", "TenantName");

            return View(tooltip);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tooltip tooltip)

        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (ModelState.IsValid)
            {
                if (CurrentUser.SuperUser != true && tooltip.TenantId != CurrentTenantId)
                {
                    tooltip.TenantId = CurrentTenantId;
                }

                _tooltipServices.Save(tooltip, CurrentUserId);
                return RedirectToAction("Index");
            }

            return View(tooltip);



        }

        public ActionResult TooltipList()
        {
            var viewModel = GridViewExtension.GetViewModel("TooltipListGridView");

            if (viewModel == null)
                viewModel = TooltipListCustomBinding.CreateTooltipGridViewModel();

            if (string.IsNullOrEmpty(viewModel.FilterExpression) && ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("TooltipsList"))
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["TooltipsList"];
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

            return TooltipGridActionCore(viewModel);
        }

        public ActionResult _TooltipListPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("TooltipListGridView");
            viewModel.Pager.Assign(pager);
            return TooltipGridActionCore(viewModel);
        }

        public ActionResult _TooltipsFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("TooltipListGridView");
            viewModel.ApplyFilteringState(filteringState);
            return TooltipGridActionCore(viewModel);
        }


        public ActionResult _TooltipsGetDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("TooltipListGridView");
            viewModel.ApplySortingState(column, reset);
            return TooltipGridActionCore(viewModel);
        }

        public ActionResult TooltipGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    TooltipListCustomBinding.TooltipGetDataRowCount(args, CurrentTenantId, CurrentUser.SuperUser == true);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        TooltipListCustomBinding.TooltipGetData(args, CurrentTenantId, CurrentUser.SuperUser == true);
                    })
            );
            return PartialView("_TooltipList", gridViewModel);
        }

        [OutputCache(Duration = 86400, VaryByParam = "*")]
        public async Task<JsonResult> GetTooltipsDetailByKey(string[] keys)
        {
            var tooltips = await _tooltipServices.GetTooltipsByKey(keys, CurrentTenantId);

            return Json(_mapper.Map<List<TooltipViewModel>>(tooltips), JsonRequestBehavior.AllowGet);

        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.IsSuperUser = CurrentUser.SuperUser == true;

            base.OnActionExecuting(filterContext);
        }

    }
}