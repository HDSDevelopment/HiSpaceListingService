using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSpaceListingModels;

namespace HiSpaceListingService.ViewModel
{
    public class PeopleDetailResponse
    {
		public PeopleDetailResponse()
		{
			User = new User();
			Listing = new Listing();
			REProfessionalMasters = new List<REProfessionalMaster>();
		}

		public User User { set; get; }
		public Listing Listing { set; get; }
		public List<REProfessionalMaster> REProfessionalMasters { set; get; }
		public int ProjectCount { set; get; }
	}
}
