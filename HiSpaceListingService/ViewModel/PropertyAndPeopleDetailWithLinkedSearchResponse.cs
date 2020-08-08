using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiSpaceListingService.ViewModel
{
	public class PropertyAndPeopleDetailWithLinkedSearchResponse
	{
		public int ListingId { set; get; }
		public string Name { set; get; }
		public string ListingType { set; get; }
		public string RE_FirstName { set; get; }
		public string RE_LastName { set; get; }
	}
}
