using System;
using System.Collections.Generic;
using System.IO;

namespace SQLBakVersion.Class
{
    /// <summary>
    /// Main class for SQL Server version detection from BAK files
    /// </summary>
    public class SQLVersion
    {
        private const int TapeHeaderStart = 0x45504154; // "TAPE" in little endian
        private const int MsciMarker = 0x4943534D;      // "MSCI" in little endian

        private static readonly Dictionary<int, string> VersionMap = new Dictionary<int, string>
        {
            // SQL Server 2005
            { 611, "SQL Server 2005" },
            { 612, "SQL Server 2005" },

            // SQL Server 2008/R2
            { 655, "SQL Server 2008" },
            { 660, "SQL Server 2008" },
            { 661, "SQL Server 2008" },

            // SQL Server 2012
            { 684, "SQL Server 2012" },
            { 706, "SQL Server 2012" },

            // SQL Server 2014
            { 782, "SQL Server 2014" },

            // SQL Server 2016
            { 852, "SQL Server 2016" },

            // SQL Server 2017
            { 868, "SQL Server 2017" },
            { 869, "SQL Server 2017" },

            // SQL Server 2019
            { 895, "SQL Server 2019" },
            { 896, "SQL Server 2019" },
            { 897, "SQL Server 2019" },
            { 902, "SQL Server 2019" },
            { 904, "SQL Server 2019" },

            // SQL Server 2022
            { 950, "SQL Server 2022" },
            { 957, "SQL Server 2022" }
        };

        /// <summary>
        /// Get SQL Server version from a BAK file
        /// </summary>
        /// <param name="filePath">Path to the .bak file</param>
        /// <returns>SQL Server version or error message</returns>
        public string GetVersion(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Step 1: Validate the BAK file header
                    if (!ValidateTapeHeader(fs))
                    {
                        return "Not a valid SQL Server backup file";
                    }

                    // Step 2: Find the version marker
                    if (!FindVersionMarker(fs))
                    {
                        return "SQL version information not found";
                    }

                    // Step 3: Read the version number
                    return DetermineVersion(fs);
                }
            }
            catch (IOException ex)
            {
                return $"Cannot access file: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Validates if the file has a valid SQL BAK header
        /// </summary>
        private bool ValidateTapeHeader(FileStream fs)
        {
            byte[] headerBytes = new byte[4];
            if (fs.Read(headerBytes, 0, 4) != 4)
                return false;

            return BitConverter.ToInt32(headerBytes, 0) == TapeHeaderStart;
        }

        /// <summary>
        /// Finds the MSCI marker that precedes version info
        /// </summary>
        private bool FindVersionMarker(FileStream fs)
        {
            byte[] markerBytes = new byte[4];
            while (fs.Read(markerBytes, 0, 4) == 4)
            {
                if (BitConverter.ToInt32(markerBytes, 0) == MsciMarker)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Reads and interprets the SQL Server version number
        /// </summary>
        private string DetermineVersion(FileStream fs)
        {
            byte[] versionBytes = new byte[2];
            fs.Seek(168, SeekOrigin.Current);
            fs.Read(versionBytes, 0, 2);
            int dbVersion = BitConverter.ToInt16(versionBytes, 0);

            return VersionMap.TryGetValue(dbVersion, out string version) ? version : $"Unknown SQL Server version (code: {dbVersion})";
        }
    }
}
