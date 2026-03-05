using System;
using UnityEngine;

public class PlayerAccountData : ISaveLoad
{
    public DataSourceType SaveDataSouceType => DataSourceType.Local;

    // [AnimalBreakOut] 튜토리얼
    //public bool IsTutorialComplete { get; private set; }

    // [AnimalBreakOut] 일일 무료 가챠
    //public int GachaSingleAdsCount { get; private set; } = 1;
    //public int GachaSingleAdsRemainCount { get; set; }
    //private DateTime gachaLastUpdate;

    private float bgmVolume;
    public float BgmVolume
    {
        get => bgmVolume;
        set { bgmVolume = Mathf.Clamp(value, 0.0001f, 1f); }
    }

    private float sfxVolume;
    public float SfxVolume
    {
        get => sfxVolume;
        set { sfxVolume = Mathf.Clamp(value, 0.0001f, 1f); }
    }

    // [AnimalBreakOut] 프레임레이트, 언어
    //public int frameRateIndex { get; private set; }
    //public LanguageSettingType languageSettingType { get; private set; }

    public PlayerAccountData()
    {
        SaveLoadSystem.Instance.RegisterOnSaveAction(this);
    }

    // [AnimalBreakOut] 튜토리얼
    //public void OnTutorialComplete() { IsTutorialComplete = true; }

    public void Save()
    {
        var saveData = SaveLoadSystem.Instance.CurrentSaveData.playerAccountDataSave = new();

        // [AnimalBreakOut] 튜토리얼/가챠
        //saveData.tutorialCompleted = IsTutorialComplete;
        //saveData.gachaSingleAdsRemainCount = GachaSingleAdsRemainCount;
        //saveData.gachaLastUpdate = DateTime.Now;

        saveData.bgmVolume = SoundManager.Instance.bgmVolume;
        saveData.sfxVolume = SoundManager.Instance.sfxVolume;

        // [AnimalBreakOut] 프레임레이트, 언어
        //saveData.frameRateIndex = GameDataManager.Instance.frameRateIndex;
        //saveData.languageSettingType = (int)GameDataManager.Instance.languageSettingType;
    }

    public void Load()
    {
        // [AnimalBreakOut] 가챠 초기화
        //GachaSingleAdsRemainCount = GachaSingleAdsCount;
        //IsTutorialComplete = false;

        BgmVolume = 1f;
        SfxVolume = 1f;
    }

    public void Load(PlayerAccountDataSave saveData)
    {
        if (saveData == null)
        {
            Load();
            return;
        }

        // [AnimalBreakOut] 튜토리얼/가챠
        //IsTutorialComplete = saveData.tutorialCompleted;
        //if (DateTime.Today > saveData.gachaLastUpdate) { Load(); }
        //else { GachaSingleAdsRemainCount = saveData.gachaSingleAdsRemainCount; }

        BgmVolume = saveData.bgmVolume;
        SoundManager.Instance.SetBgmVolume(BgmVolume);

        SfxVolume = saveData.sfxVolume;
        SoundManager.Instance.SetSfxVolume(SfxVolume);

        // [AnimalBreakOut] 프레임레이트, 언어
        //frameRateIndex = saveData.frameRateIndex;
        //GameDataManager.Instance.SetFrameRateIndex(frameRateIndex);
        //languageSettingType = (LanguageSettingType)saveData.languageSettingType;
        //GameDataManager.Instance.SetLanguageSettingType(languageSettingType);
    }
}
