using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wonton.CrossUI.Web.Services
{
    public class BridgeSettings
    {        
        /// <summary>
        /// Gets the socket port.
        /// </summary>
        /// <value>
        /// The socket port.
        /// </value>
        public static string SocketPort { get; internal set; }

        /// <summary>
        /// Gets the web port.
        /// </summary>
        /// <value>
        /// The web port.
        /// </value>
        public static string WebPort { get; internal set; }
    }
}
