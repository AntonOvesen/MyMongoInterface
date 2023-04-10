using MyMongoInterface.AutoMapper;
using MyMongoInterface.DAL;
using MyMongoInterface.Extensions;
using MyMongoInterface.Persistence;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;
var Services = builder.Services;

Services.AddEndpointsApiExplorer();
Services.AddSwaggerGen();

Services.AddMongoDbContext<StudentContext>("myTestDatabase", Configuration);

Services.AddTransient<IStudentDAL, StudentDAL>();

Services.AddAutoMapper(typeof(MappingProfile));

Services.AddMvc(options =>
{
    options.EnableEndpointRouting = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
