using UnityEngine;
using UnityEngine.SceneManagement;

public class BT_MoveScen : MonoBehaviour
{
    // 인스펙터에서 지정할 씬 이름
    [SerializeField] private string sceneName = "GameScene";

    public void OnClickStartButton()
    {
        // 씬을 이름으로 로드
        SceneManager.LoadScene(sceneName);
    }

    private void EnsureGameManagerExists()
    {
        if (GameManager.Instance == null)
        {
            var go = new GameObject("_GameManager");
            go.AddComponent<GameManager>();
        }
    }

    /// <summary>
    /// 새 게임 버튼에 연결
    /// </summary>
    public void OnClickNewGame()
    {
        EnsureGameManagerExists();
        GameManager.Instance.NewGame();
    }

    /// <summary>
    /// 불러오기 버튼에 연결
    /// </summary>
    public void OnClickLoadGame()
    {
        EnsureGameManagerExists();
        
        if (!SaveManager.SaveExists())
        {
            Debug.LogWarning("저장된 게임이 없습니다!");
            return;
        }
        
        GameManager.Instance.LoadGame();
    }
}
