using UnityEngine;
using UnityEngine.UI;

public class ScrollbarOnlyScroll : ScrollRect
{
    public override void OnScroll(UnityEngine.EventSystems.PointerEventData data)
    {
        // Do nothing → disables scroll wheel input
    }
}
