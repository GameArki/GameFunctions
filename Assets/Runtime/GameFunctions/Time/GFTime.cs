using System;
using UnityEngine;

namespace GameFunctions {

    public static class GFTime {

        public static int SecTo_SS_XXX(float timeSec, ref char[] buffer) {

            // No GC
            // Format: ss.xxx
            int len = 0;

            int sec = (int)timeSec;
            int ms = (int)((timeSec - sec) * 1000);

            int digit = 1;
            while (sec > 0) {
                buffer[len++] = (char)(sec % 10 + '0');
                sec /= 10;
                digit++;
            }

            if (digit == 1) {
                buffer[len++] = '0';
            }

            // Reverse
            Array.Reverse(buffer, 0, len);

            buffer[len++] = '.';
            buffer[len++] = (char)(ms / 100 + '0');
            buffer[len++] = (char)(ms / 10 % 10 + '0');
            buffer[len++] = (char)(ms % 10 + '0');
            return len;

        }
    }
}