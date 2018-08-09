using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEW_API.Models
{
    public class Familia
    {
        public string NombreCompleto { get; set; }
        public string Nombre1 { get; set; }
        public string Nombre2 { get; set; }
        public string Apellido1 { get; set; }
        public string Apellido2 { get; set; }
        public string Cedula { get; set; }
        public Int32 IdxEstudiante { get; set; }

        public Familia()
        {

        }
    }
}