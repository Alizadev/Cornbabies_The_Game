using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsLoader : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdsLoader Instance { get; private set; }

    public GameObject AdsHeadsUp;
    string _adUnitId = null;
   
    private void Awake()
    {
        Instance = this;
        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = "Interstitial_iOS";
#elif UNITY_ANDROID
        _adUnitId = "Interstitial_Android";
#endif

        //check ads
        if (PlayerPrefs.HasKey(CornUtility.ADS_PREFS))
        {
            PlayerPrefs.DeleteKey(CornUtility.ADS_PREFS);
            StartCoroutine(ShowAD());
            Debug.Log("[AdsLoader] played long enough to play ads..");
        }
    }

    IEnumerator ShowAD()
    {

        if (Advertisement.isInitialized == false)
        {
            Debug.Log("[AdsLoader] Ads not initialized.");
            yield break;
        }
        AdsHeadsUp.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("[AdsLoader] Ad Loaded: " + adUnitId);
        AdsHeadsUp.SetActive(false);
        if (adUnitId.Equals(_adUnitId))
        {
            Advertisement.Show(_adUnitId, this);
        }
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("[AdsLoader] Unity Ads Rewarded Ad Completed");
            // Grant a reward.
            AdsHeadsUp.SetActive(false);
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
        AdsHeadsUp.SetActive(false);
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
        AdsHeadsUp.SetActive(false);

    }


    public void OnUnityAdsShowStart(string adUnitId)
    {
        AdsHeadsUp.SetActive(false);
    }
    public void OnUnityAdsShowClick(string adUnitId)
    {
        AdsHeadsUp.SetActive(false);
    }

}
