using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P010Store.Entities;

namespace P010Store.WebAPIUsing.Areas.Admin.Controllers
{
	[Area("Admin"), Authorize(Policy = "AdminPolicy")]
	public class ContactsController : Controller
	{
		private readonly HttpClient _httpClient;
		private readonly string _apiAdres = "https://localhost:7212/Api/Contacts";

		public ContactsController(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}
		// GET: ContactsController
		public async Task<ActionResult> Index()
		{
			var model = await _httpClient.GetFromJsonAsync<List<Contact>>(_apiAdres);
			return View(model);
		}

		// GET: ContactsController/Details/5
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: ContactsController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: ContactsController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Create(Contact contact)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var response = await _httpClient.PostAsJsonAsync(_apiAdres, contact);
					if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
				}
				catch
				{
					ModelState.AddModelError("", "Hata Oluştu!");
				}
			}

			return View(contact);
		}

		// GET: ContactsController/Edit/5
		public async Task<ActionResult> Edit(int id)
		{
			var model = await _httpClient.GetFromJsonAsync<Contact>(_apiAdres + "/" + id);
			return View(model);
		}

		// POST: ContactsController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(int id, Contact contact)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var response = await _httpClient.PutAsJsonAsync(_apiAdres, contact);
					if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
				}
				catch
				{
					ModelState.AddModelError("", "Hata Oluştu!");
				}
			}

			return View(contact);
		}

		// GET: ContactsController/Delete/5
		public async Task<ActionResult> Delete(int id)
		{
			var model = await _httpClient.GetFromJsonAsync<Contact>(_apiAdres + "/" + id);
			return View(model);
		}

		// POST: ContactsController/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Delete(int id, Contact contact)
		{
			try
			{
				await _httpClient.DeleteAsync(_apiAdres + "/" + id);
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				ModelState.AddModelError("", "Hata Oluştu!");
			}
			return View(contact);
		}
	}
}
