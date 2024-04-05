using System;
using System.IO;

namespace SQLBakVersion.Class
{
    public interface ITapeHeaderValidator
    {
        bool Validate(FileStream fileStream);
    }

    public class TapeHeaderValidator : ITapeHeaderValidator
    {
        private const int TapeHeaderSize = 4;
        private const int TapeHeaderStart = 0x45504154; // Start of tape header "TAPE" Little Endian
        public bool Validate(FileStream fileStream)
        {
            byte[] tapeHeaderStartBytes = new byte[TapeHeaderSize];

            if (fileStream.Read(tapeHeaderStartBytes, 0, TapeHeaderSize) != TapeHeaderSize)
                return false;  // Not enough data in the file
            
            return BitConverter.ToInt32(tapeHeaderStartBytes, 0) == TapeHeaderStart;
        }
    }
}
