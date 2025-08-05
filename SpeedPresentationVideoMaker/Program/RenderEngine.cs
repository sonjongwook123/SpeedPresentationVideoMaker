using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using VideoMakerTool;

public static class RenderEngine
{
    public static void ExportFinalVideo(List<Block> blocks, List<MusicTrack> musicTracks, TitleBlock titleBlock, string outputPath)
    {
        var sb = new StringBuilder("-y ");
        var filterComplex = new StringBuilder();
        var videoInputs = new List<string>();
        var audioInputs = new List<string>();
        int videoInputCount = 0;
        int audioInputCount = 0;
        int filterCount = 0;

        // Temporary directory for intermediate files
        var tempDir = Path.Combine(Path.GetDirectoryName(outputPath), "temp_video_maker");
        if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true);
        Directory.CreateDirectory(tempDir);

        // Step 1: Add title block if it exists
        if (titleBlock != null && (!string.IsNullOrEmpty(titleBlock.TeamName) || !string.IsNullOrEmpty(titleBlock.LogoPath)))
        {
            string titleVideoPath = Path.Combine(tempDir, "title.mp4");
            // Simplified title rendering - this part is complex and should be handled by a dedicated function
            sb.Append($"-f lavfi -i color=c=white:s=1280x720:d=5 "); // A 5-second white background
            videoInputs.Add($"[{(videoInputCount++)}:v]fade=t=out:st=4:d=1[title_v];");

            Process.Start("ffmpeg", $"-y -f lavfi -i color=c=white:s=1280x720:d=5 {titleVideoPath}").WaitForExit();
            sb.Append($"-i \"{titleVideoPath}\" ");
            videoInputs.Add($"[{videoInputCount}:v] [title_v];");
            videoInputCount++;
        }

        // Step 2: Create inputs and filters for video blocks
        for (int i = 0; i < blocks.Count; i++)
        {
            var b = blocks.ElementAtOrDefault(i);
            if (b is VideoPart vp)
            {
                sb.Append($"-i \"{vp.FilePath}\" ");
                videoInputs.Add($"[{videoInputCount}:v]trim=start={vp.StartTime.TotalSeconds}:end={vp.EndTime.TotalSeconds},setpts=PTS-STARTPTS,fade=t=in:st=0:d=0.5,fade=t=out:st={vp.Duration.TotalSeconds - 0.5}:d=0.5[v{filterCount}];");
                videoInputCount++;
            }
            else if (b is ImageBlock ib)
            {
                sb.Append($"-loop 1 -t {b.Duration.TotalSeconds} -i \"{ib.ImagePath}\" ");
                videoInputs.Add($"[{videoInputCount}:v]fade=t=in:st=0:d=0.5,fade=t=out:st={b.Duration.TotalSeconds - 0.5}:d=0.5[v{filterCount}];");
                videoInputCount++;
            }
            else if (b is PdfBlock pb)
            {
                var imagePath = PdfToImageConverter.ConvertPdfPageToImage(pb.PdfPath, pb.PageNumber - 1, tempDir);
                if (!string.IsNullOrEmpty(imagePath))
                {
                    sb.Append($"-loop 1 -t {b.Duration.TotalSeconds} -i \"{imagePath}\" ");
                    videoInputs.Add($"[{videoInputCount}:v]fade=t=in:st=0:d=0.5,fade=t=out:st={b.Duration.TotalSeconds - 0.5}:d=0.5[v{filterCount}];");
                    videoInputCount++;
                }
            }
            filterCount++;
        }

        // Step 3: Concatenate video filters
        if (filterCount > 0)
        {
            filterComplex.Append(string.Join("", videoInputs));
            filterComplex.Append(string.Join("", Enumerable.Range(0, filterCount).Select(i => $"[v{i}]")));
            filterComplex.Append($"concat=n={filterCount}:v=1:a=0[outv];");
        }
        else
        {
            filterComplex.Append("[title_v] [outv];");
        }

        // Step 4: Add music tracks
        if (musicTracks != null && musicTracks.Any())
        {
            foreach (var track in musicTracks)
            {
                sb.Append($"-i \"{track.FilePath}\" ");
                audioInputs.Add($"[{audioInputCount++}:a]atrim=start={track.StartTime.TotalSeconds}:end={track.EndTime.TotalSeconds},asetpts=PTS-STARTPTS[a{audioInputs.Count - 1}];");
            }
            filterComplex.Append(string.Join("", audioInputs));
            filterComplex.Append(string.Join("", Enumerable.Range(0, audioInputs.Count).Select(i => $"[a{i}]")));
            filterComplex.Append($"amix=inputs={audioInputs.Count}[outa]");
        }

        var ffmpegArgs = $"{sb} -filter_complex \"{filterComplex}\" -map \"[outv]\" {(musicTracks.Any() ? "-map \"[outa]\"" : "")} -c:v libx264 -c:a aac \"{outputPath}\"";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = ffmpegArgs,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        process.WaitForExit();
    }
}