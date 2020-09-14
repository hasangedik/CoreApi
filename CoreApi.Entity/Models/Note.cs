using System;
using System.ComponentModel.DataAnnotations;
using CoreApi.Common.Attributes;

namespace CoreApi.Entity.Models
{
    public class Note : IEntity, ISoftDelete, IHasCreatedAt, IHasUserId
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }

        [Encrypted]
        public string Title { get; set; }

        [Encrypted]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}
