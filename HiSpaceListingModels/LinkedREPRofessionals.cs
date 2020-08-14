using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
    public class LinkedREPRofessionals
    {
        public int Property_ListingId { set; get; }
        public int ReProfessional_ListingId { set; get; }
        public int REProfessionalMasterId { set; get; }
        public int UserId { set; get; }
        public string ProjectRole { set; get; }
        public string ProjectName { set; get; }
        public string REFirstName { set; get; }
        public string RELastName { set; get; }
        public string ImageUrl { set; get; }
        public string OperatorName { set; get; }
        public string LinkingStatus { set; get; }
        public string RE_UserName { set; get; }
        public string RE_Address { set; get; }
    }
}
