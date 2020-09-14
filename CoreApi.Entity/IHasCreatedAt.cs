using System;

namespace CoreApi.Entity
{
    public interface IHasCreatedAt
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}