using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using baseproject.Models;
using baseproject.Base.Models;
using System.Web.Helpers;

namespace baseproject.Controllers
{
    public class TripController : BaseController
    {
        bool success_flag = false;
        public TripController()
        {
            masters = new Masters();
            result = new Result();
        }
        // GET: Trip
        public ActionResult Index(int id = 0)
        {
            trip c = new trip();
            ViewBag.client_contract_rate = 0;
            if (id != 0)
            {
                c = db.trips.Find(Convert.ToInt32(id));
                ViewBag.client_contract_rate = c.contract == null ? 0 : c.contract.client_contract_rate;
            }
            ViewBag.vehicle_id = new SelectList(masters.GetVehicleSelectList(), "Value", "Text", c.vehicle_id);
            ViewBag.client_id = new SelectList(masters.GetClientSelectList(), "Value", "Text", c.client_id);
            ViewBag.trip_type_id = new SelectList(masters.GetTripTypeSelectList(), "Value", "Text", c.trip_type_id);
            ViewBag.contract_material_id = new SelectList(masters.GetMaterialSelectList(), "Value", "Text", c.contract == null ? 0 : c.contract.contract_material_id);
            ViewBag.trip_driver_id = new SelectList(masters.GetDriverSelectList(), "Value", "Text", c.trip_driver_id);
            ViewBag.financer_id = new SelectList(masters.GetFinancerSelectList(), "Value", "Text", c.financer_id);

            ViewBag.result = (Result)TempData["result"];
            return View(c);
        }
        public ActionResult Getcontract(string id)
        {
            int clientID = 0;
            int materialID = 0;
            string[] info = id.Split(',');
            if (id.Length > 0)
            {
                clientID = String.IsNullOrEmpty(info[0]) ? 0 : Convert.ToInt32(info[0]);
                materialID = String.IsNullOrEmpty(info[1]) ? 0 : Convert.ToInt32(info[1]);
            }
            var data = new SelectList((from li in db.contracts
                                       where li.client_id == clientID && li.contract_material_id == materialID && li.is_active
                                       select new SelectListItem
                                       {
                                           Value = li.contract_id.ToString(),
                                           Text = li.contractLocation.contract_location_name + "-" + li.contractLocation1.contract_location_name,
                                       }).ToList(), "Value", "Text");
            return Json(data);
        }
        public ActionResult GetcontractRate(int id)
        {
            var data = (from li in db.contracts
                        where li.contract_id == id && li.is_active
                        select new
                        {
                            li.contract_id,
                            li.client_contract_rate,
                            li.contract_partner_rate,
                            //li.partner.partner_name,
                        }).FirstOrDefault();
            return Json(data);
        }
        public ActionResult GetPartnerByVehicleOwner(int id)
        {
            var data = (from li in db.vehicles
                        where li.vehicle_id == id && li.is_active
                        select new
                        {
                            li.vehicle_id,
                            li.ownership_type_id,
                            li.ownership_type.ownership_type_name
                        }).FirstOrDefault();
            return Json(data);
        }
        public ActionResult CreateEditTrip(trip tp)
        {
            var data = new trip();
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            try
            {
                if (masters.GetVehicleIsowned(tp.vehicle_id))
                {
                    tp.contract_partner_rate = 0;
                }
                if (tp.trip_id > 0)
                {
                    data = db.trips.Find(tp.trip_id);
                    data.vehicle_id = tp.vehicle_id;
                    data.trip_date = tp.trip_date;
                    data.trip_type_id = tp.trip_type_id;
                    data.financer_id = tp.financer_id;
                    data.client_id = tp.client_id;
                    data.contract_id = tp.contract_id;
                    data.contract_partner_rate = tp.contract_partner_rate;
                    data.material_weight = tp.material_weight;
                    data.trip_driver_id = tp.trip_driver_id;
                    data.diesel_exp = tp.diesel_exp;
                    data.rokad = tp.rokad;
                    data.bitti = tp.bitti;
                    data.other_exp = tp.other_exp;
                    data.is_active = tp.is_active;
                    data.description = data.description + " " + tp.description;
                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    success_flag = true;

                    ////////////////////////// Code for update partner contract rate if not match trip partner rate //////////////////
                    var contaract_data = db.contracts.Find(tp.contract_id);
                    if (contaract_data.contract_partner_rate != tp.contract_partner_rate)
                    {
                        contaract_data.contract_partner_rate = tp.contract_partner_rate;
                        db.Entry(contaract_data).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        success_flag = true;
                    }
                }
                else
                {
                    tp.created_date = DateTime.Now;
                    db.trips.Add(tp);
                    db.SaveChanges();
                    success_flag = true;
                }
                if (success_flag)
                {
                    result.title = "Trip";
                    result.MessageType = MessageType.Success;
                    result.Message = "Successfully Saved";
                }
                else
                {
                    result.title = "Trip";
                    result.MessageType = MessageType.Error;
                    result.Message = "!Not Saved";
                }
                TempData["result"] = result;
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("TripList");
        }
        public ActionResult TripList()
        {
            ViewBag.vehicle_id = new SelectList(masters.GetVehicleSelectList(), "Value", "Text");
            ViewBag.client_id = new SelectList(masters.GetClientSelectList(), "Value", "Text");

            ViewBag.result = (Result)TempData["result"];
            return View();
        }
        public ActionResult GetTripList(int vehicle_id = 0, DateTime? trip_date_from = null, DateTime? trip_date_to = null, int client_id = 0)
        {
            List<trip> list = new List<trip>();
            list = db.trips.AsEnumerable().ToList();
            if (vehicle_id > 0)
            {
                list = list.Where(x => x.vehicle_id == vehicle_id).ToList();
            }
            if (trip_date_from != null && trip_date_from != DateTime.MinValue)
            {
                list = list.Where(x => x.trip_date >= trip_date_from).ToList();
            }
            if (trip_date_to != null && trip_date_to != DateTime.MinValue)
            {
                list = list.Where(x => x.trip_date <= trip_date_to).ToList();
            }
            if (client_id > 0)
            {
                list = list.Where(x => x.client_id == client_id).ToList();
            }

            var data = (from li in list
                        orderby li.created_date
                        select new
                        {
                            li.contract_id,
                            li.client.client_name,
                            trip_date = li.trip_date.ToString("dd-MMM-yyyy"),
                            li.trip_id,
                            li.trip_driver_id,
                            financer_name = li.financer != null ? li.financer.financer_name : "",
                            li.driver.driver_name,
                            li.tripType.trip_type_name,
                            li.vehicle.rego_no,
                            li.vehicle.vehicleOwnership.owner_name,
                            li.contract.contractMaterial.contract_material_name,
                            tripfrom_to = li.contract.contractLocation.contract_location_name + "-" + li.contract.contractLocation1.contract_location_name,
                            li.contract.client_contract_rate,
                            li.contract_partner_rate,
                            li.material_weight,
                            li.diesel_exp,
                            li.rokad,
                            li.bitti,
                            li.other_exp,
                            description = li.description ?? "",
                            li.is_active
                        }).ToList();
            return Json(data);
        }
    }
}