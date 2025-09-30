using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnPointerDownEvent;
    public UnityEvent OnPointerUpEvent;
    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownEvent?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpEvent?.Invoke();
    }
}
