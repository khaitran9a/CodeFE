using Nop.Core.Domain.Zalo;
using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.ServiceExtents.ZaloOA
{
    public interface IZaloAccountService
    {
        IList<ZaloAccount> GetAllZaloAccounts(string appId = "");

        IPagedList<ZaloAccount> SearchZaloAccounts(string Keysearch = null, int pageIndex = 0, int pageSize = int.MaxValue);

        ZaloAccount GetZaloAccountById(int Id);
        ZaloAccount GetAllZaloAccountByAppId(string appId = "");
        ZaloAccount GetZaloAccountByName(string name = "");

        void InsertZaloAccount(ZaloAccount entity);

        void UpdateZaloAccount(ZaloAccount entity);

        void DeleteZaloAccount(ZaloAccount entity);
    }
}
