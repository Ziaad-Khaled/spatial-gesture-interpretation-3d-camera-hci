using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    internal class WritingOnDisk
    {
        
        public static void WriteFile<T>(T obj, string path)
        {
            System.Diagnostics.Debug.WriteLine(path);
            using (FileStream fs = File.Create(path))
            using (GZipStream gzip = new GZipStream(fs, CompressionMode.Compress))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(gzip, obj);
            }
        }
        

        public static void WriteToFile(string path, string gesture)
        {
            File.AppendAllText(path ,gesture + Environment.NewLine);
        }

        
        public static T LoadFile<T>(string path)
        {
            System.Diagnostics.Debug.WriteLine(path);
            using (FileStream fs = File.OpenRead(path))
            using (GZipStream gzip = new GZipStream(fs, CompressionMode.Decompress))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (T)bf.Deserialize(gzip);
            }
        }
        
    }
}
