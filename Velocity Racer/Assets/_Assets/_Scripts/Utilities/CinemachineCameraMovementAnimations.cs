using UnityEngine;
using Unity.Cinemachine;
public class CinemachineCameraMovementAnimations : MonoBehaviour {
    [SerializeField] private CinemachineDollyCart cinemachineDollyCart;
    [SerializeField] private CinemachineSmoothPath path;
    [SerializeField] private CinemachineVirtualCamera mainLookCamera;
    [SerializeField] private Transform lookAtTarget;
    [SerializeField] private GameObject[] cameraObjects;
    private bool isMoving;
    [ContextMenu("Set Up")]
    public void SetUp(){
        foreach(GameObject gO in cameraObjects){
            if(gO.TryGetComponent(out CinemachineSmoothPath cinemachineSmoothPath)){
                path = cinemachineSmoothPath;
            }
            if(gO.TryGetComponent(out CinemachineVirtualCamera cVc)){
                mainLookCamera = cVc;
            }
        }
    }
    private void Awake(){
        isMoving = false;
        StopCameraMovement();
    }
    /* private void Update(){
        if(!isMoving) return;
        float distance = Vector3.Distance(path.EvaluatePosition(cinemachineDollyCart.m_Position),lookAtTarget.position);
        if(distance >= .1f){
            lookAtTarget.position = path.EvaluatePosition(cinemachineDollyCart.m_Position);
        }
    } */
    public void StartCameraMovement(){
        isMoving = true;
        foreach(GameObject camObjects in cameraObjects){
            camObjects.SetActive(true);
        }
        cinemachineDollyCart.gameObject.SetActive(true);
    }
    public void StopCameraMovement(){
        isMoving = false;
        foreach(GameObject camObjects in cameraObjects){
            camObjects.SetActive(false);
        }
        cinemachineDollyCart.gameObject.SetActive(false);
    }
}