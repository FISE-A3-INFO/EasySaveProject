using EasySave.Core.Models;

namespace EasySave.Core.Services
{
    public interface IBackupService
    {
        void Execute(SaveWork work);
    }
}
