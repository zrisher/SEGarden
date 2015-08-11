using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.ModAPI;

namespace SEGarden.Files {

    abstract class Handler {

        protected String FileName;
        protected Type TypeForFolderNamespace;
        private bool Loaded;

        public Handler(String fileName) {
            FileName = fileName;

            // TODO: If SE actually picks up one top level namespace in a mod vs 
            // another, we should let filemanager provide this so we can write
            // files in multiple folders

            // make note in logger that Type is indeed depedenant on 
            // top-level mod, and since this must always be below
            // it's invariant and can stay there
            TypeForFolderNamespace = typeof(Handler);
        }

        public virtual void LoadData() {
            Loaded = true;
        }

        public void ConditionalUnloadData() {
            if (Loaded) {
                Close();
                Loaded = false;
            }
        }

        public virtual void Close() { }

    }

    class TextHandler : Handler {

        private System.IO.TextReader TextReader;
        private System.IO.TextWriter TextWriter;

        public TextHandler(String fileName) : base(fileName) { }

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

        // TODO: Allow writing more types of objects
        // TextWriter can handle pretty much anything

        private bool LoadWriter() {
            if (MyAPIGateway.Utilities == null)
                return false;

            try {

                // I believe this takes care of file creation too
                if (TypeForFolderNamespace == null)
                    TextWriter = MyAPIGateway.Utilities.
                        WriteFileInGlobalStorage(FileName);
                else
                    TextWriter = MyAPIGateway.Utilities.
                        WriteFileInLocalStorage(FileName, TypeForFolderNamespace);

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

            base.Close();
        }

    }

    class BinaryHandler : Handler {

        BinaryHandler(String fileName) : base(fileName) { }


        /*


        private System.IO.BinaryReader BinaryReader;
        private System.IO.BinaryWriter BinaryWriter;



        public Handler(String fileName) {
            FileName = fileName;
        }

        public LoadData(){
          // BinaryReader ReadBinaryFileInGlobalStorage(string file);
          // ReadBinaryFileInLocalStorage(string file, Type callingType);
          // BinaryWriter WriteBinaryFileInGlobalStorage(string file);
          // BinaryWriter WriteBinaryFileInLocalStorage(string file, Type callingType);
        }
         * */
    }
}
