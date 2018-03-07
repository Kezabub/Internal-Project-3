using IP3Latest.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace IP3Latest.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private ApplicationDbContext dbContext;
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public AdminController()
        {
            this.dbContext = new ApplicationDbContext();
        }

        [HttpGet]
        public ActionResult Index()
        {
            // Get all users
            var currentUsers = this.dbContext.Users.ToList();

            var model = new AdminIndexModels
            {
                Users = currentUsers
            };

            return this.View(model);
        }
        [HttpGet]
        public async Task<ActionResult> EditName(string id)
        {
            // Get relevant user
            var user = await this.UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return this.View("Error");
            }

            var model = new AdminNameModels
            {
                Id = user.Id,
                FullName = user.FullName
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditName(AdminNameModels model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            var user = await this.UserManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return this.View("Error");
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            var result = await this.UserManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                // Do something with error here
                return this.View(model);
            }

            // Add confirmation message here
            return this.RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> EditPassword(string id)
        {
            // Get relevant user
            var user = await this.UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return this.View("Error");
            }

            var model = new AdminPasswordModels
            {
                Id = user.Id,
                FullName = user.FullName
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditPassword(AdminPasswordModels model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            var user = await this.UserManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return this.View("Error");
            }

            user.PasswordHash = this.UserManager.PasswordHasher.HashPassword(model.NewPassword);

            var result = await this.UserManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                // Do something with error here
                return this.View(model);
            }

            // Add confirmation message here
            return this.RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> EditArchived(string id)
        {
            // Get relevant user
            var user = await this.UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return this.View("Error");
            }

            var model = new AdminArchivedModels
            {
                Id = user.Id,
                FullName = user.FullName
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditArchived(AdminArchivedModels model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            var user = await this.UserManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return this.View("Error");
            }

            if (user.Archived == true)
            {
                user.Archived = false;
            }
            else
            {
                user.Archived = true;
            }
            var result = await this.UserManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                // Do something with error here
                return this.View(model);
            }

            // Add confirmation message here
            return this.RedirectToAction("Index");
        }
    }

   
}
