using Ecam.Contracts;
using Ecam.Framework;
using Ecam.Framework.Repository;
using Ecam.Models;
using Ecam.Views.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Ecam.Views
{

    public static class Common
    {
        public static IEnumerable<string> GetAllLeisureAWBFiles(string awbnos)
        {
            IEnumerable<string> files = null;
            try
            {
                using (var client = new HttpClient())
                {
                    string url = string.Empty;
                    //if(Helper.IsLocal == "true") {
                    //	url = "http://localhost:1010";
                    //awbnos = "09865432111";
                    //} else {
                    url = "http://leisure.ecamshub.com/";
                    //}
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    //Call HttpClient.GetAsync to send a GET request to the appropriate URI   
                    HttpResponseMessage resp = client.GetAsync(string.Format("/AWBView/Files?awbno={0}", awbnos)).Result;
                    //This method throws an exception if the HTTP response status is an error code.  
                    resp.EnsureSuccessStatusCode();
                    files = resp.Content.ReadAsAsync<IEnumerable<string>>().Result;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += ex.InnerException.Message;
                }
                Helper.Log("GetAllAWBFiles ex=" + errorMessage);
            }
            return files;
        }

        private static List<string> GetRoles(List<IdentityUserRole> identityRoles)
        {
            List<string> roles = new List<string>();
            if (identityRoles != null)
            {
                string roleIds = string.Empty;
                foreach (var role in identityRoles)
                {
                    roleIds += string.Format("'{0}',", role.RoleId.ToString());
                }
                if (string.IsNullOrEmpty(roleIds) == false)
                {
                    roleIds = roleIds.Substring(0, roleIds.Length - 1);
                }
                if (string.IsNullOrEmpty(roleIds) == false)
                {
                    string sql = string.Format("select * from aspnetroles where Id in({0})", roleIds);
                    using (MySqlDataReader dr = MySqlHelper.ExecuteReader(Ecam.Framework.Helper.ConnectionString, sql))
                    {
                        while (dr.Read())
                        {
                            roles.Add(dr["Name"].ToString());
                        }
                        dr.Close();
                    }
                }
            }
            return roles;
        }

        #region IdentityUser
        public static async Task<IdentityUser> GetUser_ByIdentityUser(UserManager<IdentityUser> UserManager, string userName, string password)
        {
            IdentityUser user = null;
            user = await UserManager.FindAsync(userName, password);
            if (user == null)
            {
                user = await UserManager.FindByEmailAsync(userName);
                if (user != null)
                {
                    user = await UserManager.FindAsync(user.UserName, password);
                }
            }
            return user;
        }
          
        #endregion

        #region ApplicationUser
        public static async Task<ApplicationUser> GetUser(UserManager<ApplicationUser> UserManager, string userName, string password)
        {
            ApplicationUser user = null;
            user = await UserManager.FindAsync(userName, password);
            if (user == null)
            {
                user = await UserManager.FindByEmailAsync(userName);
                if (user != null)
                {
                    user = await UserManager.FindAsync(user.UserName, password);
                }
            }
            return user;
        }
         
         
        #endregion

     
    }


    public enum CurrencyEnum
    {
        USD = 31
    }

    // public class TokenAuth {

    //  private const string _KEY = "BearerToken";

    //public static string GetToken() {
    //    string token = string.Empty;
    //    if(HttpContext.Current.Request.Cookies.AllKeys.Contains(_KEY)) {
    //        token = Convert.ToString(HttpContext.Current.Request.Cookies[_KEY].Value);
    //    } else {
    //        token = string.Empty;
    //    }
    //    return token;
    //}

    //public static void SetToken(string token) {
    //    TokenAuth.SignOut();
    //    HttpCookie cookie = new HttpCookie(_KEY,token);
    //    int authTokenExpire = DataTypeHelper.ToInt32(Helper.AuthTokenExpire);
    //    DateTime cashExpireTime = DateTime.Now.AddMinutes(authTokenExpire).AddSeconds(20);
    //    cookie.Expires = cashExpireTime;
    //    HttpContext.Current.Response.Cookies.Add(cookie);
    //    //HttpContext.Current.Session[_KEY] = token;
    //}

    //public static void SignOut() {
    //    if(HttpContext.Current.Request.Cookies.AllKeys.Contains(_KEY)) {
    //        //HttpCookie cookie = HttpContext.Current.Response.Cookies[_KEY];
    //        HttpContext.Current.Request.Cookies.Remove(_KEY);
    //        //cookie.Expires = DateTime.Now.AddDays(-1);
    //        //HttpContext.Current.Response.Cookies.Add(cookie);
    //    }
    //    if(HttpContext.Current.Response.Cookies.AllKeys.Contains(_KEY)) {
    //        //HttpCookie cookie = HttpContext.Current.Response.Cookies[_KEY];
    //        HttpContext.Current.Response.Cookies.Remove(_KEY);
    //        //cookie.Expires = DateTime.Now.AddDays(-1);
    //        //HttpContext.Current.Response.Cookies.Add(cookie);
    //    }
    //}
    //}
}