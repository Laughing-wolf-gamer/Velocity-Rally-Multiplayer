using UnityEngine;
public class CheckPointSingle : MonoBehaviour {
    private TrackCheckPoint trackCheckPoint;
    public void SetTrackCheckPoint(TrackCheckPoint trackCheckPoint){
        this.trackCheckPoint = trackCheckPoint;
    }
    private void OnTriggerEnter(Collider coli){
        if(coli.TryGetComponent(out CarDriver carDriver)){
            trackCheckPoint.PlayerThrowCheckPoint(this,carDriver.transform);
        }
        if(coli.TryGetComponent(out CarDriverAI driverAI)){
            trackCheckPoint.PlayerThrowCheckPoint(this,driverAI.transform);
        }
    }
}