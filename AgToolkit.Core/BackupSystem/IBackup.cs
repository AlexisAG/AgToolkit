using System.Collections;

namespace AgToolkit.AgToolkit.Core.BackupSystem
{
    public interface IBackup
    {
        IEnumerator Save();
        IEnumerator Load();
    }
}
