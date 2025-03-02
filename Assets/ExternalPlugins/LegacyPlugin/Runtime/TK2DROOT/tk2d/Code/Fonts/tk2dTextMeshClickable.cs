using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


public class tk2dTextMeshClickable : tk2dTextMesh
{
    [Serializable]
    public class ClickableLink
    {
        public string clickableText;
        public BoxCollider boxCollider;
        [HideInInspector]
        public Bounds bounds;
    }

    #region Variables

    [SerializeField] Color clickableColor;
    [SerializeField] ClickableLink[] clickables;

    [SerializeField] float lineThickness = 5;
    [SerializeField] float lineUpOffset = 8;
    [SerializeField] Vector2 colliderExtends;

    string originalText;

    [SerializeField] GameObject underliningGO;
    [SerializeField] MeshFilter underliningMeshFilter;
    [SerializeField] MeshRenderer underliningMeshRenderer;
    #endregion


    #region Properties

    public MeshFilter UnderliningMeshFilter
    {
        get
        {
            if (underliningMeshFilter == null)
            {
                CreateUnderliningGO();
            }
            return underliningMeshFilter;
        }
    }

    public MeshRenderer UnderliningMeshRenderer
    {
        get
        {
            if (underliningMeshRenderer == null)
            {
                CreateUnderliningGO();
            }
            return underliningMeshRenderer;
        }
    }

    public GameObject UnderliningGO
    {
        get
        {
            if (underliningGO == null)
            {
                CreateUnderliningGO();
            }
            return underliningGO;
        }
    }

    #endregion


    #region ILayoutCellHandler

    public override void RepositionForCell(LayoutCellInfo info)
    {
        if (Application.isPlaying)
        {
            SetColors();
        }
        base.RepositionForCell(info);
        Rebuild();
    }

    #endregion


    #region Public methods

    public void Rebuild () 
    {
        string formattedText = GetFormattedText();

        foreach(ClickableLink clickable in clickables)
        {
            if (!IsValidSettings(clickable))
            {
                continue;
            }
            Bounds bounds = GetBounds(clickable, formattedText);
            clickable.bounds = bounds;
            clickable.bounds.extents /= 2;
            PlaceBoxCollider(clickable, bounds);
        }
        PlaceUnderlining();

    }

    #endregion


    #region Private methods

    void SetColors()
    {
        if (string.IsNullOrEmpty(originalText))
        {
            originalText = text;
        }
        else
        {
            text = originalText;
        }
           
        foreach (ClickableLink clickable in clickables)
        {
            text = text.Replace(clickable.clickableText, GetColorTag(clickableColor) + clickable.clickableText + GetColorTag(color));
        }
    }


    Bounds GetBounds(ClickableLink clickable, string formattedText)
    {
        Bounds bounds = new Bounds();

        int startChar = formattedText.IndexOf(clickable.clickableText);
        int endChar = startChar + (clickable.clickableText.Length - 1);
        if (endChar != -1 && startChar != -1)
        {
            int verteciesPerChar = 4;
            Vector3[] vertices = CachedMeshFilter.sharedMesh.vertices;
            Matrix4x4 transformMatrix = transform.localToWorldMatrix;

            Vector3 point1 = transformMatrix.MultiplyPoint(vertices[startChar * verteciesPerChar]);
            Vector3 point2 = transformMatrix.MultiplyPoint(vertices[startChar * verteciesPerChar + 2]);
            Vector3 point3 = transformMatrix.MultiplyPoint(vertices[endChar * verteciesPerChar + 1]);

            Vector3 extents = new Vector3();
            Vector3 center = new Vector3();

            extents.x = (point3.x - point1.x);
            extents.y = FontInst.lineHeight / 4; // extends and height from center
            center.x = (point1.x + point3.x) / 2;
            center.y = (point1.y + point2.y) / 2;

            bounds.extents = extents;
            bounds.center = center;
        }

        return bounds;
    }


    bool IsValidSettings(ClickableLink clickable)
    {
        if (clickable.boxCollider == null || string.IsNullOrEmpty(clickable.clickableText))
        {
            return false;
        } 
        return true;
    }


    string GetFormattedText()
    {
        string formattedText = FormattedText.ToString();
        formattedText = formattedText.Replace(Environment.NewLine, "");
        return formattedText;
    }


    void PlaceBoxCollider(ClickableLink clickable, Bounds bounds)
    {
        Vector3 size = new Vector3(bounds.extents.x + colliderExtends.x, bounds.extents.y + colliderExtends.y, 100);
        Vector3 center = new Vector3(bounds.center.x, bounds.center.y, transform.position.y);
        if (clickable.boxCollider != null)
        {
            clickable.boxCollider.center = transform.worldToLocalMatrix.MultiplyPoint(center);
            clickable.boxCollider.size = size;
        }
    }


    void PlaceUnderlining()
    {
        
        if (UnderliningMeshFilter.sharedMesh == null)
        {
            UnderliningMeshFilter.sharedMesh = new Mesh();
        }
        Mesh mainMesh = UnderliningMeshFilter.sharedMesh;
        mainMesh.Clear();

        List<CombineInstance> combineInstances = new List<CombineInstance>();

        for(int i = 0; i < clickables.Length; i++)
        {
            CombineInstance combineInstance = new CombineInstance();
            Mesh mesh = new Mesh();
            Bounds bounds = clickables[i].bounds;
            Vector3[] vertices = new Vector3[4]; 

            Vector3 center = UnderliningGO.transform.worldToLocalMatrix.MultiplyPoint(bounds.center) - (Vector3.up * ((bounds.extents.y) - lineUpOffset));


            vertices[0] = center + new Vector3(- bounds.extents.x, lineThickness, 0);  
            vertices[1] = center + new Vector3(bounds.extents.x, lineThickness, 0);
            vertices[2] = center + new Vector3(bounds.extents.x, -lineThickness, 0);
            vertices[3] = center + new Vector3(- bounds.extents.x, - lineThickness, 0);

            mesh.vertices = vertices;
            Color[] colors = new Color[mesh.vertices.Length];
            for(int c = 0; c < mesh.vertices.Length; c++)
            {
                colors[c] = clickableColor;
            }
            mesh.colors = colors;

            int[] triangles = new int[] {0, 1, 2, 2, 3, 0};

            mesh.SetTriangles(triangles, 0);

            combineInstance.mesh = mesh;
            combineInstance.transform = Matrix4x4.identity;
            combineInstances.Add(combineInstance);
        }

        mainMesh.CombineMeshes(combineInstances.ToArray());
    }


    void CreateUnderliningGO()
    {
        underliningGO = new GameObject("Underlining");
        underliningGO.transform.parent = transform;
        underliningMeshFilter = underliningGO.AddComponent<MeshFilter>();
        underliningMeshRenderer = underliningGO.AddComponent<MeshRenderer>();

        underliningMeshRenderer.sharedMaterial = new Material(GetComponent<MeshRenderer>().sharedMaterial.shader);
    }


    string GetColorTag(Color c)
    {
        string result = "^C" + ColorUtility.ToHtmlStringRGBA(c);
        return result;
    }
       
    #endregion
}
