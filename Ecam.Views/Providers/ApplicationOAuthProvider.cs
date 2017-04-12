using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Ecam.Framework;
using Ecam.Views.Models;
using Ecam.Models;
using Ecam.Framework.Repository;
using MySql.Data.MySqlClient;
using Ecam.Contracts;
using Ecam.Models.Helpers;
using Newtonsoft.Json;
using Ecam.Contracts.Enums;
using Microsoft.Owin.Security.Infrastructure;
using System.Collections.Concurrent;

namespace Ecam.Views.Providers {
    public class ApplicationOAuthProvider:OAuthAuthorizationServerProvider {
        private readonly string _publicClientId;
        private readonly Func<UserManager<IdentityUser>> _userManagerFactory;

        public ApplicationOAuthProvider(string publicClientId,Func<UserManager<IdentityUser>> userManagerFactory) {
            if(publicClientId == null) {
                throw new ArgumentNullException("publicClientId");
            }

            if(userManagerFactory == null) {
                throw new ArgumentNullException("userManagerFactory");
            }

            _publicClientId = publicClientId;
            _userManagerFactory = userManagerFactory;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            UserManager<IdentityUser> UserManager = _userManagerFactory();
            IdentityUser user = null;
            string returnUrl = Helper.SiteUrl;
            ClaimsIdentity identity = null;
            List<ErrorInfo> errors = new List<ErrorInfo>();
            if (errors.Any() == false)
            {
                user = await Common.GetUser_ByIdentityUser(UserManager, context.UserName, context.Password);
                if (user != null)
                {
                }
                else
                {
                    errors.Add(new ErrorInfo { ErrorMessage = "Invalid Login" });
                }
            }
            //if (errors.Any() == false)
            //{
            //    if (userIdentity != null)
            //    {
            //        if (userIdentity.Errors != null)
            //        {
            //            if (userIdentity.Errors.Count > 0)
            //            {
            //                foreach (var err in userIdentity.Errors)
            //                {
            //                    errors.Add(new ErrorInfo { ErrorMessage = err.ErrorMessage });
            //                }
            //            }
            //        }
            //    }
            //}
            //if (errors.Any() == false)
            //{
            //    if (user != null && userIdentity != null)
            //    {
            //        identity = await Common.GetIdentity_ByIdentityUser(UserManager, user, userIdentity);
            //        if (identity == null)
            //        {
            //            errors.Add(new ErrorInfo { ErrorMessage = "Identity is null" });
            //        }
            //    }
            //}
            if (errors.Any() == true)
            {
                foreach(var err in errors)
                {
                    context.SetError("invalid_grant", err.ErrorMessage);
                }
            }
            else
            {
                AuthenticationProperties properties = CreateProperties(user.UserName);
                AuthenticationTicket ticket = new AuthenticationTicket(identity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(identity);
            }
        }

        /*
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context) {
            using(UserManager<IdentityUser> userManager = _userManagerFactory()) {
                //var httpContext = HttpContextFactory.GetHttpContext();
                //string loginRole = string.Empty;
                //if(httpContext != null) {
                //    loginRole = httpContext.Request["role"];
                //}
                IdentityUser user = await userManager.FindAsync(context.UserName,context.Password);
                EcamUserManager ecamUserManager = new EcamUserManager();
                IdentityManager identityManager = new IdentityManager();
                ec_user ecUser = null;
                // IdentityUserRole currentRole = null;
                //IdentityRole role = null;
                string currentRole = string.Empty;
                string groupIds = string.Empty;
                string userRoles = string.Empty;

                if(user == null) {
                    context.SetError("invalid_grant","The user name or password is incorrect.");
                    return;
                } else {
                    List<IdentityUserRole> identityRoles = user.Roles.ToList();
                    List<string> roleNames = this.GetRoles(identityRoles);

                    if(roleNames.Count() > 0) {
                        if(roleNames.Count() > 1) {
                            currentRole = (from q in roleNames
                                           where q != "AA" && q != "AM"
                                           select q).FirstOrDefault();
                            if(string.IsNullOrEmpty(currentRole) == true) {
                                currentRole = roleNames[0];
                            }
                        } else {
                            currentRole = roleNames[0];
                        }
                    }

                    userRoles = string.Empty;
                    foreach(string rn in roleNames) {
                        userRoles += rn + ",";
                    }
                    if(string.IsNullOrEmpty(userRoles) == false) {
                        userRoles = userRoles.Substring(0,userRoles.Length - 1);
                    }

                    if(string.IsNullOrEmpty(currentRole) == true) {
                        context.SetError("invalid_grant","The user is inactive (no rules assigned). Contact administrator.");
                        return;
                    }

                    //role = identityManager.GetRoleById(currentRole.RoleId);
                    // check ecam user active;
                    ecUser = ecamUserManager.FindUser(user.Id);
                    if(ecUser == null) {
                        context.SetError("invalid_grant","The user is inactive. Contact administrator.");
                        return;
                    }
                    if(ecUser != null) {
                        if((ecUser.is_active ?? false) == false) {
                            context.SetError("invalid_grant","The user is inactive. Contact administrator.");
                            return;
                        }
                    }

                    if((currentRole == "GA" || currentRole == "GM" || currentRole == "CA" || currentRole == "CM")) {
                        if(ecUser.group_id <= 0) {
                            context.SetError("invalid_grant","The group is not assign.Contact administrator.");
                            return;
                        } else {
                            IGroupRepository groupRepository = new GroupRepository();
                            ec_group ecGroup = groupRepository.Get(ecUser.group_id);
                            if((ecGroup.is_active ?? false) == false) {
                                context.SetError("invalid_grant","The group is not active.Contact administrator.");
                                return;
                            }
                            groupIds = ecUser.group_id.ToString();
                        }
                    }

                    if(currentRole == "AA" || currentRole == "AM" || currentRole == "LA" || currentRole == "LM") {
                        string sql = string.Format("select distinct c.group_id " +
                                                     " from ec_agent_airline aa" +
                                                     " join tra_company c on aa.company_id = c.company_id" +
                                                     " join ec_agent ag on ag.agent_id = aa.agent_id" +
                                                     " join ec_user_agent uag on uag.agent_id = ag.agent_id where uag.user_id = {0}",ecUser.id);
                        MySqlDataReader reader = MySqlHelper.ExecuteReader(Ecam.Framework.Helper.ConnectionString,sql);
                        using(reader) {
                            while(reader.Read()) {
                                groupIds += string.Format("{0},",reader["group_id"]);
                            }
                        }
                        if(groupIds != "") {
                            groupIds = groupIds.Substring(0,groupIds.Length - 1);
                        }
                        //if(groupIds == string.Empty) {
                        // context.SetError("invalid_grant","The group is not assign.Contact administrator.");
                        // return;
                        //}
                    }


                    // Add claims
                    ClaimsIdentity oAuthIdentity = await userManager.CreateIdentityAsync(user,context.Options.AuthenticationType);
                    oAuthIdentity.AddClaim(new Claim(Authentication.IDKey,ecUser.id.ToString()));
                    oAuthIdentity.AddClaim(new Claim(Authentication.RoleKey,currentRole));
                    oAuthIdentity.AddClaim(new Claim(Authentication.GroupIDKey,groupIds));
                    oAuthIdentity.AddClaim(new Claim(Authentication.UserRolesKey,userRoles));

                    // Add claims
                    //ClaimsIdentity cookiesIdentity = await userManager.CreateIdentityAsync(user,CookieAuthenticationDefaults.AuthenticationType);
                    //cookiesIdentity.AddClaim(new Claim(Authentication.IDKey,ecUser.id.ToString()));
                    //cookiesIdentity.AddClaim(new Claim(Authentication.RoleKey,currentRole));
                    //cookiesIdentity.AddClaim(new Claim(Authentication.GroupIDKey,groupIds));
                    //cookiesIdentity.AddClaim(new Claim(Authentication.UserRolesKey,userRoles));

                    IUserRepository userRepository = new UserRepository();
                    int photoFileId = 0;
                    string photoFileName = "";
                    EC_USER contractUser = userRepository.Get(new EC_USER { id = ecUser.id },new Paging { }).rows.FirstOrDefault();
                    if(contractUser != null) {
                        photoFileId = (contractUser.photo_file_id ?? 0);
                        photoFileName = contractUser.photo_file_name;
                    }
                    //ICacheManager cacheManager = new MemoryCacheManager();
                    //int authTokenExpire = DataTypeHelper.ToInt32(Helper.AuthTokenExpire);
                    //DateTime cashExpireTime = DateTime.Now.AddMinutes(authTokenExpire).AddSeconds(20);

                    //cacheManager.Remove(string.Format(MemoryCacheManager.USER_AIRLINE,ecUser.id,currentRole));
                    //cacheManager.Remove(string.Format(MemoryCacheManager.USER_AIRPORT,ecUser.id,currentRole));
                    //cacheManager.Remove(string.Format(MemoryCacheManager.USER_PERMISSION,ecUser.id,currentRole));
                    //cacheManager.Remove(string.Format(MemoryCacheManager.USER_AGENT,ecUser.id,currentRole));

                    //List<EC_USER_AIRLINE> userAirlines = null;
                    //List<EC_USER_AGENT> userAgents = null;
                    //FilterExtension.GetAuthFilterList(ref userAirlines,ref userAgents,ecUser.id,currentRole);

                    //ec_user_permission userPermission = userRepository.GetUserPermission(ecUser.id);
                    //cacheManager.Set(string.Format(MemoryCacheManager.USER_PERMISSION,ecUser.id,currentRole),userPermission,cashExpireTime);

                    //AuthenticationProperties properties = CreateProperties(user,currentRole,ecUser,photoFileId,photoFileName,userPermission);
                    //AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity,properties);
                    //context.Validated(ticket);
                }
                //cookie
                //context.Request.Context.Authentication.SignIn(cookiesIdentity);
            }
        }
        */

        public List<string> GetRoles(List<IdentityUserRole> identityRoles) {
            List<string> roles = new List<string>();
            if(identityRoles != null) {
                string roleIds = string.Empty;
                foreach(var role in identityRoles) {
                    roleIds += string.Format("'{0}',",role.RoleId.ToString());
                }
                if(string.IsNullOrEmpty(roleIds) == false) {
                    roleIds = roleIds.Substring(0,roleIds.Length - 1);
                }
                if(string.IsNullOrEmpty(roleIds) == false) {
                    string sql = string.Format("select * from aspnetroles where Id in({0})",roleIds);
                    using(MySqlDataReader dr = MySqlHelper.ExecuteReader(Ecam.Framework.Helper.ConnectionString,sql)) {
                        while(dr.Read()) {
                            roles.Add(dr["Name"].ToString());
                        }
                        dr.Close();
                    }
                }
            }
            return roles;
        }

        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context) {
            return base.GrantRefreshToken(context);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context) {
            foreach(KeyValuePair<string,string> property in context.Properties.Dictionary) {
                context.AdditionalResponseParameters.Add(property.Key,property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context) {
            // Resource owner password credentials does not provide a client ID.
            if(context.ClientId == null) {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context) {
            if(context.ClientId == _publicClientId) {
                Uri expectedRootUri = new Uri(context.Request.Uri,"/");

                if(expectedRootUri.AbsoluteUri == context.RedirectUri) {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName) {
            IDictionary<string,string> data = new Dictionary<string,string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

      
    }

    /*
    public class RefreshTokenProvider:IAuthenticationTokenProvider {

        public async Task CreateAsync(AuthenticationTokenCreateContext context) {
            Create(context);
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context) {
            Receive(context);
        }

        public void Create(AuthenticationTokenCreateContext context) {
            object inputs;
            context.OwinContext.Environment.TryGetValue("Microsoft.Owin.Form#collection",out inputs);

            Microsoft.Owin.FormCollection collection = (Microsoft.Owin.FormCollection)inputs;

            string grantType = collection["grant_type"]; // ((FormCollection)inputs)?.GetValues("grant_type");

            //if(grantType.Equals("refresh_token")) return;

            int authTokenExpire = DataTypeHelper.ToInt32(Helper.AuthTokenExpire);

            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddMinutes(authTokenExpire);

            context.SetToken(context.SerializeTicket());
        }

        public void Receive(AuthenticationTokenReceiveContext context) {
            object inputs;
            context.OwinContext.Environment.TryGetValue("Microsoft.Owin.Form#collection",out inputs);
            Microsoft.Owin.FormCollection collection = (Microsoft.Owin.FormCollection)inputs;

            int authTokenExpire = DataTypeHelper.ToInt32(Helper.AuthTokenExpire);

            context.DeserializeTicket(context.Token);

            if(context.Ticket == null) {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                context.Response.ReasonPhrase = "invalid token";
                return;
            }

            if(context.Ticket.Properties.ExpiresUtc <= DateTime.UtcNow) {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                context.Response.ReasonPhrase = "unauthorized";
                return;
            }

            if(collection["IsLogout"] == "true") {
                context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(-1);
            } else {
                context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddMinutes(authTokenExpire);
            }
            context.SetTicket(context.Ticket);
        }
    }
    */
}