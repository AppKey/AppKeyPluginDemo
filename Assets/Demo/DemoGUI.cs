using UnityEngine;
using System.Collections;

public class DemoGUI : MonoBehaviour {

	void Update () {
		//Enable android back arrow
		if(Input.GetKey(KeyCode.Escape)) {
			Application.Quit();	
		}
	}

#if UNITY_ANDROID

	private string AppKeyResult="";
	
	void OnEnable()
	{
		// Add AppKey event handlers when this script is enabled
		AppKeyManager.appKeyEnabledEvent	+= AppKeyCallback_Enable;
		AppKeyManager.appKeyDisabledEvent	+= AppKeyCallback_Disable;
	}
	
	void OnDisable() {
		// Remove event handlers when the script is disabled
		AppKeyManager.appKeyEnabledEvent	-= AppKeyCallback_Enable;
		AppKeyManager.appKeyDisabledEvent	-= AppKeyCallback_Disable;
	}
	//void Start() {
	//	AppKeyManager.appKeyEnabledEvent += AppKeyCallback_Enabled;	
	//	AppKeyManager.appKeyDisabledEvent += AppKeyCallback_Disabled;	
	//}

	void OnGUI () {

		GUI.skin.label.fontSize = 20;
		GUI.skin.button.fontSize = 20;

		GUI.Label(new Rect(50, 50, 600, 200), "AppKey status updated on app start and/or resume");
		GUI.Label(new Rect(50, 100, 600, 200), "Status = "+AppKeyResult);
		GUI.Label(new Rect(300, 200, 400, 100), "PromptUser tells the user how to enable AppKey");
		if (GUI.Button(new Rect(50,200,200,100),"Call PromptUser")) {
			AppKeyManager.PromptUser("[some awesome thing]");
		}

		//OpenAppKey opens appkey to the store view. Typically not used but available if it works for your UX
		//GUI.Label(new Rect(300, 400, 340, 100), "OpenAppKey opens AppKey to the store view");
		//if (GUI.Button(new Rect(50,400,200,100),"Call OpenAppKey")) {
		//	AppKeyManager.OpenAppKey();
		//}
	}

	public void AppKeyCallback_Enable() {
		// This called when AppKey is installed, running, and active on the user's phone.

		// <Your code to ENABLE features for AppKey users goes here.>
		string message = "AppKey features unlocked!";
		Debug.Log(message);
		AppKeyResult=message;
	}
	
	public void AppKeyCallback_Disable(AppKeyManager.LockedReasons reason) {
		// This called when AppKey is not enabled. reason is passed but only needed if you
		// want to override the default AppKey messaging. Most apps ignore the reason and
		// use the default user messaging by calling AppKeyManager.PromptUser()
		
		// <Your code to DISABLE features for AppKey users goes here.>
		string message = "AppKey features locked. Reason = "+reason;
		Debug.Log(message);
		AppKeyResult=message;
	}
#else
	void OnGUI () {
		
		GUI.skin.label.fontSize = 20;
		GUI.skin.button.fontSize = 20;
		GUI.Label(new Rect(50, 50, 600, 200), "AppKey is designed for Android");
		GUI.Label(new Rect(50, 100, 600, 200), "Switch platform to Android to run this demo");
	}
#endif
}
