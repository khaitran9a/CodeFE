using K4os.Compression.LZ4.Internal;
using Nop.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.ServiceExtents.ZaloOA.Dtos
{

    public class ZoaFollowerDto
    {
        public string oa_id { get; set; }
        public string user_id_by_app { get; set; }
        public string event_name { get; set; }
        public string source { get; set; }
        public string app_id { get; set; }
        public string timestamp { get; set; }
        public ZoaDataId follower { get; set; }
        public ZoaDataId sender { get; set; }
        public ZoaDataId recipient { get; set; }
        public CustomerInfo info { get; set; }
        public Message message { get; set; }
    }

    public class ZoaDataId
    {
        public string id { get; set; }
    }

    public class CustomerInfo
    {
        public string address { get; set; }
        public decimal phone { get; set; }
        public string city { get; set; }
        public string district { get; set; }
        public string name { get; set; }
        public string ward { get; set; }
    }

    public class Message
    {
        public string note { get; set; }
        public int rate { get; set; }
        public string submit_time { get; set; }
        public string msg_id { get; set; }
        public string tracking_id { get; set; }
        public IList<string> feedbacks { get; set; }
    }
}