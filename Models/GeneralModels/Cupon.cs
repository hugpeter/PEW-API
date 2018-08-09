using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEW_API.Models.GeneralModels
{
    public class Cupon
    {
        public string url { get; set; }
        public string src { get; set; }

        public Cupon(string _url, string _src)
        {
            url = _url;
            src = _src;
        }
    }
}