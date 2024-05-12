using GamerWolf.Utils;
using UnityEngine;
using UnityEngine.Events;
public class UIInputReciver : InputReciver {
    
    [SerializeField] private UnityEvent clickEvent;

    public override void OnInputRecives() {
        foreach(var handlers in inputHandlers){
            handlers.ProcessesInput(Input.mousePosition,gameObject,() => {
                clickEvent?.Invoke();
                AudioManager.Current?.PlayOneShotMusic(Sounds.SoundType.BtnClick);
            });
        }
    }
}
