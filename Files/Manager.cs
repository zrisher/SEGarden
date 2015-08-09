﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private static Dictionary<String, Handler> FileHandlers = 
            new Dictionary<String, Handler>();

        private readonly static string[] TextFileExtensions = { "txt", "log" };


         // SE loads the storage path for file operations from a Mod's assembly,
        // which it gets from any class within it
        //private static readonly Type s_ModType = typeof(Manager);

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
            TextHandler textHandler = getHandler(fileName) as TextHandler;
            if (textHandler != null) textHandler.WriteLine(output);
        }
        
        /// <summary>
        /// Get the handler dedicated to filename
        /// </summary>
        public static Handler getHandler(String filename) {
            if (filename == null) return null;

            if (FileHandlers.ContainsKey(filename))
                return FileHandlers[filename];

            String extension = System.IO.Path.GetExtension(filename);

            // TODO: handle more file types
            if (TextFileExtensions.Any(extension.Contains)) {
                TextHandler handler = new TextHandler(filename);
                FileHandlers.Add(filename, handler);
                return handler;
            }
                
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



        /// <summary>
        /// closes all handlers
        /// </summary>
        public static void Close() {
            foreach (KeyValuePair<String, Handler> pair in FileHandlers) {
                pair.Value.Close();
            }

            FileHandlers.Clear();
        }

    }

}
