using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public string name;
    public int score;
    private Brick[] blocks;
    public int remBlocks;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void SavePlayerInformation(int m_score, string m_name)
    {
        SaveData data = new SaveData();
        score = OnBetterSaveInt(m_score);
        data.score = OnBetterSaveInt(m_score);

        name = OnBetterSaveString(m_name, m_score);
        data.name = OnBetterSaveString(m_name, m_score);


        string value = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", value);
    }

    public void LoadPlayerInformation()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path))
        {
            string savedScore = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(savedScore);
            score = data.score;
            name = data.name;
        }
    }

    int OnBetterSaveInt(int score)
    {
        string path = Application.persistentDataPath + "/savefile.json";
        string savedName = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(savedName);
        if (data.score > score && File.Exists(path))
        {
            return score;
        }
        else
        {
            return data.score;
        }
    }
    string OnBetterSaveString(string value, int m_score)
    {
        string path = Application.persistentDataPath + "/savefile.json";
        string savedScore = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(savedScore);
        if (data.name == value && File.Exists(path))
        {
            return data.name;
        }
        else
        {
            return value;
        }
    }

    public void AddBlock()
    {
        blocks = FindObjectsOfType<Brick>();
        remBlocks = blocks.Length;
    }

    public bool Winner()
    {
        return remBlocks == 0 ? true : false;
    }
}


[Serializable]
public class SaveData
{
    public string name;
    public int score;
}
