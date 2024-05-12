using System.IO;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
public static class SaveSystemManager {
    /// <summary>
    /// Save object with a string identifier
    /// </summary>
    /// <typeparam name="T">Type of object to save</typeparam>
    /// <param name="objectToSave">Object to save</param>
    /// <param name="key">String identifier for the data to load</param>
    public static void Save<T>(T objectToSave, string key) {
        SaveToFile<T>(objectToSave, key);
    }

    /// <summary>
    /// Save object with a string identifier
    /// </summary>
    /// <param name="objectToSave">Object to save</param>
    /// <param name="key">String identifier for the data to load</param>
    public static void Save(Object objectToSave, string key) {
        SaveToFile<Object>(objectToSave, key);
    }
    public static bool HasSaveFile(string fileName){
        // Check if FileName Exist doesn't already exist
        string path = string.Concat(Application.persistentDataPath, "/saves/");
        string pathAndNameCombine = string.Concat(path , fileName , ".txt");
        if(!Directory.Exists(path)) return false;
        DebugLog($"{pathAndNameCombine} file Exist.");
        return File.Exists(pathAndNameCombine);
    }
    public static void DeleteFile(string fileName){
        // Check if FileName Exist doesn't already exist
        string path = string.Concat(Application.persistentDataPath, "/saves/");
        string pathAndNameCombine = string.Concat(path , fileName , ".txt");
        if(File.Exists(pathAndNameCombine)){
            DebugLog(string.Concat(pathAndNameCombine," file Deleted at."));
            File.Delete(pathAndNameCombine);
        }
    }

    /// <summary>
    /// Handle saving data to File
    /// </summary>
    /// <typeparam name="T">Type of object to save</typeparam>
    /// <param name="objectToSave">Object to serialize</param>
    /// <param name="fileName">Name of file to save to</param>
    private static void SaveToFile<T>(T objectToSave, string fileName) {
        // Set the path to the persistent data path (works on most devices by default)
        string path = string.Concat(Application.persistentDataPath, "/saves/");
        // Create the directory IF it doesn't already exist
        if(!Directory.Exists(path)) { 
            Directory.CreateDirectory(path);
        }else{
            DebugLog($"{path} Directory Already Exist.");
        }
        // Grab an instance of the BinaryFormatter that will handle serializing our data
        BinaryFormatter formatter = new BinaryFormatter();
        // Open up a filestream, combining the path and object key
        string pathAndFileNameCombine = string.Concat(path , fileName , ".txt");
        FileStream fileStream = File.Create(pathAndFileNameCombine);
        // Try/Catch/Finally block that will attempt to serialize/write-to-stream, closing stream when complete
        try {
            formatter.Serialize(fileStream, objectToSave);
            DebugLog(string.Concat("Saveing Is Successfull" ," at ",System.DateTime.Today , System.DateTime.Now));
        } catch (SerializationException exception) {
            DebugLog("Save failed. Error: " + exception.Message);
        }  finally {
            fileStream.Close();
        }
    }

    /// <summary>
    /// Load data using a string identifier
    /// </summary>
    /// <typeparam name="T">Type of object to load</typeparam>
    /// <param name="key">String identifier for the data to load</param>
    /// <returns></returns>
    public static T Load<T>(string key) {
        // Set the path to the persistent data path (works on most devices by default)
        string path = string.Concat(Application.persistentDataPath,"/saves/");
        // Grab an instance of the BinaryFormatter that will handle serializing our data
        BinaryFormatter formatter = new BinaryFormatter();
        string pathAndFileNameCombine = string.Concat(path , key , ".txt");
        // Open up a filestream, combining the path and object key
        FileStream fileStream = File.Open(pathAndFileNameCombine, FileMode.Open);
        DebugLog(string.Concat("Loaded Data " ," at ",System.DateTime.Today, "/",System.DateTime.Now));
        // Initialize a variable with the default value of whatever type we're using
        T returnValue = default(T);
        /* 
         * Try/Catch/Finally block that will attempt to deserialize the data
         * If we fail to successfully deserialize the data, we'll just return the default value for Type
         */
        try {
            returnValue = (T)formatter.Deserialize(fileStream);
        } catch (SerializationException exception) {
            DebugLog("Load failed. Error: " + exception.Message);
        } finally {
            fileStream.Close();
        }
        return returnValue;
    }
    public static void DebugLog(string val){
        Debug.Log($"<color=green>[Save System]</color> <color=cyan>[{val}]</color>");
    }
}