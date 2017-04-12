using Ecam.Contracts;
using Ecam.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Ecam.Models
{

    public static class Authentication
    {

        public const string IDKey = "id";
        public const string UserNameKey = "username";
        public const string RoleKey = "role";
        public const string GroupIDKey = "groupids";
        public const string UserRolesKey = "user_roles";

        public static int? CurrentUserID {
            get {
                var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
                int id = 0;
                if (claimsIdentity != null)
                {
                    var claim = claimsIdentity.FindFirst(Authentication.IDKey);
                    if (claim != null)
                    {
                        int.TryParse(claim.Value, out id);
                    }
                }
                if (id > 0) { return id; } else { return null; }
            }
        }

        public static ClaimsIdentity CurrentIdentity {
            get {
                var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
                return claimsIdentity;
            }
        }

        public static string GetCliamValue(string key)
        {
            string value = string.Empty;
            var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var claim = claimsIdentity.FindFirst(key);
                if (claim != null)
                {
                    value = claim.Value;
                }
            }
            return value;
        }

        public static string CurrentUserName {
            get {
                var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
                string userName = string.Empty;
                if (claimsIdentity != null)
                {
                    var claim = claimsIdentity.FindFirst(Authentication.UserNameKey);
                    if (claim != null)
                    {
                        userName = claim.Value;
                    }
                }
                return userName;
            }
        }

        public static string CurrentRole {
            get {
                var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
                string role = string.Empty;
                if (claimsIdentity != null)
                {
                    var claim = claimsIdentity.FindFirst(Authentication.RoleKey);
                    if (claim != null)
                    {
                        role = claim.Value;
                    }
                }
                return role;
            }
        }

        public static IDictionary<string, string> Cliams {
            get {
                var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
                IDictionary<string, string> data = new Dictionary<string, string>();
                if (claimsIdentity != null)
                {
                    List<Claim> cliams = claimsIdentity.Claims.ToList();
                    foreach (var cliam in cliams)
                    {
                        if (cliam.Type.Contains("schemas.xmlsoap.org") == false
                            && cliam.Type.Contains("SecurityStamp") == false
                            && cliam.Type.Contains("schemas.microsoft.com") == false
                            && cliam.Type.Contains("company_airline_codes") == false)
                        {
                            if (data.ContainsKey(cliam.Type) == false)
                            {
                                data.Add(cliam.Type, cliam.Value);
                            }
                        }
                    }
                }
                return data;
            }
        }

        public static string CurrentGroupIDs {
            get {
                var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
                string groupIds = string.Empty;
                if (claimsIdentity != null)
                {
                    var claim = claimsIdentity.FindFirst(Authentication.GroupIDKey);
                    if (claim != null)
                    {
                        groupIds = claim.Value;
                    }
                }
                if (string.IsNullOrEmpty(groupIds) == true)
                {
                    groupIds = "-1";
                }
                return groupIds;
            }
        }

        public static List<int> CurrentGroupIDList {
            get {
                List<int> idList = Helper.ConvertIntIds(Authentication.CurrentGroupIDs);
                if (idList.Count() <= 0)
                {
                    idList.Add(-1);
                }
                return idList;
            }
        }

        public static string CurrentCompanyIDs()
        {
            string ids = string.Empty;
            string role = Authentication.CurrentRole;
            if (string.IsNullOrEmpty(role) == false)
            {
                var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
                if (claimsIdentity != null)
                {
                    string key = string.Format("{0}_{1}", role, "company_ids");
                    var claim = claimsIdentity.FindFirst(key);
                    if (claim != null)
                    {
                        ids = claim.Value;
                    }
                }
            }
            if (string.IsNullOrEmpty(ids) == true) ids = "-1";
            return ids;
        }

        public static List<int> CurrentCompanyIDList()
        {
            List<int> companyIdList = Helper.ConvertIntIds(Authentication.CurrentCompanyIDs());
            if (companyIdList.Count() <= 0)
                companyIdList.Add(-1);
            return companyIdList;
        }

        public static List<string> CurrentAirlineCodeList(string companyIds = "")
        {
            List<int> companyIDList = Helper.ConvertIntIds(companyIds);
            List<UserCompanyAirline> userCompanyAirlines = Authentication.CurrentCompanyAirlines;
            List<string> airlineCodeList = null;
            if (string.IsNullOrEmpty(companyIds) == false)
                airlineCodeList = (from q in userCompanyAirlines
                                   where companyIDList.Contains(q.company_id) == true
                                   select q.airline_code).ToList();
            else
                airlineCodeList = (from q in userCompanyAirlines
                                   select q.airline_code).ToList();

            airlineCodeList = airlineCodeList.Distinct().ToList();
            if (airlineCodeList.Count() <= 0)
                airlineCodeList.Add("-1");
            return airlineCodeList;
        }

        public static List<string> CurrentAirportCodeList(string companyIds = "", string airlineIds="")
        {
            List<int> companyIDList = Helper.ConvertIntIds(companyIds);            
            List<UserCompanyAirport> userCompanyAirports = Authentication.CurrentCompanyAirports;
            List<string> airportCodeList = null;
            List<string> airlineCodeList = null;
            if (string.IsNullOrEmpty(companyIds) == false && string.IsNullOrEmpty(airlineIds)==false)
                airportCodeList = (from q in userCompanyAirports
                                   where companyIDList.Contains(q.company_id) == true
                                   && airlineIds.Contains(q.airline_code)==true
                                   select q.airport_code).ToList();
            else
                airportCodeList = (from q in userCompanyAirports
                                   select q.airport_code).ToList();

            airportCodeList = airportCodeList.Distinct().ToList();
            if (airportCodeList.Count() <= 0)
                airportCodeList.Add("-1");
            return airportCodeList;
        }

        //public static string CurrentAirlineCodes(string companyIds = "") {
        //	string ids = string.Empty;
        //	List<string> airlineCodes = Authentication.CurrentAirlineCodeList();
        //	ids = Helper.ConvertStringIds(airlineCodes);
        //	if(string.IsNullOrEmpty(ids) == true) ids = "-1";
        //	return ids;
        //}

        public static string CurrentAgentIDs {
            get {
                return Authentication.GetAuthPermisionIDs(Authentication.CurrentRole, "agent_ids");
            }
        }

        public static List<UserAgent> CurrentUserAgents {
            get {
                List<UserAgent> userAgents = new List<UserAgent>();
                string ids = Authentication.CurrentAgentIDs;
                List<int> agentIds = Helper.ConvertIntIds(ids);
                int userId = (Authentication.CurrentUserID ?? 0);
                foreach (int id in agentIds)
                {
                    userAgents.Add(new UserAgent
                    {
                        user_id = userId,
                        agent_id = id
                    });
                }
                return userAgents;
            }
        }

        private static string CurrentCompanyAirlineCodes {
            get {
                return Authentication.GetAuthPermisionIDs(Authentication.CurrentRole, "company_airline_codes");
                //return SessionProvider.GetCompanyAirlineCodes(Authentication.CurrentRole);
            }
        }

        private static string CurrentCompanyAirportCodes {
            get {
                return Authentication.GetAuthPermisionIDs(Authentication.CurrentRole, "company_airport_codes");
                //return SessionProvider.GetCompanyAirlineCodes(Authentication.CurrentRole);
            }
        }

        public static List<UserCompanyAirline> CurrentCompanyAirlines {
            get {
                List<UserCompanyAirline> userCompanyAirlines = new List<UserCompanyAirline>();
                string companyAirlineCodes = Authentication.CurrentCompanyAirlineCodes;
                if (string.IsNullOrEmpty(companyAirlineCodes) == false)
                {
                    string[] caids = companyAirlineCodes.Split((",").ToCharArray());
                    foreach (string strid in caids)
                    {
                        string[] arr = strid.Split(("|").ToCharArray());
                        if (arr.Length >= 2)
                        {
                            int companyId = DataTypeHelper.ToInt32(arr[0]);
                            string airlineCode = DataTypeHelper.ConvertCode(arr[1]);
                            if (companyId > 0 && string.IsNullOrEmpty(airlineCode) == false)
                            {
                                userCompanyAirlines.Add(new UserCompanyAirline
                                {
                                    company_id = companyId,
                                    airline_code = airlineCode
                                });
                            }
                        }
                    }
                }
                return userCompanyAirlines;
            }
        }

        public static List<UserCompanyAirport> CurrentCompanyAirports {
            get {
                List<UserCompanyAirport> userCompanyAirports = new List<UserCompanyAirport>();
                string companyAirportCodes = Authentication.CurrentCompanyAirportCodes;
                if (string.IsNullOrEmpty(companyAirportCodes) == false)
                {
                    string[] caids = companyAirportCodes.Split((",").ToCharArray());
                    foreach (string strid in caids)
                    {
                        string[] arr = strid.Split(("|").ToCharArray());
                        if (arr.Length >= 3)
                        {
                            int companyId = DataTypeHelper.ToInt32(arr[0]);
                            string airlineCode = DataTypeHelper.ConvertCode(arr[1]);
                            string airportCode = DataTypeHelper.ConvertCode(arr[2]);
                            if (companyId > 0 && string.IsNullOrEmpty(airlineCode) == false && string.IsNullOrEmpty(airportCode) == false)
                            {
                                userCompanyAirports.Add(new UserCompanyAirport
                                {
                                    company_id = companyId,
                                    airline_code = airlineCode,
                                    airport_code = airportCode
                                });
                            }
                        }
                    }
                }
                return userCompanyAirports;
            }
        }

        private static string GetAuthPermisionIDs(string role, string type)
        {
            var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
            string ids = string.Empty;
            if (claimsIdentity != null)
            {
                string key = string.Format("{0}_{1}", role, type);
                var claim = claimsIdentity.FindFirst(key);
                if (claim != null)
                {
                    ids = claim.Value;
                }
            }
            if (string.IsNullOrEmpty(ids) == true)
            {
                ids = "-1";
            }
            return ids;
        }

  

    }
}
