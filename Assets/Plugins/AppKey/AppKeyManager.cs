using UnityEngine;
using System;
using System.Collections;

// AppKeyManager
// 
// AppKeyManager manages communication with the AppKeyPlugin. 
// 
// This code leverages a preprocessor directives to eliminate any
// load on non-android builds.
// 
// In most cases, all you need need to know is:
// 1) AppKeyManager.AppKeyEnabled is a bool indicating whether or not AppKey items should be available to the user
// 2) AppKeyManager.PromptUser(benefit) handles all the user interactions for the user to install AppKey
//
public class AppKeyManager : MonoBehaviour {
	//AppKeyUnlocked is automatically updated whenever:
	//1. User launches the app
	//2. User switches back to this app
	//Checking AppKeyUnlocked is the easiest way to determine whether or not AppKey is
	//unlocked. You can also hook into the events if that works better for your app.
	public static bool isAppKeyEnabled = false;

	//IsAppKeySupported 
	public static bool isAppKeySupported() {
		#if UNITY_ANDROID
			return true;
		#else
			return false;
		#endif
	}

#if UNITY_ANDROID

	public static event Action appKeyEnabledEvent;
	public static event Action<LockedReasons> appKeyDisabledEvent;

	public string AppID;
	public bool UserAnalytics;  // Whether or not to send user behavior analytics to the AppKey platform to optimize your revenue
	private bool LOGD=false;	// Set to true to activate debug logs.
	public static LockedReasons AppKeyLockedReason = LockedReasons.UNDEFINED;
	public enum LockedReasons {
		NOT_INSTALLED=0,
		NOT_RUNNING=1,
		INACTIVE=2,
		UNDEFINED=3
	}

	private static AndroidJavaClass mAppKeyPluginClass;
	private static AndroidJavaObject mAppKeyPlugin;
	private static AndroidJavaObject mActivity;
	
	private static AppKeyManager instance;

	void Start () {
		instance=this;

		if (LOGD) Debug.Log("AppKeyManager.Start Called");

		if (gameObject.name!="AppKeyManager") Debug.LogError("AppKeyManager: Game object name must be AppKeyManager");
		mAppKeyPluginClass = new AndroidJavaClass("com.appkey.plugin.AppKeyPlugin");
		mAppKeyPlugin = mAppKeyPluginClass.CallStatic<AndroidJavaObject>("INSTANCE");
		mAppKeyPlugin.Call("setDebugLogging", LOGD);
		mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		mAppKeyPlugin.Call("init", mActivity, instance.AppID, instance.UserAnalytics);
		StartCoroutine (CheckAppKeyDelayed());
	}

	//OnApplicationPause(false) aligns with Activity.onResume() in native Android development. It is
	//called every time the activity starts or resumes from the background.
	void OnApplicationPause(bool paused) {
		if (paused == false) {
			StartCoroutine (CheckAppKeyDelayed());
		}
	}

	//Let Unity have priority on resumes
	IEnumerator CheckAppKeyDelayed(){
		yield return new WaitForSeconds(.5f);
		CheckAppKey();
	}
	
	public static void CheckAppKey() {
		if(instance == null) {
			Debug.LogWarning("AppKeyManager: No AppKeyManager instance detected, please make sure to add the AppKeyManager prefab to your scene.");
			return;
		}
		instance._CheckAppKey();
	}
	
	public static void PromptUser(string benefit) {
		if(instance.LOGD) Debug.Log("AppKeyManager.PromptUser benefit= " + benefit);
		instance._PromptUser(benefit);
	}

	protected void _CheckAppKey() {
		if (LOGD) Debug.Log("Calling: mAppKeyPlugin.Call('checkAccess'), where instance.AppID="+instance.AppID+", instance.AnalyticsEnabled="+instance.UserAnalytics);
		mAppKeyPlugin.Call("checkAccess");
	}		
	
	protected void _PromptUser(string benefit) {
		if (LOGD) Debug.Log("Calling: mAppKeyPlugin.Call('promptUser',benefit), where instance.AppID="+instance.AppID+", instance.AnalyticsEnabled="+instance.UserAnalytics+", benefit="+benefit);

		AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
		mAppKeyPlugin.Call("promptUser", activity, benefit);
	}
	
	public static void OpenAppKey() {
		if (instance.LOGD) Debug.Log("AppKeyManager: Opening AppKey");
		mAppKeyPlugin.Call("openAppKey");
	}

	public void allow(string empty) {
		if (LOGD) Debug.Log("AppKeyManager: allow() Called");
		
		isAppKeyEnabled = true;

		if (appKeyEnabledEvent!=null) {
			appKeyEnabledEvent();
		}
	}
	

	public void dontAllow(string strReason) {
		if (LOGD) Debug.Log("AppKeyManager: dontAllow() Called.  Reason=" + strReason);
		LockedReasons reason;
		switch (strReason) {
			case ("NOT_INSTALLED"):
					reason=LockedReasons.NOT_INSTALLED;
					break;
			case ("NOT_RUNNING"):
					reason=LockedReasons.NOT_RUNNING;
					break;
			case ("INACTIVE"):
					reason=LockedReasons.INACTIVE;
					break;
			default:
				reason=LockedReasons.UNDEFINED;
				break;
		}
		
		isAppKeyEnabled = false;
		AppKeyLockedReason = reason;
		
		if (appKeyDisabledEvent!=null) {
		    appKeyDisabledEvent(reason);
		}
	}
#endif
}
