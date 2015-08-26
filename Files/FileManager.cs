using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Sandbox.Common;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage;

using SEGarden.Logging;
using SEGarden.Files.Handlers;

namespace SEGarden.Files {

    /// <summary>
    /// A static class that manages file operations in a mod's Storage directory. 
    /// TODO: Make this thread-safe 
    /// if file operations are attempted from outside of this mod during its
    /// update frames, bad things could happen. We don't expect calls to ModAPI 
    /// or file writing to be thread safe if dont outside of these controls.
    /// 
    /// Be VERY careful adding logging in here - logging relies on some of this,
    /// and calling it within will cause an infinite loop.
    /// </summary>
    class FileManager {
  
        #region Static Fields

        private readonly static string[] TextFileExtensions = { "txt", "log" };

        // SE loads the storage path for file operations from a Mod's assembly
        private readonly static Type TypeForFolder = typeof(FileManager);

        private static Logger Log = new Logger("SEGarden.Files.Manager");

        #endregion

        // Manage a list of handlers for each filename
        private Dictionary<String, FileHandlerBase> FileHandlers =
            new Dictionary<String, FileHandlerBase>();

        public bool Ready { get; private set; }

        public void Initialize() {
            Ready = true;
            Log.Trace("Initialized File Manager", "Initialize");
        }

        public void writeLine(String output, String fileName) {
            if (output == null || fileName == null) return;
            if (!Ready) return;
            TextFileHandler textHandler = getHandler(fileName) as TextFileHandler;
            if (textHandler != null) textHandler.WriteLine(output);
        }

        /*
        public static void Write(String output, String fileName) {
            Handler textHandler = getHandler(fileName);
            if (textHandler != null) textHandler.Write(output);
        }
        */

        private bool CanWriteStringToFile(String output, String fileName) {

            if (!Ready) {
                Log.Error("Filemanager not initialized", "CanWriteStringToFile");
                return false;
            }

            if (fileName == null || fileName.Length < 1) {
                Log.Error("Null or empty filename", "CanWriteStringToFile");
                return false;
            }

            // Note: SE silently fails on these
            int firstIllegalChar = fileName.IndexOfAny(Path.GetInvalidFileNameChars());
            if (firstIllegalChar >= 0) {
                Log.Error("Illegal filename character at position " + firstIllegalChar, "CanWriteStringToFile");
                return false;
            }

            if (output == null || output.Length < 1) {
                Log.Warning("Null or empty output", "CanWriteStringToFile");
                return false;
            }

            FileHandlerBase handler = getHandler(fileName);
            if (handler == null) {
                Log.Error("Error retrieving handler", "CanWriteStringToFile");
                return false;
            }

            return true;
        }

        public void Overwrite(String output, String fileName) {
            Log.Trace(String.Format("Overwrite file \"{0}\" with string of length {1}",
                fileName, output.Length), "Overwrite");

            if (!CanWriteStringToFile(output, fileName)) return;

            DeleteFile(fileName);
            getHandler(fileName).Write(output);
        }

        public void Read<T>(String fileName, ref T result) {
            Log.Trace(String.Format("Read file \"{0}\"", fileName), "Read");

            if (!Exists(fileName)) return;
            FileHandlerBase handler = getHandler(fileName);
            if (handler != null) handler.Read<T>(ref result);
        }

        public T ReadXML<T>(String fileName) {
            Log.Trace("ReadXML from " + fileName, "ReadXML");

            if (!Exists(fileName)) {
                Log.Error("No file found", "ReadXML");
                return default(T);
            }

            String serialized = "";
            Read<String>(fileName, ref serialized);
            if (serialized == null || serialized.Length < 1) {
                Log.Error("Existing file blank", "ReadXML");
                return default(T);
            }

            return MyAPIGateway.Utilities.SerializeFromXML<T>(serialized);
        }

        public bool WriteXML<T>(String fileName, T serializable) {
            if (serializable == null) {
                Log.Error("Null serializable object.", "WriteXML");
                return false;
            }

            if (fileName == null || fileName == "") {
                Log.Error("No filename supplied.", "SaveGrids");
                return false;
            }

            Overwrite(
                MyAPIGateway.Utilities.SerializeToXML<T>(serializable),
                fileName
            );

            if (!Exists(fileName)) {
                Log.Error("Failed writing to file.", "SaveGrids");
                return false;
            }

            return true;
        }

        public bool Exists(String fileName) {
            if (fileName == null) {
                Log.Error("Null filename.", "Exists");
                return false;
            }
            if (!Ready) {
                Log.Error("Manager not ready.", "Exists");
                return false;
            }
            if (!MyAPIGateway.Utilities.FileExistsInLocalStorage(fileName, TypeForFolder)) {
                Log.Trace("File does not exist: " + fileName, "Exists");
                return false;
            }

            Log.Trace("File exists: " + fileName, "Exists");
            return true;
        }

        /// <summary>
        /// Deletes a file if it exists
        /// </summary>
        /// <param name="filename">name of the file to delete</param>
        /// <param name="type">a type from which SE will determine the assembly's storage path</param>
        /// <returns>True if successful</returns>
        public bool DeleteFile(string filename) {
            if (filename == null) return false;
            if (!Ready) return false;

            DropHandler(filename);

            if (MyAPIGateway.Utilities.FileExistsInLocalStorage(filename, TypeForFolder)) ;
            MyAPIGateway.Utilities.DeleteFileInLocalStorage(filename, TypeForFolder);

            return true;
        }

        /*

				// load file

        */

        /// <summary>
        /// Get the handler dedicated to filename
        /// </summary>
        /// <remarks>
        /// DON'T PUT LOGGING IN HERE!
        /// </remarks>
        private FileHandlerBase getHandler(String filename) {
            if (filename == null) return null;

            if (FileHandlers.ContainsKey(filename))
                return FileHandlers[filename];

            String extension = System.IO.Path.GetExtension(filename);

            if (TextFileExtensions.Any(extension.Contains)) {
                TextFileHandler handler = new TextFileHandler(filename);
                FileHandlers.Add(filename, handler);
                return handler;
            }
            else {
                BinaryFileHandler handler = new BinaryFileHandler(filename);
                FileHandlers.Add(filename, handler);
                return handler;
            }            
        }

        /// <summary>
        /// Get the handler dedicated to filename
        /// </summary>
        private void DropHandler(String filename) {
            if (filename == null) return;

            FileHandlerBase handler;
            FileHandlers.TryGetValue(filename, out handler);

            if (handler == null) return;

            handler.Close();
            FileHandlers.Remove(filename);
        }

        /// <summary>
        /// closes all handlers
        /// </summary>
        public void Terminate() {
            Log.Trace("Terminating File Manager", "Close");

            foreach (KeyValuePair<String, FileHandlerBase> pair in FileHandlers) {
                pair.Value.Close();
            }

            FileHandlers.Clear();

            Ready = false;
        }

    }

}
