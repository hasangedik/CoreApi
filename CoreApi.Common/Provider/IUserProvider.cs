namespace CoreApi.Common.Provider
{
    public interface IUserProvider
    {
        int GetUserId();
        bool IsUserFilterEnabled();
    }
}