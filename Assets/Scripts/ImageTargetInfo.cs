using UnityEngine;
using Vuforia;

public class ImageTargetInfo : MonoBehaviour, ITrackableEventHandler {
	private TrackableBehaviour mTrackableBehaviour;

	void Start() {
		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		if (mTrackableBehaviour) {
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}
	}

	public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus) {
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED) {
			OnTrackingFound();
		}
	}

	private void OnTrackingFound() {
		if (mTrackableBehaviour is ImageTargetAbstractBehaviour) {
			ImageTargetAbstractBehaviour itb = mTrackableBehaviour as ImageTargetAbstractBehaviour;
			ImageTarget it = itb.ImageTarget;
			SetActivityInNativePlugin();
			ShowTargetInfo(it.Name, it.GetSize().x, it.GetSize().y);
		}
	}

	#if UNITY_ANDROID
	private AndroidJavaObject javaObj = null;

	private AndroidJavaObject GetJavaObject() {
		if (javaObj == null) {
			javaObj = new AndroidJavaObject("edu.temple.gamemanager.ImageTargetDetailProvider");
		}
		return javaObj;
	}

	private void SetActivityInNativePlugin() {
		// Retrieve current Android Activity from the Unity Player
		AndroidJavaClass jclass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = jclass.GetStatic<AndroidJavaObject>("currentActivity");

		// Pass reference to the current Activity into the native plugin,
		// using the 'setActivity' method that we defined in the ImageTargetLogger Java class
		GetJavaObject().Call("setActivity", activity);
	}

	private void ShowTargetInfo(string targetName, float targetWidth, float targetHeight) {
		GetJavaObject().Call("showTargetInfo", targetName, targetWidth, targetHeight);
	}
	#else
	private void SetActivityInNativePlugin() {
		Debug.Log("SetActivityInNativePlugin method placeholder for Play Mode (not running on Android device)");
	}
	private void ShowTargetInfo(string targetName, float targetWidth, float targetHeight) {
		Debug.Log("ShowTargetInfo method placeholder for Play Mode (not running on Android device)");
	}
	#endif
}