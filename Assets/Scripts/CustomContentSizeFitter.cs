using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics.Tracing;

[AddComponentMenu("Layout/Custom Content Size Fitter", 141)]
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
/// <summary>
/// Resizes a RectTransform to fit the size of its content.
/// </summary>
/// <remarks>
/// The ContentSizeFitter can be used on GameObjects that have one or more ILayoutElement components, such as Text, Image, HorizontalLayoutGroup, VerticalLayoutGroup, and GridLayoutGroup.
/// </remarks>
public class CustomContentSizeFitter : UIBehaviour, ILayoutSelfController
{
    /// <summary>
    /// The size fit modes avaliable to use.
    /// </summary>
    public enum FitMode
    {
        /// <summary>
        /// Don't perform any resizing.
        /// </summary>
        Unconstrained,
        /// <summary>
        /// Resize to the minimum size of the content.
        /// </summary>
        MinSize,
        /// <summary>
        /// Resize to the preferred size of the content.
        /// </summary>
        PreferredSize
    }

    [SerializeField] protected FitMode m_HorizontalFit = FitMode.Unconstrained;

    /// <summary>
    /// The fit mode to use to determine the width.
    /// </summary>
    public FitMode horizontalFit { get { return m_HorizontalFit; } set { if (SetPropertyUtility.SetStruct(ref m_HorizontalFit, value)) SetDirty(); } }

    [SerializeField] protected FitMode m_VerticalFit = FitMode.Unconstrained;

    /// <summary>
    /// The fit mode to use to determine the height.
    /// </summary>
    public FitMode verticalFit { get { return m_VerticalFit; } set { if (SetPropertyUtility.SetStruct(ref m_VerticalFit, value)) SetDirty(); } }
    
    [SerializeField] private RectTransform Source;
    [SerializeField] private RectTransform Target;
    [SerializeField] private float Padding = 0f;
    [System.NonSerialized] private RectTransform m_Rect;
    private RectTransform rectTransform
    {
        get
        {
            if (Source != null)
                m_Rect = Source;
            else
                m_Rect = GetComponent<RectTransform>();
            if (Target != null)
                return Target;
            else
                return GetComponent<RectTransform>();
        }
    }

    // field is never assigned warning
    #pragma warning disable 649
    private DrivenRectTransformTracker m_Tracker;
    #pragma warning restore 649

    protected CustomContentSizeFitter()
    {}

    protected override void OnEnable()
    {
        base.OnEnable();
        SetDirty();
    }

    protected override void OnDisable()
    {
        m_Tracker.Clear();
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        base.OnDisable();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        SetDirty();
    }

    private void HandleSelfFittingAlongAxis(int axis)
    {
        FitMode fitting = (axis == 0 ? horizontalFit : verticalFit);
        if (fitting == FitMode.Unconstrained)
        {
            // Keep a reference to the tracked transform, but don't control its properties:
            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.None);
            return;
        }

        m_Tracker.Add(this, rectTransform, (axis == 0 ? DrivenTransformProperties.SizeDeltaX : DrivenTransformProperties.SizeDeltaY));

        // Set size to min or preferred size
        if (fitting == FitMode.MinSize)
            rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetMinSize(m_Rect, axis) + Padding);
        else
            rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis)axis, LayoutUtility.GetPreferredSize(m_Rect, axis) + Padding);
    }

    /// <summary>
    /// Calculate and apply the horizontal component of the size to the RectTransform
    /// </summary>
    public virtual void SetLayoutHorizontal()
    {
        m_Tracker.Clear();
        HandleSelfFittingAlongAxis(0);
    }

    /// <summary>
    /// Calculate and apply the vertical component of the size to the RectTransform
    /// </summary>
    public virtual void SetLayoutVertical()
    {
        HandleSelfFittingAlongAxis(1);
    }

    protected void SetDirty()
    {
        if (!IsActive())
            return;

        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

internal static class SetPropertyUtility
{
    public static bool SetColor(ref Color currentValue, Color newValue)
    {
        if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
            return false;

        currentValue = newValue;
        return true;
    }

    public static bool SetStruct<T>(ref T currentValue, T newValue) where T: struct
    {
        if (currentValue.Equals(newValue))
            return false;

        currentValue = newValue;
        return true;
    }

    public static bool SetClass<T>(ref T currentValue, T newValue) where T: class
    {
        if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
            return false;

        currentValue = newValue;
        return true;
    }
}

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        SetDirty();
    }

#endif
}

