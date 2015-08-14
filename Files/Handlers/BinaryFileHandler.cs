using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEGarden.Files.Handlers {

    class BinaryFileHandler : FileHandlerBase {

        private System.IO.BinaryReader BinaryReader;
        private System.IO.BinaryWriter BinaryWriter;

        public BinaryFileHandler(String fileName) : base(fileName) { }

        public override void Write(object output) {
            // check for type and use appropriate writer function
            //BinaryWriter.Write()
        }

        public override void Read<T>(ref T result) {

        }

        // BinaryReader ReadBinaryFileInGlobalStorage(string file);
        // ReadBinaryFileInLocalStorage(string file, Type callingType);
        // BinaryWriter WriteBinaryFileInGlobalStorage(string file);
        // BinaryWriter WriteBinaryFileInLocalStorage(string file, Type callingType);


        public override void Close() {
            if (BinaryWriter != null) {
                BinaryWriter.Flush();
                BinaryWriter.Close();
                BinaryWriter = null;
            }

            if (BinaryReader != null) {
                BinaryReader.Close();
                BinaryReader = null;
            }
        }

    }

}
