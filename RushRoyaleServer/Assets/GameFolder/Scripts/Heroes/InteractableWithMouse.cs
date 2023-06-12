using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InteractableWithMouse : MonoBehaviour
{
    private Vector3 _mouseOffset;
    private float _mousePositionZ;
    private Vector3 _startPosition;
    public bool Destroyable = false;

    private void Start()
    {
        Invoke("SetInitialPosition", 1f);
    }

    void OnMouseDown()
    {
        Destroyable = true;
        GetComponent<BoxCollider2D>().enabled = false;
        _mousePositionZ = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        _mouseOffset = gameObject.transform.position - GetMouseAsWorldPoint();
        GetComponent<SpriteRenderer>().sortingLayerName = "OnMouseButtonDrag";
    }

    private void OnMouseUp()
    {
        transform.localScale = new Vector3(.3f, .3f, .3f);
        GetComponent<SpriteRenderer>().sortingLayerName = "Hero";
        GetComponent<BoxCollider2D>().enabled = true;
        transform.DOMove(_startPosition, 1f);
        StartCoroutine(OnButtonDown());
    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = _mousePositionZ;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        transform.localScale = new Vector3(.5f, .5f, .5f);

        transform.position = GetMouseAsWorldPoint() + _mouseOffset;
    }

    IEnumerator OnButtonDown()
    {
        yield return new WaitForSeconds(.2f);
        Destroyable = false;
    }

    private void SetInitialPosition()
    {
        _startPosition = transform.position;
    }

}
