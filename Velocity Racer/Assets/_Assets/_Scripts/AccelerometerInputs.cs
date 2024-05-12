using UnityEngine;
[RequireComponent(typeof(OrentationHandler))]
public class AccelerometerInputs : MonoBehaviour {
    [SerializeField] private GameDataSO gameData;
    [SerializeField] private float steerFactor = 1;
    [SerializeField] private bool useAccelerometer = true;

    [Tooltip("Multiplier for input addition based on rate of change of input")]
    [SerializeField] private float deltaFactor = 10;
    private Vector3 accelerationPrev;
    private Vector3 accelerationDelta;
    private OrentationHandler orentationHandler;
    private float currentSteer;
    private void Awake() {
        orentationHandler = GetComponent<OrentationHandler>();
    }

    public float GetSteeringValue() {
        if (orentationHandler != null) {
            accelerationDelta = Input.acceleration - accelerationPrev;
            accelerationPrev = Input.acceleration;

            if (useAccelerometer) {
                // Accelerometer input
                deltaFactor = gameData.GetControllsSensitivity();
                currentSteer = (Input.acceleration.x + accelerationDelta.x * deltaFactor) * steerFactor;
            }
            else {
                currentSteer = orentationHandler.steer;
            }
        }
        return currentSteer;
    }
    
}