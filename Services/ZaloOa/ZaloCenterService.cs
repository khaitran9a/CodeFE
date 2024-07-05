using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Zalo;
using Nop.ServiceExtents.ZaloOA.Dtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nop.ServiceExtents.ZaloOA
{
    public class ZaloCenterService : IZaloCenterService
    {
        #region fileds
        private readonly HttpClient _httpClient;
        private readonly IZaloAccountService _zaloAccountService;
        #endregion

        #region ctor
        public ZaloCenterService(HttpClient httpClient
            , IZaloAccountService zaloAccountService)
        {
            _httpClient = httpClient;
            _zaloAccountService = zaloAccountService;
        }
        #endregion

        #region account methods
        public async Task<MessageReturn> AccountCreate(ZaloAccountDto item)
        {
            var endPoint = $"{ZaloCenterExtension.EndPoint}{ZaloCenterExtension.AccountCreate}";
            var payLoad = item.toStringJson() ;
            return await SendRequestPost(payLoad, endPoint);
        }
        public virtual async Task<MessageReturn> GetAccountByIdAsync(int id)
        {
            var action = string.Format(ZaloCenterExtension.GetAccountById, id);
            var endPoint = $"{ZaloCenterExtension.EndPoint}{action}";

            return await SendRequestGet(endPoint);
        }

        public async Task<MessageReturn> AccountUpdate(int id, ZaloAccountDto item)
        {
            var acction = string.Format(ZaloCenterExtension.AccountUpdate, id);
            var endPoint = $"{ZaloCenterExtension.EndPoint}{acction}";
            var payLoad = item.toStringJson();
            return await SendRequestPut(payLoad, endPoint);
        }


        public async Task<MessageReturn> GetListAccount()
		{
			var acction = string.Format(ZaloCenterExtension.ListAccount);
			var endPoint = $"{ZaloCenterExtension.EndPoint}{acction}";
			return await SendRequestGet(endPoint);
		}

		public async Task<MessageReturn> GetListAccount(int id)
		{
			var acction = string.Format(ZaloCenterExtension.GetAccountById, id);
			var endPoint = $"{ZaloCenterExtension.EndPoint}{acction}";
			return await SendRequestGet(endPoint);
		}
		#endregion

		#region BroadCast Methods

		public virtual async Task<MessageReturn> GetListBroadCastAsync(string appId)
        {
            _httpClient.DefaultRequestHeaders.Remove("oa_app_id");
            _httpClient.DefaultRequestHeaders.Add("oa_app_id", appId);
            var endPoint = $"{ZaloCenterExtension.EndPoint}{ZaloCenterExtension.BroadCastList}";

            return await SendRequestGet(endPoint);
        }

        public virtual async Task<MessageReturn> DetailBroadCastAsync(string appId, string articleId)
        {
            _httpClient.DefaultRequestHeaders.Remove("oa_app_id");
            _httpClient.DefaultRequestHeaders.Add("oa_app_id", appId);
            var action = string.Format(ZaloCenterExtension.BroadCastDetail, articleId);
            var endPoint = $"{ZaloCenterExtension.EndPoint}{action}";

            return await SendRequestGet(endPoint);
        }

        public virtual async Task<MessageReturn> PrepareSendBroadCast()
        {
            var endPoint = $"{ZaloCenterExtension.EndPoint}{ZaloCenterExtension.BroadCastSend}";

            return await SendRequestGet(endPoint);
        }

        public virtual async Task<MessageReturn> SendBroadCast(SendBroadCastDto model)
        {
            var account =  _zaloAccountService.GetZaloAccountById(model.ZaloAccountId);
            _httpClient.DefaultRequestHeaders.Remove("oa_app_id");
            _httpClient.DefaultRequestHeaders.Add("oa_app_id", account.AppId);
            var endPoint = $"{ZaloCenterExtension.EndPoint}{ZaloCenterExtension.BroadCastSend}";

            return await SendRequestPost(model.toStringJson(), $"{endPoint}?articleId={model.ArticleId}");
        }


        #endregion

        #region ZNS

        public virtual async Task<MessageReturn> GetListZSNTemplate(string appId, int offSet = 0,  int limit = 100)
        {
            _httpClient.DefaultRequestHeaders.Remove("oa_app_id");
            _httpClient.DefaultRequestHeaders.Add("oa_app_id", appId);
            var action = string.Format(ZaloCenterExtension.ZnsListTemp, offSet, limit);

            var endPoint = $"{ZaloCenterExtension.EndPoint}{action}";

            return await SendRequestGet(endPoint);
        }

        public virtual async Task<MessageReturn> DetailZNSTemplate(string appId, string templateId)
        {
            _httpClient.DefaultRequestHeaders.Remove("oa_app_id");
            _httpClient.DefaultRequestHeaders.Add("oa_app_id", appId);
            var action = string.Format(ZaloCenterExtension.ZnsDetailTemp, templateId);
            var endPoint = $"{ZaloCenterExtension.EndPoint}{action}";

            return await SendRequestGet(endPoint);
        }
        // tin truyền thông
        public virtual async Task<MessageReturn> SendMessageMedia(RequestElementModel model, int accountZaloId = 1)
		{
			var account = _zaloAccountService.GetZaloAccountById(accountZaloId);
			_httpClient.DefaultRequestHeaders.Remove("oa_app_id");
			_httpClient.DefaultRequestHeaders.Add("oa_app_id", account.AppId);
			var endPoint = $"{ZaloCenterExtension.EndPoint}{ZaloCenterExtension.TruyenThong}";

			return await SendRequestPost(model.toStringJson(), $"{endPoint}");
		}
		// tin giao dịch
		public virtual async Task<MessageReturn> SendTransactionNews(RequestElementModel model, int accountZaloId = 1)
		{
			var account = _zaloAccountService.GetZaloAccountById(accountZaloId);
			_httpClient.DefaultRequestHeaders.Remove("oa_app_id");
			_httpClient.DefaultRequestHeaders.Add("oa_app_id", account.AppId);
			var endPoint = $"{ZaloCenterExtension.EndPoint}{ZaloCenterExtension.GiaoDich}";

			return await SendRequestPost(model.toStringJson(), $"{endPoint}");
		}
        // send zns
        public virtual async Task<MessageReturn> SendZNS(ZNSDto model, string appId )
        {
            _httpClient.DefaultRequestHeaders.Remove("oa_app_id");
            _httpClient.DefaultRequestHeaders.Add("oa_app_id", appId);
            var endPoint = $"{ZaloCenterExtension.EndPoint}{ZaloCenterExtension.Zns}";
            return await SendRequestPost(model.toStringJson(), $"{endPoint}");
        }
        #endregion
        #region Token
        public virtual async Task<MessageReturn> RefreshToken(string appId)
        {

            var endPoint = $"{ZaloCenterExtension.EndPoint}{ZaloCenterExtension.RefreshToken}";

            return await SendRequestPost("", $"{endPoint}?appId={appId}");
        }

        #endregion
        #region private
        private async Task<MessageReturn> SendRequestPost(string json, string endPoint)
        {
            var payLoad = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endPoint, payLoad);
            var result = (await response.Content.ReadAsStringAsync()).toEntity<MessageReturn>();

            return result;
        }
        private async Task<MessageReturn> SendRequestPut(string json, string endPoint)
        {
            var payLoad = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(endPoint, payLoad);
            var result = (await response.Content.ReadAsStringAsync()).toEntity<MessageReturn>();

            return result;
        }

        private async Task<MessageReturn> SendRequestGet(string endPoint)
        {
            var response = await _httpClient.GetAsync(endPoint);
            var result = (await response.Content.ReadAsStringAsync()).toEntity<MessageReturn>();

            return result;
        }

        private async Task<MessageReturn> SendRequestDelete(string endPoint)
        {
            var response = await _httpClient.DeleteAsync(endPoint);
            var result = (await response.Content.ReadAsStringAsync()).toEntity<MessageReturn>();

            return result;
        }
        #endregion
    }
}
