using System;
using System.Text;
using System.Collections.Generic;

namespace GameClasses {

    internal class VDFLocalizationOneLangModel {

        internal string lang;
        Dictionary<string, string> all;

        internal VDFLocalizationOneLangModel(string lang) {
            this.lang = lang;
            all = new Dictionary<string, string>();
        }

        internal void Add(string title, string desc) {
            all.Add(title, desc);
        }

        internal string ToVDF() {
            StringBuilder sb = new StringBuilder();
            string gap1 = "\t";
            string gap2 = "\t\t";
            string gap3 = "\t\t\t";
            sb.AppendLine(gap1 + "\"" + lang + "\"");
            sb.AppendLine(gap1 + "{");

            sb.AppendLine(gap2 + "\"Tokens\"");
            sb.AppendLine(gap2 + "{");
            foreach (var item in all) {
                sb.AppendLine(gap3 + "\"" + item.Key + "\"" + gap1 + "\"" + item.Value + "\"");
            }
            sb.AppendLine(gap2 + "}");

            sb.AppendLine(gap1 + "}");

            return sb.ToString();
        }
    }

}