using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using System.Collections;
using System.Collections.Generic;

namespace Nop.Web.Areas.CRM.Models.Zalo
{
    public partial class ZaloAccountModel : BaseNopEntityModel
    {
        public string Name { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string AppId { get; set; }

        public bool Active { get; set; }
        public string SecretKey { get; set; }
		public string WebhookUrl { get; set; }

        public bool IsSuaWebhookUrl { get; set; }
        public string OaId { get; set; }
        public string CodeVerifier { get; set; }
        public string CodeChallenge { get; set; }

    }
    public partial class ZaloAccountSearchModel : BaseSearchModel
    {
        public ZaloAccountSearchModel()
        {
            AvailableActives = new List<SelectListItem>();
        }

        public string SearchKeyWord { get; set; }
        public IList<SelectListItem> AvailableActives { get; set; }
    }
    public partial class ZaloAccountListModel : BasePagedListModel<ZaloAccountModel>
    {

    }
}
