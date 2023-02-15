using catalogapi.Context;
using catalogapi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefautConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

//endpoints (categories)
app.MapPost("/categories", async (Category category, AppDbContext db) =>
{
    db.Categories.Add(category);
    await db.SaveChangesAsync();

    return Results.Created($"/categories/{category.CategoryId}", category);
});

app.MapGet("/categories", async (AppDbContext db) => await db.Categories.ToListAsync());

app.MapGet("/categories /{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Categories.FindAsync(id)
    is Category category  ? Results.Ok(category) : Results.NotFound();
});

app.MapPut("/categories/{id:int}", async (int id, Category category, AppDbContext db) =>
{
    if (category.CategoryId != id)
    {
        return Results.BadRequest();
    }

    var categoryDB = await db.Categories.FindAsync(id);

    if (categoryDB is null) return Results.NotFound();

    categoryDB.Name = category.Name;
    categoryDB.Description = category.Description;

    await db.SaveChangesAsync();
    return Results.Ok(categoryDB);
});

app.MapDelete("/categories/{id:int}", async (int id, AppDbContext db) =>
{
    var category = await db.Categories.FindAsync(id);
    if (category is null) return Results.NotFound();

    db.Categories.Remove(category);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// enpoint (produto)
app.MapPost("/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();

    return Results.Created($"/products/{product.ProductId}", product);
}).Produces<Product>(StatusCodes.Status201Created)
.WithName("CriarNovoProduto")
.WithTags("Products");

app.MapGet("/products", async (AppDbContext db) =>
await db.Products.ToListAsync())
    .Produces<List<Product>>(StatusCodes.Status200OK)
    .WithTags("Products");

app.MapGet("/products /{id:int}", async (int id, AppDbContext db) =>
{
    return await db.Products.FindAsync(id)
    is Product product ? Results.Ok(product) : Results.NotFound("Produto não encontrado");
    
}).Produces<Product>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithTags("Products");

app.MapPut("/products", async (int productId, string productName ,AppDbContext db) =>
{
    var productDB = db.Products.SingleOrDefault(s => s.ProductId == productId);

    if (productDB == null) return Results.NotFound();

    productDB.Name = productName;


    await db.SaveChangesAsync();
    return Results.Ok(productDB);
}).Produces<Product>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("AtualizandoNomeProduto")
.WithTags("Products");

app.MapPut("/products/{id:int}", async (int id, Product product, AppDbContext db) =>
{
    if (product.ProductId != id)
    {
        return Results.BadRequest("Id não confere");
    }

    var productDB = await db.Products.FindAsync(id);

    if (productDB is null) return Results.NotFound("Produto não encontrado");

    productDB.Name = product.Name;
    productDB.Description = product.Description;
    productDB.DatePurchase = product.DatePurchase;
    productDB.Price = product.Price;
    productDB.stock = product.stock;
    productDB.CategoryId = product.CategoryId;

    await db.SaveChangesAsync();
    return Results.Ok(productDB);
}).Produces<Product>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status404NotFound)
.WithName("AtualizaProduto")
.WithTags("Products");

//app.MapDelete("/products/{id:int}", async (int id, AppDbContext db) =>
//{
//    var product = await db.Products.FindAsync(id);
//    if (product is null) return Results.NotFound();

//    db.Products.Remove(product);
//    await db.SaveChangesAsync();

//    return Results.NoContent();
//});




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.Run();


