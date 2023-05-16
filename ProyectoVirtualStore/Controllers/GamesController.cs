using Microsoft.AspNetCore.Mvc;
using ProyectoNugetVirtualStore.Models;
using ProyectoVirtualStore.Extensions;
using ProyectoVirtualStore.Filters;

using ProyectoVirtualStore.Services;
using System.Security.Claims;

namespace ProyectoVirtualStore.Controllers
{
    public class GamesController : Controller
    {
        
        private ServiceApiTienda service;
        private ServiceStorageBlobs serviceStorageBlobs;




        public GamesController( ServiceApiTienda service, ServiceStorageBlobs serviceStorage) 
        {
            this.serviceStorageBlobs = serviceStorage;
         
            this.service = service;
        }

       

        [AuthorizeUsers]
        public async Task<IActionResult> Index(int idjuego, int? idjuegocarrito)
        {
            if (idjuegocarrito != null) 
            {
                List<int> carrito;
                if (HttpContext.Session.GetObject<List<int>>("CARRITO") == null)
                {
                    carrito = new List<int>();
                }
                else {
                    carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");

                }
                if (carrito.Contains(idjuegocarrito.Value) == false) {
                    carrito.Add(idjuegocarrito.Value);
                    HttpContext.Session.SetObject("CARRITO", carrito);
                
                }

            }
           
            ViewData["IMAGENES"] = await this.serviceStorageBlobs.GetBlobsAsync("juego" + idjuego);
            ViewData["USUARIOS"] = await this.serviceStorageBlobs.GetBlobsPrivatesAsync("usuarios");

            DatosJuego datosJuego = new DatosJuego();
            datosJuego.Juego = await this.service.FindJuegoAsync(idjuego);
            datosJuego.VistaComentarios = await this.service.GetVistaComentariosAsync(idjuego);
            return View(datosJuego);
        }


        [AuthorizeUsers]
        [HttpPost]
        public async Task<IActionResult> Index(int idjuego,string texto)
        {
            // Obtener la fecha y hora actual como DateTime
            DateTime now = DateTime.Now;


            string token =
               HttpContext.Session.GetString("TOKEN");
            Usuario usuario = await this.service.DetailsUsuarioAsync(token);

            await this.service.InsertComentarioAsync(idjuego, usuario.IdUsuario,texto, now);
           

            DatosJuego datosJuego = new DatosJuego();

            ViewData["IMAGENES"] = await this.serviceStorageBlobs.GetBlobsAsync("juego" + idjuego);
            ViewData["USUARIOS"] = await this.serviceStorageBlobs.GetBlobsPrivatesAsync("usuarios");
            datosJuego.Juego = await this.service.FindJuegoAsync(idjuego);
            datosJuego.VistaComentarios = await this.service.GetVistaComentariosAsync(idjuego);

            return View(datosJuego);
        }

        [AuthorizeUsers]
        public async  Task<IActionResult> Carrito(int? idproducto)
        {
            List<int> carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");
            if (carrito == null)
            {
                return View();
            }
            else 
            {
                if (idproducto != null) { 
                    carrito.Remove(idproducto.Value);
                    HttpContext.Session.SetObject("CARRITO", carrito);

                }
                List<Juegos> juegos = await this.service.GetJuegosCarrito(carrito);
                return View(juegos);
            }

        }

        public async Task<IActionResult> Compra() {

            DateTime now = DateTime.Now;


            


            List<int> carrito = HttpContext.Session.GetObject<List<int>>("CARRITO");
            List<Juegos> juegos = await this.service.GetJuegosCarrito(carrito);

            string token =
               HttpContext.Session.GetString("TOKEN");
            await this.service.InsertCompraAsync(juegos,token);
            HttpContext.Session.Remove("CARRITO");
            return RedirectToAction("Index","Home");

        }

        


        

    }
}
