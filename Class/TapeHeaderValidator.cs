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
        private const string TapeHeaderStart = "54-41-50-45"; // Start of tape header "TAPE"

        public bool Validate(FileStream fileStream)
        {
            byte[] tapeHeaderStartBytes = new byte[4];
            fileStream.Read(tapeHeaderStartBytes, 0, 4);
            string hexValue = BitConverter.ToString(tapeHeaderStartBytes);
            return hexValue == TapeHeaderStart;
        }
    }
}
