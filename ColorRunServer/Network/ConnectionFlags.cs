using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ColorRunServer.Network
{
    /// <summary>
    /// Represents any possible situation for when a user logs in
    /// </summary>
    internal enum ConnectionFlags
    {
        Accepted,
        BadFirstPacket,
        WrongPassword,
        SocketError,
        OtherError
    }
}
