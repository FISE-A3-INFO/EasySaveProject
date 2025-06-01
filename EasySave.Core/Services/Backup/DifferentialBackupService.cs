using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using EasySave.Core.Models;
using EasySave.Core.Enums;
using EasySave.Core.Services;
using EasySave.Logger;

namespace EasySave.Core.Services.Backup
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

            work.Status = EasySave.Core.Enums.SaveState.Active;
            System.Threading.Thread.Sleep(3000);

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

            // ==== PRIORITE ====
            var prioritaryFiles = toCopy.Where(f => ConfigService.PrioritaryExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();
            var nonPrioritaryFiles = toCopy.Where(f => !ConfigService.PrioritaryExtensions.Contains(Path.GetExtension(f).ToLower())).ToList();

            foreach (var src in prioritaryFiles.Concat(nonPrioritaryFiles))
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
                long sizeBytes = new FileInfo(src).Length;

                // ========== GESTION FICHIER VOLUMINEUX ==========
                LargeFileTransferCoordinator.EnterIfLargeFile(sizeBytes);

                try
                {
                    File.Copy(src, dst, true);
                    sw.Stop();
                }
                catch
                {
                    sw.Stop();
                    LoggerService.Instance.Log(new LogEntry
                    {
                        Name = work.Name,
                        FileSource = src,
                        FileTarget = dst,
                        FileSize = sizeBytes,
                        FileTransferTime = -1,
                        Time = DateTime.Now
                    });
                    continue;
                }
                finally
                {
                    LargeFileTransferCoordinator.ExitIfLargeFile(sizeBytes);
                }
                // ========== FIN GESTION FICHIER VOLUMINEUX ==========

                LoggerService.Instance.Log(new LogEntry
                {
                    Name = work.Name,
                    FileSource = src,
                    FileTarget = dst,
                    FileSize = sizeBytes,
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

            work.Status = EasySave.Core.Enums.SaveState.Completed;
        }
    }
}
