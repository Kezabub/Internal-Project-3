using System.ComponentModel.DataAnnotations;

namespace IP3Latest.Models
{
    public class AdminNameModels
    {
        public string Id { get; set; }

        public string FullName { get; set; }


        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}