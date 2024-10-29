using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class EmployeeDto : BaseDto
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Occupation { get; set; }
        public string EmployeeStatus { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public DateTime LastModified { get; set; }

        /// <summary>
        /// This property is derived from the Company class. 
        /// While we don't use Entity Framework (EF), I've added this here for consistency. 
        /// Please note that it is not directly related to the Employee entity.
        /// </summary>
        public string CompanyName { get; set; }
    }
}