using UnityEngine;
using UnityEditor;


public class ShaderFeatureKeywordEnumDrawer : MaterialPropertyDrawer
{
    #region Fields

    private readonly string keyword;
    private readonly GUIContent[] keywords;

    #endregion



    #region Class lifecycle

    public ShaderFeatureKeywordEnumDrawer(string _keyword, string kw1) : this(_keyword, new[] { kw1 }) { }
    public ShaderFeatureKeywordEnumDrawer(string _keyword, string kw1, string kw2) : this(_keyword, new[] { kw1, kw2 }) { }
    public ShaderFeatureKeywordEnumDrawer(string _keyword, string kw1, string kw2, string kw3) : this(_keyword, new[] { kw1, kw2, kw3 }) { }
    public ShaderFeatureKeywordEnumDrawer(string _keyword, string kw1, string kw2, string kw3, string kw4) : this(_keyword, new[] { kw1, kw2, kw3, kw4 }) { }
    public ShaderFeatureKeywordEnumDrawer(string _keyword, string kw1, string kw2, string kw3, string kw4, string kw5) : this(_keyword, new[] { kw1, kw2, kw3, kw4, kw5 }) { }
    public ShaderFeatureKeywordEnumDrawer(string _keyword, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6) : this(_keyword, new[] { kw1, kw2, kw3, kw4, kw5, kw6 }) { }
    public ShaderFeatureKeywordEnumDrawer(string _keyword, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7) : this(_keyword, new[] { kw1, kw2, kw3, kw4, kw5, kw6, kw7 }) { }
    public ShaderFeatureKeywordEnumDrawer(string _keyword, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8) : this(_keyword, new[] { kw1, kw2, kw3, kw4, kw5, kw6, kw7, kw8 }) { }
    public ShaderFeatureKeywordEnumDrawer(string _keyword, string kw1, string kw2, string kw3, string kw4, string kw5, string kw6, string kw7, string kw8, string kw9) : this(_keyword, new[] { kw1, kw2, kw3, kw4, kw5, kw6, kw7, kw8, kw9 }) { }
    public ShaderFeatureKeywordEnumDrawer(string _keyword, params string[] _keywords)
    {
        keyword = _keyword;
        keywords = new GUIContent[_keywords.Length];

        for (int i = 0; i < _keywords.Length; ++i)
        {
            keywords[i] = new GUIContent(_keywords[i]);
        }
    }

    #endregion



    #region Public methods

    public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
    {
        if (!IsPropertyTypeSuitable(prop))
        {
            return IsKeywordEnabled(prop) ? EditorGUIUtility.singleLineHeight * 2.5f : 0f;
        }
        return IsKeywordEnabled(prop) ? base.GetPropertyHeight(prop, label, editor) : 0f;
    }


    public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
        if (IsKeywordEnabled(prop))
        {
            if (!IsPropertyTypeSuitable(prop))
            {
                EditorGUI.HelpBox(position, "KeywordEnum used on a non-float property: " + prop.name, MessageType.Warning);
                return;
            }

            EditorGUI.BeginChangeCheck();

            EditorGUI.showMixedValue = prop.hasMixedValue;
            var value = (int)prop.floatValue;
            value = EditorGUI.Popup(position, label, value, keywords);
            EditorGUI.showMixedValue = false;

            if (EditorGUI.EndChangeCheck())
            {
                prop.floatValue = value;
                SetKeyword(prop, value);
            }
        }
    }


    public override void Apply(MaterialProperty prop)
    {
        base.Apply(prop);
        if (!IsPropertyTypeSuitable(prop))
            return;

        if (prop.hasMixedValue)
            return;

        SetKeyword(prop, (int)prop.floatValue);
    }

    #endregion



    #region Private Methods

    static bool IsPropertyTypeSuitable(MaterialProperty prop)
    {
        return prop.type == MaterialProperty.PropType.Float || prop.type == MaterialProperty.PropType.Range;
    }


    static string GetKeywordName(string propName, string name)
    {
        string n = propName + "_" + name;
        return n.Replace(' ', '_').ToUpperInvariant();
    }


    bool IsKeywordEnabled(MaterialProperty _property)
    {
        bool isKeywordEnabled = true;

        foreach (Material material in _property.targets)
        {
            if (!material.IsKeywordEnabled(keyword))
            {
                isKeywordEnabled = false;
                break;
            }
        }

        return isKeywordEnabled;
    }


    void SetKeyword(MaterialProperty prop, int index)
    {
        for (int i = 0; i < keywords.Length; ++i)
        {
            string keyword = GetKeywordName(prop.name, keywords[i].text);
            foreach (Material material in prop.targets)
            {
                if (index == i)
                    material.EnableKeyword(keyword);
                else
                    material.DisableKeyword(keyword);
            }
        }
    }

    #endregion
}
