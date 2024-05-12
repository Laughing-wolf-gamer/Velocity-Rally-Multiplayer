using UnityEngine;
[ExecuteInEditMode]
public class LookAtTarget : MonoBehaviour {
    [SerializeField] private Transform camTransform;
    private enum LookingType{
        TowardCamera,
        BackingCamera,
        DirectLookAt,
        ReverseLookAt,
    }
    [SerializeField] private LookingType lookingType;
    private void Update(){
        switch(lookingType){
            case LookingType.TowardCamera:
                transform.forward = -camTransform.forward;
            break;
            case LookingType.BackingCamera:
                transform.forward = camTransform.forward;
            break;
            case LookingType.DirectLookAt:
                transform.LookAt(camTransform);
            break;
            case LookingType.ReverseLookAt:
                transform.LookAt(new Vector3(camTransform.position.x,camTransform.position.y,-camTransform.position.z));
            break;
        }
    }
}