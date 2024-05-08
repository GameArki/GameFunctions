using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace GameFunctions.Editors {

    public class GFAttributeMainEditor : Editor {

        // Fields
        List<GFEnumToggleButtonAttribute> enumToggleButtons;
        // Properties

        // Methods
        List<GFButtonAttribute> buttons;
        // Slider
        List<GFSliderAttribute> sliders;

        void OnEnable() {


            sliders = new List<GFSliderAttribute>();
            enumToggleButtons = new List<GFEnumToggleButtonAttribute>();


            // Init And Cache:
            // - Fields
            // - Properties
            // - Methods

            FieldInfo[] fields = target.GetType().GetFields();

            //-----------------Field-----------------------
            for (int i = 0; i < fields.Length; i += 1) {
                var field = fields[i];
                var attrs = field.GetCustomAttributes(true);
                foreach (var attr in attrs) {
                    //Slider
                    if (attr is GFSliderAttribute) {
                        var tar = attr as GFSliderAttribute;
                        tar.SetBelongField(field);
                        sliders.Add(tar);
                    }
                    //EnumToggleButton
                    if (attr is GFEnumToggleButtonAttribute) {
                        var tar = attr as GFEnumToggleButtonAttribute;
                        tar.SetBelongField(field);
                        enumToggleButtons.Add(tar);
                    }
                }
            }

            //-----------------Button-----------------------
            buttons = new List<GFButtonAttribute>();

            MethodInfo[] methods = target.GetType().GetMethods();

            foreach (var method in methods) {
                var atButton = (GFButtonAttribute)method.GetCustomAttributes(typeof(GFButtonAttribute), true).FirstOrDefault();
                if (atButton != null) {
                    atButton.SetButtonFunction(method);
                    buttons.Add(atButton);
                }
            }

        }

        void OnDisable() {
            // Clean Up:
            // - Fields
            // - Properties
            // - Methods

            sliders?.Clear();
            buttons?.Clear();
            enumToggleButtons?.Clear();
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            //---------------Slider------------------
            foreach (var slider in sliders) {
                FieldInfo belongField = slider.BelongField;
                object sliderValueObj = belongField.GetValue(target);
                if (sliderValueObj != null) {
                    float sliderValue = (float)sliderValueObj;
                    float min = (float)belongField.DeclaringType.GetField(slider.MinName, FLAGS).GetValue(target);
                    float max = (float)belongField.DeclaringType.GetField(slider.MaxName, FLAGS).GetValue(target);
                    float newValue = EditorGUILayout.Slider(belongField.Name, sliderValue, min, max);
                    if (newValue != sliderValue) {
                        belongField.SetValue(target, newValue);
                    }
                }
            }

            //----------------Button-------------------
            foreach (var button in buttons) {
                if (GUILayout.Button(button.ButtonName)) {
                    button.ButtonMethod.Invoke(target, null);
                }
            }

            //-------------EnumToggleButton-------------
            foreach (var enumButton in enumToggleButtons) {

                GUILayout.Label(enumButton.Label);
                var values = Enum.GetValues(enumButton.BelongField.FieldType);
                int enumLength = values.Length;
                for (int i = 0; i < enumLength; i++) {

                    if (i % 4 == 0) {
                        GUILayout.BeginHorizontal();
                    }
                    
                    int value = (int)values.GetValue(i);
                    GUI.backgroundColor = enumButton.IsContainValue(target, value) ? Color.cyan : Color.white;
                    GUIStyle myGUIStyle = new GUIStyle(GUI.skin.button);

                    if (enumButton.IsContainValue(target, value)) {
                        myGUIStyle.normal.textColor = Color.yellow;
                    }

                    string name = Enum.GetName(enumButton.BelongField.FieldType, value);
                    if (GUILayout.Button(name, myGUIStyle, GUILayout.Width(enumButton.ButtonWidth), GUILayout.Height(enumButton.ButtonHeight))) {
                        enumButton.SetBelongFieldValue(target, value);
                    }

                    if (i % 4 == 3 || i == enumLength - 1) {
                        GUILayout.EndHorizontal();
                    }

                    GUI.backgroundColor = Color.white;
                }
            }
        }

    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), editorForChildClasses: true, isFallback = true)]
    public class ATMonoBehaviourEditor : GFAttributeMainEditor {

    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ScriptableObject), editorForChildClasses: true, isFallback = true)]
    public class ATScriptableObjectEditor : GFAttributeMainEditor {

    }

}