using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Net;
using System.Collections.Specialized;

namespace TeslaMergeWin
{
    public class MergeThread
    {
        public MergeThread(TeslaMergeConfig config)
        {
            _config = config;
        }

        public void RunOnce()
        {
            loopForTriggerFile(true);
        }

        /// <summary>
        /// Starts the MergeThread
        /// </summary>
        public void Start()
        {
            _stop = false;

            _thread = new Thread(new ParameterizedThreadStart(execute));
            _thread.Start(this);
        }

        /// <summary>
        /// Stops the MergeThread
        /// </summary>
        public void Stop()
        {
            _stop = true;

            _thread?.Join();
        }

        private static void execute(object obj)
        {
            MergeThread merge = (MergeThread)obj;

            merge.loopForTriggerFile();
        }

        /// <summary>
        /// Waits for the presence of a trigger file
        /// </summary>
        private void loopForTriggerFile(bool runOnce = false)
        {
            if (runOnce)
            {
                runTeslaMerge();
            }
            else
            {
                while (!_stop)
                {

                    if (File.Exists(_config.TriggerFile))
                    {
                        runTeslaMerge();

                        File.Delete(_config.TriggerFile);
                    }
                    else
                    {
                        Thread.Sleep(5000);
                    }
                }
            }
        }

        /// <summary>
        /// Loops through dashcam directory to process videos
        /// </summary>
        private void runTeslaMerge()
        {
            int counter = 0;
            Stopwatch timer = Stopwatch.StartNew();
            foreach (var file in Directory.EnumerateFiles(_config.DashCamFolder, "*-front.mp4", SearchOption.AllDirectories))
            {
                if (_stop)
                {
                    break;
                }

                if (processFile(file))
                {
                    counter++;
                }
            }
            timer.Stop();

            int totalSeconds = (int)timer.ElapsedMilliseconds / 1000;

            string message = $"{counter} incidents in {getPrettyTime(totalSeconds)} Average {(timer.ElapsedMilliseconds / 1000 / counter)}s";
            Debug.WriteLine(message);

            sendPushover(message);
        }

        private string getPrettyTime(int totalTimeInSeconds)
        {
            int hours = totalTimeInSeconds / 60 / 60 % 24;
            int minutes = totalTimeInSeconds / 60 % 60;
            int seconds = totalTimeInSeconds % 60;

            string results = string.Empty;
            if (hours > 0)
            {
                results = hours + "h";
            }
            if (minutes > 0)
            {
                results += minutes + "m";
            }
            if (seconds > 0)
            {
                results += seconds + "s";
            }

            return results.Trim();
        }

        /// <summary>
        /// Preps all filename variables for ffmpeg
        /// </summary>
        private bool processFile(string frontCam)
        {
            Stopwatch currentRender = Stopwatch.StartNew();

            string directory = frontCam.Substring(0, frontCam.LastIndexOf('\\'));

            string fileName = frontCam.Substring(frontCam.LastIndexOf('\\') + 1);
            string prefix = fileName.Substring(0, fileName.LastIndexOf('-'));

            string doneFile = Path.Combine(directory, prefix + "-done");

            if (File.Exists(doneFile))
            {
                return false;
            }

            string leftCam = Path.Combine(directory, prefix + "-left_repeater.mp4");
            string rightCam = Path.Combine(directory, prefix + "-right_repeater.mp4");
            string outputFile = Path.Combine(directory, prefix + "-combined.mp4");
            string previewFile = Path.Combine(directory, prefix + "-preview.mp4");
            string timelapseFile = Path.Combine(directory, prefix + "-timelapse.mp4");

            int year = int.Parse(prefix.Substring(0, 4));
            int month = int.Parse(prefix.Substring(5, 2));
            int day = int.Parse(prefix.Substring(8, 2));
            int hour = int.Parse(prefix.Substring(11, 2));
            int minute = int.Parse(prefix.Substring(14, 2));

            DateTime t = new DateTime(year, month, day, hour, minute, 0);

            string date = t.ToString(_config.DateFormat).Replace(":",@"\:");

            createVideo(_config.CombinedFileArguments
                        .Replace("$RIGHTCAM", rightCam)
                        .Replace("$FRONTCAM", frontCam)
                        .Replace("$LEFTCAM", leftCam)
                        .Replace("$OUTPUTFILE", outputFile)
                        .Replace("$DATE", date));

            if (isFileOk(outputFile))
            {
                createVideo(_config.PreviewFileArguments
                        .Replace("$OUTPUTFILE", outputFile)
                        .Replace("$PREVIEWFILE", previewFile)
                        .Replace("$DATE", date));

                int time = getTimeInSeconds(previewFile);

                if (time > 40)
                {
                    createVideo(_config.TimelapseFileArguments
                                    .Replace("$PREVIEWFILE", previewFile)
                                    .Replace("$TIMELAPSEFILE", timelapseFile)
                                    .Replace("$DATE", date));
                }
                else
                {
                    File.Copy(previewFile, timelapseFile, true);
                }

                if (_config.DeleteOriginals)
                {
                    File.Delete(leftCam);
                    File.Delete(rightCam);
                    File.Delete(frontCam);
                }
                else
                {
                    using (File.Create(doneFile)) ;
                }
            }

            currentRender.Stop();
            Debug.WriteLine($"{prefix} done in {currentRender.ElapsedMilliseconds / 1000}s");

            return true;
        }

        /// <summary>
        /// Verifies a file both exists and is not empty
        /// </summary>
        private bool isFileOk(string filePath)
        {
            FileInfo file = new FileInfo(filePath);

            if (file.Exists && file.Length > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Executs ffmpeg 
        /// </summary>
        private void createVideo(string arguments)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = Path.Combine(_config.ffmpegBinaryFolder, "ffmpeg.exe");
            p.StartInfo.Arguments = arguments;
            p.Start();
            p.WaitForExit();
        }

        /// <summary>
        /// Executes ffprobe to get duration of video in seconds
        /// </summary>
        private int getTimeInSeconds(string fileName)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = Path.Combine(_config.ffmpegBinaryFolder, "ffprobe.exe");
            p.StartInfo.Arguments = fileName;

            p.Start();

            string output = p.StandardError.ReadToEnd();

            p.WaitForExit();

            string temp = output.Substring(output.IndexOf("Duration") + 10);
            temp = temp.Substring(0, temp.IndexOf(','));

            TimeSpan ts = TimeSpan.Parse(temp);

            return (int)ts.TotalSeconds;
        }

        /// <summary>
        /// Sends Pushover Message via WebClient
        /// </summary>
        private void sendPushover(string message)
        {
            if (string.IsNullOrWhiteSpace(_config.PushOverApplication) || string.IsNullOrWhiteSpace(_config.PushOverUser))
            {
                return;
            }

            var parameters = new NameValueCollection {
                { "token", "aqcdhp11ugmgnkh4wegxjfmhe32y9n" },
                { "user", "uvhtsz1w1zqzacx5y5mgwaxn262zjj" },
                { "message", message }
            };

            getClient().UploadValues("https://api.pushover.net/1/messages.json", parameters);
        }

        /// <summary>
        /// Used to recycle client to prevent Denial-of-service through too many WebClient creations.
        /// </summary>
        /// <returns></returns>
        private WebClient getClient()
        {
            if (_client == null)
            {
                _client = new WebClient();
            }

            return _client;
        }

        private bool _stop = false;
        private readonly TeslaMergeConfig _config;
        private WebClient _client = new WebClient();

        private Thread _thread;
    }
}
