using Photon.Pun;
using UnityEngine;
public class Pun2_RigidbodySync : MonoBehaviourPun, IPunObservable {
    private Rigidbody rb;

    private Vector3 latestPos;
    private Quaternion latestRot;
    private Vector3 velocity;
    private Vector3 angularVelocity;
    private bool valuesReceived = false;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
        } else {
            //Network player, receive data
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
            velocity = (Vector3)stream.ReceiveNext();
            angularVelocity = (Vector3)stream.ReceiveNext();

            valuesReceived = true;
        }
    }

    private void Update() {
        if (!photonView.IsMine && valuesReceived) {
            //Update Object position and Rigidbody parameters
            transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 5);
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }
}