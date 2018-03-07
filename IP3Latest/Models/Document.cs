using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq;
using System.Web;

namespace IP3Latest.Models
{
    //document POCO class
    public class Document
    {
       [Key]
       [ScaffoldColumn(false)]
       [DisplayName("ID")]
       public int DocumentID { get; set; }

       [Required]
       [DisplayName("Document Title")]
       public string DocTitle { get; set; }

       [Required(ErrorMessage = "Must be Unique")]
       [DisplayName("Revision Number")]
       public int RevisionNumber { get; set; }

       [ScaffoldColumn(false)]
       [DisplayName("Document Author")]
       public string DocumentAuthor { get; set; } 
       
       [ScaffoldColumn(false)]
       [Column(TypeName = "datetime2")]
       [DisplayName("Creation Date")]
       public DateTime CreationDate { get; set; }

       [ScaffoldColumn(false)]
       [Column(TypeName = "datetime2")]
       [DisplayName("Activation Date")]
       public DateTime ActivationDate { get; set; }

       [ScaffoldColumn(false)]
       [DisplayName("File Path")]
       public string FilePath { get; set; }

       [ScaffoldColumn(false)]
       [DisplayName("Document Status")]
       public string DocumentStatus { get; set; }

       [ScaffoldColumn(false)]
       public string Distributee { get; set; }
    }
}