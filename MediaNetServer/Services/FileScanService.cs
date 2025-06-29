using Emby.Naming.TV;

namespace MediaNetServer.Services;

using System.IO;
using Emby.Naming.Common;
using Emby.Naming.Video;
using Emby.Naming.TV;
using Microsoft.Extensions.Logging;

public class FileScanService
{
     public VideoFileInfo? ParseMovieFile(string path)
     {
          var file = new FileInfo(path);
          var option = new NamingOptions();
          if (VideoResolver.IsVideoFile(path: path, namingOptions: option))
          {
               if (path.StartsWith(@"movie\"))
               {
                    return VideoResolver.ResolveFile(path: path, namingOptions: option);
               }
          }
          return null;
     }
}