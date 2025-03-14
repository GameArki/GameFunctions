using System;
using System.Text;
using System.Collections.Generic;

namespace GameClasses {

    public class VDFLocalizationEntity {

        Dictionary<string, VDFLocalizationOneLangModel> all;

        public VDFLocalizationEntity() {
            all = new Dictionary<string, VDFLocalizationOneLangModel>();
        }

        public void Add(string lang, string title, string desc) {
            if (!all.TryGetValue(lang, out VDFLocalizationOneLangModel model)) {
                model = new VDFLocalizationOneLangModel(lang);
                all.Add(lang, model);
            }
            model.Add(title, desc);
        }

        public string ToVDF() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\"lang\"");
            sb.AppendLine("{");
            foreach (var item in all) {
                sb.AppendLine(item.Value.ToVDF());
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

    }

}