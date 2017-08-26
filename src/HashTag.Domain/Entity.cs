using System;
using HashTag.Domain.Models;

namespace HashTag.Domain
{
    public abstract class Entity<TKey> : EntityBase<TKey>
    {
        public DateTime CreatedAt { get; protected set; }

        public User CreatedBy { get; protected set; }

        public DateTime? UpdatedAt { get; protected set; }

        public User UpdatedBy { get; protected set; }

        public DateTime? DeletedAt { get; protected set; }

        public User DeletedBy { get; protected set; }

        public bool IsDeleted => DeletedAt.HasValue;

        public void BeforeInsert(User user)
        {
            CreatedAt = DateTime.Now;
            CreatedBy = user;
        }

        public void BeforeUpdate(User user)
        {
            UpdatedAt = DateTime.Now;
            UpdatedBy = user;
        }

        public void MarkAsDeleted(User user)
        {
            DeletedAt = DateTime.Now;
            DeletedBy = user;
        }
    }
}