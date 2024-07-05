using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.DanhMuc;
using Nop.Core.Domain.Zalo;
using Nop.ServiceExtents.ZaloOA;
using Nop.ServiceExtents.ZaloOA.Dtos;
using Nop.Services.Customers;
using Nop.Services.DanhMuc;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Areas.CRM.Models.Zalo;
using Nop.Web.Areas.HRM.Models.DanhMuc;
using Nop.Web.Framework.Models.Extensions;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using static iTextSharp.text.pdf.AcroFields;
using Nop.Services.Orders;
using System.Reflection;
using Nop.Core.Domain.Orders;
using Microsoft.Rest.Azure.OData;
using System.Text.RegularExpressions;
using Nop.ServiceExtents;
using Nop.Core.Domain.MangLuoi;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.HoSoNhanSu;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.MAR.Factories.MangLuoi;
using Nop.Web.Areas.HRM.Factories.HoSoNhanSu;
using Nop.Web.Areas.Admin.Models.Customers;
using System.Linq;
using Nop.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Services.HoSoNhanSu;
using Nop.Services.Logging;

namespace Nop.Web.Areas.CRM.Factories.ZaloOA
{
    public class ZaloOAModelFactory : IZaloOAModelFactory
    {
        #region Fields    		
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IZaloAccountService _itemService;
        private readonly IZaloCenterService _zaloCenterService;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly ICustomerModelFactory  _customerModelFactory;
        private readonly IOrderModelFactory     _orderModelFactory   ;
        private readonly ITeleSaleModelFactory  _teleSaleModelFactory;
        private readonly INhanVienModelFactory  _nhanVienModelFactory;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ZaloSetting _zaloSetting;
        private readonly INhanVienService _nhanVienService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILogger _logger;


        #endregion

        #region Ctor

        public ZaloOAModelFactory(
            ILocalizationService localizationService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IStaticCacheManager staticCacheManager,
            IZaloAccountService itemService,
            IZaloCenterService zaloCenterService,
            ICustomerService customerService
            , IOrderService orderService
            ,ICustomerModelFactory customerModelFactory
            ,IOrderModelFactory    orderModelFactory   
            ,ITeleSaleModelFactory teleSaleModelFactory
            , INhanVienModelFactory nhanVienModelFactory
            , IBaseAdminModelFactory baseAdminModelFactory
            , ZaloSetting zaloSetting
            , INhanVienService nhanVienService
            , ICustomerActivityService customerActivityService
            ,ILogger logger
            )
        {
            this._localizationService = localizationService;
            this._storeContext = storeContext;
            this._workContext = workContext;
            this._staticCacheManager = staticCacheManager;
            this._itemService = itemService;
            _zaloCenterService = zaloCenterService;
            _customerService = customerService;
            _orderService = orderService;
            _customerModelFactory = customerModelFactory;
            _orderModelFactory = orderModelFactory;
            _teleSaleModelFactory = teleSaleModelFactory;
            _nhanVienModelFactory = nhanVienModelFactory;
            _baseAdminModelFactory = baseAdminModelFactory;
            _zaloSetting = zaloSetting;
            _nhanVienService = nhanVienService;
            _customerActivityService = customerActivityService;
            _logger = logger;
        }
        #endregion
        public ZaloAccountListModel PrepareZaloAccountListModel(ZaloAccountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
      
            //get items
            var items = _itemService.SearchZaloAccounts(Keysearch: searchModel.SearchKeyWord, pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new ZaloAccountListModel().PrepareToGrid(searchModel, items, () =>
            {
                var ls = new List<ZaloAccountModel>();
                foreach (var item in items)
                {
                    var itemModel = item.ToModel<ZaloAccountModel>();
                    ls.Add(itemModel);
                }
                return ls;

            });

            return model;
        }

        public ZaloAccountModel PrepareZaloAccountModel(ZaloAccount entity, ZaloAccountModel model)
        {
            if (entity != null)
            {
                //fill in model values from the entity
                model = entity.ToModel<ZaloAccountModel>();
            }
            //more

            return model;
        }
        public void PrepareZaloAccount(ZaloAccountModel model, ZaloAccount item)
        {
            item.Name = model.Name;
            item.AppId = model.AppId;
            item.Active = model.Active;
		}

        public ZaloAccountSearchModel PrepareZaloAccountSearchModel(ZaloAccountSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.SetGridPageSize();
            return searchModel;
        }

        public bool CheckAppId(string appId, int id)
        {
            bool check = true;
            if (id == 0)
            {
                var existsAccount = _itemService.GetAllZaloAccountByAppId(appId);
                if (existsAccount != null)
                {
                    check = false;
                }
            }
            else
            {
                var existsAccount = _itemService.GetAllZaloAccountByAppId(appId);
               
                if (existsAccount != null && existsAccount.Id != id)
                {
                    check = false;
                }
            }

            return check;
        }
        public bool CheckName(string name, int id)
        {
            bool check = true;
            if (id == 0)
            {
                var existsAccount = _itemService.GetZaloAccountByName(name);
                if (existsAccount != null)
                {
                    check = false;
                }
            }
            else
            {
                var existsAccount = _itemService.GetZaloAccountByName(name);

                if (existsAccount != null && existsAccount.Id != id)
                {
                    check = false;
                }
            }

            return check;
        }
		#region p
		public ZNSDto PrepareZnsDetail(OrderModel orderModel,string appId, string templateId = "304505", TeleSale teleSale = null, Customer customer = null, NhanVien nhanVien = null)
        {
            var order = _orderService.GetOrderById(orderModel.Id);
            var items = _zaloCenterService.DetailZNSTemplate(appId, templateId);
            var template = new Template();
            if(items != null && items.Result != null && items.Result.IsSuccess)
            {
				var response = items.Result.ObjectInfo.toStringJson().Replace("\\r\\n", "");
				response = response.Substring(1, response.Length - 2);
				response = response.Replace("\\", string.Empty);
                template = response.toEntity<Template>();
			}
            
            var item = new ZNSDto();
            var listParam = template.ListParams;
            var newJobject = new JObject();
            
            foreach (var param in listParam)
            {
                string paramName = param.Name;
                if (paramName.Contains(ZaloOaDefault.OAOrder))
                {
                    object value = GetOrderPropertyValue(order, paramName);

                    if (value is DateTime)
                    {
                        string formattedDateTime = ((DateTime)value).ToString("dd/MM/yyyy");

                        newJobject.Add(paramName, JToken.FromObject(formattedDateTime));
                    }
                    else
                    {
                        newJobject.Add(paramName, JToken.FromObject(value));
                    }
                }
            }
            item.template_data_string = newJobject.toStringJson();

            item.template_id = templateId;
            item.phone = _customerService.GetCustomerMainPhone(orderModel.CustomerId).ToVNPhoneNumber();
            item.tracking_id = order.Id.ToString();
            item.AppId = appId;
            return item;
        }
        

        public object GetOrderPropertyValue(Order order, string propertyName)
        {
            PropertyInfo property = typeof(Order).GetProperty(propertyName.Replace("order_", ""));
            
            if (propertyName.Contains("SalePhone"))
            {
                var sale = _nhanVienService.GetNhanVienById((int)order.NhanVienId);
                var phone = "";
                if(sale != null)
                {
                    phone = phone + sale.DienThoai;
                }
                if (string.IsNullOrEmpty(phone))
                {
                     phone = "Không có";
                }
                return phone;
            }
            if (propertyName.Contains("CustomerName"))
            {
                var customer = _customerService.GetCustomerById(order.CustomerId);
                var name = "";
                if (customer != null)
                {
                    name = _customerService.GetCustomerFullName(customer, IsLastName: true);
                }
                return name;
            }
            if (propertyName.Contains("OrderStatusId"))
            {
                int statusId = order.OrderStatusId;
                var statusString = GetVietnameseStatus(statusId);
                return statusString;
            }
            if (propertyName.Contains("OrderTotal"))
            {
                int orderTotal = (int)Math.Ceiling(order.OrderTotal);
                return orderTotal;
            }
            if (property != null)
            {
                return property.GetValue(order);
            }
            return null;
        }
        public void SenZNS(ZNSDto item, string appId)
        {
            _logger.Information("Đang gửi tin Zns cho khách hàng:  " + item.phone, fullMessalge: item.toStringJson());

            var ret = _zaloCenterService.SendZNS(item, appId);
            if(ret.Result.IsSuccess)
            {
                _logger.Information("Gửi tin Zns thành công", fullMessalge: item.toStringJson());
            }
            else
            {
                _logger.Information("Gửi tin Zns không thành công", fullMessalge: item.toStringJson());     
            }
        }

        public CauHinhZnsModel PrepareCauHinhZnsModel(string templateId, CauHinhZns item = null, List<CauHinhZns> listCauHinh = null, CauHinhZnsModel model = null)
        {
            model = new CauHinhZnsModel();

            if (item == null)
            {
                model = new CauHinhZnsModel();
                model.TemplateId = templateId;

            }
            else
            {
                model.TemplateId = item.TemplateId;
                model.LoaiId = item.LoaiId;
                model.OrderStatusId = item.OrderStatusId;
            }


            _baseAdminModelFactory.PrepareOrderStatuses(model.AvailableOrderStatuses);
			model.AvailableTrangThaiTeleSale = model.trangThai.ToSelectList();

			if (listCauHinh.Any())
            {
                var orderStatusDaDung = listCauHinh.Where(s=> s.TemplateId != model.TemplateId).Select(s => s.OrderStatusId).ToArray();
                model.AvailableOrderStatuses = new List<SelectListItem>();
                _baseAdminModelFactory.PrepareOrderStatuses(model.AvailableOrderStatuses, valuesToExclude: orderStatusDaDung);

                var trangThaiTelesaleDaDung = listCauHinh.Where(s => s.TemplateId != model.TemplateId).Select(s => s.TrangThaiId).ToArray();
				model.AvailableTrangThaiTeleSale = model.trangThai.ToSelectList(valuesToExclude: trangThaiTelesaleDaDung);
			}

			model.AvailableLoai = model.Loai.ToSelectList();
            return model;
        }
		public IList<TemplateModel> PrepareListZnsTemplateModel(IList<TemplateModel> templates)
        {
            var znsSetting = _zaloSetting.ZnsSetting;
            var listCauhinh = znsSetting.toEntities<CauHinhZns>();
            if(listCauhinh == null || listCauhinh.Count == 0)
            {
                return templates;
            }
            foreach (var template in templates) {
                var cauHinh = listCauhinh.FirstOrDefault(s => s.TemplateId == template.TeamplateId);
                if (cauHinh == null) continue;

                template.LoaiId = cauHinh.LoaiId;
                template.OrderStatusId = cauHinh.OrderStatusId;
                template.TrangThaiId = cauHinh.TrangThaiId;
            }
            return templates.OrderBy(s=> s.Status).ToList();
        }
        public string GetVietnameseStatus(int statusId)
        {
            if (_vietnameseTranslations.ContainsKey(statusId))
            {
                return _vietnameseTranslations[statusId];
            }
            else
            {
                return "Không xác định";
            }
        }

        #endregion

        #region static 
        public static Dictionary<int, string>  _vietnameseTranslations = new Dictionary<int, string>
        {
            { (int)OrderStatus.Draf, "Nháp" },
            { (int)OrderStatus.Pending, "Chờ xử lý" },
            { (int)OrderStatus.Checking, "Đang xác nhận" },
            { (int)OrderStatus.Checked, "Đã xác nhận" },
            { (int)OrderStatus.Processing, "Chờ xuất kho" },
            { (int)OrderStatus.Packaging, "Đang đóng gói" },
            { (int)OrderStatus.Packaged, "Đã đóng gói" },
            { (int)OrderStatus.PreShipping, "Chờ thu gom" },
            { (int)OrderStatus.Shipping, "Đang giao hàng" },
            { (int)OrderStatus.Returning, "Đang trả hàng" },
            { (int)OrderStatus.Returned, "Đã trả hàng" },
            { (int)OrderStatus.Complete, "Hoàn thành" },
            { (int)OrderStatus.Cancelled, "Đã hủy" }
        };
       
        #endregion
    }
}
