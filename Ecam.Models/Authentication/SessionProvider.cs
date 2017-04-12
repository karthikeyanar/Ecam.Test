using Ecam.Contracts;
using Ecam.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace Ecam.Models {

    ///// <summary>
    ///// Stores the current user and entity in the session
    ///// </summary>
    //public class SessionProvider1 {

    //    public static void ClearAll() {
    //        HttpConFac.GetHttpContext().Session.Abandon();
    //    }

    //    private static void RefreshSession(string role) {
    //        int id = (Authentication.CurrentUserID ?? 0);
    //        if(id > 0) {
    //            Helper.SessionLog("RefreshSession UserId=" + id + ",Role=" + role);
    //            List<AuthPermissions> permissions = Authentication.GetAuthPermissions(id,role);
    //            foreach(var p in permissions) {
    //                SessionProvider1.SetCompanyAirlineCodes(p.role,p.company_airline_codes);
    //            }
    //        }
    //    }

    //    public static string GetCompanyAirlineCodes(string role) {
    //        string ids = "-1";
    //        string key = string.Format("{0}_{1}",role,"company_airline_codes");
    //        if(HttpConFac.GetHttpContext().Session != null) {
    //            if(HttpConFac.GetHttpContext().Session[key] != null) {
    //                ids = Convert.ToString(HttpConFac.GetHttpContext().Session[key]);
    //            } else {
    //                Helper.SessionLog("GetCompanyAirlineCodes session expired");
    //                //Session is exipred. Go to db.
    //                RefreshSession(role);
    //                if(HttpConFac.GetHttpContext().Session[key] != null) {
    //                    ids = Convert.ToString(HttpConFac.GetHttpContext().Session[key]);
    //                }
    //            }
    //        }
    //        return ids;
    //    }

    //    public static void SetCompanyAirlineCodes(string role,string companyAirlineCodes) {
    //        string key = string.Format("{0}_{1}",role,"company_airline_codes");
    //        if(HttpConFac.GetHttpContext().Session != null) {
    //            HttpConFac.GetHttpContext().Session[key] = companyAirlineCodes;
    //        }
    //        Helper.SessionLog("SetCompanyAirlineCodes session Role=" + role);
    //    }
    //}

    //public static class HttpConFac  {
    //    [ThreadStatic]
    //    private static HttpContextBase mockHttpContext;

    //    public static void SetHttpContext(HttpContextBase httpContextBase) {
    //        mockHttpContext = httpContextBase;
    //    }

    //    public static void ResetHttpContext() {
    //        mockHttpContext = null;
    //    }

    //    public static HttpContextBase GetHttpContext() {
    //        if(mockHttpContext != null) {
    //            return mockHttpContext;
    //        }

    //        if(HttpContext.Current != null) {
    //            return new HttpContextWrapper(HttpContext.Current);
    //        }

    //        return null;
    //    }
    //}
}
