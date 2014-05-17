﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
namespace EventManagerServer
{
	public enum LogLevel
	{
		Debug,
		Info,
		Warning,
		Error
	}

	public delegate void LogEvent(string message, LogLevel level);

	public static class Logger
	{
		public const string LogFileName = "server.log";
		private static bool disposed = false;
		private static string prefix = string.Empty;
		private static TextWriter textWriter;
		public static event LogEvent OnLogEvent;

		static Logger()
		{
			LoadLogFile();
		}
		private static void LoadLogFile()
		{
			textWriter = new StreamWriter(LogFileName, true);
		}

		public static void SetPrefix(string prefix)
		{
			Logger.prefix = prefix;
		}

		public static void ClearPrefix()
		{
			Logger.prefix = string.Empty;
		}

		public static void Log(string message, LogLevel level = LogLevel.Debug, params object[] format)
		{
			if (format.Length != 0) {
				message = String.Format(message, format);
			}
			StringBuilder lineBuilder = new StringBuilder();
			ConsoleColor lineColor = ConsoleColor.Gray;

			switch (level) {
				case LogLevel.Debug:
					lineBuilder.Append("[DEB]\t");
					lineColor = ConsoleColor.Gray;
					break;
				case LogLevel.Info:
					lineBuilder.Append("[INF]\t");
					lineColor = ConsoleColor.Green;
					break;
				case LogLevel.Warning:
					lineBuilder.Append("[WRN]\t");
					lineColor = ConsoleColor.Yellow;
					break;
				case LogLevel.Error:
					lineBuilder.Append("[ERR]\t");
					lineColor = ConsoleColor.DarkRed;
					break;
			}
			lineBuilder.Append(prefix);
			lineBuilder.Append(message);

			WriteToConsole(lineColor, level, lineBuilder);

			if ((level == LogLevel.Error || level == LogLevel.Warning) && OnLogEvent != null) {
				OnLogEvent(lineBuilder.ToString(), level);
			}
			lineBuilder.Insert(0, DateTime.Now.ToString("[MMM dd - HH:mm:ss.fff]\t"));
			WriteToLogFile(lineBuilder);
		}

		private static void WriteToLogFile(StringBuilder lineBuilder)
		{
			if (!disposed) {
				textWriter.WriteLine(lineBuilder.ToString());
				textWriter.Flush();
			}
		}

		private static void WriteToConsole(ConsoleColor lineColor, LogLevel level, StringBuilder lineBuilder)
		{
			var prevColor = Console.ForegroundColor;
			Console.ForegroundColor = lineColor;
			Console.WriteLine(lineBuilder.ToString());
			Console.ForegroundColor = prevColor;
		}

		public static void ClearLog()
		{
			textWriter.Close();
			File.Delete(LogFileName);
			LoadLogFile();
		}
		public static void Dispose()
		{
			Log("Shutting down logger", LogLevel.Info);
			textWriter.Close();
			textWriter.Dispose();
			disposed = true;
		}
	}
}
