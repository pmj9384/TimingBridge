using System;
using GoogleMobileAds.Api;
using UnityCommunity.UnitySingleton;
using UnityEngine;

public class AdsManager : PersistentMonoSingleton<AdsManager>
{
    private const string RewardedAdUnitId = "ca-app-pub-4717936789267161/3672102644";

    private RewardedAd rewardedAd;
    private bool isInitialized = false;

    public override void InitializeSingleton()
    {
        base.InitializeSingleton();
        MobileAds.Initialize(status =>
        {
            isInitialized = true;
            Debug.Log("[AdsManager] AdMob 초기화 완료");
            LoadRewardedAd();
        });
    }

    public void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        RewardedAd.Load(RewardedAdUnitId, new AdRequest(), (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogWarning($"[AdsManager] 리워드 광고 로드 실패: {error}");
                return;
            }
            rewardedAd = ad;
            Debug.Log("[AdsManager] 리워드 광고 로드 완료");
        });
    }

    public bool IsRewardedAdReady => rewardedAd != null;

    public void ShowRewardedAd(Action onRewarded, Action onFailed = null)
    {
        if (!IsRewardedAdReady)
        {
            Debug.LogWarning("[AdsManager] 광고가 준비되지 않았습니다.");
            onFailed?.Invoke();
            return;
        }

        rewardedAd.Show(reward =>
        {
            Debug.Log("[AdsManager] 리워드 지급 완료");
            onRewarded?.Invoke();
        });

        // 광고 닫힌 후 다음 광고 미리 로드
        rewardedAd.OnAdFullScreenContentClosed += () => LoadRewardedAd();
        rewardedAd.OnAdFullScreenContentFailed += error =>
        {
            Debug.LogWarning($"[AdsManager] 광고 표시 실패: {error}");
            onFailed?.Invoke();
            LoadRewardedAd();
        };
    }
}
