using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.ModAPI;

namespace SEGarden.Files {

    abstract class Handler {

        protected String FileName;
        protected Type TypeForFolder;
        private bool Loaded;

        public Handler(String fileName) {
            FileName = fileName;

            // TODO: If SE actually picks up one top level namespace in a mod vs 
            // another, we should let filemanager provide this so we can write
            // files in multiple folders

            // make note in logger that Type is indeed depedenant on 
            // top-level mod, and since this must always be below
            // it's invariant and can stay there
            TypeForFolder = typeof(Handler);
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

        public abstract void Write(object ouput);

        public abstract void Read<T>(ref T result);

        public virtual void Close() { }

        public bool Ready() {
            return (MyAPIGateway.Utilities != null);
        }

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

            base.Close();
        }

    }

    class BinaryHandler : Handler {

        private System.IO.BinaryReader BinaryReader;
        private System.IO.BinaryWriter BinaryWriter;

        BinaryHandler(String fileName) : base(fileName) { }



        public override void Write(object output) {
            // check for type and use appropriate writer function
            //BinaryWriter.Write()
        }



        public override void Read<T>(ref T result) {

        }


        public void LoadData(){
          // BinaryReader ReadBinaryFileInGlobalStorage(string file);
          // ReadBinaryFileInLocalStorage(string file, Type callingType);
          // BinaryWriter WriteBinaryFileInGlobalStorage(string file);
          // BinaryWriter WriteBinaryFileInLocalStorage(string file, Type callingType);
        }

    }
}
