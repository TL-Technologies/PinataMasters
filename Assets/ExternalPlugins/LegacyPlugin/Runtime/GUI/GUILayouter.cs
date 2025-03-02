using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum GUILayouterType
{
    Horizontal,
    Vertical
}


public enum GUILayouterRotationType
{
    Both,
    Portrait,
    Landscape
}


public class GUILayouter : MonoBehaviour, ILayoutCellHandler 
{
    #region Variables

    [SerializeField] GUILayouterType type = GUILayouterType.Horizontal;
    [HideInInspector][SerializeField] GUILayouterRotationType rotationType = GUILayouterRotationType.Both;
    [SerializeField] bool isRootLayouter = true;
    [SerializeField] bool isInversed;

    GUILayoutCell[] cells;
    bool isDirty;
    Rect? occupiedPixels = null;

    Transform cachedTransform;

    #endregion



    #region Properties

    public GUILayouterRotationType RotationType
    {
        get
        {
            return rotationType;
        }
        set
        {
            if (!Application.isPlaying)
            {
                rotationType = value;
            }
        }
    }


    public GUILayouterType Type
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


    public bool IsRootLayouter
    {
        get
        {
            return isRootLayouter;
        }
        set
        {
            isRootLayouter = value;
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


    public Rect? OccupiedPixels
    {
        get
        {
            return occupiedPixels;
        }
        set
        {
            occupiedPixels = value;

            if (cells == null || cells.Length == 0)
            {
                cells = GetComponentsInChildren<GUILayoutCell>(true);

                List<GUILayoutCell> myCells = new List<GUILayoutCell>();

                foreach (var cell in cells)
                {
                    if (cell.transform.parent == CachedTransform)
                    {
                        myCells.Add(cell);
                    }
                }

                cells = myCells.ToArray();
            }

            UpdateLayout();
        }
    }

    #endregion



    #region ILayoutCellHandler

    public void RepositionForCell(LayoutCellInfo info)
    {        
        if (info.type == GUILayouterType.Horizontal)
        {
            OccupiedPixels = info.cellRect;
        }
        else if (info.type == GUILayouterType.Vertical)
        {
            OccupiedPixels = info.cellRect;
        }

        if (info.anchor == AnchorType.Left)
        {
            CachedTransform.localPosition = new Vector3(-info.cellRect.width * 0.5f, 0f, CachedTransform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Right)
        {
            CachedTransform.localPosition = new Vector3(info.cellRect.width * 0.5f, 0f, CachedTransform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Top)
        {
            CachedTransform.localPosition = new Vector3(0f, info.cellRect.height * 0.5f, CachedTransform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Bottom)
        {
            CachedTransform.localPosition = new Vector3(0f, -info.cellRect.height * 0.5f, CachedTransform.localPosition.z);
        }
        else if (info.anchor == AnchorType.Center)
        {
            CachedTransform.localPosition = new Vector3(0f, 0f, CachedTransform.localPosition.z);
        }
    }

    #endregion



	#region Unity Lifecycle

    protected virtual void Awake()
    {
        if (IsRootLayouter)
        {
            GUIOrientationManager.OnDeviceOrientationChanged += GUIOrientationManager_OnDeviceOrientationChanged;
        }

		ResetLayouter();
    }


    protected virtual void OnEnable()
    {
        if (isDirty)
        {
            isDirty = false;
            StartCoroutine(CoroutineInitialize());
        }
    }
        

    protected virtual void Start()
    {
        if ((rotationType != GUILayouterRotationType.Both) && 
            IsRootLayouter)
        {
            gameObject.SetActive(rotationType == GUIOrientationManager.CurrentRotationType);
        }

		Initialize();
    }


	protected virtual void OnDestroy()
	{
		if (IsRootLayouter)
		{
			GUIOrientationManager.OnDeviceOrientationChanged -= GUIOrientationManager_OnDeviceOrientationChanged;
		}
	}

	#endregion


    #region Public methods

    public void Initialize()
    {
        if (occupiedPixels == null)
        {            
            int screenWidth = ScreenDimensions.Width;
            int screenHeight = ScreenDimensions.Height;

            int width = screenWidth;
            int height = screenHeight;
            bool isLandscapeOrientation;

            if (rotationType == GUILayouterRotationType.Both)
            {
                isLandscapeOrientation = (GUIOrientationManager.CurrentRotationType == GUILayouterRotationType.Landscape);
            }
            else
            {
                isLandscapeOrientation = (rotationType == GUILayouterRotationType.Landscape);
            }

            width = (isLandscapeOrientation) ? (Mathf.Max(screenWidth, screenHeight)) : (Mathf.Min(screenWidth, screenHeight));
            height = (isLandscapeOrientation) ? (Mathf.Min(screenWidth, screenHeight)) : (Mathf.Max(screenWidth, screenHeight));

            OccupiedPixels = new Rect?(new Rect(0f, 0f, width, height));
        }
    }


    public void ResetLayouter()
    {
        occupiedPixels = null;
        cells = null;
    }


    public void UpdateLayout()
    {
        if (cells == null || 
            cells.Length == 0 || 
            cells[0] == null)
        {
            return;
        }

        int availablePixels = (int)((type == GUILayouterType.Horizontal) ? 
            (occupiedPixels.GetValueOrDefault().width) : 
            (occupiedPixels.GetValueOrDefault().height));
        int occupiedFixedSize = 0;
        float flexibleAreasTotalWeight = 0f;

        bool hasFlexibleAreas = false;

        for (int i = 0; i < cells.Length; i++)
        {
            GUILayoutCell cell = cells[i];
            if (cell.Type == GUILayoutCellType.FixedSize)
            {
                occupiedFixedSize += (int)cell.SizeValue;
            }
            else if (cell.Type == GUILayoutCellType.RelativeFixedSize)
            {
                occupiedFixedSize += (int)(cell.SizeValue * availablePixels);
            }
            else if (cell.Type == GUILayoutCellType.Flexible)
            {
                hasFlexibleAreas = true;
                flexibleAreasTotalWeight += cell.SizeValue;
            }
        }

        int unoccupiedArea = availablePixels - occupiedFixedSize;

        if (unoccupiedArea > 0 && 
            flexibleAreasTotalWeight < float.Epsilon && 
            hasFlexibleAreas)
        {
            CustomDebug.LogError(gameObject.name + ": Weights for flexible cells is 0!");
        }

        //setting cells sizes
        int currentPosition = (isInversed) ? (availablePixels) : (0);

        for (int i = 0; i < cells.Length; i++)
        {
            GUILayoutCell cell = cells[i];
            Rect cellRect = new Rect();
            if (type == GUILayouterType.Horizontal)
            {
                if (cell.Type == GUILayoutCellType.FixedSize)
                {
                    cellRect = new Rect(0f, 0f, cell.SizeValue, occupiedPixels.GetValueOrDefault().height);
                }
                else if (cell.Type == GUILayoutCellType.RelativeFixedSize)
                {
                    cellRect = new Rect(0f, 0f, cell.SizeValue * availablePixels, occupiedPixels.GetValueOrDefault().height);
                }
                else if (cell.Type == GUILayoutCellType.Flexible)
                {
                    cellRect = new Rect(0f, 0f, cell.SizeValue * (float)unoccupiedArea / (float)flexibleAreasTotalWeight, occupiedPixels.GetValueOrDefault().height);
                }
                if (isInversed)
                {
                    cellRect.center = new Vector3(currentPosition - cellRect.width * 0.5f, 0f, 0f);
                    currentPosition -= (int)cellRect.width;
                }
                else
                {
                    cellRect.center = new Vector3(currentPosition + cellRect.width * 0.5f, 0f, 0f);
                    currentPosition += (int)cellRect.width;
                }
            }
            else if (type == GUILayouterType.Vertical)
            {
                if (cell.Type == GUILayoutCellType.FixedSize)
                {
                    cellRect = new Rect(0f, cell.SizeValue * 0.5f, occupiedPixels.GetValueOrDefault().width, cell.SizeValue);
                }
                else if (cell.Type == GUILayoutCellType.RelativeFixedSize)
                {
                    cellRect = new Rect(0f, 0f, occupiedPixels.GetValueOrDefault().width, cell.SizeValue * availablePixels);
                }
                else if (cell.Type == GUILayoutCellType.Flexible)
                {
                    cellRect = new Rect(0f, 0f, occupiedPixels.GetValueOrDefault().width, cell.SizeValue * (float)unoccupiedArea / (float)flexibleAreasTotalWeight);
                }
                if (isInversed)
                {
                    cellRect.center = new Vector3(0f, currentPosition - cellRect.height * 0.5f, 0f);
                    currentPosition -= (int)cellRect.height;
                }
                else
                {
                    cellRect.center = new Vector3(0f, currentPosition + cellRect.height * 0.5f, 0f);
                    currentPosition += (int)cellRect.height;
                }
            }
            cell.Reposition(type, cellRect);
        }
    }


    #if UNITY_EDITOR
    public void UpdateLayoutDebug(Vector3 debugScreenSize)
    {
        occupiedPixels = new Rect?(new Rect(0f, 0f, debugScreenSize.x, debugScreenSize.y));
        cells = GetComponentsInChildren<GUILayoutCell>(true);
        List<GUILayoutCell> myCells = new List<GUILayoutCell>();

        foreach (var cell in cells)
        {
            if (cell.transform.parent == CachedTransform)
            {
                myCells.Add(cell);
            }
        }

        cells = myCells.ToArray();

        UpdateLayout();
    }
    #endif

    #endregion



    #region Private methods

    IEnumerator CoroutineInitialize()
    {
        yield return new WaitForEndOfFrame();
        Initialize();
    }

    #endregion



    #region Events Handlers

    void GUIOrientationManager_OnDeviceOrientationChanged(GUILayouterRotationType newRotationType)
    {
        if (this.IsNull())
        {
            GUIOrientationManager.OnDeviceOrientationChanged -= GUIOrientationManager_OnDeviceOrientationChanged;
            return;
        }

        if (rotationType != GUILayouterRotationType.Both)
		{
            gameObject.SetActive(rotationType == newRotationType);
		}

        ResetLayouter();

        if (isActiveAndEnabled)
        {
            StartCoroutine(CoroutineInitialize());
        }
        else
        {
            isDirty = true;
        }
    }

    #endregion
}