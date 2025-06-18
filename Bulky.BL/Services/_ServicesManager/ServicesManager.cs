using AutoMapper;
using Bulky.BL.Common.Attachments;
using Bulky.BL.Common.Payment;
using Bulky.BL.Services.Carts;
using Bulky.BL.Services.Categories;
using Bulky.BL.Services.Companies;
using Bulky.BL.Services.Orders;
using Bulky.BL.Services.Products;
using Bulky.DataAccess.Repository.UnitOfWork.UnitOfWork;
using StackExchange.Redis;

namespace Bulky.BL.Services._ServicesManager
{
    public class ServicesManager(IUnitOfWork _unitOfWork, IMapper _mapper, IAttachmentService _attachmentService ) : IServicesManager
    {

        private IProductService? _productService;

        public IProductService ProductService
        {
            get
            {
                if (_productService == null)
                {
                    _productService = new ProductService(_unitOfWork , _mapper);
                }
                return _productService;
            }
        }

        private ICategoryService? _categoryService;
        public ICategoryService CategoryService =>_categoryService ??= new CategoryService(_unitOfWork);



        private ICompanyService? _companyService;
        public ICompanyService CompanyService => _companyService ??=  new CompanyService(_unitOfWork,_mapper);

        private ICartServices? _cartService;
        public ICartServices CartServices => _cartService ??= new CartServices(_unitOfWork.CartRepository);


        public IMapper Mapper => _mapper;

        public IAttachmentService AttachmentService => _attachmentService;


        private IOrderServices? _orderService;
        public IOrderServices OrderServices => _orderService ??= new OrderServices(_unitOfWork);

        private IPaymentService? _paymentService;
        public IPaymentService PaymentService => _paymentService ??= new PaymentService(OrderServices);
    }
}
