using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
	[Table("WorkingHours")]
	public class WorkingHours
	{
		[Key]
		public int WorkingHoursId { set; get; }
		public int ListingId { set; get; }
		public bool? Is24 { set; get; }
		public bool? SunAvail { set; get; }
		public string SunOpen { set; get; }
		public string SunClose { set; get; }
		public bool? MonAvail { set; get; }
		public string MonOpen { set; get; }
		public string MonClose { set; get; }
		public bool? TueAvail { set; get; }
		public string TueOpen { set; get; }
		public string TueClose { set; get; }
		public bool? WedAvail { set; get; }
		public string WedOpen { set; get; }
		public string WedClose { set; get; }
		public bool? ThuAvail { set; get; }
		public string ThuOpen { set; get; }
		public string ThuClose { set; get; }
		public bool? FriAvail { set; get; }
		public string FriOpen { set; get; }
		public string FriClose { set; get; }
		public bool? SatAvail { set; get; }
		public string SatOpen { set; get; }
		public string SatClose { set; get; }
		public string Description { set; get; }
		public bool Status { set; get; }
		public DateTime? CreatedDateTime { set; get; }
		public int? ModifyBy { set; get; }
		public DateTime? ModifyDateTime { set; get; }
	}
}
