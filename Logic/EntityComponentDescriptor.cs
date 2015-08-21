using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEGarden.Logic {

    /// <summary>
    /// DISABLED - currently reflection not allowed in scripts.
    /// Use this attribute to tag EntityComponents for ComponentManager to load
    /// </summary>
    /// <remarks>
    /// Should we let this be inherited?
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EntityComponentDescriptor : SessionComponentDescriptor
    {
        public Type EntityBuilderType;
        public string[] EntityBuilderSubTypeNames;

        public EntityComponentDescriptor(RunLocation targetLocation, 
            Type entityBuilderType) : 
            this(targetLocation, entityBuilderType, new string[0]) {

        }

        public EntityComponentDescriptor(RunLocation targetLocation, 
            Type entityBuilderType, params string[] entityBuilderSubTypeNames) : 
            base(targetLocation) {

            EntityBuilderType = entityBuilderType;
            EntityBuilderSubTypeNames = entityBuilderSubTypeNames;
        }


    }

}
