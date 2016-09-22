using UnityEngine;
using System.Collections;

public class RestrictedAreaMonitor : MonoBehaviour {
	/// <summary>
	/// Start this instance.  If environment is Android, instantiate the location tracker, and send it
	/// an instance of the LocationUpdateListener, attached to the primary sphere object.
	/// </summary>
	void Start () {
		#if UNITY_ANDROID
		AndroidJavaObject javaObj = new AndroidJavaObject("edu.temple.gamemanager.LocationTracker");
		MaterialSwapper listener = new MaterialSwapper(GameObject.Find("Sphere"));
		javaObj.Call("SetLocationUpdateListener", listener);
		#else
		Debug.Log("LocationUpdateListener placeholder for Play Mode (not running on Android device)");
		#endif
	}

	/// <summary>
	/// Update this instance.  Currently unused, since all of the event handling is done with the
	/// LocationUpdateListener class below.
	/// </summary>
	void Update () { }

	/// <summary>
	/// An internal class implementing the basics of the LocationUpdateListener interface.  Responsible
	/// for storing an instance of a target game object, and toggling its first and second render materials
	/// when entering or leaving a restricted area, as indicated by the listener events.
	/// 
	/// NOTE:  THE UNITY ENVIRONMENT IS NOT RESPONSIBLE FOR DETERMINING WHEN THE USER HAS ENTERED OR
	/// LEFT A RESTRICTED AREA.  THIS LOGIC IS CONTAINED ENTIRELY WITHIN THE GAME MANAGER ANDROID LIBRARY.
	/// </summary>
	class MaterialSwapper : AndroidJavaProxy {
		private Renderer targetRenderer;
		private AndroidJavaObject toastDebugger;

		/// <summary>
		/// Initializes the listener class.  Accepts a game object target for which to update the
		/// rendered material.
		/// </summary>
		/// <param name="target">Target for which to update the rendered material</param>
		public MaterialSwapper(GameObject target) 
			: base("edu.temple.gamemanager.LocationUpdateListener") {
			targetRenderer = target.GetComponent<Renderer>();
			toastDebugger = new AndroidJavaObject("edu.temple.gamemanager.ToastDebugger");
			SetActivityInNativePlugin();
		}

		/// <summary>
		/// Interface-required method to respond to the user entering a restricted area.  In our
		/// case, the material for the target will switch to the second in the list (ex: red color)
		/// </summary>
		public void onRestrictedAreaEntered() {
			ShowMessage("Entered a restricted area!");
			targetRenderer.material = targetRenderer.materials [1];
		}

		/// <summary>
		/// Interface-required method to respond to the user leaving a restricted area.  In our 
		/// case, the material for the target will switch to the first in the list (ex: blue color)
		/// </summary>
		public void onRestrictedAreaLeft() {
			ShowMessage("Left a restricted area!");
			targetRenderer.material = targetRenderer.materials [0];
		}

		/// <summary>
		/// Private internal method to initialize the activity property for our debugger toast maker
		/// </summary>
		private void SetActivityInNativePlugin() {
			AndroidJavaClass jclass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = jclass.GetStatic<AndroidJavaObject>("currentActivity");
			toastDebugger.Call("setActivity", activity);
		}

		/// <summary>
		/// Private internal method to display a debugger message to the user via Android toast
		/// </summary>
		/// <param name="message">Message to display on the toast</param>
		private void ShowMessage(string message) {
			toastDebugger.Call("showMessage", message);
		}
	}
}