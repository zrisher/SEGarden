/*
using System;
//using System.Collections.Generic;
//using System.Text;

namespace SEModGarden.Logging {

	/// <summary>
	/// Base class for Core processes
	/// </summary>
	public abstract class StaticLoggable {
		private static Logger s_Logger;
        protected static String s_LoggerClassName = "LoggableClass";

		protected void log(String message, String methodName = "LoggableMethod", 
            Logger.severity level = Logger.severity.DEBUG) {

            if (s_Logger == null)
                s_Logger = new Logger(s_LoggerClassName, "Static");

            s_Logger.log(level, methodName, message);
		}
	}
}
*/