using K4os.Compression.LZ4.Internal;
using Nop.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.ServiceExtents.ZaloOA.Dtos
{
	public class ResponseModel
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public Object ObjectInfo { get; set; }
    }
}
