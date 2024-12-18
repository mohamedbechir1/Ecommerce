using Microsoft.AspNetCore.Mvc;
using Ecommerce.Data;
using Ecommerce.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Models.ViewModels;


namespace Ecommerce.Controllers
{
    public class ProductController : Controller
    {
        private readonly EcommerceDbContext _context;
        private const int PageSize = 10;

        public ProductController(EcommerceDbContext context)
        {
            _context = context;
        }

        public IActionResult Catalog(int page = 1, string category = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var categories = _context.Categories.ToList();
		    ViewBag.Categories = categories;  


			var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                productsQuery = productsQuery.Where(p => p.Category.Name == category);
            }

            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            var products = productsQuery.Skip((page - 1) * PageSize).Take(PageSize).ToList();
            var totalProducts = productsQuery.Count();

            var model = new ProductCatalogViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalProducts / PageSize),
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            return View(model);
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products.Include(p => p.Category)
                                           .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
