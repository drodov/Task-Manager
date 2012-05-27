using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TcpUdpConnections
{
    public static unsafe class GetTcpUdp
    {
        const string IPHLPAPI = "Iphlpapi.dll";
        const uint AF_INET = 2;
        const uint AF_INET6 = 23;

        enum TCP_TABLE_CLASS : uint
        {
            TCP_TABLE_BASIC_LISTENER,
            TCP_TABLE_BASIC_CONNECTIONS,
            TCP_TABLE_BASIC_ALL,
            TCP_TABLE_OWNER_PID_LISTENER,
            TCP_TABLE_OWNER_PID_CONNECTIONS,
            TCP_TABLE_OWNER_PID_ALL,
            TCP_TABLE_OWNER_MODULE_LISTENER,
            TCP_TABLE_OWNER_MODULE_CONNECTIONS,
            TCP_TABLE_OWNER_MODULE_ALL
        }

        // IPv4
        [StructLayout(LayoutKind.Sequential)]
        struct MIB_TCPTABLE_OWNER_PID
        {
            public uint dwNumEntries;
            public MIB_TCPROW_OWNER_PID table;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MIB_TCPROW_OWNER_PID
        {
            public uint dwState;
            public uint dwLocalAddr;
            public uint dwLocalPort;
            public uint dwRemoteAddr;
            public uint dwRemotePort;
            public int dwOwningPid;
        }

        // IPv6
        [StructLayout(LayoutKind.Sequential)]
        struct MIB_TCP6TABLE_OWNER_PID
        {
            public uint dwNumEntries;
            public MIB_TCP6ROW_OWNER_PID table;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MIB_TCP6ROW_OWNER_PID
        {
            public fixed byte ucLocalAddr[16];
            public uint dwLocalScopeId;
            public uint dwLocalPort;
            public fixed byte ucRemoteAddr[16];
            public uint dwRemoteScopeId;
            public uint dwRemotePort;
            public uint dwState;
            public int dwOwningPid;
        }

        [DllImport(IPHLPAPI, SetLastError = true)]
        static extern uint GetExtendedTcpTable(
            [Out] void* pTcpTable,
            [In, Out] int* pdwSize,
            [In] bool bOrder,
            [In] uint ulAf,
            [In] TCP_TABLE_CLASS TableClass,
            [In] uint Reserved
            );

        enum UDP_TABLE_CLASS : uint
        {
            UDP_TABLE_BASIC,
            UDP_TABLE_OWNER_PID,
            UDP_TABLE_OWNER_MODULE
        }

        // IPv4
        [StructLayout(LayoutKind.Sequential)]
        struct MIB_UDPTABLE_OWNER_PID
        {
            public uint dwNumEntries;
            public MIB_UDPROW_OWNER_PID table;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MIB_UDPROW_OWNER_PID
        {
            public uint dwLocalAddr;
            public uint dwLocalPort;
            public int dwOwningPid;
        }

        // IPv6
        [StructLayout(LayoutKind.Sequential)]
        struct MIB_UDP6TABLE_OWNER_PID
        {
            public uint dwNumEntries;
            public MIB_UDP6ROW_OWNER_PID table;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MIB_UDP6ROW_OWNER_PID
        {
            public fixed byte ucLocalAddr[16];
            public uint dwLocalScopeId;
            public uint dwLocalPort;
            public int dwOwningPid;
        }

        [DllImport(IPHLPAPI, SetLastError = true)]
        static extern uint GetExtendedUdpTable(
            [Out] void* pUDPTable,
            [In, Out] int* pdwSize,
            [In] bool bOrder,
            [In] uint ulAf,
            [In] UDP_TABLE_CLASS TableClass,
            [In] uint Reserved
            );

        /// <summary>
        /// List collection of connections. 
        /// </summary>
        static List<CSocket> SocketList;

        /// <summary>
        /// Get list of TCP/UDP connections.
        /// </summary>
        /// <returns>List of TCP/UDP connections.</returns>
        public static List<CSocket> GetTcpUdpList()
        {
            SocketList = new List<CSocket>();
            uint resultV4, resultV6, ipVersion;
            int tblSizeV4 = 0, tblSizeV6 = 0;
            void* pMemV4 = null;
            void* pMemV6 = null;

            ipVersion = 2;
            resultV4 = GetExtendedTcpTable(null, &tblSizeV4, true, ipVersion, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
            if (tblSizeV4 != 0)
            {
                pMemV4 = (void*)Marshal.AllocHGlobal(tblSizeV4);
                resultV4 = GetExtendedTcpTable(pMemV4, &tblSizeV4, true, ipVersion, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
                if (resultV4 == 0)
                    GetIPv4Info((MIB_TCPTABLE_OWNER_PID*)pMemV4);
            }

            ipVersion = 23;
            resultV6 = GetExtendedTcpTable(null, &tblSizeV6, true, ipVersion, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
            if (tblSizeV6 != 0)
            {
                pMemV6 = (void*)Marshal.AllocHGlobal(tblSizeV6);
                resultV6 = GetExtendedTcpTable(pMemV6, &tblSizeV6, true, ipVersion, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL, 0);
                if (resultV6 == 0)
                    GetIPv6Info((MIB_TCP6TABLE_OWNER_PID*)pMemV6);
            }

            if (pMemV4 != null)
            {
                Marshal.FreeHGlobal((IntPtr)pMemV4);
                pMemV4 = null;
            }
            if (pMemV6 != null)
            {
                Marshal.FreeHGlobal((IntPtr)pMemV6);
                pMemV6 = null;
            }

            //
            uint UDPresultV4, UDPresultV6;
            tblSizeV4 = 0;
            tblSizeV6 = 0;
            void* UDPpMemV4 = null;
            void* UDPpMemV6 = null;

            ipVersion = 2;
            UDPresultV4 = GetExtendedUdpTable(null, &tblSizeV4, true, ipVersion, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
            if (tblSizeV4 != 0)
            {
                UDPpMemV4 = (void*)Marshal.AllocHGlobal(tblSizeV4);
                UDPresultV4 = GetExtendedUdpTable(UDPpMemV4, &tblSizeV4, true, ipVersion, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
                if (UDPresultV4 == 0)
                    GetIPv4Info((MIB_UDPTABLE_OWNER_PID*)UDPpMemV4);
            }

            ipVersion = 23;
            UDPresultV6 = GetExtendedUdpTable(null, &tblSizeV6, true, ipVersion, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
            if (tblSizeV6 != 0)
            {
                UDPpMemV6 = (void*)Marshal.AllocHGlobal(tblSizeV6);
                UDPresultV6 = GetExtendedUdpTable(UDPpMemV6, &tblSizeV6, true, ipVersion, UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID, 0);
                if (UDPresultV6 == 0)
                    GetIPv6Info((MIB_UDP6TABLE_OWNER_PID*)UDPpMemV6);
            }

            if (UDPpMemV4 != null)
            {
                Marshal.FreeHGlobal((IntPtr)UDPpMemV4);
                UDPpMemV4 = null;
            }
            if (UDPpMemV6 != null)
            {
                Marshal.FreeHGlobal((IntPtr)UDPpMemV6);
                UDPpMemV6 = null;
            }
            return SocketList;
        }

        /// <summary>
        /// Add to list TCP connections using IPv4.
        /// </summary>
        /// <param name="table"></param>
        static void GetIPv4Info(MIB_TCPTABLE_OWNER_PID* table)
        {
            uint tblSize = table->dwNumEntries; // Number of elements in table.
            var firstRow = &table->table;       // The first element in table.

            for (uint i = 0; i < tblSize; ++i)
            {
                MIB_TCPROW_OWNER_PID row = firstRow[i];
                SocketList.Add(new CSocket("TCP", row.dwLocalAddr, row.dwLocalPort, row.dwRemoteAddr, row.dwRemotePort, row.dwOwningPid, row.dwState));
            }
        }

        /// <summary>
        /// Add to list TCP connections using IPv6.
        /// </summary>
        /// <param name="table"></param>
        static void GetIPv6Info(MIB_TCP6TABLE_OWNER_PID* table)
        {
            uint tblSize = table->dwNumEntries; // Number of elements in table.
            var firstRow = &table->table;       // The first element in table.

            for (uint i = 0; i < tblSize; ++i)
            {
                MIB_TCP6ROW_OWNER_PID row = firstRow[i];
                SocketList.Add(new CSocket("TCP", row.dwLocalScopeId, row.dwLocalPort, row.dwRemoteScopeId, row.dwRemotePort, row.dwOwningPid, row.dwState));
            }
        }

        /// <summary>
        /// Add to list UDP connections using IPv4.
        /// </summary>
        /// <param name="table"></param>
        static void GetIPv4Info(MIB_UDPTABLE_OWNER_PID* table)
        {
            uint tblSize = table->dwNumEntries; // Number of elements in table.
            var firstRow = &table->table;       // The first element in table.

            for (uint i = 0; i < tblSize; ++i)
            {
                MIB_UDPROW_OWNER_PID row = firstRow[i];
                SocketList.Add(new CSocket("UDP", row.dwLocalAddr, row.dwLocalPort, 0, 0, row.dwOwningPid, 0));
            }
        }

        /// <summary>
        /// Add to list UDP connections using IPv4.
        /// </summary>
        /// <param name="table"></param>
        static void GetIPv6Info(MIB_UDP6TABLE_OWNER_PID* table)
        {
            uint tblSize = table->dwNumEntries; // Number of elements in table.
            var firstRow = &table->table;       // The first element in table.

            for (uint i = 0; i < tblSize; ++i)
            {
                MIB_UDP6ROW_OWNER_PID row = firstRow[i];
                SocketList.Add(new CSocket("UDP", row.dwLocalScopeId, row.dwLocalPort, 0, 0, row.dwOwningPid, 0));
            }
        }
    }
}
