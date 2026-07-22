using System;
using UnityEditor;
using UnityEngine;

namespace Survivor.Core.Editor
{
    [CustomPropertyDrawer(typeof(EnumTable<, >), true)]
    public class EnumTableDrawer : PropertyDrawer
    {
        private int m_EnumCount;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float result = EditorGUIUtility.singleLineHeight * (m_EnumCount + 1);
            return result;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var dataArray = property.FindPropertyRelative("m_DataArray");
            if(dataArray != null)
            {
                var fieldType = fieldInfo.FieldType;
                Debug.Assert(fieldType.IsGenericType);

                var enumType   = fieldType.GetGenericArguments()[0];
                var enumValues = Enum.GetValues(enumType);

                if(enumValues != null)
                {
                    m_EnumCount = enumValues.Length;

                    if(dataArray.arraySize != enumValues.Length)
                    {
                        dataArray.arraySize = enumValues.Length;
                    }

                    EditorGUI.BeginProperty(position, label, property);
                    {
                        var foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

                        if(property.isExpanded)
                        {
                            ++EditorGUI.indentLevel;

                            float paddingY = 2.0f;
                            float cursorY  = foldoutRect.y + EditorGUIUtility.singleLineHeight + paddingY;
                            for(int enumIdx = 0; enumIdx < enumValues.Length; ++enumIdx)
                            {
                                var    elementProp = dataArray.GetArrayElementAtIndex(enumIdx);
                                var    rowRect     = new Rect(position.x, cursorY, position.width, position.height);
                                string enumLabel   = enumValues.GetValue(enumIdx).ToString();

                                EditorGUI.PropertyField(rowRect, elementProp, new(enumLabel), true);

                                cursorY += EditorGUIUtility.singleLineHeight + paddingY;
                            }

                            --EditorGUI.indentLevel;
                        }
                    }
                    EditorGUI.EndProperty();
                }
                else
                {
                    EditorGUI.LabelField(position, "Internal: Could Not Resolve The Enum Type");
                }
            }
            else
            {
                EditorGUI.LabelField(position, "Internal: Could Not Resolve The Data Table");
            }
        }
    }
}