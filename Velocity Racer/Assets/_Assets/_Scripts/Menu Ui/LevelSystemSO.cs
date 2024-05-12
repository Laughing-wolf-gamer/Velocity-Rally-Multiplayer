using System;
using UnityEngine;
[CreateAssetMenu(fileName = "Level System",menuName = "Configs/Level System")]
public class LevelSystemSO : ScriptableObject {
    public int level;
    public int experiance;
    public int experianceToNextLevel = 100;
    public LevelSystemSaveData levelSystemSaveData;
    public void AddExperiance(int amount){
        experiance += amount;
        while(experiance >= experianceToNextLevel){
            level ++;
            experiance -= experianceToNextLevel;
        }
        SetLevelSaveData();
    }
    public void SetLevelSaveData(){
        levelSystemSaveData.saveLevelAmount = level;
        levelSystemSaveData.saveExperience = experiance;
        levelSystemSaveData.saveExperianceToNextLevel = experianceToNextLevel;
        SaveSystemManager.Save<LevelSystemSaveData>(levelSystemSaveData,KeysHolder.LEVEL_DATA_SAVE_KEY);
    }
    public void Load(){
        levelSystemSaveData = SaveSystemManager.Load<LevelSystemSaveData>(KeysHolder.LEVEL_DATA_SAVE_KEY);
    }
    public int GetLevelNumber(){
        return level + 1;
    }
    public float GetExperianceNormalized(){
        return (float)experiance / experianceToNextLevel;
    }

    public void DecreaseExp(int amount) {
        experiance -= amount;
        while(experiance < experianceToNextLevel){
            level--;
            if(level <= 0){
                level = 0;
            }
            experiance = 0;
        }
        SetLevelSaveData();
    }

    [Serializable]
    public class LevelSystemSaveData{
        public int saveLevelAmount,saveExperience,saveExperianceToNextLevel;
    }
}

