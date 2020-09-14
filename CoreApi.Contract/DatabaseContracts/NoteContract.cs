using System;

namespace CoreApi.Contract.DatabaseContracts
{
    [Serializable]
    public class NoteContract : IContract
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        
        public string Title { get; set; }

        public string Content { get; set; }
    }
}
