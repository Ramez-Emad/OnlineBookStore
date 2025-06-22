using AutoMapper;
using Bulky.BL.Common.Attachments;
using Bulky.BL.Common.Payment;
using Bulky.BL.Services.Carts;
using Bulky.BL.Services.Categories;
using Bulky.BL.Services.Companies;
using Bulky.BL.Services.Orders;
using Bulky.BL.Services.Products;
using Bulky.BL.Services.Users;
using Bulky.DataAccess.Repository.Companies;

namespace Bulky.BL.Services._ServicesManager
{
    public interface IServicesManager 
    {
        IMapper Mapper {  get; }
        IAttachmentService AttachmentService { get; }
        IProductService ProductService { get; }
        ICategoryService CategoryService { get; }
        ICompanyService  CompanyService { get; }
        IOrderServices OrderServices { get; }
        ICartServices CartServices { get; }
        IPaymentService PaymentService { get; }
        IUserService UserService { get; }


    }
}
