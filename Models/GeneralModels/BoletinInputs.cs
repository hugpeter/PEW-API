using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEW_API.Models.GeneralModels
{
    public class BoletinInputs
    {
        public Int32 IdColegio { get; set; }
        public Int32 Ano { get; set; }
        public Int32 Bimestre { get; set; }
        public string Cedula { get; set; }

        public BoletinInputs()
        {

        }
    }
}