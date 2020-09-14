using CoreApi.Common.Extensions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoreApi.Infrastructure
{
    internal sealed class EncryptionConverter : ValueConverter<string, string>
    {
        private const string SecurityKey = "tQ&146eH@Za1t0sa@!q?";
        /// <summary>
        /// Creates a new <see cref="EncryptionConverter"/> instance.
        /// </summary>
        /// <param name="mappingHints">Entity Framework mapping hints</param>
        public EncryptionConverter(ConverterMappingHints mappingHints = null) 
            : base(x => x.Encrypt(SecurityKey, true), x => x.Decrypt(SecurityKey,true), mappingHints)
        {
        }
    }
}