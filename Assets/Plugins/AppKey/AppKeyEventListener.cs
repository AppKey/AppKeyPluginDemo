using UnityEngine;
using System.Collections;

// Example implementation of event-based AppKey integration
public class AppKeyEventListener : MonoBehaviour {
	
#if UNITY_ANDROID
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

	public void AppKeyCallback_Enable() {
		// This called when AppKey is installed, running, and active on the user's phone.
		
		// <Your code to ENABLE features for AppKey users goes here.>
		
		Debug.Log("AppKey features unlocked!");
	}
	
	public void AppKeyCallback_Disable(AppKeyManager.LockedReasons reason) {
		// This called when AppKey is not enabled. reason is passed but only needed if you
		// want to override the default AppKey messaging. Most apps ignore the reason and
		// use the default user messaging by calling AppKeyManager.PromptUser()
		
		// <Your code to DISABLE features for AppKey users goes here.>
		
		Debug.Log("AppKey features locked. Reason = "+reason); 
	}
#endif
}
