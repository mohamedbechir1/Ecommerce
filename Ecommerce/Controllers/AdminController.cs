using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Data;
using Ecommerce.Models;

namespace Proj.Controllers
{
    public class AdminController : Controller
    {
        private readonly EcommerceDbContext _context;

        public AdminController(EcommerceDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/AdminDashboard
        public IActionResult AdminDashboard()
        {
            var totalUsers = _context.Users.Count() - 1; // Exclude admin
            var totalProducts = _context.Products.Count();
            var totalCategories = _context.Categories.Count();

            var products = _context.Products.ToList();
            var categories = _context.Categories.ToList();

            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.Products = products;
            ViewBag.Categories = categories;

            return View();
        }

        // GET: /Admin/ManageProducts
        public IActionResult ManageProducts()
        {
            var products = _context.Products.ToList();
            ViewBag.Products = products;
            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;

            return View();
        }

        // GET: /Admin/AddProduct
        [HttpGet]
        public IActionResult AddProduct()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: /Admin/AddProduct
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product, IFormFile imageFile)
        {
            if (imageFile.Length > 0)
            {
                // Set the image path with the '/images/' prefix
                product.ImagePath = "/images/" + Path.GetFileName(imageFile.FileName);

                var fileName = Path.GetFileName(imageFile.FileName);
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                var filePath = Path.Combine(uploadsFolder, fileName);

                Directory.CreateDirectory(uploadsFolder);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("ManageProducts");
        }

        // POST: /Admin/DeleteProduct
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);

            if (product != null)
            {
                // Delete associated image file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction("ManageProducts");
        }

        // GET: /Admin/EditProduct
        public IActionResult EditProduct(int id)
        {
            var product = _context.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Optional: Ensure ImagePath is not null
            if (product.ImagePath == null)
            {
                product.ImagePath = "default-image.jpg"; // Set a default image if ImagePath is null
            }

            if (product.Category == null)
            {
                product.Category = new Category { Name = "Default Category" }; // Set a default category if null
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(product); // Pass the product to the view
        }


        // POST: /Admin/EditProduct
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product updatedProduct, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    updatedProduct.ImagePath = "/images/" + Path.GetFileName(imageFile.FileName); // Update the image path in the database
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    Directory.CreateDirectory(uploadsFolder);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                }

                _context.Products.Update(updatedProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction("ManageProducts");
            }

            ViewBag.Categories = _context.Categories.ToList();
            return View(updatedProduct);
        }

        // GET: /Admin/ManageCategory
        public IActionResult ManageCategory()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // POST: /Admin/AddCategory
        [HttpPost]
        public IActionResult AddCategory(string name)
        {
            if (ModelState.IsValid)
            {
                var category = new Category { Name = name };
                _context.Categories.Add(category);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Category added successfully!";
                return RedirectToAction("ManageCategory");
            }

            TempData["ErrorMessage"] = "There was an error adding the category.";
            return RedirectToAction("ManageCategory");
        }

        // POST: /Admin/EditCategory
        [HttpPost]
        public IActionResult EditCategory(int id, string name)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction("ManageCategory");
            }

            category.Name = name;
            _context.Categories.Update(category);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Category updated successfully!";
            return RedirectToAction("ManageCategory");
        }

        // POST: /Admin/DeleteCategory
        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found!";
                return RedirectToAction("ManageCategory");
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Category deleted successfully!";
            return RedirectToAction("ManageCategory");
        }
        // ------------------- MANAGE USERS -------------------

        // GET: /Admin/ManageUsers
        public IActionResult ManageUsers()
        {
            var users = _context.Users.Where(u => u.Role != "Admin").ToList(); // Exclude admin
            ViewBag.Users = users;

            return View();
        }

        // POST: /Admin/DeleteUser
        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "User deleted successfully!";
            return RedirectToAction("ManageUsers");
        }
        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            // Optional: Provide a default role if the user doesn't have one
            if (user.Role == null)
            {
                user.Role = "User"; // Set a default role
            }

            ViewBag.Roles = new List<string> { "Admin", "User" };
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(User updatedUser)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.Find(updatedUser.Id);
                if (user == null)
                    return NotFound();

                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                user.Role = updatedUser.Role;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction("ManageUsers");
            }

            ViewBag.Roles = new List<string> { "Admin", "User" };
            return View(updatedUser);
        }

    }
}
