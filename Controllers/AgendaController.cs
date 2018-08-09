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
    public class AgendaController : ApiController
    {
        PEW_Utils util = new PEW_Utils();
        string connectionString = "Server=192.168.0.31\\SQLEXPRESS2012;Database=MrbConfig;User Id=bios;Password=bios_123;";
        string instance = "[BS-DATOS\\SQLEXPRESS2012]";

        // GET: api/agenda
        [HttpGet]
        [Route("api/agenda")]
        public HttpResponseMessage GetAgenda(
            int idcolegio,
            int ano,
            string cedula,
            int bimestre,
            string fechaI,
            string fechaF
        )
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_pa_Agenda_del_estudiante";

            SqlCommand cmd = new SqlCommand();
            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idcolegio;
            cmd.Parameters.Add("@Ano", SqlDbType.Int).Value = ano;
            cmd.Parameters.Add("@CodEst", SqlDbType.VarChar, 50).Value = cedula;
            cmd.Parameters.Add("@Bimestre", SqlDbType.Int).Value = bimestre;
            cmd.Parameters.Add("@FechaI", SqlDbType.VarChar, 10).Value = fechaI;
            cmd.Parameters.Add("@FechaF", SqlDbType.VarChar, 10).Value = fechaF;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // GET: api/agendaDetalle
        [HttpGet]
        [Route("api/agendaDetalle")]
        public HttpResponseMessage GetAgendaDetalle(
            int idColegio,
            int Ano,
            string currentDate,
            string Cedula
        )
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_pa_Agenda_semanal_estu";

            SqlCommand cmd = new SqlCommand();
            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idColegio;
            cmd.Parameters.Add("@Ano", SqlDbType.Int).Value = Ano;
            cmd.Parameters.Add("@Fecha_Dia_Actual", SqlDbType.VarChar, 10).Value = currentDate;
            cmd.Parameters.Add("@Cedula", SqlDbType.VarChar, 50).Value = Cedula;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        public string CreateDateString(int date)
        {
            int year = date / 10000;
            int month = ((date - (10000 * year)) / 100);
            int day = (date - (10000 * year) - (100 * month));

            return year.ToString() + "/" + month.ToString() + "/" + day.ToString();
        }
    }
}
