using UnityCommunity.UnitySingleton;
using UnityEngine;

public class GameDataManager : PersistentMonoSingleton<GameDataManager>
{
    public PlayerAccountData PlayerAccountData { get; private set; }

    // [AnimalBreakOut] 게임 전용 시스템
    //public GoldAnimalTokenKeySystem GoldAnimalTokenKeySystem { get; private set; }
    //public PlayerLevelSystem PlayerLevelSystem { get; private set; }
    //public StaminaSystem StaminaSystem { get; private set; }
    //public AnimalUserDataList AnimalUserDataList { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        // [AnimalBreakOut] 네이티브 서비스, 설정 이벤트
        //NativeServiceManager.Instance.InitializeSingleton();
        //QualitySettings.vSyncCount = 0;
        //SettingPanel.onFrameRateIndexChanged += OnFrameRateIndexChangedHandler;
        //SettingPanel.onLanguageSettingTypeChanged += OnLanguageSettingTypeChangedHandler;
    }

    public override void InitializeSingleton()
    {
        base.InitializeSingleton();
        SaveLoadSystem.Instance.Load();

        // [AnimalBreakOut] 로컬라이제이션
        //LocalizationUtility.PreloadLocalizedTables();

        PlayerAccountData = new();
        PlayerAccountData.Load(SaveLoadSystem.Instance.CurrentSaveData.playerAccountDataSave);

        // [AnimalBreakOut] 게임 전용 시스템 초기화
        //GoldAnimalTokenKeySystem = new(); GoldAnimalTokenKeySystem.Load(...);
        //PlayerLevelSystem = new(); PlayerLevelSystem.Load(...);
        //StaminaSystem = new(); StaminaSystem.Load(...);
        //AnimalUserDataList = new(); AnimalUserDataList.Load(...);
    }

    // [AnimalBreakOut] 프레임레이트, 언어 설정
    //public int frameRateIndex { get; private set; }
    //public LanguageSettingType languageSettingType { get; private set; }
    //public void SetFrameRateIndex(int index) { ... }
    //public void SetLanguageSettingType(LanguageSettingType type) { ... }
}


