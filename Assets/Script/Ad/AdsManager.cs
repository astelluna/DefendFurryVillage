using GoogleMobileAds.Api;
using GoogleMobileAds.Common; // MobileAdsEventExecutor 사용
using UnityEngine;

public class AdsManager : Singleton<AdsManager>
{
    private InterstitialAd interstitialAd;

    private string adUnitId = "ca-app-pub-1407745203471016/9025885840"; //실제 id
    //private string adUnitId = "ca-app-pub-3940256099942544/1033173712"; //테스트용 id
    private float lastShownTime = -999f;
    private const float MinInterval = 600f;
    protected override void OnAwake()
    {
        MobileAds.Initialize(initStatus => LoadInterstitial());
    }

    public void LoadInterstitial()
    {
        // 기존 광고가 남아있다면 콜백 중복 구독 방지를 위해 정리
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        AdRequest request = new AdRequest();
        InterstitialAd.Load(adUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"광고 로드 실패: {error}");
                return;
            }
            interstitialAd = ad;
            Debug.Log("전면 광고 로드 완료");
        });
    }

    public void ShowInterstitial(System.Action onClosed)
    {
        if(IAPManager.Instance.IsAdRemoved ||
                Time.realtimeSinceStartup - lastShownTime < MinInterval)
        {
            onClosed?.Invoke();
            return;
        }

        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            // 핵심 수정: 콜백 내용을 메인 스레드 Update 루프로 넘겨서 실행
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                MobileAdsEventExecutor.ExecuteInUpdate(() =>
                {
                    LoadInterstitial();
                    onClosed?.Invoke();
                });
            };
            lastShownTime = Time.realtimeSinceStartup;
            interstitialAd.Show();
        }
        else
        {
            onClosed?.Invoke();
            LoadInterstitial();
        }
    }


}