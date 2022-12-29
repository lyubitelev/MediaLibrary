using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace MediaStream.Extensions
{
    public static class FFmpegExtensions
    {
        private const string ToolsFolderName = "Tools";

        public static void DownloadFFmpeg()
        {
            var path = Path.Combine(AppContext.BaseDirectory, ToolsFolderName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            FFmpeg.SetExecutablesPath(path);

            // ToDo you need to add logging and verification for downloading
            FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official, path)
                            .Wait();
        }
    }
}
