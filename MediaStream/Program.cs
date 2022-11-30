using MediaStream;
using MediaStream.DbContext;
using MediaStream.Extensions;
using MediaStream.Impl;
using MediaStream.Interfaces;

var builder = WebApplication.CreateBuilder(args);

FFmpegExtensions.DownloadFFmpeg();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton<IDbContextFactory, DbContextFactory>();
builder.Services.AddTransient<IMediaFileRepository, MediaFileRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    //ToDo configure needed
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

var app = builder.Build();

app.Services.GetRequiredService<IDbContextFactory>()
            .CreateContext();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();
