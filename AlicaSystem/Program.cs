var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddScoped<AlicaSystem.Datos.ConexionBD>();
builder.Services.AddScoped<AlicaSystem.Datos.UsuarioDatos>();
builder.Services.AddScoped<AlicaSystem.Datos.CategoriaDatos>();

// Esto prende el sistema de Session, y le decimos cómo comportarse:
// que la sesión expire tras 30 minutos sin actividad
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// Esto activa Session para que ya se pueda USAR dentro de las páginas
// (tiene que ir después de UseRouting y antes de UseAuthorization)
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();