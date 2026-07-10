using System;
using System.Collections.Generic;
using System.IO;

namespace SQLBakVersion.Class
{
    /// <summary>
    /// Version values stored in an SQL Server MDF boot page.
    /// </summary>
    public sealed class MdfVersion
    {
        public static readonly MdfVersion Unknown = new MdfVersion(null, null, null, null);

        public MdfVersion(int? year, int? internalVersion, int? createVersion, string failureReason)
        {
            Year = year;
            InternalVersion = internalVersion;
            CreateVersion = createVersion;
            FailureReason = failureReason;
        }

        public int? Year { get; private set; }
        public int? InternalVersion { get; private set; }
        public int? CreateVersion { get; private set; }
        public string FailureReason { get; private set; }

        public static MdfVersion Unavailable(string failureReason)
        {
            return new MdfVersion(null, null, null, failureReason);
        }
    }

    /// <summary>
    /// Detects the SQL Server version of an MDF file from its boot page.
    /// </summary>
    public static class MdfVersionDetector
    {
        private const int PageSize = 8192;
        private const int BootPageNumber = 9;
        private const int PageHeaderSize = 96;
        private const int DbiVersionOffset = 4;
        private const int DbiCreateVersionOffset = 6;
        private const long DbiVersionFileOffset = (long)BootPageNumber * PageSize + PageHeaderSize + DbiVersionOffset;
        private const long DbiCreateVersionFileOffset = (long)BootPageNumber * PageSize + PageHeaderSize + DbiCreateVersionOffset;

        private static readonly IDictionary<int, int> VersionYears = new Dictionary<int, int>
        {
            { 539, 2000 },
            { 611, 2005 },
            { 612, 2005 },
            { 655, 2008 },
            { 660, 2008 },
            { 661, 2008 },
            { 684, 2012 },
            { 706, 2012 },
            { 782, 2014 },
            { 852, 2016 },
            { 868, 2017 },
            { 869, 2017 },
            { 895, 2019 },
            { 896, 2019 },
            { 897, 2019 },
            { 902, 2019 },
            { 904, 2019 },
            { 950, 2022 },
            { 957, 2022 }
        };

        public static MdfVersion Detect(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return MdfVersion.Unavailable("No MDF file was selected.");
            }

            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (stream.Length < DbiCreateVersionFileOffset + sizeof(ushort))
                    {
                        return MdfVersion.Unavailable("The file is too small to contain the MDF boot page.");
                    }

                    stream.Position = DbiVersionFileOffset;
                    byte[] bytes = new byte[sizeof(ushort) * 2];
                    if (!ReadExactly(stream, bytes))
                    {
                        return MdfVersion.Unavailable("The MDF boot page could not be read completely.");
                    }

                    int internalVersion = ToUInt16LittleEndian(bytes, 0);
                    int createVersion = ToUInt16LittleEndian(bytes, sizeof(ushort));
                    int year;

                    return VersionYears.TryGetValue(internalVersion, out year)
                        ? new MdfVersion(year, internalVersion, createVersion, null)
                        : new MdfVersion(null, internalVersion, createVersion, null);
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                return MdfVersion.Unavailable("The MDF file cannot be accessed: " + exception.Message);
            }
            catch (IOException exception)
            {
                return MdfVersion.Unavailable("The MDF file cannot be read. If it is attached to SQL Server, stop the service or use a copy of the file. " + exception.Message);
            }
            catch (Exception exception)
            {
                return MdfVersion.Unavailable("Unable to read the MDF boot page: " + exception.Message);
            }
        }

        private static bool ReadExactly(Stream stream, byte[] bytes)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < bytes.Length)
            {
                int bytesRead = stream.Read(bytes, totalBytesRead, bytes.Length - totalBytesRead);
                if (bytesRead == 0)
                {
                    return false;
                }

                totalBytesRead += bytesRead;
            }

            return true;
        }

        private static int ToUInt16LittleEndian(byte[] bytes, int offset)
        {
            return bytes[offset] | (bytes[offset + 1] << 8);
        }
    }
}
