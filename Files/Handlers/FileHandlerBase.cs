using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace SEGarden.Files.Handlers {

    abstract class FileHandlerBase {

        // SE loads the storage path for file operations from a Mod's assembly
        private readonly static Type ClassType = typeof(FileHandlerBase);

        //private bool Loaded;

        protected String FileName;
        protected Type TypeForFolder;


        public FileHandlerBase(String fileName) {
            FileName = fileName;
            TypeForFolder = ClassType;
        }

        /*
        public virtual void LoadData() {
            Loaded = true;
        }

        public void ConditionalUnloadData() {
            if (Loaded) {
                Close();
                Loaded = false;
            }
        }
         * */

        public abstract void Write(object ouput);

        public abstract void Read<T>(ref T result);

        /// <summary>
        /// Close open file handlers
        /// </summary>
        public abstract void Close();

                /*
        public bool Ready() {
            return (MyAPIGateway.Utilities != null);
        }
         * */

    }
}
