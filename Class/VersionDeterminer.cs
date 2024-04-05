using System;
using System.IO;

namespace SQLBakVersion.Class
{
    public interface IVersionDeterminer
    {
        string DetermineVersion(FileStream fileStream, long MSCIOffset);
    }

    public class VersionDeterminer : IVersionDeterminer
    {

        public string DetermineVersion(FileStream fileStream, long MSCIOffset)
        {
            long versionOffset = MSCIOffset + 172;
            byte[] versionBytes = new byte[2];

            fileStream.Seek(versionOffset, SeekOrigin.Begin);
            fileStream.Read(versionBytes, 0, 2);
            Int16 dbVersion = BitConverter.ToInt16(versionBytes, 0);

            switch (dbVersion)
            {
                case 611:
                case 612:
                    return "SQL Server 2005";
                case 655:
                case 660:
                case 661:
                    return "SQL Server 2008";
                case 684:
                case 706:
                    return "SQL Server 2012";
                case 782:
                    return "SQL Server 2014";
                case 852:
                    return "SQL Server 2016";
                case 868:
                case 869:
                    return "SQL Server 2017";
                case 895:
                case 896:
                case 897:
                case 902:
                case 904:
                    return "SQL Server 2019";
                case 950:
                case 957:
                    return "SQL Server 2022";
                default:
                    return "Unknown version";
            }
        }
    }
}
