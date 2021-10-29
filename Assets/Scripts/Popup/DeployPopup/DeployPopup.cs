using UnityEngine;
using UnityEngine.UI;
using Varriables;
using Varriables.Setting;
using System;

public class DeployPopup : MonoBehaviour
{
    [SerializeField]
    private Transform gunCamera;

    [SerializeField]
    private GameObject noneMagPrefab;

    [SerializeField]
    private Text gunT;
    [SerializeField]
    private Text sightT;
    [SerializeField]
    private Text tacticalT;
    [SerializeField]
    private Text muzzleT;
    [SerializeField]
    private Text handleT;

    private GunSetting primary = new GunSetting();
    private GunSetting secondary = new GunSetting();

    [SerializeField]
    private Text spawnTimer;

    [SerializeField]
    private Text[] cartridgeNames;
    [SerializeField]
    private InputField[] cartridgeCounts;

    [SerializeField]
    private Text[] grenadesT;

    [SerializeField]
    private Text[] magsI;

    [SerializeField]
    private int[] count;

    [SerializeField]
    private string[] grenades;

    [SerializeField]
    private DeployPopupScrollView deployPopupScrollView;
    [SerializeField]
    private GameObject scrollView;
    [SerializeField]
    private GameObject gun;

    public string displayMode = DisplayMode.Primary;

    private const float gunCameraSpace = 1.5f;

    [SerializeField]
    private bool isSpawn = true;

    void Start()
    {
        spawnTimer.text = "Deploy";
        isSpawn = true;

        Public.weaponLoadout.initMag();
        Public.weaponLoadout.initGrenade();

        string[] _cartridgeNames = new string[]
        { ComponentLoader.getGunScript(Path.loadGun(Setting.weaponLoadoutSetting.primaryGun)).availableBulletType[0], ComponentLoader.getGunScript(Path.loadGun(Setting.weaponLoadoutSetting.secondaryGun)).availableBulletType[0] };
        int[] counts = new int[] { count[0], count[1] };

        for (int i = 0; i < Public.weaponLoadout.mags.Count; i++)
        {
            magsI[i].text = Setting.weaponLoadoutSetting.mags[i];
        }

        primary.gunName = Setting.weaponLoadoutSetting.primaryGun;
        secondary.gunName = Setting.weaponLoadoutSetting.secondaryGun;

        primary.gunSightName = Setting.weaponLoadoutSetting.primarySight;
        secondary.gunSightName = Setting.weaponLoadoutSetting.secondarySight;

        for (int i = 0; i < cartridgeNames.Length; i++)
        {
            cartridgeNames[i].text = _cartridgeNames[i];
            cartridgeCounts[i].text = counts[i].ToString();
        }

        primary.gunTacticalName = Setting.weaponLoadoutSetting.primaryTactical;
        secondary.gunTacticalName = Setting.weaponLoadoutSetting.secondaryTactical;

        primary.gunMuzzleName = Setting.weaponLoadoutSetting.primaryMuzzle;
        secondary.gunMuzzleName = Setting.weaponLoadoutSetting.secondaryMuzzle;

        primary.gunHandleName = Setting.weaponLoadoutSetting.primaryHandle;
        secondary.gunHandleName = Setting.weaponLoadoutSetting.secondaryHandle;

        for(int i = 0; i < grenades.Length; i++)
        {
            grenadesT[i].text = grenades[i];
        }

        onPopupDisplayModeChange(displayMode);
    }

    private void Update()
    {
        if (Public.isGameStarted)
        {
            if (Public.leftTime <= 0)
            {
                spawnTimer.text = "Deploy";
                isSpawn = true;
            }
            else
            {
                spawnTimer.text = ((int)Public.leftTime + 1).ToString() + " seconds left";
                Public.leftTime -= Time.deltaTime;
                isSpawn = false;
            }

        }
        if (displayMode == DisplayMode.Primary)
        {
            gunCamera.position = new Vector3(Public.primaryIntantiatePosition.x - gunCameraSpace, Public.primaryIntantiatePosition.y, Public.primaryIntantiatePosition.z);
        }
        if (displayMode == DisplayMode.Secondary)
        {
            gunCamera.position = new Vector3(Public.secondaryIntantiatePosition.x - gunCameraSpace, Public.secondaryIntantiatePosition.y, Public.secondaryIntantiatePosition.z);
        }
    }

    public void onDeploy()
    {
        if (isSpawn)
        {
            initAll();

            //스폰
            GameObject.FindGameObjectWithTag("Manager").GetComponent<NetworkManager>().spawn(0);
            GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().setPopup(false, Popup.DeployPopup);
        }
    }

    private void initGun(string primaryGunName, string secondaryGunName)
    {
        Destroy(Public.weaponLoadout.primaryWeapon);
        Destroy(Public.weaponLoadout.secondaryWeapon);

        Public.weaponLoadout.currentWeaponType = WeaponType.primary;

        //총기 초기화
        GameObject primaryGun = Path.loadGun(primaryGunName);
        GameObject secondaryGun = Path.loadGun(secondaryGunName);

        try
        {
            Public.weaponLoadout.primaryWeapon = Instantiate(primaryGun, Public.primaryIntantiatePosition, Quaternion.identity);
            Public.weaponLoadout.secondaryWeapon = Instantiate(secondaryGun, Public.secondaryIntantiatePosition, Quaternion.identity);
        }
        catch
        {
            return;
        }

        Public.weaponLoadout.getGunScript(WeaponType.primary).init();
        Public.weaponLoadout.getGunScript(WeaponType.secondary).init();
        Public.weaponLoadout.getGunScript(WeaponType.primary).initGun();
        Public.weaponLoadout.getGunScript(WeaponType.secondary).initGun();
    }

    private void initMag(string[] _mags)
    {
        //탄창 초기화
        for (int i = 0; i < _mags.Length; i++)
        {
            Destroy(Public.weaponLoadout.mags[i]);
            GameObject mag = Path.loadMag(_mags[i]);
            try
            {
                Public.weaponLoadout.mags[i] = Instantiate(mag, Public.magIntantiatePosition, Quaternion.identity);
            }
            catch
            {
                return;
            }
            Public.weaponLoadout.getMagScript(i).init(i, Public.weaponLoadout.getMagScript(i).bulletMaxReloaded);
        }

        GameObject primaryWeaponMag = Instantiate(noneMagPrefab, Public.magIntantiatePosition, Quaternion.identity);
        GameObject secondaryWeaponMag = Instantiate(noneMagPrefab, Public.magIntantiatePosition, Quaternion.identity);

        Public.weaponLoadout.getGunScript(WeaponType.primary).magInstantiate = primaryWeaponMag;
        Public.setParent(
            Public.weaponLoadout.getGunScript(WeaponType.primary).magInstantiate.transform,
            Public.weaponLoadout.getGunScript(WeaponType.primary).magReloadAnimationHolder,
            Vector3.zero,
            Quaternion.identity);
        ComponentLoader.getMagScript(primaryWeaponMag).init(Public.maxMag + 1, 0);

        Public.weaponLoadout.getGunScript(WeaponType.secondary).magInstantiate = secondaryWeaponMag;

        Public.setParent(
            Public.weaponLoadout.getGunScript(WeaponType.secondary).magInstantiate.transform,
            Public.weaponLoadout.getGunScript(WeaponType.secondary).magReloadAnimationHolder,
            Vector3.zero,
            Quaternion.identity);
        ComponentLoader.getMagScript(secondaryWeaponMag).init(Public.maxMag + 2, 0);
    }

    private void initSight(string primarySightName, string secondarySightName)
    {
        try
        {
            //조준경 초기화
            Public.weaponLoadout.getGunScript(WeaponType.primary).sightPrefab = Path.loadSight(primarySightName);
            Public.weaponLoadout.getGunScript(WeaponType.secondary).sightPrefab = Path.loadSight(secondarySightName);

            Public.weaponLoadout.getGunScript(WeaponType.primary).initSight();
            Public.weaponLoadout.getGunScript(WeaponType.secondary).initSight();
        }
        catch
        {
            return;
        }
    }

    private void initTactical(string primaryTacticalName, string secondaryTacticalName)
    {
        try
        {
            //전술 도구 초기화
            Public.weaponLoadout.getGunScript(WeaponType.primary).tacticalPrefab = Path.loadTactical(primaryTacticalName);
            Public.weaponLoadout.getGunScript(WeaponType.secondary).tacticalPrefab = Path.loadTactical(secondaryTacticalName);

            Public.weaponLoadout.getGunScript(WeaponType.primary).initTactical(WeaponType.primary);
            Public.weaponLoadout.getGunScript(WeaponType.secondary).initTactical(WeaponType.secondary);
        }
        catch
        {
            return;
        }
    }

    private void initMuzzle(string primaryMuzzleName, string secondaryMuzzleName)
    {
        try
        {
            //전술 도구 초기화
            Public.weaponLoadout.getGunScript(WeaponType.primary).muzzlePrefab = Path.loadMuzzle(primaryMuzzleName);
            Public.weaponLoadout.getGunScript(WeaponType.secondary).muzzlePrefab = Path.loadMuzzle(secondaryMuzzleName);

            Public.weaponLoadout.getGunScript(WeaponType.primary).initMuzzle();
            Public.weaponLoadout.getGunScript(WeaponType.secondary).initMuzzle();
        }
        catch
        {
            return;
        }
    }

    private void initHandle(string primaryHandleName, string secondaryHandleName)
    {
        try
        {
            //손잡이 초기화
            Public.weaponLoadout.getGunScript(WeaponType.primary).handlePrefab = Path.loadHandle(primaryHandleName);
            Public.weaponLoadout.getGunScript(WeaponType.secondary).handlePrefab = Path.loadHandle(secondaryHandleName);

            Public.weaponLoadout.getGunScript(WeaponType.primary).initHandle();
            Public.weaponLoadout.getGunScript(WeaponType.secondary).initHandle();
        }
        catch
        {
            return;
        }
    }

    private void initBulletCartridge(string[] cartridgeNames, int[] counts)
    {
        try
        {
            Public.weaponLoadout.initBulletCartridge();
            for(int i = 0; i < cartridgeNames.Length; i++)
            {
                Public.weaponLoadout.addBulletCartridge(cartridgeNames[i], counts[i]);
            }
        }
        catch
        {
            return;
        }
    }

    private void initGrenades(string[] grenades)
    {
        try
        {
            for(int i = 0; i < grenades.Length; i++)
            {
                Destroy(Public.weaponLoadout.grenade[i]);
                GameObject grenade = Instantiate(Path.loadGrenade(grenades[i]), Public.grenadeInstantiatePosition, Quaternion.identity);
                Public.weaponLoadout.grenade[i] = grenade;
            }
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void onItemEnter(string text)
    {
        if(displayMode == DisplayMode.Primary)
        {
            if(deployPopupScrollView.displayMode == DisplayMode.Gun)
            {
                primary.gunName = text;

                initGun(primary.gunName, secondary.gunName);

                primary.gunSightName = ComponentLoader.getSightScript(Public.weaponLoadout.getGunScript(WeaponType.primary).defaultSightPrefab).sightName;
                primary.gunTacticalName = ComponentLoader.getTacticalScript(Public.weaponLoadout.getGunScript(WeaponType.primary).defaultTacticalPrefab).tacticalName;
                primary.gunMuzzleName = ComponentLoader.getMuzzleScript(Public.weaponLoadout.getGunScript(WeaponType.primary).defaultMuzzlePrefab).muzzleName;
                primary.gunHandleName = ComponentLoader.getHandleScript(Public.weaponLoadout.getGunScript(WeaponType.primary).defaultHandlePrefab).handleName;
            }
            if(deployPopupScrollView.displayMode == DisplayMode.Sight)
            {
                primary.gunSightName = text;
            }
            if(deployPopupScrollView.displayMode == DisplayMode.Tactical)
            {
                primary.gunTacticalName = text;
            }
            if(deployPopupScrollView.displayMode == DisplayMode.Muzzle)
            {
                primary.gunMuzzleName = text;
            }
            if(deployPopupScrollView.displayMode == DisplayMode.Handle)
            {
                primary.gunHandleName = text;
            }
        }
        if(displayMode == DisplayMode.Secondary)
        {
            if (deployPopupScrollView.displayMode == DisplayMode.Gun)
            {
                secondary.gunName = text;

                initGun(primary.gunName, secondary.gunName);

                secondary.gunSightName = ComponentLoader.getSightScript(Public.weaponLoadout.getGunScript(WeaponType.secondary).defaultSightPrefab).sightName;
                secondary.gunTacticalName = ComponentLoader.getTacticalScript(Public.weaponLoadout.getGunScript(WeaponType.secondary).defaultTacticalPrefab).tacticalName;
                secondary.gunMuzzleName = ComponentLoader.getMuzzleScript(Public.weaponLoadout.getGunScript(WeaponType.secondary).defaultMuzzlePrefab).muzzleName;
                secondary.gunHandleName = ComponentLoader.getHandleScript(Public.weaponLoadout.getGunScript(WeaponType.secondary).defaultHandlePrefab).handleName;
            }
            if (deployPopupScrollView.displayMode == DisplayMode.Sight)
            {
                secondary.gunSightName = text;
            }
            if (deployPopupScrollView.displayMode == DisplayMode.Tactical)
            {
                secondary.gunTacticalName = text;
            }
            if (deployPopupScrollView.displayMode == DisplayMode.Muzzle)
            {
                secondary.gunMuzzleName = text;
            }
            if (deployPopupScrollView.displayMode == DisplayMode.Handle)
            {
                secondary.gunHandleName = text;
            }
        }
        for(int i = 0; i < Public.maxMag; i++)
        {
            if(deployPopupScrollView.displayMode == DisplayMode.Mag + (i+1).ToString())
            {
                magsI[i].text = text;
            }
        }
        for (int i = 0; i < Public.weaponLoadout.bulletCartridges.Count; i++)
        {
            if (deployPopupScrollView.displayMode == DisplayMode.Cartridge + (i + 1).ToString())
            {
                cartridgeNames[i].text = text;
            }
        }

        onPopupDisplayModeChange(displayMode);
        initAll();
    }

    public void onDisplayModeChange(string displayMode)
    {
        deployPopupScrollView.setDisplayMode(displayMode);
    }

    public void initAll()
    {
        initGun(primary.gunName, secondary.gunName);

        string[] _mags = new string[Public.maxMag];
        for (int i = 0; i < _mags.Length; i++)
        {
            _mags[i] = magsI[i].text;
        }

        initMag(_mags);

        initSight(primary.gunSightName, secondary.gunSightName);

        string[] _cartridgeNames = new string[] { cartridgeNames[0].text, cartridgeNames[1].text };
        int[] _counts = new int[] { int.Parse(cartridgeCounts[0].text), int.Parse(cartridgeCounts[1].text) };

        initBulletCartridge(_cartridgeNames, _counts);

        initTactical(primary.gunTacticalName, secondary.gunTacticalName);

        initMuzzle(primary.gunMuzzleName, secondary.gunMuzzleName);

        initHandle(primary.gunHandleName, secondary.gunHandleName);

        string[] _grenades = new string[Public.maxGrenades];
        for (int i = 0; i < _grenades.Length; i++)
        {
            _grenades[i] = grenadesT[i].text;
        }
        initGrenades(_grenades);

        FileLoader.SaveWeaponLoadoutSetting(new WeaponLoadoutSetting(Public.weaponLoadout));
    }

    public void onPopupDisplayModeChange(string text)
    {
        displayMode = text;
        if(displayMode == DisplayMode.Primary)
        {
            gunT.text = primary.gunName;
            sightT.text = primary.gunSightName;
            tacticalT.text = primary.gunTacticalName;
            muzzleT.text = primary.gunMuzzleName;
            handleT.text = primary.gunHandleName;
        }
        if (displayMode == DisplayMode.Secondary)
        {
            gunT.text = secondary.gunName;
            sightT.text = secondary.gunSightName;
            tacticalT.text = secondary.gunTacticalName;
            muzzleT.text = secondary.gunMuzzleName;
            handleT.text = secondary.gunHandleName;
        }
        if(displayMode == DisplayMode.None)
        {
            scrollView.SetActive(false);
            gun.SetActive(false);
        }
        else
        {
            scrollView.SetActive(true);
            gun.SetActive(true);
        }
        deployPopupScrollView.setDisplayMode(deployPopupScrollView.displayMode);
    }
}
