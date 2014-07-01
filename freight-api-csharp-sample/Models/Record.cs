using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace freight_api_csharp_sample.Models
{
    public class Record
    {
        public string packingslipno { get; set; }
        public string consignee { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string suburb { get; set; }
        public string city { get; set; }
        public string postcode { get; set; }
        public string country { get; set; }
        public string delvref { get; set; }
        public string delvinstructions { get; set; }
        public string contactname { get; set; }
        public string contactphone { get; set; }
        public string email { get; set; }
    }
    public class RecordStatus
    {
        public string packingslipno { get; set; }
        public string consignee { get; set; }
        public string Status { get; set; }
        public string ticketnumber { get; set; }
        public string trackingurl { get; set; }
        public DateTime? Picked { get; set; }
        public DateTime? Delivered { get; set; }
    }

    public class BatchOrdersResponse
    {
        public string packingslipno { get; set; }
        public bool result { get; set; }
    }
}
