using Newtonsoft.Json;
using NLog;
using ServiceUtils.Exeptions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Serialization;

namespace ServiceUtils
{
    public class CacheProvider
    {
        private byte[] _key;
        private ILogger _logger;

        public CacheProvider(
            byte[] key,
            ILogger logger)
        {
            _key = key;
            _logger = logger;
        }

        public byte[]? CacheDatabaseConnection(DatabaseConnectionInfo connections)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(DatabaseConnectionInfo));
                using MemoryStream memoryStream = new MemoryStream();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                xmlSerializer.Serialize(xmlTextWriter, connections);
                return ProtectData(memoryStream.ToArray());
            }
            catch (Exception ex)
            {
                if (_logger != null) _logger.Error($"Error data protect. Message: {ex.Message}");
                throw new CacheProviderSerializeException();
            }
        }

        public DatabaseConnectionInfo GetDatabaseConnectionFromCahce(byte[] cache)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(DatabaseConnectionInfo));
                byte[] unprotectedData = UnProtectData(cache);
                using MemoryStream memoryStream = new MemoryStream(unprotectedData);
                XmlTextReader xmlTextReader = new XmlTextReader(memoryStream);
                return (DatabaseConnectionInfo)xmlSerializer.Deserialize(xmlTextReader);

            }
            catch (Exception ex)
            {
                if (_logger != null) _logger.Error($"Error data unprotect. Message: {ex.Message}");
                throw new CacheProviderDeserializeException();
            }
        }

        private byte[]? ProtectData(byte[] data)
        {
            try
            {
                return ProtectedData.Protect(data, _key, DataProtectionScope.CurrentUser);
            }
            catch (Exception ex)
            {
                if (_logger != null) _logger.Error($"Error protect data. Message: {ex.Message}");
                throw new CacheProviderProtectDataException();
            }
        }

        private byte[]? UnProtectData(byte[] data)
        {
            try
            {
                return ProtectedData.Unprotect(data, _key, DataProtectionScope.CurrentUser);
            }
            catch (Exception ex)
            {
                if (_logger != null) _logger.Error($"Error unprotect data. Message: {ex.Message}");
                throw new CacheProviderUnProtectDataException();
            }
        }
    }
}