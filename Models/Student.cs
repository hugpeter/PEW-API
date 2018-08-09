using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEW_API.Models
{
    public class Student
    {
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string TipoMaestro { get; set; }
        public Int32 Ano { get; set; }
        public Int32 Periodo { get; set; }
        public Int32 IdColegio { get; set; }
        public string Colegio { get; set; }
        public Int32 Bloqueado { get; set; }
        public string Aviso { get; set; }
        public Int32 Periodo_pre { get; set; }
        public Int32 Nivel { get; set; }
        public Int32 GrupoUsuario { get; set; }
        public byte Cambiarpass { get; set; }
        public Int32 Idioma { get; set; }
        public Int32 IdxMaestro { get; set; }

        public Student()
        {

        }
    }
}