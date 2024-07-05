using K4os.Compression.LZ4.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.ServiceExtents.ZaloOA.Dtos
{
	public class SendBroadCastDto
    {
        public RecipientModel recipient { get; set; }
        public MessageModel message { get; set; }
        public int ZaloAccountId { get; set; }
        public string ArticleId { get; set; }
        public string AppId { get; set; }
    }

    public class RecipientModel
    {
        public RecipientModel()
        {
            AvailableAges = new List<SelectListItem>();
            AvailableGenders = new List<SelectListItem>();
            AvailableLocations = new List<SelectListItem>();
            AvailableCities = new List<SelectListItem>();
            AvailablePlatform = new List<SelectListItem>();
        }
        public Target target { get; set; }
        public IList<SelectListItem> AvailableAges { get; set; }
        public IList<SelectListItem> AvailableGenders { get; set; }
        public IList<SelectListItem> AvailableLocations { get; set; }
        public IList<SelectListItem> AvailableCities { get; set; }
        public IList<SelectListItem> AvailablePlatform { get; set; }
    }
    public class Target
    {
        public string ages { get; set; }
        public string gender { get; set; }
        public string locations { get; set; }
        public string cities { get; set; }
        public string platform { get; set; }

    }

    public class MessageModel
    {
        public AttachmentModel attachment { get; set; }
    }

    public class AttachmentModel
    {
        public string type { get; set; }
        public AttachmentPayloadModel payload { get; set; }
    }

    public class AttachmentPayloadModel
    {
        public AttachmentPayloadModel()
        {
            elements = new List<PayloadElementModel>();
        }
        public string template_type { get; set; }
        public IList<PayloadElementModel> elements { get; set; }
    }

    public class PayloadElementModel
    {
        public string media_type { get; set; }
        public string attachment_id { get; set; }
    }

    public class ListCitiesModel
    {
        public ListCitiesModel()
        {
            ItemsList = new List<SelectListItem>();
        }
        public IList<SelectListItem> ItemsList { get; set; }
    }
}
