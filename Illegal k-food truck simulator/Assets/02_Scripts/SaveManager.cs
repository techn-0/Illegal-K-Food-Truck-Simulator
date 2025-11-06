using System;
using System.IO;
using UnityEngine;

/// <summary>
/// 게임 저장/불러오기를 관리하는 정적 클래스
/// </summary>
public static class SaveManager
{
    private static readonly string DefaultFileName = "save.json";

    public static string GetSavePath(string fileName = null)
    {
        if (string.IsNullOrEmpty(fileName)) fileName = DefaultFileName;
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    public static void SaveGame(GameSave save, string fileName = null)
    {
        try
        {
            string path = GetSavePath(fileName);
            string json = JsonUtility.ToJson(save, true);
            File.WriteAllText(path, json);
            Debug.Log($"게임 저장 완료: {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"게임 저장 실패: {e}");
        }
    }

    public static GameSave LoadGame(string fileName = null)
    {
        try
        {
            string path = GetSavePath(fileName);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"저장 파일을 찾을 수 없습니다: {path}");
                return null;
            }

            string json = File.ReadAllText(path);
            GameSave save = JsonUtility.FromJson<GameSave>(json);
            Debug.Log($"게임 로드 완료: {path}");
            return save;
        }
        catch (Exception e)
        {
            Debug.LogError($"게임 로드 실패: {e}");
            return null;
        }
    }

    public static bool SaveExists(string fileName = null)
    {
        string path = GetSavePath(fileName);
        return File.Exists(path);
    }

    public static string[] ListSaves()
    {
        try
        {
            string dir = Application.persistentDataPath;
            if (!Directory.Exists(dir)) return new string[0];
            
            var files = Directory.GetFiles(dir, "*.json");
            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }
            return files;
        }
        catch (Exception e)
        {
            Debug.LogError($"저장 파일 목록 조회 실패: {e}");
            return new string[0];
        }
    }

    public static bool DeleteSave(string fileName)
    {
        try
        {
            string path = GetSavePath(fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"저장 파일 삭제 완료: {path}");
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.LogError($"저장 파일 삭제 실패: {e}");
            return false;
        }
    }
}

