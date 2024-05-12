using Photon.Pun;
using UnityEngine;
public class PunLagSync : MonoBehaviourPun, IPunObservable {
   //Values that will be synced over network
    private Vector3 latestPos;
    private Quaternion latestRot;
    //Lag compensation
    private float currentTime = 0;
    private double currentPacketTime = 0;
    private double lastPacketTime = 0;
    private Vector3 positionAtLastPacket = Vector3.zero;
    private Quaternion rotationAtLastPacket = Quaternion.identity;
    private void Awake(){
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 10;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        } else {
            //Network player, receive data
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();

            //Lag compensation
            currentTime = 0.0f;
            lastPacketTime = currentPacketTime;
            currentPacketTime = info.SentServerTime;
            positionAtLastPacket = transform.position;
            rotationAtLastPacket = transform.rotation;
        }
    }

    private void Update() {
        if (!photonView.IsMine) {
            //Lag compensation
            double timeToReachGoal = currentPacketTime - lastPacketTime;
            currentTime += Time.deltaTime;

            //Update remote player
            transform.position = Vector3.Lerp(positionAtLastPacket, latestPos, (float)(currentTime / timeToReachGoal));
            transform.rotation = Quaternion.Lerp(rotationAtLastPacket, latestRot, (float)(currentTime / timeToReachGoal));
        }
    }
}