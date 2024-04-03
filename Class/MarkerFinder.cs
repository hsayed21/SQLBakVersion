using System;
using System.IO;

namespace SQLBakVersion.Class
{
    public interface IMarkerFinder
    {
        long FindMarker(FileStream fileStream);
    }

    public class MarkerFinder : IMarkerFinder
    {
        private const string MsciMarker = "4D-53-43-49"; // "MSCI" marker

        public long FindMarker(FileStream fileStream)
        {
            byte[] markerBytes = new byte[4];
            while (fileStream.Read(markerBytes, 0, 4) == 4)
            {
                string hexValue = BitConverter.ToString(markerBytes);
                if (hexValue == MsciMarker)
                {
                    return fileStream.Position - 4;
                }
                fileStream.Seek(-2, SeekOrigin.Current);
            }
            return -1; // Marker not found
        }
    }
}
