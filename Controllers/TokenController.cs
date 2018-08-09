using System.Net;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using PEW_API.Util;
using PEW_API.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System;

namespace PEW_API.Controllers
{
    public class TokenController : ApiController
    {
        PEW_Utils util = new PEW_Utils();
        string connectionString = "Server=192.168.0.31\\SQLEXPRESS2012;Database=MrbConfig;User Id=bios;Password=bios_123;";
        string instance = "[BS-DATOS\\SQLEXPRESS2012]";

        [AllowAnonymous]
        [Route("api/auth")]
        [HttpGet]
        public HttpResponseMessage Get(string username, string password)
        {
            Session session = CheckUser(username, password);

            if (session.IsValid())
            {
                session.Token = JwtManager.GenerateToken(session);

                //convert data to json
                string content = JsonConvert.SerializeObject(session, Formatting.Indented);

                //create an http response
                var res = Request.CreateResponse(HttpStatusCode.OK);

                //set the content of the response to be JSON format
                res.Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json");

                return res;
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        public Session CheckUser(string username, string password)
        {
            Session session = new Session();

            GetUserStartData(session, username, password);

            //buscar por el nombre de colegio
            GetNombreColegio(session);

            //check to see if user is a family member
            if (session.Student.IdxMaestro == 0)
            {
                GetFamiliaData(session);
                GetContactos(session);

                session.IsFamilia = true;
            }

            return session;
        }

        private Session GetContactos(Session session)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].app_msg_allcontacts";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                //set inputs
                cmd.Parameters.Add("@idColegio", SqlDbType.Int).Value = session.Student.IdColegio;
                cmd.Parameters.Add("@idxMaestro", SqlDbType.Int).Value = session.Student.IdxMaestro;
                cmd.Parameters.Add("@tipoMaestro", SqlDbType.VarChar, 3).Value = session.Student.TipoMaestro;

                //  Stored procedure name
                cmd.CommandText = storedProcedure;
                // set it to stored proc
                cmd.CommandType = CommandType.StoredProcedure;
                // add connection
                cmd.Connection = connection;

                try
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter
                    {
                        SelectCommand = cmd
                    };

                    DataTable contactos = new DataTable();
                    adapter.Fill(contactos);

                    foreach (DataRow row in contactos.Rows)
                    {
                        Contacto contacto = new Contacto
                        {
                            CodigoContacto = row["codigoContacto"].ToString(),
                            NombreContacto = row["nombreContacto"].ToString(),
                            GrupoContacto = row["grupoContacto"].ToString(),
                            IdGrupo = Convert.ToInt32(row["idgrupo"]),
                            Cargo = row["cargo"].ToString()
                        };

                        session.Contactos.Add(contacto);
                    }

                    return session;
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
        }

        private Session GetFamiliaData(Session session)
        {
            string storedProcedure = instance + ".[MrbApps].[dbo].App_Buscar_Estudiantes_Codfamilia";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                //set inputs
                cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Value = session.Student.IdColegio;
                cmd.Parameters.Add("@codfamilia", SqlDbType.VarChar, 100).Value = session.Student.Cedula;
                //set outputs
                cmd.Parameters.Add("@cantidad", SqlDbType.Int).Direction = ParameterDirection.Output;

                //  Stored procedure name
                cmd.CommandText = storedProcedure;
                // set it to stored proc
                cmd.CommandType = CommandType.StoredProcedure;
                // add connection
                cmd.Connection = connection;

                try
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter
                    {
                        SelectCommand = cmd
                    };
              
                    DataTable dt = new DataTable();

                    adapter.Fill(dt);

                    if(dt.Rows.Count > 0)
                    {
                        for(int i=0; i<dt.Rows.Count; i++)
                        {
                            Familia member = new Familia();

                            member.NombreCompleto = string.IsNullOrEmpty(dt.Rows[i]["NombreCompleto"].ToString()) ? string.Empty : dt.Rows[i]["NombreCompleto"].ToString();
                            member.Cedula = string.IsNullOrEmpty(dt.Rows[i]["Cedula"].ToString()) ? string.Empty : dt.Rows[i]["Cedula"].ToString();
                            member.IdxEstudiante = string.IsNullOrEmpty(dt.Rows[i]["idxEstudiante"].ToString()) ? 0 : Convert.ToInt32(dt.Rows[i]["idxEstudiante"]);

                            member.Nombre1 = string.IsNullOrEmpty(dt.Rows[i]["nombre1"].ToString()) ? string.Empty : dt.Rows[i]["nombre1"].ToString();
                            member.Nombre2 = string.IsNullOrEmpty(dt.Rows[i]["nombre2"].ToString()) ? string.Empty : dt.Rows[i]["nombre2"].ToString();
                            member.Apellido1 = string.IsNullOrEmpty(dt.Rows[i]["apellido1"].ToString()) ? string.Empty : dt.Rows[i]["apellido1"].ToString();
                            member.Apellido2 = string.IsNullOrEmpty(dt.Rows[i]["apellido2"].ToString()) ? string.Empty : dt.Rows[i]["apellido2"].ToString();

                            session.FamilyOptions.Add(member.Nombre1 + ' ' + member.Nombre2 + ' ' + member.Apellido1 + ' ' + member.Apellido2);
                            session.FamilyMembers.Add(member);
                        }
                        
                    } 
                    else
                    {
                        Familia member = new Familia();

                        member.NombreCompleto = "not found";
                        member.Cedula = "not found";
                        member.IdxEstudiante = 0;

                        session.FamilyOptions.Add("no options");
                        session.FamilyMembers.Add(member);
                    }

                    return session;
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
        }

        private Session GetUserStartData(Session session, string username, string password)
        {
            string storedProcedure = instance + ".[MrbConfig].[dbo].[App_Usuario_FindFK_Login]";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                //Set Inputs
                cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = username;
                cmd.Parameters.Add("@Password", SqlDbType.VarChar, 50).Value = password;
                //Set Outputs
                cmd.Parameters.Add("@Cedula", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@tipoMaestro", SqlDbType.Char, 1).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Ano", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Periodo", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@idcolegio", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@bloqueado", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@aviso", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@periodo_pre", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@nivel", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@GrupoUsuario", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@cambiarpass", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Idioma", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@idxMaestro", SqlDbType.Int).Direction = ParameterDirection.Output;
                //  Stored procedure name
                cmd.CommandText = storedProcedure;
                // set it to stored proc
                cmd.CommandType = CommandType.StoredProcedure;
                // add connection
                cmd.Connection = connection;

                SqlDataReader reader = null;

                try
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    reader = cmd.ExecuteReader();

                    session.Usuario = cmd.Parameters["@Usuario"].Value.ToString();
                    session.Password = cmd.Parameters["@Password"].Value.ToString();
                    session.Student.Cedula = cmd.Parameters["@Cedula"].Value.ToString();
                    session.Student.Nombre = cmd.Parameters["@Nombre"].Value.ToString();
                    session.Student.TipoMaestro = cmd.Parameters["@tipoMaestro"].Value.ToString();
                    session.Student.Ano = string.IsNullOrEmpty(cmd.Parameters["@Ano"].Value.ToString()) ? 0 : Convert.ToInt32(cmd.Parameters["@Ano"].Value);
                    session.Student.Periodo = string.IsNullOrEmpty(cmd.Parameters["@Periodo"].Value.ToString()) ? 0 : Convert.ToInt32(cmd.Parameters["@Periodo"].Value);
                    session.Student.IdColegio = string.IsNullOrEmpty(cmd.Parameters["@idcolegio"].Value.ToString()) ? 0 : Convert.ToInt32(cmd.Parameters["@idcolegio"].Value);
                    session.Student.Bloqueado = string.IsNullOrEmpty(cmd.Parameters["@bloqueado"].Value.ToString()) ? 0 : Convert.ToInt32(cmd.Parameters["@bloqueado"].Value);
                    session.Student.Aviso = cmd.Parameters["@aviso"].Value.ToString();
                    session.Student.Periodo_pre = string.IsNullOrEmpty(cmd.Parameters["@periodo_pre"].Value.ToString()) ? 0 : Convert.ToInt32(cmd.Parameters["@periodo_pre"].Value);
                    session.Student.Nivel = string.IsNullOrEmpty(cmd.Parameters["@nivel"].Value.ToString()) ? 0 : Convert.ToInt32(cmd.Parameters["@nivel"].Value);
                    session.Student.GrupoUsuario = string.IsNullOrEmpty(cmd.Parameters["@GrupoUsuario"].Value.ToString()) ? 0 : Convert.ToInt32(cmd.Parameters["@GrupoUsuario"].Value);
                    session.Student.Cambiarpass = string.IsNullOrEmpty(cmd.Parameters["@cambiarpass"].Value.ToString()) ? byte.MinValue : Convert.ToByte(cmd.Parameters["@cambiarpass"].Value);
                    session.Student.Idioma = string.IsNullOrEmpty(cmd.Parameters["@Idioma"].Value.ToString()) ? 0 : Convert.ToInt32(cmd.Parameters["@Idioma"].Value);
                    session.Student.IdxMaestro = string.IsNullOrEmpty(cmd.Parameters["@idxMaestro"].Value.ToString()) ? 0 : Convert.ToInt32(cmd.Parameters["@idxMaestro"].Value);
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

                return session;
            }
        }

        public Session GetNombreColegio(Session session)
        {
            string queryString = "SELECT Nombre FROM " + instance + ".[MrbConfig].[dbo].Colegio where idColegio = " + session.Student.IdColegio;

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
                            session.Student.Colegio = string.IsNullOrEmpty(reader.GetString(0)) ? " " : reader.GetString(0);
                        }

                        if(!reader.Read())
                        {
                            session.Student.Colegio = string.Empty;
                        }
                    }
                }
                catch(Exception e)
                {
                    throw (e);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return session;
        }
    }
}
