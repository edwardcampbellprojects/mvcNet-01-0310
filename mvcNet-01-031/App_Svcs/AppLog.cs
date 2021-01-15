using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Linq;
using System.Web.Hosting;

namespace AppLogService
{
    public enum LogSeverity { Trace, Debug, Info, Warn, Error, Fatal };

    // our standard logging interface
    interface IAppLog
    {
        //parameterized log methods
        void Log(string logText, LogSeverity logSeverity);

        //mnemonic aliases, ascending severity
        void LogTrace(string traceText);        //'K
        void LogDebug(string debugText);        //Huh
        void LogInfo (string infoText);         //Oh
        void LogWarn (string warnText);         //Oops
        void LogError(string errorText);        //What
        void LogFatal(string fatalText);        //OMG
    }

    public enum LogBG
    {
        Trace = 0xD3D3D3,
        Debug = 0x90EE90,
        Info  = 0x0000FF,
        Warn  = 0xFFFF00,
        Error = 0xFF0000,
        Fatal = 0x000000
    }

    public enum LogFG
    {
        Trace = 0x062571,
        Debug = 0x062571,
        Info  = 0x062571,
        Warn  = 0x062571,
        Error = 0xFFFFFF,
        Fatal = 0xFFFF00
    }

    public static class AppLog
    {
        private static NLog.ILogger logger = LogManager.GetCurrentClassLogger();
        private static string PathToLogFile { get; set; }
        private static readonly string InternalLog_RelPath = @"\App_Data\NLog-Internal";
        private static readonly string LogFile_RelPath = @"\App_Data\Logs\AppLog";
        private static readonly string LogFile_Ext = @".log";
        private static readonly char LogStatDelimiter = '|';

        //ctor
        static AppLog()
        {
            #region AUTO_LOG_READER
#if AUTO_LOG_READER
            // Setup the logging view for Sentinel - http://sentinel.codeplex.com
            var sentinalTarget = new NLogViewerTarget()
            {
                Name = "sentinal",
                Address = "udp://127.0.0.1:9999",                          //"udp://10.9.4.156:2019", //Address = "udp://127.0.0.1:9999",
                IncludeNLogData = false
            };
            var sentinalRule = new LoggingRule("*", LogLevel.Trace, sentinalTarget);
            LogManager.Configuration.AddTarget("sentinal", sentinalTarget);
            LogManager.Configuration.LoggingRules.Add(sentinalRule);

            // Setup the logging view for Harvester - http://harvester.codeplex.com
            var harvesterTarget = new OutputDebugStringTarget()
            {
                Name = "harvester",
                Layout = "${log4jxmlevent:includeNLogData=false}"
            };
            var harvesterRule = new LoggingRule("*", LogLevel.Trace, harvesterTarget);
            LogManager.Configuration.AddTarget("harvester", harvesterTarget);
            LogManager.Configuration.LoggingRules.Add(harvesterRule);
#endif
            #endregion
            #region NLOG_CONFIGURATION
            // NLog configuration in code SUPERCEDES NLog.config
            var config = new NLog.Config.LoggingConfiguration();

            // log to calling app executable home dir/LogFile_RelPath
            string appDir = HostingEnvironment.ApplicationPhysicalPath;
            #region NLOG_INTERNAL_CONFIGURATION
            NLog.Common.InternalLogger.IncludeTimestamp = true;
            NLog.Common.InternalLogger.LogFile = $"{appDir}{InternalLog_RelPath}{LogFile_Ext}";
            NLog.Common.InternalLogger.LogLevel = LogLevel.Error;
            NLog.Common.InternalLogger.LogToConsole = true;
#if DEBUG
            NLog.Common.InternalLogger.Error("NLog-Internal Log Started, no error");
#endif
            #endregion
            #region NLOG_LOGFILE_CONFIGURATION
            PathToLogFile = $"{appDir}{LogFile_RelPath}{LogFile_Ext}";

            // define log targets
            string shortDate = DateTime.Now.ToString("MMddyyyyThh.mm.ss");

            //var logfile = new NLog.Targets.FileTarget("logfile") { FileName = PathToLogFile };
            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveFileName = $"{appDir}{LogFile_RelPath}-{shortDate}{LogFile_Ext}",                //ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                FileName = PathToLogFile,
                KeepFileOpen = false,
                Layout = "${longdate} - ${level:uppercase=true}: ${message}${onexception:${newline} EXCEPTION: [${exception:format=ToString}]}",
                //ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
                MaxArchiveFiles = 31
            };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            var sentinelNLogViewer = new NLog.Targets.NLogViewerTarget("SentinelNLogViewer") { Address = "udp://127.0.0.1:9999" };

            // add REQUIRED rule to enable logging to targets
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, logfile);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, sentinelNLogViewer);
            #endregion

            #endregion
            NLog.LogManager.Configuration = config;
        }

        // public methods
        public static void ShutDown()
        {
            NLog.LogManager.Shutdown();
        }
        public static void Log(string logText, LogSeverity logSeverity = LogSeverity.Info)
        {
            switch (logSeverity)
            {
                case LogSeverity.Trace:
                    logger.Trace(logText);
                    break;
                case LogSeverity.Debug:
                    logger.Debug(logText);
                    break;
                case LogSeverity.Info:
                    logger.Info(logText);
                    break;
                case LogSeverity.Warn:
                    logger.Warn(logText);
                    break;
                case LogSeverity.Error:
                    logger.Error(logText);
                    break;
                case LogSeverity.Fatal:
                    logger.Fatal(logText);
                    break;
                default:
                    logger.Info(logText);
                    break;
            }
        }
        public static void LogStatus(string logStatText)
        {
            if (!logStatText.Contains(LogStatDelimiter))
            {
                Log(logStatText);
            }
            else
            {
                string statusType = logStatText.Split(LogStatDelimiter)[0];
                string logText = logStatText.Split('|')[1];
                switch (statusType)
                {
                    case "Trace":
                        logger.Trace(logText);
                        break;
                    case "Debug":
                        logger.Debug(logText);
                        break;
                    case "Info":
                        logger.Info(logText);
                        break;
                    case "Warn":
                        logger.Warn(logText);
                        break;
                    case "Error":
                        logger.Error(logText);
                        break;
                    case "Fatal":
                        logger.Fatal(logText);
                        break;
                    default:
                        logger.Info(logText);
                        break;
                }
            }
        }
        public static void LogDebug(string debugText)
        {
            Log(debugText, LogSeverity.Debug);
        }

        public static void LogError(string errorText)
        {
            Log(errorText, LogSeverity.Error);
        }

        public static void LogFatal(string fatalText)
        {
            Log(fatalText, LogSeverity.Fatal);
        }

        public static void LogInfo(string infoText)
        {
            Log(infoText, LogSeverity.Info);
        }

        public static void LogTrace(string traceText)
        {
            Log(traceText, LogSeverity.Trace);
        }

        public static void LogWarn(string warnText)
        {
            Log(warnText, LogSeverity.Warn);
        }
    }
}

