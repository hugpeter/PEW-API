using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEW_API.Models.GeneralModels
{
    public class EstadoInputs
    {
        public string Cedula { get; set; }
        public int Idioma { get; set; }
        public int IdColegio { get; set; }

        public EstadoInputs(){ }
    }
}