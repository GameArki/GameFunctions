using System.Collections.Generic;

namespace GameFunctions.CSharpGenerator {

    public enum VisitLevel {
        Public,
        Private,
        Protected,
        Internal,
    }

    internal static class VisitLevelExtention {
        public static string ToFullString(this VisitLevel level) {
            return level.ToString().ToLower();
        }
    }

}