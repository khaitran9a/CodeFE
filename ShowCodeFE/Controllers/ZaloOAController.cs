using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Configuration;
using Nop.Core.Domain.HeThong;
using Nop.Core.Domain.Zalo;
using Nop.ServiceExtents;
using Nop.ServiceExtents.ZaloOA;
using Nop.ServiceExtents.ZaloOA.Dtos;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.CRM.Factories.ZaloOA;
using Nop.Web.Areas.CRM.Models.Zalo;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Renci.SshNet.Messages.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZaloNotification.Entities;

namespace Nop.Web.Areas.CRM.Controllers
{
    public class ZaloOAController : BaseCRMController
    {
        #region fields
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IZaloOAModelFactory _zaloAccountModelFactory;
        private readonly IZaloAccountService _itemService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IZaloCenterService _zaloCenterService;
        private readonly ZaloSetting _zaloSetting;
        private readonly ISettingService _settingService;
        #endregion


        #region Ctor
        public ZaloOAController(INotificationService notificationService
            , IPermissionService permissionService
            , IWorkContext workContext
            , IStoreContext storeContext
            , IZaloOAModelFactory zaloAccountModelFactory,
            IZaloAccountService itemService
            , ICustomerActivityService customerActivityService
            ,IZaloCenterService zaloCenterService,
            ZaloSetting zaloSetting
            , ISettingService settingService
            )
        {
            _notificationService = notificationService;
            _permissionService = permissionService;
            _workContext = workContext;
            _storeContext = storeContext;
            _zaloAccountModelFactory = zaloAccountModelFactory;
            _itemService = itemService;
            _customerActivityService = customerActivityService;
            _zaloCenterService = zaloCenterService;
            _zaloSetting = zaloSetting;
            _settingService = settingService;
        }
        #endregion

        #region Account
        public IActionResult Index()
        {
            return RedirectToAction("ListZaloAccount");
        }

        public virtual IActionResult ListZaloAccount()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = _zaloAccountModelFactory.PrepareZaloAccountSearchModel(new ZaloAccountSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ListAccount(ZaloAccountSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _zaloAccountModelFactory.PrepareZaloAccountListModel(searchModel);
            var a = _zaloCenterService.GetAccountByIdAsync(4);

			return Json(model);
        }

        public virtual IActionResult _ThemMoiZaloAccount(int id = 0)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedPartialView();

            _zaloCenterService.RefreshToken("1858654334366107519");

            //prepare model
            var model = new ZaloAccountModel();

            if (id > 0)
            {
                var item = _itemService.GetZaloAccountById(id);
                if (item != null)
                {
                    model = _zaloAccountModelFactory.PrepareZaloAccountModel(item, null);
                }
            }
            else
            {
                model = _zaloAccountModelFactory.PrepareZaloAccountModel(null, new ZaloAccountModel());
            }
            model.IsSuaWebhookUrl = _permissionService.Authorize(AdvancePermissionProvider.CRMSuaWebhook);
            return PartialView(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> _ThemMoiAccount(ZaloAccountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                var item = model.ToEntity<ZaloAccount>();
                // call api zalo center -> true / false
                // true -> continiue
                // false -> return message
                var itemToCallApi = new ZaloAccountDto
                {
                    Name = model.Name,
                    SecretKey = model.SecretKey,
                    RefreshToken = model.RefreshToken,
                    AccessToken = model.AccessToken,
                    AppId  = model.AppId,
                    OaId = model.OaId,

                };
                var codeverfy = Guid.NewGuid();
                itemToCallApi.CodeVerifier = codeverfy.ToString();
                itemToCallApi.CodeChallenge = Guid.NewGuid().ToString();
                if (!string.IsNullOrEmpty(model.WebhookUrl))
                {
                    itemToCallApi.WebhookUrl = model.WebhookUrl;
				}

                var ret = await _zaloCenterService.AccountCreate(itemToCallApi);
                if (!ret.IsSuccess)
                {
                    return JsonErrorMessage("Thêm mới tài khoản không thành công!");
                }
                _itemService.InsertZaloAccount(item);
                _customerActivityService.InsertActivity("AddNewZaloAccount", string.Format("Thêm mới: {0}", item.Id), item);
                return JsonSuccessMessage("Thêm mới tài khoản thành công!");
            }

            //prepare model
            model = _zaloAccountModelFactory.PrepareZaloAccountModel(null, model);
            return PartialView(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> _CapNhatAccount(ZaloAccountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var item = _itemService.GetZaloAccountById(model.Id);
            if (item == null)
                return RedirectToAction("ListZaloAccount");
            if (ModelState.IsValid)
            {
                _zaloAccountModelFactory.PrepareZaloAccount(model, item);
				var itemToCallApi = new ZaloAccountDto
				{
					Name = item.Name,
					SecretKey = item.SecretKey,
					AppId = item.AppId,
                    RefreshToken = model.RefreshToken
				};
				if (!string.IsNullOrEmpty(model.WebhookUrl))
				{
					itemToCallApi.WebhookUrl = model.WebhookUrl;
				}

				_itemService.UpdateZaloAccount(item);
                var ret = await _zaloCenterService.AccountUpdate(item.Id, itemToCallApi);

                _customerActivityService.InsertActivity("EditZaloAccount", string.Format("Cập nhật: {0}", item.Id), item);
                return JsonSuccessMessage("Cập nhật tài khoản thành công!");
            }
            //prepare model
            model = _zaloAccountModelFactory.PrepareZaloAccountModel(item, model);
            return PartialView(model);
        }


        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HRMDanhMucChung))
                return AccessDeniedView();
            //prepare model
            var model = _zaloAccountModelFactory.PrepareZaloAccountModel(new ZaloAccount(), null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(ZaloAccountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HRMDanhMucChung))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var item = model.ToEntity<ZaloAccount>();
                _itemService.InsertZaloAccount(item);
                _customerActivityService.InsertActivity("AddNewZaloAccount", string.Format("Thêm mới: {0}", item.Id), item);
                _notificationService.SuccessNotification("Tạo mới dữ liệu thành công !");
                return continueEditing ? RedirectToAction("Edit", new { id = item.Id }) : RedirectToAction("ListZaloAccount");
            }

            //prepare model
            model = _zaloAccountModelFactory.PrepareZaloAccountModel(null, model);
            return View(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HRMDanhMucChung))
                return AccessDeniedView();

            var item = _itemService.GetZaloAccountById(id);
            if (item == null)
                return RedirectToAction("ListZaloAccount");
            //prepare model
            var model = _zaloAccountModelFactory.PrepareZaloAccountModel(item, null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(ZaloAccountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HRMDanhMucChung))
                return AccessDeniedView();
            //try to get a store with the specified id
            var item = _itemService.GetZaloAccountById(model.Id);
            if (item == null)
                return RedirectToAction("ListZaloAccount");
            if (ModelState.IsValid)
            {
                _zaloAccountModelFactory.PrepareZaloAccountModel(item, model);
                _itemService.UpdateZaloAccount(item);
                _customerActivityService.InsertActivity("EditZaloAccount", string.Format("Cập nhật: {0}", item.Id), item);
                _notificationService.SuccessNotification("Cập nhật dữ liệu thành công !");
                return continueEditing ? RedirectToAction("Edit", new { id = item.Id }) : RedirectToAction("ListZaloAccount");
            }
            //prepare model
            model = _zaloAccountModelFactory.PrepareZaloAccountModel(item, model);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.HRMDanhMucChung))
                return AccessDeniedView();
            //try to get a store with the specified id
            var item = _itemService.GetZaloAccountById(id);
            if (item == null)
                return RedirectToAction("ListZaloAccount");
            try
            {
                _itemService.DeleteZaloAccount(item);
                //activity log  
                _customerActivityService.InsertActivity("DeleteZaloAccount", string.Format("Xóa: {0}", item.Id), item);
                _notificationService.SuccessNotification("Xoá dữ liệu thành công");
                return RedirectToAction("ListZaloAccount");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = item.Id });
            }
        }

        #endregion

        #region tin Broadcast 

        [HttpGet("listBroadCast")]
        public virtual async Task<IActionResult> ListBroadCast(int accountId)
        {
            if (! _permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return Forbid();
            var account =  _itemService.GetZaloAccountById(accountId);
            var items = await _zaloCenterService.GetListBroadCastAsync(account.AppId);
            if (!items.IsSuccess) return BadRequest(items.Message);
            var response = items.ObjectInfo.toStringJson().Replace("\\r\\n", "");
            response = response.Substring(1, response.Length - 2);
            response = response.Replace("\\", string.Empty);
            var model = response.toEntity<BroadCastWrapperModel>();
            model.TenTaiKhoan = account.Name;
            model.AppId = account.AppId;
            model.ZaloId = account.Id;
            return View(model);
        }

        [HttpGet("detailBroadCast")]
        public virtual async Task<IActionResult> DetailBroadCast(int accountId, string articleId)
        {
            if (! _permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return Forbid();

            var account =  _itemService.GetZaloAccountById(accountId);
            var item = await _zaloCenterService.DetailBroadCastAsync(account.AppId, articleId);
            if (!item.IsSuccess) return BadRequest(item.Message);
            var response = item.ObjectInfo.toStringJson().Replace("\\r\\n", "");
            response = response.Substring(1, response.Length - 2);
            response = response.Replace("\\", string.Empty);
            var model = response.toEntity<Article>();

            var sendItem = await _zaloCenterService.PrepareSendBroadCast();
            if (!sendItem.IsSuccess) return BadRequest(sendItem.Message);
            var response2 = sendItem.ObjectInfo.toStringJson();
            model.SendBroadCastDto = response2.toEntity<SendBroadCastDto>();
            model.SendBroadCastDto.recipient.AvailableCities = PripareAvailableCities();
            model.SendBroadCastDto.ArticleId = articleId;
            model.SendBroadCastDto.ZaloAccountId = accountId;
            model.SendBroadCastDto.AppId = account.AppId;
            return View(model);
        }

        //[HttpGet("sendBroadCast")]
        //public virtual async Task<IActionResult> SendBroadCast()
        //{
        //    if (! _permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
        //        return Forbid();

        //    var item =  _zaloAccountModelFactory.PrepareSendBroadCast();

        //    return item.IsSuccess ? Ok(item.ObjectInfo) : BadRequest(item.Message);
        //}

        [HttpPost("sendBroadCast")]
        public virtual async Task<IActionResult> SendBroadCast(SendBroadCastDto model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return Forbid();
            MessageModel message = new MessageModel
            {
                attachment = new AttachmentModel
                {
                    type = "template",
                    payload = new AttachmentPayloadModel
                    {
                        template_type = "media",
                        elements = new List<PayloadElementModel>
            {
                new PayloadElementModel
                {
                    media_type = "article",
                    attachment_id = model.ArticleId
                }
            }
                    }
                }
            };
            model.message = message;

            var item = await _zaloCenterService.SendBroadCast(model);
            if (item.IsSuccess)
            {
               return  RedirectToAction("ListAccount");
            }
            return JsonErrorMessage("Có lỗi xảy ra!");
        }
        private IList<SelectListItem> PripareAvailableCities()
        {
            
        var cityDictionary = new Dictionary<int, string>
            {
                {0, "Đồng Tháp"},
                {1, "Bình Phước"},
                {2, "Ninh Bình"},
                {3, "Bạc Liêu"},
                {4, "Hồ Chí Minh"},
                {5, "Vĩnh Long"},
                {6, "Lâm Đồng"},
                {7, "Yên Bái"},
                {8, "Hà Nam"},
                {9, "Hà Nội"},
                {10, "Hải Dương"},
                {11, "Hậu Giang"},
                {12, "An Giang"},
                {13, "Trà Vinh"},
                {14, "Tiền Giang"},
                {15, "Tây Ninh"},
                {16, "Đồng Nai"},
                {17, "Đắk Lắk"},
                {18, "Bình Định"},
                {19, "Kon Tum"},
                {20, "Đà Nẵng"},
                {21, "Bắc Giang"},
                {22, "Bắc Kạn"},
                {23, "Điện Biên"},
                {24, "Hòa Bình"},
                {25, "Thái Bình"},
                {26, "Vĩnh Phúc"},
                {27, "Hà Giang"},
                {28, "Kiên Giang"},
                {29, "Bình Bình"},
                {30, "Bình Thuận"},
                {31, "Đắk Nông"},
                {32, "Khánh Hòa"},
                {33, "Gia Lai"},
                {34, "Quảng Nam"},
                {35, "Quảng Trị"},
                {36, "Hà Tĩnh"},
                {37, "Hưng Yên"},
                {38, "Quảng Ninh"},
                {39, "Thanh Hóa"},
                {40, "Phú Thọ"},
                {41, "Lai Châu"},
                {42, "Thái Nguyên"},
                {43, "Cao Bằng"},
                {44, "Cà Mau"},
                {45, "Cần Thơ"},
                {46, "Sóc Trăng"},
                {47, "Bến Tre"},
                {48, "Long An"},
                {49, "Bà Rịa Vũng Tàu"},
                {50, "Ninh Thuận"},
                {51, "Phú Yên"},
                {52, "Quãng Ngãi"},
                {53, "Thừa Thiên Huế"},
                {54, "Quảng Bình"},
                {55, "Nghệ An"},
                {56, "Nam Định"},
                {57, "Hải Phòng"},
                {58, "Lạng Sơn"},
                {59, "Lào Cai"},
                {60, "Sơn La"},
                {61, "Bắc Ninh"},
                {62, "Tuyên Quang"},
                {63, "Không Thuộc Việt Nam"}
            };

            // Tạo danh sách SelectListItem từ Dictionary
            var AvailableCities = cityDictionary.Select(x => new SelectListItem
            {
                Text = x.Value,
                Value = x.Key.ToString()
            }).ToList();
            return AvailableCities;
        }
        #endregion


        #region tin Broadcast 

        [HttpGet("ListZnsTemplate")]
        public virtual async Task<IActionResult> ListZnsTemplate(int accountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return Forbid();
            var account = _itemService.GetZaloAccountById(accountId);
            var items = await _zaloCenterService.GetListZSNTemplate(account.AppId, 0, 20);
            if (!items.IsSuccess) return BadRequest(items.Message);
            var response = items.ObjectInfo.toStringJson().Replace("\\r\\n", "");
            response = response.Substring(1, response.Length - 2);
            response = response.Replace("\\", string.Empty);
            var model = new ZNSModel();
            model.templates = response.toEntities<TemplateModel>();
            model.templates = _zaloAccountModelFactory.PrepareListZnsTemplateModel(model.templates);

			model.AppName = account.Name;
            model.AppId = account.AppId;
            model.AccountId = account.Id;

            return View(model);
        }

        [HttpGet("ZnsTemplateInfor")]
        public virtual async Task<IActionResult> GetZnsTemplateInfor(int accountId, string templateId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return Forbid();

            var account = _itemService.GetZaloAccountById(accountId);
            var item = await _zaloCenterService.DetailZNSTemplate(account.AppId, templateId);
            if (!item.IsSuccess) return BadRequest(item.Message);
            var response = item.ObjectInfo.toStringJson().Replace("\\r\\n", "");
            response = response.Substring(1, response.Length - 2);
            response = response.Replace("\\", string.Empty);
            var model = response.toEntity<Article>();

            var sendItem = await _zaloCenterService.PrepareSendBroadCast();
            if (!sendItem.IsSuccess) return BadRequest(sendItem.Message);
            var response2 = sendItem.ObjectInfo.toStringJson();
            model.SendBroadCastDto = response2.toEntity<SendBroadCastDto>();
            model.SendBroadCastDto.recipient.AvailableCities = PripareAvailableCities();
            
            return View(model);
        }

        #region ZNS
        public virtual IActionResult _CauHinhZns(string tid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedPartialView();

          
            var znsSetting = _zaloSetting.ZnsSetting;
            //prepare model
            var listCauHinh = new List<CauHinhZns>();
            if (!string.IsNullOrEmpty(znsSetting))
            {
                listCauHinh = znsSetting.toEntities<CauHinhZns>();
            }
            var item = listCauHinh.FirstOrDefault(s => s.TemplateId == tid);
            var model = _zaloAccountModelFactory.PrepareCauHinhZnsModel(templateId: tid,  item: item,  listCauHinh: listCauHinh,  model: null);
            return PartialView(model);
        }

        [HttpPost]
        public virtual  IActionResult _CauHinhZns(CauHinhZns item)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            switch (item.LoaiId)
            {
                case (int)LoaiDoiTuongTinZnsEnum.TeleSale:
                    item.OrderStatusId = 0;
                    break;
                case (int)LoaiDoiTuongTinZnsEnum.Order:
                    item.TrangThaiId = 0;
                    break;
            }
            var znsSetting = _zaloSetting.ZnsSetting;

            var listCauHinh = new List<CauHinhZns>();
            if (!string.IsNullOrEmpty(znsSetting))
            {
                listCauHinh = znsSetting.toEntities<CauHinhZns>();
            }
            var cauHinh = listCauHinh.Where(s => s.TemplateId == item.TemplateId).FirstOrDefault();
            if (cauHinh != null)
            {
                listCauHinh.Remove(cauHinh);
                cauHinh = item;
                listCauHinh.Add(cauHinh);
            }
            else
            {
                listCauHinh.Add(item);
            }

            var setting = _settingService.GetSetting("zaloSetting.ZnsSetting");
            znsSetting = listCauHinh.toStringJson();
            _settingService.SetSetting(setting.Name, znsSetting);
            return JsonSuccessMessage("Cài đặt cấu hình thành công!");

            //return PartialView(model);
        }

		[HttpPost]
		public virtual IActionResult _XoaCauHinhZns(string tid)
		{
			if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			if(string.IsNullOrEmpty(tid)) {
				return JsonErrorMessage("Không lấy được thông tin Zns!");

			}
			var setting = _settingService.GetSetting("zaloSetting.ZnsSetting");

			var znsSetting = _zaloSetting.ZnsSetting;

			var listCauHinh = new List<CauHinhZns>();
			if (!string.IsNullOrEmpty(znsSetting))
			{
				listCauHinh = znsSetting.toEntities<CauHinhZns>();
			}
			var cauHinh = listCauHinh.Where(s => s.TemplateId == tid).FirstOrDefault();
			if (cauHinh != null)
			{
				listCauHinh.Remove(cauHinh);
				znsSetting = listCauHinh.toStringJson();
				_settingService.SetSetting(setting.Name, znsSetting);
				return JsonSuccessMessage("Cài đặt lại cấu hình thành công!");

			}
			return JsonErrorMessage("Zns này chưa được cậu hình.");

		}

		#endregion

		#endregion
	}
}
