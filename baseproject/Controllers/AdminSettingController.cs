using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using baseproject.Models;
using baseproject.Base.Models;

namespace baseproject.Controllers
{
    public class AdminSettingController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AdminSettingController()
        {
            result = new Result();
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: AdminSetting
        [Authorize(Roles = "SuperAdmin,admin")]
        public ActionResult Index()
        {
            return View();
        }

        #region UserCreate
        public ActionResult CreateUser()
        {
            ViewBag.rolename = RoleUtil.GetRoleSelectList();
            return View();
        }

        [Authorize(Roles = "SuperAdmin,admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(RegisterViewModel model, string rolename)
        {
            if (ModelState.IsValid)
            {
                rolename = rolename == null || rolename.Length == 0 ? "Employee" : rolename;
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = UserManager.CreateAsync(user, model.Password);
                if (result.Result.Succeeded)
                {
                    if (result.Result.Succeeded)
                    {
                        _ = UserManager.AddToRole(user.Id, rolename);
                        try
                        {
                            usrpasrecord obj = new usrpasrecord();
                            obj.user_hash_id = user.Id;
                            obj.create_date = DateTime.Now;
                            obj.pass = model.Password;
                            db.usrpasrecords.Add(obj);
                            db.SaveChanges();
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    //SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    return RedirectToAction("CreateUser", "AdminSetting");
                }
                foreach (var error in result.Result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize(Roles = "SuperAdmin,admin")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteUser(string id)
        {
            if (id == null)
            {
                result.Message = "Something Is Wrong!";
                result.MessageType = MessageType.Error;
                return Json(result);
            }

            //get User Data from Userid
            var user = UserManager.FindById(id);

            //Delete User
            UserManager.DeleteAsync(user);
            result.Message = "";
            result.MessageType = MessageType.Success;
            return Json(result);
        }

        [Authorize(Roles = "SuperAdmin,admin")]
        public ActionResult UserList(string id)
        {
            var list = db.AspNetUsers.Distinct().ToList();
            var data = (from li in list
                        where li.AspNetRoles.FirstOrDefault().Name != "SuperAdmin"
                        orderby li.UserName
                        select new
                        {
                            li.UserName,
                            li.Id,
                            li.Email,
                            rolename = UserManager.GetRoles(li.Id).FirstOrDefault() ?? ""
                        }).ToList();
            return Json(data);
        }
        #endregion
    }
}