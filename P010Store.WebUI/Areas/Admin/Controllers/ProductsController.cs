using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using P010Store.Entities;
using P010Store.Service.Abstract;
using P010Store.WebUI.Utils;

namespace P010Store.WebUI.Areas.Admin.Controllers
{
	[Area("Admin"), Authorize(Policy = "AdminPolicy")]
	public class ProductsController : Controller
	{
		private readonly IService<Product> _service;
		private readonly IService<Category> _serviceCategory;
		private readonly IService<Brand> _serviceBrand;
		private readonly IProductService _productService; //InvalidOperationException: Unable to resolve service for type 'P010Store.Service.Abstract.IProductService' while attempting to activate 'P010Store.WebUI.Areas.Admin.Controllers.ProductsController'. Bu servisi kullandığımızda bu hatayı alırız, bu sorunu çözmek için servisi program.cs de tanımlamamız gerekir.

		public ProductsController(IService<Product> service, IService<Category> serviceCategory, IService<Brand> serviceBrand, IProductService productService)
		{
			_service = service;
			_serviceCategory = serviceCategory;
			_serviceBrand = serviceBrand;
			_productService = productService;
		}

		// GET: ProductsController
		public async Task<ActionResult> Index()
		{
			var model = await _productService.GetAllProductsByCategoriesBrandsAsync(); // eskisi bu _service.GetAll();
			return View(model);
		}

		// GET: ProductsController/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: ProductsController/Create
		public async Task<ActionResult> Create()
		{
			ViewBag.CategoryId = new SelectList(await _serviceCategory.GetAllAsync(), "Id", "Name");
			ViewBag.BrandId = new SelectList(await _serviceBrand.GetAllAsync(), "Id", "Name");
			return View();
		}

		// POST: ProductsController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(Product product, IFormFile? Image)
		{
			if (ModelState.IsValid)
			{
				try
				{
					if (Image is not null) product.Image = await FileHelper.FileLoaderAsync(Image, filePath: "/wwwroot/Img/Products/");
					await _service.AddAsync(product);
					await _service.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}
				catch
				{
					ModelState.AddModelError("", "Hata Oluştu!");
				}
			}
			ViewBag.CategoryId = new SelectList(await _serviceCategory.GetAllAsync(), "Id", "Name"); // Burada ürün ekleme esnasında ekleme başarısız olursa ekrandaki select elementlerine verileleri tekrar gönderiyoruz aksi takdirde null reference hatası alırız.
			ViewBag.BrandId = new SelectList(await _serviceBrand.GetAllAsync(), "Id", "Name");

			return View(product);
		}

		// GET: ProductsController/Edit/5
		public async Task<ActionResult> Edit(int id)
		{
			var model = await _service.FindAsync(id);
			ViewBag.CategoryId = new SelectList(await _serviceCategory.GetAllAsync(), "Id", "Name");
			ViewBag.BrandId = new SelectList(await _serviceBrand.GetAllAsync(), "Id", "Name");
			return View(model);
		}

		// POST: ProductsController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(int id, Product product, IFormFile? Image, bool? resmiSil)
		{
			if (ModelState.IsValid)
			{
				try
				{
					if (resmiSil == true)
					{
						FileHelper.FileRemover(product.Image, filePath: "/wwwroot/Img/Products/");
						product.Image = string.Empty;
					}
					if (Image is not null)
					{
						FileHelper.FileRemover(product.Image, filePath: "/wwwroot/Img/Products/");
						product.Image = await FileHelper.FileLoaderAsync(Image, filePath: "/wwwroot/Img/Products/");
					}
					_service.Update(product);
					await _service.SaveChangesAsync();
					return RedirectToAction(nameof(Index));
				}
				catch
				{
					ModelState.AddModelError("", "Hata Oluştu!");
				}
			}
			ViewBag.CategoryId = new SelectList(await _serviceCategory.GetAllAsync(), "Id", "Name");
			ViewBag.BrandId = new SelectList(await _serviceBrand.GetAllAsync(), "Id", "Name");

			return View(product);
		}

		// GET: ProductsController/Delete/5
		public async Task<ActionResult> Delete(int id)
		{
			var model = await _service.FindAsync(id);
			return View(model);
		}

		// POST: ProductsController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, Product product)
		{
			try
			{
				FileHelper.FileRemover(product.Image, filePath: "/wwwroot/Img/Products/");
				_service.Delete(product);
				_service.SaveChanges();
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}
	}
}
