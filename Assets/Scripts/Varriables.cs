using UnityEngine;
using Photon.Pun;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Varriables.Setting;

//각종 변수 관리
namespace Varriables
{
    //총기 발사 모드
    public class FireMode
    {
        public const int FIREMODE_AUTO = 0;
        public const int FIREMODE_BURST = 1;
        public const int FIREMODE_SEMI = 2;
        public const int FIREMODE_SAFE = 3;

        public static string[] FIREMODE_STRING_LIST = new string[] { "AUTO", "BURST", "SEMI", "SAFE "};
    }

    //키보드 키 관리 변수
    public class Key
    {
        public static KeyCode Map = KeyCode.M;
        public static KeyCode[] Alphas = new KeyCode[] 
        {KeyCode.Alpha1, KeyCode.Alpha2 , KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0};
        public static KeyCode PrimaryWeapon = KeyCode.Alpha1;
        public static KeyCode SecondaryWeapon = KeyCode.Alpha2;
        public static KeyCode Relaod = KeyCode.R;
        public static KeyCode ChangeFireMode = KeyCode.V;
        public static KeyCode Jump = KeyCode.Space;
        public static KeyCode Run = KeyCode.LeftShift;
        public static KeyCode PeekRight = KeyCode.E;
        public static KeyCode PeekLeft = KeyCode.Q;
        public static KeyCode FreeLook = KeyCode.LeftAlt;
        public static KeyCode ReloadChamber = KeyCode.C;
        public static KeyCode InsertBulletInMag = KeyCode.T;
        public static KeyCode Cancel = KeyCode.C;
        public static KeyCode ScoreTab = KeyCode.Tab;
        public static KeyCode Tactical = KeyCode.B;
        public static KeyCode Crouch = KeyCode.Z;
        public static KeyCode Crawl = KeyCode.X;
        public static int Fire = 0;
        public static int Aim = 1;
    }

    public class Public
    {
        public static LogManager logManager;
        public static WeaponLoadout weaponLoadout = new WeaponLoadout();
        public static GameMode gameMode = new GameMode(GameModeSetting.defaultSetting);
        public static bool isJoin = false;
        public const float defaultFOV = 60f;
        public static bool isPause = false;
        public static string userName = "default";
        public static string id;
        public static string roomName = "default";
        public static float currentTime = 0;
        public static int score = 0;
        public static int death = 0;
        public static int kill = 0;
        public const int maxMag = 7;
        public const int maxGrenades = 3;
        public static float leftTime = 5f;
        public const float defaultLeftTime = 5f;
        public static bool isGameStarted = false;
        public static float currentGameTime = 0f;
        public const float maxIntensity = 2.5f;
        public const float minIntensity = 0.02f;
        public const int maxCartridge = 2;
        public static Vector3 primaryIntantiatePosition = new Vector3(0f, -5f, 0f);
        public static Vector3 secondaryIntantiatePosition = new Vector3(0f, -5f, 10f);
        public static Vector3 magIntantiatePosition = new Vector3(0f, -5f, 20f);
        public static Vector3 grenadeInstantiatePosition = new Vector3(0f, -5f, 30f);

        public static WeaponLoadout getWeaponLoadout(PhotonView PV, WeaponLoadout _weaponLoadout)
        {
            if (PV.IsMine)
            {
                return weaponLoadout;
            }
            else
            {
                return _weaponLoadout;
            }
        }

        public static void setParent(Transform child, Transform parent, Vector3 localPosition, Quaternion localRotation)
        {
            DebugLog("setParent", child.name + " to " + parent.name, null);
            child.parent = parent;
            child.localPosition = localPosition;
            child.localRotation = localRotation;
            child.localScale = new Vector3(1f, 1f, 1f);
        }

        public static void DebugLog(string tag, string message, PhotonView PV)
        {
#if DEBUGING
            if(PV != null)
            {
                Debug.Log("<" + tag + "> " + message + "(" + PV.ViewID + ")");
            }
            else
            {
                Debug.Log("<" + tag + "> " + message);
            }
#endif
        }
    }

    public class GameMode
    {
        public int maxPlayer;
        public string gameModeName;
        public bool isScore;
        public bool isKill;
        public bool isTime;
        public int targetScore;
        public int targetKill;
        public int targetTime;
        public string mapName;
        public bool isDayNightSystem;
        public bool isDay;
        public int dayNightTime;

        public GameMode(GameModeSetting gameModeSetting)
        {
            this.maxPlayer = gameModeSetting.maxPlayer;
            this.gameModeName = gameModeSetting.gameModeName;
            this.isScore = gameModeSetting.isScore;
            this.isKill = gameModeSetting.isKill;
            this.isTime = gameModeSetting.isTime;
            this.targetScore = gameModeSetting.targetScore;
            this.targetKill = gameModeSetting.targetKill;
            this.targetTime = gameModeSetting.targetTime;
            this.mapName = gameModeSetting.mapName;
            this.isDayNightSystem = gameModeSetting.isDayNightSystem;
            this.isDay = gameModeSetting.isDay;
            this.dayNightTime = gameModeSetting.dayNightTime;
        }

        public GameMode(
            int maxPlayer, 
            string gameModeName, 
            bool isScore, 
            bool isKill, 
            bool isTime, 
            int targetScore,
            int targetKill, 
            int targetTime, 
            string mapName,
            bool isDayNightSystem,
            bool isDay,
            int dayNightTime)
        {
            this.maxPlayer = maxPlayer;
            this.gameModeName = gameModeName;
            this.isScore = isScore;
            this.isKill = isKill;
            this.isTime = isTime;
            this.targetScore = targetScore;
            this.targetKill = targetKill;
            this.targetTime = targetTime;
            this.mapName = mapName;
            this.isDayNightSystem = isDayNightSystem;
            this.isDay = isDay;
            this.dayNightTime = dayNightTime;
        }
    }

    namespace Setting
    {
        using Varriables;

        public class Setting
        {
            public static GraphicSetting graphicSetting = GraphicSetting.defaultSetting;
            public static DefaultSetting defaultSetting = DefaultSetting.defaultSetting;
            public static WeaponLoadoutSetting weaponLoadoutSetting = WeaponLoadoutSetting.defaultSetting;
            public static GameModeSetting gameModeSetting = GameModeSetting.defaultSetting;

            public static void Save()
            {
                FileLoader.Save(new GraphicSetting(graphicSetting), new DefaultSetting(defaultSetting));
            }

            public static void Load()
            {
                defaultSetting = FileLoader.LoadDefaultSetting();
                graphicSetting = FileLoader.LoadGraphicSetting();
                weaponLoadoutSetting = FileLoader.LoadWeaponLoadoutSetting();
                gameModeSetting = FileLoader.LoadGameModeSetting();
                Public.gameMode = new GameMode(gameModeSetting);
            }
        }

        [System.Serializable]
        public class GraphicSetting
        {
            public static GraphicSetting defaultSetting = new GraphicSetting(1080, 1920, true);

            public int height;
            public int width;
            public bool isFullScreen;

            public GraphicSetting(GraphicSetting graphicSetting)
            {
                this.height = graphicSetting.height;
                this.width = graphicSetting.width;
                this.isFullScreen = graphicSetting.isFullScreen;
            }
            public GraphicSetting(int height, int width, bool isFullScreen)
            {
                this.height = height;
                this.width = width;
                this.isFullScreen = isFullScreen;
            }

            public void setScreenSize()
            {
                Screen.SetResolution(width, height, isFullScreen);
            }
        }

        [System.Serializable]
        public class DefaultSetting
        {
            public static DefaultSetting defaultSetting = new DefaultSetting(100f, 100f);

            public const float changeFreeLookRotation = 5f;
            public float mouseSensitivityMax = 1000f;

            public float mouseSensitivityX;
            public float mouseSensitivityY;

            public DefaultSetting(DefaultSetting defaultSetting)
            {
                this.mouseSensitivityX = defaultSetting.mouseSensitivityX;
                this.mouseSensitivityY = defaultSetting.mouseSensitivityY;
            }
            public DefaultSetting(float mouseSensitivityX, float mouseSensitivityY)
            {
                this.mouseSensitivityX = mouseSensitivityX;
                this.mouseSensitivityY = mouseSensitivityY;
            }
        }

        [System.Serializable]
        public class WeaponLoadoutSetting
        {
            public static WeaponLoadoutSetting defaultSetting = 
                new WeaponLoadoutSetting(
                    "TestGun", "TestGun", "None", "None", "None",
                    "TestGun2", "TestGun2", "None", "None", "None",
                    new string[] { "TestGun", "TestGun", "TestGun", "TestGun", "TestGun2", "TestGun2", "TestGun2" },
                    new string[] { "5.56", "9"},
                    new int[] { 100, 50 });

            public WeaponLoadoutSetting(
                string primaryGun,
                string primarySight,
                string primaryTactical,
                string primaryMuzzle,
                string primaryHandle,
                string secondaryGun,
                string secondarySight,
                string secondaryTactical,
                string secondaryMuzzle,
                string secondaryHandle,
                string[] mags, 
                string[] cartridges, 
                int[] counts)
            {
                this.primaryGun = primaryGun;
                this.primarySight = primarySight;
                this.primaryTactical = primaryTactical;
                this.primaryMuzzle = primaryMuzzle;
                this.primaryHandle = primaryHandle;
                this.secondaryGun = secondaryGun;
                this.secondarySight = secondarySight;
                this.secondaryTactical = secondaryTactical;
                this.secondaryMuzzle = secondaryMuzzle;
                this.secondaryHandle = secondaryHandle;
                this.mags = mags;
                this.cartridges = cartridges;
                this.counts = counts;
            }
            public WeaponLoadoutSetting(WeaponLoadout weaponLoadout)
            {
                this.primaryGun = weaponLoadout.getGunScript(WeaponType.primary).gunName;
                this.secondaryGun = weaponLoadout.getGunScript(WeaponType.secondary).gunName;

                this.primarySight = weaponLoadout.getGunScript(WeaponType.primary).getSightScript().sightName;
                this.secondarySight = weaponLoadout.getGunScript(WeaponType.secondary).getSightScript().sightName;

                this.primaryTactical = weaponLoadout.getGunScript(WeaponType.primary).getTacticalScript().tacticalName;
                this.secondaryTactical = weaponLoadout.getGunScript(WeaponType.secondary).getTacticalScript().tacticalName;

                this.primaryMuzzle = weaponLoadout.getGunScript(WeaponType.primary).getMuzzleScript().muzzleName;
                this.secondaryMuzzle = weaponLoadout.getGunScript(WeaponType.secondary).getMuzzleScript().muzzleName;

                this.primaryHandle = weaponLoadout.getGunScript(WeaponType.primary).getHandleScript().handleName;
                this.secondaryHandle = weaponLoadout.getGunScript(WeaponType.secondary).getHandleScript().handleName;

                string[] _mags = new string[weaponLoadout.mags.Count];
                for(int i = 0; i< _mags.Length; i++)
                {
                    _mags[i] = weaponLoadout.getMagScript(i).magName;
                }

                string[] _cartridges = new string[weaponLoadout.bulletCartridges.Count];
                for (int i = 0; i < _cartridges.Length; i++)
                {
                    _cartridges[i] = weaponLoadout.bulletCartridges[i].bulletType;
                }

                int[] _counts = new int[weaponLoadout.bulletCartridges.Count];
                for (int i = 0; i < _counts.Length; i++)
                {
                    _counts[i] = weaponLoadout.bulletCartridges[i].bulletCount;
                }
                this.mags = _mags;
                this.cartridges = _cartridges;
                this.counts = _counts;
            }

            public string primaryGun;
            public string primarySight;
            public string primaryTactical;
            public string primaryMuzzle;
            public string primaryHandle;
            public string secondaryGun;
            public string secondarySight;
            public string secondaryTactical;
            public string secondaryMuzzle;
            public string secondaryHandle;
            public string[] mags;
            public string[] cartridges;
            public int[] counts;
        }

        [System.Serializable]
        public class GameModeSetting
        {
            public static GameModeSetting defaultSetting = new GameModeSetting("Default", 60, 0, 0, 5, true, false, false, "Default", true, true, 600);

            public GameModeSetting(
                string gameModeName, 
                int targetScore, 
                int targetKill, 
                int targetTime, 
                int maxPlayer, 
                bool isScore, 
                bool isKill, 
                bool isTime, 
                string mapName, 
                bool isDayNightSystem,
                bool isDay,
                int dayNightTime)
            {
                this.gameModeName = gameModeName;
                this.isScore = isScore;
                this.isKill = isKill;
                this.isTime = isTime;
                this.targetScore = targetScore;
                this.targetKill = targetKill;
                this.targetTime = targetTime;
                this.maxPlayer = maxPlayer;
                this.mapName = mapName;
                this.isDayNightSystem = isDayNightSystem;
                this.isDay = isDay;
                this.dayNightTime = dayNightTime;
            }

            public GameModeSetting(GameMode gameMode)
            {
                this.gameModeName = gameMode.gameModeName;
                this.targetScore = gameMode.targetScore;
                this.targetKill = gameMode.targetKill;
                this.targetTime = gameMode.targetTime;
                this.maxPlayer = gameMode.maxPlayer;
                this.isScore = gameMode.isScore;
                this.isKill = gameMode.isKill;
                this.isTime = gameMode.isTime;
                this.mapName = gameMode.mapName;
                this.isDayNightSystem = gameMode.isDayNightSystem;
                this.isDay = gameMode.isDay;
                this.dayNightTime = gameMode.dayNightTime;
            }

            public int maxPlayer;
            public string gameModeName;
            public int targetScore;
            public int targetKill;
            public int targetTime;
            public bool isScore;
            public bool isKill;
            public bool isTime;
            public string mapName;
            public bool isDayNightSystem;
            public bool isDay;
            public int dayNightTime;
        }
    }

    public class FileLoader
    {
        private const string graphicSettingFile = "GraphicSetting.bin";
        private const string defaultSettingFile = "DefaultSetting.bin";
        private const string weaponLoadoutSettingFile = "WeaponLoadoutSetting.bin";
        private const string gameModeSettingFile = "GameModeSettingFile.bin";

        public static void Save(GraphicSetting graphicSetting, DefaultSetting defaultSetting)
        {
            checkDirectory();

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(Path.settingPath + graphicSettingFile, FileMode.Create);

            Public.DebugLog("FileLoader", "Save: " + Path.settingPath + graphicSettingFile, null);

            formatter.Serialize(stream, graphicSetting);
            stream.Close();

            stream = new FileStream(Path.settingPath + defaultSettingFile, FileMode.Create);

            Public.DebugLog("FileLoader", "Save: " + Path.settingPath + defaultSettingFile, null);

            formatter.Serialize(stream, defaultSetting);
        }

        public static void SaveWeaponLoadoutSetting(WeaponLoadoutSetting weaponLoadoutSetting)
        {
            checkDirectory();

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(Path.settingPath + weaponLoadoutSettingFile, FileMode.Create);
            Public.DebugLog("FileLoader", "Save: " + Path.settingPath + weaponLoadoutSettingFile, null);
            formatter.Serialize(stream, weaponLoadoutSetting);
            stream.Close();
        }

        public static void SaveGameModeSetting(GameModeSetting gameModeSetting)
        {
            checkDirectory();

            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(Path.settingPath + gameModeSettingFile, FileMode.Create);
            Public.DebugLog("FileLoader", "Save: " + Path.settingPath + gameModeSettingFile, null);
            formatter.Serialize(stream, gameModeSetting);
            stream.Close();
        }

        public static GraphicSetting LoadGraphicSetting()
        {
            checkDirectory();

            if (File.Exists(Path.settingPath + graphicSettingFile))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(Path.settingPath + graphicSettingFile, FileMode.Open);

                Public.DebugLog("FileLoader", "Load: " + Path.settingPath + graphicSettingFile, null);

                GraphicSetting _graphicSetting = formatter.Deserialize(stream) as GraphicSetting;

                stream.Close();

                return _graphicSetting;
            }
            else
            {
                return GraphicSetting.defaultSetting;
            }
        }

        public static DefaultSetting LoadDefaultSetting()
        {
            checkDirectory();

            if (File.Exists(Path.settingPath + defaultSettingFile))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(Path.settingPath + defaultSettingFile, FileMode.Open);

                Public.DebugLog("FileLoader", "Load: " + Path.settingPath + defaultSettingFile, null);

                DefaultSetting _defaultSetting = formatter.Deserialize(stream) as DefaultSetting;

                stream.Close();

                return _defaultSetting;
            }
            else
            {
                return DefaultSetting.defaultSetting;
            }
        }

        public static WeaponLoadoutSetting LoadWeaponLoadoutSetting()
        {
            checkDirectory();

            if (File.Exists(Path.settingPath + weaponLoadoutSettingFile))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(Path.settingPath + weaponLoadoutSettingFile, FileMode.Open);

                Public.DebugLog("FileLoader", "Load: " + Path.settingPath + weaponLoadoutSettingFile, null);

                WeaponLoadoutSetting _defaultSetting = formatter.Deserialize(stream) as WeaponLoadoutSetting;

                stream.Close();

                return _defaultSetting;
            }
            else
            {
                return WeaponLoadoutSetting.defaultSetting;
            }
        }

        public static GameModeSetting LoadGameModeSetting()
        {
            checkDirectory();

            if (File.Exists(Path.settingPath + gameModeSettingFile))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(Path.settingPath + gameModeSettingFile, FileMode.Open);

                Public.DebugLog("FileLoader", "Load: " + Path.settingPath + gameModeSettingFile, null);

                GameModeSetting _defaultSetting = formatter.Deserialize(stream) as GameModeSetting;

                stream.Close();

                return _defaultSetting;
            }
            else
            {
                return GameModeSetting.defaultSetting;
            }
        }

        private static void checkDirectory()
        {
            if (!Directory.Exists(Path.settingPath))
            {
                Directory.CreateDirectory(Path.settingPath);
            }
        }

    }

    //speed 리스트의 각 인덱스 변수
    public class Speed
    {
        public const int reloadTime = 0;
        public const int reloadChamberTime = 1;
        public const int fireSpeed = 2;
        public const int burstSpeed = 3;
        public const int aimSpeed = 4;
        public const int restoreRecoilSpeed = 5;    
        public const int changeFireModeSpeed = 6;
        public const int sliderPullSpeed = 7;
        public const int sliderLetSpeed = 8;
        public const int sliderRestoreRecoilSpeed = 9;
        public const int magInOutSpeed = 10;
        public const int magToPocketSpeed = 11;
    }

    public class WeaponType
    {
        public const int primary = 0;
        public const int secondary = 1;
        public const int all = 2;
        public static string[] weaponTypes = new string[] { "Primary", "Secondary", "All" };
    }

    public class Popup
    {
        public const int DeployPopup = 0;
        public const int ExitPopup = 1;
    }

    public class Path
    {
        public const string gunPrefabPath = "Prefabs/Guns/Guns/";
        public const string sightPrefabPath = "Prefabs/Guns/Sights/";
        public const string magPrefabPath = "Prefabs/Guns/Mags/";
        public const string sliderPrefabPath = "Prefabs/Guns/Sliders/";
        public const string emptyShellPrefaPath = "Prefabs/Bullets/EmptyShells/";
        public const string bulletsPrefabPath = "Prefabs/Bullets/Bullets/";
        public const string bulletPrefabPath = "Prefabs/Bullets/";
        public const string bulletCartridgeSpritePath = "Image/BulletCartridge/";
        public const string tacticalPrefabPath = "Prefabs/Guns/Tacticals/";
        public static string settingPath = Application.dataPath + "/Settings/";
        public const string mapPath = "Maps/";
        public const string bulletHole = "Prefabs/Bullets/BulletHoles/";
        public const string muzzlePrefabPath = "Prefabs/Guns/Muzzles/";
        public const string handlePrefabPath = "Prefabs/Guns/Handles/";
        public const string grenadesPrefabPath = "Prefabs/grenades/";

        public static GameObject loadGun(string gunName)
        {
            return Resources.Load(gunPrefabPath + gunName + "_Gun") as GameObject;
        }

        public static GameObject loadSight(string sightName)
        {
            return Resources.Load(sightPrefabPath + sightName + "_Sight") as GameObject;
        }

        public static GameObject loadMag(string magName)
        {
            return Resources.Load(magPrefabPath + magName + "_Mag") as GameObject;
        }

        public static GameObject loadSlider(string sliderName) 
        {
            return Resources.Load(sliderPrefabPath + sliderName + "_Slider") as GameObject;
        }

        public static GameObject loadTactical(string tacticalName)
        {
            return Resources.Load(tacticalPrefabPath + tacticalName + "_Tactical") as GameObject;
        }

        public static GameObject loadMuzzle(string muzzleName)
        {
            return Resources.Load(muzzlePrefabPath + muzzleName + "_Muzzle") as GameObject;
        }

        public static GameObject loadHandle(string handleName)
        {
            return Resources.Load(handlePrefabPath + handleName + "_Handle") as GameObject;
        }

        public static GameObject loadGrenade(string grenadeName)
        {
            return Resources.Load(grenadesPrefabPath + grenadeName + "_Grenade") as GameObject;
        }
    }

    public class BulletType
    {
        public static string[] bulletTypes = new string[] {"5.56", "7.26", "9" };
    }

    public class Guns
    {
        public static string[] availableGunNames = 
            new string[] { 
                "TestGun", 
                "TestGun2", 
                "M24" };
    }

    public class Mags
    {
        public static string[] availableMagNames =
            new string[]
            {
                "TestGun",
                "TestGun2",
                "M24"
            };
    }

    public class DisplayMode
    {
        public const string Mag = "Mag";
        public const string Cartridge = "Cartridge";
        public const string Gun = "Gun";
        public const string Sight = "Sight";
        public const string Tactical = "Tactical";
        public const string Muzzle = "Muzzle";
        public const string Primary = "Primary";
        public const string Secondary = "Secondary";
        public const string Handle = "Handle";
        public const string None = "None";
    }

    public class ComponentLoader
    {
        public static Sight getSightScript(GameObject sight)
        {
            return sight.GetComponent<Sight>();
        }

        public static Mag getMagScript(GameObject mag)
        {
            return mag.GetComponent<Mag>();
        }

        public static Tactical getTacticalScript(GameObject tactical)
        {
            return tactical.GetComponent<Tactical>();
        }

        public static Gun getGunScript(GameObject gun)
        {
            return gun.GetComponent<Gun>();
        }

        public static Muzzle getMuzzleScript(GameObject muzzle)
        {
            return muzzle.GetComponent<Muzzle>();
        }

        public static Handle getHandleScript(GameObject handle)
        {
            return handle.GetComponent<Handle>();
        }

    }

    public class ErrorRange 
    {
        public const float aimPosition = 0.00005f;
        public const float changeArm = 0.005f;
        public const float sliderAnimation = 0.005f;
        public const float changeGun = 0.05f;
        public const float insertBullet = 0.005f;
        public const float magReload = 0.005f;
        public const float recoilGunHolder = 0.0001f;
    }

    public class WinType
    {
        public const int score = 0;
        public const int scoreByTime = 1;
        public const int kill = 2;
        public const int killByTime = 3;
    }

    public class GunSetting
    {
        public string gunName;
        public string gunSightName;
        public string gunTacticalName;
        public string gunMuzzleName;
        public string gunHandleName;
    }
}