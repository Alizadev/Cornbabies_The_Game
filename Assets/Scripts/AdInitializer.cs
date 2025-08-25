using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField]
    string _androidGameId;
    [SerializeField]
    string _iosGameId;
    [SerializeField]
    bool _testMode = false;

    private void Start()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
        Advertisement.Initialize(Application.platform == RuntimePlatform.IPhonePlayer ? _iosGameId : _androidGameId, _testMode, this);
    }


    public void OnInitializationComplete()
    {
        Debug.Log("[Ads] Unity Ads initialization complete => " + (Application.platform == RuntimePlatform.IPhonePlayer ? _iosGameId : _androidGameId));
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"[Ads] Unity Ads Initialization Failed: {error} - {message}");
    }
}