using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
	[Table("Listing")]
	public class Listing
	{
		[Key]
		public int ListingId { set; get; }
		public int UserId { set; get; }
		public string Name { set; get; }
		public string Email { set; get; }
		public string Phone { set; get; }
		public string SecondaryPhone { set; get; }
		public string ListingType { set; get; }
		public string CommercialInfraType { set; get; }
		public string CommercialType { set; get; }
		public string CoworkingType { set; get; }
		public string REprofessionalsType { set; get; }
		public string Fax { set; get; }
		public string street_number { set; get; }
		public string route { set; get; }
		public string locality { set; get; }
		public string administrative_area_level_1 { set; get; }
		public string country { set; get; }
		public string postal_code { set; get; }
		public DateTime? BuildYear { set; get; }
		public DateTime? RecentInnovation { set; get; }
		public bool CM_IntrestedCoworking { set; get; }
		public bool RentalHour { set; get; }
		//public bool NoNullRentalHour
		//{
		//	get { return RentalHour ?? false; }
		//	set { RentalHour = value; }
		//}
		public bool RentalDay { set; get; }
		//public bool NoNullRentalDay
		//{
		//	get { return RentalDay ?? false; }
		//	set { RentalDay = value; }
		//}
		public bool RentalMonth { set; get; }
		//public bool NoNullRentalMonth
		//{
		//	get { return RentalMonth ?? false; }
		//	set { RentalMonth = value; }
		//}
		public decimal? PriceMin { set; get; }
		public decimal? PriceMax { set; get; }
		public int? TotalSeats { set; get; }
		public int? CurrentOccupancy { set; get; }
		public decimal? SpaceSize { set; get; }
		public int? CW_CafeSeats { set; get; }
		public int? CW_MeetingRoom { set; get; }
		public int? CW_MeetingRoomSeats { set; get; }
		public int? CW_Coworking { set; get; }
		public int? CW_CoworkingSeats { set; get; }
		public int? CW_PrivateOffice { set; get; }
		public int? CW_PrivateOfficeSeats { set; get; }
		public bool CW_VirtualOffice { set; get; }
		public string latitude_view { set; get; }
		public string longitude_view { set; get; }
		public string Description { set; get; }
		public string PrimaryConatctName { set; get; }
		public string PrimaryConatctPhone { set; get; }
		public bool Status { set; get; }
		public DateTime? CreatedDateTime { set; get; }
		public int? ModifyBy { set; get; }
		public DateTime? ModifyDateTime { set; get; }
		public bool RE_Warehouse { set; get; }
		public bool RE_Office { set; get; }
		public bool RE_Retail { set; get; }
		public bool RE_Coworking { set; get; }
		public bool RE_PropertyManagement { set; get; }
		public string CMCW_PropertyFor { set; get; }
		public string CMCW_ReraId { set; get; }
		public string CMCW_SurveyNumber { set; get; }
		public string CMCW_PropertyTaxBillNumber { set; get; }
		public string CMCW_CTSNumber { set; get; }
		public string CMCW_MilkatNumber { set; get; }
		public string CMCW_GatNumber { set; get; }
		public string CMCW_PlotNumber { set; get; }
		public string RE_FirstName { set; get; }
		public string RE_LastName { set; get; }
		//public string RE_Roles { set; get; }
	}
}
