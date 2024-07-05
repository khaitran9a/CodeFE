using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.ServiceExtents.ZaloOA.Dtos
{

	public class CommonModel
	{
	}

	public class RequestElementModel
	{
		public string AppId { get; set; }
		public string UserId { get; set; }
		public string Title { get; set; }
		public string ImageBannerUrl { get; set; }
		public string Header { get; set; }
		public List<TableElement> TableElements { get; set; }
		public List<ButtonElement> ButtonElements { get; set; }
	}
	public class TableElement
	{
		public string Name { get; set; }
		public string Value { get; set; }
		public string ColorType { get; set; }
	}
	public class ButtonElement
	{
		public string ButtonName { get; set; }
		public string ImageIcon { get; set; }
		public string Url { get; set; }
		public string Content { get; set; }
		public string PhoneNumber { get; set; }
		public string Payload { get; set; }
		public ButtonType ButtonType { get; set; }
	}

	public class SelectListItems
	{
		public string Title { get; set; }
		public string Value { get; set; }
		public int Index { get; set; }
	}
}
