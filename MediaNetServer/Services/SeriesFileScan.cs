using System.IO;
using Emby.Naming.Common;
using Emby.Naming.Video;
using Emby.Naming.TV;

namespace MediaNetServer.Services;

public class SeriesFileScan
{
    public SeriesInfo? ParseSeriesFile(string path)
    {
        var file = new FileInfo(path);
        var option = new NamingOptions();
        if (VideoResolver.IsVideoFile(path: path, namingOptions: option))
        {
            if (path.StartsWith(@"series\"))
            {
                return SeriesResolver.Resolve(options: option, path: path);
            }
        }
        return null;
    }
}