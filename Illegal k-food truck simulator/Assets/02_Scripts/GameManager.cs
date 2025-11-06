using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 전체 상태를 관리하는 매니저 (싱글톤)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Scene Names")]
    public string tutorialSceneName = "01_in a dream";
    public string gameSceneName = "02_game";

    public GameSave Save { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 기본 Save 객체 생성
        if (Save == null) Save = new GameSave();
    }

    /// <summary>
    /// 새 게임 시작 (튜토리얼부터)
    /// </summary>
    public void NewGame()
    {
        Save = new GameSave();
        Save.currentDay = 1;
        Save.money = 0;
        Save.todayEarnings = 0;
        Save.totalEarnings = 0;
        Save.tutorialCompleted = false;

        Debug.Log("새 게임 시작");
        SceneManager.LoadScene(tutorialSceneName);
    }

    /// <summary>
    /// 저장된 게임 불러오기
    /// </summary>
    public void LoadGame()
    {
        GameSave loaded = SaveManager.LoadGame();
        if (loaded == null)
        {
            Debug.LogWarning("불러올 저장 데이터가 없습니다.");
            return;
        }

        Save = loaded;
        Save.todayEarnings = 0;
        
        // 레시피 해금 상태 복원
        if (RecipeUnlockManager.Instance != null)
        {
            RecipeUnlockManager.Instance.LoadUnlockedRecipes(Save.unlockedRecipeIds);
        }
        
        // 인벤토리 상태 복원
        if (Inventory.Instance != null)
        {
            Inventory.Instance.LoadFromSaveData(Save.inventorySlots);
        }
        
        Debug.Log($"게임 로드 완료 - Day {Save.currentDay}, 돈: {Save.money}원, 총 매출: {Save.totalEarnings}원");
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// 하루 종료 후 저장 (확인 버튼에서 호출)
    /// </summary>
    public void EndDayAndSave()
    {
        Save.totalEarnings += Save.todayEarnings;
        Save.currentDay += 1;

        // 레시피 해금 상태 저장
        if (RecipeUnlockManager.Instance != null)
        {
            Save.unlockedRecipeIds = RecipeUnlockManager.Instance.GetUnlockedRecipeIds();
        }
        
        // 인벤토리 상태 저장
        if (Inventory.Instance != null)
        {
            Save.inventorySlots = Inventory.Instance.GetSaveData();
        }

        SaveManager.SaveGame(Save);

        Debug.Log($"Day {Save.currentDay - 1} 종료, 오늘 매출: {Save.todayEarnings}원, 총 누적 매출: {Save.totalEarnings}원");

        Save.todayEarnings = 0;

        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// 튜토리얼 완료 후 호출
    /// </summary>
    public void CompleteTutorial()
    {
        Save.tutorialCompleted = true;
        Debug.Log("튜토리얼 완료");
        SceneManager.LoadScene(gameSceneName);
    }
}
