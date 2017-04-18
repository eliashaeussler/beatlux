using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public static GameObject item;
	private GameObject cloned;
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

		// Clone element
		cloned = Instantiate<GameObject> (item);
		cloned.transform.SetParent (transform.parent);
		cloned.transform.SetSiblingIndex (transform.GetSiblingIndex ());
	}

	public void OnDrag (PointerEventData eventData)
	{
		// Update transformations
		transform.GetComponent<RectTransform> ().pivot = Vector2.up;
		transform.position = new Vector3 (Input.mousePosition.x + 20, Input.mousePosition.y - 20, Input.mousePosition.z);

		// Set canvas as parent
		transform.SetParent (GameObject.Find ("Canvas").transform);
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		// Reset element
		item = null;

		// Remove cloned element
		GameObject.DestroyImmediate (cloned);
		cloned = null;

		// Reset parent
		transform.SetParent (startParent);

		// Reset transformations
		transform.GetComponent<RectTransform> ().pivot = startPivot;
		transform.position = startPos;
		transform.SetSiblingIndex (startIndex);
	}

	public static void End (DropHandler drop, PlaylistObj playlist)
	{
		drop = null;

		Playlist pl = Camera.main.GetComponent <Playlist> ();
		pl.ToggleFiles (playlist, true);
	}
}
