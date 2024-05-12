using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IInputHandler{
    void ProcessesInput(Vector3 inputPosition,GameObject selectedObject,System.Action callBack);
    
}
