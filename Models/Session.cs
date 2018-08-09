using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace PEW_API.Models
{
    public class Session
    {
        public string Usuario { get; set; }
        public string Password { get; set; }

        public string Token { get; set; }
       
        public Student Student { get; set; }

        public List<Familia> FamilyMembers { get; set; }
        public List<String> FamilyOptions { get; set; }
        public Boolean IsFamilia { get; set; }

        public List<Contacto> Contactos { get; set; }

        public Boolean IsValid()
        {
            return this.Student.IdxMaestro == 0;
        }

        public Session()
        {
            Student = new Student();
            FamilyMembers = new List<Familia>();
            FamilyOptions = new List<String>();
            Contactos = new List<Contacto>();
        }
    }
}