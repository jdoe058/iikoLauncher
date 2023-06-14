using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace iikoLauncher.Models
{
    [Serializable()]
    public class Server
    {
        [XmlAttribute]
        public string ClientName { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Login { get; set; }

        [XmlAttribute]
        public string Password { get; set; }

        [XmlAttribute]
        public string Address { get; set; }

        [XmlAttribute]
        public string Port { get; set; }


    }

    [Serializable()]
    public class Servers
    { 
        [XmlElement]
        public List<Server> Server { get; set; }
    }
}
