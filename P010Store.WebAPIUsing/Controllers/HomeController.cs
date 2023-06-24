﻿using Microsoft.AspNetCore.Mvc;
using P010Store.Entities;
using P010Store.WebAPIUsing.Models;
using System.Diagnostics;

namespace P010Store.WebAPIUsing.Controllers
{
	public class HomeController : Controller
	{
        private readonly HttpClient _httpClient;
        private readonly string _apiAdres = "https://localhost:7212/Api/";

        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomePageViewModel()
            {
                Carousels = await _httpClient.GetFromJsonAsync<List<Carousel>>(_apiAdres + "Carousel"),
                Products = await _httpClient.GetFromJsonAsync<List<Product>>(_apiAdres + "Products"),
                Brands = await _httpClient.GetFromJsonAsync<List<Brand>>(_apiAdres + "Brands")
            };
            return View(model);
        }
        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [Route("Iletisim")]
        public IActionResult ContactUs()
        {
            return View();
        }
        [Route("Iletisim"), HttpPost]
        public async Task<IActionResult> ContactUs(Contact contact)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync(_apiAdres + "Contacts", contact);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Mesaj"] = "<div class = 'alert alert-success'>Mesajınız Gönderildi. Teşekkürler...</div>";
                        return RedirectToAction("ContactUs");
                    }
                }
                catch (Exception)
                {

                    ModelState.AddModelError("", "Hata Oluştu! Mesajınız Gönderilemedi!");
                }

            }
            return View();
        }
        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}