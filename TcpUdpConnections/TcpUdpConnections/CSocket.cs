using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net;

namespace TcpUdpConnections
{
    public class CSocket
    {
        /// <summary>
        /// Create object containing information about TCP/UDP connection.
        /// </summary>
        /// <param name="type">TCP or UDP</param>
        /// <param name="locAdr">Local address</param>
        /// <param name="locPort">Local port</param>
        /// <param name="remoteAdr">Remote address</param>
        /// <param name="remotePort">Remote port</param>
        /// <param name="pid">Owner's PID</param>
        /// <param name="state">State of connection</param>
        public CSocket(string type, uint locAdr, uint locPort, uint remoteAdr, uint remotePort, int pid, uint state)
        {
            Type = type;
            if (Type == "TCP")
            {
                RemoteAddr = new IPAddress(remoteAdr).ToString() + ":" + (((remotePort << 8) & 65280) + (remotePort >> 8)).ToString();
            }
            else
            {
                RemoteAddr = "";
            }
            LocalAddr = new IPAddress(locAdr).ToString() + ":" + (((locPort << 8) & 65280) + (locPort >> 8)).ToString();
            OwnPid = pid;
            OwnProcName = Process.GetProcessById(OwnPid).ProcessName;
            switch (state)
            {
                case 1: State = "CLOSED"; break;
                case 2: State = "LISTEN"; break;
                case 3: State = "SYN-SENT"; break;
                case 4: State = "SYN-RECEIVED"; break;
                case 5: State = "ESTABLISHED"; break;
                case 6: State = "FIN-WAIT-1"; break;
                case 7: State = "FIN-WAIT-2"; break;
                case 8: State = "CLOSE-WAIT"; break;
                case 9: State = "CLOSING"; break;
                case 10: State = "LAST-ACK"; break;
                case 11: State = "TIME-WAIT"; break;
                case 12: State = "DELETE TCB"; break;
                default: State = ""; break;
            }
        }

        /// <summary>
        /// Type of connection: TCP or Udp.
        /// </summary>
        public string Type { set; get; }

        /// <summary>
        /// Local address.
        /// </summary>
        public string LocalAddr { set; get; }

        /// <summary>
        /// Remote address.
        /// </summary>
        public string RemoteAddr { set; get; }

        /// <summary>
        /// Owner's PID.
        /// </summary>
        public int OwnPid { set; get; }

        /// <summary>
        /// Owner's process name.
        /// </summary>
        public string OwnProcName { set; get; }

        /// <summary>
        /// State of connection.
        /// </summary>
        public string State { set; get; }
    }
}
