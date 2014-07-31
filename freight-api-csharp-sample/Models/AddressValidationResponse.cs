using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace freight_api_csharp_sample.Models
{
    public class AddressValidationResponse
    {
        public AddressToValidate Address { get; set; }
        public bool Validated { get; set; }
        public List<string> Errors { get; set; }
    }

    public class AddressToValidate
    {
        public string Consignee { get; set; }

        public AddressObject Address { get; set; }

        public string Email { get; set; }

        public string ContactPerson { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public bool IsRural { get; set; }
        
        public string DeliveryInstructions { get; set; }
        
        public class AddressObject
        {
            public string BuildingName { get; set; }
            public string StreetAddress { get; set; }
            public string Suburb { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }
            public string CountryCode { get; set; }
        }
    }
}
