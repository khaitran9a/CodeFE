
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Zalo;
using Nop.ServiceExtents.ZaloOA.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Nop.ServiceExtents.ZaloOA
{
    public interface IZaloCenterService
    {
        Task<MessageReturn> AccountCreate(ZaloAccountDto item);
        Task<MessageReturn> GetAccountByIdAsync(int id);
        Task<MessageReturn> AccountUpdate(int id, ZaloAccountDto item);
        Task<MessageReturn> GetListAccount();
        Task<MessageReturn> GetListAccount(int id);
		Task<MessageReturn> GetListBroadCastAsync(string appId);
        Task<MessageReturn> DetailBroadCastAsync(string appId, string articleId);
        Task<MessageReturn> PrepareSendBroadCast();
        Task<MessageReturn> SendBroadCast(SendBroadCastDto model);
        Task<MessageReturn> GetListZSNTemplate(string appId, int offSet = 0, int limit = 100);
        Task<MessageReturn> DetailZNSTemplate(string appId, string templateId);
        Task<MessageReturn> SendZNS(ZNSDto model, string appId);
        Task<MessageReturn> RefreshToken(string appId);
    }
}
