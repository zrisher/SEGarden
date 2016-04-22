using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VRage;

using SEGarden.Exceptions;
using SEGarden.Extensions;

namespace SEGarden.Versioning {

    public class AppVersion {

        public ushort Major { get; private set; }
        public ushort Minor { get; private set; }
        public ushort Patch { get; private set; }
        public String GitSHA { get; private set; }
        public String Full {
            get {
                return String.Format("{0}.{1}.{2}-{3}",
                    Major, Minor, Patch, GitSHA);
            }
        }

        public AppVersion(
            ushort major, ushort minor, ushort patch, String sha = ""
        ) {
            if (sha == null) 
                throw new ArgumentException("Git-SHA cannot be null");

            Major = major;
            Minor = minor;
            Patch = patch;
            GitSHA = sha;
        }

        public AppVersion(ByteStream stream) {
            if (stream == null) throw new ArgumentException("null stream");

            Major = stream.getUShort();
            Minor = stream.getUShort();
            Patch = stream.getUShort();
            GitSHA = stream.getString();
        }

        public void AddToByteSteam(ByteStream stream) {
            if (stream == null) throw new ArgumentException("null stream");

            stream.addUShort(Major);
            stream.addUShort(Minor);
            stream.addUShort(Patch);
            stream.addString(GitSHA);
        }

        public String ToString() {
            return Full;
        }

    }

}
