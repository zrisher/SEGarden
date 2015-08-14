using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.ModAPI;

namespace SEGarden.Files.Handlers {

    class TextFileHandler : FileHandlerBase {

        private System.IO.TextReader TextReader;
        private System.IO.TextWriter TextWriter;

        public TextFileHandler(String fileName) : base(fileName) { }

        public void WriteLine(StringBuilder stringBuilder) {
            if (TextWriter == null) {
                if (!LoadWriter()) return;
            }

            TextWriter.WriteLine(stringBuilder);
            TextWriter.Flush();
        }

        public void WriteLine(String output) {
            if (TextWriter == null) {
                if (!LoadWriter()) return;
            }

            TextWriter.WriteLine(output);
            TextWriter.Flush();
        }

        public override void Write(object output) {
            if (TextWriter == null) {
                if (!LoadWriter()) return;
            }

            TextWriter.Write(output);
        }

        public override void Read<T>(ref T result) {
            if (TextReader == null) {
                if (!LoadReader()) return;
            }

            if (result is String)
                result = (T)(object)TextReader.ReadToEnd();
        }


        // TODO: Allow writing more types of objects
        // TextWriter can handle pretty much anything

        private bool LoadWriter() {
            if (MyAPIGateway.Utilities == null)
                return false;

            try {

                // I believe this takes care of file creation too
                if (TypeForFolder == null)
                    TextWriter = MyAPIGateway.Utilities.
                        WriteFileInGlobalStorage(FileName);
                else
                    TextWriter = MyAPIGateway.Utilities.
                        WriteFileInLocalStorage(FileName, TypeForFolder);

                return true;
            }
            catch {
                return false;
            }
        }

        private bool LoadReader() {
            if (MyAPIGateway.Utilities == null)
                return false;

            if (!MyAPIGateway.Utilities.FileExistsInLocalStorage(
                FileName, TypeForFolder))
                return false;

            try {
                if (TypeForFolder == null)
                    TextReader = MyAPIGateway.Utilities.
                        ReadFileInGlobalStorage(FileName);
                else
                    TextReader = MyAPIGateway.Utilities.
                        ReadFileInLocalStorage(FileName, TypeForFolder);

                return true;
            }
            catch {
                return false;
            }
        }


        // TODO: Text reading
        // TODO: Object serialization
        // TODO: Object deserialization

        public override void Close() {
            if (TextWriter != null) {
                TextWriter.Flush();
                TextWriter.Close();
                TextWriter = null;
            }

            if (TextReader != null) {
                TextReader.Close();
                TextWriter = null;
            }
        }

    }
}
