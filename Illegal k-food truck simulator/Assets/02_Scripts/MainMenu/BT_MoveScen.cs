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
}
