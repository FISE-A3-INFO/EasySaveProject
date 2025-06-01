using System;
using System.Diagnostics;
using System.IO;
using EasySave.Core.Models;
using EasySave.Core.Enums;
using EasySave.Core.Services;
using EasySave.Logger;

namespace EasySave.Core.Services.Backup
{
    public class FullBackupService : IBackupService
    {
        public void Execute(SaveWork work)
        {
            if (!Directory.Exists(work.SourcePath))
            {
                Console.WriteLine($"[ERROR] Source introuvable : {work.SourcePath}");
                return;
            }

            work.Status = EasySave.Core.Enums.SaveState.Active;
            System.Threading.Thread.Sleep(3000);
            
            var files = Directory.GetFiles(work.SourcePath, "*", SearchOption.AllDirectories);
            int total = files.Length;
            long size = 0;
            foreach (var f in files) size += new FileInfo(f).Length;

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
            foreach (var src in files)
            {
                while (work.PauseRequested)
                    System.Threading.Thread.Sleep(200);

                while (PauseService.GlobalPauseRequested)
                {
                    work.Status = SaveState.Paused;
                    System.Threading.Thread.Sleep(300);
                }
                if (work.Status == SaveState.Paused)
                    work.Status = SaveState.Active;

                var rel = Path.GetRelativePath(work.SourcePath, src);
                var dst = Path.Combine(work.TargetPath, rel);
                Directory.CreateDirectory(Path.GetDirectoryName(dst)!);

                var sw = Stopwatch.StartNew();
                try
                { File.Copy(src, dst, true); sw.Stop(); }
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
                StateService.Instance.WriteState(new System.Collections.Generic.List<SaveStateEntry> { state });
            }

            work.Status = EasySave.Core.Enums.SaveState.Completed;
        }
    }
}
