namespace CoreApi.Common.Provider
{
    public class SoftDeleteProvider : ISoftDeleteProvider
    {
        private readonly bool _isSoftDeleteFilterEnabled;
        
        public SoftDeleteProvider(bool isSoftDeleteFilterEnabled)
        {
            _isSoftDeleteFilterEnabled = isSoftDeleteFilterEnabled;
        }
        
        public bool IsSoftDeleteFilterEnabled()
        {
            return _isSoftDeleteFilterEnabled;
        }
    }
}