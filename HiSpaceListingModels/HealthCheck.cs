using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
	[Table("HealthCheck")]
	public class HealthCheck
	{
		[Key]
		public int HealthCheckId { set; get; }
		public int ListingId { set; get; }
		public string AQI_Data { set; get; }
		public string AQI_Grade { set; get; }
		public string Temperature_Data { set; get; }
		public string Temperature_Grade { set; get; }
		public string Humidity_Data { set; get; }
		public string Humidity_Grade { set; get; }
		public string CO2_Data { set; get; }
		public string CO2_Grade { set; get; }
		public string CO_Data { set; get; }
		public string CO_Grade { set; get; }
		public string PM2Point5_Data { set; get; }
		public string PM2Point5_Grade { set; get; }
		public string PM10_Data { set; get; }
		public string PM10_Grade { set; get; }
		public string Moisture_Data { set; get; }
		public string Moisture_Grade { set; get; }
		public bool Status { set; get; }
		public DateTime? CreatedDateTime { set; get; }
		public int? ModifyBy { set; get; }
		public DateTime? ModifyDateTime { set; get; }
	}
}
