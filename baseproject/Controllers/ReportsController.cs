using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using baseproject.Base.Models;
using baseproject.Models;

namespace baseproject.Controllers
{
    public class ReportsController : BaseController
    {
        Reports reports;
        public ReportsController()
        {
            masters = new Masters();
            reports = new Reports();
            result = new Result();
        }
        // GET: Reports
        public ActionResult TripReport()
        {
            ViewBag.vehicle_id = new SelectList(masters.GetVehicleSelectList(), "Value", "Text");
            ViewBag.client_id = new SelectList(masters.GetClientSelectList(), "Value", "Text");
            ViewBag.trip_type_id = new SelectList(masters.GetTripTypeSelectList(), "Value", "Text");
            ViewBag.contract_material_id = new SelectList(masters.GetMaterialSelectList(), "Value", "Text");
            ViewBag.trip_driver_id = new SelectList(masters.GetDriverSelectList(), "Value", "Text");
            ViewBag.partner_id = new SelectList(masters.GetPartnerSelectList(), "Value", "Text");
            ViewBag.trip_from_to = new SelectList(masters.GetLocationSelectList(), "Value", "Text");

            return View();
        }
        public ActionResult GetTripReportList(int vehicle_id = 0, DateTime? trip_date_from = null, DateTime? trip_date_to = null, int client_id = 0, int material_id = 0, int partner_id = 0)
        {
            result = reports.GetTripReport(vehicle_id, trip_date_from, trip_date_to, client_id, material_id, partner_id);
            return Json(result.Object);
        }
        public ActionResult GetTotalTripscal(int vehicle_id = 0, DateTime? trip_date_from = null, DateTime? trip_date_to = null, int client_id = 0, int material_id = 0, int partner_id = 0)
        {
            result = reports.GetTotalTripscal(vehicle_id, trip_date_from, trip_date_to, client_id, material_id, partner_id);
            return Json(result.Object);
        }
    }
}