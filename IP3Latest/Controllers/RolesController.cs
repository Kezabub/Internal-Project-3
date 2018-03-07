using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using IP3Latest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace IP3Latest.Controllers
{
    [Authorize (Roles = "Administrator")]
    public class RolesController : Controller
    {
        private ApplicationDbContext context = new ApplicationDbContext();
        /// <summary>
        /// Displays a list of the roles currently present within the database
        /// </summary>
        /// <returns>Roles</returns>
        //
        // GET: /Roles/
        public ActionResult Index()
        {
            var Roles = context.Roles.ToList();
            return View(Roles);
        }

        //
        // GET: /Roles/Create
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Reads in a role name from the view and adds it to the ASPNetRoles table in the database
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        //
        // POST: /Roles/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {


                context.Roles.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityRole()
                {

                    Name = collection["RoleName"],

                });

                string RoleName = collection["RoleName"];

                if (String.IsNullOrWhiteSpace(RoleName))
                {


                    return View();

                }



                else
                {
                    context.SaveChanges();
                    ViewBag.ResultMessage = "Role created successfully !";
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
        }

        /// <summary>
        /// Returns role name to be edited 
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        //
        // GET: /Roles/Edit/5
        public ActionResult Edit(string roleName)
        {
            var thisRole = context.Roles.Where(r => r.Name.Equals(roleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            return View(thisRole);
        }

        /// <summary>
        /// Allows the user to edit the role name of an existing role within the database
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        //
        // POST: /Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Microsoft.AspNet.Identity.EntityFramework.IdentityRole role)
        {
            try
            {
                context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                if (role.Name != null)
                {
                    context.SaveChanges();
                }

                else
                {
                    return View();
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        /// <summary>
        /// Deletes an existing role from the database where the role name is equal to the input parameter RoleName
        /// </summary>
        /// <param name="RoleName"></param>
        /// <returns></returns>
        //
        // GET: /Roles/Delete/5
        public ActionResult Delete(string RoleName)
        {
            var thisRole = context.Roles.Where(r => r.Name.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            context.Roles.Remove(thisRole);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// This method is used to select user to which role information should be added 
        /// </summary>
        /// <returns></returns>
        //


        public ActionResult ManageUserRoles()
        {
            var Userlist = context.Users.OrderBy(r => r.UserName).ToList().Select(rr => new SelectListItem { Value = rr.UserName.ToString(), Text = rr.UserName }).ToList();
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
            ViewBag.Users = Userlist;
            return View();
        }
        /// <summary>
        /// Adds a User selected from the manage user roles method to a role /// </summary>
        /// <param name="UserName"></param>
        /// <param name="RoleName"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string UserName, string RoleName)
        {

            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var Userlist = context.Users.OrderBy(r => r.UserName).ToList().Select(rr => new SelectListItem { Value = rr.UserName.ToString(), Text = rr.UserName }).ToList();
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var idResult = um.AddToRole(user.Id, RoleName);
            ViewBag.ResultMessage = "Role created successfully !";

            // prepopulate roles for the view dropdown
            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            ViewBag.Roles = list;
            ViewBag.Users = Userlist;

            return View("ManageUserRoles");
        }
        /// <summary>
        /// retrive all the roles assigned to a chosen user from the database
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                var account = new AccountController();
                var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                ViewBag.RolesForThisUser = um.GetRoles(user.Id);

                // prepopulat roles for the view dropdown
                var Userlist = context.Users.OrderBy(r => r.UserName).ToList().Select(rr => new SelectListItem { Value = rr.UserName.ToString(), Text = rr.UserName }).ToList();
                var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
                ViewBag.Roles = list;
                ViewBag.Users = Userlist;
            }

            return View("ManageUserRoles");
        }
        /// <summary>
        /// this method allows a user in thhe stores manager role to revoke roles from all users in the database
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="RoleName"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ApplicationUser user = context.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (um.IsInRole(user.Id, RoleName))
            {
                um.RemoveFromRole(user.Id, RoleName);
                ViewBag.ResultMessage = "Role removed from this user successfully !";
            }
            else
            {
                ViewBag.ResultMessage = "This user doesn't belong to selected role.";
            }
            // prepopulate roles for the view dropdown

            var list = context.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            var Userlist = context.Users.OrderBy(r => r.UserName).ToList().Select(rr => new SelectListItem { Value = rr.UserName.ToString(), Text = rr.UserName }).ToList();
            ViewBag.Users = Userlist;
            ViewBag.Roles = list;

            return View("ManageUserRoles");
        }
    }
}