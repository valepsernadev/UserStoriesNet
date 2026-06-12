using Firmeza.Admin.Data;
using Firmeza.Admin.Models;
using Firmeza.Admin.Services;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Tests;

public class ProductServiceTests
{
  private ApplicationDbContext CreateInMemoryContext()
  {
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
      .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
      .Options;

    return new ApplicationDbContext(options);
  }

  [Fact]
  public async Task GetAllAsync_ShouldNotReturn_SoftDeletedProducts()
  {
    // Arrange
    var context = CreateInMemoryContext();
    var service = new ProductService(context);

    context.Products.AddRange(
      new Product { Name = "Cemento", Price = 25000, Stock = 10 },
      new Product { Name = "Arena", Price = 5000, Stock = 50, DeletedAt = DateTime.UtcNow }
    );
    await context.SaveChangesAsync();

    // Act
    var result = await service.GetAllAsync();

    // Assert
    Assert.Single(result);
    Assert.Equal("Cemento", result[0].Name);
  }

  [Fact]
  public async Task DeleteAsync_ShouldSetDeletedAt_NotRemoveRecord()
  {
    // Arrange
    var context = CreateInMemoryContext();
    var service = new ProductService(context);

    context.Products.Add(new Product { Name = "Ladrillo", Price = 1500, Stock = 100 });
    await context.SaveChangesAsync();

    var product = context.Products.First();

    // Act
    await service.DeleteAsync(product.Id);

    // Assert
    var deleted = await context.Products.FindAsync(product.Id);
    Assert.NotNull(deleted);
    Assert.NotNull(deleted.DeletedAt);
  }
}