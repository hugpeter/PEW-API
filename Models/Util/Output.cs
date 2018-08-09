using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PEW_API.Models.Util
{
    public class Output
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Output(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}