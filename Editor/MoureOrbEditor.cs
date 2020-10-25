/*
Copyright(c) 2015 Konstantinos Mourelas

zlib Licence

This software is provided 'as-is', without any express or implied
warranty.In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions :

1. The origin of this software must not be misrepresented; you must not
claim that you wrote the original software.If you use this software
in a product, an acknowledgment in the product documentation would be
appreciated but is not required.

2. Altered source versions must be plainly marked as such, and must not be
misrepresented as being the original software.

3. This notice may not be removed or altered from any source
distribution.
*/

/*
 * Modified by arcanistry
 * 1. Added in editing of Shape Mask
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class MoureOrbEditor : MaterialEditor {

    public override void OnInspectorGUI()
    {
        if (!isVisible)
            return;

        serializedObject.Update();
        var theShader = serializedObject.FindProperty("m_Shader");
        Shader s = theShader.objectReferenceValue as Shader;
        Material m = target as Material;

        //EditorGUILayout.LabelField(m.name);
        EditorGUI.indentLevel++;

        for (int j = 0; j < ShaderUtil.GetPropertyCount(s); j++)
        {
            string propName = ShaderUtil.GetPropertyName(s, j);
            object preVal;
            object val;
            float Tex01Mult;
            float Tex02Mult;
            float Tex03Mult;
            float LineMult;

            float WiggleAmount;
            float LineWidth;
            float Spherical;
            float Scale;

            float PanSpeed01x;
            float PanSpeed01y;
            float PanSpeed02x;
            float PanSpeed02y;
            float PanSpeed03x;
            float PanSpeed03y;
            float Value;


            if (propName == "_BgTex01")
            {
                preVal = m.GetTexture(propName);
                EditorGUILayout.BeginVertical();
                val = EditorGUILayout.ObjectField("", (Texture)preVal, typeof(Texture), false);
                EditorGUILayout.LabelField("(R)Additive Texture 1", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("(G)Additive Texture 2", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("(B)Multiply Texture", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("(A)Wiggle Texture", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
                if (val != preVal)
                {
                    Undo.RecordObject(m, m.name);
                    m.SetTexture(propName, (Texture)val);
                }
            }

            if (propName == "_Shape")
            {
                preVal = m.GetTexture(propName);
                EditorGUILayout.BeginVertical();
                val = EditorGUILayout.ObjectField("Shape Mask", (Texture)preVal, typeof(Texture), false);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
                if (val != preVal)
                {
                    Undo.RecordObject(m, m.name);
                    m.SetTexture(propName, (Texture)val);
                }
            }

            if (propName == "_Tex01Col")
            {
                preVal = m.GetColor(propName);
                EditorGUILayout.BeginVertical();
                val = EditorGUILayout.ColorField("(R) Color Multiply", (Color)preVal);
                EditorGUILayout.EndVertical();
                if (val != preVal)
                {
                    Undo.RecordObject(m, m.name);
                    m.SetColor(propName, (Color)val);
                }
            }
            if (propName == "_Tex02Col")
            {
                preVal = m.GetColor(propName);
                EditorGUILayout.BeginVertical();
                val = EditorGUILayout.ColorField("(G) Color Multiply", (Color)preVal);
                EditorGUILayout.EndVertical();
                if (val != preVal)
                {
                    Undo.RecordObject(m, m.name);
                    m.SetColor(propName, (Color)val);
                }
            }
            if (propName == "_Tex03Col")
            {
                preVal = m.GetColor(propName);
                EditorGUILayout.BeginVertical();
                val = EditorGUILayout.ColorField("(B) Color Multiply", (Color)preVal);
                EditorGUILayout.EndVertical();
                if (val != preVal)
                {
                    Undo.RecordObject(m, m.name);
                    m.SetColor(propName, (Color)val);
                }
            }
            if (propName == "_LineCol")
            {
                preVal = m.GetColor(propName);
                EditorGUILayout.BeginVertical();
                val = EditorGUILayout.ColorField("Line Color Multiply", (Color)preVal);
                EditorGUILayout.EndVertical();
                if (val != preVal)
                {
                    Undo.RecordObject(m, m.name);
                    m.SetColor(propName, (Color)val);
                }
            }


            if (propName == "_Params01")
            {
                preVal = m.GetVector(propName);
                Vector4 _Val = (Vector4)preVal;
                Vector4 blah;

                EditorGUILayout.BeginVertical();
                Tex01Mult = EditorGUILayout.Slider("(R) Multiplier", _Val.x, 0, 100);
                Tex02Mult = EditorGUILayout.Slider("(G) Multiplier", _Val.y, 0, 100);
                Tex03Mult = EditorGUILayout.Slider("(B) Multiplier", _Val.z, 0, 100);
                LineMult = EditorGUILayout.Slider("(A) Multiplier", _Val.w, 0, 100);
                EditorGUILayout.EndVertical();

                blah.x = Tex01Mult;
                blah.y = Tex02Mult;
                blah.z = Tex03Mult;
                blah.w = LineMult;

                if (blah != _Val)
                {
                    Undo.RecordObject(m, m.name);
                    m.SetVector(propName, (Vector4)blah);
                }
            }
            
            if (propName == "_Params02")
            {
                preVal = m.GetVector(propName);
                Vector4 _Val = (Vector4)preVal;
                Vector4 blah;

                EditorGUILayout.BeginVertical();
                WiggleAmount = EditorGUILayout.Slider("Wiggle Amount", _Val.x, 0, 0.1f);
                LineWidth = EditorGUILayout.Slider("Line Width", _Val.y, 0, 0.1f);
                Spherical = EditorGUILayout.Slider("Spherical Distortion", _Val.z, 0, 8);
                Scale = EditorGUILayout.Slider("Scale", _Val.w, 0, 1);
                EditorGUILayout.EndVertical();

                blah.x = WiggleAmount;
                blah.y = LineWidth;
                blah.z = Spherical;
                blah.w = Scale;

                if (blah != _Val)
                {
                    Undo.RecordObject(m, m.name);
                    m.SetVector(propName, (Vector4)blah);
                }
            }

            if (propName == "_Params03")
            {
                preVal = m.GetVector(propName);
                Vector4 _Val = (Vector4)preVal;
                Vector4 blah;

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("UV Panner Speed 1");

                PanSpeed01x = EditorGUILayout.Slider("X", _Val.x, -0.2f, 0.2f);
                PanSpeed01y = EditorGUILayout.Slider("Y", _Val.y, -0.2f, 0.2f);

                EditorGUILayout.LabelField("UV Panner Speed 2");

                PanSpeed02x = EditorGUILayout.Slider("X", _Val.z, -0.2f, 0.2f);
                PanSpeed02y = EditorGUILayout.Slider("Y", _Val.w, -0.2f, 0.2f);

                EditorGUILayout.EndVertical();

                blah.x = PanSpeed01x;
                blah.y = PanSpeed01y;
                blah.z = PanSpeed02x;
                blah.w = PanSpeed02y;

                if (blah != _Val)
                {
                    Undo.RecordObject(m, m.name);
                    m.SetVector(propName, (Vector4)blah);
                }
            }

            if (propName == "_Params04")
            {
                preVal = m.GetVector(propName);
                Vector4 _Val = (Vector4)preVal;
                Vector4 blah;

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("UV Panner Speed 3");

                PanSpeed03x = EditorGUILayout.Slider("X", _Val.x, -0.2f, 0.2f);
                PanSpeed03y = EditorGUILayout.Slider("Y", _Val.y, -0.2f, 0.2f);

                EditorGUILayout.EndVertical();

                blah.x = PanSpeed03x;
                blah.y = PanSpeed03y;
                blah.z = 0;
                blah.w = 0;

                if (blah != _Val)
                {
                    Undo.RecordObject(m, m.name);
                    m.SetVector(propName, (Vector4)blah);
                }
            }
            if (propName == "_Value")
            {
                preVal = m.GetFloat(propName);
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Orb Value", EditorStyles.boldLabel);
                Value = EditorGUILayout.Slider("", (float)preVal, 0f, 1f);
                EditorGUILayout.EndVertical();
                if (Value != (float)preVal)
                {
                    Undo.RecordObject(m, m.name);
                    m.SetFloat(propName, (float)Value);
                }
            }
        }   
        EditorGUI.indentLevel--;

    }
}
