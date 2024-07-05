using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.HoSoNhanSu;
using Nop.Core.Domain.MangLuoi;
using Nop.Core.Domain.Zalo;
using Nop.ServiceExtents.ZaloOA.Dtos;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.CRM.Models.Zalo;
using System.Collections.Generic;

namespace Nop.Web.Areas.CRM.Factories.ZaloOA
{
    public interface IZaloOAModelFactory
    {
        ZaloAccountSearchModel PrepareZaloAccountSearchModel(ZaloAccountSearchModel searchModel);
        ZaloAccountListModel PrepareZaloAccountListModel(ZaloAccountSearchModel searchModel);
        ZaloAccountModel PrepareZaloAccountModel(ZaloAccount entity, ZaloAccountModel model);
        bool CheckAppId(string appId, int id);
        bool CheckName(string name, int id);

        void PrepareZaloAccount(ZaloAccountModel model, ZaloAccount item);
        ZNSDto PrepareZnsDetail(OrderModel orderModel, string appId, string templateId = "304505", TeleSale teleSale = null, Customer customer = null, NhanVien nhanVien = null);
        void SenZNS(ZNSDto item, string appId);

        CauHinhZnsModel PrepareCauHinhZnsModel(string templateId, CauHinhZns item = null, List<CauHinhZns> listCauHinh = null, CauHinhZnsModel model = null);
		IList<TemplateModel>  PrepareListZnsTemplateModel(IList<TemplateModel> templates);

	}
}
