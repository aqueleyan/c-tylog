using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Logger
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Fatal
    }

    public sealed class Logger : IDisposable
    {
        private static readonly Lazy<Logger> lazyInstance = new Lazy<Logger>(() => new Logger());
        public static Logger Instance => lazyInstance.Value;
        private static LogLevel _configuredMinLevel = LogLevel.Debug;
        private static string _configuredLogName = string.Empty;
        private static bool _newSessionOnExistingFile;
        private readonly BlockingCollection<(DateTime, LogLevel, string)> _logQueue;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly Task _loggingTask;
        private readonly string _mainLogFilePath = "./logs/app.log";
        private readonly string _errorLogFilePath = "./logs/errors.log";
        private readonly string _fatalLogFilePath = "./logs/fatal.log";
        private readonly LogLevel _minLogLevel;
        private readonly string _logName;
        private readonly bool _sessionOnExistingFile;
        private readonly TimeZoneInfo _timeZone = TimeZoneInfo.Local;
        private bool _mainPrefixDone;
        private bool _errorPrefixDone;
        private bool _fatalPrefixDone;
        private bool _mainFileExisted;
        private bool _errorFileExisted;
        private bool _fatalFileExisted;

        private Logger()
        {
            _logQueue = new BlockingCollection<(DateTime, LogLevel, string)>(1000);
            _cancellationTokenSource = new CancellationTokenSource();
            _minLogLevel = _configuredMinLevel;
            _logName = _configuredLogName;
            _sessionOnExistingFile = _newSessionOnExistingFile;
            _loggingTask = Task.Factory.StartNew(() => ProcessLogQueue(_cancellationTokenSource.Token), TaskCreationOptions.LongRunning);
        }

        public static void Configure(LogLevel minLogLevel, string? logName = null, bool newSessionOnExistingFile = false)
        {
            _configuredMinLevel = minLogLevel;
            _configuredLogName = logName ?? string.Empty;
            _newSessionOnExistingFile = newSessionOnExistingFile;
        }

        public void Log(LogLevel level, string message)
        {
            if (level < _minLogLevel) return;
            try { _logQueue.Add((DateTime.UtcNow, level, message)); } catch (Exception ex) { Console.Error.WriteLine($"Erro ao adicionar log na fila: {ex}"); }
        }

        public void Debug(string message) => Log(LogLevel.Debug, message);
        public void Info(string message) => Log(LogLevel.Info, message);
        public void Warning(string message) => Log(LogLevel.Warning, message);
        public void Error(string message) => Log(LogLevel.Error, message);
        public void Fatal(string message) => Log(LogLevel.Fatal, message);

        private void ProcessLogQueue(CancellationToken token)
        {
            try
            {
                _mainFileExisted = File.Exists(_mainLogFilePath);
                _errorFileExisted = File.Exists(_errorLogFilePath);
                _fatalFileExisted = File.Exists(_fatalLogFilePath);
                CreateDir(_mainLogFilePath);
                CreateDir(_errorLogFilePath);
                CreateDir(_fatalLogFilePath);
                using var mainStream = new FileStream(_mainLogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                using var mainWriter = new StreamWriter(mainStream, Encoding.UTF8);
                using var errorStream = new FileStream(_errorLogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                using var errorWriter = new StreamWriter(errorStream, Encoding.UTF8);
                using var fatalStream = new FileStream(_fatalLogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                using var fatalWriter = new StreamWriter(fatalStream, Encoding.UTF8);
                while (!token.IsCancellationRequested)
                {
                    var entry = _logQueue.Take(token);
                    var localTime = TimeZoneInfo.ConvertTimeFromUtc(entry.Item1, _timeZone);
                    var line = $"[{localTime:dd-MM-yyyy - HH:mm:ss}] [{entry.Item2}] {entry.Item3}";
                    WriteMainIfNeeded(mainWriter, localTime);
                    mainWriter.WriteLine(line);
                        mainWriter.Flush();
                    if (entry.Item2 == LogLevel.Error || entry.Item2 == LogLevel.Fatal)
                    {
                        WriteErrorIfNeeded(errorWriter, localTime);
                        errorWriter.WriteLine(line);
                        errorWriter.Flush();
                    }
                    if (entry.Item2 == LogLevel.Fatal)
                    {
                        WriteFatalIfNeeded(fatalWriter, localTime);
                        fatalWriter.WriteLine(line);
                        fatalWriter.Flush();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                FlushRemaining();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro no processamento de logs: {ex}");
            }
            finally
            {
                FlushRemaining();
            }
        }

        private void FlushRemaining()
        {
            try
            {
                CreateDir(_mainLogFilePath);
                CreateDir(_errorLogFilePath);
                CreateDir(_fatalLogFilePath);
                using var mainStream = new FileStream(_mainLogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                using var mainWriter = new StreamWriter(mainStream, Encoding.UTF8);
                using var errorStream = new FileStream(_errorLogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                using var errorWriter = new StreamWriter(errorStream, Encoding.UTF8);
                using var fatalStream = new FileStream(_fatalLogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
                using var fatalWriter = new StreamWriter(fatalStream, Encoding.UTF8);
                while (_logQueue.TryTake(out var entry))
                {
                    var localTime = TimeZoneInfo.ConvertTimeFromUtc(entry.Item1, _timeZone);
                    var line = $"[{localTime:dd-MM-yyyy - HH:mm:ss}] [{entry.Item2}] {entry.Item3}";
                    WriteMainIfNeeded(mainWriter, localTime);
                    mainWriter.WriteLine(line);
                    mainWriter.Flush();
                    if (entry.Item2 == LogLevel.Error || entry.Item2 == LogLevel.Fatal)
                    {
                        WriteErrorIfNeeded(errorWriter, localTime);
                        errorWriter.WriteLine(line);
                        errorWriter.Flush();
                    }
                    if (entry.Item2 == LogLevel.Fatal)
                    {
                        WriteFatalIfNeeded(fatalWriter, localTime);
                        fatalWriter.WriteLine(line);
                        fatalWriter.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao finalizar flush dos logs: {ex}");
            }
        }

        private void WriteMainIfNeeded(StreamWriter writer, DateTime localTime)
        {
            if (_mainPrefixDone) return;
            _mainPrefixDone = true;
            if (_sessionOnExistingFile && _mainFileExisted)
            {
                writer.WriteLine($"--- NEW LOGGER SESSION: {(_logName != "" ? _logName : _mainLogFilePath)} started at [{localTime:dd-MM-yyyy - HH:mm:ss}] ---");
                writer.Flush();
            }
            else if (!_mainFileExisted)
            {
                writer.WriteLine(!string.IsNullOrEmpty(_logName) ? $"REAL TIME AUDIT: {_logName}" : $"REAL TIME AUDIT: {_mainLogFilePath}");
                writer.WriteLine("Date set in the format [DD-MM-YYYY - HH:MM:SS]");
                writer.Flush();
            }
        }

        private void WriteErrorIfNeeded(StreamWriter writer, DateTime localTime)
        {
            if (_errorPrefixDone) return;
            _errorPrefixDone = true;
            if (_sessionOnExistingFile && _errorFileExisted)
            {
                writer.WriteLine($"--- NEW LOGGER SESSION: {(_logName != "" ? _logName : _errorLogFilePath)} started at [{localTime:dd-MM-yyyy - HH:mm:ss}] ---");
                writer.Flush();
            }
            else if (!_errorFileExisted)
            {
                writer.WriteLine(!string.IsNullOrEmpty(_logName) ? $"REAL TIME AUDIT: {_logName} (error)" : $"REAL TIME AUDIT: {_errorLogFilePath}");
                writer.WriteLine("Date set in the format [DD-MM-YYYY - HH:MM:SS]");
                writer.Flush();
            }
        }

        private void WriteFatalIfNeeded(StreamWriter writer, DateTime localTime)
        {
            if (_fatalPrefixDone) return;
            _fatalPrefixDone = true;
            if (_sessionOnExistingFile && _fatalFileExisted)
            {
                writer.WriteLine($"--- NEW LOGGER SESSION: {(_logName != "" ? _logName : _fatalLogFilePath)} started at [{localTime:dd-MM-yyyy - HH:mm:ss}] ---");
                writer.Flush();
            }
            else if (!_fatalFileExisted)
            {
                writer.WriteLine(!string.IsNullOrEmpty(_logName) ? $"REAL TIME AUDIT: {_logName} (fatal errors)" : $"REAL TIME AUDIT: {_fatalLogFilePath}");
                writer.WriteLine("Date set in the format [DD-MM-YYYY - HH:MM:SS]");
                writer.Flush();
            }
        }

        private void CreateDir(string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _logQueue.CompleteAdding();
            try
            {
                _loggingTask.Wait();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Erro ao aguardar término da task: {ex}");
            }
            finally
            {
                _cancellationTokenSource.Dispose();
            }
        }
    }
}
