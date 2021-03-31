using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using baseproject.Base.Models;

namespace baseproject.Models
{
    public class Reports
    {
        private Result result;
        private BaseEntities db;
        public Reports()
        {
            result = new Result();
            db = new BaseEntities();
        }
        public Result GetTripReport(int vehicle_id = 0, DateTime? trip_date_from = null, DateTime? trip_date_to = null, int client_id = 0, int material_id = 0, int partner_id = 0)
        {
            List<trip> list = new List<trip>();
            list = db.trips.AsEnumerable().Where(x => x.is_active).ToList();
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
            if (material_id > 0)
            {
                list = list.Where(x => x.contract.contract_material_id == material_id).ToList();
            }
            if (partner_id > 0)
            {
                list = list.Where(x => x.contract.partner_id == partner_id).ToList();
            }

            result.Object = (from li in list
                             orderby li.created_date
                             select new
                             {
                                 li.contract_id,
                                 li.trip_id,
                                 li.trip_driver_id,
                                 trip_date = li.trip_date.ToString("dd-MMM-yyyy"),
                                 li.vehicle.rego_no,
                                 li.client.client_name,
                                 li.contract.contractMaterial.contract_material_name,
                                 li.contract.client_contract_rate,
                                 li.contract_partner_rate,
                                 li.driver.driver_name,
                                 tripfrom_to = li.contract.contractLocation.contract_location_name + "-" + li.contract.contractLocation1.contract_location_name,
                                 li.material_weight,
                                 freight = li.material_weight * li.contract.client_contract_rate ?? 0,
                                 li.diesel_exp,
                                 li.rokad,
                                 li.bitti,
                                 li.other_exp,
                                 trip_balance = (li.material_weight * li.contract.client_contract_rate ?? 0) - (li.diesel_exp + li.rokad + li.bitti + li.other_exp),
                                 partner_cost = (li.material_weight * li.contract_partner_rate ?? 0),
                                 trip_margin = (li.material_weight * li.contract.client_contract_rate ?? 0) - (li.material_weight * li.contract_partner_rate ?? 0),
                                 partner_balance = (li.material_weight * li.contract_partner_rate ?? 0) - (li.diesel_exp + li.rokad + li.bitti + li.other_exp),
                                 financer_name = li.financer != null ? li.financer.financer_name : "",
                                 li.tripType.trip_type_name,
                                 li.vehicle.vehicleOwnership.owner_name,
                                 description = li.description ?? ""
                             }).ToList();
            return result;
        }
        public Result GetTotalTripscal(int vehicle_id = 0, DateTime? trip_date_from = null, DateTime? trip_date_to = null, int client_id = 0, int material_id = 0, int partner_id = 0)
        {
            List<trip> list = new List<trip>();
            list = db.trips.AsEnumerable().Where(x => x.is_active).ToList();
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
            if (material_id > 0)
            {
                list = list.Where(x => x.contract.contract_material_id == material_id).ToList();
            }
            if (partner_id > 0)
            {
                list = list.Where(x => x.contract.partner_id == partner_id).ToList();
            }

            result.Object = (new
            {
                total_trip = list.Count(),
                total_weight = list.Sum(x => x.material_weight),
                total_trip_cost = list.Sum(x => x.material_weight * x.contract.client_contract_rate),
                total_expenses = list.Sum(x => x.diesel_exp + x.rokad + x.bitti + x.other_exp),
            });
            
            return result;
        }
    }
}