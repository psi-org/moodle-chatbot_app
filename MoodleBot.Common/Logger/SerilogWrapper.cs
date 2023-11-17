using System;
using System.Collections.ObjectModel;
using System.Data;
using Datadog.Trace;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Context;
using Serilog.Sinks.Datadog.Logs;
using Serilog.Sinks.MSSqlServer;

namespace MoodleBot.Common
{
    public class SerilogWrapper : ILogger
    {
        private readonly Serilog.Core.Logger _logger;
        private readonly string _applicationName;

        public SerilogWrapper(IConfiguration configuration)
        {
            _logger = ConfigureLogger(configuration);
            _applicationName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
        }

        #region Log level method 
        public void Trace(string message, string userName = null, long? userId = null)
        {
            Log(LoggingEventType.Trace, message, null, userName, userId);
        }

        public void Debug(string message, string userName = null, long? userId = null)
        {
            Log(LoggingEventType.Debug, message, null, userName, userId);
        }

        public void Info(string message, string userName = null, long? userId = null, Exception exception = null)
        {
            Log(LoggingEventType.Info, message, exception, userName, userId);
        }

        public void Warn(string message, Exception exception = null, string userName = null, long? userId = null)
        {
            Log(LoggingEventType.Warn, message, exception, userName, userId);
        }

        public void Error(string message, Exception exception, string userName = null, long? userId = null)
        {
            Log(LoggingEventType.Error, message, exception, userName, userId);
        }

        public void Fatal(string message, Exception exception, string userName = null, long? userId = null)
        {
            Log(LoggingEventType.Fatal, message, exception, userName, userId);
        }

        public void Log(LoggingEventType severity, string message, Exception exception, string userName = null, long? userId = null)
        {
            using (LogContext.PushProperty("UserId", userId))
            using (LogContext.PushProperty("DateUpdated", DateTime.UtcNow))
            using (LogContext.PushProperty("dd_env", CorrelationIdentifier.Env))
            using (LogContext.PushProperty("dd_service", _applicationName))
            using (LogContext.PushProperty("UserName", userName))
            using (LogContext.PushProperty("dd_version", CorrelationIdentifier.Version))
            using (LogContext.PushProperty("dd_trace_id", CorrelationIdentifier.TraceId.ToString()))
            using (LogContext.PushProperty("dd_span_id", CorrelationIdentifier.SpanId.ToString()))
            {
                switch (severity)
                {
                    case LoggingEventType.Trace:
                    case LoggingEventType.Debug:
                        {
                            _logger.Debug(exception, message);
                            break;
                        }
                    case LoggingEventType.Info:
                        {
                            _logger.Information(exception, message);
                            break;
                        }
                    case LoggingEventType.Warn:
                        {
                            _logger.Warning(exception, message);
                            break;
                        }
                    case LoggingEventType.Error:
                        {

                            _logger.Error(exception, message);
                            break;
                        }
                    case LoggingEventType.Fatal:
                        {
                            _logger.Fatal(exception, message);
                            break;
                        }
                }
            }
        }
        #endregion

        #region Configuration
        private Serilog.Core.Logger ConfigureLogger(IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("DefaultConnection");
            var datadogApiKey = Environment.GetEnvironmentVariable("DD_API_KEY") ?? "<api-key>";
            var sinkOpts = GetSqlServerSinkOptions();
            var columnOpts = GetColumnOptions();

            var result = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Deleted", false)
                .Enrich.WithMachineName()
                .WriteTo.Console()
                //TODO CHANGE TO MOODLE DB
                //.WriteTo.MSSqlServer(connectionString: connString, sinkOptions: sinkOpts, columnOptions: columnOpts)
                .WriteTo.DatadogLogs(
                    datadogApiKey,
                    configuration: new DatadogConfiguration { Url = "https://http-intake.logs.datadoghq.eu" })
                .CreateLogger();
            return result;
        }

        private MSSqlServerSinkOptions GetSqlServerSinkOptions()
        {
            var result = new MSSqlServerSinkOptions { TableName = "Logs" };
            return result;
        }

        private ColumnOptions GetColumnOptions()
        {
            var columnOpts = new ColumnOptions();

            // Remove unnessary columns
            //columnOpts.Store.Remove(StandardColumn.TimeStamp);
            columnOpts.Store.Remove(StandardColumn.LogEvent);
            columnOpts.Store.Remove(StandardColumn.Properties);
            columnOpts.Store.Remove(StandardColumn.Message);

            // Customise column names
            columnOpts.TimeStamp.ColumnName = "DateCreated";
            columnOpts.Level.ColumnName = "EventLevel";
            columnOpts.MessageTemplate.ColumnName = "EventMessage";
            // End Customise column names

            columnOpts.TimeStamp.NonClusteredIndex = true;

            columnOpts.AdditionalColumns = new Collection<SqlColumn>
            {
                 new SqlColumn {ColumnName = "DateUpdated", PropertyName = "DateUpdated",DataType = SqlDbType.DateTime},
                 new SqlColumn {ColumnName = "UserName", PropertyName= "UserName", DataType = SqlDbType.NVarChar, AllowNull = true},
                 new SqlColumn() {ColumnName = "MachineName", PropertyName= "MachineName", DataType = SqlDbType.NVarChar, AllowNull = true},
                 new SqlColumn {ColumnName = "UserId", PropertyName= "UserId", DataType = SqlDbType.Int, AllowNull = true},
            };
            return columnOpts;
        }
        #endregion
    }
}
