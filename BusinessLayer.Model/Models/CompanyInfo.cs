using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace BusinessLayer.Model.Models
{
    public class CompanyInfo : BaseInfo
    {
        [Required]
        [MaxLength(100)]
        public string CompanyName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostalZipCode { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string EquipmentCompanyCode { get; set; }
        public string Country { get; set; }
        public  List<ArSubledgerInfo> ArSubledgers { get; set; }
        public DateTime LastModified { get; set; }
    }
}
