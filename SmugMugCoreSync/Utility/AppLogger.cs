using System.Diagnostics;
using SmugMugCoreSync.Configuration;

namespace SmugMugCoreSync.Utility
{
    /// <summary>
    /// Object used for reporting errors for self-running applications
    /// </summary>
    public class AppLogger : TraceListener
    {
        private string _automatedAppName = "[unknown]";
        private string _logFolder = string.Empty;
        private int _filePurgeCount = -1;
        private string _logFileName = string.Empty;
        private string _logFileNameBase = string.Empty;
        private readonly object obj = new();
        private DateTime _timeStart;
        private bool _lineStart = true;
        private LoggingConfig _loggingConfig;

        /// <summary>
        /// Provide a log file suffix
        /// </summary>
        public AppLogger(LoggingConfig loggingConfig)
        {
            _loggingConfig = loggingConfig;
        }

        /// <summary>
        /// Sets up the log for the particular thread.
        /// </summary>
        public void SetupAppLog(string logSuffix = "")
        {
            // Record the name of the app which setup this error handler manager
            var sf = new StackFrame(1);
            var method = sf.GetMethod();
            if (method != null)
            {
                _automatedAppName = method.DeclaringType?.Assembly.GetName().Name ?? "[unknown]";
            }
            _timeStart = DateTime.Now;

            LoadSettingValues();

            // Purge oldest if we have too many files
            PurgeLocation(_logFolder, _filePurgeCount);

            // Appends an underscore for formatting
            if (logSuffix.Length > 0)
                logSuffix += "_";

            // Log Name
            _logFileNameBase = _logFolder + @"\" +
                _automatedAppName + "_" + logSuffix +
                DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss");
            _logFileName = _logFileNameBase + ".txt";

            // Setup the event hooks to the LogBook
            if (!Debugger.IsAttached)
            {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            }
            Trace.Listeners.Add(this);

            File.AppendAllText(_logFileName, Environment.NewLine);
            Trace.WriteLine("******************************************");
            Trace.WriteLine("** Application Started **");
            Trace.WriteLine("** " + AppDomain.CurrentDomain.FriendlyName.ToString() + " " + System.Reflection.Assembly.GetExecutingAssembly().GetName()?.Version?.ToString());
            Trace.WriteLine("** Command Line: " + Environment.CommandLine);
        }

        /// <summary>
        /// Record Exceptions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            lock (obj)
            {
                Trace.WriteLine("*** EXCEPTION DETECTED ***");

                string extendedDetails = string.Empty;
                if (e.ExceptionObject is SmugMugCore.Net.Core20.SmugMugException smugMugException)
                {
                    var paramData = smugMugException.RequestObject.Parameters;
                    if (paramData != null)
                    {
                        foreach (var keyPair in paramData)
                        {
                            extendedDetails += $" {keyPair.Name} : {keyPair.Value} \r\n";
                        }

                        if (paramData.Count > 0)
                        {
                            extendedDetails = "\r\n\r\nExtended Details: " + extendedDetails;
                        }
                    }

                    extendedDetails += $"\r\nDetails (if available): {smugMugException.ErrorCode} : {smugMugException.ErrorMessage}";
                    extendedDetails += $"\r\nResource: {smugMugException.RequestObject.Resource}";
                }

                Trace.WriteLine(Environment.NewLine + e.ExceptionObject.ToString() + extendedDetails);

                // Open the log file up
                bool doOpenLogFileOnError = _loggingConfig.OpenLogOnFailure;
                if (doOpenLogFileOnError)
                    OpenLogFile();
            }

            // Exit the application
            Environment.Exit(-1);
        }

        /// <summary>
        /// Mark that this application has ended
        /// </summary>
        public void FinishAppLog()
        {
            Trace.WriteLine("** Duration: " + DateTime.Now.Subtract(_timeStart).ToString());
            Trace.WriteLine("** Application Finished **");
            Trace.WriteLine("******************************************");
        }

        /// <summary>
        /// Load the values needed for this from the config
        /// </summary>
        private void LoadSettingValues()
        {
            // Create the directory if it doesn't exist
            _logFolder = Environment.CurrentDirectory + @"\Logs";
            if (!Directory.Exists(_logFolder))
                Directory.CreateDirectory(_logFolder);

            //
            // Set the purge count, if not in config, then set it to 30.
            //
            var settingFilePurgeCount = _loggingConfig.FilePurgeCount;
            if (!int.TryParse(settingFilePurgeCount.ToString(), out _filePurgeCount))
                throw new Exception("File Purge Count is not valid.");
        }

        /// <summary>
        /// Write the message to the log file
        /// </summary>
        /// <param name="logMessage"></param>
        public void WriteLog(string? logMessage)
        {
            if (logMessage == null)
                return;

            string logText;
            if (_lineStart)
            {
                logText = DateTime.Now.ToString("u") + new string('\t', Trace.IndentLevel + 1) + logMessage;
            }
            else
            {
                logText = logMessage;
            }

            lock (obj)
            {
                File.AppendAllText(_logFileName, logText);
            }
            Console.Write(logText);
        }

        /// <summary>
        /// Purge the files over the threshold
        /// </summary>
        /// <param name="pathName"></param>
        /// <param name="filePurgeCount"></param>
        private static void PurgeLocation(string pathName, int filePurgeCount)
        {
            string[] fileList = Directory.GetFiles(pathName);
            if (fileList.Length > filePurgeCount)
            {
                int finalIndex = fileList.Length - filePurgeCount;
                for (int i = 0; i < finalIndex; i++)
                {
                    File.Delete(fileList[i]);
                }
            }
        }

        /// <summary>
        /// Displays the log file in the selected editor
        /// </summary>
        public void OpenLogFile()
        {
            string logFileEditor = _loggingConfig.LogFileEditorPath;
            var startInfo = new ProcessStartInfo()
            {
                FileName = logFileEditor,
                Arguments = @"""" + _logFileName + @""""
            };
            Process.Start(startInfo);
        }

        /// <summary>
        /// Writes the line from the trace listener
        /// </summary>
        /// <param name="message"></param>
        public override void Write(string? message)
        {
            WriteLog(message);
            _lineStart = false;
        }

        /// <summary>
        /// Writes the line from the trace listener
        /// </summary>
        /// <param name="message"></param>
        public override void WriteLine(string? message)
        {
            WriteLog(message + Environment.NewLine);
            _lineStart = true;
        }
    }
}
