using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSpaceListingModels;

namespace HiSpaceListingService.ViewModel
{
    public class PropertyPeopleResponse
    {
        public PropertyPeopleResponse()
        {
            Operator = new User();
            Projects = new List<REProfessionalMaster>();
            Listing = new Listing();
            LinkedOpr = new List<LinkedOperators>();
        }

        public User Operator { get; set; }
        //public int ListingId { get; set; }
        public Listing Listing { get; set; }
        public List<LinkedOperators> LinkedOpr { get; set; }
        public List<REProfessionalMaster> Projects { get; set; }
        public List<string> Roles { get; set; }
        public int TotalProjects { get; set; }
    }
}
