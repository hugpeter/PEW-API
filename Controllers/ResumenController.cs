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
    public class ResumenController : ApiController
    {
        PEW_Utils util = new PEW_Utils();
        string connectionString = "Server=192.168.0.31\\SQLEXPRESS2012;Database=MrbConfig;User Id=bios;Password=bios_123;";
        string instance = "[BS-DATOS\\SQLEXPRESS2012]";

        // GET: api/resumen
        [HttpGet]
        [Route("api/resumen")]
        public HttpResponseMessage GetResumen(
            int idioma,
            int idcolegio,
            int ano,
            int periodo,
            int idxmaestro,
            string usuario,
            string password
        )
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_Eiante_resumen_ejecutivo";

            SqlCommand cmd = new SqlCommand();
            //set inputs
            cmd.Parameters.Add("@idioma", SqlDbType.Int).Value = idioma;
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idcolegio;
            cmd.Parameters.Add("@ano", SqlDbType.Int).Value = ano;
            cmd.Parameters.Add("@periodo", SqlDbType.Int).Value = periodo;
            cmd.Parameters.Add("@idxmaestro", SqlDbType.Int).Value = idxmaestro;
            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = usuario;
            cmd.Parameters.Add("@Password", SqlDbType.VarChar, 50).Value = password;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);
            
            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // GET: api/resumenReprobada
        [HttpGet]
        [Route("api/resumenReprobada")]
        public HttpResponseMessage GetResumenReprobada(int idcolegio, int ano, int periodo, int idxMaestro)
        {
            string storedProcedure = "App_Eiante_Materias_reprobadas_periodo"; //missing the location to this stored procedure???

            SqlCommand cmd = new SqlCommand();
            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idcolegio;
            cmd.Parameters.Add("@ano", SqlDbType.Int).Value = ano;
            cmd.Parameters.Add("@periodo", SqlDbType.Int).Value = periodo;
            cmd.Parameters.Add("@idxEstudiante", SqlDbType.Int).Value = idxMaestro;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // GET: api/resumenAnual
        [HttpGet]
        [Route("api/resumenAnual")]
        public HttpResponseMessage GetResumenAnual(int idcolegio, int ano, int periodo, int idxMaestro)
        {
            string storedProcedure = "App_Eiante_Materias_reprobadas_Anual"; //missing the location to this stored procedure???

            SqlCommand cmd = new SqlCommand();
            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idcolegio;
            cmd.Parameters.Add("@ano", SqlDbType.Int).Value = ano;
            cmd.Parameters.Add("@periodo", SqlDbType.Int).Value = periodo;
            cmd.Parameters.Add("@idxEstudiante", SqlDbType.Int).Value = idxMaestro;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }
    }
}
