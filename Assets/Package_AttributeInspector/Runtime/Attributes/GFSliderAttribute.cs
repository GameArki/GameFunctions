using System;
using System.Reflection;

namespace GameFunctions {

    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class GFSliderAttribute : Attribute {

        FieldInfo belongField;
        public FieldInfo BelongField => belongField;
        public void SetBelongField(FieldInfo value) => belongField = value;

        public string MinName { get; }
        public string MaxName { get; }

        public GFSliderAttribute(string minName, string maxName) {
            this.MinName = minName;
            this.MaxName = maxName;
        }
        
        
    }

}
