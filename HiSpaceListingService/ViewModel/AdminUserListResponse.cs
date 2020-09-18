using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HiSpaceListingModels;

namespace HiSpaceListingService.ViewModel
{
	public class AdminUserListResponse
	{
		public AdminUserListResponse()
		{
			User = new User();
		}

		public User User { get; set; }
		public int TotalProperties { get; set; }
	}
}
