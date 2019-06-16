using System;
using System.IO;
using System.Linq;
using VRCModLoader;
using UnityEngine;


namespace Mod
{

    [VRCModInfo("Log Manager", "1.0", "Bluscream")]
    public class LogManager : VRCMod
    {
        // DirectoryInfo logDir = new DirectoryInfo(CombinePaths(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..", "LocalLow", "VRChat", "vrchat"));
        static DirectoryInfo logdir_game = new DirectoryInfo(Application.persistentDataPath);
        static DirectoryInfo logdir_vrcml = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "Logs"));
        static DirectoryInfo logdir_final = new DirectoryInfo(Path.Combine(logdir_game.FullName, "logs"));
        static string pattern_game = "output_log_*-*-*_??.txt";
        static string pattern_vrcml = "VRCModLoader_????-??-??-??-??-??-??.log";
        
        void OnApplicationStart()
        {
            Utils.Log($"OnApplicationStart");
            MoveAllLogs(all: true);
        }
        void OnApplicationQuit()
        {
            Utils.Log($"OnApplicationQuit");
            MoveAllLogs();
        }
        /* void OnUpdate()
         {
             if (firstUpdate)
             {
                 Log($"OnUpdate");
                 firstUpdate = false;
                 var source = lastLog().FullName; Log(source);
                 var target = CombinePaths(logDir.FullName, "output_log.txt"); Log(target);
                 CreateSymLink(source, target);
             }
         }*/

        /*public static void CreateSymLink(string name, string target, bool isDirectory = false)
        {
            if (!Win32.CreateSymbolicLink(name, target, isDirectory ? Win32.SymLinkFlag.Directory : Win32.SymLinkFlag.File)) {
                throw new Win32Exception();
            }
        }*/

        /*FileInfo lastLog() {
            return getLogs().First();
        }*/

        IOrderedEnumerable<FileInfo> getGameLogs(bool all = false) => getLogs(logdir_game, pattern_game, all);
        IOrderedEnumerable<FileInfo> getVRCMLLogs(bool all = false) => getLogs(logdir_vrcml, pattern_vrcml, all);
        IOrderedEnumerable<FileInfo> getLogs(DirectoryInfo dir, string pattern, bool latestIncluded = false) {
            var ret = dir.GetFiles(pattern).OrderByDescending(f => f.LastWriteTime);
            if (!latestIncluded) return ret.Skip(1).OrderByDescending(f => f.LastWriteTime);
            return ret;
        }

        private void MoveAllLogs(bool all = false) { 
            Utils.Log("Target Directory:", logdir_final.FullName);
            var logs = getGameLogs(all);
            Utils.Log("Moving", logs.Count(), "old game logs");
            MoveLogs(logs);
            logs = getVRCMLLogs(all);
            Utils.Log("Moving", logs.Count(), "old VRCModLoader logs");
            MoveLogs(logs);
        }

        private void MoveLogs(IOrderedEnumerable<FileInfo> logs, bool delete = false)  {
            if (!logdir_final.Exists) Directory.CreateDirectory(logdir_final.FullName);
            foreach (var log in logs) {
                try {
                    if (delete) log.Delete();
                    else log.MoveTo(Path.Combine(logdir_final.FullName, log.Name));
                } catch (Exception ex) { Utils.Log($"Could not {(delete ? "delete" : "move")} {log.Name} ({ex.Message})"); }
            }
        }
        /*internal static class Win32 {
            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AllocConsole();

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymLinkFlag dwFlags);

            internal enum SymLinkFlag
            {
                File = 0,
                Directory = 1
            }
        }
        */
    }
}