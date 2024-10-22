using PersonalWebsite.Data;
using PersonalWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

public class DemoController : Controller
{
    private readonly ApplicationDbContext _context;

    // Constructor
    public DemoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Display the main page including the detailed information of this project
    public IActionResult Index()
    {
        return View("Index");
    }

    // Display all products on the Amazon page
    public IActionResult Amazon()
    {
        // Search for all available products in database
        var products = _context.Products.ToList();

        var viewModel = new DemoViewModel
        {
            Products = products,
        };

        return View("Amazon", viewModel);
    }

    // Send the data (search results and elapsed time) back to the view
    public IActionResult SearchCartByUser(object cartItems = null, string elapsedTime = null)
    {
        // Save the elapsed time in the ViewBag
        ViewBag.ElapsedTime = elapsedTime;

        // If the search results are not empty...
        if (cartItems != null)
        {
            var itemsList = cartItems as IEnumerable<dynamic>;
            return View(itemsList);
        }

        return View();
    }

    public async Task<IActionResult> AddToCart(int userId, int productId, int quantity)
    {
        // Search for the cart item by user ID and product ID
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

        if (cartItem == null)
        {
            cartItem = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity
            };

            _context.CartItems.Add(cartItem);
        }
        else
        {
            cartItem.Quantity += quantity;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Amazon");
    }

    // Search products by a given name
    public async Task<IActionResult> GetAllProduct(string ProductName)
    {
        var products = new List<Product>();

        // If the product name is empty, retrieve all products
        if (ProductName == null)
        {
            products = await _context.Products.ToListAsync();
        }
        else
        {
            products = await _context.Products
                .Where(p => p.Name.ToLower().Contains(ProductName.ToLower()))
                .ToListAsync();

        }

        var viewModel = new DemoViewModel
        {
            Products = products,
        };

        Console.WriteLine("Product: " + products);

        return View("Amazon", viewModel);
    }


    public async Task<IActionResult> GetCartItemsNPlusOne(int userId)
    {
        var stopwatch = Stopwatch.StartNew();  // Start!

        var user = await _context.Users
            .Include(u => u.CartItems)
            .FirstOrDefaultAsync(u => u.Id == userId);

        var cartItems = user.CartItems.Select(ci => new CartItemViewModel
                {
                ProductId = ci.ProductId,
                ProductName = _context.Products.FirstOrDefault(p => p.Id == ci.ProductId).Name,
                Quantity = ci.Quantity
                }).ToList();

        stopwatch.Stop();  // Stop!
        var elapsedTime = stopwatch.ElapsedMilliseconds;

        TempData["ElapsedTimeNPlusOne"] = elapsedTime.ToString();

        var viewModel = new DemoViewModel
        {
            CartItems = cartItems
        };

        return View("SearchCartByUser", viewModel);
    }

    public async Task<IActionResult> GetCartItemsOptimized(int userId)
    {
        var stopwatch = Stopwatch.StartNew();  // Start!

        var cartItems = await _context.CartItems
            .Where(ci => ci.UserId == userId)
            .Include(ci => ci.Product)
            .Select(ci => new CartItemViewModel
                    {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Quantity = ci.Quantity
                    }).ToListAsync();

        stopwatch.Stop();  // Stop!
        var elapsedTime = stopwatch.ElapsedMilliseconds;

        TempData["ElapsedTimeOptimized"] = elapsedTime.ToString();

        var viewModel = new DemoViewModel
        {
            CartItems = cartItems
        };

        return View("SearchCartByUser", viewModel);
    }
}

