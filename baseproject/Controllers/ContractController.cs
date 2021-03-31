using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using baseproject.Models;
using baseproject.Base.Models;
using System.Security.Cryptography;

namespace baseproject.Controllers
{
    public class ContractController : BaseController
    {
        bool success_flag = false;
        public ContractController()
        {
            db = new BaseEntities();
            masters = new Masters();
            result = new Result();

        }
        // GET: Contract
        public ActionResult Index(int id = 0)
        {
            contract c = new contract();
            //if (id != 0)
            //{
            //    c = db.contracts.Find(Convert.ToInt32(id));
            //}
            ViewBag.client_id = new SelectList(masters.GetClientSelectList(), "Value", "Text", c.client_id);
            ViewBag.contract_material_id = new SelectList(masters.GetMaterialSelectList(), "Value", "Text", c.contract_material_id);
            ViewBag.location_from = new SelectList(masters.GetLocationSelectList(), "Value", "Text", c.location_from);
            ViewBag.location_to = new SelectList(masters.GetLocationSelectList(), "Value", "Text", c.location_to);
            ViewBag.partner_id = new SelectList(masters.GetPartnerSelectList(), "Value", "Text");

            ModelState.AddModelError("", (string)TempData["ModelState1"] ?? "");
            ViewBag.result = (Result)TempData["result"];

            return View(c);
        }
        public ActionResult CreateEditContract(contract cnt)
        {
            var data = new contract();
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            if (db.contracts.Where(d => d.client_id == cnt.client_id && d.contract_material_id == cnt.contract_material_id && d.location_from == cnt.location_from && d.location_to == cnt.location_to && d.is_active && d.contract_id != cnt.contract_id).Count() != 0)
            {
                TempData["ModelState1"] = "No repeated contract will create.";
                return RedirectToAction("Index");
            }
            try
            {
                if (cnt.contract_id > 0)
                {
                    data = db.contracts.Find(cnt.contract_id);
                    data.client_id = cnt.client_id;
                    data.contract_material_id = cnt.contract_material_id;
                    data.client_contract_rate = cnt.client_contract_rate;
                    data.location_from = cnt.location_from;
                    data.location_to = cnt.location_to;
                    data.valid_from = cnt.valid_from;
                    data.valid_to = cnt.valid_to;
                    data.description = cnt.description;
                    data.is_active = cnt.is_active;
                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    success_flag = true;
                }
                else
                {
                    cnt.created_date = DateTime.Now;
                    db.contracts.Add(cnt);
                    db.SaveChanges();
                    success_flag = true;
                }
                if (success_flag)
                {
                    result.title = "Contract";
                    result.MessageType = MessageType.Success;
                    result.Message = "Successfully Saved";
                }
                else
                {
                    result.title = "Contract";
                    result.MessageType = MessageType.Error;
                    result.Message = "!Not Saved";
                }
                TempData["result"] = result;
            }
            catch (Exception ex)
            {

            }

            return RedirectToAction("Index");
        }
        public ActionResult ContractEdit(string id)
        {
            var data = (from c in db.contracts.AsEnumerable()
                        where c.contract_id == Convert.ToInt32(id)
                        select new
                        {
                            c.contract_id,
                            c.client_id,
                            c.location_from,
                            c.location_to,
                            c.contract_material_id,
                            c.client_contract_rate,
                            valid_from = c.valid_from.ToString("dd-MMM-yyyy"),
                            valid_to = c.valid_to.ToString("dd-MMM-yyyy"),
                            c.description,
                            c.is_active
                        }).FirstOrDefault();
            return Json(data);
        }
        public ActionResult ContractList()
        {
            var list = db.contracts.AsEnumerable().ToList();
            var data = (from li in list
                        orderby li.valid_from
                        select new
                        {
                            li.contract_id,
                            li.client.client_name,
                            contract_location_name1 = li.contractLocation.contract_location_name,
                            contract_location_name2 = li.contractLocation1.contract_location_name,
                            li.contractMaterial.contract_material_name,
                            li.client_contract_rate,
                            li.contract_partner_rate,
                            li.partner_id,
                            //partner_name = li.partner != null ? li.partner.partner_name : "",
                            valid_from = li.valid_from.ToString("dd-MMM-yyyy"),
                            valid_to = li.valid_to.ToString("dd-MMM-yyyy"),
                            description = li.description ?? "",
                            li.is_active
                        }).ToList();
            return Json(data);
        }
        public ActionResult GetContractInfo(int id = 0)
        {
            var data = (from c in db.contracts.AsEnumerable()
                        where c.contract_id == Convert.ToInt32(id)
                        select new
                        {
                            c.contract_id,
                            c.client.client_name,
                            material_name = c.contractMaterial.contract_material_name,
                            location = c.contractLocation.contract_location_name + " - " + c.contractLocation1.contract_location_name,
                            c.contract_partner_rate,
                            c.partner_id,
                            c.is_active
                        }).FirstOrDefault();
            return Json(data);
        }
        public ActionResult AddPartner(contract cnt)
        {
            if (cnt.contract_id > 0)
            {
                try
                {
                    var data = db.contracts.Find(cnt.contract_id);
                    //data.partner_id = cnt.partner_id;
                    data.contract_partner_rate = cnt.contract_partner_rate;
                    db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    success_flag = true;

                    if (success_flag)
                    {
                        result.title = "Partner";
                        result.MessageType = MessageType.Success;
                        result.Message = "Successfully Saved";
                    }
                    else
                    {
                        result.title = "Partner";
                        result.MessageType = MessageType.Error;
                        result.Message = "!Not Saved";
                    }
                    TempData["result"] = result;
                }
                catch (Exception ex)
                {
                }
            }
            return RedirectToAction("Index");
        }
    }
}