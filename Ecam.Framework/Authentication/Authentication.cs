using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Ecam.Framework {
    public static class Authentication {
        public const string IDKey = "id";
        public const string RoleKey = "role";
        public const string GroupIDKey = "groupids";
        public const string UserRolesKey = "user_roles";

        public static int? CurrentUserID {
            get {
                var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
                int id = 0;
                if(claimsIdentity != null) {
                    var claim = claimsIdentity.FindFirst(Authentication.IDKey);
                    if(claim != null) {
                        int.TryParse(claim.Value,out id);
                    }
                }
                if(id > 0) { return id; } else { return null; }
            }
        }

        public static string CurrentRole {
            get {
                var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
                string role = string.Empty;
                if(claimsIdentity != null) {
                    var claim = claimsIdentity.FindFirst(Authentication.RoleKey);
                    if(claim != null) {
                        role = claim.Value;
                    }
                }
                return role;
            }
        }

        public static IDictionary<string,string> Cliams {
            get {
                var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
                IDictionary<string,string> data = new Dictionary<string,string>();
                if(claimsIdentity != null) {
                    List<Claim> cliams = claimsIdentity.Claims.ToList();
                    foreach(var cliam in cliams) {
                        if(cliam.Type.Contains("schemas.xmlsoap.org") == false
                            && cliam.Type.Contains("SecurityStamp") == false
                            && cliam.Type.Contains("schemas.microsoft.com") == false) {
                            if(data.ContainsKey(cliam.Type) == false) {
                                data.Add(cliam.Type,cliam.Value);
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
                if(claimsIdentity != null) {
                    var claim = claimsIdentity.FindFirst(Authentication.GroupIDKey);
                    if(claim != null) {
                        groupIds = claim.Value;
                    }
                }
                if(string.IsNullOrEmpty(groupIds) == true) {
                    groupIds = "-1";
                }
                return groupIds;
            }
        }

        public static List<int> CurrentGroupIDList {
            get {
                List<int> idList = Helper.ConvertIntIds(Authentication.CurrentGroupIDs);
                if(idList.Count() <= 0) {
                    idList.Add(-1);
                }
                return idList;
            }
        }

        public static string CurrentCompanyIDs() {
            string ids = string.Empty;
            List<int> companyIds = Authentication.CurrentCompanyIDList();
            ids = Helper.ConvertIds(companyIds);
            if(string.IsNullOrEmpty(ids) == true) ids = "-1";
            return ids;
        }

        public static List<int> CurrentCompanyIDList() {
            List<UserCompanyAirline> userCompanyAirlines = Authentication.CurrentCompanyAirlines;
            List<int> companyIdList = userCompanyAirlines.Select(q => q.company_id).Distinct().ToList();
            if(companyIdList.Count() <= 0)
                companyIdList.Add(-1);
            return companyIdList;
        }

        public static List<int> CurrentAirlineIDList(string companyIds = "") {
            List<int> companyIDList = Helper.ConvertIntIds(companyIds);
            List<UserCompanyAirline> userCompanyAirlines = Authentication.CurrentCompanyAirlines;
            List<int> airlineIDList = null;
            if(string.IsNullOrEmpty(companyIds) == false)
                airlineIDList = (from q in userCompanyAirlines
                                 where companyIDList.Contains(q.company_id) == true
                                 select q.airline_id).ToList();
            else
                airlineIDList = (from q in userCompanyAirlines
                                 select q.airline_id).ToList();

            airlineIDList = airlineIDList.Distinct().ToList();
            if(airlineIDList.Count() <= 0)
                airlineIDList.Add(-1);
            return airlineIDList;
        }


        public static string CurrentAirlineIDs(string companyIds = "") {
            string ids = string.Empty;
            List<int> airlineIds = Authentication.CurrentAirlineIDList();
            ids = Helper.ConvertIds(airlineIds);
            if(string.IsNullOrEmpty(ids) == true) ids = "-1";
            return ids;
        }

        public static string CurrentAgentIDs {
            get {
                return Authentication.GetAuthPermisionIDs(Authentication.CurrentRole,"agent_ids");
            }
        }

        public static List<UserAgent> CurrentUserAgents {
            get {
                List<UserAgent> userAgents = new List<UserAgent>();
                string ids = Authentication.GetAuthPermisionIDs(Authentication.CurrentRole,"agent_ids");
                List<int> agentIds = Helper.ConvertIntIds(ids);
                int userId = (Authentication.CurrentUserID ?? 0);
                foreach(int id in agentIds) {
                    userAgents.Add(new UserAgent {
                        user_id = userId,
                        agent_id = id
                    });
                }
                return userAgents;
            }
        }

        private static string CurrentCompanyAirlineIDs {
            get {
                return Authentication.GetAuthPermisionIDs(Authentication.CurrentRole,"company_airline_ids");
            }
        }

        public static List<UserCompanyAirline> CurrentCompanyAirlines {
            get {
                List<UserCompanyAirline> userCompanyAirlines = new List<UserCompanyAirline>();
                string companyAirlineIds = Authentication.CurrentCompanyAirlineIDs;
                if(string.IsNullOrEmpty(companyAirlineIds) == false) {
                    string[] caids = companyAirlineIds.Split((",").ToCharArray());
                    foreach(string strid in caids) {
                        string[] arr = strid.Split(("|").ToCharArray());
                        if(arr.Length >= 2) {
                            int companyId = DataTypeHelper.ToInt32(arr[0]);
                            int airlineId = DataTypeHelper.ToInt32(arr[1]);
                            if(companyId > 0 && airlineId > 0) {
                                userCompanyAirlines.Add(new UserCompanyAirline {
                                    company_id = companyId,
                                    airline_id = airlineId
                                });
                            }
                        }
                    } 
                }
                return userCompanyAirlines;
            }
        }

        private static string GetAuthPermisionIDs(string role,string type) {
            var claimsIdentity = HttpContext.Current.User.Identity as ClaimsIdentity;
            string ids = string.Empty;
            if(claimsIdentity != null) {
                string key = string.Format("{0}_{1}",role,type);
                var claim = claimsIdentity.FindFirst(key);
                if(claim != null) {
                    ids = claim.Value;
                }
            }
            if(string.IsNullOrEmpty(ids) == true) {
                ids = "-1";
            }
            return ids;
        }
    }


    public class AuthPermissions {
        public AuthPermissions() {
            // this.company_ids = string.Empty;
            // this.airline_ids = string.Empty;
            this.agent_ids = string.Empty;
            this.company_airline_ids = string.Empty;
            this.role = string.Empty;
        }
        //public string company_ids { get; set; }
        //public string airline_ids { get; set; }
        public string agent_ids { get; set; }
        public string company_airline_ids { get; set; }
        public string role { get; set; }
    }

    public class UserCompanyAirline {
        public int company_id { get; set; }
        public int airline_id { get; set; }
    }

    public class UserAgent {
        public int user_id { get; set; }
        public int agent_id { get; set; }
    }
}
