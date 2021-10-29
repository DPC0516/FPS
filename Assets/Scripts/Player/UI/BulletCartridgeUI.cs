using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Varriables;

public class BulletCartridgeUI : MonoBehaviour
{
    [SerializeField]
    private ScrollRect bulletCartirdgeScrollView;

    [SerializeField]
    private GameObject bulletCartirdgeViewPrefab;

    private List<RectTransform> bulletCartirdgeViews = new List<RectTransform>();

    private const float space = 10f;

    private void Start()
    {
        for(int i = 0; i < Public.weaponLoadout.bulletCartridges.Count; i++)
        {
            addView();
        }
    }

    private void Update()
    {
        for(int i = 0; i < bulletCartirdgeViews.Count; i++)
        {
            bulletCartirdgeViews[i].GetComponentsInChildren<Image>()[1].sprite = Public.weaponLoadout.bulletCartridges[i].bulletCartridgeSprite;
            bulletCartirdgeViews[i].GetComponentInChildren<Text>().text = Public.weaponLoadout.bulletCartridges[i].bulletCount.ToString();
        }
    }

    public void addView()
    {
        RectTransform newPlayerScoreView = Instantiate(bulletCartirdgeViewPrefab, bulletCartirdgeScrollView.content).GetComponent<RectTransform>();
        bulletCartirdgeViews.Add(newPlayerScoreView);

        float x = 0f;
        for (int i = 0; i < bulletCartirdgeViews.Count; i++)
        {
            bulletCartirdgeViews[i].anchoredPosition = new Vector2(x, 0f);
            x += bulletCartirdgeViews[i].sizeDelta.y + space;
        }
        bulletCartirdgeScrollView.content.sizeDelta = new Vector2(-x, bulletCartirdgeScrollView.content.sizeDelta.y);
    }

    public void removeView(int index)
    {
        Destroy(bulletCartirdgeViews[index].gameObject);
        bulletCartirdgeViews.RemoveAt(index);

        float x = 0f;
        for (int i = 0; i < bulletCartirdgeViews.Count; i++)
        {
            bulletCartirdgeViews[i].anchoredPosition = new Vector2(x, 0f);
            x += bulletCartirdgeViews[i].sizeDelta.y + space;
        }
        bulletCartirdgeScrollView.content.sizeDelta = new Vector2(-x, bulletCartirdgeScrollView.content.sizeDelta.y);
    }

    public void removeAll()
    {
        for(int i = 0; 0 == bulletCartirdgeViews.Count; i++)
        {
            removeView(i);
        }
    }
}
