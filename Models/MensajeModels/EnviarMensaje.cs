using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEW_API.Models.MensajeModels
{
    public class EnviarMensaje
    {
        public int idColegio { get; set; }
        public int RemidxMaestro { get; set; }
        public string RemTipoMaestro { get; set; }
        public string RemCedula { get; set; }
        public string RemNombre { get; set; }
        public int RespondeAidxMsg { get; set; }
        public string DesidxMaestro { get; set; }
        public string DesNombre { get; set; }
        public string DesidxMaestroCC { get; set; }
        public string DesNombreCC { get; set; }
        public string Asunto { get; set; }
        public string Contenido { get; set; }
        public byte Urgente { get; set; }
        public string Archivo { get; set; }
        public byte ParaApps { get; set; }
        public string background { get; set; }
        public object DesidxMaestroCCO { get; set; }
        public object DesNombreCCO { get; set; }

        public EnviarMensaje() { }
    }
}