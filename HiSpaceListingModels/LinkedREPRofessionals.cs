using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
    public class LinkedREPRofessionals
    {
        public int ListingId { set; get; }
        public int REProfessionalMasterId { set; get; }
        public int UserId { set; get; }
        public string ProjectRole { set; get; }
        public string REFirstName { set; get; }
        public string RELastName { set; get; }
    }
}
