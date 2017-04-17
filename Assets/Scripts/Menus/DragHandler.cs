using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public static GameObject item;
	private Vector3 startPos;
	private Vector2 startPivot;
	private int startIndex;
	private Transform startParent;
 
	public void OnBeginDrag (PointerEventData eventData)
	{
		// Get element
		item = gameObject;

		// Get start transformations
		startPos = transform.position;
		startPivot = gameObject.GetComponent<RectTransform> ().pivot;
		startParent = transform.parent;
		startIndex = transform.GetSiblingIndex ();
	}

	public void OnDrag (PointerEventData eventData)
	{
		// Update transformations
		transform.GetComponent<RectTransform> ().pivot = Vector2.up;
		transform.position = new Vector3 (Input.mousePosition.x + 20, Input.mousePosition.y - 20, Input.mousePosition.z);

		// Set canvas as parent
		transform.parent = GameObject.Find ("Canvas").transform;
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		// Reset element
		item = null;

		// Reset parent
		transform.parent = startParent;

		// Reset transformations
		transform.GetComponent<RectTransform> ().pivot = startPivot;
		transform.position = startPos;
		transform.SetSiblingIndex (startIndex);
	}
}
