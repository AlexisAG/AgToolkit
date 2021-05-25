using System.Collections;

namespace AgToolkit.Core.DataSystem
{
    public interface IBackup
    {
        IEnumerator Save();
        IEnumerator Load();
    }
}
