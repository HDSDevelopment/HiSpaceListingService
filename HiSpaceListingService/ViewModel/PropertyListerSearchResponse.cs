using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiSpaceListingService.ViewModel
{
	public class PropertyListerSearchResponse
	{
		public int UserId { set; get; }
		public string CompanyName { set; get; }
		public double PropertyListerInUseCount { set; get; }
	}
}
