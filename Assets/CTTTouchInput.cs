using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Collections;

public class CTTTouchInput : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
	public event Action<PointerEventData> OnTap = delegate{};
	public event Action<PointerEventData> OnTouchStart = delegate{};
	public event Action<PointerEventData> OnTouchEnd = delegate{};
	public event Action<PointerEventData> OnInitializeDrag = delegate{};
	public event Action<PointerEventData> OnDragStart = delegate{};
	public event Action<PointerEventData> OnDragging = delegate{};
	public event Action<PointerEventData> OnDragEnd = delegate{};
	public event Action<PointerEventData> OnDragDrop = delegate{};

	public void OnPointerClick (PointerEventData eventData)
	{
		OnTap(eventData);
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		OnTouchStart(eventData);
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		OnTouchEnd(eventData);
	}

	public void OnInitializePotentialDrag (PointerEventData eventData)
	{
		OnInitializeDrag(eventData);
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		OnDragStart(eventData);
	}

	public void OnDrag (PointerEventData eventData)
	{
		OnDragging(eventData);
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		OnDragEnd(eventData);
	}

	public void OnDrop (PointerEventData eventData)
	{
		OnDragDrop(eventData);
	}
}
