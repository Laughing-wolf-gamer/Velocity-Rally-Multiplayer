using UnityEngine;

namespace GamerWolf.Utils {
    public class GenericSingleton<TSingelton> : MonoBehaviour where TSingelton : Component {
        private static TSingelton current;
        public static TSingelton Current {
            get {
                if (current == null) {
                    current = FindObjectOfType<TSingelton>();
                    if (current == null) {
                        GameObject obj = new GameObject();
                        obj.name = typeof(TSingelton).Name;
                        current = obj.AddComponent<TSingelton>();
                    }
                }/* else{
                    Debug.LogError(nameof(TSingelton) + " is Found in the Scene");
                    Destroy(current.gameObject);
                } */
                return current;
            }
        }
        
    }
}