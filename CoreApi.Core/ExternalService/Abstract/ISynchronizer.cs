using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApi.Common.Constants;

namespace CoreApi.Core.ExternalService.Abstract
{
    public interface ISynchronizer
    {
        Task<bool> WaitAsync(string type, string value, int count = 1, int millisecondsTimeout = SynchronizerConstants.MillisecondsTimeout);
        Task<bool> WaitAsync(string type, IEnumerable<string> values, int count = 1, int millisecondsTimeout = SynchronizerConstants.MillisecondsTimeout);
        int Release(string type, string value);
        int Release(string type, IEnumerable<string> values);
        void UnlockThreadLocks(string type);
    }
}