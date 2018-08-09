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
using PEW_API.Models.MensajeModels;

namespace PEW_API.Controllers
{
    [JwtAuthentication]
    public class MensajeController : ApiController
    {
        PEW_Utils util = new PEW_Utils();
        string connectionString = "Server=192.168.0.31\\SQLEXPRESS2012;Database=MrbConfig;User Id=bios;Password=bios_123;";
        string instance = "[BS-DATOS\\SQLEXPRESS2012]";

        // GET: api/inbox
        [HttpGet]
        [Route("api/inbox")]
        public HttpResponseMessage GetInbox(int idColegio, string cedula)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_Msnaje_buscar_bandeja_entrada";

            SqlCommand cmd = new SqlCommand();
            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idColegio;
            cmd.Parameters.Add("@cedula", SqlDbType.VarChar, 500).Value = cedula;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // GET: api/sent
        [HttpGet]
        [Route("api/sent")]
        public HttpResponseMessage GetSent(int idColegio, string cedula)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_Msnaje_buscar_bandeja_enviados";

            SqlCommand cmd = new SqlCommand();
            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idColegio;
            cmd.Parameters.Add("@cedula", SqlDbType.VarChar, 500).Value = cedula;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // GET: api/deleted
        [HttpGet]
        [Route("api/deleted")]
        public HttpResponseMessage GetDeleted(int idColegio, string cedula)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_Msnaje_buscar_bandeja_eliminados";

            SqlCommand cmd = new SqlCommand();
            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idColegio;
            cmd.Parameters.Add("@cedula", SqlDbType.VarChar, 500).Value = cedula;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // this call gives the total number of new messages, used for home screen.
        // GET: api/newMessageCount
        [HttpGet]
        [Route("api/newMessageCount")]
        public HttpResponseMessage GetNewMessageCount(int idColegio, string cedula)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_Msnaje_buscar_cantmens_nuevos";

            SqlCommand cmd = new SqlCommand();
            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idColegio;
            cmd.Parameters.Add("@cedula", SqlDbType.VarChar, 500).Value = cedula;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        //mark message as read
        // GET: api/mensajeLeido
        [HttpGet]
        [Route("api/mensajeLeido")]
        public HttpResponseMessage MensajeMarkedRead(int idMensaje, int idxMaestro, string tipoMaestro)
        {
            string storedProcedure = instance + ".[mrbMensaje].[dbo].Dtario_ActionU_Leido";

            SqlCommand cmd = new SqlCommand();

            //set inputs
            cmd.Parameters.Add("@idxMensaje", SqlDbType.Int).Value = idMensaje;
            cmd.Parameters.Add("@DesidxMaestro", SqlDbType.Int).Value = idxMaestro;
            cmd.Parameters.Add("@DesTipoMaestro", SqlDbType.Char).Value = tipoMaestro;
            cmd.Parameters.Add("@Leido", SqlDbType.Bit).Value = 1;

            cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@msg", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

            string JSON = util.RunStoredProcedureOutputs(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        //get a specific messages info
        // GET: api/mensaje
        [HttpGet]
        [Route("api/mensaje")]
        public HttpResponseMessage GetMensaje(int idMensaje)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_mnsaje_BuscarPK";

            SqlCommand cmd = new SqlCommand();

            //set inputs
            cmd.Parameters.Add("@idxMensaje", SqlDbType.Int).Value = idMensaje;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        //send message
        // POST: api/enviarMensaje
        [HttpPost]
        [Route("api/enviarMensaje")]
        public HttpResponseMessage PostEnviarMensaje(EnviarMensaje mensaje)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].app_mens_ActionI_new";

            SqlCommand cmd = new SqlCommand();

            //set inputs
            cmd.Parameters.Add("@idColegio", SqlDbType.Int).Value = mensaje.idColegio;

            cmd.Parameters.Add("@RemidxMaestro", SqlDbType.Int).Value = mensaje.RemidxMaestro;
            cmd.Parameters.Add("@RemTipoMaestro", SqlDbType.VarChar, 1).Value = mensaje.RemTipoMaestro;
            cmd.Parameters.Add("@RemCedula", SqlDbType.VarChar, 50).Value = mensaje.RemCedula;
            cmd.Parameters.Add("@RemNombre", SqlDbType.VarChar, 100).Value = mensaje.RemNombre;

            cmd.Parameters.Add("@RespondeAidxMsg", SqlDbType.Int).Value = mensaje.RespondeAidxMsg;

            cmd.Parameters.Add("@DesidxMaestro", SqlDbType.VarChar, 5000).Value = mensaje.DesidxMaestro;
            cmd.Parameters.Add("@DesNombre", SqlDbType.VarChar, 5000).Value = mensaje.DesNombre;
            cmd.Parameters.Add("@DesidxMaestroCC", SqlDbType.VarChar, 5000).Value = mensaje.DesidxMaestroCC;
            cmd.Parameters.Add("@DesNombreCC", SqlDbType.VarChar, 5000).Value = mensaje.DesNombreCC;

            cmd.Parameters.Add("@DesidxMaestroCCO", SqlDbType.VarChar, 5000).Value = mensaje.DesidxMaestroCCO;
            cmd.Parameters.Add("@DesNombreCCO", SqlDbType.VarChar, 5000).Value = mensaje.DesNombreCCO;

            cmd.Parameters.Add("@Asunto", SqlDbType.VarChar, 100).Value = mensaje.Asunto;
            cmd.Parameters.Add("@Contenido", SqlDbType.Text).Value = mensaje.Contenido;
            cmd.Parameters.Add("@Urgente", SqlDbType.Bit).Value = mensaje.Urgente;
            cmd.Parameters.Add("@Archivo", SqlDbType.VarChar, 200).Value = mensaje.Archivo;
            cmd.Parameters.Add("@ParaApps", SqlDbType.Bit).Value = mensaje.ParaApps;

            cmd.Parameters.Add("@importante", SqlDbType.Bit).Value = 1;
            cmd.Parameters.Add("@adjunto", SqlDbType.Bit).Value = 0;
            cmd.Parameters.Add("@background", SqlDbType.VarChar, 2000).Value = mensaje.background;


            //set outputs
            cmd.Parameters.Add("@idxMensaje", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@flag", SqlDbType.Bit).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@msg", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

            string JSON = util.RunStoredProcedureOutputs(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // reply to a message
        // POST: api/responder
        [HttpPost]
        [Route("api/responder")]
        public HttpResponseMessage PostResponder(Responder responder)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_Mnsaje_Responder";

            SqlCommand cmd = new SqlCommand();

            //set inputs
            cmd.Parameters.Add("@idxMensaje", SqlDbType.Int).Value = responder.IdxMensaje;
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = responder.IdColegio;
            cmd.Parameters.Add("@cedula", SqlDbType.VarChar, 50).Value = responder.Cedula;
            cmd.Parameters.Add("@Contenido", SqlDbType.Text).Value = responder.Contenido;
            cmd.Parameters.Add("@Asunto", SqlDbType.VarChar, 200).Value = responder.Asunto;
            cmd.Parameters.Add("@Companeros", SqlDbType.Bit).Value = responder.Companeros;

            //set outputs
            cmd.Parameters.Add("@Flag", SqlDbType.Bit).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Msg", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

            string JSON = util.RunStoredProcedureOutputs(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        //delete message from inbox
        // POST: api/eliminarMensajeEntrada
        [HttpPost]
        [Route("api/eliminarMensajeEntrada")]
        public HttpResponseMessage PostEliminarMensajeEntrada(EliminarMensajeEntrada inputs)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_Mnsaje_Eliminar";

            SqlCommand cmd = new SqlCommand();

            //set inputs
            cmd.Parameters.Add("@idxMensaje", SqlDbType.Int).Value = inputs.IdxMensaje;
            cmd.Parameters.Add("@idxMaestro", SqlDbType.Int).Value = inputs.IdxMaestro;

            //set outputs
            cmd.Parameters.Add("@Flag", SqlDbType.Bit).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Msg", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

            string JSON = util.RunStoredProcedureOutputs(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        //delete message from sent
        // POST: api/eliminarMensajeSalida
        [HttpPost]
        [Route("api/eliminarMensajeSalida")]
        public HttpResponseMessage PostEliminarMensajeSalida(EliminarMensajeSalida inputs)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_Mnsaje_Eliminar_enviados";

            SqlCommand cmd = new SqlCommand();

            //set inputs
            cmd.Parameters.Add("@idxMensaje", SqlDbType.Int).Value = inputs.IdxMensaje;
            cmd.Parameters.Add("@idxMaestro", SqlDbType.Int).Value = inputs.IdxMaestro;

            //set outputs
            cmd.Parameters.Add("@Flag", SqlDbType.Bit).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@Msg", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

            string JSON = util.RunStoredProcedureOutputs(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // GET: api/contactGroups
        [HttpGet]
        [Route("api/contactGroups")]
        public HttpResponseMessage GetContactGroups(int idColegio, string tipoMaestro)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_Msnaje_buscar_Grupo_Contactos";

            SqlCommand cmd = new SqlCommand();

            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idColegio;
            cmd.Parameters.Add("@TipoMaestro", SqlDbType.VarChar, 1).Value = tipoMaestro;       

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // GET: api/allContacts
        [HttpGet]
        [Route("api/allContacts")]
        public HttpResponseMessage GetAllContacts(int idcolegio, int idxMaestro, string tipoMaestro)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].app_msg_allcontacts";

            SqlCommand cmd = new SqlCommand();

            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idcolegio;
            cmd.Parameters.Add("@idxMaestro", SqlDbType.Int).Value = idxMaestro; 
            cmd.Parameters.Add("@tipoMaestro", SqlDbType.VarChar, 3).Value = tipoMaestro;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        //get list of pre made groups
        // GET: api/contactGroups
        [HttpGet]
        [Route("api/contactGroups")]
        public HttpResponseMessage GetContactGroups(int idcolegio, int ano, int userId, string tipoMaestro)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].app_MsgFindFK_Grupos";

            SqlCommand cmd = new SqlCommand();

            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idcolegio;
            cmd.Parameters.Add("@ano", SqlDbType.Int).Value = ano;
            cmd.Parameters.Add("@idxMaestro", SqlDbType.Int).Value = userId;
            cmd.Parameters.Add("@tipomaestro", SqlDbType.VarChar, 2).Value = tipoMaestro;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        //get list of contacts in a specific pre made group
        // GET: api/contactGroup
        [HttpGet]
        [Route("api/contactGroup")]
        public HttpResponseMessage GetContactGroup(int idColegio, int groupId)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_Msg_Lista_grupo";

            SqlCommand cmd = new SqlCommand();

            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = idColegio;
            cmd.Parameters.Add("@idxGrupoxplan", SqlDbType.Int).Value = groupId;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // GET: api/customGroups
        [HttpGet]
        [Route("api/customGroups")]
        public HttpResponseMessage GetCustomGroups(int idxMaestro, string tipoMaestro)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].app_msg_ListaM";

            SqlCommand cmd = new SqlCommand();

            //set inputs
            cmd.Parameters.Add("@idxMaestro", SqlDbType.Int).Value = idxMaestro;
            cmd.Parameters.Add("@tipoMaestro", SqlDbType.VarChar, 3).Value = tipoMaestro;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // GET: api/customGroup
        [HttpGet]
        [Route("api/customGroup")]
        public HttpResponseMessage GetCustomGroup(int customGroupID)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].apps_msg_ListaD";

            SqlCommand cmd = new SqlCommand();

            //set inputs
            cmd.Parameters.Add("@idxMsglistaM", SqlDbType.Int).Value = customGroupID;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }

        // POST: api/mensajegrupo
        [HttpPost]
        [Route("api/mensajegrupo")]
        public HttpResponseMessage PostMensajeGrupo(MensajeGrupo inputs)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].app_Msnaje_buscar_contactos_estu";

            SqlCommand cmd = new SqlCommand();

            //set inputs
            cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = inputs.IdColegio;
            cmd.Parameters.Add("@ano", SqlDbType.Int).Value = inputs.Ano;
            cmd.Parameters.Add("@idxremMaestro", SqlDbType.Int).Value = inputs.IdxRemMaestro;
            cmd.Parameters.Add("@remtipoMaestro", SqlDbType.Char, 1).Value = inputs.RemTipoMaestro;
            cmd.Parameters.Add("@codigo", SqlDbType.Int).Value = 1;

            string JSON = util.RunStoredProcedure(cmd, storedProcedure, connectionString);

            var res = Request.CreateResponse(HttpStatusCode.OK);
            res.Content = new StringContent(JSON, System.Text.Encoding.UTF8, "application/json");

            return res;
        }
    }
}
