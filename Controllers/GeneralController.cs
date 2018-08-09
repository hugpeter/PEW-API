using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using PEW_API.Util;
using PEW_API.Filters;
using PEW_API.Models.GeneralModels;

namespace PEW_API.Controllers
{
    [JwtAuthentication]
    public class GeneralController : ApiController
    {
        PEW_Utils util = new PEW_Utils();
        string connectionString = "Server=192.168.0.31\\SQLEXPRESS2012;Database=MrbConfig;User Id=bios;Password=bios_123;";
        string instance = "[BS-DATOS\\SQLEXPRESS2012]";

        // GET: api/colegio/idColegio
        [HttpGet]
        [Route("api/colegio/{idColegio}")]
        public string GetColegio(int idColegio)
        {
            return GetNombreColegio(idColegio);
        }

        // POST: api/boletin
        [HttpPost]
        [Route("api/boletin")]
        public HttpResponseMessage PostBoletin(BoletinInputs inputs)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_pa_boletin_N_AT";

            SqlCommand cmd = new SqlCommand();
            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = inputs.IdColegio;
            cmd.Parameters.Add("@ano", SqlDbType.Int).Value = inputs.Ano;
            cmd.Parameters.Add("@bimestre", SqlDbType.Int).Value = inputs.Bimestre;
            cmd.Parameters.Add("@codest", SqlDbType.VarChar, 50).Value = inputs.Cedula;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // GET: api/estado
        [HttpGet]
        [Route("api/estado")]
        public HttpResponseMessage GetEstado(int idColegio, int idioma, string cedula)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_Euenta_BuscarFK_CodEst";

            SqlCommand cmd = new SqlCommand();
            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idColegio;
            cmd.Parameters.Add("@Idioma", SqlDbType.Int).Value = idioma;
            cmd.Parameters.Add("@Codest", SqlDbType.VarChar, 50).Value = cedula;

            //set outputs
            cmd.Parameters.Add("@ParamOutId", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@ParamOutMsg", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // GET: api/notas
        [HttpGet]
        [Route("api/notas")]
        public HttpResponseMessage GetNotas(int ano, int idColegio, int idioma, string cedula)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_Eiante_BuscarMatricula";

            SqlCommand cmd = new SqlCommand();
            //set inputs
            cmd.Parameters.Add("@Ano", SqlDbType.Int).Value = ano;
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idColegio;
            cmd.Parameters.Add("@Idioma", SqlDbType.Int).Value = idioma;
            cmd.Parameters.Add("@Codest", SqlDbType.VarChar, 50).Value = cedula;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // GET: api/notasDetalle
        [HttpGet]
        [Route("api/notasDetalle")]
        public HttpResponseMessage GetNotasDetalle(int ano, int bimestre, int idColegio, int idioma, string cedula, string codmat)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_pa_calif_ficha_x_mate";

            SqlCommand cmd = new SqlCommand();
            //set inputs
            cmd.Parameters.Add("@ano", SqlDbType.Int).Value = ano;
            cmd.Parameters.Add("@bimestre", SqlDbType.Int).Value = bimestre;
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idColegio;
            cmd.Parameters.Add("@Idioma", SqlDbType.Int).Value = idioma;
            cmd.Parameters.Add("@codest", SqlDbType.Char, 13).Value = cedula;
            cmd.Parameters.Add("@codmat", SqlDbType.VarChar, 5).Value = codmat;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // GET: api/cupones
        [HttpGet]
        [Route("api/cupones")]
        public HttpResponseMessage GetCupones()
        {
            List<Cupon> Cupones = new List<Cupon>();

            Cupon digg = new Cupon("http://www.digg.com", "https://bseducativo.com/mrbroot/placeholders/slider/apppromo.jpg");
            Cupon bios = new Cupon("http://www.biossoft.net/", "https://bseducativo.com/mrbroot/placeholders/slider/apppromo2.jpg");

            Cupones.Add(digg);
            Cupones.Add(bios);

            string JSON = JsonConvert.SerializeObject(Cupones, Formatting.Indented);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // GET: api/documents
        [HttpGet]
        [Route("api/documents")]
        public HttpResponseMessage GetContactGroups(int idColegio, string cedula, int userID)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].app_buscar_documentos";

            SqlCommand cmd = new SqlCommand();

            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idColegio;
            cmd.Parameters.Add("@cedula", SqlDbType.VarChar, 50).Value = cedula;
            cmd.Parameters.Add("@idxestudiante", SqlDbType.Int).Value = userID;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        public string GetNombreColegio(int idColegio)
        {
            string queryString = "SELECT Nombre FROM " + instance + ".[MrbConfig].[dbo].Colegio where idColegio = " + idColegio;
            string nombre = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(queryString, connection);

                try
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            nombre = string.IsNullOrEmpty(reader.GetString(0)) ? "" : reader.GetString(0);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw (e);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return nombre;
        }

    }
}
