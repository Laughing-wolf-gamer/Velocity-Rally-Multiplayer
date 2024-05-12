using UnityEngine;
using Random = UnityEngine.Random;

public class TrackCheckVisual : MonoBehaviour {
    [SerializeField] private bool showLinesGizmos = true,showArrow = true;
    [SerializeField] private float arrowHeadLength = .25f;
    [SerializeField,Range(2f,10f)] private float CheckPointSphereRadius = 4f;
    [SerializeField,Range(10f,90f)] private float arrowHeadAngle = 30f;
    [SerializeField,Range(0,1f)] private float maxCheckPointAlpha = .2f;
    [SerializeField] private Color connectionLineColor,arrowColor,sideLengthColor,checkPointViewColor = Color.white,CheckPointSphereColor = Color.cyan;
    [SerializeField] private Transform currentTrackCheckPointHolder;

    public void GetRandomColorForVisual(){
        // call from Editor Script.
        CheckPointSphereColor = Random.ColorHSV();
        connectionLineColor = Random.ColorHSV();
        arrowColor = Random.ColorHSV();
        sideLengthColor = Random.ColorHSV();
        Color newColor = Random.ColorHSV();
        newColor.a = maxCheckPointAlpha;
        checkPointViewColor = newColor;
    }
    private void OnDrawGizmos(){
        if(!showLinesGizmos) return;
        
        if(GetTotalCheckPointCount() > 0){
            for (int i = 0; i < currentTrackCheckPointHolder.childCount; i++) {
                if(i + 1 < currentTrackCheckPointHolder.childCount){
                    //Check Point Connecting Lines..................
                    Gizmos.color = connectionLineColor;
                    Vector3 p1 = currentTrackCheckPointHolder.GetChild(i).GetChild(0).position;
                    Vector3 p2 = currentTrackCheckPointHolder.GetChild(i+1).GetChild(0).position;
                    Gizmos.DrawLine(p1,p2);
                    if(showArrow){
                        // Arrow.
                        Gizmos.color = arrowColor;
                        Vector3 dir = (p1 - p2).normalized;
                        Vector3 lineCenter = (p1 + p2) / 2;
                        Vector3 right = Quaternion.LookRotation(dir) * 
                            Quaternion.Euler(0,180 + arrowHeadAngle,Time.realtimeSinceStartup * 360f) * new Vector3(0,0,1);
                        Vector3 left = Quaternion.LookRotation(dir) * 
                            Quaternion.Euler(0,180 - arrowHeadAngle,Time.realtimeSinceStartup * -360f) * new Vector3(0,0,1);
                                                
                        Gizmos.DrawRay(lineCenter + dir, right * arrowHeadLength);
                        Gizmos.DrawRay(lineCenter + dir, left *  arrowHeadLength);
                        Gizmos.color = CheckPointSphereColor;
                        Gizmos.DrawSphere(p1,CheckPointSphereRadius);
                        Gizmos.DrawSphere(p2,CheckPointSphereRadius);
                    }

                }
                Gizmos.color = sideLengthColor;
               /*  Gizmos.DrawRay(currentTrackCheckPointHolder.GetChild(i).transform.position,currentTrackCheckPointHolder.GetChild(i).transform.forward * maxCheckPointAlpha);
                Gizmos.DrawRay(currentTrackCheckPointHolder.GetChild(i).transform.position,currentTrackCheckPointHolder.GetChild(i).transform.forward * -maxCheckPointAlpha); */
            }
            checkPointViewColor.a = maxCheckPointAlpha;
            Gizmos.color = checkPointViewColor;
            for(int d = 0; d < currentTrackCheckPointHolder.childCount; d++){
                Transform currentCheckPoint = currentTrackCheckPointHolder.GetChild(d).GetChild(0);
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(currentCheckPoint.position, currentCheckPoint.rotation, currentCheckPoint.lossyScale);
                Gizmos.matrix = rotationMatrix;  
                Gizmos.DrawCube (Vector3.zero,Vector3.one);
            }
        }
    }
    public int GetTotalCheckPointCount(){// Gives total Number of CheckPoint.
        return currentTrackCheckPointHolder.childCount;
    }
}