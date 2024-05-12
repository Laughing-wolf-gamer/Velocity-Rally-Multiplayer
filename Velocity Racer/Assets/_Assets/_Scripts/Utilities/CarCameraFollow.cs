using UnityEngine;
using Cinemachine;

public class CarCameraFollow : MonoBehaviour {
    [SerializeField] private CinemachineVirtualCamera lookCam;
    [SerializeField] private Rigidbody carBody;
    [SerializeField] public Transform car;
	[SerializeField] private float distance = 4.33f;
	[SerializeField] private float height = 2.55f;
	[SerializeField] private float rotationDamping = 3.0f;
	[SerializeField] private float heightDamping = 2.0f;
	[SerializeField] private float zoomRatio = 0.5f;
	[SerializeField] private float defaultFOV = 60f;
    public bool canZoom{get; set;}

	private Vector3 rotationVector;
    private void Awake(){
        lookCam = GetComponent<CinemachineVirtualCamera>();
        lookCam.m_LookAt = null;
        lookCam.m_Follow = null;

    }
	void LateUpdate(){
		float wantedAngle = rotationVector.y;
		float wantedHeight = car.position.y + height;
		float myAngle = transform.eulerAngles.y;
		float myHeight = transform.position.y;

		myAngle = Mathf.LerpAngle(myAngle, wantedAngle, rotationDamping*Time.deltaTime);
		myHeight = Mathf.Lerp(myHeight, wantedHeight, heightDamping*Time.deltaTime);

		Quaternion currentRotation = Quaternion.Euler(0, myAngle, 0);
		transform.position = car.position;
		transform.position -= currentRotation * Vector3.forward*distance;
		Vector3 temp = transform.position; //temporary variable so Unity doesn't complain
		temp.y = myHeight;
		transform.position = temp;
		transform.LookAt(car);
	}

	void FixedUpdate(){
		Vector3 localVelocity = car.InverseTransformDirection(carBody.velocity);
		if (localVelocity.z < -0.1f){
			Vector3 temp = rotationVector; //because temporary variables seem to be removed after a closing bracket "}" we can use the same variable name multiple times.
			temp.y = car.eulerAngles.y + 180;
			rotationVector = temp;
		}
		else{
			Vector3 temp = rotationVector;
			temp.y = car.eulerAngles.y;
			rotationVector = temp;
		}
		float acc = carBody.velocity.magnitude;
        if(canZoom){
		    lookCam.m_Lens.FieldOfView = defaultFOV + acc * zoomRatio * Time.deltaTime;  //he removed * Time.deltaTime but it works better if you leave it like this.
        }
	}

}