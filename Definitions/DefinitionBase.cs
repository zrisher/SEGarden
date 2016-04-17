using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using SEGarden.Logging;

namespace SEGarden.Definitions {

    public abstract class DefinitionBase {

        public abstract bool Validate();

    }

}
