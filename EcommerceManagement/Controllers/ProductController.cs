﻿using EcommerceManagement.Data;
using EcommerceManagement.Models.Domain;
using EcommerceManagement.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceManagement.Controllers
{
    public class ProductController : Controller
    {

        private readonly EcommerceDbContext _ecommerceDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(EcommerceDbContext ecommerceDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _ecommerceDbContext = ecommerceDbContext;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productList = _ecommerceDbContext.Products.ToList();
            var categoryList = _ecommerceDbContext.Categories.ToList();
            var categoryProduct = productList.Join(// outer sequence 
                       categoryList,  // inner sequence 
                       product => product.CategoryRefId,   // outerKeySelector
                       category => category.CategoryId, // innerKeySelector
                       (product, category) => new ProductCategoryDto // result selector
                       {
                           ProductId = product.ProductId,
                           ProductName = product.ProductName,
                           ProductDes = product.ProductDes,
                           ProductPrice = product.ProductPrice,
                           ProductImage = product.ProductImage,
                           IsAvailable = product.IsAvailable,
                           IsTrending = product.IsTrending,
                           CatagoryId = category.CategoryId,
                           CatagoryName = category.CategoryName
                       }).ToList();
            return View(categoryProduct);
        }

        [HttpGet]

        public async Task<IActionResult> GetProductsByCategory(Guid categoryId)
        {
            var productList = await _ecommerceDbContext.Products.ToListAsync();
            var categoryList = await _ecommerceDbContext.Categories.ToListAsync();
            var categoryProduct = productList.Join(// outer sequence 
                       categoryList,  // inner sequence 
                       product => product.CategoryRefId,   // outerKeySelector
                       category=>category.CategoryId, // innerKeySelector
                       (product, category) => new ProductCategoryDto // result selector
                       {
                           ProductId= product.ProductId,
                           ProductName=product.ProductName,
                           ProductDes=product.ProductDes,
                           ProductPrice=product.ProductPrice,
                           ProductImage=product.ProductImage,
                           IsAvailable=product.IsAvailable,
                           IsTrending=product.IsTrending,
                           CatagoryId=category.CategoryId,
                           CatagoryName=category.CategoryName
                       }).ToList();
            var selectedProducts = categoryProduct.Where(x=>x.CatagoryId== categoryId).ToList();
            if (categoryId == Guid.Parse("00000000-0000-0000-0000-000000000000")){
                return PartialView("_ProductListPartial", categoryProduct);
            }
            else
            {
                return PartialView("_ProductListPartial", selectedProducts);
            }

        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            ViewBag.CategoryList = await _ecommerceDbContext.Categories.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductDto addProductDto)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = "";
                if (addProductDto.ProductImage != null)
                {
                    string uploadFoler = Path.Combine(_webHostEnvironment.WebRootPath, "image");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + addProductDto.ProductImage.FileName;
                    string filePath = Path.Combine(uploadFoler, uniqueFileName);
                    addProductDto.ProductImage.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                var product = new ProductModel()
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = addProductDto.ProductName,
                    ProductPrice = addProductDto.ProductPrice,
                    ProductDes = addProductDto.ProductDes!,
                    IsAvailable = addProductDto.IsAvailable,
                    IsTrending = addProductDto.IsTrending,
                    CategoryRefId = addProductDto.CategoryRefId,
                    ProductImage = uniqueFileName,
                };
                _ecommerceDbContext.Products.Add(product);
                await _ecommerceDbContext.SaveChangesAsync();
                ViewBag.Success = "Student Added Successfully";
               
                return RedirectToAction("GetAll");
            }
            return View(addProductDto);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid id)
        {
            var product = await _ecommerceDbContext.Products.FirstOrDefaultAsync(x => x.ProductId == id);
            ViewBag.CategoryList = await _ecommerceDbContext.Categories.ToListAsync();
            if (product != null)
            {
                var newProduct = new UpdateProductDto()
                {
                    ProductName = product.ProductName,
                    ProductPrice = product.ProductPrice,
                    ProductDes = product.ProductDes,
                    IsAvailable = product.IsAvailable,
                    IsTrending = product.IsTrending,
                    ProductImage = product.ProductImage,
                    CategoryRefId = product.CategoryRefId,
                    Category = product.Category,
                };
                return await Task.Run(() => View("Update", newProduct));
            }
            return RedirectToAction("GetAll");
        }

        [HttpPost]

        public async Task<IActionResult> Update(UpdateProductDto updateProductDto)
        {
            var product = await _ecommerceDbContext.Products.FirstOrDefaultAsync(x => x.ProductId == updateProductDto.ProductId);
            ViewBag.CourseList = _ecommerceDbContext.Categories.ToList();
            if (product != null)
            {
                var newProduct = new UpdateProductDto()
                {
                    ProductName = product.ProductName,
                    ProductPrice = product.ProductPrice,
                    ProductDes = product.ProductDes,
                    IsAvailable = product.IsAvailable,
                    IsTrending = product.IsTrending,
                    ProductImage = product.ProductImage,
                    CategoryRefId = product.CategoryRefId,
                    Category = product.Category,
                };
                await _ecommerceDbContext.SaveChangesAsync();
            }
            return RedirectToAction("GetAll");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var student = _ecommerceDbContext.Products.FirstOrDefault(x => x.ProductId == id);
            if (student != null)
            {
               _ecommerceDbContext.Products.Remove(student);
                await _ecommerceDbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}
