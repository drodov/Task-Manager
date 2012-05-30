using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;

namespace TaskManager
{
    /// <summary>
    /// Represnets access to method making dump of process.
    /// </summary>
    public static class MiniDump
    {
        /// <summary>
        /// Dump types.
        /// </summary>
        [Flags]
        enum MINIDUMP_TYPE : uint
        {
            MiniDumpNormal = 0x00000000,
            MiniDumpWithDataSegs = 0x00000001,
            MiniDumpWithFullMemory = 0x00000002,
            MiniDumpWithHandleData = 0x00000004,
            MiniDumpFilterMemory = 0x00000008,
            MiniDumpScanMemory = 0x00000010,
            MiniDumpWithUnloadedModules = 0x00000020,
            MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
            MiniDumpFilterModulePaths = 0x00000080,
            MiniDumpWithProcessThreadData = 0x00000100,
            MiniDumpWithPrivateReadWriteMemory = 0x00000200,
            MiniDumpWithoutOptionalData = 0x00000400,
            MiniDumpWithFullMemoryInfo = 0x00000800,
            MiniDumpWithThreadInfo = 0x00001000,
            MiniDumpWithCodeSegs = 0x00002000,
            MiniDumpWithoutAuxiliaryState = 0x00004000,
            MiniDumpWithFullAuxiliaryState = 0x00008000,
            MiniDumpWithPrivateWriteCopyMemory = 0x00010000,
            MiniDumpIgnoreInaccessibleMemory = 0x00020000,
            MiniDumpWithTokenInformation = 0x00040000
        };

        /// <summary>
        /// Writes user-mode minidump information to the specified file.
        /// </summary>
        /// <param name="hProcess">A handle to the process for which the information is to be generated.</param>
        /// <param name="ProcessId">The identifier of the process for which the information is to be generated.</param>
        /// <param name="hFile">A handle to the file in which the information is to be written.</param>
        /// <param name="DumpType">The type of information to be generated. This parameter can be one or more of the values from the MINIDUMP_TYPE enumeration.</param>
        /// <param name="ExceptionParam">A pointer to a MINIDUMP_EXCEPTION_INFORMATION structure describing the client exception that caused the minidump to be generated.</param>
        /// <param name="UserStreamParam">A pointer to a MINIDUMP_USER_STREAM_INFORMATION structure.</param>
        /// <param name="CallbackParam">A pointer to a MINIDUMP_CALLBACK_INFORMATION structure that specifies a callback routine which is to receive extended minidump information.</param>
        /// <returns>If the function succeeds, the return value is TRUE; otherwise, the return value is FALSE.</returns>
        [DllImport("DbgHelp.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            int ProcessId,
            IntPtr hFile,
            MINIDUMP_TYPE DumpType,
            IntPtr ExceptionParam,
            IntPtr UserStreamParam,
            IntPtr CallbackParam
            );

        /// <summary>
        /// Makes dump of process.
        /// </summary>
        /// <param name="procId">PID of process to be dumped.</param>
        public static void MakeDump(int procId)
        {
            using (var process = Process.GetProcessById(procId))
            {
                if (process == null)
                    return;
                string fileName = "C:\\TaskManager\\Dump\\" + process.ProcessName + ".dmp";
                using (var file = File.Open(fileName, FileMode.Create, FileAccess.Write))
                {
                    var dumpType = MINIDUMP_TYPE.MiniDumpWithFullMemory;
                    if (!MiniDumpWriteDump(
                            process.Handle, process.Id, file.Handle,
                            dumpType, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero)
                        )
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }
    }
}
