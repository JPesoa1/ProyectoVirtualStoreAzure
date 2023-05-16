using Microsoft.AspNetCore.Mvc;
using ProyectoNugetVirtualStore.Models;
using ProyectoVirtualStore.Filters;


using ProyectoVirtualStore.Services;
using System.Diagnostics;

namespace ProyectoVirtualStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ServiceApiTienda services;
       

        public HomeController(ILogger<HomeController> logger, ServiceApiTienda services)
        {
            _logger = logger;
            this.services = services;
         
        }

       



        [AuthorizeUsers]
        public async Task<IActionResult> Index()
        {
            DatosJuegosEstados estados = new DatosJuegosEstados();


            estados.juegosPopular = await this.services.GetJuegosEstadosAsync("popular");
            estados.juegosTendencia = await this.services.GetJuegosEstadosAsync("tendencia");
            estados.juegosEstablecido = await this.services.GetJuegosEstadosAsync("establecido");


            

           
            return View(estados);
        }


        public async Task<IActionResult> Filtros(int? posicion ,string categoria , Decimal precio) 
        {
            if (posicion == null)
            {
                posicion = 1;
                ViewData["CATEGORIAS"] = await this.services.GetCategoriasAsync();
                List<Juegos> juegos = await this.services.GetJuegosAsync();
                return View(juegos);

            }
            else {

                ModelPaginarJuegos model = await this.services.GetJuegosFiltrosAsync(posicion.Value, precio, categoria);
                List<Juegos> juegos = model.Juegos;
                int numRegistros = model.NumeroRegistros;
                ViewData["CATEGORIAS"] = await this.services.GetCategoriasAsync();
                ViewData["REGISTROS"] = numRegistros;
                ViewData["CATEGORIA"] = categoria;
                ViewData["PRECIO"] = precio;
                return View(juegos);
            }
            
        }


        [HttpPost]
        public async Task<IActionResult> Filtros(Decimal precio, string categoria )
        {
            ModelPaginarJuegos model = await this.services.GetJuegosFiltrosAsync(1, precio, categoria);
            List<Juegos> juegos = model.Juegos;
            int numRegistros = model.NumeroRegistros;
            ViewData["CATEGORIAS"] = await this.services.GetCategoriasAsync();
            ViewData["REGISTROS"] = numRegistros;
            ViewData["CATEGORIA"] = categoria;
            ViewData["PRECIO"] = precio;
            return View(juegos);
        }


        
        
       
    }
}