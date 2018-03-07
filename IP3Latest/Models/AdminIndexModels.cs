using System.Collections.Generic;

namespace IP3Latest.Models
{
    public class AdminIndexModels
    {
        public AdminIndexModels()
        {
            this.Users = new List<ApplicationUser>();
        }

        public List<ApplicationUser> Users { get; set; }
    }
}