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
        private const int MsciMarkerSize = 4;
        private const int MsciMarker = 0x4943534D; // "MSCI" marker Little Endian

        public long FindMarker(FileStream fileStream)
        {
            byte[] markerBytes = new byte[4];
            while (fileStream.Read(markerBytes, 0, MsciMarkerSize) == 4)
            {
                if (BitConverter.ToInt32(markerBytes, 0) == MsciMarker)
                {
                    fileStream.Seek(-MsciMarkerSize, SeekOrigin.Current);
                    return fileStream.Position;
                }
                fileStream.Seek(-2, SeekOrigin.Current);
            }
            return -1; // Marker not found
        }
    }
}
