using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Mvc.Ajax;
using System.Web.Routing;
using System.ComponentModel;
using System.Data.Entity.Validation;
using System.Web.Script.Serialization;
using System.Linq.Expressions;
using System.Web.Security;
using System.Data;
using System.Collections;
using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using context = System.Web.HttpContext;
using baseproject.Models;
using Microsoft.AspNet.Identity;

namespace baseproject.Base.Models
{
    #region Enum & Classes
    public enum datetypes
    {
        ThisWeek = 1,
        LastWeek = 2,
        ThisMonth = 3,
        LastMonth = 4,
        ThisYear = 5,
        LastYear = 6
    }

    public class BaseEntities : TransportCorp_DBEntities
    {
        public override int SaveChanges()
        {
            //Add/New...
            var entitiesNew = this.ChangeTracker.Entries().Where(e => e.State == System.Data.Entity.EntityState.Added);
            try
            {
                foreach (var entry in entitiesNew)
                {
                    var entity = entry.Entity;
                    if (entity != null)
                    {
                        //Int32 systemID = Convert.ToInt32(BaseUtil.GetWebConfigValue("SYSTEM_ID"));
                        //Int32 companyID = Convert.ToInt32(BaseUtil.GetWebConfigValue("COMPANY_ID"));

                        //if (BaseUtil.IsAuthenticated())
                        //{
                        //    //if (BaseUtil.GetSessionValue(UserInfo.IsCompanyAddUpdate.ToString()) == "1")
                        //    //{
                        //    //    BaseUtil.SetProperty(entity, "company_id", BaseUtil.IsAuthenticated() ? SessionUtil.GetCompanyID() : companyID);
                        //    //}
                        //    BaseUtil.SetProperty(entity, "is_active", true);

                        //    //BaseUtil.SetProperty(entity, "is_login_enable", true);
                         //   BaseUtil.SetProperty(entity, "created_by", HttpContext.Current.User.Identity.GetUserId());
                        //    BaseUtil.SetProperty(entity, "created_date", DateTime.Now);
                        //}
                    }
                }
                //Update/Modified...
                var entitiesModified = this.ChangeTracker.Entries().Where(e => e.State == System.Data.Entity.EntityState.Modified);
                foreach (var entry in entitiesModified)
                {
                    var entity = entry.Entity;
                    if (entity != null)
                    {
                        //Int32 systemID = Convert.ToInt32(BaseUtil.GetWebConfigValue("SYSTEM_ID"));
                        //Int32 companyID = Convert.ToInt32(BaseUtil.GetWebConfigValue("COMPANY_ID"));
                        //if (BaseUtil.IsAuthenticated())
                        //{
                        //    //if (BaseUtil.GetSessionValue(UserInfo.IsCompanyAddUpdate.ToString()) == "1")
                        //    //{
                        //    //    BaseUtil.SetProperty(entity, "company_id", BaseUtil.IsAuthenticated() ? SessionUtil.GetCompanyID() : companyID);
                        //    //}
                        //    BaseUtil.SetProperty(entity, "modify_by", HttpContext.Current.User.Identity.GetUserId());
                        //    BaseUtil.SetProperty(entity, "modify_date", DateTime.Now);
                        //}
                    }
                }
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                var fullErrorMessage = string.Join("; ", errorMessages);

                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
    }
    //public class User
    //{
    //    internal static readonly object Identity;
    //    public int UserID { get; set; }
    //    public string Name { get; set; }
    //}
    public static class DbContextExt
    {
        public static void AddEntity<T>(this DbContext db, T obj) where T : class
        {
            //db.Set<T>().Add(obj);
            db.Entry<T>(obj).State = EntityState.Added;
        }
        public static T GetEntity<T>(this DbContext db, T obj, object id) where T : class
        {
            //db.Set<T>().Add(obj);
            //return db.Set<T>().Find(id);
            //var t = obj.GetType();
            return db.Set<T>().Find(id);
        }
    }
    public static class EnumUtil
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        public static string GetJsEnum<T>()
        {
            StringBuilder jsStr = new StringBuilder();
            string enumName = typeof(T).Name.ToString();
            var enumNo = Enum.GetValues(typeof(T)).Cast<int>().Select(x => x.ToString()).ToArray();
            var enumVal = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
            for (int i = 0; i < enumNo.Length; i++)
            {
                if (i == 0 && enumNo.Length > 1)
                {
                    jsStr.Append(string.Format("var {0} ={{", enumName));
                }
                jsStr.Append(string.Format("{0}: {1},", enumVal[i], enumNo[i]));
            }
            jsStr.Append("}");
            return jsStr.ToString();
            //"var enum1 ={Success: 1,Info: 2,}"
        }
        public static string ParseName<T>(T value)
        {
            return Convert.ToString(Enum.GetName(typeof(T), value));
        }
        public static string GetJsConst(Type type)
        {
            StringBuilder jsStr = new StringBuilder();
            string constName = type.Name.ToString();
            List<string> list = new List<string>();
            FieldInfo[] fields = type.GetFields(BindingFlags.Public
                | BindingFlags.Static
                | BindingFlags.FlattenHierarchy);

            for (int i = 0; i < fields.Length; i++)
            {
                var fieldInfo = fields[i];
                if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly && fieldInfo.FieldType == typeof(string))
                {
                    if (i == 0 && fields.Length > 1)
                    {
                        jsStr.Append(string.Format("{0} ={{}};\n", constName));
                    }
                    string str = constName + "." + fieldInfo.Name + "=\"" + fieldInfo.GetRawConstantValue() + "\";\n";
                    jsStr.Append(str);
                }
            }
            return jsStr.ToString();
        }
        public static string GetJs(object obj, string name)
        {
            StringBuilder Js = new StringBuilder();
            var obj_js = JsonConvert.SerializeObject((obj), Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = BaseConst.DATE });
            Js.Append("<script>\r\n");
            Js.Append(string.Format("json_{0} = {1}", name, obj_js));
            Js.Append("</script>\r\n");
            string line = Convert.ToString(Js);
            return (string.IsNullOrEmpty(line) ? "" : line);
        }
    }

    public static class BaseConst
    {
        public const string VALIDATION_ISREQUIRED = "data-valc-isrequired";
        public const string VALIDATION_REQ_MSG = "data-valc-required-msg";
        public const string VALIDATION_ID = "data-valc-validation-id";
        public const string HTML_PREFIX = "dmx";
        public const string DATE = "dd-MMM-yyyy";
        public const string DATE_NUM = "dd-MM-yyyy";
        public const string MONTH_year = "MMM-yyyy";
        public const string MONTH_YEAR = "MMM-YYYY";
        public const string year_MONTH = "yyyy-MMM";
        public const string TIME = "hh:mm:ss";
        public const string TIME24 = "HH:mm";
        public const string DATETIME = "dd-MMM-yyyy hh:mm";
        public const string DATETIME24 = "dd-MMM-yyyy HH:mm";
        public const string MSG_SUCCESS_CREATE = "{0} created successfully";
        public const string MSG_SUCCESS_UPDATE = "{0} updated successfully";
        public const string MSG_SUCCESS_DELETE = "{0} deleted successfully";
        public const string MSG_SUCCESS_UNLOCK = "{0} unlocked successfully";
        public const string MSG_SUCCESS_EMAIL = "{0} sent successfully";
        public const string MSG_INVALID_OLD_PASSWORD = "Old {0} is invalid";
        public const string MSG_FEEDBACK_SENT = "{0} sent successfully";
        public const string MSG_INVALID_OPERATION = "{0} You dont have permission to perform the action.";
    }
    public static class DateFormat
    {
        public const string DATE = "dd-MMM-yyyy";
        public const string DATE_START_WITH_MONTH = "MM/dd/yyyy";
        public const string DATE_NUM = "dd-MM-yyyy";
        public const string MONTH_year = "MMM-yyyy";
        public const string MONTH_YEAR = "MMM-YYYY";
        public const string year_MONTH = "yyyy-MMM";
    }
    public static class DateTimeFormat
    {
        public const string TIME = "hh:mm:ss";
        public const string TIME24 = "HH:mm";
        public const string TIME_AM_PM = "hh:mm tt";
        public const string DATETIME = "dd-MMM-yyyy hh:mm";
        public const string DATETIME24 = "dd-MMM-yyyy HH:mm";
        public static string GetTimeFormateByTime(TimeSpan t, string formate = "")
        {
            TimeSpan timespan = new TimeSpan(t.Hours, t.Minutes, t.Seconds);
            DateTime time = DateTime.Today.Add(timespan);
            string displayTime = string.IsNullOrEmpty(formate) ? time.ToString(TIME_AM_PM) : time.ToString(formate);
            return displayTime;
        }
    }

    public enum Gender
    {
        M = 1,
        F = 2
    }
    #endregion

    #region ConvertUtil
    public static class ConvertUtil
    {
        public static string NumberToWords(double doubleNumber)
        {
            doubleNumber = Convert.ToDouble(doubleNumber.ToString("#.##"));
            var beforeFloatingPoint = (int)Math.Floor(doubleNumber);
            var beforeFloatingPointWord = NumberToWords(beforeFloatingPoint);
            double after_point_value = 0;
            var ponit_value = doubleNumber - (double)beforeFloatingPoint;
            if (ponit_value > 0)
            {
                var afterpont_str_value = Convert.ToString(doubleNumber).Split('.')[1];
                after_point_value = Convert.ToDouble("." + afterpont_str_value);
                var afterFloatingPointWord = SmallNumberToWord((int)((after_point_value) * 100), "");
                return beforeFloatingPointWord + " and " + afterFloatingPointWord + " fils";

            }
            else
            {
                return beforeFloatingPointWord;
            }
            //var afterFloatingPointWord = SmallNumberToWord((int)((doubleNumber - beforeFloatingPoint) * 100), "");
            //var afterFloatingPointWord = SmallNumberToWord((int)after_point_value, "");

            //string t3 = beforeFloatingPointWord + " point " + afterFloatingPointWord;

            //return beforeFloatingPointWord + " point " + afterFloatingPointWord;
        }
        public static string UpperCaseStartingWord(string value)
        {
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }
        public static string PascalCase(this string word)
        {
            return string.Join(" ", word.Split(' ')
                         .Select(w => w.Trim())
                         .Where(w => w.Length > 0)
                         .Select(w => w.Substring(0, 1).ToUpper() + w.Substring(1).ToLower()));
        }
        private static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            var words = "";

            if (number / 1000000000 > 0)
            {
                words += NumberToWords(number / 1000000000) + " billion ";
                number %= 1000000000;
            }

            if (number / 1000000 > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if (number / 1000 > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            words = SmallNumberToWord(number, words);

            return words;
        }
        private static string SmallNumberToWord(int number, string words)
        {
            if (number <= 0) return words;
            if (words != "")
                words += " ";

            var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += " " + unitsMap[number % 10];
            }
            return words;
        }
    }

    #endregion

    #region BaseUtil
    public static class BaseUtil
    {
        private static BaseEntities db = new BaseEntities();

        #region Date & Time
        public static DateTime? GetNullDate()
        {
            return (DateTime?)null;
        }
        #endregion

        #region Configuration
        public static string GetWebConfigValue(string Name)
        {
            return System.Configuration.ConfigurationManager.AppSettings[Name].ToString(); ;
        }
        #endregion

        #region Set Property
        public static void SetProperty(object p, string propName, object value)
        {
            Type t = p.GetType();
            PropertyInfo info = t.GetProperty(propName);
            if (info == null)
                return;
            if (!info.CanWrite)
                return;
            info.SetValue(p, value, null);
        }
        public static object GetValue(object p, string propName)
        {
            return p.GetType().GetProperty(propName).GetValue(p, null);
        }
        #endregion

        #region SMS API
        public static string Apicall(string url)
        {
            HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                HttpWebResponse httpres = (HttpWebResponse)httpreq.GetResponse();
                StreamReader sr = new StreamReader(httpres.GetResponseStream());
                string results = sr.ReadToEnd();
                sr.Close();
                return results;
            }
            catch
            {
                return "0";
            }
        }
        #endregion

        #region Session
        public static void SetSessionValue(String Key, String Value)
        {
            HttpContext.Current.Session[Key] = Value;
        }
        public static String GetSessionValue(String Key)
        {
            return HttpContext.Current.Session[Key] != null ? HttpContext.Current.Session[Key].ToString() : string.Empty;
        }
        #endregion

        #region Accesible Pages
        public static List<string> ListControllerExcluded()
        {
            List<string> list = new List<string>() { "BASE", "JSON", "ACCOUNT", "API" };
            return list;
        }
        public static bool CheckAuthentication(ActionExecutingContext filterContext)
        {
            bool result = false;

            string actionName = filterContext.ActionDescriptor.ActionName;
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            String Action = string.Format("{0}Controller{1}", controllerName, actionName).ToUpper();

            //List<string> accesiblePages = AccesiblePages();

            //foreach (var item in accesiblePages)
            //{
            //    if (Action == item)
            //    {
            //        result = true;
            //        break;
            //    }

            //}

            return result;
        }
        #endregion

        #region Login and PWD
        public static bool IsAuthenticated()
        {
            return string.IsNullOrEmpty(GetSessionValue(UserInfo.UserID.ToString())) ? false : true;
        }
        public static string GetRandomPasswordString(int pwdLength)
        {
            int asciiZero;
            int asciiNine;
            int asciiA;
            int asciiZ;
            int count = 0;
            int randNum;
            string randomString;

            System.Random rRandom = new System.Random(System.DateTime.Now.AddMinutes(0).Millisecond);

            asciiZero = 48;
            asciiNine = 57;
            asciiA = 64;
            asciiZ = 90;

            randomString = "";
            while ((count < pwdLength))
            {
                if (count % 2 == 0)
                {
                    randNum = rRandom.Next(asciiZero, asciiNine);
                }
                else
                {
                    randNum = rRandom.Next(asciiA, asciiZ);
                }
                if (((randNum >= asciiZero) && (randNum <= asciiNine)) || ((randNum >= asciiA) && (randNum <= asciiZ)))
                {
                    randomString = (randomString + ((char)(randNum)));
                    count = (count + 1);
                }
            }
            return randomString;
        }
        public static string GetRandomPasswordNumber(int pwdLength)
        {
            int asciiZero;
            int asciiNine;
            int asciiA;
            int asciiZ;
            int count = 0;
            int randNum;
            string randomString;

            System.Random rRandom = new System.Random(System.DateTime.Now.AddMinutes(0).Millisecond);

            asciiZero = 48;
            asciiNine = 57;
            asciiA = 64;
            asciiZ = 90;

            randomString = "";
            while ((count < pwdLength))
            {
                //if (count % 2 == 0)
                //{
                randNum = rRandom.Next(asciiZero, asciiNine);
                //}
                //else
                //{
                //    randNum = rRandom.Next(asciiA, asciiZ);
                //}
                if (((randNum >= asciiZero) && (randNum <= asciiNine)))// || ((randNum >= asciiA) && (randNum <= asciiZ)))
                {
                    randomString = (randomString + ((char)(randNum)));
                    count = (count + 1);
                }
            }
            return randomString;
        }
        #endregion

        #region Application Path
        static HttpContext Context
        {
            get { return HttpContext.Current; }
        }
        static HttpRequest Request
        {
            get { return Context.Request; }
        }
        public static AjaxOptions ConfigAjaxOptions()
        {
            AjaxOptions options = new AjaxOptions();
            options.HttpMethod = "Post";
            options.OnSuccess = "OnSuccess";
            options.OnBegin = "LockUIOnCallback('true')";
            options.LoadingElementId = "PleaseWait";
            options.OnComplete = "LockUIOnCallback('false')";
            options.OnFailure = "OnFail";
            return options;
        }
        public static UrlHelper GetUrlHelper()
        {
            return new UrlHelper(HttpContext.Current.Request.RequestContext);
        }
        public static string GetActionPath(string controllerActionName) //Ex: "Category/Edit"
        {
            return GetApplicationPath(Request.RequestContext) + string.Format("/{0}", controllerActionName);
        }
        public static string GetActionPath(string controllerName, string actionName, string id = null)
        {
            id = string.IsNullOrEmpty(id) ? "" : "/" + id;
            return GetApplicationPath(Request.RequestContext) + string.Format("/{0}/{1}{2}", controllerName, actionName, id);
        }
        public static string GetApplicationPath(RequestContext requestContext)
        {
            string retPath;
            string httpOrigin = Request.ServerVariables["HTTP_ORIGIN"];
            string applicationPath = Request.ApplicationPath;
            //Approach #1: OK:Post
            //retPath = (applicationPath == "/" ? httpOrigin : httpOrigin + applicationPath);
            //Approach #2 OK:All
            retPath = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority) + (applicationPath == "/" ? "" : applicationPath);
            return retPath;
        }
        public static string GetApplicationPath()
        {
            string retPath;
            string applicationPath = Request.ApplicationPath;
            retPath = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority) + (applicationPath == "/" ? "" : applicationPath);
            return retPath;
        }
        public static string GetCurrentController()
        {
            return Convert.ToString(Request.RequestContext.RouteData.Values["controller"]);
        }
        public static string GetCurrentAction()
        {
            return Convert.ToString(Request.RequestContext.RouteData.Values["action"]);
        }
        public static List<string> GetControllerNames()
        {
            List<string> controllerNames = new List<string>();
            GetSubClasses<Controller>().ForEach(
                type => controllerNames.Add(type.Name));
            return controllerNames;
        }
        private static List<Type> GetSubClasses<T>()
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }
        public static List<string> GetControllerActionNames(Type t)
        {
            List<string> list = new List<string>();
            if (t != null)
            {
                list = t.GetMethods().Where(m => m.ReturnType == typeof(ActionResult))
                    .Select(m => m.Name).Distinct().ToList();
            }
            return list;
        }
        public static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
        #endregion

        #region Dropdown
        public static List<SelectListItem> GetListController()
        {
            List<SelectListItem> listAllControler = (from M in GetControllerNames().AsEnumerable()
                                                     orderby M
                                                     select new SelectListItem
                                                     {
                                                         Value = M,
                                                         Text = M.Replace("Controller", ""),
                                                     }).ToList();


            List<SelectListItem> listActual = (from la in listAllControler
                                               where !(from exc in ListControllerExcluded() select exc).Contains(la.Text.ToUpper())
                                               select la).ToList();
            return listActual;
        }
        public static List<SelectListItem> GetListAllActionByController(string controllerName)
        {
            var listAll = (from M in GetControllerActionNames(GetType("baseproject.Controllers." + controllerName)).AsEnumerable()
                           orderby M
                           select new SelectListItem
                           {
                               Value = M,
                               Text = M,
                           }).ToList();
            return listAll;
        }
        #endregion

        #region Calculate Distance
        public static double distance(double lat1, double lon1, double lat2, double lon2, String sr)
        {


            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (sr.Equals("K"))
            {
                dist = dist * 1.609344;
            }
            else if (sr.Equals("N"))
            {
                dist = dist * 0.8684;
            }
            return (dist);

            //double theta = lon1 - lon2;
            //double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            //dist = Math.Abs(Math.Round(rad2deg(Math.Acos(dist)) * 60 * 1.1515 * 1.609344 * 1000, 0));
            //return (dist);
        }
        public static double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }
        public static double rad2deg(double rad)
        {
            return (rad * 180.0 / Math.PI);
        }
        #endregion

        public static string ListToJson(IList<System.Web.WebPages.Html.SelectListItem> listSelected)
        {
            StringBuilder selected = new StringBuilder();
            if (listSelected != null)
            {
                if (listSelected.Count() > 0)
                {
                    //["9", "4"]
                    selected.Append("[");
                    for (int i = 0; i < listSelected.Count(); i++)
                    {
                        string str = "'{0}'" + (i < listSelected.Count() - 1 ? "," : "]");
                        selected.Append(string.Format(str, listSelected[i].Value));
                    }
                }
            }
            return selected.ToString();
        }
        public static List<SelectListItem> GetTimeZoneInfo()
        {

            return (from c in TimeZoneInfo.GetSystemTimeZones()
                    select new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.DisplayName
                    }).ToList();
        }
        public static string TimeAgo(this DateTime dateTime)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(dateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} seconds ago", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format("about {0} minutes ago", timeSpan.Minutes) :
                    "about a minute ago";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format("about {0} hours ago", timeSpan.Hours) :
                    "about an hour ago";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format("about {0} days ago", timeSpan.Days) :
                    "yesterday";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format("about {0} months ago", timeSpan.Days / 30) :
                    "about a month ago";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("about {0} years ago", timeSpan.Days / 365) :
                    "about a year ago";
            }

            return result;
        }
    }
    #endregion

    #region Misc
    public static class SessionUtil
    {
        public static int GetUserID()
        {
            return Convert.ToInt32(BaseUtil.GetSessionValue(UserInfo.UserID.ToString()));
        }
        public static string GetLoginID()
        {
            return BaseUtil.GetSessionValue(UserInfo.LoginID.ToString());
        }
        public static int GetRoleID()
        {
            return Convert.ToInt32(BaseUtil.GetSessionValue(UserInfo.RoleID.ToString()));
        }
        public static int GetRoleName()
        {
            return Convert.ToInt32(BaseUtil.GetSessionValue(UserInfo.RoleName.ToString()));
        }
        public static int GetCompanyID()
        {
            return Convert.ToInt32(BaseUtil.GetSessionValue(UserInfo.CompanyID.ToString()));
        }
    }
    public static class ExUtil
    {
        public static string GetMessage(DbEntityValidationException dbEx)
        {
            string errors = string.Empty;
            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    errors += string.Format("{0}-{1} ", validationError.PropertyName, validationError.ErrorMessage);
                }
            }
            return errors;
        }
    }
    #endregion

    #region Custom Html Helper
    public class RegExConst
    {
        public const string LoginID = "[a-zA-Z0-9_-]{4,20}";
        public const string Password = "^.{4,12}$";
        public const string General = @"^[ A-Za-z0-9_@./,#&+-]{1,500}";
        public const string GeneralWithOutSpace = @"^[A-Za-z0-9_@./,#&+-]{4,500}";
        public const string GeneralWithSpace = @"^[ A-Za-z0-9_@./,#&+-]{4,500}";
        public const string CharacterAndSpaceOnly = "^[a-zA-Z ]*$";
        public const string CharactersAndParanthisisOnly = "^[a-zA-Z ()'.-]+$";
        public const string CharactersParanthisisAndSpecialCharOnly = "^[a-zA-Z() ]*$";
        public const string CharactersAndCommaOnly = "^[a-zA-Z, ]{2,}$";
        public const string CharactersOnly = "^[a-zA-Z]*$";
        public const string NumbersOnly = "^[0-9-+]+$";
        public const string NumbersAnddecimalOnly = "^[0-9]{0,6}(.[0-9]{1,2})?$";
        public const string NumbersAndOneDecimalOnly = "/^/d+(/./d{1,2})?$/";
        public const string NumbersWithPlusOnly = "^[0-9+/]+$";
        public const string NumbersWithPlusAndSpaceOnly = "^[0-9+/ ]+$";
        public const string AlphaNumericOnly = "^[a-zA-Z0-9]*$";
        public const string AlphaNumericAndSpaceOnly = "^[a-z A-Z 0-9 ]*$";//"(?=.*\\S)[a-z A-Z 0-9 \\s]*"; ////////// need to check
        public const string IntegerGreaterthanZero = "^[1-9][0-9]*$";
        public const string DecimalGreaterthanZero = "^\\s*(?=.*[1-9])\\d*(?:\\.\\d{1,2})?\\s*$";
        public const string EmailAddress = "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$";
        public const string EmailAddressMulti = @"^((\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)\s*[,]{0,1}\s*)+$";
        public const string WebAddress = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&amp;=]*)?";
        public const string IpAddress = @"^([\d]{1,3}\.){3}[\d]{1,3}$";
        public const string SkypeId = @"[a-zA-Z][a-zA-Z0-9\.,\-_]{5,31}";
    }
    #endregion

    #region Cookies
    public static class CookieStore
    {
        public static void SetCookie(string key, string value, TimeSpan expires)
        {
            HttpCookie encodedCookie = HttpSecureCookie.Encode(new HttpCookie(key, value), CookieProtection.None);

            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                var cookieOld = HttpContext.Current.Request.Cookies[key];
                cookieOld.Expires = DateTime.Now.Add(expires);
                cookieOld.Value = encodedCookie.Value;
                HttpContext.Current.Response.Cookies.Add(cookieOld);
            }
            else
            {
                encodedCookie.Expires = DateTime.Now.Add(expires);
                HttpContext.Current.Response.Cookies.Add(encodedCookie);
            }
        }
        public static string GetCookie(string key)
        {
            string value = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];

            if (cookie != null)
            {
                // For security purpose, we need to encrypt the value.
                HttpCookie decodedCookie = HttpSecureCookie.Decode(cookie, CookieProtection.None);
                value = decodedCookie.Value;
            }
            return value;
        }
    }

    public static class HttpSecureCookie
    {

        public static HttpCookie Encode(HttpCookie cookie)
        {
            return Encode(cookie, CookieProtection.All);
        }

        public static HttpCookie Encode(HttpCookie cookie,
                      CookieProtection cookieProtection)
        {
            HttpCookie encodedCookie = CloneCookie(cookie);
            encodedCookie.Value = MachineKeyCryptography.Encode(cookie.Value, cookieProtection);
            return encodedCookie;
        }

        public static HttpCookie Decode(HttpCookie cookie)
        {
            return Decode(cookie, CookieProtection.All);
        }

        public static HttpCookie Decode(HttpCookie cookie,
                      CookieProtection cookieProtection)
        {
            HttpCookie decodedCookie = CloneCookie(cookie);
            decodedCookie.Value =
              MachineKeyCryptography.Decode(cookie.Value, cookieProtection);
            return decodedCookie;
        }

        public static HttpCookie CloneCookie(HttpCookie cookie)
        {
            HttpCookie clonedCookie = new HttpCookie(cookie.Name, cookie.Value);
            clonedCookie.Domain = cookie.Domain;
            clonedCookie.Expires = cookie.Expires;
            clonedCookie.HttpOnly = cookie.HttpOnly;
            clonedCookie.Path = cookie.Path;
            clonedCookie.Secure = cookie.Secure;

            return clonedCookie;
        }
    }

    public static class MachineKeyCryptography
    {

        /// <summary>
        /// Encodes a string and protects it from tampering
        /// </summary>
        /// <param name="text">String to encode</param>
        /// <returns>Encoded string</returns>
        public static string Encode(string text)
        {
            return Encode(text, CookieProtection.All);
        }

        /// <summary>
        /// Encodes a string
        /// </summary>
        /// <param name="text">String to encode</param>
        /// <param name="cookieProtection">The method in which the string is protected</param>
        /// <returns></returns>
        public static string Encode(string text, CookieProtection cookieProtection)
        {
            if (string.IsNullOrEmpty(text) || cookieProtection == CookieProtection.None)
            {
                return text;
            }
            byte[] buf = Encoding.UTF8.GetBytes(text);
            return CookieProtectionHelperWrapper.Encode(cookieProtection, buf, buf.Length);
        }

        /// <summary>
        /// Decodes a string and returns null if the string is tampered
        /// </summary>
        /// <param name="text">String to decode</param>
        /// <returns>The decoded string or throws InvalidCypherTextException if tampered with</returns>
        public static string Decode(string text)
        {
            return Decode(text, CookieProtection.All);
        }

        /// <summary>
        /// Decodes a string
        /// </summary>
        /// <param name="text">String to decode</param>
        /// <param name="cookieProtection">The method in which the string is protected</param>
        /// <returns>The decoded string or throws InvalidCypherTextException if tampered with</returns>
        public static string Decode(string text, CookieProtection cookieProtection)
        {
            if (string.IsNullOrEmpty(text) || cookieProtection == CookieProtection.None)
            {
                return text;
            }
            byte[] buf;
            try
            {
                buf = CookieProtectionHelperWrapper.Decode(cookieProtection, text);
            }
            catch (Exception ex)
            {
                throw new InvalidCypherTextException("Unable to decode the text", ex.InnerException);
            }
            if (buf == null || buf.Length == 0)
            {
                throw new InvalidCypherTextException("Unable to decode the text");
            }
            return Encoding.UTF8.GetString(buf, 0, buf.Length);
        }
    }

    public class InvalidCypherTextException : Exception
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidCypherTextException()
            : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidCypherTextException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidCypherTextException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public static class CookieProtectionHelperWrapper
    {

        private static MethodInfo _encode;
        private static MethodInfo _decode;

        /// <summary>
        /// Constructor
        /// </summary>
        static CookieProtectionHelperWrapper()
        {
            // obtaining a reference to System.Web assembly
            Assembly systemWeb = typeof(HttpContext).Assembly;
            if (systemWeb == null)
            {
                throw new InvalidOperationException("Unable to load System.Web.");
            }
            // obtaining a reference to the internal class CookieProtectionHelper
            Type cookieProtectionHelper = systemWeb.GetType("System.Web.Security.CookieProtectionHelper");
            if (cookieProtectionHelper == null)
            {
                throw new InvalidOperationException("Unable to get the internal class System.Web.Security.CookieProtectionHelper.");
            }
            // obtaining references to the methods of CookieProtectionHelper class
            _encode = cookieProtectionHelper.GetMethod("Encode", BindingFlags.NonPublic | BindingFlags.Static);
            _decode = cookieProtectionHelper.GetMethod("Decode", BindingFlags.NonPublic | BindingFlags.Static);

            if (_encode == null || _decode == null)
            {
                throw new InvalidOperationException("Unable to get the methods to invoke.");
            }
        }

        /// <summary>
        /// Wrapper arround CookieProtectionHelper.Encode
        /// </summary>
        /// <param name="cookieProtection">Protection Type</param>
        /// <param name="buf">Bytes buffer to encode</param>
        /// <param name="count">The number of bytes in the buffer</param>
        /// <returns>Encoded text</returns>
        public static string Encode(CookieProtection cookieProtection, byte[] buf, int count)
        {
            return (string)_encode.Invoke(null, new object[] { cookieProtection, buf, count });
        }

        /// <summary>
        /// Wrapper arround CookieProtectionHelper.Decode
        /// </summary>
        /// <param name="cookieProtection">Protection Type</param>
        /// <param name="data">String to decode</param>
        /// <returns>Decoded bytes</returns>
        public static byte[] Decode(CookieProtection cookieProtection, string data)
        {
            return (byte[])_decode.Invoke(null, new object[] { cookieProtection, data });
        }

    }
    #endregion



    #region CSV Utility
    public static class CSVUtility
    {
        public static MemoryStream GetCSV(DataTable data)
        {
            string[] fieldsToExpose = new string[data.Columns.Count];
            for (int i = 0; i < data.Columns.Count; i++)
            {
                fieldsToExpose[i] = data.Columns[i].ColumnName;
            }
            return GetCSV(fieldsToExpose, data);
        }

        public static MemoryStream GetCSV(string[] fieldsToExpose, DataTable data)
        {
            MemoryStream stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                for (int i = 0; i < fieldsToExpose.Length; i++)
                {
                    if (i != 0) { writer.Write(","); }
                    writer.Write("\"");
                    writer.Write(fieldsToExpose[i].Replace("\"", "\"\""));
                    writer.Write("\"");
                }
                writer.Write("\n");

                foreach (DataRow row in data.Rows)
                {
                    for (int i = 0; i < fieldsToExpose.Length; i++)
                    {
                        if (i != 0) { writer.Write(","); }
                        writer.Write("\"");
                        writer.Write(row[fieldsToExpose[i]].ToString()
                            .Replace("\"", "\"\""));
                        writer.Write("\"");
                    }
                    writer.Write("\n");
                }
            }
            return stream;
        }
    }
    #endregion

    #region BaseModel
    public class BaseModel
    {
        public string ControllerName { get; set; }
        public string GetPartialViewPath(string viewName)
        {
            return string.Format("~/Views/{0}/{1}.cshtml", this.ControllerName.Replace("Controller", ""), viewName);
        }
        public string GetViewPath(string controllerName, string viewName)
        {
            return string.Format("~/Views/{0}/{1}.cshtml", controllerName, viewName);
        }
    }

    public class Result
    {
        public Result()
        {
            this.MessageType = MessageType.Success;
        }
        public string title { get; set; }
        public string Message { get; set; }
        public MessageType MessageType { get; set; }
        public ActionResult ActionResult { get; set; }
        public PartialViewResult PartialViewResult { get; set; }
        public string Redirect { get; set; }
        public object Id { get; set; }
        public object Info { get; set; }
        public object Object { get; set; }
        public int TabNo { get; set; }
        public MvcHtmlString htmlString { get; set; }
    }
    public enum MessageType
    {
        Success = 0,
        Error = 1,
        Info = 2,
        Warning = 3,
        InvalidPassword = 4
    }
    public enum DesignationType
    {
        SuperAdmin = 1,
        Admin = 2,
    }
    #endregion
}