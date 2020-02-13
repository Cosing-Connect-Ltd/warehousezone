using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ganedata.Core.Models;
using DevExpress.Web.Mvc;
using WMS.CustomBindings;
using System.Web;

namespace WMS.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILookupServices _lookupServices;
        private readonly IMarketServices _marketServices;

        public AccountController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _marketServices = marketServices;
            _userService = userService;
            _invoiceService = invoiceService;
            _lookupServices = lookupServices;
        }
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            int? marketId = 0;
            if (Session["MarketId"] != null)
            {
                marketId = (int?)Session["MarketId"];
            }
            ViewBag.marketId = marketId;
            ViewBag.MarketDetailId = new SelectList(_marketServices.GetAllValidMarkets(CurrentTenantId, CurrentWarehouseId), "MarketId", "MarketName", marketId);


            return View();
        }

        // GET: /Customer/Details/5
        public ActionResult Details(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account customer = AccountServices.GetAccountsById((int)id);
            if (customer == null)
            {
                return HttpNotFound();
            }


            ViewBag.dCountry = LookupServices.GetAllGlobalCountries().Where(x => x.CountryID == customer.CountryID).Select(x => x.CountryName).FirstOrDefault();
            ViewBag.owneruser = _userService.GetAuthUserById(customer.OwnerUserId)?.UserName;
            return View(customer);
        }

        // GET: /Customer/Create
        public ActionResult Create()

        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            AccountAddressSessionInit();
            ViewBag.AccountAddresses = new List<AccountAddresses>();
            ViewBag.AccountContacts = new List<AccountContacts>();

            var taxes = _lookupServices.GetAllValidGlobalTaxes().ToList();
            ViewBag.TaxID = new SelectList(taxes, "TaxID", "TaxName", taxes.Select(x => x.TaxID).FirstOrDefault());

            return View();
        }

        // POST: /Customer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Account model, List<int> AccountAddressIds, List<int> AccountContactIds, int GlobalCountryIds, int GlobalCurrencyIds, int AccountStatusIds, int PriceGroupId, int OwnerUserId, string StopComment)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (ModelState.IsValid)
            {
                var newContacts = Session["contacts"] as List<AccountContacts>;
                var newAddresses = Session["addresses"] as List<AccountAddresses>;
                AccountServices.SaveAccount(model, AccountAddressIds, AccountContactIds, GlobalCountryIds, GlobalCurrencyIds, AccountStatusIds, PriceGroupId, OwnerUserId, newAddresses, newContacts, CurrentUserId, CurrentTenantId, StopComment);
                return RedirectToAction("Index");
            }

            AccountAddressSessionInit();
            ViewBag.AccountAddresses = new List<AccountAddresses>();
            ViewBag.AccountContacts = new List<AccountContacts>();

            var taxes = _lookupServices.GetAllValidGlobalTaxes().ToList();
            ViewBag.TaxID = new SelectList(taxes, "TaxID", "TaxName", taxes.Select(x => x.TaxID).FirstOrDefault());
            return View(model);
        }

        public ActionResult Edit(int? id, int? MarketId)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.MarketId = MarketId ?? 0;
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            Account customer = AccountServices.GetAccountsById((int)id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            AccountAddressSessionInit(customer.OwnerUserId);
            Session["account"] = id;
            ViewBag.AccountAddresses = AccountServices.GetAllValidAccountAddressesByAccountId((int)id);
            ViewBag.AccountContacts = AccountServices.GetAllValidAccountContactsByAccountId((int)id, CurrentTenantId);
            ViewBag.LatestStopComment = AccountServices.GetLatestAuditComment(id.Value,CurrentTenantId);
            ViewBag.SelectedAddresses = AccountServices.GetAllValidAccountAddressesByAccountId((int)id).Select(a => a.AddressID).ToList();
            ViewBag.SelectedContacts = AccountServices.GetAllValidAccountContactsByAccountId((int)id, CurrentTenantId).Select(a => a.AccountContactId).ToList();

            var taxes = _lookupServices.GetAllValidGlobalTaxes().ToList();
            ViewBag.TaxID = new SelectList(taxes, "TaxID", "TaxName", customer.TaxID);

            return View(customer);
        }

        // POST: /Customer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(Account model, List<int> AccountAddressIds, List<int> AccountContactIds, string StopComment, int? MarketIds)

        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (ModelState.IsValid)
            {
                var newContacts = Session["contacts"] as List<AccountContacts>;

                var newAddresses = Session["addresses"] as List<AccountAddresses>;

                AccountServices.SaveAccount(model, AccountAddressIds, AccountContactIds, 0, 0, 0, 0, model.OwnerUserId, newAddresses, newContacts, CurrentUserId, CurrentTenantId, StopComment);
                Session["MarketId"] = MarketIds ?? 0;
                return RedirectToAction("Index");
            }

            Account customer = AccountServices.GetAccountsById(model.AccountID);
            model.GlobalCurrency = customer.GlobalCurrency;
            AccountAddressSessionInit(customer.OwnerUserId);
            Session["account"] = model.AccountID;
            ViewBag.AccountAddresses = AccountServices.GetAllValidAccountAddressesByAccountId(model.AccountID);
            ViewBag.AccountContacts = AccountServices.GetAllValidAccountContactsByAccountId(model.AccountID, CurrentTenantId);
            ViewBag.LatestStopComment = AccountServices.GetLatestAuditComment(model.AccountID,CurrentTenantId);
            ViewBag.SelectedAddresses = AccountServices.GetAllValidAccountAddressesByAccountId(model.AccountID).Select(a => a.AddressID).ToList();
            ViewBag.SelectedContacts = AccountServices.GetAllValidAccountContactsByAccountId(model.AccountID, CurrentTenantId).Select(a => a.AccountContactId).ToList();
            var taxes = _lookupServices.GetAllValidGlobalTaxes().ToList();
            ViewBag.TaxID = new SelectList(taxes, "TaxID", "TaxName", customer.TaxID);
            ViewBag.MarketId = MarketIds ?? 0;
            return View(model);



        }

        // GET: /Customer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var customer = AccountServices.GetAccountsById((int)id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }






        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            AccountServices.DeleteAccount(id, CurrentUserId);

            return RedirectToAction("Index");
        }


        public JsonResult IsUnique(string AccountCodes, int CustomerID = 0)
        {
            if (!String.IsNullOrEmpty(AccountCodes)) AccountCodes = AccountCodes.Trim();

            if (CustomerID == 0)
            {
                var c = AccountServices.GetAccountsByCode(AccountCodes, CurrentTenantId);
                return Json(c == null, JsonRequestBehavior.AllowGet);
            }

            var account = AccountServices.GetAccountsByCode(AccountCodes, CurrentTenantId);
            return Json(account != null && account.AccountID != CustomerID, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult AccountList(int Mid)
        {
            if (!caSession.AuthoriseSession())
            { return Redirect((string)Session["ErrorUrl"]); }
            Session["MarketId"] = Mid;
            var viewModel = GridViewExtension.GetViewModel("accountsGridView");

            if (viewModel == null)
                viewModel = AccountCustomBinding.CreateAccountGridViewModel();

            if (string.IsNullOrEmpty(viewModel.FilterExpression) && ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("AccountsList"))
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["AccountsList"];
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

            return AccountGridActionCore(viewModel);

        }



        public ActionResult _AccountPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("accountsGridView");
            viewModel.Pager.Assign(pager);
            return AccountGridActionCore(viewModel);
        }


        public ActionResult _AccountFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("accountsGridView");
            viewModel.ApplyFilteringState(filteringState);
            return AccountGridActionCore(viewModel);
        }

        public ActionResult _AccountSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("accountsGridView");
            viewModel.ApplySortingState(column, reset);
            return AccountGridActionCore(viewModel);
        }


        public ActionResult AccountGridActionCore(GridViewModel gridViewModel)
        {
            int? marketId = 0;
            if (Session["MarketId"] != null)
            {
                marketId = (int?)Session["MarketId"];
            }
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    AccountCustomBinding.AccountGetDataRowCount(args, CurrentTenantId, marketId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        AccountCustomBinding.AccountGetData(args, CurrentTenantId, marketId);
                    })
            );
            return PartialView("_AccountList", gridViewModel);
        }




        public ActionResult _AccountAddress()
        {

            ViewBag.GlobalCountries = LookupServices.GetAllGlobalCountries();
            return PartialView("_AccountAddress", new AccountAddresses() { AddTypeDefault = true });
        }
        public ActionResult _AccountAddressSave(int? id)
        {
            ViewBag.GlobalCountries = LookupServices.GetAllGlobalCountries();
            if (id == null)
            {
                return PartialView("_AccountAddress", new AccountAddresses() { AddTypeDefault = true });
            }
            var model = AccountServices.GetAccountAddressById((int)id);
            if (model == null)
                return HttpNotFound();

            return PartialView("_AccountAddress", model);
        }
        public ActionResult _AccountContactSave(int? id)
        {
            if (id == null)
            {
                return PartialView("_AccountContact");
            }
            var model = AccountServices.GetAccountContactById((int)id);
            if (model == null)
                return HttpNotFound();
            return PartialView("_AccountContact", model);
        }


        [HttpPost]
        public ActionResult SaveAccountContact(AccountContacts model)
        {
            if (model.AccountID == 0)
            {
                model.AccountID = int.Parse(Session["account"].ToString());
            }

            AccountServices.SaveAccountContact(model, CurrentUserId);

            return Redirect(Url.Action("Edit", new { id = Session["account"] }) + "#contacts");
        }
        [HttpPost]
        public ActionResult SaveAccountAddress(AccountAddresses model)
        {
            caUser user = caCurrent.CurrentUser();
            if (model.AccountID == 0)
            {
                model.AccountID = int.Parse(Session["account"].ToString());
            }

            AccountServices.SaveAccountAddress(model, CurrentUserId);

            return Redirect(Url.Action("Edit", new { id = Session["account"] }) + "#addresses");
        }


        public void _SaveAddresses(AccountAddresses address)
        {
            var lst = Session["addresses"] as List<AccountAddresses>;
            lst.Add(address);
        }
        public ActionResult _AccountContact()
        {
            return PartialView("_AccountContact");
        }

        public void _SaveAccountContact(AccountContacts contact)
        {
            var lst = Session["contacts"] as List<AccountContacts>;
            lst.Add(contact);
        }

        public void AccountAddressSessionInit(int? owneruserId=null)
        {
            Session["addresses"] = null;
            Session["contacts"] = null;
            List<AccountAddresses> lstAddresses = new List<AccountAddresses>();
            Session["addresses"] = lstAddresses;
            List<AccountContacts> lstContacts = new List<AccountContacts>();
            Session["contacts"] = lstContacts;

            caUser user = caCurrent.CurrentUser();

            var cntries = (from cntry in LookupServices.GetAllGlobalCountries()
                           select new
                           {
                               CountryId = cntry.CountryID,
                               CountryName = cntry.CountryName + "-" + cntry.CountryCode

                           }).ToList();

            ViewBag.Countries = new SelectList(cntries.OrderBy(o => o.CountryId), "CountryID", "CountryName");
            ViewBag.Currencies = new MultiSelectList(LookupServices.GetAllGlobalCurrencies().OrderBy(o => o.CurrencyID), "CurrencyId", "CurrencyName");
            ViewBag.AccountStatus = new SelectList(LookupServices.GetAllAccountStatuses(), "AccountStatusID", "AccountStatus");
            ViewBag.PriceGroups = new MultiSelectList(LookupServices.GetAllPriceGroups(CurrentTenantId), "PriceGroupID", "Name");
            ViewBag.OwnerUsers = new SelectList(_userService.GetAllAuthUsers(CurrentTenantId), "UserId", "UserName", owneruserId);
            ViewBag.OwnerUserId = owneruserId.HasValue?owneruserId : user.UserId;

        }

        public ActionResult _AddressesByAccount(int AccountID)
        {
            ViewData["accountid"] = AccountID;
            var addresses = AccountServices.GetAllValidAccountAddressesByAccountId(AccountID).ToList();
            return PartialView("_AddressesByAccount", addresses);
        }
        public ActionResult _ContactsByAccount(int AccountID)
        {
            ViewData["accountid"] = AccountID;
            var addresses = AccountServices.GetAllValidAccountContactsByAccountId(AccountID, CurrentTenantId).ToList();
            return PartialView("_ContactsByAccount", addresses);
        }

        public ActionResult _AddressByAccount(int AccountID)
        {
            ViewData["accountid"] = AccountID;
            var addresses = AccountServices.GetAllValidAccountAddressesByAccountId(AccountID).ToList();
            return PartialView(addresses);
        }
        public ActionResult SaveAccountAddress(int? id)
        {

            return View(id);
        }
        public ActionResult SaveAccountContact(int? id)
        {

            return View(id);
        }

        [HttpPost]
        public ActionResult EditAccountAddress(AccountAddresses model)
        {
            return RedirectToAction("Edit", new { id = Session["account"] });
        }
        public ActionResult DeleteAccountAddress(int? id)
        {

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var address = AccountServices.GetAccountAddressById((int)id);
            if (address == null)
            {
                return HttpNotFound();
            }
            return View(address);

        }
        [HttpPost, ActionName("DeleteAccountAddress")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccountAddressConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AccountServices.DeleteAccountAddress(id.Value, CurrentUserId);

            return Redirect(Url.Action("Edit", new { id = Session["account"] }) + "#addresses");

        }
        public ActionResult DeleteAccountContact(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contact = AccountServices.GetAccountContactById((int)id);

            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        [HttpPost, ActionName("DeleteAccountContact")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAccountContactConfirmed(int? id)
        {
            AccountServices.DeleteAccountContact(id ?? 0, CurrentUserId);
            return Redirect(Url.Action("Edit", new { id = Session["account"] }) + "#contacts");
        }

        public ActionResult _AccountAudits(int id)
        {
            ViewData["AccountID"] = id;
            var model = AccountServices.GetAccountAudits(id);
            return PartialView("_AccountAuditsGridPartial", model);
        }


        public string _Market(int AccountID,int MarketId)
        {
            var Market = _marketServices.GetMarketName(AccountID,MarketId);
            return Market;

        }
    }
}
