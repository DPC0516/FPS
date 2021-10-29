using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Varriables;

public class DeployPopupScrollView : MonoBehaviour
{
    [SerializeField]
    private ScrollRect deployPopupScrollView;

    [SerializeField]
    private GameObject deployPopupViewPrefab;

    private List<RectTransform> deployPopupViews = new List<RectTransform>();

    private const float space = 25f;

    [SerializeField]
    private DeployPopup deployPopup;

    public string displayMode = DisplayMode.Gun;

    public void Start()
    {
        setDisplayMode(DisplayMode.Gun);
    }

    public void setDisplayMode(string _displayMode)
    {
        displayMode = _displayMode;
        try
        {
            removeAll();
        }
        catch { }
        if (displayMode == DisplayMode.Gun)
        {
            for (int i = 0; i < Guns.availableGunNames.Length; i++)
            {
                addView(Guns.availableGunNames[i]);
            }
        }
        if (deployPopup.displayMode == DisplayMode.Primary)
        {
            if(displayMode == DisplayMode.Sight)
            {
                for (int i = 0; i < Public.weaponLoadout.getGunScript(WeaponType.primary).availableSight.Length; i++)
                {
                    addView(ComponentLoader.getSightScript(Public.weaponLoadout.getGunScript(WeaponType.primary).availableSight[i]).sightName);
                }
            }
            if(displayMode == DisplayMode.Tactical)
            {
                for (int i = 0; i < Public.weaponLoadout.getGunScript(WeaponType.primary).availableTactical.Length; i++)
                {
                    addView(ComponentLoader.getTacticalScript(Public.weaponLoadout.getGunScript(WeaponType.primary).availableTactical[i]).tacticalName);
                }
            }
            if(displayMode == DisplayMode.Muzzle)
            {
                for (int i = 0; i < Public.weaponLoadout.getGunScript(WeaponType.primary).availableMuzzle.Length; i++)
                {
                    addView(ComponentLoader.getMuzzleScript(Public.weaponLoadout.getGunScript(WeaponType.primary).availableMuzzle[i]).muzzleName);
                }
            }
            if (displayMode == DisplayMode.Handle)
            {
                for (int i = 0; i < Public.weaponLoadout.getGunScript(WeaponType.primary).availableHandle.Length; i++)
                {
                    addView(ComponentLoader.getHandleScript(Public.weaponLoadout.getGunScript(WeaponType.primary).availableHandle[i]).handleName);
                }
            }
        }
        if(deployPopup.displayMode == DisplayMode.Secondary)
        {
            if (displayMode == DisplayMode.Sight)
            {
                for (int i = 0; i < Public.weaponLoadout.getGunScript(WeaponType.secondary).availableSight.Length; i++)
                {
                    addView(ComponentLoader.getSightScript(Public.weaponLoadout.getGunScript(WeaponType.secondary).availableSight[i]).sightName);
                }
            }
            if (displayMode == DisplayMode.Tactical)
            {
                for (int i = 0; i < Public.weaponLoadout.getGunScript(WeaponType.secondary).availableTactical.Length; i++)
                {
                    addView(ComponentLoader.getTacticalScript(Public.weaponLoadout.getGunScript(WeaponType.secondary).availableTactical[i]).tacticalName);
                }
            }
            if (displayMode == DisplayMode.Muzzle)
            {
                for (int i = 0; i < Public.weaponLoadout.getGunScript(WeaponType.secondary).availableMuzzle.Length; i++)
                {
                    addView(ComponentLoader.getMuzzleScript(Public.weaponLoadout.getGunScript(WeaponType.secondary).availableMuzzle[i]).muzzleName);
                }
            }
            if (displayMode == DisplayMode.Handle)
            {
                for (int i = 0; i < Public.weaponLoadout.getGunScript(WeaponType.secondary).availableHandle.Length; i++)
                {
                    addView(ComponentLoader.getHandleScript(Public.weaponLoadout.getGunScript(WeaponType.secondary).availableHandle[i]).handleName);
                }
            }
        }
        if (displayMode.Contains(DisplayMode.Mag))
        {
            for(int i = 0; i < Mags.availableMagNames.Length; i++)
            {
                addView(Mags.availableMagNames[i]);
            }
        }
        if (displayMode.Contains(DisplayMode.Cartridge))
        {
            for (int i = 0; i < BulletType.bulletTypes.Length; i++)
            {
                addView(BulletType.bulletTypes[i]);
            }
        }
    }

    public void addView(string text)
    {
        RectTransform newPlayerScoreView = Instantiate(deployPopupViewPrefab, deployPopupScrollView.content).GetComponent<RectTransform>();
        deployPopupViews.Add(newPlayerScoreView);
        newPlayerScoreView.GetComponentInChildren<Text>().text = text;

        float y = 0f;
        for (int i = 0; i < deployPopupViews.Count; i++)
        {
            deployPopupViews[i].anchoredPosition = new Vector2(0f, y);
            y += deployPopupViews[i].sizeDelta.y + space;
        }
        deployPopupScrollView.content.sizeDelta = new Vector2(deployPopupScrollView.content.sizeDelta.x, 2 * y);
    }

    public void removeView(int index)
    {
        Destroy(deployPopupViews[index].gameObject);
        deployPopupViews.RemoveAt(index);

        float y = 0f;
        for (int i = 0; i < deployPopupViews.Count; i++)
        {
            deployPopupViews[i].anchoredPosition = new Vector2(0f, y);
            y += deployPopupViews[i].sizeDelta.y + space;
        }
        deployPopupScrollView.content.sizeDelta = new Vector2(deployPopupScrollView.content.sizeDelta.x, 2 * y);
    }

    public void removeAll()
    {
        int a = deployPopupViews.Count;
        for (int i = 0; i < a;  i++)
        {
            Destroy(deployPopupViews[i].gameObject);
        }
        deployPopupViews = new List<RectTransform>(); 
        deployPopupScrollView.content.sizeDelta = new Vector2(deployPopupScrollView.content.sizeDelta.x, 0f);
    }
}
