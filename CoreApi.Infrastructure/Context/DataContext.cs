using System.Linq;
using CoreApi.Common.Attributes;
using CoreApi.Common.Extensions;
using CoreApi.Common.Provider;
using CoreApi.Entity;
using CoreApi.Entity.Models;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace CoreApi.Infrastructure.Context
{
    [UsedImplicitly]
    public class DataContext : DbContext
    {
        private readonly int _userId;
        private readonly IUserProvider _userProvider;
        private readonly ISoftDeleteProvider _softDeleteProvider;

        public DataContext(DbContextOptions<DataContext> options, ISoftDeleteProvider softDeleteProvider, IUserProvider userProvider) : base(options)
        {
            _userProvider = userProvider;
            _userId = _userProvider.GetUserId();
            _softDeleteProvider = softDeleteProvider;
        }

        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Note> Note { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            #region Soft Delete Filter

            if (_softDeleteProvider == null || _softDeleteProvider.IsSoftDeleteFilterEnabled())
            {
                var typesSoftDelete = modelBuilder.Model.GetEntityTypes()
                    .Where(e => typeof(ISoftDelete).IsAssignableFrom(e.ClrType));

                foreach (var entityType in typesSoftDelete)
                {
                    var isActiveProperty = entityType.FindProperty("IsDeleted");
                    if (isActiveProperty == null || isActiveProperty.ClrType != typeof(bool))
                        continue;
                    
                    modelBuilder.Entity(entityType.ClrType).AddQueryFilter<ISoftDelete>(e => !e.IsDeleted);
                }
            }

            #endregion
            
            #region UserId Filter

            if (_userProvider != null && _userProvider.IsUserFilterEnabled())
            {
                var typesClient = modelBuilder.Model.GetEntityTypes()
                    .Where(e => typeof(IHasUserId).IsAssignableFrom(e.ClrType) );

                foreach (var entityType in typesClient)
                {
                    var userIdProperty = entityType.FindProperty("UserId");
                    if (userIdProperty == null || userIdProperty.ClrType != typeof(int))
                        continue;

                    modelBuilder.Entity(entityType.ClrType).AddQueryFilter<IHasUserId>(e => e.UserId == _userId);
                }
            }

            #endregion

            #region Encryption
            
            var encryptionConverter = new EncryptionConverter();
            
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    var attributes = property.PropertyInfo.GetCustomAttributes(typeof(EncryptedAttribute), false);
                    if (!attributes.Any()) continue;
                    property.SetValueConverter(encryptionConverter);
                }
            }
            
            #endregion
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }
    }
}