/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private static GameObject _item;
    public static string Dir;

    private GameObject _cloned;
    private int _startIndex;
    private Transform _startParent;
    private Vector2 _startPivot;
    private Vector3 _startPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Get element
        _item = gameObject;

        // Get dir
        Dir = transform.parent.name;

        // Get start transformations
        _startPos = transform.position;
        _startPivot = gameObject.GetComponent<RectTransform>().pivot;
        _startParent = transform.parent;
        _startIndex = transform.GetSiblingIndex();

        // Clone element
        _cloned = Instantiate(_item);
        _cloned.transform.SetParent(transform.parent);
        _cloned.transform.SetSiblingIndex(transform.GetSiblingIndex());

        // Update pivot
        transform.GetComponent<RectTransform>().pivot = Vector2.up;

        // Set canvas as parent
        transform.SetParent(GameObject.Find("Canvas").transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Update position
        transform.position = new Vector3(Input.mousePosition.x + 20, Input.mousePosition.y - 20, Input.mousePosition.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Reset element
        _item = null;

        // Remove cloned element
        DestroyImmediate(_cloned);
        _cloned = null;

        // Reset parent
        transform.SetParent(_startParent);

        // Reset transformations
        transform.GetComponent<RectTransform>().pivot = _startPivot;
        transform.position = _startPos;
        transform.SetSiblingIndex(_startIndex);

        // Check if element was dragged correctly
        var dropObj = eventData.pointerCurrentRaycast.gameObject;
        var dropHandler = FindParentWithTag(dropObj, "PlaylistDrop");

        if (dropHandler != null) dropHandler.GetComponent<DropHandler>().OnDrop(eventData);
    }


    private static GameObject FindParentWithTag(GameObject child, string tag)
    {
        var t = child.transform;
        while (t.parent != null)
        {
            if (t.parent.CompareTag(tag)) return t.parent.gameObject;

            t = t.parent.transform;
        }

        return null;
    }
}