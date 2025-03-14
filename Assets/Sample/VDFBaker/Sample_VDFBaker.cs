using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClasses.Sample {

    public class Sample_VDFBaker : MonoBehaviour {

        void Start() {

            var vdf = VDFBaker.Localization_Create();
            vdf.Add("english", "title1", "desc1");
            vdf.Add("english", "title2", "desc2");
            vdf.Add("schinese", "title1", "desc1");
            vdf.Add("schinese", "title2", "desc2");

            string vdfStr = vdf.ToVDF();
            Debug.Log(vdfStr);

        }

    }
}
