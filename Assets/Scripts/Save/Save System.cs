using System.IO;
using UnityEngine;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Hashing;
using System;


public static class SaveSystem
{
    private static readonly string DataPath = Application.persistentDataPath;
    private static readonly string SaveName = "Survivor.save";

    //
    // Registering
    //
    private static List<ISaveable> m_Saveables;

    public static void RegisterSaveable(ISaveable saveable)
    {
        if(saveable != null)
        {
            if(m_Saveables == null)
            {
                m_Saveables = new List<ISaveable>();
            }

            m_Saveables.Add(saveable);
        }
    }

    //
    // Saving/Loading
    //

    [System.Serializable]
    private struct SaveEntry
    {
        public int    Key;
        public string Value;
    }

    [System.Serializable]
    private struct SaveFile
    {
        public List<SaveEntry> m_Entries;

        public SaveFile(List<SaveEntry> entries)
        {
            m_Entries = entries;
        }
    }


    private static Dictionary<int, string> m_LoadedState;

    public static void SaveGameState()
    {
        var saveEntries = new List<SaveEntry>();
        foreach(var saveable in m_Saveables)
        {
            saveEntries.Add(new SaveEntry()
            {
                Key   = saveable.SaveKey,
                Value = saveable.SaveState(),
            });
        }

        var    saveFile    = new SaveFile(saveEntries);
        string fileContent = JsonUtility.ToJson(saveFile, prettyPrint:true);
        if(fileContent != null)
        {
            string filePath = Path.Combine(DataPath, SaveName);
            File.WriteAllText(filePath, fileContent);     
        }
    }

    public static void LoadGameState()
    {
        string filePath    = Path.Combine(DataPath, SaveName);
        string fileContent = File.ReadAllText(filePath);

        if(fileContent != null)
        {
            var saveFile = JsonUtility.FromJson<SaveFile>(fileContent);
            m_LoadedState = saveFile.m_Entries.ToDictionary(e => e.Key, e => e.Value);
        }
    }

    public static bool TryFindSaveData<T>(int saveKey, out T result)
    {
        bool foundValue = false;
        
        if(m_LoadedState != null)
        {
            if(m_LoadedState.TryGetValue(saveKey, out var value))
            {
                result     = JsonUtility.FromJson<T>(value);
                foundValue = true;
            }
            else
            {
                result = default;
            }
        }
        else
        {
            result = default;
        }

        return foundValue;
    }

    //
    // Helpers
    //

    public static int StringKeyToIntKey(string key)
    {
        Debug.Assert(key != null);

        var stringBytes = Encoding.UTF8.GetBytes(key);
        var hashBytes   = XxHash32.Hash(stringBytes);
        int result      = BitConverter.ToInt32(hashBytes);

        return result;
    }

    public static string AsSaveData<T>(T data)
    {
        string result = JsonUtility.ToJson(data);
        return result;
    }
}