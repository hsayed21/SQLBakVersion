using System;
using System.IO;

namespace SQLBakVersion.Class
{
    public class SQLVersion
    {
        private readonly ITapeHeaderValidator _tapeHeaderValidator;
        private readonly IMarkerFinder _markerFinder;
        private readonly IVersionDeterminer _versionDeterminer;

        public SQLVersion()
        {
            // Initialize dependencies
            _tapeHeaderValidator = new TapeHeaderValidator();
            _markerFinder = new MarkerFinder();
            _versionDeterminer = new VersionDeterminer();
        }

        public string GetVersion(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    if (!_tapeHeaderValidator.Validate(fs))
                    {
                        return "Invalid Tape Header";
                    }

                    if (!_markerFinder.FindMarker(fs))
                    {
                        return "MSCI marker not found";
                    }

                    return _versionDeterminer.DetermineVersion(fs);
                }
            }
            catch (Exception ex)
            {
                return $"Error reading file: {ex.Message}";
            }
        }
    }
}
