using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Dialogue
{
    /// <summary>
    /// 씬 로딩 헬퍼 클래스
    /// UnityEvent에서 직접 호출 가능한 씬 전환 메서드 제공
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        [Header("씬 전환 설정")]
        [SerializeField] private float delayBeforeLoad = 0.5f; // 씬 전환 전 대기 시간

        /// <summary>
        /// 씬 이름으로 로드 (즉시)
        /// </summary>
        /// <param name="sceneName">로드할 씬 이름</param>
        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[SceneLoader] 씬 이름이 비어있습니다.");
                return;
            }

            Debug.Log($"[SceneLoader] 씬 로드: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// 씬 인덱스로 로드 (즉시)
        /// </summary>
        /// <param name="sceneIndex">로드할 씬 인덱스</param>
        public void LoadScene(int sceneIndex)
        {
            Debug.Log($"[SceneLoader] 씬 로드: Index {sceneIndex}");
            SceneManager.LoadScene(sceneIndex);
        }

        /// <summary>
        /// 씬 이름으로 로드 (지연 포함)
        /// </summary>
        /// <param name="sceneName">로드할 씬 이름</param>
        public void LoadSceneWithDelay(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError("[SceneLoader] 씬 이름이 비어있습니다.");
                return;
            }

            StartCoroutine(LoadSceneCoroutine(sceneName));
        }

        /// <summary>
        /// 씬 인덱스로 로드 (지연 포함)
        /// </summary>
        /// <param name="sceneIndex">로드할 씬 인덱스</param>
        public void LoadSceneWithDelay(int sceneIndex)
        {
            StartCoroutine(LoadSceneCoroutine(sceneIndex));
        }

        /// <summary>
        /// 다음 씬으로 이동 (Build Settings 순서 기준)
        /// </summary>
        public void LoadNextScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = currentSceneIndex + 1;

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                Debug.Log($"[SceneLoader] 다음 씬 로드: Index {nextSceneIndex}");
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                Debug.LogWarning("[SceneLoader] 다음 씬이 없습니다. 첫 씬으로 이동합니다.");
                SceneManager.LoadScene(0);
            }
        }

        /// <summary>
        /// 현재 씬 재시작
        /// </summary>
        public void ReloadCurrentScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            Debug.Log($"[SceneLoader] 현재 씬 재시작: {SceneManager.GetActiveScene().name}");
            SceneManager.LoadScene(currentSceneIndex);
        }

        /// <summary>
        /// 게임 종료
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("[SceneLoader] 게임 종료");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 씬 로드 코루틴 (지연 포함)
        /// </summary>
        private IEnumerator LoadSceneCoroutine(string sceneName)
        {
            Debug.Log($"[SceneLoader] {delayBeforeLoad}초 후 씬 로드: {sceneName}");
            yield return new WaitForSeconds(delayBeforeLoad);
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// 씬 로드 코루틴 (지연 포함)
        /// </summary>
        private IEnumerator LoadSceneCoroutine(int sceneIndex)
        {
            Debug.Log($"[SceneLoader] {delayBeforeLoad}초 후 씬 로드: Index {sceneIndex}");
            yield return new WaitForSeconds(delayBeforeLoad);
            SceneManager.LoadScene(sceneIndex);
        }
    }
}

