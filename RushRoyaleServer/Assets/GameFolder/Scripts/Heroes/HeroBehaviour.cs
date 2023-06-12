using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBehaviour : MonoBehaviour
{
    private int ID;

    [SerializeField]
    private GameObject[] heroLevelImages;

    public int Level = 1;

    public int spawnPosListNumber;

    private void Start()
    {
        SetHeroLevelImage();
        ID = GetInstanceID();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<HeroSpawnArea>() != null && !GetComponent<InteractableWithMouse>().Destroyable)
        {
            spawnPosListNumber = collision.gameObject.GetComponent<HeroSpawnArea>().ListNumber;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponents<HeroBehaviour>() != null)
        {
            if (collision.gameObject.GetComponent<InteractableWithMouse>().Destroyable && collision.gameObject.GetComponent<HeroBehaviour>().Level == Level && Level < 3)
            {
                OnMerge(collision.gameObject.GetComponent<HeroBehaviour>().spawnPosListNumber);
                Destroy(collision.gameObject);
            }
        }
    }


    private void OnMerge(int value)
    {
        Level++;
        SetHeroLevelImage();
        EventManager.Instance.OnHeroManaLevelUp?.Invoke(GetComponent<Hero>());
        EventManager.Instance.OnMergeHero?.Invoke(value);
    }

    private void SetHeroLevelImage()
    {
        switch (Level)
        {
            case 1:
                heroLevelImages[0].SetActive(true);
                break;
            case 2:
                heroLevelImages[0].SetActive(false);
                heroLevelImages[1].SetActive(true);
                GetComponent<SpriteRenderer>().color = new Color32(50, 229, 145, 255);
                break;
            case 3:
                heroLevelImages[1].SetActive(false);
                heroLevelImages[2].SetActive(true);
                GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            default:
                break;
        }
    }

}
