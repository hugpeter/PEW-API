using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEW_API.Models
{
    public class Claims
    {
        public string User { get; set; }
        public string Pass { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public Int32 Ano { get; set; }
        public Int32 IdColegio { get; set; }
        public Int32 Idioma { get; set; }
        public string Colegio { get; set; }
        public Int32 IdxMaestro { get; set; }
        public Int32 Bloqueado { get; set; }
        public string TipoMaestro { get; set; }
        public Int32 Periodo { get; set; }
    }
}