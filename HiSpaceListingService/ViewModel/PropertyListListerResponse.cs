using HiSpaceListingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiSpaceListingService.ViewModel
{
	public class PropertyListListerResponse
	{
		public PropertyListListerResponse()
		{
			PropertyDetail = new List<PropertyDetailResponse>();
			LinkedREPRofessionals = new List<LinkedREPRofessionals>();
			SpaceUser = new User();
		}
		public List<PropertyDetailResponse> PropertyDetail { set; get; }
		public User SpaceUser { get; set; }
		public int PropertyCount { get; set; }
		public List<LinkedREPRofessionals> LinkedREPRofessionals { set; get; }
		public int LinkedREProfCount { set; get; }
	}
}
