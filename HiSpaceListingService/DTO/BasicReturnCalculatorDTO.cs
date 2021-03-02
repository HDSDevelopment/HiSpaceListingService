using System.ComponentModel.DataAnnotations;

namespace HiSpaceListingService.DTO
{
    public class BasicReturnCalculatorDTO
    {
        public decimal Investment { get; set; }

        public decimal CurrentNOI { get; set; }

        public decimal EstimatedCapExOrTIThroughHold { get; set; }

        [Range(0, 100, ErrorMessage = "Should be between 0 to 100")]
        public decimal Inflation { get; set; }

        [Range(0, 100, ErrorMessage = "Should be between 0 to 100")]
        public decimal LoanToValue { get; set; }

        [Range(0, 100, ErrorMessage = "Should be between 0 to 100")]
        public decimal InterestRate { get; set; }

        [Range(0, 100, ErrorMessage = "Should be between 0 to 100")]
        public decimal HoldingPeriodInYears { get; set; }

        public string ExitMethod { get; set; }

        [Range(0, 100, ErrorMessage = "Should be between 0 to 100")]
        public decimal ExitCapRate { get; set; }

        [Range(0, 100, ErrorMessage = "Should be between 0 to 100")]
        public decimal DiscountRate { get; set; }

        [Range(0, 100, ErrorMessage = "Should be between 0 to 100")]
        public decimal TaxRate { get; set; }

        [Range(11, 120, ErrorMessage = "Should be between 11 to 120")]
        public int AdditionalYears { get; set; }
    }
}