using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif


public interface ILayoutCellHandler
{
    void RepositionForCell(LayoutCellInfo info);
}


public enum GUILayoutCellType
{
    FixedSize,
    Flexible,
    RelativeFixedSize
}


public struct LayoutCellInfo
{
    public GUILayouterType type;
    public AnchorType anchor;
    public Rect cellRect;

    public LayoutCellInfo(GUILayouterType t, AnchorType a, Rect r)
    {
        type = t;
        anchor = a;
        cellRect = r;
    }
}


public class GUILayoutCell : MonoBehaviour 
{
	#region Variables

    [SerializeField] GUILayoutCellType type;
	[SerializeField] List<GameObject> layoutHandlerObjects = new List<GameObject>();
    [SerializeField] protected float sizeValue;
    [SerializeField] AnchorType cellContentAnchor = AnchorType.Center;
    [SerializeField] protected Rect receivedRect;

    protected GUILayouterType receivedType;
    List<ILayoutCellHandler> layoutHandlers = new List<ILayoutCellHandler>();

    string cachedName;
    Transform cachedTransform;

    #endregion



    #region Properties

    public List<GameObject> LayoutHandlerObjects
    {
        get
        {
            return layoutHandlerObjects;
        }
    }


    public Transform CachedTransform
    {
        get
        {
            if (cachedTransform == null)
            {
                cachedTransform = transform;
            }

            return cachedTransform;
        }
    }


    public GUILayoutCellType Type
    {
        get
        {
            return type;
        }
        set 
        {
            type = value;
        }
    }


    public Rect ReceivedRect
    {
        get
        {
            return receivedRect;
        }
    }


    public GUILayouterType ReceivedType
    {
        get
        {
            return receivedType;
        }
    }


    public float SizeValue
    {
        get
        {
            return sizeValue;
        }
        set
        {
            sizeValue = value;

            MultiplyRetinaSize();
        }
    }


    public float FixedSizeValue
    {
        set
        {
            sizeValue = value;
        }
    }


    public AnchorType CellContentAnchor
    {
        get
        {
            return cellContentAnchor;
        }
    }


    public string CachedName
    {
        get
        {
            if (cachedName == null)
            {
                cachedName = gameObject.name;
            }

            return cachedName;
        }
    }

    #endregion



	#region Unity lifecycle

    protected virtual void Awake()
    {
        InitHandlerObjects();
        MultiplyRetinaSize();
    }


    #if UNITY_EDITOR
	void OnDrawGizmos()
	{
        bool isNeedDrawGizmo = false;

		foreach (var go in Selection.gameObjects) 
        {
            if (go == gameObject) 
            {
                isNeedDrawGizmo = true;
                break;
			}
		}

		if (isNeedDrawGizmo) 
		{
			Gizmos.color = new Color(0f, 1f, 0f, 0.5f);

            bool isPositionedHorizontally = false;
            bool isPositionedVertically = false;

            if (ScreenDimensions.Height > receivedRect.size.y)
            {
                isPositionedVertically = true;
            }
            else
            {
                isPositionedVertically = IsPositionedVerticallyInParents(this);
            }

            if (ScreenDimensions.Width > receivedRect.size.x)
            {
                isPositionedHorizontally = true;
            }
            else
            {
                isPositionedHorizontally = IsPositionedHorizontallyInParents(this);
            }

            Vector3 offset = Vector3.zero;

            if (!isPositionedHorizontally)
            {
                offset += Vector3.right * ScreenDimensions.Width * 0.5f;
            }

            if (!isPositionedVertically)
            {
                offset += Vector3.up * ScreenDimensions.Height * 0.5f;
            }

            Gizmos.DrawCube(CachedTransform.position + offset, new Vector3(receivedRect.size.x, receivedRect.size.y, 1f));
		}
	}
	#endif

	#endregion



	#region Public methods

    //TODO hotfix
    //#if UNITY_EDITOR
    public void ResetLayoutHandlers()
    {
        layoutHandlers.Clear();
    }
    //#endif

    public void Reposition(GUILayouterType t, Rect rect)
    {
        receivedRect = rect;
        receivedType = t;

        if (receivedType == GUILayouterType.Horizontal)
        {
			CachedTransform.localPosition = new Vector3(receivedRect.center.x, CachedTransform.localPosition.y, CachedTransform.localPosition.z);
        }
        else if (receivedType == GUILayouterType.Vertical)
        {
			CachedTransform.localPosition = new Vector3(CachedTransform.localPosition.x, receivedRect.center.y, CachedTransform.localPosition.z);
        }

		if (layoutHandlers.Count == 0 && 
            layoutHandlerObjects.Count > 0)
        {
            InitHandlerObjects();
        }

        foreach (ILayoutCellHandler handler in layoutHandlers)
        {
			if (handler != null)
			{
                handler.RepositionForCell(new LayoutCellInfo(receivedType, cellContentAnchor, receivedRect));
			}
        }
    }

	#endregion



	#region Protected methods

    protected virtual void MultiplyRetinaSize()
    {
        if (type == GUILayoutCellType.FixedSize && 
            tk2dSystem.IsRetina)
        {
            sizeValue *= 2;
        }
    }

	#endregion



    #region Private methods

    void InitHandlerObjects()
    {
        foreach (var obj in layoutHandlerObjects)
        {
            if (obj != null)
            {
                ILayoutCellHandler handler = obj.GetComponent<ILayoutCellHandler>();
                if (handler != null)
                {
                    if (!layoutHandlers.Contains(handler))
                    {
                        layoutHandlers.Add(handler);
                    }
                }
                else
                {
                    gameObject.name = CachedName + "(NOT_FOUND_HANDLERS)";
                    CustomDebug.LogWarning("no handlers found in GUILayoutCell references, gameObject name = " + CachedName, this);
                }
            }
            else
            {
                gameObject.name = CachedName + "(NOT_FOUND_HANDLERS)";
                CustomDebug.LogWarning("no handlers found in GUILayoutCell references, gameObject name = " + CachedName, this);
            }
        }
    }


    #if UNITY_EDITOR
    static GUILayoutCell GetParentCell(GUILayouter layouter)
    {
        GUILayoutCell[] cells = layouter.GetComponentsInParent<GUILayoutCell>();

        foreach (var c in cells)
        {
            foreach (var obj in c.LayoutHandlerObjects)
            {
                if (obj == layouter.gameObject)
                {
                    return c;
                }
            }
        }

        return null;
    }


    static bool IsPositionedVerticallyInParents(GUILayoutCell cell)
    {
        if (cell != null)
        {
            GUILayouter topLayouter = cell.transform.parent.GetComponent<GUILayouter>();

            if (topLayouter != null)
            {
                if (topLayouter.Type == GUILayouterType.Vertical)
                {
                    return true;
                }
                else
                {
                    return IsPositionedVerticallyInParents(GetParentCell(topLayouter));
                }
            }
        }

        return false;
    }


    bool IsPositionedHorizontallyInParents(GUILayoutCell cell)
    {
        if (cell != null)
        {
            GUILayouter topLayouter = cell.transform.parent.GetComponent<GUILayouter>();

            if (topLayouter != null)
            {
                if (topLayouter.Type == GUILayouterType.Horizontal)
                {
                    return true;
                }
                else
                {
                    return IsPositionedHorizontallyInParents(GetParentCell(topLayouter));
                }
            }
        }

        return false;
    }
    #endif

    #endregion
}