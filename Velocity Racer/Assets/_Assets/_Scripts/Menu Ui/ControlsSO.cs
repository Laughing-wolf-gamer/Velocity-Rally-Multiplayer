using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Configs/ControlsSO", fileName = "ControlsSO")]
public class ControlsSO : ScriptableObject {
    public bool isActive;
    public ControllsSelectionManager.ControllType controllType;

}