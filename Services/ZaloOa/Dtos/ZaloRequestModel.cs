using K4os.Compression.LZ4.Internal;
using Nop.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.ServiceExtents.ZaloOA.Dtos
{
	public class ZaloRequestModel
    {
        public string AppId { get; set; }
        public string Phone { get; set; }
        public string UserId { get; set; }
        public string MessageContent { get; set; }
        public string ImageUrl { get; set; }
        public string TemplateId { get; set; }
    }
}
