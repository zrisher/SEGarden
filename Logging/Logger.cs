
using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading;

using Sandbox.Common;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage;

namespace SEGarden.Logging
{
	//
	/// <summary>
	/// Generates log files
	/// </summary>
	/// <remarks>
    /// Pattern: [%time][%thread][%level][%class][%instance][%method] - %Message
    /// 
    /// This must be a MySessionComponentBase in order to load before the world does
	/// </remarks>
	//[Sandbox.Common.MySessionComponentDescriptor(Sandbox.Common.MyUpdateOrder.NoUpdate)]
	public class Logger { //: Sandbox.Common.MySessionComponentBase {

        #region Static

        public static Severity.Level Level = Severity.Level.All;
        public static readonly String DefaultLogFileName = 
            "log-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".log";

        //private static int s_Line = 1;
        //private static int s_MaxLinesPerFile = 1000000;
        //private static int s_MaxRotationsToKeep = 3;

        #endregion
        #region Instance Fields

		// Log descriptors, functions take precedence over strings
        private Func<String> InstanceNameFunc;
		private String ClassName, InstanceName, ThreadName;

        private String FileName;
        private StringBuilder StringCache = new StringBuilder();

        #endregion
        #region Constructors

        /// <summary>
		/// Creates a Logger that holds a description of context for logging
		/// </summary>
        /// <param name="className">the name of the class this belongs to</param>
		/// <param name="instanceID">an identifier for the instance this belongs to</param>
        /// <param name="thread">an identifier for the thread this belongs to</param>
        public Logger(String className, String instanceName = "Static", 
            String fileName = null, String threadName = "Main")
		{
            if (fileName == null) fileName = DefaultLogFileName;
            this.ClassName = className;
            this.FileName = fileName;
            this.InstanceName = instanceName;
            this.ThreadName = threadName;
		}

		/// <summary>
		/// Creates a Logger that gets the instance name from a supplied function
		/// </summary>
        /// <param name="className">the name of the class this belongs to</param>
        /// <param name="instanceFunc">a function that will generate the instance identifier</param>
        public Logger(String className, Func<String> instanceFunc, String fileName = null, 
            String threadName = "Main")
		{
            if (fileName == null) fileName = DefaultLogFileName;
			this.ClassName = className;
            this.FileName = fileName;
			this.InstanceNameFunc = instanceFunc;
            this.ThreadName = threadName;
		}

        #endregion
        #region Logging

        /// <summary>
        /// Log a Fatal message
        /// </summary>
        /// <param name="toLog">message to log</param>
        /// <param name="methodName">calling method</param>
        public void Fatal(string message, string methodName)
        {
            Log(message, methodName, Severity.Level.Fatal);
        }

        /// <summary>
        /// Log an Error message
        /// </summary>
        /// <param name="toLog">message to log</param>
        /// <param name="methodName">calling method</param>
        public void Error(string message, string methodName)
        {
            Log(message, methodName, Severity.Level.Error);
        }

        /// <summary>
        /// Log a Warning message
        /// </summary>
        /// <param name="toLog">message to log</param>
        /// <param name="methodName">calling method</param>
        public void Warning(string message, string methodName)
        {
            Log(message, methodName, Severity.Level.Warning);
        }

        /// <summary>
        /// Log an Info message
        /// </summary>
        /// <param name="toLog">message to log</param>
        /// <param name="methodName">calling method</param>
        public void Info(string message, string methodName)
        {
            Log(message, methodName, Severity.Level.Info);
        }

        /// <summary>
		/// Log a Debug message
		/// </summary>
		/// <param name="toLog">message to log</param>
		/// <param name="methodName">calling method</param>
		public void Debug(string message, string methodName) 
        { 
            Log(message, methodName, Severity.Level.Debug); 
        }

		/// <summary>
		/// Log a message
		/// </summary>
        /// <param name="message">message to log</param>
		/// <param name="methodName">calling method</param>
        /// <param name="level">severity level</param>
        public void Log(String message, String methodName, Severity.Level level = Severity.Level.Trace)
		{
 
			if (level > Level) return;
            
            // prepare logging details
            if (InstanceNameFunc != null) InstanceName = InstanceNameFunc.Invoke();
            
            // build log line
            appendWithBrackets(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"));
            appendWithBrackets(level.ToString());
            appendWithBrackets(ThreadName);
            appendWithBrackets(ClassName);
            appendWithBrackets(InstanceName);
            appendWithBrackets(methodName);
            StringCache.Append(" - " + message);
            StringCache.Append(" - Filename is " + FileName);

            // rotate log files if this one's too long
            // how would this work if we're reusing an existing file? Count the lines first?
            /*
			if (s_Line >= s_MaxLinesPerFile) {
                //s_FileManager.doneUsing(s_FileName);
                s_FileName = "log - " + DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");
                //s_FileManager.startUsing(s_FileName);
                s_Line = 1;
            }
            **/

            Files.Manager.writeLine(StringCache.ToString(), FileName); 
            StringCache.Clear();
		}

		private void appendWithBrackets(String append)
		{
			if (append == null)
				append = String.Empty;

			StringCache.Append('[');
			StringCache.Append(append);
			StringCache.Append(']');
		}

        #endregion

    }
}