using CoreApi.Common.Extensions;
using Microsoft.AspNetCore.Http;

namespace CoreApi.Common.Provider
{
    public class UserProvider : IUserProvider
    {
        private readonly bool _isUserFilterEnabled;
        private readonly IHttpContextAccessor _accessor;
        
        public UserProvider(IHttpContextAccessor accessor, bool isUserFilterEnabled)
        {
            _isUserFilterEnabled = isUserFilterEnabled;
            _accessor = accessor;
        }
        public int GetUserId()
        {
            if(_accessor != null)
                return _isUserFilterEnabled ? _accessor.HttpContext.User.Identity.GetId() : default;
            
            return default;
        }

        public bool IsUserFilterEnabled()
        {
            return _isUserFilterEnabled;
        }
    }
}