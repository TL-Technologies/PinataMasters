using UnityEditor;
using UnityEngine;


namespace Modules.General.Editor
{
    [CustomPropertyDrawer(typeof(SerializableDictionaryAttribute))]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        private const string KeysList = "keysList";
        private const string ValuesList = "valuesList";
        private const int ItemsPerPage = 20;

        private int page;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var keysList = property.FindPropertyRelative(KeysList);
            var valuesList = property.FindPropertyRelative(ValuesList);

            property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, property.name);

            if (property.isExpanded)
            {
                int indexToRemove = -1;
                int itemsCount = Mathf.Min(keysList.arraySize, valuesList.arraySize);
                for (int i = page * ItemsPerPage; i < itemsCount && i < (page + 1) * ItemsPerPage; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(keysList.GetArrayElementAtIndex(i), GUIContent.none);
                        EditorGUILayout.PropertyField(valuesList.GetArrayElementAtIndex(i), GUIContent.none);

                        if (GUILayout.Button("-"))
                        {
                            indexToRemove = i;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (indexToRemove >= 0)
                {
                    RemoveElementAtIndex(keysList, indexToRemove);
                    RemoveElementAtIndex(valuesList, indexToRemove); 
                    CheckAndRemoveOutboundElements(keysList, valuesList);
                    property.serializedObject.ApplyModifiedProperties();
                }

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Add"))
                    {
                        keysList.InsertArrayElementAtIndex(keysList.arraySize);
                        valuesList.InsertArrayElementAtIndex(valuesList.arraySize);
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
                EditorGUILayout.EndHorizontal();

                int maxPages = Mathf.FloorToInt(itemsCount / (float) ItemsPerPage);

                if (maxPages > 0)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (GUILayout.Button("<<"))
                        {
                            page--;
                        }

                        var centeredStyle = new GUIStyle(GUI.skin.label);
                        centeredStyle.alignment = TextAnchor.UpperCenter;
                        EditorGUILayout.LabelField($"{page + 1}/{maxPages + 1}", centeredStyle, GUILayout.MaxWidth(80));

                        if (GUILayout.Button(">>"))
                        {
                            page++;
                        }

                        page = Mathf.Clamp(page, 0, maxPages);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            bool hasDuplicateKeys = false;

            for (int i = 0; i < keysList.arraySize; i++)
            {
                for (int x = 0; x < keysList.arraySize; x++)
                {
                    if (x != i && SerializedProperty.DataEquals(keysList.GetArrayElementAtIndex(i),
                        keysList.GetArrayElementAtIndex(x)))
                    {
                        hasDuplicateKeys = true;
                        break;
                    }
                }

                if (hasDuplicateKeys)
                {
                    break;
                }
            }

            if (hasDuplicateKeys)
            {
                EditorGUILayout.HelpBox("Duplicate keys found", MessageType.Error);
            }

            EditorGUILayout.Space();
        }

        
        private void RemoveElementAtIndex(SerializedProperty property, int indexToRemove)
        {
            // Following solution required for deletion an element when arraySize stays same as before first deletion
            // arraySize stays same cause `property.DeleteArrayElementAtIndex(int)` sets element to null if it's not null instead of real deletion
            int arraySize = property.arraySize;
            property.DeleteArrayElementAtIndex(indexToRemove);
            if (arraySize == property.arraySize)
            {
                property.DeleteArrayElementAtIndex(indexToRemove);
            }
        }

        
        private void RemoveOutboundElements(SerializedProperty inboundProperty, SerializedProperty outboundProperty)
        {
            int outboundIndex = inboundProperty.arraySize;
            while (outboundIndex < outboundProperty.arraySize)
            {
                outboundProperty.DeleteArrayElementAtIndex(outboundIndex);
            }
        }

        
        private void CheckAndRemoveOutboundElements(SerializedProperty keysList, SerializedProperty valuesList)
        {
            if (keysList.arraySize < valuesList.arraySize)
            {
                RemoveOutboundElements(keysList, valuesList);
            }
            else
            {
                RemoveOutboundElements(valuesList, keysList);
            }
        }

    }
}
