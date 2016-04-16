using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEGarden.Exceptions {

    // Exceptions missing from the whitelist

    public class NotImplementedException : System.Exception { 
        public NotImplementedException() : base() { }
        public NotImplementedException(String msg) : base(msg) { }
    }

    public class FieldAccessException : System.Exception {
        public FieldAccessException() : base() { }
        public FieldAccessException(String msg) : base(msg) { }
    }


}
