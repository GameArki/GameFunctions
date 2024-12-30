using System;
using System.Collections.Generic;

namespace GameFunctions {

    public static class IntHelper {

        [ThreadStatic] static Dictionary<int, string> numberToStringNoGC = new Dictionary<int, string>(10000);

        public static string ToStringNoGC(int number) {
            if (numberToStringNoGC.TryGetValue(number, out string result)) {
                return result;
            } else {
                string str = number.ToString();
                numberToStringNoGC.Add(number, str);
                return str;
            }
        }
    }
}