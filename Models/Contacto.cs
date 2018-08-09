using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEW_API.Models
{
    public class Contacto
    {
        public string CodigoContacto { get; set; }
        public string NombreContacto { get; set; }
        public string GrupoContacto { get; set; }
        public Int32 IdGrupo { get; set; }
        public string Cargo { get; set; }

        public Contacto()
        {

        }
    }
}