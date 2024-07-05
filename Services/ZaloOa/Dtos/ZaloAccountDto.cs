using K4os.Compression.LZ4.Internal;
using Nop.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.ServiceExtents.ZaloOA.Dtos
{
    public class ZaloAccountDto /*: BaseEntity*/
    {
        public string Name { get; set; }
        public string SecretKey { get; set; }
        public string AppId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
		public string WebhookUrl { get; set; }
        public string OaId { get; set; }
        public string CodeVerifier { get; set; }
        public string CodeChallenge { get; set; }

    }
}
