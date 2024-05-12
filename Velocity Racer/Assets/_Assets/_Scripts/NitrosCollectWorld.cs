using Photon.Pun;
using UnityEngine;
using GamerWolf.Utils;
[RequireComponent(typeof(AudioSource))]
public class NitrosCollectWorld : MonoBehaviourPun,IPunObservable {
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject visual;
    [SerializeField] private float RespawnTime,nostIncreaseAmount = 2f;
    private float currentRespawnTime;
    private bool show;
    private void Start(){
        if(audioSource == null){
            audioSource = GetComponent<AudioSource>();
        }
        show = true;
        currentRespawnTime = RespawnTime;
        if(photonView.IsMine){
            photonView.RPC(nameof(ShowHideVisualRpc),RpcTarget.All);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if(stream.IsWriting){
            stream.SendNext(currentRespawnTime);
        }else{
            currentRespawnTime = (float) stream.ReceiveNext();
        }
    }
    private void Update(){
        if(photonView.IsMine){
            if(!show){
                currentRespawnTime -= Time.deltaTime;
                if(currentRespawnTime <= 0f){
                    currentRespawnTime = RespawnTime;
                    show = true;
                    photonView.RPC(nameof(ShowHideVisualRpc),RpcTarget.All);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider coli){
        AudioManager.Current?.PlayAudioAtPos(audioSource,Sounds.SoundType.BoosterPickUp,true);
        if(coli.transform.TryGetComponent(out CarDriver driver)){
            show = false;
            driver.IncreaseNosTime(nostIncreaseAmount);
            if(photonView.IsMine){
                photonView.RPC(nameof(ShowHideVisualRpc),RpcTarget.All);
            }
        }
        if(coli.transform.TryGetComponent(out CarDriverAI driverAi)){
            show = false;
            driverAi.IncreaseNosTime(nostIncreaseAmount);
            if(photonView.IsMine){
                photonView.RPC(nameof(ShowHideVisualRpc),RpcTarget.All);
            }
        }
    }
    [PunRPC]
    private void ShowHideVisualRpc(){
        visual.SetActive(show);
    }
}