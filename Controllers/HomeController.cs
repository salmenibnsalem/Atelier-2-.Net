using Atelier2.Models;
using Atelier2.Models.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Atelier2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Product> _productRepository;

        public HomeController(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Index()
        {
            // Retrieve all products
            IEnumerable<Product> products = _productRepository.GetAll();
            return View(products);
        }
    }
}
