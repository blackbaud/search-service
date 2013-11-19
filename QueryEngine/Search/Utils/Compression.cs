using System.IO;
using System.IO.Compression;

namespace Search.Utils
{
    public class Compression
    {
        public static byte[] Compress(byte[] bytes)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var shrinker = new DeflateStream(outputStream, CompressionMode.Compress))
                {
                    shrinker.Write(bytes, 0, bytes.Length);
                    shrinker.Close();
                    //Console.WriteLine("Array Length - {0}", outputStream.ToArray().Length);
                    return outputStream.ToArray();
                }
            }
        }

        public static Stream Decompress(byte[] bytes)
        {
            var output = new MemoryStream();
            using (var compressedStream = new MemoryStream(bytes))
            {
                using (var inflater = new DeflateStream(compressedStream, CompressionMode.Decompress))
                {
                    inflater.CopyTo(output);
                    inflater.Close();
                    output.Position = 0;
                    return output;
                }
            }

        }
    }
}
