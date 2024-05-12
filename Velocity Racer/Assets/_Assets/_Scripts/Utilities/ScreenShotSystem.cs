using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class ScreenShotSystem : MonoBehaviour {
    [SerializeField] private UnityEvent OnBeforShot;
    [SerializeField] private UnityEvent OnAfterShot;
    [SerializeField] private int shotNumber;
    
    private bool takeScreenShotOnNextFrame;
    private Camera cam;
    private void Start(){
        if(PlayerPrefs.HasKey("ShotNum")){
            shotNumber = PlayerPrefs.GetInt("ShotNum");
        }
        if(cam == null){
            cam = Camera.main;
        }
    }
    private void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            OnBeforShot?.Invoke();
            ScreenCapture.CaptureScreenshot(string.Concat(Application.dataPath,"/ScreenShots/","S",shotNumber,".png"));
            shotNumber++;
            PlayerPrefs.SetInt("ShotNum",shotNumber);
            Invoke(nameof(InvokeAfterShot),0.1f);
        }
    }
    private void InvokeAfterShot(){
        OnAfterShot?.Invoke();
    }
    private void OnPostRender(){
        if(takeScreenShotOnNextFrame){
            takeScreenShotOnNextFrame = false;
            RenderTexture renderTexture = cam.targetTexture;
            Texture2D renderResult = new Texture2D(renderTexture.width,renderTexture.height,TextureFormat.ARGB32,false);
            Rect rect = new Rect(0,0,renderTexture.width,renderResult.height);
            renderResult.ReadPixels(rect,0,0);

            byte[] byteArray = renderResult.EncodeToPNG();
            System.IO.File.WriteAllBytes(string.Concat(Application.dataPath + "/Screen Shots/S",shotNumber,".png"),byteArray);
            Debug.Log("Saved ScreenShot"+shotNumber+".png");
            RenderTexture.ReleaseTemporary(renderTexture);
            cam.targetTexture = null;
            shotNumber++;
        }
    }
    private void TakeScreenShot(int width,int height){
        cam.targetTexture = RenderTexture.GetTemporary(width,height,16);
        takeScreenShotOnNextFrame = true;
    }

    
}
