using System;
using System.ComponentModel.DataAnnotations;
using CoreApi.Common.Attributes;

namespace CoreApi.Entity.Models
{
    public class User : IEntity, ISoftDelete, IHasCreatedAt
    {
        [Key]
        public int Id { get; set; }
        
        [Encrypted]
        public string Name { get; set; }
        
        [Encrypted]
        public string Email { get; set; }
        
        public string Password { get; set; }

        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}
