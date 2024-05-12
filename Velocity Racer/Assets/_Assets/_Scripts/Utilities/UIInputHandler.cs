using System;
using UnityEngine;

public class UIInputHandler : MonoBehaviour, IInputHandler {
    public void ProcessesInput(Vector3 inputPosition, GameObject selectedObject, Action callBack) {
        callBack?.Invoke();
    }
}
