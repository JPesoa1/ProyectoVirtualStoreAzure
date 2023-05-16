
using Newtonsoft.Json;
using ProyectoNugetVirtualStore.Models;
using ProyectoVirtualStore.Helpers;
using StackExchange.Redis;

namespace ProyectoVirtualStore.Services
{
    public class ServiceCacheRedis
    {

        private IDatabase database;
       

        public ServiceCacheRedis()
        {
            this.database = HelperCacheMultiplexer.GetConnection.GetDatabase();

        }

        public void AddJuegosFavorito(Juegos juegos)

        {

            //CACHE REDIS FUNCIONA CON CLAVES/VALUES PARA  

            //TRABAJAR CON CADA USUARIO 

            //DICHA CLAVE ES PARA TODOS LOS USUARIOS 

            //DEBERIAMOS TENER ALGUN METODO PARA SABER EL USUARIO 

            //POR EJEMPLO, UTILIZANDO UN LOGIN 

            //PARA ALMACENAR UN VALUE, SE UTILIZA STRING EN FORMATO 

            //JSON 

            //LO QUE ALMACENAREMOS SERA UNA COLECCION DE PRODUCTOS 

            //RECUPERAMOS LOS PRODUCTOS SI YA EXISTEN 

            

            string jsonJuegos = this.database.StringGet("favoritos");

            List<Juegos> juegosList;

            if (jsonJuegos == null)
            {
                //NO HEMOS ALMACENADO FAVORITOS, CREAMOS LA COLECCION 
                juegosList = new List<Juegos>();
            }

            else

            {
                //CONVERTIMOS EL JSON DE PRODUCTOS A COLECCION List<> 
                juegosList = JsonConvert.DeserializeObject<List<Juegos>>(jsonJuegos);

            }

            //AÑADIMOS EL NUEVO PRODUCTO A LA COLECCION 

            juegosList.Add(juegos);

            //SERIALIZAMOS LA COLECCION A JSON PARA ALMACENARLA EN CACHE 

            jsonJuegos =

            JsonConvert.SerializeObject(juegosList);

            this.database.StringSet("favoritos", jsonJuegos);

        }

        //METODO PARA RECUPERAR TODOS LOS FAVORITOS 

        public List<Juegos> GetJuegosFavoritos()

        {

            string jsonJuegos = this.database.StringGet("favoritos");

            if (jsonJuegos == null)

            {

                return null;

            }

            else

            {

                List<Juegos> favoritos =

                JsonConvert.DeserializeObject<List<Juegos>>(jsonJuegos);

                return favoritos;

            }

        }



        //METODO PARA ELIMINAR UN PRODUCTO FAVORITO 

        public void DeleteProductoFavorito(int idjuego)

        {

            List<Juegos> favoritos = this.GetJuegosFavoritos();

            if (favoritos != null)

            {

                //BUSCAMOS EL PRODUCTO A ELIMINAR 

                Juegos productoDelete =

                favoritos.FirstOrDefault(z => z.IdJuego == idjuego);

                favoritos.Remove(productoDelete);

                //SI NO TENEMOS FAVORITOS, ELIMINAMOS LA CLAVE DE CACHE REDIS 

                if (favoritos.Count == 0)

                {

                    this.database.KeyDelete("favoritos");

                }

                else

                {

                    string jsonProductos =

                    JsonConvert.SerializeObject(favoritos);

                    //SI NO PONEMOS TIEMPO EN EL MOMENTO DE ALMACENAR 

                    //ELEMENTOS DENTRO DE CACHE REDIS, EL TIEMPO DE  

                    //ALMACENAMIENTO ES DE 24 HORAS 

                    //TAMBIEN PODEMOS INDICARLO DENTRO DEL PORTAL DE AZURE 

                    //PODEMOS INDICAR DE FORMA EXPLICITA EL TIEMPO 

                    //QUE DESEAMOS ALMACENAR LOS DATOS HASTA QUE SON  

                    //ELIMINADOS DE FORMA AUTOMATICA 

                    this.database.StringSet("favoritos", jsonProductos, TimeSpan.FromMinutes(30));

                }

            }

        }


    }
}
