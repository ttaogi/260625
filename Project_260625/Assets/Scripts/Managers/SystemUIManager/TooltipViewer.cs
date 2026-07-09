using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipViewer : MonoBehaviour
{
    #region Inspector
    public EventTrigger blockTouch;

    [Space]
    public Canvas canvasParent;
    public CanvasGroup canvasGroupTooltipGroup;
    public GameObject objTooltipGroup;
    public GameObject objTooltip;
    public ContentSizeFitter fitTooltip;
    public TMP_Text textTitle;
    public TMP_Text textDesc;
    #endregion Inspector

    private RectTransform _rectThis = null;
    private RectTransform _rectCanvasParent = null;
    private RectTransform _rectTooltip = null;
    private Coroutine _coSetPosition = null;

    /////////////////////////////////////////////////

    protected void Awake()
    {
        blockTouch.SetOnClickEvent(HideTooltip);
    }

    public void Init()
    {
        canvasGroupTooltipGroup.alpha = 0f;
        textTitle.text = string.Empty;
        textDesc.text = string.Empty;

        _rectThis = GetComponent<RectTransform>();
        _rectCanvasParent = canvasParent.GetComponent<RectTransform>();
        _rectTooltip = objTooltip.GetComponent<RectTransform>();

        objTooltipGroup.SetActive(false);
    }



    public void ShowTooltip(Transform target, string title, string desc)
    {
        textTitle.text = title;
        textDesc.text = desc;

        if (_coSetPosition != null)
            StopCoroutine(_coSetPosition);

        _coSetPosition = StartCoroutine(SetPosition(target));
    }

    public void HideTooltip()
    {
        canvasGroupTooltipGroup.alpha = 0f;
        objTooltipGroup.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectThis, Input.mousePosition, null, out Vector2 localPoint))
                Debug.Log($"mouse : {Input.mousePosition}, localPoint: {localPoint}"); //로컬 좌표 디버깅
    }

    public void TT(Transform target)
    {
        Camera camera = Utils.GetCamera(eLayer.UI);
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(camera, target.position);
        Utils.Log($" scr : {screenPos} tar : {target.position}");
    }

    private IEnumerator SetPosition(Transform target)
    {
        Camera camera = Utils.GetCamera(eLayer.UI);

        if (camera == null)
        {
            HideTooltip();
            yield break;
        }

        objTooltipGroup.SetActive(true);
        canvasGroupTooltipGroup.alpha = 0f;

        yield return null;

        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectThis);
        fitTooltip.SetLayoutHorizontal();
        fitTooltip.SetLayoutVertical();

        yield return null;

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(camera, target.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectCanvasParent, screenPos, camera, out Vector2 localPoint);

        float tooltipWidthHalf = _rectTooltip.rect.width / 2;
        float tooltipHeightHalf = _rectTooltip.rect.height / 2;

        Vector2 tooltipPos = new(localPoint.x + tooltipWidthHalf, localPoint.y + tooltipHeightHalf);
        float checkOverX = tooltipPos.x + tooltipWidthHalf - Screen.width / 2;
        float checkOverY = tooltipPos.y + tooltipHeightHalf - Screen.height / 2;

        Utils.Log($"[TooltipViewer] screenPos : {screenPos}, localPoint : {localPoint}");

        if (checkOverX > 0)
            tooltipPos.x -= checkOverX;
        if (checkOverY > 0)
            tooltipPos.y -= checkOverY;

        _rectTooltip.anchoredPosition = tooltipPos;

        canvasGroupTooltipGroup.alpha = 1f;

        yield break;
    }
}
