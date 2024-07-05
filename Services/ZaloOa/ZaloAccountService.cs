using Nop.Core.Domain.DanhMuc;
using Nop.Core.Domain.QLChamCong;
using Nop.Core;
using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Zalo;
using Nop.Services.DanhMuc;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Events;
using System.Linq;
using Nop.Data.Mapping.Builders.Zalo;
using Nop.Services.Caching.Extensions;

namespace Nop.ServiceExtents.ZaloOA
{
    public class ZaloAccountService : IZaloAccountService
    {

        private readonly IEventPublisher _eventPublisher;

        private readonly HRMSettings _hrmRMSettings;

        private readonly ICacheKeyService _cacheKeyService;

        private readonly IStaticCacheManager _staticCacheManager;

        private readonly IRepository<ZaloAccount> _itemRepository;

        public ZaloAccountService(HRMSettings hrmRMSettings, IEventPublisher eventPublisher, ICacheKeyService cacheKeyService, IStaticCacheManager staticCacheManager, IRepository<ZaloAccount> zaloAccountRepository)
        {
            _hrmRMSettings = hrmRMSettings;
            _eventPublisher = eventPublisher;
            _cacheKeyService = cacheKeyService;
            _staticCacheManager = staticCacheManager;
            _itemRepository = zaloAccountRepository;
        }


        public IList<ZaloAccount> GetAllZaloAccounts(string appId = "")
        {
            IQueryable<ZaloAccount> source = _itemRepository.Table;
            if(!string.IsNullOrEmpty(appId))
            {
                source = source.Where(s => s.AppId .Contains(appId));
            }
            return source.ToList();
        }
        public IPagedList<ZaloAccount> SearchZaloAccounts(string Keysearch = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            IQueryable<ZaloAccount> source = _itemRepository.Table;
            if (!string.IsNullOrEmpty(Keysearch))
            {
                source = source.Where(s => s.Name.Contains(Keysearch) || s.AppId.Contains(Keysearch));
            }
            return new PagedList<ZaloAccount>(source, pageIndex, pageSize);
        }

        public ZaloAccount GetAllZaloAccountByAppId(string appId = "")
        {
            IQueryable<ZaloAccount> source = _itemRepository.Table;
            if (!string.IsNullOrEmpty(appId))
            {
                source = source.Where(s => s.AppId == appId);
            }
            return source.FirstOrDefault();
        }

        public ZaloAccount GetZaloAccountById(int Id)
        {
            if (Id == 0)
            {
                return null;
            }

            return _itemRepository.ToCachedGetById(Id);
        }

        public ZaloAccount GetZaloAccountByName(string name = "")
        {
            IQueryable<ZaloAccount> source = _itemRepository.Table;
            if (!string.IsNullOrEmpty(name))
            {
                source = source.Where(s => s.Name == name);
            }
            return source.FirstOrDefault();
        }

        public void InsertZaloAccount(ZaloAccount entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if(entity.Id == 0) {
                entity.Id = 1;
            }
            _itemRepository.Insert(entity);
            _eventPublisher.EntityInserted(entity);
        }

   

        public void UpdateZaloAccount(ZaloAccount entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

         

            _itemRepository.Update(entity);
            _eventPublisher.EntityUpdated(entity);
        }
        public void DeleteZaloAccount(ZaloAccount entity)
        {
            throw new NotImplementedException();
        }
    }
}
