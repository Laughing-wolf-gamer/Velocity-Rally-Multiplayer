using TMPro;
using UnityEngine;
public class CoinMultiplier : MonoBehaviour {
    [SerializeField] private bool isCoinMultiplier = true;

    [SerializeField] private GameDataSO gameData;
    [SerializeField] private TextMeshProUGUI coinAmountText;
    [SerializeField] private UICoinParitcalEffect uICoinParitcalEffect;

    public void CollectCoin(int coinValue,float increaseSpeed = 0.1f){
        StopAllCoroutines();
        // StartCoroutine(CollectionRoutine(coinValue,increaseSpeed));
        if(uICoinParitcalEffect != null){
            if(coinValue > 0){
                uICoinParitcalEffect.FromTo();
            }
        }
        gameData.IncreaseCoin(coinValue);
    }
    public void CollectCash(int cashValue,float increaseSped = 0.1f){
        StopAllCoroutines();
        if(cashValue > 0){
            uICoinParitcalEffect.FromTo();
        }
        // StartCoroutine(CollectionRoutine(dimondValue,increaseSped));
        gameData.IncreaseCash(cashValue);
    }
    
    // private IEnumerator CollectionRoutine(int coinValue,float increaseTime){
    //     int currentValue = playerData.GetCashAmount();
    //     coinAmountText.SetText(currentValue.ToString());
    //     int totalvalue = playerData.GetCashAmount() + coinValue;
    //     if(isCoinMultiplier){
    //         playerData.AddCoins(coinValue);
    //     }else{
    //         playerData.AddDimond(coinValue);
    //     }
    //     if(coinValue > 0){
    //         while(currentValue != totalvalue){
    //             currentValue++;
    //             coinAmountText.SetText(currentValue.ToString());
    //             yield return new WaitForSeconds(increaseTime);
    //         }
    //     }
        
    //     currentValue = 0;
        
    // }
    
}
