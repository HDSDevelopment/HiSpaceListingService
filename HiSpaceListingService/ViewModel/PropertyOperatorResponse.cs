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
        }

        public User Operator { get; set; }
        public int TotalCommercial { get; set; }
        public int TotalCoWorking { get; set; }
        public int TotalREProfessional { get; set; }
    }
}
