using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model.Models
{
    public class ErrorGeneral : Exception
    {
        public string TrackingNumber { get;}
        public string Title { get; set; }
        public string Description { get; set; }

        public ErrorGeneral()
        {
            TrackingNumber = Guid.NewGuid().ToString();
        }
    }
}
