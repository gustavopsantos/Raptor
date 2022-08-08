using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Raptor.Serialization
{
    public static class BinarySerializer
    {
        private static readonly BinaryFormatter _binaryFormatter = new();

        public static byte[] Serialize<T>(T obj)
        {
            using var stream = new MemoryStream();
            _binaryFormatter.Serialize(stream, obj);
            return stream.ToArray();
        }

        public static object Deserialize(byte[] bytes)
        {
            using var stream = new MemoryStream(bytes);
            return _binaryFormatter.Deserialize(stream);
        }
    }
}