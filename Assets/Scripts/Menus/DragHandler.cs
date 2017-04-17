using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public static GameObject item;
	private Vector3 startPos;
	private Vector2 startPivot;
	private Transform startParent;
 
	public void OnBeginDrag (PointerEventData eventData)
	{
		item = gameObject;
		startPos = transform.position;
		startPivot = gameObject.GetComponent<RectTransform> ().pivot;
		startParent = transform.parent;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
	}

	public void OnDrag (PointerEventData eventData)
	{
		transform.GetComponent<RectTransform> ().pivot = Vector2.up;
		transform.position = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 1);
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		item = null;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
		transform.GetComponent<RectTransform> ().pivot = startPivot;
		transform.position = startPos;
	}
}
