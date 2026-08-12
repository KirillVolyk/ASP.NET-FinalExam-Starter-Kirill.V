using DotNetDrinksWebUI.Data;
using DotNetDrinksWebUI.Models;
using Microsoft.EntityFrameworkCore;
using DotNetDrinksWebUI.Controllers;
using DotNetDrinksWebUI.Models;
using DotNetDrinksWebUI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotNetDrinksWebUI.Tests;

[TestClass]
public class ProductsContollerTests
{
    private ProductsController _controller;
    private ApplicationDbContext _context;

    [TestInitialize]
    public void TestIntialize()
    {
        // This method runs before each test method in the class. Use it to set up any common test data or state.
        // Arrange
        // DB setup
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"MockDB_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);

        // At this point the DB is empty, so we can add some mock data to it for testing purposes
        var cat1 = new Category { Id = 1, Name = "Coffee" };
        var cat2 = new Category { Id = 2, Name = "Tea" };

        _context.Categories.AddRange(cat1, cat2); // << Add range is more efficient than adding one by one

        var prod1 = new Product { Id = 1, Name = "Latte", Price = 2, CategoryId = 1 };
        var prod2 = new Product { Id = 2, Name = "Green Tea", Price = 1, CategoryId = 2 };
        var prod3 = new Product { Id = 3, Name = "Earl Grey", Price = 2, CategoryId = 1 };

        _context.Products.AddRange(prod1, prod2, prod3);
        _context.SaveChanges();

        // Controller instance
        _controller = new ProductsController(_context);

    }

    // Sadly I can't figure this one out
    [TestMethod]
    public void DeleteGetOfProductsControllerReturnsViewResultOnValidID()
    {
        // Arrange
        var id = 1;

        // Act
        var result = _controller.Delete(id).Result as ViewResult;
        var model = result?.Model as Product;

        // Assert
        Assert.IsInstanceOfType(result, typeof(ViewResult));
    }

    [TestMethod]
    public void DeleteConfirmedOfProductsControllerFromInMemoryDB()
    {
        // Arrange: we have 3 products in the database, we will delete one of them and check if it is removed from the database
        var p1 = 1;
        var p2 = 2;

        // Act
        var result = _controller.DeleteConfirmed(p1).Result as RedirectToActionResult;

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Index", result.ActionName);

        // Assert
        var deleted = _context.Products.Find(p1);
        // Check if the deleted product is null, meaning it was removed from the database
        Assert.IsNull(deleted);
        // Check if 2 products remain in the database
        Assert.AreEqual(2, _context.Products.Count());
    }

    [TestCleanup]
    public void TestCleanup()
    {
        // Opposite of TestInitialize, this method runs after each test method in the class
        // Use it to clean up any resources or reset state
        _context = null;
    }
}
