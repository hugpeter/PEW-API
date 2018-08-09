using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEW_API.Models.MensajeModels
{
    public class Responder
    {
        public int IdxMensaje { get; set; }
        public int IdColegio { get; set; }
        public string Cedula { get; set; }
        public string Contenido { get; set; }
        public string Asunto { get; set; }
        public int Companeros { get; set; }

        public Responder() { }
    }
}