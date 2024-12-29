using System;
using System.Collections.Generic;

namespace GameFunctions {

    public static class IntHelper {

        static Dictionary<int, string> numberToStringNoGC = new Dictionary<int, string>();
        static IntHelper() {
            for (int i = 0; i < 10000; i++) {
                numberToStringNoGC.Add(i, i.ToString());
            }
        }

        public static string ToStringNoGC_in10000(int number) {
            if (numberToStringNoGC.TryGetValue(number, out string result)) {
                return result;
            }
            return number.ToString();
        }
    }
}