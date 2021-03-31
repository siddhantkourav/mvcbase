using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using baseproject.Models;
using baseproject.Base.Models;
using System.Web.Mvc;

namespace baseproject.Models
{
    public class Masters
    {
        private Result result;
        private BaseEntities db;
        public Masters()
        {
            result = new Result();
            db = new BaseEntities();
        }

        #region DropDownList
        public List<SelectListItem> GetVehicleSelectList()
        {
            var list = (from ct in db.vehicles.Where(ct => ct.is_active)
                        orderby ct.rego_no
                        select new SelectListItem
                        {
                            Text = ct.rego_no + " (" + ct.vehicleOwnership.owner_name + ")",
                            Value = ct.vehicle_id.ToString(),
                        }).ToList();
            return list;
        }
        public List<SelectListItem> GetClientSelectList()
        {
            var list = (from ct in db.clients.Where(ct => ct.is_active)
                        orderby ct.client_name
                        select new SelectListItem
                        {
                            Text = ct.client_name,
                            Value = ct.client_id.ToString(),
                        }).ToList();
            return list;
        }
        public List<SelectListItem> GetTripTypeSelectList()
        {
            var list = (from ct in db.tripTypes.Where(ct => ct.is_active)
                        orderby ct.trip_type_name
                        select new SelectListItem
                        {
                            Text = ct.trip_type_name,
                            Value = ct.trip_type_id.ToString(),
                        }).ToList();
            return list;
        }
        public List<SelectListItem> GetMaterialSelectList()
        {
            var list = (from ct in db.contractMaterials.Where(ct => ct.is_active)
                        orderby ct.contract_material_name
                        select new SelectListItem
                        {
                            Text = ct.contract_material_name,
                            Value = ct.contract_material_id.ToString(),
                        }).ToList();
            return list;
        }
        public List<SelectListItem> GetDriverSelectList()
        {
            var list = (from ct in db.drivers.Where(ct => ct.is_active)
                        orderby ct.driver_name
                        select new SelectListItem
                        {
                            Text = ct.driver_name,
                            Value = ct.driver_id.ToString(),
                        }).ToList();
            return list;
        }
        public List<SelectListItem> GetVehicleOwnerSelectList()
        {
            var list = (from vo in db.vehicleOwnerships.Where(vo => vo.is_active)
                        orderby vo.owner_name
                        select new SelectListItem
                        {
                            Text = vo.owner_name,
                            Value = vo.owner_id.ToString(),
                        }).ToList();
            return list;
        }
        public List<SelectListItem> GetOwnershipTypeSelectList()
        {
            var list = (from vo in db.ownership_type.Where(vo => vo.is_active)
                        orderby vo.ownership_type_name
                        select new SelectListItem
                        {
                            Text = vo.ownership_type_name,
                            Value = vo.ownership_type_id.ToString(),
                        }).ToList();
            return list;
        }
        public List<SelectListItem> GetLocationSelectList()
        {
            var list = (from ct in db.contractLocations.Where(ct => ct.is_active)
                        orderby ct.contract_location_name
                        select new SelectListItem
                        {
                            Text = ct.contract_location_name,
                            Value = ct.contract_location_id.ToString(),
                        }).ToList();
            return list;
        }
        public List<SelectListItem> GetPartnerSelectList()
        {
            var list = (from ct in db.partners.Where(ct => ct.is_active)
                        orderby ct.partner_name
                        select new SelectListItem
                        {
                            Text = ct.partner_name,
                            Value = ct.partner_id.ToString(),
                        }).ToList();
            return list;
        }
        public List<SelectListItem> GetFinancerSelectList()
        {
            var list = (from fn in db.financers.Where(fn => fn.is_active)
                        select new SelectListItem
                        {
                            Text = fn.financer_name,
                            Value = fn.financer_id.ToString(),
                        }).ToList();
            return list;
        }
        //public List<SelectListItem> GetSelectList()
        //{
        //    var list = ().ToList();
        //    return list;
        //}
        #endregion

        #region Master Create Edit
        public Result ClientCreateEdit(client c)
        {
            var data = new client();
            try
            {
                if (c.client_id > 0)
                {
                    data = db.clients.Find(c.client_id);
                    data.client_name = c.client_name;
                    data.is_active = c.is_active;
                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    c.is_active = true;
                    db.clients.Add(c);
                    db.SaveChanges();
                }
                result.title = "Contract";
                result.MessageType = MessageType.Success;
                result.Message = "Successfully Saved";
            }
            catch (Exception ex)
            {
                result.title = "Contract";
                result.MessageType = MessageType.Error;
                result.Message = "!Not Saved";
            }
            return result;
        }
        public Result DriverCreateEdit(driver d)
        {
            var data = new driver();
            try
            {
                if (d.driver_id > 0)
                {
                    data = db.drivers.Find(d.driver_id);
                    data.driver_name = d.driver_name;
                    data.driver_contact_no = d.driver_contact_no;
                    data.is_active = d.is_active;
                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    d.is_active = true;
                    db.drivers.Add(d);
                    db.SaveChanges();
                }
                result.title = "Driver";
                result.MessageType = MessageType.Success;
                result.Message = "Successfully Saved";
            }
            catch (Exception ex)
            {
                result.title = "Driver";
                result.MessageType = MessageType.Error;
                result.Message = "!Not Saved";
            }
            return result;
        }
        public Result VehicleOwnerCreateEdit(vehicleOwnership vo)
        {
            var data = new vehicleOwnership();
            try
            {
                if (vo.owner_id > 0)
                {
                    data = db.vehicleOwnerships.Find(vo.owner_id);
                    data.owner_name = vo.owner_name;
                    data.owner_contact_no = vo.owner_contact_no;
                    data.is_active = vo.is_active;
                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    vo.is_active = true;
                    db.vehicleOwnerships.Add(vo);
                    db.SaveChanges();
                }
                result.title = "Vehicle Owner";
                result.MessageType = MessageType.Success;
                result.Message = "Successfully Saved";
            }
            catch (Exception ex)
            {
                result.title = "Vehicle Owner";
                result.MessageType = MessageType.Error;
                result.Message = "!Not Saved";
            }
            return result;
        }
        public Result VehicleCreateEdit(vehicle v)
        {
            var data = new vehicle();
            try
            {
                if (v.vehicle_id > 0)
                {
                    if (db.vehicles.Where(x => x.rego_no == v.rego_no && x.vehicle_id != v.vehicle_id).Count() == 0)
                    {
                        data = db.vehicles.Find(v.vehicle_id);
                        data.rego_no = v.rego_no;
                        data.ownership_type_id = v.ownership_type_id;
                        data.vehicle_type = v.vehicle_type;
                        data.manufacturing_year = v.manufacturing_year;
                        data.chassis_no = v.chassis_no;
                        data.engine_no = v.engine_no;
                        data.vehicle_owner_id = v.vehicle_owner_id;
                        data.is_active = v.is_active;
                        db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else
                {
                    if (db.vehicles.Where(x => x.rego_no == v.rego_no).Count() == 0)
                    {
                        v.is_active = true;
                        db.vehicles.Add(v);
                        db.SaveChanges();
                    }
                }
                result.title = "Vehicle";
                result.MessageType = MessageType.Success;
                result.Message = "Successfully Saved";
            }
            catch (Exception ex)
            {
                result.title = "Vehicle";
                result.MessageType = MessageType.Error;
                result.Message = "!Not Saved";
            }
            return result;
        }
        public Result TripTypeCreateEdit(tripType tt)
        {
            var data = new tripType();
            try
            {
                if (tt.trip_type_id > 0)
                {
                    data = db.tripTypes.Find(tt.trip_type_id);
                    data.trip_type_name = tt.trip_type_name;
                    data.is_active = tt.is_active;
                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    tt.is_active = true;
                    db.tripTypes.Add(tt);
                    db.SaveChanges();
                }
                result.title = "Trip Type";
                result.MessageType = MessageType.Success;
                result.Message = "Successfully Saved";
            }
            catch (Exception ex)
            {
                result.title = "Trip Type";
                result.MessageType = MessageType.Error;
                result.Message = "!Not Saved";
            }
            return result;
        }
        public Result MaterialCreateEdit(contractMaterial cm)
        {
            var data = new contractMaterial();
            try
            {
                if (cm.contract_material_id > 0)
                {
                    data = db.contractMaterials.Find(cm.contract_material_id);
                    data.contract_material_name = cm.contract_material_name;
                    data.is_active = cm.is_active;
                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    cm.is_active = true;
                    db.contractMaterials.Add(cm);
                    db.SaveChanges();
                }
                result.title = "Material";
                result.MessageType = MessageType.Success;
                result.Message = "Successfully Saved";
            }
            catch (Exception ex)
            {
                result.title = "Material";
                result.MessageType = MessageType.Error;
                result.Message = "!Not Saved";
            }
            return result;
        }
        public Result LocationCreateEdit(contractLocation cl)
        {
            var data = new contractLocation();
            try
            {
                if (cl.contract_location_id > 0)
                {
                    data = db.contractLocations.Find(cl.contract_location_id);
                    data.contract_location_name = cl.contract_location_name;
                    data.is_active = cl.is_active;
                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    cl.is_active = true;
                    db.contractLocations.Add(cl);
                    db.SaveChanges();
                }
                result.title = "Location";
                result.MessageType = MessageType.Success;
                result.Message = "Successfully Saved";
            }
            catch (Exception ex)
            {
                result.title = "Location";
                result.MessageType = MessageType.Error;
                result.Message = "!Not Saved";
            }
            return result;
        }
        public Result PartnerCreateEdit(partner p)
        {
            var data = new partner();
            try
            {
                if (p.partner_id > 0)
                {
                    data = db.partners.Find(p.partner_id);
                    data.partner_name = p.partner_name;
                    data.is_active = p.is_active;
                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    p.is_active = true;
                    db.partners.Add(p);
                    db.SaveChanges();
                }
                result.title = "Partner";
                result.MessageType = MessageType.Success;
                result.Message = "Successfully Saved";
            }
            catch (Exception ex)
            {
                result.title = "Partner";
                result.MessageType = MessageType.Error;
                result.Message = "!Not Saved";
            }
            return result;
        }
        public Result FinancerCreateEdit(financer fn)
        {
            var data = new financer();
            try
            {
                if (fn.financer_id > 0)
                {
                    data = db.financers.Find(fn.financer_id);
                    data.financer_name = fn.financer_name;
                    data.is_active = fn.is_active;
                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    fn.is_active = true;
                    db.financers.Add(fn);
                    db.SaveChanges();
                }
                result.title = "Financer";
                result.MessageType = MessageType.Success;
                result.Message = "Successfully Saved";
            }
            catch (Exception ex)
            {
                result.title = "Financer";
                result.MessageType = MessageType.Error;
                result.Message = "!Not Saved";
            }
            return result;
        }
        #endregion

        public bool GetVehicleIsowned(int id)
        {
            var count = db.vehicles.Where(x => x.ownership_type_id == 1 && x.vehicle_id == id).Count();
            return count == 0 ? true : false;
        }
    }
}