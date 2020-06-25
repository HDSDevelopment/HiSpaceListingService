using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HiSpaceListingModels
{
	[Table("GreenBuildingCheck")]
	public class GreenBuildingCheck
	{
		[Key]
		public int GreenBuildingCheckId { set; get; }
		public int ListingId { set; get; }
		public int? SF_GreenPolicy { set; get; }
		public int? SF_WasteCD { set; get; }
		public int? SF_Commuting { set; get; }
		public int? SF_Landscaping { set; get; }
		public int? SF_NonRoof { set; get; }
		public int? SF_Roof { set; get; }
		public int? SF_PollutionReduction { set; get; }
		public int? SF_BuildingOM { set; get; }
		public int? WE_Fixtures { set; get; }
		public int? WE_Harvesting { set; get; }
		public int? WE_Treatment { set; get; }
		public int? WE_Reuse { set; get; }
		public int? WE_Metering { set; get; }
		public int? WE_TurfArea { set; get; }
		public int? EE_Refrigerants { set; get; }
		public int? EE_MinimumEP { set; get; }
		public int? EE_ImprovedEP { set; get; }
		public int? EE_OnSiteRE { set; get; }
		public int? EE_OffSiteRE { set; get; }
		public int? EE_EnergyMetering { set; get; }
		public int? HC_SmokeControl { set; get; }
		public int? HC_VEntilation { set; get; }
		public int? HC_CO2Control { set; get; }
		public int? HC_PollutionEquipment { set; get; }
		public int? HC_EcoChemicals { set; get; }
		public int? HC_ThermalComfort { set; get; }
		public int? HC_AbledPeople { set; get; }
		public int? HC_Facilities { set; get; }
		public int? IC_Innovation { set; get; }
		public int? IC_IGBC { set; get; }
		public bool Status { set; get; }
		public DateTime? CreatedDateTime { set; get; }
		public int? ModifyBy { set; get; }
		public DateTime? ModifyDateTime { set; get; }
		public string CertificationStatus { set; get; }
		public string CertifiedBy { set; get; }
		public string IntrestedToApply { set; get; }
		public string CertificationNumber { set; get; }
	}
}
