using PersonalWebsite.Data;
using PersonalWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

public class DemoController : Controller
{
    private readonly ApplicationDbContext _context;

    public DemoController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult SearchCartByUser(object cartItems = null, string elapsedTime = null)
    {
        ViewBag.ElapsedTime = elapsedTime;

        // 將查詢結果轉換為 List 以便傳遞到視圖
        if (cartItems != null)
        {
            var itemsList = cartItems as IEnumerable<dynamic>;
            return View(itemsList);
        }

        return View();
    }

    public IActionResult Index()
    {
        return View("Index");
    }

    public IActionResult Amazon()
    {
        // Search product by name if ProductName is provided; otherwise, get all products
        var products = _context.Products.ToList();

        var viewModel = new DemoViewModel
        {
            Products = products,
        };

        Console.WriteLine("Product: " + products);

        return View("Amazon", viewModel);
    }

    public async Task<IActionResult> AddToCart(int userId, int productId, int quantity)
    {
        // Log the values being processed
        Console.WriteLine($"Attempting to add to cart - UserId: {userId}, ProductId: {productId}, Quantity: {quantity}");

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

        Console.WriteLine("CartItem: " + cartItem);
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

    public async Task<IActionResult> GetAllProduct(string ProductName)
    {
        // Search product by name if ProductName is provided; otherwise, get all products
        var products = new List<Product>();

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
        var stopwatch = Stopwatch.StartNew();

        var user = await _context.Users
            .Include(u => u.CartItems)
            .FirstOrDefaultAsync(u => u.Id == userId);

        var cartItems = user.CartItems.Select(ci => new CartItemViewModel
                {
                ProductId = ci.ProductId,
                ProductName = _context.Products.FirstOrDefault(p => p.Id == ci.ProductId).Name,
                Quantity = ci.Quantity
                }).ToList();

        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;

        TempData["ElapsedTimeNPlusOne"] = elapsedTime.ToString(); // 轉換為字串

        var viewModel = new DemoViewModel
        {
            CartItems = cartItems
        };

        return View("SearchCartByUser", viewModel);
    }

    public async Task<IActionResult> GetCartItemsOptimized(int userId)
    {
        var stopwatch = Stopwatch.StartNew();

        var cartItems = await _context.CartItems
            .Where(ci => ci.UserId == userId)
            .Include(ci => ci.Product)
            .Select(ci => new CartItemViewModel
                    {
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    Quantity = ci.Quantity
                    }).ToListAsync();

        stopwatch.Stop();
        var elapsedTime = stopwatch.ElapsedMilliseconds;

        TempData["ElapsedTimeOptimized"] = elapsedTime.ToString(); // 轉換為字串

        var viewModel = new DemoViewModel
        {
            CartItems = cartItems
        };

        return View("SearchCartByUser", viewModel);
    }
}

