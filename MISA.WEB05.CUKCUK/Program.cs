using Microsoft.Extensions.FileProviders;
using MISA.WEB05.CORE.Interfaces.Repository;
using MISA.WEB05.CORE.Interfaces.Services;
using MISA.WEB05.CORE.Services;
using MISA.WEB05.INFRASTRUCTURE.Repository;

var builder = WebApplication.CreateBuilder(args);

StaticFileOptions staticFileOptions = new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "UploadFile")),
    RequestPath = "/UploadFile"
};

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(jsonOptions =>
{
    jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

// Add scoped repository
builder.Services.AddScoped<IFoodRepository, FoodRepository>();
builder.Services.AddScoped<IFoodAdditionRepository, FoodAdditionRepository>();
builder.Services.AddScoped<IFoodDetailRepository, FoodDetailRepository>();
builder.Services.AddScoped<IFoodUnitRepository, FoodUnitRepository>();
builder.Services.AddScoped<IFoodCategoryRepository, FoodCategoryRepository>();
builder.Services.AddScoped<IKitchenRepository, KitchenRepository>();

// Add scoped service
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IFoodUnitService, FoodUnitService>();
builder.Services.AddScoped<IFoodDetailService, FoodDetailService>();
builder.Services.AddScoped<IFoodCategoryService, FoodCategoryService>();
builder.Services.AddScoped<IFoodAdditionService, FoodAdditionService>();
builder.Services.AddScoped<IKitchenService, KitchenService>();

builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles(staticFileOptions);

app.UseAuthorization();

app.UseCors(o => o.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader());

app.MapControllers();

app.Run();
