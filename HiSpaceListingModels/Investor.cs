using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
	[Table("Investor")]
	public class Investor
	{
		[Key]
		public int InvestorId { set; get; }
		public string FirstName { set; get; }
		public string LastName { set; get; }
		public string Email { set; get; }
		public string Phone { set; get; }
		public string InvestmentType { set; get; }
		public string PropertyType { set; get; }
		public string Currency { set; get; }
		public string MinRange { set; get; }
		public string MaxRange { set; get; }
		public string During { set; get; }
		public string Country { set; get; }
		public string State { set; get; }
		public string District { set; get; }
		public string Neighborhood { set; get; }
		public string Comment { set; get; }
		public DateTime? CreatedDateTime { set; get; } 
	}
}
