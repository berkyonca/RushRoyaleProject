using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DamageIndicator : MonoBehaviour
{
    public float yPos = 1f;
    public float durationTime = .2f;
    public float scaleValue = 1.5f;
    public float lifeTime = .1f;

    private void Awake()
    {
        transform.DOScale(scaleValue, .2f);
        transform.DOMoveY(transform.position.y + yPos, durationTime);
        Destroy(gameObject, lifeTime);
    }





}
