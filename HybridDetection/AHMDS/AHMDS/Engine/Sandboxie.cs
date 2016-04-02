using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AHMDS.Engine
{
    public class SandboxPaths
    {
        public string FilePath;
        public string KeyPath;
        public string IpcPath;
    }

    public class ProcessInformation
    {
        public UInt32 ProcessId;
        public string BoxName;
        public string ImageName;
        public string SidString;
        public UInt32 SessionId;

        public override string ToString()
        {
            try { return string.IsNullOrWhiteSpace(this.ImageName) ? base.ToString() : this.ImageName; }
            catch { return base.ToString(); }
        }
    }

    /// <summary>
    /// Wrapper class to dynamically load Sandboxie dll (SbieDll.dll).  See http://www.sandboxie.com/index.php?SBIE_DLL_API
    /// </summary>
    public class Sandboxie : IDisposable
    {
        private const string kernal32dll = "kernel32.dll";

        [DllImport(kernal32dll)]
        private static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport(kernal32dll)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport(kernal32dll)]
        private static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// Enumerate Sandbox Names
        /// </summary>
        /// <param name="index">Specifies which sandbox to return. Initialize to -1. Sandboxes are enumerated in the order they appear in Sandboxie.ini.</param>
        /// <param name="box_name">Receives the sandbox name</param>
        /// <returns>Returns the next value to use for the index parameter. Returns -1 when there is nothing left to enumerate.</returns>
        private delegate Int32 SbieApi_EnumBoxes(Int32 index, byte[] box_name);

        /// <summary>
        /// Query Sandbox Paths by Sandbox Name
        /// </summary>
        /// <param name="box_name">Specifies the name of the sandbox for which to return path information.</param>
        /// <param name="file_path">Receives the path to the root directory of the sandbox, as set by the FileRootPath setting. The buffer receives at most the number of bytes specified by the file_path_len parameter.  Pass NULL to ignore this parameter.</param>
        /// <param name="key_path">Receives the path to the root key of the sandbox registry, as set by the KeyRootPath setting. The buffer receives at most the number of bytes specified by the key_path_len parameter.  Pass NULL to ignore this parameter.</param>
        /// <param name="ipc_path">Receives the path to the root object directory of the sandbox, as set by the IpcRootPath setting. The buffer receives at most the number of bytes specified by the ipc_path_len parameter.  Pass NULL to ignore this parameter.</param>
        /// <param name="file_path_len">Specifies the length in bytes of the file_path buffer.  On return, receives the length in bytes needed to receive a complete buffer.</param>
        /// <param name="key_path_len">Specifies the length in bytes of the key_path buffer.  On return, receives the length in bytes needed to receive a complete buffer.</param>
        /// <param name="ipc_path_len">Specifies the length in bytes of the ipc_path buffer.  On return, receives the length in bytes needed to receive a complete buffer.</param>
        /// <returns>Returns zero on success, a non-zero value on error.</returns>
        private delegate Int32 SbieApi_QueryBoxPath(byte[] box_name, byte[] file_path, byte[] key_path, byte[] ipc_path,
            ref ulong file_path_len, ref ulong key_path_len, ref ulong ipc_path_len);

        /// <summary>
        /// Query Sandbox Paths by Process ID
        /// </summary>
        /// <param name="process_id">Specifies the ID of the sandboxed process to query.</param>
        /// <param name="file_path">Receives the path to the root directory of the sandbox, as set by the FileRootPath setting. The buffer receives at most the number of bytes specified by the file_path_len parameter.  Pass NULL to ignore this parameter.</param>
        /// <param name="key_path">Receives the path to the root key of the sandbox registry, as set by the KeyRootPath setting. The buffer receives at most the number of bytes specified by the key_path_len parameter.  Pass NULL to ignore this parameter.</param>
        /// <param name="ipc_path">Receives the path to the root object directory of the sandbox, as set by the IpcRootPath setting. The buffer receives at most the number of bytes specified by the ipc_path_len parameter.  Pass NULL to ignore this parameter.</param>
        /// <param name="file_path_len">Specifies the length in bytes of the file_path buffer.  On return, receives the length in bytes needed to receive a complete buffer.</param>
        /// <param name="key_path_len">Specifies the length in bytes of the key_path buffer.  On return, receives the length in bytes needed to receive a complete buffer.</param>
        /// <param name="ipc_path_len">Specifies the length in bytes of the ipc_path buffer.  On return, receives the length in bytes needed to receive a complete buffer.</param>
        /// <returns>Returns zero on success, a non-zero value on error.</returns>
        private delegate Int32 SbieApi_QueryProcessPath(uint process_id, byte[] file_path, byte[] key_path, byte[] ipc_path,
            ref ulong file_path_len, ref ulong key_path_len, ref ulong ipc_path_len);

        /// <summary>
        /// Enumerate Running Processes
        /// </summary>
        /// <param name="box_name">Specifies the name of the sandbox in which processes will be enumerated.</param>
        /// <param name="all_sessions">Specifies TRUE to enumerate processes in all logon sessions or only in a particular logon session</param>
        /// <param name="which_session">Specifies the logon session number in which processes will be enumerated.  Ignored if all_sessions if TRUE. Pass the value -1 to specify the current logon session.</param>
        /// <param name="boxed_pids">Receives the process ID (PID) numbers. The first ULONG receives the number of processes enumerated. The second ULONG receives the first PID, the third ULONG receives the second PID, and so on.</param>
        /// <returns>Returns zero on success, a non-zero value on error.</returns>
        private delegate Int32 SbieApi_EnumProcessEx(byte[] box_name, bool all_sessions, Int32 which_session,
            [MarshalAs(UnmanagedType.LPArray)] UInt32[] boxed_pids);

        /// <summary>
        /// Query Process Information
        /// </summary>
        /// <param name="process_id">Specifies the ID of the sandboxed process to query.</param>
        /// <param name="box_name">Receives the name of the sandbox in which the process is running.  Pass NULL to ignore this parameter.</param>
        /// <param name="image_name">Receives the process name.  Pass NULL to ignore this parameter.</param>
        /// <param name="sid_string">Receives the SID string for the process.  Pass NULL to ignore this parameter.</param>
        /// <param name="session_id">Receives the logon session number in which the process is running.  Pass NULL to ignore this parameter.</param>
        /// <returns>Returns zero on success, a non-zero value on error.</returns>
        private delegate Int32 SbieApi_QueryProcess(uint process_id, byte[] box_name, byte[] image_name, byte[] sid_string, UIntPtr session_id);

        /// <summary>
        /// Terminate a Single Sandboxed Process
        /// </summary>
        /// <param name="process_id">Specifies the process ID for the sandboxed process that should be terminated.</param>
        /// <returns>Returns TRUE on success, FALSE on failure.  The target process is terminated by the Sandboxie service (SbieSvc) with exit code 1 through a call to the Windows API TerminateProcess (ProcessId, 1).</returns>
        private delegate bool SbieDll_KillOne(uint process_id);

        /// <summary>
        /// Terminate All Sandboxed Processes
        /// </summary>
        /// <param name="session_id">Specifies the logon session number in which sandboxed programs should be terminated.  Specify -1 to indicate the current logon session.</param>
        /// <param name="box_name">Specifies the sandbox name in which sandboxed programs should be terminated.</param>
        /// <returns>Returns TRUE on success, FALSE on failure.  The target processes are terminated in the fashion described above; see SbieDll_KillOne.</returns>
        private delegate bool SbieDll_KillAll(Int32 session_id, byte[] box_name);

        /// <summary>
        /// Query Configuration from Sandboxie.ini
        /// </summary>
        /// <param name="section_name">Specifies the section name that contains the setting to query.</param>
        /// <param name="setting_name">Specifies the setting name to query.</param>
        /// <param name="setting_index">Specifies the zero-based index number for a setting that may appear multiple times.  The index number can be logically OR'ed with these special values: 0x40000000 - do not scan the [GlobalSettings] section if the specified setting name does appear in the specified section. 0x20000000 - do not expand any variables in the result.</param>
        /// <param name="value">Receives the value of the specified setting.</param>
        /// <param name="value_len">Specifies the maximum length in bytes of the buffer pointed to by the value parameter.</param>
        /// <returns>Returns zero on success.  Returns 0xC000008B if the setting was not found.  Any other return value indicates some other error.</returns>
        private delegate Int32 SbieApi_QueryConf(byte[] section_name, byte[] setting_name, UInt32 setting_index, byte[] value, UInt32 value_len);

        /// <summary>
        /// Reload Configuration from Sandboxie.ini
        /// </summary>
        /// <param name="session_id">Specifies the logon session number to which Sandboxie will log any error messages.  Pass -1 for the current logon session.</param>
        /// <returns></returns>
        private delegate Int32 SbieApi_ReloadConf(Int32 session_id);

        //TODO: SbieDll_Hook
        //TODO: DllCallback

        private bool disposed = false;
        private IntPtr dllPtr = IntPtr.Zero;

        public Sandboxie()
            : this(@"C:\Program Files\Sandboxie\SbieDll.dll")
        {
        }

        public Sandboxie(string sbieDllPath)
        {
            if (string.IsNullOrWhiteSpace(sbieDllPath))
                throw new ArgumentNullException("sbieDllPath");
            if (!File.Exists(sbieDllPath))
                throw new FileNotFoundException("Supplied SbieDll.dll not found", sbieDllPath);
            this.dllPtr = LoadLibrary(sbieDllPath);
            if (this.dllPtr == IntPtr.Zero)
                throw new FileLoadException("Unable to load supplied SbieDll.dll", sbieDllPath);
        }

        ~Sandboxie()
        {
            this.Dispose(false);
        }

        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Enumerate Sandbox Names
        /// </summary>
        /// <returns>An array of sandbox names</returns>
        public string[] EnumBoxes()
        {
            SbieApi_EnumBoxes extMethod = (SbieApi_EnumBoxes)this.GetExternalMethodDelegate<SbieApi_EnumBoxes>();

            List<string> result = new List<string>();
            Int32 index = -1;
            while (true)
            {
                byte[] sandboxNameBytes = new byte[34];
                index = extMethod(index, sandboxNameBytes);
                if (index == -1)
                    break;
                string sandboxName = ConvertFromWChar(sandboxNameBytes);
                result.Add(sandboxName);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Query Sandbox Paths by Sandbox Name
        /// </summary>
        /// <param name="box_name">The name of the sandbox for which to return path information.</param>
        /// <returns>A <see cref="SandboxPaths"/> for the supplied <paramref name="box_name"/></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="box_name"/> is null or empty</exception>
        /// <exception cref="System.ApplicationException">Thrown when <see cref="SbieApi_QueryBoxPath"/> returns non-zero</exception>
        public SandboxPaths QueryBoxPath(string box_name)
        {
            if (string.IsNullOrWhiteSpace(box_name))
                throw new ArgumentNullException("box_name");

            SbieApi_QueryBoxPath extMethod = (SbieApi_QueryBoxPath)this.GetExternalMethodDelegate<SbieApi_QueryBoxPath>();

            ulong file_path_len = 0;
            ulong key_path_len = 0;
            ulong ipc_path_len = 0;
            byte[] box_name_bytes = Encoding.Unicode.GetBytes(box_name);

            Int32 methodResult = extMethod(box_name_bytes, null, null, null, ref file_path_len, ref key_path_len, ref ipc_path_len);
            if (methodResult != 0)
                throw new ApplicationException(extMethod.GetType().Name + " returned " + methodResult.ToString());

            byte[] file_path = new byte[file_path_len];
            byte[] key_path = new byte[key_path_len];
            byte[] ipc_path = new byte[ipc_path_len];

            methodResult = extMethod(box_name_bytes, file_path, key_path, ipc_path, ref file_path_len, ref key_path_len, ref ipc_path_len);
            if (methodResult != 0)
                throw new ApplicationException(extMethod.GetType().Name + " returned " + methodResult.ToString());

            string filePath = ConvertFromWChar(file_path);
            string keyPath = ConvertFromWChar(key_path);
            string ipcPath = ConvertFromWChar(ipc_path);

            return new SandboxPaths() { FilePath = filePath, KeyPath = keyPath, IpcPath = ipcPath };
        }

        /// <summary>
        /// Query Sandbox Paths by Process ID
        /// </summary>
        /// <param name="process_id">Specifies the ID of the sandboxed process to query.</param>
        /// <returns>A <see cref="SandboxPaths"/> for the supplied <paramref name="process_id"/></returns>
        /// <exception cref="System.ApplicationException">Thrown when <see cref="SbieApi_QueryProcessPath"/> returns non-zero</exception>
        public SandboxPaths QueryProcessPath(uint process_id)
        {
            SbieApi_QueryProcessPath extMethod = (SbieApi_QueryProcessPath)this.GetExternalMethodDelegate<SbieApi_QueryProcessPath>();

            ulong file_path_len = 0;
            ulong key_path_len = 0;
            ulong ipc_path_len = 0;

            Int32 methodResult = extMethod(process_id, null, null, null, ref file_path_len, ref key_path_len, ref ipc_path_len);
            if (methodResult != 0)
                throw new ApplicationException(extMethod.GetType().Name + " returned " + methodResult.ToString());

            byte[] file_path = new byte[file_path_len];
            byte[] key_path = new byte[key_path_len];
            byte[] ipc_path = new byte[ipc_path_len];

            methodResult = extMethod(process_id, file_path, key_path, ipc_path, ref file_path_len, ref key_path_len, ref ipc_path_len);
            if (methodResult != 0)
                throw new ApplicationException(extMethod.GetType().Name + " returned " + methodResult.ToString());

            string filePath = ConvertFromWChar(file_path);
            string keyPath = ConvertFromWChar(key_path);
            string ipcPath = ConvertFromWChar(ipc_path);

            return new SandboxPaths() { FilePath = filePath, KeyPath = keyPath, IpcPath = ipcPath };
        }

        /// <summary>
        /// Enumerate Running Processes
        /// </summary>
        /// <param name="box_name">Specifies the name of the sandbox in which processes will be enumerated.</param>
        /// <param name="all_sessions">Specifies TRUE to enumerate processes in all logon sessions or only in a particular logon session</param>
        /// <param name="which_session">Specifies the logon session number in which processes will be enumerated.  Ignored if all_sessions if TRUE. Pass the value -1 to specify the current logon session.</param>
        /// <returns>An array of process ID's for the supplied <paramref name="box_name"/></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="box_name"/> is null or empty</exception>
        public UInt32[] EnumProcess(string box_name, bool all_sessions = true, Int32 which_session = -1)
        {
            if (string.IsNullOrWhiteSpace(box_name))
                throw new ArgumentNullException("box_name");

            SbieApi_EnumProcessEx extMethod = (SbieApi_EnumProcessEx)this.GetExternalMethodDelegate<SbieApi_EnumProcessEx>();

            byte[] box_name_bytes = Encoding.Unicode.GetBytes(box_name);
            UInt32[] boxed_pids = new UInt32[512];

            Int32 methodResult = extMethod(box_name_bytes, all_sessions, which_session, boxed_pids);
            if (methodResult != 0)
                throw new ApplicationException(extMethod.GetType().Name + " returned " + methodResult.ToString());

            List<UInt32> result = new List<UInt32>();
            for (int loop = 1; loop <= boxed_pids[0]; loop++)
            {
                if (boxed_pids[0] > 0)
                    result.Add(boxed_pids[loop]);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Query Process Information
        /// </summary>
        /// <param name="process_id">Specifies the ID of the sandboxed process to query.</param>
        /// <returns>A <see cref="ProcessInformation"/> for the supplied <paramref name="process_id"/></returns>
        /// <exception cref="System.ApplicationException">Thrown when <see cref="SbieApi_QueryProcess"/> returns non-zero</exception>
        public ProcessInformation QueryProcess(uint process_id)
        {
            SbieApi_QueryProcess extMethod = (SbieApi_QueryProcess)this.GetExternalMethodDelegate<SbieApi_QueryProcess>();

            byte[] box_name_bytes = new byte[34];
            byte[] image_name_bytes = new byte[96];
            byte[] sid_string_bytes = new byte[96];
            UIntPtr session_id = new UIntPtr();

            Int32 methodResult = extMethod(process_id, box_name_bytes, image_name_bytes, sid_string_bytes, session_id);
            if (methodResult != 0)
                throw new ApplicationException(extMethod.GetType().Name + " returned " + methodResult.ToString());

            string box_name = this.ConvertFromWChar(box_name_bytes);
            string image_name = this.ConvertFromWChar(image_name_bytes);
            string sid_string = this.ConvertFromWChar(sid_string_bytes);

            return new ProcessInformation()
            {
                ProcessId = process_id,
                BoxName = box_name,
                ImageName = image_name,
                SidString = sid_string,
                SessionId = session_id.ToUInt32()
            };
        }

        /// <summary>
        /// Terminate a Single Sandboxed Process
        /// </summary>
        /// <remarks>
        /// When run against version 3.58, fails with "SBIE2203 Failed to communicate with Sandboxie Service:  connect C0000041"
        /// See http://www.sandboxie.com/index.php?SBIE2203
        /// </remarks>
        /// <param name="process_id">Specifies the process ID for the sandboxed process that should be terminated.</param>
        /// <returns>Returns TRUE on success, FALSE on failure.  The target process is terminated by the Sandboxie service (SbieSvc) with exit code 1 through a call to the Windows API TerminateProcess (<paramref name="process_id"/>, 1).</returns>
        public bool KillOne(uint process_id)
        {
            SbieDll_KillOne extMethod = (SbieDll_KillOne)this.GetExternalMethodDelegate<SbieDll_KillOne>();
            return extMethod(process_id);
        }

        /// <summary>
        /// Terminate All Sandboxed Processes
        /// </summary>
        /// <param name="box_name">Specifies the sandbox name in which sandboxed programs should be terminated.</param>
        /// <returns>Returns TRUE on success, FALSE on failure.  The target processes are terminated in the fashion described above; see KillOne.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="box_name"/> is null or empty</exception>
        public bool KillAll(string box_name)
        {
            return this.KillAll(-1, box_name);
        }

        /// <summary>
        /// Terminate All Sandboxed Processes
        /// </summary>
        /// <param name="session_id">Specifies the logon session number in which sandboxed programs should be terminated.  Specify -1 to indicate the current logon session.</param>
        /// <param name="box_name">Specifies the sandbox name in which sandboxed programs should be terminated.</param>
        /// <returns>Returns TRUE on success, FALSE on failure.  The target processes are terminated in the fashion described above; see KillOne.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="box_name"/> is null or empty</exception>
        public bool KillAll(Int32 session_id, string box_name)
        {
            if (string.IsNullOrWhiteSpace(box_name))
                throw new ArgumentNullException("box_name");

            SbieDll_KillAll extMethod = (SbieDll_KillAll)this.GetExternalMethodDelegate<SbieDll_KillAll>();

            byte[] box_name_bytes = Encoding.Unicode.GetBytes(box_name);
            return extMethod(session_id, box_name_bytes);
        }

        /// <summary>
        /// Query Configuration from Sandboxie.ini
        /// </summary>
        /// <param name="section_name">Specifies the section name that contains the setting to query.</param>
        /// <param name="setting_name">Specifies the setting name to query.</param>
        /// <param name="setting_index">Specifies the zero-based index number for a setting that may appear multiple times.  The index number can be logically OR'ed with these special values: 0x40000000 - do not scan the [GlobalSettings] section if the specified setting name does appear in the specified section. 0x20000000 - do not expand any variables in the result.</param>
        /// <returns>Returns zero on success.  Returns 0xC000008B if the setting was not found.  Any other return value indicates some other error.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="section_name"/> is null or empty</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="setting_name"/> is null or empty</exception>
        /// <exception cref="System.ApplicationException">Thrown when <see cref="SbieApi_QueryConf"/> returns non-zero</exception>
        public string QueryConf(string section_name, string setting_name, UInt32 setting_index = 0)
        {
            if (string.IsNullOrWhiteSpace(section_name))
                throw new ArgumentNullException("section_name");
            if (string.IsNullOrWhiteSpace(setting_name))
                throw new ArgumentNullException("setting_name");

            SbieApi_QueryConf extMethod = (SbieApi_QueryConf)this.GetExternalMethodDelegate<SbieApi_QueryConf>();

            byte[] section_name_bytes = Encoding.Unicode.GetBytes(section_name);
            byte[] setting_name_bytes = Encoding.Unicode.GetBytes(setting_name);
            byte[] value = new byte[8000];

            Int32 methodResult = extMethod(section_name_bytes, setting_name_bytes, setting_index, value, (UInt32)value.Length);
            if (methodResult != 0)
                throw new ApplicationException(extMethod.GetType().Name + " returned " + methodResult.ToString());

            return this.ConvertFromWChar(value);
        }

        /// <summary>
        /// Reload Configuration from Sandboxie.ini
        /// </summary>
        /// <param name="session_id">Specifies the logon session number to which Sandboxie will log any error messages.  Pass -1 for the current logon session.</param>
        public void ReloadConf(Int32 session_id = -1)
        {
            SbieApi_ReloadConf extMethod = (SbieApi_ReloadConf)this.GetExternalMethodDelegate<SbieApi_ReloadConf>();

            Int32 methodResult = extMethod(session_id);
            if (methodResult != 0)
                throw new ApplicationException(extMethod.GetType().Name + " returned " + methodResult.ToString());
        }

        /// <summary>
        /// Enumerate Running Query Process Information
        /// </summary>
        /// <param name="box_name">Specifies the name of the sandbox in which processes will be enumerated.</param>
        /// <param name="all_sessions">Specifies TRUE to enumerate processes in all logon sessions or only in a particular logon session</param>
        /// <param name="which_session">Specifies the logon session number in which processes will be enumerated.  Ignored if all_sessions if TRUE. Pass the value -1 to specify the current logon session.</param>
        /// <returns>An array of <see cref="ProcessInformation"/> for the supplied <paramref name="box_name"/></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="box_name"/> is null or empty</exception>
        public ProcessInformation[] QueryEnumProcess(string box_name, bool all_sessions = true, Int32 which_session = -1)
        {
            if (string.IsNullOrWhiteSpace(box_name))
                throw new ArgumentNullException("box_name");

            UInt32[] pids = this.EnumProcess(box_name, all_sessions, which_session);
            return pids.Select(pid => this.QueryProcess(pid)).ToArray();
        }

        /// <summary>
        /// Returns true if the sandbox is idle (has no running processes)
        /// </summary>
        /// <param name="box_name">Specifies the name of the sandbox in which to check for running processes.</param>
        /// <returns>Returns true if the sandbox specified in <paramref name="box_name"/> has no running processes.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="box_name"/> is null or empty</exception>
        public bool BoxIsIdle(string box_name)
        {
            if (string.IsNullOrWhiteSpace(box_name))
                throw new ArgumentNullException("box_name");
            return this.EnumProcess(box_name).Length == 0;
        }

        /// <summary>
        /// Returns true if the sandbox is idle (has running processes)
        /// </summary>
        /// <param name="box_name">Specifies the name of the sandbox in which to check for running processes.</param>
        /// <returns>Returns true if the sandbox specified in <paramref name="box_name"/> has running processes.</returns>
        public bool BoxIsBusy(string box_name)
        {
            return !this.BoxIsIdle(box_name);
        }

        /// <summary>
        /// Enumerate Idle Sandbox Names
        /// </summary>
        /// <returns>An array of sandbox names that are idle</returns>
        public string[] EnumIdleBoxes()
        {
            return this.EnumBoxes().Where(box => this.BoxIsIdle(box)).ToArray();
        }

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                try
                {
                    if (disposing)
                    {
                        if (this.dllPtr != IntPtr.Zero)
                            FreeLibrary(this.dllPtr);
                        this.dllPtr = IntPtr.Zero;
                    }
                }
                finally { this.disposed = true; }
            }
        }

        private Delegate GetExternalMethodDelegate<T>()
        {
            Type type = typeof(T);
            IntPtr funcAddr = GetProcAddress(this.dllPtr, type.Name);
            if (funcAddr == IntPtr.Zero)
                throw new ApplicationException(string.Format("External method '{0}' not found", type.Name));
            return Marshal.GetDelegateForFunctionPointer(funcAddr, type);
        }

        private string ConvertFromWChar(byte[] stringBytes)
        {
            string result = Encoding.Unicode.GetString(stringBytes);
            int idx = result.IndexOf("\0");
            if (idx != -1)
                result = result.Remove(idx);
            return result;
        }
    }
}
