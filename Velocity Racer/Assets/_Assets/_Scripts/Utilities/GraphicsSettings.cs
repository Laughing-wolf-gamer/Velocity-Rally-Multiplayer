using UnityEngine;
using Random = UnityEngine.Random;

public class GraphicsSettings : MonoBehaviour {
    [SerializeField] private bool autoGraphics,debugerActive = true;
    [SerializeField] private int maxQuality = 4;
    [SerializeField,Tooltip("Do Not Edit")] private int graphicsMemorySize;
    private int currenQuality;
    private int[] randomMinimum;
    private void SetAutoGraphics(){
        randomMinimum = new int[]{3,2};
        graphicsMemorySize = SystemInfo.graphicsMemorySize;
        currenQuality = maxQuality;
        if(graphicsMemorySize >= (7 * 1000)){
            currenQuality = maxQuality;
        }else{
            if(graphicsMemorySize < (7 * 1000) && graphicsMemorySize >= (6 * 1000)){
                currenQuality -= 1;
            }else if(graphicsMemorySize < (6 * 1000) && graphicsMemorySize >= (1.8 * 1000)){
                int reductionAmount = randomMinimum[Random.Range(0,randomMinimum.Length)];
                currenQuality -= reductionAmount;
            }
        }
        QualitySettings.SetQualityLevel(currenQuality);
        Debug.Log("Device Graphics Memory Size " + SystemInfo.graphicsMemorySize);
        switch(currenQuality){
            case 0:
                Debug.LogWarning("Current Quality is Lowest ,Graphics Memory = " + graphicsMemorySize);
            break;
            case 1:
                Debug.LogWarning("Current Quality is Performant ,Graphics Memory = " + graphicsMemorySize);
            break;
            case 2:
                Debug.LogWarning("Current Quality is Balanced ,Graphics Memory = " + graphicsMemorySize);
            break;
            case 3:
                Debug.LogWarning("Current Quality is Highest ,Graphics Memory = " + graphicsMemorySize);
            break;
        }
    }
    private void OnEnable(){
        Debug.unityLogger.logEnabled = debugerActive;
        if(autoGraphics){
            SetAutoGraphics();
        }
    }
}