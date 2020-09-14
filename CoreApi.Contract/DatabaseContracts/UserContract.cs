using System;

namespace CoreApi.Contract.DatabaseContracts
{
    [Serializable]
    public class UserContract : IContract
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Email { get; set; }
        
        public string Password { get; set; }
    }
}
