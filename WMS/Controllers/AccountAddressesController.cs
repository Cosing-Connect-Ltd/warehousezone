using Ganedata.Core.Entities.Domain;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class AccountAddressesController : BaseController
    {

        public AccountAddressesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            
        }
        // GET: /CustomerAddresses/Create
        public ActionResult Create(int id)
        {
            if (id==0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.CountryID = new SelectList(LookupServices.GetAllGlobalCountries(), "CountryID", "CountryName");
            ViewBag.CustomerID = id;

            ViewBag.Addresses =  AccountServices.GetAllValidAccountAddressesByAccountId(id).ToList();
            return View();
        }

        // POST: /CustomerAddresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Name,ContactName,AddressLine1,AddressLine2,AddressLine3,Telephone,Town,County,PostCode,CountryID,CustomerID")] AccountAddresses customeraddresses)
        {
            if (ModelState.IsValid)
            {  
                AccountServices.SaveAccountAddress(customeraddresses, CurrentUserId);

                if (!String.IsNullOrEmpty(customeraddresses.Name))
                
                return RedirectToAction("Create", new { id = customeraddresses.AccountID});
            }

            ViewBag.CountryID = new SelectList(LookupServices.GetAllGlobalCountries(), "CountryID", "CountryName", customeraddresses.CountryID);
            ViewBag.Addresses = AccountServices.GetAllValidAccountAddressesByAccountId(customeraddresses.AccountID).ToList();
            return View(customeraddresses);
        }

        // GET: /CustomerAddresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var customeraddresses = AccountServices.GetAccountAddressById((int) id);
            if (customeraddresses == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryID = new SelectList(LookupServices.GetAllGlobalCountries(), "CountryID", "CountryCode", customeraddresses.CountryID);
            return View(customeraddresses);
        }

        // POST: /CustomerAddresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for more details see 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="AddRecNo,Name,ContactName,AddressLine1,AddressLine2,AddressLine3,Telephone,Town,County,PostCode,CountryID,CustomerID")] AccountAddresses customeraddresses)
        {
            if (ModelState.IsValid)
            {
                AccountServices.SaveAccountAddress(customeraddresses, CurrentUserId);
                 
                return RedirectToAction("Create", new { id = customeraddresses.AccountID });
                
            }
            
            ViewBag.CountryID = new SelectList(LookupServices.GetAllGlobalCountries(), "CountryID", "CountryName", customeraddresses.CountryID);
            ViewBag.CustomerID = customeraddresses.AccountID;

            ViewBag.Addresses = AccountServices.GetAllValidAccountAddressesByAccountId(customeraddresses.AccountID).ToList();

            return View(customeraddresses);
        }
        
        // GET: /CustomerAddresses/Delete/5
        public ActionResult Delete(int AddRecNo)
        {
            var deletedAccount = AccountServices.DeleteAccountAddress(AddRecNo, CurrentUserId);
            if (deletedAccount == null)
            {
                return HttpNotFound();
            }

            var cid = deletedAccount.AccountID;

            return RedirectToAction("Create", new { id = cid }); 
        
        }
    }
}