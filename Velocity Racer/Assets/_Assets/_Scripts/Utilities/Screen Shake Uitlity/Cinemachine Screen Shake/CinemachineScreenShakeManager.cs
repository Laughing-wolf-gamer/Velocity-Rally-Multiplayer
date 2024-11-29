using UnityEngine;
using Unity.Cinemachine;

namespace GamerWolf.Utils {
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CinemachineScreenShakeManager : MonoBehaviour {

        [SerializeField] private CinemachineScreenShakePropertiensSO propertiensSO;
        private CinemachineVirtualCamera virtualCamera;
        private float startingIntensity,shakeTimeTotal;
        private float shakeTimer;

        #region Singleton......
        private void Awake(){
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }
        #endregion
        public void Shake(float intensity,float time){
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.AmplitudeGain = intensity;
            shakeTimer = time;
            shakeTimeTotal = time;
            startingIntensity = intensity;

        }
        public void Shake(CinemachineScreenShakePropertiensSO _propertiensSO){
            Shake(_propertiensSO.intensity,_propertiensSO.time);
        }
        public void Shake(){
            Shake(propertiensSO.intensity,propertiensSO.time);
        }

        private void Update(){
            if(shakeTimer > 0){
                shakeTimer -= Time.deltaTime;
                if(shakeTimer <= 0){
                    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                    cinemachineBasicMultiChannelPerlin.AmplitudeGain = Mathf.Lerp(startingIntensity,0f,(1 - (shakeTimer / shakeTimeTotal)));
                }
            }
        }
    }

}