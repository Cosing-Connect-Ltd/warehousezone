using AutoMapper;
using DevExpress.Web.Mvc;
using DevExpress.XtraPrinting;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class ResourcesController : BaseController
    {
        private readonly IEmployeeServices _employeeServices;
        private readonly ITenantLocationServices _tenantLocationsServices;
        private readonly IRolesServices _rolesServices;
        private readonly IGroupsServices _groupsServices;
        private readonly ContactNumbersServices _contactNumbersServices;
        private readonly AddressServices _addressServices;
        private readonly EmployeeShiftsStoresServices _employeeShiftsStoresServices;
        private readonly EmployeeRolesServices _employeeRolesServices;
        private readonly EmployeeGroupsServices _employeeGroupsServices;
        private readonly IActivityServices _activityServices;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;



        public ResourcesController(ICoreOrderService orderService, IUserService userService,  IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices,
            IEmployeeServices employeeServices, ITenantLocationServices tenantLocationServices, IRolesServices rolesServices, IGroupsServices groupsServices, ContactNumbersServices contactNumberServices, AddressServices addressServices,
            EmployeeShiftsStoresServices employeeShiftStoreServices, EmployeeRolesServices employeeRoleServices, EmployeeGroupsServices employeeGroupServices, IActivityServices activityServices, IMapper mapper)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _employeeServices = employeeServices;
            _tenantLocationsServices = tenantLocationServices;
            _rolesServices = rolesServices;
            _groupsServices = groupsServices;
            _contactNumbersServices = contactNumberServices;
            _addressServices = addressServices;
            _employeeShiftsStoresServices = employeeShiftStoreServices;
            _employeeRolesServices = employeeRoleServices;
            _employeeGroupsServices = employeeGroupServices;
            _activityServices = activityServices;
            _userService = userService;
            _mapper = mapper;
        }

        // GET: AppointmentResources
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View(_employeeServices.GetAllActiveAppointmentResourceses(CurrentTenantId));
        }

        // GET: AppointmentResources/Details/5
        public ActionResult Details(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var appointmentResources = _employeeServices.GetAppointmentResourceById(id.Value);
            if (appointmentResources == null)
            {
                return HttpNotFound();
            }

            return View(appointmentResources);
        }

        // GET: AppointmentResources/Create
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            // get properties of current tenant
            var tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            var wareh = _activityServices.GetAllPermittedWarehousesForUser(user.UserId, user.TenantId, user.SuperUser == true, false);

            var jTypes = LookupServices.GetAllJobTypes(CurrentTenantId).Select(m => new { m.JobTypeId, m.Name }).ToList();
            ViewBag.JobTypes = new MultiSelectList(jTypes, "JobTypeId", "Name");
            ViewBag.RolesList1 = new SelectList(_rolesServices.GetAllRoles(tenant.TenantId), "Id", "RoleName");
            ViewBag.GroupsList1 = new SelectList(_groupsServices.GetAllGroups(tenant.TenantId), "Id", "GroupName");
            ViewBag.Countries = new SelectList(LookupServices.GetAllGlobalCountries(), "CountryID", "CountryName");
            ViewBag.TenantLocations = new MultiSelectList(_tenantLocationsServices.GetAllTenantLocations(tenant.TenantId).Where(x => wareh.Any(a => a.WId == x.WarehouseId)), "WarehouseId", "WarehouseName");
            ViewBag.Users = new SelectList(_userService.GetAllAuthUsers(CurrentTenantId), "UserId", "UserName");
            return View();
        }

        // POST: AppointmentResources/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ResourcesViewModel appointmentResources, List<int> JobTypeIds, List<int> RolesList1, List<int> GroupsList1)
        {

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            // get properties of current tenant
            var tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            var wareh = _activityServices.GetAllPermittedWarehousesForUser(user.UserId, user.TenantId, user.SuperUser == true, false);

            appointmentResources.TenantId = CurrentTenantId;

            if (ModelState.IsValid)
            {

                //insert contactnumber
                int contacNumbersId = _contactNumbersServices.Insert(_mapper.Map<ContactNumbers>(appointmentResources.ContactNumbers));
                appointmentResources.ContactNumbersId = contacNumbersId;

                //insert address
                int addressId = _addressServices.Insert(_mapper.Map<Address>(appointmentResources.Address));
                appointmentResources.AddressId = addressId;

                //insert employee
                appointmentResources.CreatedBy = user.UserId;
                appointmentResources.UpdatedBy = user.UserId;
                appointmentResources.DateCreated = DateTime.UtcNow;
                appointmentResources.DateUpdated = DateTime.UtcNow;
                appointmentResources.TenantId = tenant.TenantId;
                int employeeId = _employeeServices.InsertEmployee(_mapper.Map<Resources>(appointmentResources));

                appointmentResources.ResourceId = employeeId;

                //insert employeeShifts_Store
                if (appointmentResources.StoresList != null)
                {
                    foreach (var item in appointmentResources.StoresList)
                    {
                        var insert = new EmployeeShifts_Stores()
                        {
                            ResourceId = employeeId,
                            WarehouseId = item,
                        };

                        _employeeShiftsStoresServices.Insert(insert);
                    }
                }

                //insert employeeRoles
                if (RolesList1 != null)
                {
                    foreach (var item in RolesList1)
                    {
                        if (item != 0)
                        {
                            var insert = new EmployeeRoles()
                            {
                                ResourceId = employeeId,
                                RolesId = item,
                                TenantId = tenant.TenantId,
                                DateCreated = DateTime.UtcNow,
                                DateUpdated = DateTime.UtcNow,
                                CreatedBy = user.UserId,
                                UpdatedBy = user.UserId
                            };

                            _employeeRolesServices.Insert(insert);
                        }
                    }
                }

                //insert employeeGroups
                if (GroupsList1 != null)
                {
                    foreach (var item in GroupsList1)
                    {
                        if (item != 0)
                        {
                            var insert = new EmployeeGroups()
                            {
                                ResourceId = employeeId,
                                GroupsId = item,
                                TenantId = tenant.TenantId,
                                DateCreated = DateTime.UtcNow,
                                DateUpdated = DateTime.UtcNow,
                                CreatedBy = user.UserId,
                                UpdatedBy = user.UserId
                            };

                            _employeeGroupsServices.Insert(insert);
                        }
                    }
                }

                // insert JobType Ids
                _employeeServices.AddResourceJobTypes(_mapper.Map<Resources>(appointmentResources), JobTypeIds);

                ViewBag.Success = $"Successfully Added";

                return RedirectToAction("Index");
            }

            // viewbag data
            var jTypes = LookupServices.GetAllJobTypes(CurrentTenantId).Select(m => new { m.JobTypeId, m.Name }).ToList();
            ViewBag.JobTypes = new MultiSelectList(jTypes, "JobTypeId", "Name");
            ViewBag.RolesList1 = new SelectList(_rolesServices.GetAllRoles(tenant.TenantId), "Id", "RoleName");
            ViewBag.GroupsList1 = new SelectList(_groupsServices.GetAllGroups(tenant.TenantId), "Id", "GroupName");
            ViewBag.Countries = new SelectList(LookupServices.GetAllGlobalCountries(), "CountryID", "CountryName");
            ViewBag.TenantLocations = new MultiSelectList(_tenantLocationsServices.GetAllTenantLocations(tenant.TenantId).Where(x => wareh.Any(a => a.WId == x.WarehouseId)), "WarehouseId", "WarehouseName");
            ViewBag.Users = new SelectList(_userService.GetAllAuthUsers(CurrentTenantId), "UserId", "UserName");
            ViewBag.Warning = $"There is some problem with entereis, please check and try to save again";

            return View(appointmentResources);
        }

        // GET: AppointmentResources/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            // get properties of current tenant
            var tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            var wareh = _activityServices.GetAllPermittedWarehousesForUser(user.UserId, user.TenantId, user.SuperUser == true, false);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ResourcesViewModel appointmentResources = _mapper.Map(_employeeServices.GetAppointmentResourceById(id.Value), new ResourcesViewModel());
            if (appointmentResources == null)
            {
                return HttpNotFound();
            }

            var jTypes = LookupServices.GetAllJobTypes(CurrentTenantId).Select(m => new { m.JobTypeId, m.Name }).ToList();
            ViewBag.JobTypes = new MultiSelectList(jTypes, "JobTypeId", "Name", _employeeServices.GetResourceJobTypeIds(appointmentResources.ResourceId));
            ViewBag.RolesList1 = new SelectList(_rolesServices.GetAllRoles(tenant.TenantId), "Id", "RoleName", _rolesServices.GetAllRolesByResource(appointmentResources.ResourceId));
            ViewBag.GroupsList1 = new SelectList(_groupsServices.GetAllGroups(tenant.TenantId), "Id", "GroupName", _groupsServices.GetResourceGroupIds(appointmentResources.ResourceId));
            ViewBag.Countries = new SelectList(LookupServices.GetAllGlobalCountries(), "CountryID", "CountryName", appointmentResources.GlobalCountry?.CountryID);
            ViewBag.TenantLocations = new MultiSelectList(_tenantLocationsServices.GetAllTenantLocations(tenant.TenantId).Where(x => wareh.Any(a => a.WId == x.WarehouseId)), "WarehouseId", "WarehouseName",
                _employeeShiftsStoresServices.GetEmployeeShifts_StoresByEmployeeId(appointmentResources.ResourceId).Select(p => p.WarehouseId));
            ViewBag.Users = new SelectList(_userService.GetAllAuthUsers(CurrentTenantId), "UserId", "UserName", appointmentResources?.AuthUserId);
            return View(appointmentResources);
        }

        // POST: AppointmentResources/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ResourcesViewModel appointmentResources, List<int> JobTypeIds, List<int> RolesList1, List<int> GroupsList1)
        {

            // get properties of current tenant
            var tenant = caCurrent.CurrentTenant();

            // get properties of user
            caUser user = caCurrent.CurrentUser();

            var wareh = _activityServices.GetAllPermittedWarehousesForUser(user.UserId, user.TenantId, user.SuperUser == true, false);


            if (ModelState.IsValid)
            {

                appointmentResources.TenantId = CurrentTenantId;

                //update
                Resources newEmployee = _employeeServices.GetByEmployeeId(appointmentResources.ResourceId);
                newEmployee.HolidayEntitlement = appointmentResources.HolidayEntitlement;
                newEmployee.HourlyRate = appointmentResources.HourlyRate;
                newEmployee.PersonTitle = appointmentResources.PersonTitle;
                newEmployee.FirstName = appointmentResources.FirstName;
                newEmployee.MiddleName = appointmentResources.MiddleName;
                newEmployee.SurName = appointmentResources.SurName;
                newEmployee.LikeToBeKnownAs = appointmentResources.LikeToBeKnownAs;
                newEmployee.Gender = appointmentResources.Gender;
                newEmployee.Married = appointmentResources.Married;
                newEmployee.Nationality = appointmentResources.Nationality;
                newEmployee.Color = appointmentResources.Color;
                newEmployee.IsActive = appointmentResources.IsActive;
                newEmployee.InternalStaff = appointmentResources.InternalStaff;
                newEmployee.Nationality = appointmentResources.Nationality;
                newEmployee.PayrollEmployeeNo = appointmentResources.PayrollEmployeeNo;
                newEmployee.AuthUserId = appointmentResources.AuthUserId;
                newEmployee.JobStartDate = appointmentResources.JobStartDate;
                newEmployee.IsFlexibleWorkingAllowed = appointmentResources.IsFlexibleWorkingAllowed;
                newEmployee.IsOvertimeWorkingAllowed = appointmentResources.IsOvertimeWorkingAllowed;
                newEmployee.AttendanceGracePeriodInMinutes = appointmentResources.AttendanceGracePeriodInMinutes;
                // update address         

                if (newEmployee.Address == null)
                {
                    Address newAddress = new Address();
                    newAddress.AddressLine1 = appointmentResources.Address.AddressLine1;
                    newAddress.AddressLine2 = appointmentResources.Address.AddressLine2;
                    newAddress.AddressLine3 = appointmentResources.Address.AddressLine3;
                    newAddress.CountryID = appointmentResources.Address.CountryID;
                    newAddress.County = appointmentResources.Address.County;
                    newAddress.HouseNumber = appointmentResources.Address.HouseNumber;
                    newAddress.PostCode = appointmentResources.Address.PostCode;
                    newAddress.Town = appointmentResources.Address.Town;

                    int addressId = _addressServices.Insert(_mapper.Map<Address>(appointmentResources.Address));
                    newEmployee.AddressId = addressId;
                }
                else
                {
                    newEmployee.Address.AddressLine1 = appointmentResources.Address.AddressLine1;
                    newEmployee.Address.AddressLine2 = appointmentResources.Address.AddressLine2;
                    newEmployee.Address.AddressLine3 = appointmentResources.Address.AddressLine3;
                    newEmployee.Address.CountryID = appointmentResources.Address.CountryID;
                    newEmployee.Address.County = appointmentResources.Address.County;
                    newEmployee.Address.HouseNumber = appointmentResources.Address.HouseNumber;
                    newEmployee.Address.PostCode = appointmentResources.Address.PostCode;
                    newEmployee.Address.Town = appointmentResources.Address.Town;
                }

                if (newEmployee.ContactNumbers == null)
                {
                    //insert contactnumber
                    int contacNumbersId = _contactNumbersServices.Insert(_mapper.Map<ContactNumbers>(appointmentResources.ContactNumbers));
                    newEmployee.ContactNumbersId = contacNumbersId;
                }
                else
                {
                    // update contacts
                    newEmployee.ContactNumbers.HomeNumber = appointmentResources.ContactNumbers.HomeNumber;
                    newEmployee.ContactNumbers.MobileNumber = appointmentResources.ContactNumbers.MobileNumber;
                    newEmployee.ContactNumbers.WorkNumber = appointmentResources.ContactNumbers.WorkNumber;
                    newEmployee.ContactNumbers.Fax = appointmentResources.ContactNumbers.Fax;
                    newEmployee.ContactNumbers.EmailAddress = appointmentResources.ContactNumbers.EmailAddress;
                }


                newEmployee.UpdatedBy = user.UserId;
                newEmployee.DateUpdated = DateTime.UtcNow;
                _employeeServices.UpdateEmployee(newEmployee);

                //delete first EmployeeShifts_Store
                _employeeShiftsStoresServices.DeleteEmployeeShiftsStoresByEmployeeId(appointmentResources.ResourceId);

                //update EmployeeShifts_Store
                if (appointmentResources.StoresList != null)
                {
                    foreach (var item in appointmentResources.StoresList.ToList())
                    {
                        _employeeShiftsStoresServices.UpdateEmployeeShifts_StoresByEmployeeId(appointmentResources.ResourceId, item);
                    }
                }

                //delete first EmployeeRoles
                _employeeRolesServices.DeleteEmployeeRolesByEmployeeId(appointmentResources.ResourceId);

                //update EmployeeRoles
                if (RolesList1 != null)
                {
                    foreach (var item in RolesList1)
                    {
                        if (item != 0) //0=Select a Role
                        {
                            EmployeeRoles newRole = new EmployeeRoles
                            {
                                RolesId = item,
                                ResourceId = appointmentResources.ResourceId,
                                TenantId = tenant.TenantId,
                                CreatedBy = user.UserId,
                                UpdatedBy = user.UserId,
                                DateCreated = DateTime.UtcNow,
                                DateUpdated = DateTime.UtcNow,
                            };

                            _employeeRolesServices.Insert(newRole);

                        }

                    }

                }


                //delete first EmployeeGroups
                _employeeGroupsServices.DeleteEmployeeGroupsByEmployeeId(appointmentResources.ResourceId);

                //update EmployeeGroups
                if (GroupsList1 != null)
                {
                    foreach (var item in GroupsList1)
                    {
                        if (item != 0) //0=Select a Group
                        {
                            EmployeeGroups newGroup = new EmployeeGroups()
                            {
                                GroupsId = item,
                                ResourceId = appointmentResources.ResourceId,
                                TenantId = tenant.TenantId,
                                CreatedBy = user.UserId,
                                UpdatedBy = user.UserId,
                                DateCreated = DateTime.UtcNow,
                                DateUpdated = DateTime.UtcNow
                            };
                            _employeeGroupsServices.Insert(newGroup);
                        }

                    }

                }

                _employeeServices.UpdateResourceJobTypes(_mapper.Map<Resources>(appointmentResources), JobTypeIds);
                return RedirectToAction("Index");
            }

            var jTypes = LookupServices.GetAllJobTypes(CurrentTenantId).Select(m => new { m.JobTypeId, m.Name }).ToList();
            ViewBag.JobTypes = new MultiSelectList(jTypes, "JobTypeId", "Name", _employeeServices.GetResourceJobTypeIds(appointmentResources.ResourceId));
            ViewBag.RolesList1 = new SelectList(_rolesServices.GetAllRoles(tenant.TenantId), "Id", "RoleName", _rolesServices.GetAllRolesByResource(appointmentResources.ResourceId));
            ViewBag.GroupsList1 = new SelectList(_groupsServices.GetAllGroups(tenant.TenantId), "Id", "GroupName", _groupsServices.GetResourceGroupIds(appointmentResources.ResourceId));
            ViewBag.Countries = new SelectList(LookupServices.GetAllGlobalCountries(), "CountryID", "CountryName", appointmentResources.GlobalCountry.CountryID);
            ViewBag.TenantLocations = new MultiSelectList(_tenantLocationsServices.GetAllTenantLocations(tenant.TenantId).Where(x => wareh.Any(a => a.WId == x.WarehouseId)), "WarehouseId", "WarehouseName",
                _employeeShiftsStoresServices.GetEmployeeShifts_StoresByEmployeeId(appointmentResources.ResourceId).Select(p => p.WarehouseId));
            return View(appointmentResources);
        }

        // GET: AppointmentResources/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var appointmentResources = _employeeServices.GetAppointmentResourceById(id.Value);
            if (appointmentResources == null)
            {
                return HttpNotFound();
            }
            return View(appointmentResources);
        }

        // POST: AppointmentResources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // get properties of user
            caUser user = caCurrent.CurrentUser();

            _employeeServices.DeleteAppointmentResource(id, user.UserId);
            return RedirectToAction("Index");
        }


       


        [ValidateInput(false)]
        public ActionResult ResourceListPartial(string exportRowType)
        {
            if (Request.Params["exportRowType"] != null)
            {
                ViewData["exportRowType"] = Request.Params["exportRowType"];
            }
            var viewModel = GridViewExtension.GetViewModel("_ResourceListPartial");

            if (viewModel == null)
                viewModel = ResourceCustomBinding.CreateResourceListGridViewModel();

            return ResourceListGridActionCore(viewModel, exportRowType);

        }
        public ActionResult ResourceListGridActionCore(GridViewModel gridViewModel,string exportRowType)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    ResourceCustomBinding.ResourceListGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        ResourceCustomBinding.ResourceListGetData(args, CurrentTenantId, CurrentWarehouseId, exportRowType);
                    })
            );
            return PartialView("_ResourceListPartial", gridViewModel);
        }

        public ActionResult _ResourceListGridViewsPaging(GridViewPagerState pager)
        {
            string row = "";
            if (ViewData["exportRowType"] != null)
            {
               row = ViewData["exportRowType"].ToString();
            }
            
            var viewModel = GridViewExtension.GetViewModel("ResourceList");
            viewModel.Pager.Assign(pager);
            return ResourceListGridActionCore(viewModel,row);
        }

        public ActionResult _ResourceListGridViewFiltering(GridViewFilteringState filteringState)
        {
            string row = "";
            if (ViewData["exportRowType"] != null)
            {
                row = ViewData["exportRowType"].ToString();
            }
            var viewModel = GridViewExtension.GetViewModel("ResourceList");
            viewModel.ApplyFilteringState(filteringState);
            return ResourceListGridActionCore(viewModel,row);
        }


        public ActionResult _ResourceListGridViewDataSorting(GridViewColumnState column, bool reset)
        {
            string row = "";
            if (ViewData["exportRowType"] != null)
            {
                row = ViewData["exportRowType"].ToString();
            }
            var viewModel = GridViewExtension.GetViewModel("ResourceList");
            viewModel.ApplySortingState(column, reset);
            return ResourceListGridActionCore(viewModel,row);
        }


        public ActionResult ExportTo(GridViewSettings settings, object dataObject, bool saveAsFile, XlsxExportOptions exportOptions)
        {

            return View();
        }
        [HttpPost]
        public JsonResult IsUserAvailable(int AuthUserId)
        {
            
            if (AuthUserId>0)
            {
                var result = _employeeServices.IsUserAvailable(AuthUserId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

        }

        

    }
}

