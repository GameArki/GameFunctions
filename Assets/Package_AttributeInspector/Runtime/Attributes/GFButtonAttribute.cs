using System;
using System.Reflection;

namespace GameFunctions {

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class GFButtonAttribute : Attribute {

        string buttonName;
        public string ButtonName => buttonName;

        MethodInfo buttonMethod;
        public MethodInfo ButtonMethod => buttonMethod;

        public GFButtonAttribute(string buttonName) {
            this.buttonName = buttonName;
            buttonMethod = null;
        }

        public void SetButtonFunction(MethodInfo buttonMethod) {
            this.buttonMethod = buttonMethod;
        }
    }
}
