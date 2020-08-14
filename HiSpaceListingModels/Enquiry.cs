using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
	[Table("Enquiry")]
	public class Enquiry
	{
		[Key]
		public int EnquiryId { set; get; }
		public int ListingId { set; get; }
		public int Listing_UserId { set; get; }
		public int Sender_UserId { set; get; }
		public string Sender_Name { set; get; }
		public string Sender_Email { set; get; }
		public string Sender_Message { set; get; }
		public string Sender_Phone { set; get; }
		public string To_Email { set; get; }
		public string Status { set; get; }
		public DateTime? CreatedDateTime { set; get; }
	}
}
