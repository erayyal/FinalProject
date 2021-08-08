using System.Collections.Generic;
using System.Linq;
using Business.Abstract;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;


namespace Business.Concrete
{
    public class ProductManager:IProductService
    {
        IProductDal _productDal;
        ICategoryService _categoryService;

        public ProductManager(IProductDal productDal, ICategoryService categoryService)
        {
            _productDal = productDal;
            _categoryService = categoryService;
        }

        //[CacheAspect]  //key(cache'in adı), value
        public IDataResult<List<Product>> GetAll()
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(),Messages.ProductListed);
        }

        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p=>p.CategoryId==id));
        }

        public IDataResult<List<Product>> GetByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p=>p.UnitPrice>= min && p.UnitPrice<=max));
        }

        public IDataResult<List<ProductDetailDto>> GetProductDetails()
        {
            return new SuccessDataResult<List<ProductDetailDto>>(_productDal.GetProductDetails());
        }

        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p=>p.ProductId==productId));
        }

        //JWT = Api'li yapılarda Yetkilendirme
        //Claim
        //Client=mobil, web uygulama, tarayıcılar 
        
        //[SecuredOperation("product.add,admin")] Kayıt olunuyor fakat giriş yap kısmında hata görünüyor ?_
        [ValidationAspect(typeof(ProductValidator))] //attribute = Belli kurallardır önce bunlar çalışır. Anlam yükler.
        public IResult Add(Product product)
        {
            IResult result = BusinessRules.Run(CheckIfProductNameExists(product.ProductName),
                CheckIfProductCountOfCategoryCorrect(product.CategoryId),
                CheckIfCategoryLimitExceded());

            if (result!=null)
            {
                return result;
            }
            _productDal.Add(product);
            return new SuccessResult(Messages.ProductAdded);
        }


        [ValidationAspect(typeof(ProductValidator))] 
        public IResult Update(Product product)
        {
            IResult result = BusinessRules.Run(CheckIfProductNameExists(product.ProductName),
                CheckIfProductCountOfCategoryCorrect(product.CategoryId),
                CheckIfCategoryLimitExceded());
            if (result!=null)
            {
                return result;
            }
            _productDal.Update(product);
            return new SuccessResult();
        }
        
        //BUSİNESS RULES;
        private IResult CheckIfProductCountOfCategoryCorrect(int categoryId)
        {
            var result = _productDal.GetAll(p => p.CategoryId == categoryId).Count;
            if (result>=10)
            {
                return new ErrorResult(Messages.ProductCountOfCategoryError);
            }
            return new SuccessResult();
        }

        private IResult CheckIfProductNameExists(string productName)
        {
            var result = _productDal.GetAll(p => p.ProductName == productName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExist);
            }
            return new SuccessResult();
        }
        
        private IResult CheckIfCategoryLimitExceded()
        {
            var result = _categoryService.GetAll();
            if (result.Data.Count > 15)
            {
                return new ErrorResult(Messages.CategoryLimitExceded);
            }
            return new SuccessResult();
        }
    }
}