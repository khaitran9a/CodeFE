using FluentValidation;
using Nop.Web.Areas.CRM.Factories.ZaloOA;
using Nop.Web.Areas.CRM.Models.Zalo;
using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.CRM.Validators
{
    public partial class ZaloAccountValidator : BaseNopValidator<ZaloAccountModel>
    {
        public ZaloAccountValidator(IZaloOAModelFactory _zaloAccountModelFactory)
        {
            RuleFor(x => x.AppId).NotEmpty()
                .WithMessage("AppId không được để trống");
            RuleFor(x => x.AppId).Must((model, appId) =>
            {
                if (string.IsNullOrEmpty(appId))
                {
                    return true;
                }
                return _zaloAccountModelFactory.CheckAppId(appId, model.Id);
            }).WithMessage("AppId này đã tồn tại!");

            RuleFor(x => x.Name).NotEmpty()
               .WithMessage("Tên tài khoản không được để trống");
            RuleFor(x => x.Name).Must((model, name) =>
            {
                if (string.IsNullOrEmpty(name))
                {
                    return true;
                }
                return _zaloAccountModelFactory.CheckName(name, model.Id);
            }).WithMessage("Tên tài khoản này đã tồn tại!");
        }
    
    }
}
