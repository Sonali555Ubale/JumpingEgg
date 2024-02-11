using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdController : MonoBehaviour, IUnityAdsListener
{

    public static AdController instance;


    string gameId = "3654674";
    string videoAdId = "video";
    string rewardedVideoAdId = "rewardedVideo";
    string bannerAdId = "banner";
    string interstitialAdId = "interstitial";
    bool testMode = true;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }

    void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
    }

    public void showVideoAd()
    {
        if (Advertisement.IsReady(videoAdId))
        {
            Advertisement.Show(videoAdId);
        }
    }

    public void showRewardedVideoAd()
    {
        if (Advertisement.IsReady(rewardedVideoAdId))
        {
            Advertisement.Show(rewardedVideoAdId);
        }
    }

    public void showBannerAd()
    {/*
        if (Advertisement.IsReady(bannerAdId))
        {
            Advertisement.Show(bannerAdId);
        }*/

        StartCoroutine(ShowBannerWhenInitialized());
    }

    IEnumerator ShowBannerWhenInitialized()
    {
        while (!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(0.2f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        Advertisement.Banner.Show(bannerAdId);
    }

    public void hideBannerAd()
    {
        Advertisement.Banner.Hide();
    }

    public void showInterstitialAd()
    {
        if (Advertisement.IsReady(interstitialAdId))
        {
            Advertisement.Show(interstitialAdId);
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidError(string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {// Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
            if (placementId == rewardedVideoAdId)
                GameManager.instance.rewardCoins();
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }

    }
}
