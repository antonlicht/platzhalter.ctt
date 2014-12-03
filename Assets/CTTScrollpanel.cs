using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CTTScrollpanel : MonoBehaviour 
{
	public enum ScrollpanelOrientation {Horizontal, Vertical}

	public ScrollpanelOrientation Orientation;
	public float ScrollSpeed = 4f;
	private float ScrollThreshold = 1f;

	private List<RectTransform> _elements;
	private List<float> _positions;
	private RectTransform _rectTransform;
	private RectTransform _selected;
	private Vector2 _selectedPosition;
	private bool _refresh;
	private bool _scroll;

	#region Properties
	public RectTransform SelectedElement
	{
		get{return _selected;}
	}

	public float ScrollState
	{
		get
		{
			if(Orientation == ScrollpanelOrientation.Horizontal)
			{
				return (RectTransform.anchoredPosition.x - _selectedPosition.x)/RectTransform.rect.width;
			}
			return (RectTransform.anchoredPosition.y - _selectedPosition.y)/RectTransform.rect.height;
		}
	}

	public int Count {get{return _elements.Count;}}

	private List<RectTransform> Elements {
		get
		{
			if(_elements == null)
			{
				_elements = new List<RectTransform>();
			}
			return _elements;
		}
	}

	private RectTransform RectTransform
	{
		get
		{
			if(_rectTransform == null)
			{
				_rectTransform = GetComponent<RectTransform>();
			}
			return _rectTransform;
		}
	}
	#endregion

	void Awake()
	{
		var hlg = GetComponent<HorizontalLayoutGroup>();
		var vlg = GetComponent<VerticalLayoutGroup>();

		if(Orientation == ScrollpanelOrientation.Horizontal)
		{
			if(vlg != null)
			{
				Destroy(vlg);
			}
			if(hlg == null)
			{
				gameObject.AddComponent<HorizontalLayoutGroup>();
			}
		}
		else
		{
			if(hlg != null)
			{
				Destroy(vlg);
			}
			if(vlg == null)
			{
				gameObject.AddComponent<VerticalLayoutGroup>();
			}
		}
	}

	#region Add and Remove
	public void AddElement(RectTransform element)
	{
		Remove(element);
		Elements.Add(element);
		AddElementToTransform(element);
	}

	public void AddElementAt(RectTransform element, int index)
	{
		Remove(element);
		Elements.Insert(index, element);
		AddElementToTransform(element);
	}

	public void AddElementAfter(RectTransform element, RectTransform position)
	{
		Remove(element);
		var index = Elements.IndexOf(position) + 1;
		Elements.Insert(index, element);
		AddElementToTransform(element);
	}

	public void AddElementBefore(RectTransform element, RectTransform position)
	{
		Remove(element);
		var index = Elements.IndexOf(position);
		Elements.Insert(index, element);
		AddElementToTransform(element);
	}

	private void AddElementToTransform(RectTransform element)
	{
		var layout = element.gameObject.AddComponent<LayoutElement>();
		layout.minWidth = RectTransform.rect.width;
        layout.minHeight = RectTransform.rect.height;

		element.SetParent(RectTransform);
		element.localScale = Vector3.one;
		_refresh = true;
	}

	public RectTransform Remove(RectTransform element)
	{
		if(Elements.Contains(element))
		{
			Elements.Remove(element);
			_refresh = true;
			element.parent = null;
			return element;
		}
		return null;
	}
	#endregion

	#region Selection
	public void SelectImmediately(RectTransform element)
	{
		Select(element);
		RectTransform.anchoredPosition = _selectedPosition;
	}

	public void Select(RectTransform element)
	{
		Refresh();
		
		var id = _elements.IndexOf(element);
		Select (id);

	}

	public void Select(int id)
	{
		_selected = _elements[id];
		var pos = RectTransform.anchoredPosition;
		if(Orientation == ScrollpanelOrientation.Horizontal)
		{
			pos.x = - _positions[id];
		}
		else
		{
			pos.y = - _positions[id];
		}
		_selectedPosition = pos;
	}

	public void SelectNext(int steps)
	{
		var id = _elements.IndexOf(_selected);
		var newId = Mathf.Clamp(id+steps, 0, _elements.Count-1);
		Select (newId);
	}
	
	public void SelectNext(int steps, int minId, int maxId)
	{
		var id = _elements.IndexOf(_selected);
		var newId = Mathf.Clamp(id+steps, minId, maxId);
		newId = Mathf.Clamp (newId, 0, _elements.Count-1);
		Select (newId);
	}
	#endregion

	public void SetRubberBandPosition(Vector2 delta)
	{
		_scroll = false;
		if(Orientation == ScrollpanelOrientation.Horizontal)
		{
			var start = RectTransform.anchoredPosition.x;
			var end = _selectedPosition.x + Mathf.Sign(delta.x) * RectTransform.rect.width/2f;
			var time = Mathf.Abs(delta.x)/RectTransform.rect.width;
			var newPosX = Mathf.Lerp(start, end, time);
			RectTransform.anchoredPosition = new Vector2(newPosX,_selectedPosition.y);
		}
		else
		{
			var start = RectTransform.anchoredPosition.y;
			var end = _selectedPosition.y + Mathf.Sign(delta.y) * RectTransform.rect.height/2f;
			var time = Mathf.Abs(delta.y)/RectTransform.rect.height;
			var newPosY = Mathf.Lerp(start, end, time);
			RectTransform.anchoredPosition = new Vector2(_selectedPosition.x,newPosY);
		}
	}

	public void StartScroll()
	{
		_scroll = true;
	}

	private void Scroll()
	{
		if(Orientation == ScrollpanelOrientation.Horizontal)
		{
			var start = RectTransform.anchoredPosition.x;
			var end = _selectedPosition.x;
			var time = Time.deltaTime*ScrollSpeed;
			var newPosX = Mathf.Lerp(start, end, time);
			RectTransform.anchoredPosition = new Vector2(newPosX,_selectedPosition.y);
			if(Mathf.Abs(newPosX - end)<= ScrollThreshold)
			{
				_scroll = false;
				RectTransform.anchoredPosition = _selectedPosition;
			}
		}
		else
		{
			var start = RectTransform.anchoredPosition.y;
			var end = _selectedPosition.y;
			var time = Time.deltaTime*ScrollSpeed;
			var newPosY = Mathf.Lerp(start, end, time);
			RectTransform.anchoredPosition = new Vector2(_selectedPosition.x,newPosY);
			if(Mathf.Abs(newPosY - end)<= ScrollThreshold)
			{
				_scroll = false;
				RectTransform.anchoredPosition = _selectedPosition;
			}
		}
	}

	public void Refresh()
	{
		if(_positions == null)
		{
			_positions = new List<float>();
		}
		else
		{
			_positions.Clear();
		}
		var pos = 0f;
		foreach (RectTransform element in _elements)
		{
			element.SetAsLastSibling();
			_positions.Add(pos);
			if(Orientation == ScrollpanelOrientation.Horizontal)
            {
				pos += RectTransform.rect.width;
			}
			else
            {
				pos -= RectTransform.rect.height;
			}

		}
	}

	void Update()
	{
		if(_refresh)
		{
			_refresh = false;
			Refresh ();
		}
		if(_scroll)
		{
			Scroll();
		}
	}
}
