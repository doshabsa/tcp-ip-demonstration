using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPLibrary
{
    public class MouseEventObject
    {
        public int? X { get; set; }
        public int? Y { get; set; }
        public string? Button { get; set; }

        public string JsonSerialized()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
