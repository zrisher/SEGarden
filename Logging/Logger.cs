
using System;
//using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using System.Threading;

using Sandbox.Common;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage;

namespace SEModGarden.Logging
{
	//
	/// <summary>
	/// Generates log files to be read by GamutLogViewer.
	/// </summary>
	/// <remarks>
    /// Pattern: [%time][%thread][%level][%class][%instance][%method] - %Message
    /// 
    /// This must be a MySessionComponentBase in order to load before the world does
	/// </remarks>
	//[Sandbox.Common.MySessionComponentDescriptor(Sandbox.Common.MyUpdateOrder.NoUpdate)]
	public class Logger { //: Sandbox.Common.MySessionComponentBase {

        #region Static

        // writer
        private static String s_FileName;
        //private static Files.Manager s_FileManager;
        private static int s_Line = 1;

        // config
        private static int s_MaxLinesPerFile = 1000000;
        private static int s_MaxRotationsToKeep = 3;
        public static Severity.Level s_LevelToLog = Severity.Level.All;

        // state
        private static bool s_Closed = false;

        #endregion
        #region Instance Fields

		// Log descriptors, functions take precedence over strings
        private Func<string> m_InstanceFunc;
		private string m_Class, m_Instance, m_Thread;

        private StringBuilder m_StringCache = new StringBuilder();

        #endregion
        #region SessionComponent hooks
        /*
        /// <summary>
		/// needed for MySessionComponentBase, not useful for logging
		/// </summary>
		public Logger() { }

        protected override void UnloadData()
		{ close(); }
        */
        #endregion
        #region Constructors

        /// <summary>
		/// Creates a Logger that holds a description of context for logging
		/// </summary>
        /// <param name="className">the name of the class this belongs to</param>
		/// <param name="instanceID">an identifier for the instance this belongs to</param>
        /// <param name="thread">an identifier for the thread this belongs to</param>
		public Logger(string className, string instanceID = null, string thread = null)
		{
            this.m_Class = className;
			this.m_Instance = instanceID;
            this.m_Thread = thread;
		}

		/// <summary>
		/// Creates a Logger that gets the instance name from a supplied function
		/// </summary>
        /// <param name="className">the name of the class this belongs to</param>
        /// <param name="instanceFunc">a function that will generate the instance identifier</param>
        public Logger(string className, Func<string> instanceFunc, string thread = null)
		{
			this.m_Class = className;
			this.m_InstanceFunc = instanceFunc;
            this.m_Thread = thread;
		}

        #endregion
        #region Destructors

        /// <summary>
        /// closes the static log file
        /// </summary>
        private static void close() {
            //s_FileManager.doneUsing(s_FileName);
            s_Closed = true;
        }

        #endregion
        #region Logging

        /// <summary>
        /// Log a Fatal message
        /// </summary>
        /// <param name="toLog">message to log</param>
        /// <param name="methodName">calling method</param>
        public void fatal(string message, string methodName)
        {
            log(message, methodName, Severity.Level.Fatal);
        }

        /// <summary>
        /// Log an Error message
        /// </summary>
        /// <param name="toLog">message to log</param>
        /// <param name="methodName">calling method</param>
        public void error(string message, string methodName)
        {
            log(message, methodName, Severity.Level.Error);
        }

        /// <summary>
        /// Log a Warning message
        /// </summary>
        /// <param name="toLog">message to log</param>
        /// <param name="methodName">calling method</param>
        public void warning(string message, string methodName)
        {
            log(message, methodName, Severity.Level.Warning);
        }

        /// <summary>
        /// Log an Info message
        /// </summary>
        /// <param name="toLog">message to log</param>
        /// <param name="methodName">calling method</param>
        public void info(string message, string methodName)
        {
            log(message, methodName, Severity.Level.Info);
        }

        /// <summary>
		/// Log a Debug message
		/// </summary>
		/// <param name="toLog">message to log</param>
		/// <param name="methodName">calling method</param>
		public void debug(string message, string methodName) 
        { 
            log(message, methodName, Severity.Level.Debug); 
        }

		/// <summary>
		/// Log a message
		/// </summary>
        /// <param name="message">message to log</param>
		/// <param name="methodName">calling method</param>
        /// <param name="level">severity level</param>
        public void log(string message, string methodName, Severity.Level level = Severity.Level.Trace)
		{
			if (s_Closed || level > s_LevelToLog)
				return;
            
            // prepare logging details
            if (m_InstanceFunc != null)
                m_Instance = m_InstanceFunc.Invoke();
            
            // build log line
            appendWithBrackets(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"));
            appendWithBrackets(m_Thread);
            appendWithBrackets(level.ToString());
            appendWithBrackets(m_Class);
            appendWithBrackets(m_Instance);
            appendWithBrackets(methodName);
            m_StringCache.Append(" - " + message);

            // rotate log files if this one's too long
			if (s_Line >= s_MaxLinesPerFile) {
                //s_FileManager.doneUsing(s_FileName);
                s_FileName = "log - " + DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");
                //s_FileManager.startUsing(s_FileName);
                s_Line = 1;
            }

            // write log line
            //s_FileManager.writeLine(m_StringCache, s_FileName);
            m_StringCache.Clear();
		}

		private void appendWithBrackets(string append)
		{
			if (append == null)
				append = String.Empty;
			m_StringCache.Append('[');
			m_StringCache.Append(append);
			m_StringCache.Append(']');
		}

        #endregion
        #region Notifications

        /// <summary>
		/// For a safe way to display a message as a notification, conditional on LOG_ENABLED. Logs a warning iff message cannot be displayed.
		/// </summary>
		/// <param name="message">the notification message</param>
		/// <param name="disappearTimeMs">time on screen, in milliseconds</param>
		/// <param name="level">severity level</param>
		/// <returns>true iff the message was displayed</returns>
		//[System.Diagnostics.Conditional("LOG_ENABLED")]
        public void debugNotify(string message, int disappearTimeMs = 2000, Severity.Level level = Severity.Level.Trace)
		{
			notify(message, disappearTimeMs, level);
		}

		/// <summary>
		/// For a safe way to display a message as a notification, not conditional. Logs a warning iff message cannot be displayed.
		/// </summary>
		/// <param name="message">the notification message</param>
		/// <param name="disappearTimeMs">time on screen, in milliseconds</param>
		/// <param name="level">severity level</param>
		/// <returns>true iff the message was displayed</returns>
		public void notify(string message, int disappearTimeMs = 2000, Severity.Level level = Severity.Level.Trace)
		{
			MyFontEnum font = Severity.font(level);
			if (MyAPIGateway.Utilities != null)
				MyAPIGateway.Utilities.ShowNotification(message, disappearTimeMs, font);
			else
				warning("MyAPIGateway.Utilities == null", "ShowNotificationDebug()");
        }

        #endregion

    }
}