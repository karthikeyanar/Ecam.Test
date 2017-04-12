using Ecam.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecam.Framework;
using MySql.Data.MySqlClient;
using System;

namespace Ecam.Views.Models {
    // You can add profile data for the user by adding more properties to your ApplicationUser
    // class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser:IdentityUser {

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this,DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }


    }

    public class ApplicationDbContext:IdentityDbContext<ApplicationUser> {
        static ApplicationDbContext() {
            Database.SetInitializer(new MySqlInitializer());
        }

        public ApplicationDbContext()
            : base("DefaultConnection") {
        }

        public static ApplicationDbContext Create() {
            return new ApplicationDbContext();
        }
    }

    public class IdentityManager {
        public bool RoleExists(string name) {
            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            return rm.RoleExists(name);
        }

        public IdentityRole GetRoleById(string roleId) {
            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            return rm.FindById(roleId);
        }

        public IdentityRole GetRoleByName(string roleName) {
            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            return rm.FindByName(roleName);
        }

        public bool CreateRole(string name) {
            if(this.RoleExists(name))
                return true;
            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var idResult = rm.Create(new IdentityRole(name));
            return idResult.Succeeded;
        }

        public ApplicationUser GetUser(string userName) {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            return um.FindByName(userName);
        }

        public ApplicationUser GetUserById(string id) {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            return um.FindById(id);
        }

        public ApplicationUser GetUserByEmail(string email) {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            return um.FindByEmail(email);
        }

        public bool CreateUser(string userName,string password,string email,out string errorResult) {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));                        
            var user = new ApplicationUser { UserName = userName,Email = email };
            //UserManager.UserValidator = new UserValidator<TUser>(UserManager) {
            //    AllowOnlyAlphanumericUserNames = false
            //};
            um.UserValidator = new UserValidator<ApplicationUser>(um) {
                AllowOnlyAlphanumericUserNames = false
            };
            var idResult = um.Create(user,password);
            errorResult = GetErrorResult(idResult);
            return idResult.Succeeded;
        }
         

        public bool DeleteUser(string userName,out string errorResult) {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var user = this.GetUser(userName);
            if(user != null) {
                var idResult = um.Delete(user);
                errorResult = GetErrorResult(idResult);
                return idResult.Succeeded;
            } else {
                errorResult = "User does not exist";
                return false;
            }
        }

        public bool ChangePassword(string userName,string oldPassword,string newPassword,out string errorResult) {
            var user = this.GetUser(userName);
            if(user != null) {
                var um = new UserManager<ApplicationUser>(
                        new UserStore<ApplicationUser>(new ApplicationDbContext()));
                um.UserValidator = new UserValidator<ApplicationUser>(um) {
                    AllowOnlyAlphanumericUserNames = false
                };
                var idResult = um.ChangePassword(user.Id,oldPassword,newPassword);
                errorResult = string.Empty;
                if(idResult.Succeeded == false) {
                    errorResult = GetErrorResult(idResult);
                }
                return idResult.Succeeded;
            } else {
                errorResult = "User does not exist";
                return false;
            }
        }

        public bool ChangePassword(string userName,string password,out string errorResult) {
            var user = this.GetUser(userName);
            if(user != null) {
                var um = new UserManager<ApplicationUser>(
                        new UserStore<ApplicationUser>(new ApplicationDbContext()));
                um.UserValidator = new UserValidator<ApplicationUser>(um) {
                    AllowOnlyAlphanumericUserNames = false
                };
                var idResult = um.RemovePassword(user.Id);
                if(idResult.Succeeded) {
                    idResult = um.AddPassword(user.Id,password);
                    errorResult = GetErrorResult(idResult);
                    return idResult.Succeeded;
                } else {
                    errorResult = GetErrorResult(idResult);
                    return idResult.Succeeded;
                }
            } else {
                errorResult = "User does not exist";
                return false;
            }
        }

        private string GetErrorResult(IdentityResult result) {
            if(result == null) {
                return string.Empty;
            }

            string errorResult = string.Empty;
            if(!result.Succeeded) {
                if(result.Errors != null) {
                    foreach(string error in result.Errors) {
                        //ModelState.AddModelError("",error);
                        errorResult += error;
                    }
                }
            }

            return errorResult;
        }

        public bool AddUserToRole(string userId,string roleName) {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            um.UserValidator = new UserValidator<ApplicationUser>(um) {
                AllowOnlyAlphanumericUserNames = false
            };
            var idResult = um.AddToRole(userId,roleName);
            return idResult.Succeeded;
        }

        public bool RemoveUserToRole(string userId,string roleName) {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
            um.UserValidator = new UserValidator<ApplicationUser>(um) {
                AllowOnlyAlphanumericUserNames = false
            };
            var idResult = um.RemoveFromRole(userId,roleName);
            return idResult.Succeeded;
        }

        public List<IdentityRole> GetRoles(string userName) {
            var um = this.GetUser(userName);
            var currentRoles = new List<IdentityUserRole>();
            List<IdentityRole> aspRoles = new List<IdentityRole>();
            if(um != null) {
                currentRoles.AddRange(um.Roles);
                foreach(var role in currentRoles) {
                    var findRole = this.GetRoleById(role.RoleId);
                    if(findRole != null)
                        aspRoles.Add(findRole);
                }
            }
            return aspRoles;
        }

        public bool CheckAnyOtherRoleByUser(string userName,string roleName,out string error) {
            List<IdentityRole> roles = this.GetRoles(userName);
            string exitRole = string.Empty;
            bool isResult = false;
            error = string.Empty;
            foreach(var role in roles) {
                if(role.Name != roleName) {
                    exitRole = role.Name;
                }
            }
            if(exitRole != "") {
                bool isIgnoreValidation = false;
                if(roleName == "CA" || roleName == "CM") {
                    if(exitRole == "AA" || exitRole == "AM") {
                        isIgnoreValidation = true;
                        isResult = true;
                    }
                }
                if(isIgnoreValidation == false) {
                    string existRoleName = string.Empty;
                    switch(exitRole) {
                        case "EA": existRoleName = "Ecam Admin"; break;
                        case "EM": existRoleName = "Ecam Member"; break;
                        case "CA": existRoleName = "Company Admin"; break;
                        case "CM": existRoleName = "Company Member"; break;
                        case "GA": existRoleName = "Group Admin"; break;
                        case "GM": existRoleName = "Group Member"; break;
                        case "AA": existRoleName = "Agent Admin"; break;
                        case "AM": existRoleName = "Agent Member"; break;
                    }
                    if(existRoleName != "") {
                        error = existRoleName + " Role already added this email " + userName;
                    }
                }
            } else {
                isResult = true;
            }
            return isResult;
        }

        private void UpdateUserName(string aspnetUserId,string userName) {
            string sql = string.Format("update aspnetusers set UserName='{0}' where Id='{1}'",userName,aspnetUserId);
            MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString,sql);
        }

        private void UpdateEmail(string aspnetUserId,string email) {
            string sql = string.Format("update aspnetusers set Email='{0}' where Id='{1}'",email,aspnetUserId);
            MySqlHelper.ExecuteNonQuery(Ecam.Framework.Helper.ConnectionString,sql);
        }

        public void UpdateUserNameAndEmail(int id,string userName,string email) {
            if(id>0 && string.IsNullOrEmpty(userName) == false && string.IsNullOrEmpty(email) == false) {
                string sql = string.Format("select aspuser.Id,aspuser.UserName,aspuser.Email from aspnetusers aspuser join ec_user u on aspuser.Id = u.aspnetuser_id where u.user_id={0}",id);
                string aspnetUserId = string.Empty;
                string aspnetUserName = string.Empty;
                string aspnetEmail = string.Empty;
                using(MySqlDataReader dr = MySqlHelper.ExecuteReader(Ecam.Framework.Helper.ConnectionString,sql)) {
                    while(dr.Read()) {
                        aspnetUserId = Convert.ToString(dr["id"]);
                        aspnetUserName = Convert.ToString(dr["username"]);
                        aspnetEmail = Convert.ToString(dr["email"]);
                    }
                }
                if(string.IsNullOrEmpty(aspnetUserId) == false) {
                    if(aspnetUserName != userName) {
                        this.UpdateUserName(aspnetUserId,userName);
                    }
                    if(aspnetEmail != email) {
                        this.UpdateEmail(aspnetUserId,email);
                    }
                }
            }
        }

            //public void ClearUserRoles(string userId) {
            //	var um = new UserManager<ApplicationUser>(
            //		new UserStore<ApplicationUser>(new ApplicationDbContext()));
            //	var user = um.FindById(userId);
            //	var currentRoles=new List<IdentityUserRole>();
            //	currentRoles.AddRange(user.Roles);
            //	foreach(var role in currentRoles) {
            //		var findRole = this.GetRoleById(role.RoleId);
            //		if(findRole != null)
            //			um.RemoveFromRole(userId,findRole.Name);
            //	}
            //}


        }

    public class EcamUserManager {

        //public ec_user FindUser(string aspnetUserId) {
        //    using(EcamContext context = new EcamContext()) {
        //        return context.ec_user.FirstOrDefault(q => q.aspnetuser_id == aspnetUserId && q.is_active == true);
        //    }
        //}

    }
}