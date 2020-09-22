using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSpaceListingModels;

namespace HiSpaceListingService.ViewModel
{
    public class PropertyOperatorResponse
    {
        public PropertyOperatorResponse()
        {
            Operator = new User();
            LinkedREProf = new List<LinkedREPRofessionals>();
            roles = new List<string>();
        }

        public User Operator { get; set; }
        public int TotalCommercial { get; set; }
        public int TotalCoWorking { get; set; }
        public int TotalREProfessional { get; set; }

        public List<LinkedREPRofessionals> LinkedREProf { get; set; }
        public int LinkedREProfCount { get; set; }
        public List<string> roles { get; set; }
    }
}
