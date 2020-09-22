using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSpaceListingModels;

namespace HiSpaceListingService.ViewModel
{
	public class EnquiryListResponse
	{
		public EnquiryListResponse()
		{
			Enquiry = new Enquiry();
		}

		public Enquiry Enquiry { get; set; }
		public string OperatorName { get; set; }
		public string PropertyName { get; set; }
		public string Type { get; set; }
	}
}
