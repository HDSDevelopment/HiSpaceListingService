using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiSpaceListingService.ViewModel
{
	public class PeopleFilterPeopleList
	{
		public int ListingId { get; set; }
		public string RE_FirstName { get; set; }
		public string RE_LastName { get; set; }
		public string RE_FullName { get; set; }
		public double ProjectCount { set; get; }
	}
}
