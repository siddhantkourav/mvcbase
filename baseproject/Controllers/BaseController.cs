using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using baseproject.Base.Models;
using baseproject.Models;

namespace baseproject.Controllers
{
    public class BaseController : Controller
    {
        // GET: /Base/
        public BaseEntities db = null;
        public Result result;
        public Masters masters;
        public BaseModel BaseModel { get; set; }
        public BaseController()
        {
            db = new BaseEntities();
            masters = new Masters();
            this.BaseModel = new BaseModel();
            this.BaseModel.ControllerName = this.ToString().Split('.')[this.ToString().Split('.').Length - 1];
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //filterContext.ActionDescriptor.ActionName.ToUpper().Equals("INDEX") &&
            String ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToUpper();
            String ActionName = filterContext.ActionDescriptor.ActionName.ToUpper();
            String IpAddress = string.Empty;
            int UserId = 0;
            String domainUrl = string.Empty;
            String visited_url = string.Empty;

            if (BaseUtil.ListControllerExcluded().Contains(ControllerName))
            {
                return;
            }
            else if (!User.Identity.IsAuthenticated)
            {
                filterContext.Result = null;
                filterContext.Result = new RedirectResult("/Account/Login/");
                return;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}