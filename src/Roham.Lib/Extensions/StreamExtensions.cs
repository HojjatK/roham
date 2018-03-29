namespace System.IO
{
    public static class StreamExtensions
    {
        public static void Save(this Stream stream, string fileName)
        {
            var directoryName = Path.GetDirectoryName(fileName);
            Directory.CreateDirectory(directoryName);

            using (var output = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                var size = 2048;
                var data = new byte[size];
                while (size > 0)
                {
                    size = stream.Read(data, 0, data.Length);
                    if (size > 0) output.Write(data, 0, size);
                }
            }
        }
    }
}
