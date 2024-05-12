using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;
public class UICoinParitcalEffect : MonoBehaviour {
    
    [SerializeField] private bool isCoinParitcal = true;
    [SerializeField] private GameObject parentObject;
    [SerializeField] private GameObject origin;
    [SerializeField] private GameObject destination;
    [SerializeField] private Ease easeType;
    [SerializeField] private float time;
    [SerializeField] private float rate;
    [SerializeField] private float dealy = 0.8f;
    [SerializeField] private GameObject[] firstDestination;
    [SerializeField] private Vector3 offset;
    [SerializeField] private UnityEvent onEffectComplete;
    private List<GameObject> objectsList;
    private void Start(){
        objectsList = new List<GameObject>();
    }
    public void FromTo(){
        // StartCoroutine(FromToRoutine());
    }
    /* private IEnumerator FromToRoutine(){
        foreach(GameObject firstPoint in firstDestination){

            GameObject vfx = poolingManager.SpawnFromPool((isCoinParitcal ? PoolObjectTag.cashUIParitical : PoolObjectTag.DimondUiPartical),origin.transform.position,Quaternion.identity) as GameObject;
            PooledObject poolObject = vfx.GetComponent<PooledObject>();
            poolObject.transform.SetParent(parentObject.transform);
            if(!objectsList.Contains(poolObject.gameObject)){
                objectsList.Add(poolObject.gameObject);
            }
            iTween.MoveTo(poolObject.gameObject,iTween.Hash("Position",firstPoint.transform.position + offset,"easyType",easeType,"time",time));
            poolObject.DestroyMySelf(time + 1.2f);
            yield return null;
        }
        yield return new WaitForSeconds(dealy);

        if(objectsList.Count > 0){
            for (int i = 0; i < objectsList.Count; i++){
                iTween.MoveTo(objectsList[i],iTween.Hash("Position",destination.transform.position + offset,"easyType",easeType,"time",time));
            }
        }
        yield return new WaitForSeconds(2f);
        onEffectComplete?.Invoke();
    } */
}
