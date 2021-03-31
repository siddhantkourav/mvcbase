using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using baseproject.Base.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using baseproject.Models;

namespace baseproject.Base.Models
{
    #region Classes & Enum

    public class ControllerAction
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool IsAssigned { get; set; }
    }

    public class AssignController
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool IsAssigned { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
    }

    public enum UserInfo
    {
        UserID, LoginID, EmailID, Mobile, RoleName, RoleID, CompanyID, CompanyName
    }
    #endregion

    #region Business
    public static class RoleUtil
    {
        private static TransportCorp_DBEntities db = new TransportCorp_DBEntities();
        private static Result result;

        public static List<SelectListItem> GetRoleSelectList()
        {
            var list = (from role in db.AspNetRoles.Distinct()
                        where role.Name != "SuperAdmin"
                        select new SelectListItem
                        {
                            Text = role.Name,
                            Value = role.Name,
                        }).ToList();
            return list;
        }

        public static string GetIPAddress()
        {
            return GetIp(new HttpRequestWrapper(HttpContext.Current.Request));
        }
        private static string GetIp(HttpRequestBase request)
        {
            string VisitorsIPAddress = string.Empty;
            try
            {
                if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    VisitorsIPAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else if (HttpContext.Current.Request.UserHostAddress.Length != 0)
                {
                    VisitorsIPAddress = HttpContext.Current.Request.UserHostAddress;
                }
                //string hostName = System.Net.Dns.GetHostName(); // Retrive the Name of HOST  
                //// Get the IP
                // VisitorsIPAddress = System.Net.Dns.GetHostByName(hostName).AddressList[0].ToString();
            }
            catch (Exception ex)
            {
            }
            return VisitorsIPAddress;
        }

        #region Set Session
        public static void SetSession()
        {
            //var a = HttpContext.Current.User.Identity.Name;
            //var b = User.Identity.GetUserId();
            //var user = UserManager.FindByIdAsync(User.Identity.GetUserId());
            //var user = UserManager.FindById(User.Identity.GetUserId());
            //BaseUtil.SetSessionValue(UserInfo.UserID.ToString(), user.Id);
            //BaseUtil.SetSessionValue(UserInfo.LoginID.ToString(), user.UserName);
            //BaseUtil.SetSessionValue(UserInfo.EmailID.ToString(), user.Email);
            //BaseUtil.SetSessionValue(UserInfo.Mobile.ToString(), user.PhoneNumber);
            //BaseUtil.SetSessionValue(UserInfo.RoleName.ToString(), Roles.GetRolesForUser(user.UserName).FirstOrDefault());
            //BaseUtil.SetSessionValue(UserInfo.RoleID.ToString(), user.Roles.FirstOrDefault().RoleId);
        }
        #endregion
    }
    #endregion
}