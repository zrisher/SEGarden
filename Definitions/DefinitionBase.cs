using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using SEGarden.Logging;

namespace SEGarden.Definitions {

    public abstract class DefinitionBase {

        protected abstract String ValidationName { get; }

        public abstract void Validate(ref List<ValidationError> errors);

        protected void ErrorIf(
            bool condition, String msg, 
            ref List<ValidationError> errors
        ) {
            if (condition)
                errors.Add(new ValidationError(ValidationName, msg));
        }

        protected void ValidateChild(
            DefinitionBase child, String name, 
            ref List<ValidationError> errors
        ) {
            if (child == null)
                errors.Add(new ValidationError(
                    ValidationName, name + " should not be null."
                ));
            else
                child.Validate(ref errors);
        }

        protected void ValidateChildren<T>(
            List<T> children, String name, 
            ref List<ValidationError> errors
        ) where T : DefinitionBase {
            if (children == null)
                errors.Add(new ValidationError(
                    ValidationName, name + " should not be null."
                ));
            else
                foreach (var child in children)
                    child.Validate(ref errors);
        }

    }

}
