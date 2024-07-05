using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.ServiceExtents.ZaloOA.Dtos
{
    public class ZNSDto
    {
       
        public string AppId { get; set; }
        public string phone { get; set; }
        public string template_id { get; set; }
        public string tracking_id { get; set; }
        public string template_data_string { get; set; }
    }

    public class TemplateData
    {
        public string order_code { get; set; }
        public string cost { get; set; }
        public string customer_name { get; set; }
    }
    public class ZNSResponseModel
    {
        public string error { get; set; }
        public string message { get; set; }
        public string template_id { get; set; }
    }
}
