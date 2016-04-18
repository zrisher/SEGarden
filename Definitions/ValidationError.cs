using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEGarden.Definitions {

    public class ValidationError {
        public readonly String Source = "";
        public readonly String Message = "";

        public ValidationError(String source, String message) {
            if (source != null) Source = source;
            if (message != null) Message = message;
        }
    }

}
