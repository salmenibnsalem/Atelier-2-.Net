using Atelier2.Models;
using Atelier2.Models.Repositories;
using Atelier2.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Atelier2.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepository<Product> ProductRepository;
        private readonly IWebHostEnvironment hostingEnvironment;

        public ProductController(IRepository<Product> productRepository, IWebHostEnvironment hostingEnvironment) =>
            (ProductRepository, this.hostingEnvironment) = (productRepository, hostingEnvironment);

        public IActionResult Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            IEnumerable<Product> products = ProductRepository.GetAll();

            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Désignation.Contains(searchString, System.StringComparison.OrdinalIgnoreCase));
            }

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = ProductRepository.Get(id);
            return product != null ? View(product) : NotFound();
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);
                var newProduct = new Product
                {
                    Désignation = model.Désignation,
                    Prix = model.Prix,
                    Quantite = model.Quantite,
                    Image = uniqueFileName
                };
                ProductRepository.Add(newProduct);
                return RedirectToAction("Details", new { id = newProduct.Id });
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var product = ProductRepository.Get(id);
            if (product == null) return NotFound();

            var editViewModel = new EditViewModel
            {
                Id = product.Id,
                Désignation = product.Désignation,
                Prix = product.Prix,
                Quantite = product.Quantite,
                ExistingImagePath = product.Image
            };
            return View(editViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = ProductRepository.Get(model.Id) ?? new Product();
                product.Désignation = model.Désignation;
                product.Prix = model.Prix;
                product.Quantite = model.Quantite;

                if (model.ImagePath != null)
                {
                    if (model.ExistingImagePath != null)
                    {
                        var filePath = Path.Combine(hostingEnvironment.WebRootPath, "images", model.ExistingImagePath);
                        System.IO.File.Delete(filePath);
                    }
                    product.Image = ProcessUploadedFile(model);
                }

                ProductRepository.Update(product);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var product = ProductRepository.Get(id);
            return product != null ? View(product) : NotFound();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            ProductRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private string ProcessUploadedFile(CreateViewModel model)
        {
            string uniqueFileName = string.Empty;
            if (model.ImagePath != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                model.ImagePath.CopyTo(fileStream);
            }
            return uniqueFileName;
        }
    }
}
