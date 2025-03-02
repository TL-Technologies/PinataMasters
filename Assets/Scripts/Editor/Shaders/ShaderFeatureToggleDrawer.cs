using UnityEngine;
using UnityEditor;


public class ShaderFeatureToggleDrawer : MaterialPropertyDrawer
{
    #region Fields

    readonly Color DefaultLineColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    readonly Vector4 DefaultHeaderMargins = new Vector4(5f, 5f, 5f, 0f);

    const string DefaultKeywordSuffix = "_ON";

    private readonly string keyword;
    private readonly string header;

    float propertyHeight;

    #endregion



    #region Classes lifecycle

    public ShaderFeatureToggleDrawer()
    {
    }


    public ShaderFeatureToggleDrawer(string _keyword, string _header = null)
    {
        keyword = _keyword;
        header = _header;
    }

    #endregion



    #region Public Methods

    public override float GetPropertyHeight(MaterialProperty _property, string _label, MaterialEditor _editor)
    {
        if (!IsPropertyTypeSuitable(_property))
        {
            return EditorGUIUtility.singleLineHeight * 2.5f;
        }

        return propertyHeight;
    }


    public override void OnGUI(Rect position, MaterialProperty prop, GUIContent label, MaterialEditor editor)
    {
        propertyHeight = 0f;

        if (!IsPropertyTypeSuitable(prop))
        {
            EditorGUI.HelpBox(position, "Toggle used on a non-float property: " + prop.name, MessageType.Warning);
            return;
        }

        if (!string.IsNullOrEmpty(header))
        {
            DrawShaderFeatureBlockHeader(ref position, DefaultLineColor, DefaultHeaderMargins);
        }

        EditorGUI.BeginChangeCheck();               

        bool value = (Mathf.Abs(prop.floatValue) > 0.001f);
        EditorGUI.showMixedValue = prop.hasMixedValue;

        Rect toggleRect = position;
        toggleRect.height = EditorGUIUtility.singleLineHeight;

        value = EditorGUI.Toggle(toggleRect, label, value);
        EditorGUI.showMixedValue = false;

        if (EditorGUI.EndChangeCheck())
        {
            prop.floatValue = value ? 1.0f : 0.0f;
            SetKeyword(prop, value);
        }

        propertyHeight += EditorGUIUtility.singleLineHeight;
    }


    public override void Apply(MaterialProperty _property)
    {
        base.Apply(_property);

        if (!IsPropertyTypeSuitable(_property))
            return;

        if (_property.hasMixedValue)
            return;

        SetKeyword(_property, (Mathf.Abs(_property.floatValue) > 0.001f));
    }

    #endregion



    #region Protected Methods

    protected virtual void SetKeyword(MaterialProperty _property, bool _on)
    {
        string kw = string.IsNullOrEmpty(keyword) ? _property.name.ToUpperInvariant() + DefaultKeywordSuffix : keyword;

        foreach (Material material in _property.targets)
        {
            if (_on)
            {
                material.EnableKeyword(kw);
            }
            else
            {
                material.DisableKeyword(kw);
            }
        }
    }

    #endregion



    #region Private Methods

    static bool IsPropertyTypeSuitable(MaterialProperty _property)
    {
        return _property.type == MaterialProperty.PropType.Float || _property.type == MaterialProperty.PropType.Range;
    }


    void DrawShaderFeatureBlockHeader(ref Rect _rect, Color _color, Vector4 _headerMargins, float _height = 1f)
    {
        GUIStyle headerLabelStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold
        };

        Vector2 headerSize = headerLabelStyle.CalcSize(new GUIContent(header));

        float linesWidth = (_rect.width - headerSize.x) * 0.5f;

        Rect lineRect = new Rect(_rect.x,
                                 _rect.y + headerSize.y * 0.5f + _headerMargins.y, 
                                 linesWidth - _headerMargins.x, 
                                 _height);

        EditorGUI.DrawRect(lineRect, _color);


        Rect headerRect = new Rect(_rect.x + linesWidth, _rect.y + _headerMargins.y, headerSize.x, headerSize.y);
        EditorGUI.LabelField(headerRect, header, headerLabelStyle);


        lineRect = new Rect(_rect.x + linesWidth + headerSize.x + _headerMargins.z,
                            _rect.y + headerSize.y * 0.5f + _headerMargins.y,
                            linesWidth - _headerMargins.z,
                            _height);

        EditorGUI.DrawRect(lineRect, _color);

        float offset = headerSize.y + _headerMargins.y + _headerMargins.w;

        _rect.y += offset;
        propertyHeight += offset;
    }

    #endregion
}
