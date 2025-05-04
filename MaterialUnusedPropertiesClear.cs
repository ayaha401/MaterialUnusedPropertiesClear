using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AyahaGraphicDevelopTools.MaterialPropertiesClear
{
    /// <summary>
    /// マテリアルで使用していないプロパティを消す
    /// </summary>
    public class MaterialUnusedPropertiesClear
    {
        /// <summary>
        /// マテリアルを選択して右クリックで呼び出せるようにする
        /// マテリアルで使用していないプロパティを消す
        /// </summary>
        [MenuItem("Assets/Delete Unused MaterialProperty")]
        public static void DeleteUnusedMaterialProperty()
        {
            var selectedObjects = Selection.objects;
            if (selectedObjects == null || selectedObjects.Length == 0)
            {
                return;
            }

            foreach (var selectedObject in selectedObjects)
            {
                var material = selectedObject as Material;
                if (material == null)
                {
                    return;
                }

                var shader = material.shader;
                if (shader == null)
                {
                    return;
                }

                HashSet<string> shaderProperties = new HashSet<string>();
                int count = ShaderUtil.GetPropertyCount(shader);
                for (int i = 0; i < count; i++)
                {
                    string propName = ShaderUtil.GetPropertyName(shader, i);
                    shaderProperties.Add(propName);
                }

                var serializedObject = new SerializedObject(material);
                var serializedProperty = serializedObject.FindProperty("m_SavedProperties");

                var texEnvProperty = serializedProperty.FindPropertyRelative("m_TexEnvs");
                DeleteUnusedProperty(texEnvProperty, shaderProperties);

                var floatsProperty = serializedProperty.FindPropertyRelative("m_Floats");
                DeleteUnusedProperty(floatsProperty, shaderProperties);

                var colorsProperty = serializedProperty.FindPropertyRelative("m_Colors");
                DeleteUnusedProperty(colorsProperty, shaderProperties);

                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// プロパティを消す
        /// </summary>
        /// <param name="prop">SerializedProperty</param>
        /// <param name="shaderProperties">properties</param>
        private static void DeleteUnusedProperty(SerializedProperty prop, HashSet<string> shaderProperties)
        {
            for (int i = prop.arraySize - 1; i >= 0; i--)
            {
                string propName = prop.GetArrayElementAtIndex(i).FindPropertyRelative("first").stringValue;
                if (!shaderProperties.Contains(propName))
                {
                    prop.DeleteArrayElementAtIndex(i);
                }
            }
        }
    }
}
