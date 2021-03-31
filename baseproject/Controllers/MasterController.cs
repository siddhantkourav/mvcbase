using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using baseproject.Models;
using baseproject.Base.Models;


namespace baseproject.Controllers
{
    public class MasterController : BaseController
    {
        public MasterController()
        {
            db = new BaseEntities();
            masters = new Masters();
        }

        // GET: Master
        public ActionResult Index()
        {

            return View();
        }

        #region Client
        public ActionResult ClientIndex(int id = 0)
        {
            client c = new client();
            if (id != 0)
            {
                c = db.clients.Find(Convert.ToInt32(id));
            }
            ViewBag.result = (Result)TempData["result"];
            return View(c);
        }
        public ActionResult ClientCreateEdit(client c)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ClientIndex");
            }
            result = masters.ClientCreateEdit(c);
            TempData["result"] = result;
            return RedirectToAction("ClientIndex");
        }
        public ActionResult ClientEdit(string id)
        {
            var data = (from c in db.clients.AsEnumerable()
                        where c.client_id == Convert.ToInt32(id)
                        select new
                        {
                            c.client_id,
                            c.client_name,
                            c.is_active,
                        }).FirstOrDefault();
            return Json(data);
        }
        public ActionResult ClientList()
        {
            var list = db.clients.AsEnumerable().ToList();
            var data = (from li in list
                        orderby li.client_name
                        select new
                        {
                            li.client_name,
                            li.client_id,
                            li.is_active,
                        }).ToList();
            return Json(data);
        }
        #endregion

        #region Driver
        public ActionResult DriverIndex(int id = 0)
        {
            driver c = new driver();
            if (id != 0)
            {
                c = db.drivers.Find(Convert.ToInt32(id));
            }
            ViewBag.result = (Result)TempData["result"];
            return View(c);
        }
        public ActionResult DriverCreateEdit(driver d)
        {
            var data = new driver();
            if (!ModelState.IsValid)
            {
                return RedirectToAction("DriverIndex");
            }
            result = masters.DriverCreateEdit(d);
            TempData["result"] = result;
            return RedirectToAction("DriverIndex");
        }
        public ActionResult DriverEdit(string id)
        {
            var data = (from d in db.drivers.AsEnumerable()
                        where d.driver_id == Convert.ToInt32(id)
                        select new
                        {
                            d.driver_id,
                            d.driver_name,
                            d.driver_contact_no,
                            d.is_active,
                        }).FirstOrDefault();
            return Json(data);
        }
        public ActionResult DriverList()
        {
            var list = db.drivers.AsEnumerable().ToList();
            var data = (from li in list
                        orderby li.driver_name
                        select new
                        {
                            li.driver_name,
                            li.driver_contact_no,
                            li.driver_id,
                            li.is_active,
                        }).ToList();
            return Json(data);
        }
        #endregion

        #region Vehicle Owner
        public ActionResult VehicleOwnerIndex(int id = 0)
        {
            vehicleOwnership vo = new vehicleOwnership();
            if (id != 0)
            {
                vo = db.vehicleOwnerships.Find(Convert.ToInt32(id));
            }
            ViewBag.result = (Result)TempData["result"];
            return View(vo);
        }
        public ActionResult VehicleOwnerCreateEdit(vehicleOwnership vo)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("VehicleOwnerIndex");
            }
            result = masters.VehicleOwnerCreateEdit(vo);
            TempData["result"] = result;
            return RedirectToAction("VehicleOwnerIndex");
        }
        public ActionResult VehicleOwnerEdit(string id)
        {
            var data = (from vo in db.vehicleOwnerships.AsEnumerable()
                        where vo.owner_id == Convert.ToInt32(id)
                        select new
                        {
                            vo.owner_id,
                            vo.owner_name,
                            vo.owner_contact_no,
                            vo.is_active
                        }).FirstOrDefault();
            return Json(data);
        }
        public ActionResult VehicleOwnerList()
        {
            var list = db.vehicleOwnerships.AsEnumerable().ToList();
            var data = (from li in list
                        orderby li.owner_name
                        select new
                        {
                            li.owner_id,
                            li.owner_name,
                            li.owner_contact_no,
                            li.is_active
                        }).ToList();
            return Json(data);
        }
        #endregion

        #region Vehicle
        public ActionResult VehicleIndex(int id = 0)
        {
            vehicle c = new vehicle();
            if (id != 0)
            {
                c = db.vehicles.Find(Convert.ToInt32(id));
            }
            ViewBag.vehicle_owner_id = new SelectList(masters.GetVehicleOwnerSelectList(), "Value", "Text", c.vehicle_owner_id);
            ViewBag.ownership_type_id = new SelectList(masters.GetOwnershipTypeSelectList(), "Value", "Text", c.ownership_type_id);

            ModelState.AddModelError("", (string)TempData["ModelState1"] ?? "");
            ViewBag.result = (Result)TempData["result"];
            return View(c);
        }
        public ActionResult VehicleCreateEdit(vehicle v)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("VehicleIndex");
            }
            if (db.vehicles.Where(x => x.rego_no == v.rego_no && x.vehicle_id != v.vehicle_id).Count() != 0)
            {
                TempData["ModelState1"] = "Registration No. already Exist";
                return RedirectToAction("VehicleIndex");
            }
            result = masters.VehicleCreateEdit(v);
            TempData["result"] = result;
            return RedirectToAction("VehicleIndex");
        }
        public ActionResult VehicleEdit(string id)
        {
            var data = (from v in db.vehicles.AsEnumerable()
                        where v.vehicle_id == Convert.ToInt32(id)
                        select new
                        {
                            v.vehicle_id,
                            v.ownership_type_id,
                            v.rego_no,
                            v.manufacturing_year,
                            v.engine_no,
                            v.chassis_no,
                            v.vehicle_owner_id,
                            v.vehicle_type,
                            v.is_active,
                        }).FirstOrDefault();
            return Json(data);
        }
        public ActionResult VehicleList()
        {
            var list = db.vehicles.AsEnumerable().ToList();
            var data = (from li in list
                        orderby li.rego_no
                        select new
                        {
                            li.vehicle_id,
                            ownership_type_name = li.ownership_type == null ? "" : li.ownership_type.ownership_type_name,
                            li.rego_no,
                            li.vehicleOwnership.owner_name,
                            li.vehicle_type,
                            li.manufacturing_year,
                            li.chassis_no,
                            li.engine_no,
                            li.is_active,
                        }).ToList();
            return Json(data);
        }

        ////For check both at a time .mailid and empName.   
        //[HttpGet]
        //public JsonResult IsEmpNameandMailExist(string Empname, string EmpMail)
        //{
        //    bool isExist = db.vehicles.Where(x => x.rego_no == Empname).FirstOrDefault() != null;
        //    return Json(!isExist, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult IsBugExist(string id)
        //{
        //    //id="country_name,country_id"
        //    string[] info = id.Split(',');
        //    string BugName = id;
        //    int bugId = 0;

        //    if (info.Length == 2)
        //    {
        //        BugName = info[0].Trim();
        //        bugId = info[1] != "" ? Convert.ToInt32(info[1]) : bugId;

        //    }
        //    public bug GetBugByName(string BugName, int bugId)
        //    {
        //        int company_id = SessionUtil.GetCompanyID();
        //        return bugId > 0 ?

        //        db.bugs.AsEnumerable().Where(x => x.bug_title.ToUpper() == BugName.ToUpper() && x.company_id == company_id && x.bug_id != bugId).FirstOrDefault()
        //            : db.bugs.AsEnumerable().Where(x => x.bug_title.ToUpper() == BugName.ToUpper() && x.company_id == company_id).FirstOrDefault();
        //    }

        //    return Json( == null ? false : true);
        //}
        #endregion

        #region Trip Type
        public ActionResult TripTypeIndex(int id = 0)
        {
            tripType tt = new tripType();
            if (id != 0)
            {
                tt = db.tripTypes.Find(Convert.ToInt32(id));
            }
            ViewBag.result = (Result)TempData["result"];
            return View(tt);
        }
        public ActionResult TripTypeCreateEdit(tripType tt)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("TripTypeIndex");
            }
            result = masters.TripTypeCreateEdit(tt);
            TempData["result"] = result;
            return RedirectToAction("TripTypeIndex");
        }
        public ActionResult TripTypeEdit(string id)
        {
            var data = (from tt in db.tripTypes.AsEnumerable()
                        where tt.trip_type_id == Convert.ToInt32(id)
                        select new
                        {
                            tt.trip_type_id,
                            tt.trip_type_name,
                            tt.is_active,
                        }).FirstOrDefault();
            return Json(data);
        }
        public ActionResult TripTypeList()
        {
            var list = db.tripTypes.AsEnumerable().ToList();
            var data = (from li in list
                        orderby li.trip_type_name
                        select new
                        {
                            li.trip_type_name,
                            li.trip_type_id,
                            li.is_active,
                        }).ToList();
            return Json(data);
        }
        #endregion

        #region Contract Material
        public ActionResult MaterialIndex(int id = 0)
        {
            contractMaterial cm = new contractMaterial();
            if (id != 0)
            {
                cm = db.contractMaterials.Find(Convert.ToInt32(id));
            }
            ViewBag.result = (Result)TempData["result"];
            return View(cm);
        }
        public ActionResult MaterialCreateEdit(contractMaterial cm)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("MaterialIndex");
            }
            result = masters.MaterialCreateEdit(cm);
            TempData["result"] = result;
           
            return RedirectToAction("MaterialIndex");
        }
        public ActionResult MaterialEdit(string id)
        {
            var data = (from cm in db.contractMaterials.AsEnumerable()
                        where cm.contract_material_id == Convert.ToInt32(id)
                        select new
                        {
                            cm.contract_material_id,
                            cm.contract_material_name,
                            cm.is_active
                        }).FirstOrDefault();
            return Json(data);
        }
        public ActionResult MaterialList()
        {
            var list = db.contractMaterials.AsEnumerable().ToList();
            var data = (from li in list
                        orderby li.contract_material_name
                        select new
                        {
                            li.contract_material_name,
                            li.contract_material_id,
                            li.is_active
                        }).ToList();
            return Json(data);
        }
        #endregion

        #region Contract Location
        public ActionResult LocationIndex(int id = 0)
        {
            contractLocation cl = new contractLocation();
            if (id != 0)
            {
                cl = db.contractLocations.Find(Convert.ToInt32(id));
            }
            ViewBag.result = (Result)TempData["result"];
            return View(cl);
        }
        public ActionResult LocationCreateEdit(contractLocation cl)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("LocationIndex");
            }
            result = masters.LocationCreateEdit(cl);
            TempData["result"] = result;
           
            return RedirectToAction("LocationIndex");
        }
        public ActionResult LocationEdit(string id)
        {
            var data = (from cl in db.contractLocations.AsEnumerable()
                        where cl.contract_location_id == Convert.ToInt32(id)
                        select new
                        {
                            cl.contract_location_id,
                            cl.contract_location_name,
                            cl.is_active
                        }).FirstOrDefault();
            return Json(data);
        }
        public ActionResult LocationList()
        {
            var list = db.contractLocations.AsEnumerable().ToList();
            var data = (from li in list
                        orderby li.contract_location_name
                        select new
                        {
                            li.contract_location_name,
                            li.contract_location_id,
                            li.is_active
                        }).ToList();
            return Json(data);
        }
        #endregion

        #region Partner
        public ActionResult PartnerIndex(int id = 0)
        {
            partner p = new partner();
            if (id != 0)
            {
                p = db.partners.Find(Convert.ToInt32(id));
            }
            ViewBag.result = (Result)TempData["result"];
            return View(p);
        }
        public ActionResult PartnerCreateEdit(partner p)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("PartnerIndex");
            }
            result = masters.PartnerCreateEdit(p);
            TempData["result"] = result;
            
            return RedirectToAction("PartnerIndex");
        }
        public ActionResult PartnerEdit(string id)
        {
            var data = (from p in db.partners.AsEnumerable()
                        where p.partner_id == Convert.ToInt32(id)
                        select new
                        {
                            p.partner_id,
                            p.partner_name,
                            p.is_active,
                        }).FirstOrDefault();
            return Json(data);
        }
        public ActionResult PartnerList()
        {
            var list = db.partners.AsEnumerable().ToList();
            var data = (from li in list
                        orderby li.partner_name
                        select new
                        {
                            li.partner_name,
                            li.partner_id,
                            li.is_active
                        }).ToList();
            return Json(data);
        }
        #endregion

        #region Trip Type
        public ActionResult FinancerIndex(int id = 0)
        {
            financer fn = new financer();
            if (id != 0)
            {
                fn = db.financers.Find(Convert.ToInt32(id));
            }
            ViewBag.result = (Result)TempData["result"];
            return View(fn);
        }
        public ActionResult FinancerCreateEdit(financer fn)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("FinancerIndex");
            }
            result = masters.FinancerCreateEdit(fn);
            TempData["result"] = result;
            
            return RedirectToAction("FinancerIndex");
        }
        public ActionResult FinancerEdit(string id)
        {
            var data = (from fn in db.financers.AsEnumerable()
                        where fn.financer_id == Convert.ToInt32(id)
                        select new
                        {
                            fn.financer_id,
                            fn.financer_name,
                            fn.is_active,
                        }).FirstOrDefault();
            return Json(data);
        }
        public ActionResult FinancerList()
        {
            var list = db.financers.AsEnumerable().ToList();
            var data = (from li in list
                        orderby li.financer_name
                        select new
                        {
                            li.financer_name,
                            li.financer_id,
                            li.is_active,
                        }).ToList();
            return Json(data);
        }
        #endregion
    }
}