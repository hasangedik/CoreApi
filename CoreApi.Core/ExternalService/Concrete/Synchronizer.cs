using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreApi.Core.ExternalService.Abstract;

namespace CoreApi.Core.ExternalService.Concrete
{
    public class Synchronizer : ISynchronizer
    {
        private const int MillisecondsTimeout = 10000;
        private static readonly SemaphoreSlimDictionary SemaphoreSlims = new SemaphoreSlimDictionary();
        private static readonly SemaphoreSlim WaitSemaphoreSlimInternal = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim WaitListSemaphoreSlimInternal = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim ReleaseSemaphoreSlimInternal = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim ReleaseThreadLocksSemaphoreSlimInternal = new SemaphoreSlim(1, 1);

        private static string GetSlimId(string type, string value) => string.Concat(type, "-", value);
        private static string GetSlimIdWithoutId(string type) => string.Concat(type, "-");
        public async Task<bool> WaitAsync(string type, string value, int count = 1, int millisecondsTimeout = MillisecondsTimeout)
        {
            try
            {
                await WaitSemaphoreSlimInternal.WaitAsync();

                var slimId = GetSlimId(type, value);
                if (!SemaphoreSlims.TryGetValue(slimId, out var currentSlim))
                {
                    currentSlim = new SemaphoreSlim(1, count);
                    SemaphoreSlims.TryAdd(slimId, currentSlim);
                }

                return await currentSlim.WaitAsync(millisecondsTimeout);
            }
            finally
            {
                WaitSemaphoreSlimInternal.Release();
            }
        }

        public async Task<bool> WaitAsync(string type, IEnumerable<string> values, int count = 1, int millisecondsTimeout = MillisecondsTimeout)
        {
            try
            {
                await WaitListSemaphoreSlimInternal.WaitAsync();
                var lockedItems = new List<LocketItemInfo>();

                var result = true;
                foreach (var value in values.Distinct())
                {
                    result = await WaitAsync(type, value, count, millisecondsTimeout);
                    if (result)
                    {
                        lockedItems.Add(new LocketItemInfo
                        {
                            Value = value,
                            Type = type
                        });
                    }
                    else
                    {
                        break;
                    }
                }

                if (!result)
                {
                    foreach (var lockedItem in lockedItems)
                    {
                        Release(lockedItem.Type, lockedItem.Value);
                    }
                }
                lockedItems.Clear();
                return result;
            }
            finally
            {
                WaitListSemaphoreSlimInternal.Release();
            }
        }

        public int Release(string type, string value)
        {
            try
            {
                ReleaseSemaphoreSlimInternal.Wait();

                int previousCount;
                var slimId = GetSlimId(type, value);
                if (!SemaphoreSlims.TryGetValue(slimId, out var currentSlim))
                {
                    return 0;
                }

                try
                {
                    previousCount = currentSlim.Release();
                }
                finally
                {
                    if (currentSlim.CurrentCount == 1)
                    {
                        try
                        {
                            currentSlim.Dispose();
                        }
                        finally
                        {
                            SemaphoreSlims.TryRemove(slimId, out var _);
                        }
                    }
                }

                return previousCount;
            }
            finally
            {
                ReleaseSemaphoreSlimInternal.Release();
            }
        }
        
        public int Release(string type, IEnumerable<string> values)
        {
            var totalPreviousCount = 0;
            foreach (var value in values.Distinct())
            {
                totalPreviousCount += Release(type, value);
            }

            return totalPreviousCount;
        }
        
        public void UnlockThreadLocks(string type)
        {
            try
            {
                ReleaseThreadLocksSemaphoreSlimInternal.Wait();

                var slimId = GetSlimIdWithoutId(type);
                var keysToRelease = new List<string>();
                var slimRawKeysToRelease = SemaphoreSlims.Where(p => p.Key.StartsWith(slimId)).Select(p => p.Key);

                foreach (var currentKey in slimRawKeysToRelease)
                {
                    keysToRelease.Add(currentKey.Replace(slimId, string.Empty));
                }

                Release(type, keysToRelease);
            }
            finally
            {
                ReleaseThreadLocksSemaphoreSlimInternal.Release();
            }
        }


        private sealed class SemaphoreSlimDictionary : ConcurrentDictionary<string, SemaphoreSlim>
        {
        }

        private struct LocketItemInfo
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }
}