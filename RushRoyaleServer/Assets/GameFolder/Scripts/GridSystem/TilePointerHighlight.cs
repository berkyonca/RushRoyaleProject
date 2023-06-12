using UnityEngine;

//Make tile red when mouse over at the merge screen
public class TilePointer : MonoBehaviour
{
    [SerializeField] LayerMask _boardTileLayerMask;

    SpriteRenderer _tileSprite;

    void Update()
    {
        if (_tileSprite != null)
        {
            SpriteRenderer spriteRenderer = _tileSprite.GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(1, 1, 1, .3f);
            _tileSprite = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitGround, 1000f, _boardTileLayerMask))
        {
            SpriteRenderer spriteRenderer = hitGround.transform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.red;
            }
            _tileSprite = spriteRenderer;
        }
    }
}