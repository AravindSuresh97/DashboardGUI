//using gRPC_Server.Services;
using gRPCServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

//OG
//builder.Services.AddDbContext<ProductContext>
//    (x => x.UseInMemoryDatabase("Logistic_GUI"));

// Edited
// builder.Services.AddDbContext<ProductContext>(options =>
//    options.UseSqlServer(Configuration.GetConnectionString("MySqlConnection")));

builder.Services.AddDbContext<ProductContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.MapGrpcService<GreeterService>();
app.MapGrpcService<ProductService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
