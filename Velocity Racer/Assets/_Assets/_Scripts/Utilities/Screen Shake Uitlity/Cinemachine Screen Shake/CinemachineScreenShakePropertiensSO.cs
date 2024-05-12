using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamerWolf.Utils{
    [CreateAssetMenu(fileName = "New Cinemachine Screen Shake Propetie",menuName = "ScriptableObject/Cinemachine Screen Shake/Properties")]
    public class CinemachineScreenShakePropertiensSO : ScriptableObject{
        public float intensity = 4f;
        public float time = 0.2f;
    }

}