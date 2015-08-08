﻿using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.IO;

using Sandbox.Common;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage;

namespace SEGarden.Files {

    /// <summary>
    /// A static class that manages file operations in a mod's Storage directory. 
    /// This manager is thread-safe for use from SE logic components like 
    ///   MySessionComponent and MyGameLogicComponent 
    /// but if file operations are attempted from outside of this mod during its
    /// update frames, bad things could happen. We don't expect calls to ModAPI 
    /// or file writing to be thread safe if dont outside of these controls.
    /// </summary>
    static class Manager {
  
        #region Static Fields

        // Manage a list of handlers for each filename
        private static Dictionary<String, TextWriter> s_TextWriters = 
            new Dictionary<String, TextWriter>();


         // SE loads the storage path for file operations from a Mod's assembly,
        // which it gets from any class within it
        private static readonly Type s_ModType = typeof(Manager);

        #endregion

        /*
        /// <summary>
        /// Destructor to ensure file handles and locks are cleaned up
        /// </summary>
        ~Manager() {
            // TODO: cleanup file handles
            // TODO: Move this out of destructor to custom Dispose method?

            //foreach (KeyValuePair<String, Handler> pair in s_FileHandlers)
             
        }
        */

        public static void writeLine(String output, String fileName) {

        }
        
        /// <summary>
        /// Get the handler dedicated to filename
        /// </summary>
        public static Handler getHandler(string filename) {
            // TODO: Any use for this? Not sure I see the point of having multiple
            // threads within a single update frame
            /*
            using (s_FileHandlersLock.AcquireExclusiveUsing()) {
                if (s_FileHandlers.ContainsKey(filename)) {
                    return s_FileHandlers[filename];
                }
                else {
                    Handler handler = new Handler(filename);
                    s_FileHandlers.Add(filename, handler);
                    return handler;
                }
            }
             * */
            return null;
        }


        /// <summary>
        /// Deletes a file if it exists
        /// </summary>
        /// <param name="filename">name of the file to delete</param>
        /// <param name="type">a type from which SE will determine the assembly's storage path</param>
        /// <returns>True if successful</returns>
        private static bool deleteFile(string filename) {
            if (MyAPIGateway.Utilities == null)
                return false;

            if (MyAPIGateway.Utilities.FileExistsInLocalStorage(filename, typeof(Manager)))
                MyAPIGateway.Utilities.DeleteFileInLocalStorage(filename, typeof(Manager));

            return true;
        }

        private static System.IO.TextWriter getWriter(string filename) {
            if (MyAPIGateway.Utilities == null)
                return null;

            return MyAPIGateway.Utilities.WriteFileInLocalStorage(filename, typeof(Manager));
        }


        private static bool createLogFile() {
            if (MyAPIGateway.Utilities == null)
                return false;

            //using (lock_log.AcquireExclusiveUsing())
            //{
            /*
            try { deleteFile("GardenConquest.log"); }
            catch { }
            try { deleteFile("log.txt"); }
            catch { }

            for (int i = 0; i < 10; i++)
                if (s_FileWriter == null)
                    try { s_FileWriter = MyAPIGateway.Utilities.WriteFileInLocalStorage("log-" + DateTime.Now + ".txt", typeof(Logger)); }
                    catch { }
                else
                    try { deleteFile("log-" + i + ".txt"); }
                    catch { }

            return s_FileWriter != null;
            //}
             * */
            return false;
        }

                /// <summary>
        /// closes the static log file
        /// </summary>
        private static void close() {
            /*
            if (s_FileWriter == null)
                return;
            using (s_FileLock.AcquireExclusiveUsing()) {
                s_FileWriter.Flush();
                s_FileWriter.Close();
                s_FileWriter = null;
                m_closed = true;
            }
             * */
        }

    }

}
