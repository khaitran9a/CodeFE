using K4os.Compression.LZ4.Internal;
using Nop.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.ServiceExtents.ZaloOA.Dtos
{
	public class FollowerDto
    {
        public int error { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }
    public class Data
    {
        public int total { get; set; }
        public List<UserFollower> followers { get; set; }
    }
    public class UserFollower
    {
        public string user_id { get; set; }
    }
}
