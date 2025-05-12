using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using EasySave.Core.Models;
using EasySave.Core.Enums;
using EasySave.Core.Services;
using EasySave.Logger;

namespace EasySave.ConsoleApp.Services.Backup
{
    public class DifferentialBackupService : IBackupService
    {
        public void Execute(SaveWork work)
        {
            if (!Directory.Exists(work.SourcePath))
            {
                Console.WriteLine($"[ERROR] Source introuvable : {work.SourcePath}");
                return;
            }

            work.Status = SaveState.Active;

            var allFiles = Directory.GetFiles(work.SourcePath, "*", SearchOption.AllDirectories);
            var toCopy = new List<string>();

            foreach (var src in allFiles)
            {
                var rel = Path.GetRelativePath(work.SourcePath, src);
                var dst = Path.Combine(work.TargetPath, rel);

                if (!File.Exists(dst) ||
                    File.GetLastWriteTime(src) > File.GetLastWriteTime(dst))
                {
                    toCopy.Add(src);
                }
            }

            int total = toCopy.Count;
            long size = 0;
            foreach (var f in toCopy) size += new FileInfo(f).Length;

            var state = new SaveStateEntry
            {
                Name = work.Name,
                State = "Active",
                TotalFilesToCopy = total,
                TotalFilesSize = size,
                NbFilesLeftToDo = total,
                Progression = 0
            };

            int done = 0;
            foreach (var src in toCopy)
            {
                var rel = Path.GetRelativePath(work.SourcePath, src);
                var dst = Path.Combine(work.TargetPath, rel);
                Directory.CreateDirectory(Path.GetDirectoryName(dst)!);

                var sw = Stopwatch.StartNew();
                try { File.Copy(src, dst, true); sw.Stop(); }
                catch
                {
                    sw.Stop();
                    LoggerService.Instance.Log(new LogEntry
                    {
                        Name = work.Name,
                        FileSource = src,
                        FileTarget = dst,
                        FileSize = new FileInfo(src).Length,
                        FileTransferTime = -1,
                        Time = DateTime.Now
                    });
                    continue;
                }

                LoggerService.Instance.Log(new LogEntry
                {
                    Name = work.Name,
                    FileSource = src,
                    FileTarget = dst,
                    FileSize = new FileInfo(src).Length,
                    FileTransferTime = sw.Elapsed.TotalMilliseconds,
                    Time = DateTime.Now
                });

                done++;
                state.NbFilesLeftToDo = total - done;
                state.Progression = (int)((double)done / total * 100);
                state.CurrentFileSource = src;
                state.CurrentFileTarget = dst;
                state.LastActionTime = DateTime.Now;
                StateService.Instance.WriteState(new List<SaveStateEntry> { state });
            }

            work.Status = SaveState.Completed;
        }
    }
}
