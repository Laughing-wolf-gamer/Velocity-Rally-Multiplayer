using Unity.Cinemachine;
using UnityEngine;
using System.Collections;
public class CameraSwitcher : MonoBehaviour {
    [SerializeField] private CinemachineVirtualCamera virtualCamera1,virtualCamera2;

    private bool isFirstCamera;
    private void Start(){
        StartCoroutine(SwitchCamera());
    }
    private IEnumerator SwitchCamera(){
        float randomTime = Random.Range(5f,10f);
        yield return new WaitForSeconds(randomTime);
        isFirstCamera = !isFirstCamera;
        virtualCamera1.gameObject.SetActive(isFirstCamera);
        virtualCamera2.gameObject.SetActive(!isFirstCamera);
        StartCoroutine(SwitchCamera());
    }
}