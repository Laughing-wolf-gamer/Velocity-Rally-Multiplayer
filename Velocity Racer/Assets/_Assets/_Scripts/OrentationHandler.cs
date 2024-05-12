using UnityEngine;
public class OrentationHandler : MonoBehaviour {
    // Orientation the screen is locked at
    public ScreenOrientation screenRot = ScreenOrientation.LandscapeLeft;

    [System.NonSerialized]
    public float steer;

    // Set screen orientation
    private void Awake() {
        Screen.autorotateToPortrait = screenRot == ScreenOrientation.Portrait || screenRot == ScreenOrientation.AutoRotation;
        Screen.autorotateToPortraitUpsideDown = screenRot == ScreenOrientation.PortraitUpsideDown || screenRot == ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeRight = screenRot == ScreenOrientation.LandscapeRight || screenRot == ScreenOrientation.LandscapeRight || screenRot == ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = screenRot == ScreenOrientation.LandscapeLeft || screenRot == ScreenOrientation.LandscapeLeft || screenRot == ScreenOrientation.AutoRotation;
        Screen.orientation = screenRot;
    }
    public void SetSteer(float f) {
        steer = Mathf.Clamp(f, -1, 1);
    }

}