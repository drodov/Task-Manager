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
    public static class MiniDump
    {
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
