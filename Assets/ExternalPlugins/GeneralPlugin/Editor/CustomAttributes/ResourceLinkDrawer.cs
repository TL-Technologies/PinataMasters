using Modules.Hive.Editor;
using Modules.General.Editor;
using Modules.General.HelperClasses;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(ResourceLinkAttribute))]
public class ResourceLinkDrawer : PropertyDrawer 
{
    private static Texture2D resourceLinkTexture;
    private static Texture2D ResourceLinkTexture
    {
        get
        {
            if (resourceLinkTexture == null)
            {
                resourceLinkTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    UnityPath.Combine(
                        GeneralPluginHierarchy.Instance.RootAssetPath, 
                        "Editor", 
                        "Textures", 
                        "resource_link.png"));
            }
            
            return resourceLinkTexture;
        }
    }
    
    IList list;
    int index;

    ResourceLinkAttribute NamedAttribute { get { return ((ResourceLinkAttribute)attribute); } }

    static Dictionary<string, string> normalizedNames = new Dictionary<string, string>();
    public static string UpperSeparatedName(string name)
    {
        string nName = "";
        if(!normalizedNames.TryGetValue(name, out nName))
        {
            foreach(char letter in name)
            {
                if(string.IsNullOrEmpty(nName))
                {
                    nName += char.ToUpper(letter);
                }
                else
                {
                    if (char.IsUpper(letter))
                        nName += " " + letter;
                    else
                        nName += letter;
                }
            }

            normalizedNames.Add(name, nName);
        }

        return nName;
    }


    static Dictionary<string, Object> guidAssetMapper = new Dictionary<string, Object>();
    static Object GUIDToAsset(string guid, System.Type type)
    {
        Object asset;
        if(!guidAssetMapper.TryGetValue(guid, out asset) || (asset == null))
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            asset = AssetDatabase.LoadAssetAtPath(assetPath, type);
            guidAssetMapper.Remove(guid);
            guidAssetMapper.Add(guid, asset);
        }

        return asset;
    }


    static MethodInfo SetAsset = typeof(AssetLink).GetMethod("SetAsset", BindingFlags.Instance | BindingFlags.NonPublic);



    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        Object asset = null;
        string assetGUID = _property.FindPropertyRelative("assetGUID").stringValue;
        
        if (!string.IsNullOrEmpty(assetGUID))
        {
            asset = GUIDToAsset(assetGUID, NamedAttribute.resourceType);
        }

        if (ResourceLinkTexture != null)
        {
            Rect newPosition = new Rect(_position.xMin - 15, _position.yMin + 3, 14, 14);
            GUI.DrawTexture(newPosition, ResourceLinkTexture, ScaleMode.ScaleToFit, true);
        }
        Object newAsset = EditorGUI.ObjectField(_position, UpperSeparatedName(_property.name), asset, NamedAttribute.resourceType, false);

        bool isAssetChanged = (asset != newAsset) || (asset == null && !string.IsNullOrEmpty(assetGUID));
        bool isMouseRightClick = (Event.current.type == EventType.MouseDown && Event.current.button == 1 && _position.Contains(Event.current.mousePosition));

        if (isAssetChanged || isMouseRightClick)
        {
            object current = _property.serializedObject.targetObject;
            string[] fields = _property.propertyPath.Split('.');

            for (int i = 0; i < fields.Length; i++)
            {
                string fieldName = fields[i];

                if (fieldName.Equals("Array"))
                {
                    fieldName = fields[++i];
                    string indexString = fieldName.Substring(5, fieldName.Length - 6);
                    index = int.Parse(indexString);

                    System.Type type = current.GetType();
                    if (type.IsArray)
                    {
                        System.Array array = current as System.Array;
                        current = array.GetValue(index);
                    }
                    else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        list = current as IList;
                        current = list[index];
                    }
                }
                else
                {
                    FieldInfo field = GetAllFields(current.GetType(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Find((obj) => obj.Name == fields[i]);
                    current = field.GetValue(current);
                }
            }


            if (isMouseRightClick)
            {
                GenericMenu context = new GenericMenu();

                context.AddItem(new GUIContent("Delete Element"), false, DeleteArrayElement);
                context.AddItem(new GUIContent("Duplicate Element"), false, DuplicateArrayElement);
                context.ShowAsContext();
            }
            else
            {
                AssetLink obj = current as AssetLink;
                SetAsset.Invoke(obj, new object[] { newAsset });
//            obj.asset = newAsset;
                _property.serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(_property.serializedObject.targetObject);
            }
        }
    }


    List<FieldInfo> GetAllFields(System.Type type, BindingFlags flags)
    {
        if ((type == typeof(Object)) || (type == typeof(object)))
        {
            return new List<FieldInfo>();
        }

        List<FieldInfo> fields = GetAllFields(type.BaseType, flags);
        // in order to avoid duplicates, force BindingFlags.DeclaredOnly
        fields.AddRange(type.GetFields(flags | BindingFlags.DeclaredOnly));
        return fields;
    }


    void DeleteArrayElement()
    {
        list.RemoveAt(index);
    }


    void DuplicateArrayElement()
    {
        Object asset = (list[index] as AssetLink).GetAsset();
        AssetLink newElement = new AssetLink(asset);
        list.Insert(index + 1, newElement);
    }
}
