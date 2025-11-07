using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Minigame
{
    /// <summary>미니게임 관리자 - 싱글톤</summary>
    public class MiniGameManager : MonoBehaviour
    {
        private static MiniGameManager instance;
        public static MiniGameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("MiniGameManager");
                    instance = go.AddComponent<MiniGameManager>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        [Header("미니게임 프리팹")]
        public List<MinigamePrefabMapping> minigamePrefabs = new List<MinigamePrefabMapping>();

        [Header("UI 설정")]
        public Canvas dimmerCanvas;             // 배경 어둡게
        public CanvasGroup dimmerCanvasGroup;

        private IMinigame currentMinigame;
        private GameObject currentMinigameObject;
        private Action<MiniGameResult> currentCallback;
        private PlayerInput playerInput;        // 플레이어 Input Action Map

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Dimmer Canvas 초기화
            if (dimmerCanvas == null)
            {
                CreateDimmerCanvas();
            }
        }

        /// <summary>미니게임 시작</summary>
        public void StartMinigame(MinigameId id, Action<MiniGameResult> onCompleted)
        {
            if (currentMinigame != null)
            {
                Debug.LogWarning("미니게임이 이미 실행 중입니다!");
                return;
            }

            // 프리팹 찾기
            MinigamePrefabMapping mapping = minigamePrefabs.Find(m => m.id == id);
            if (mapping == null || mapping.prefab == null)
            {
                Debug.LogError($"미니게임 프리팹을 찾을 수 없습니다: {id}");
                onCompleted?.Invoke(new MiniGameResult(0f, 0f, true));
                return;
            }

            currentCallback = onCompleted;

            // Dimmer 활성화
            ShowDimmer(true);

            // 플레이어 입력 비활성화
            DisablePlayerInput();

            // 프리팹 생성
            currentMinigameObject = Instantiate(mapping.prefab);
            currentMinigame = currentMinigameObject.GetComponent<IMinigame>();

            if (currentMinigame == null)
            {
                Debug.LogError($"미니게임 컴포넌트를 찾을 수 없습니다: {id}");
                Destroy(currentMinigameObject);
                currentMinigameObject = null;
                ShowDimmer(false);
                EnablePlayerInput();
                onCompleted?.Invoke(new MiniGameResult(0f, 0f, true));
                return;
            }

            // 미니게임 시작
            currentMinigame.Begin(OnMinigameFinished);
        }

        /// <summary>현재 미니게임 중단</summary>
        public void AbortCurrentMinigame()
        {
            if (currentMinigame != null)
            {
                currentMinigame.Abort();
            }
        }

        private void OnMinigameFinished(MiniGameResult result)
        {
            // 참조를 먼저 정리 (콜백에서 다음 미니게임을 시작할 수 있도록)
            var callback = currentCallback;
            currentMinigame = null;
            currentMinigameObject = null;
            currentCallback = null;

            // Dimmer 비활성화
            ShowDimmer(false);

            // 플레이어 입력 복구
            EnablePlayerInput();

            // 콜백 실행 (참조 정리 후)
            callback?.Invoke(result);
        }

        private void CreateDimmerCanvas()
        {
            // Dimmer Canvas 생성
            GameObject dimmerObj = new GameObject("MinigameDimmer");
            dimmerObj.transform.SetParent(transform);

            dimmerCanvas = dimmerObj.AddComponent<Canvas>();
            dimmerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            dimmerCanvas.sortingOrder = 9999;

            dimmerObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            dimmerCanvasGroup = dimmerObj.AddComponent<CanvasGroup>();
            dimmerCanvasGroup.alpha = 0f;
            dimmerCanvasGroup.blocksRaycasts = false;

            // 반투명 검정 배경
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(dimmerObj.transform);

            UnityEngine.UI.Image bgImage = bgObj.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(0, 0, 0, 0.5f);

            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            dimmerCanvas.gameObject.SetActive(false);
        }

        private void ShowDimmer(bool show)
        {
            if (dimmerCanvas != null)
            {
                dimmerCanvas.gameObject.SetActive(show);
                if (dimmerCanvasGroup != null)
                {
                    dimmerCanvasGroup.alpha = show ? 0.5f : 0f;
                    dimmerCanvasGroup.blocksRaycasts = show;
                }
            }
        }

        private void DisablePlayerInput()
        {
            // 플레이어의 PlayerInput 컴포넌트 찾기
            if (playerInput == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerInput = player.GetComponent<PlayerInput>();
                }
            }

            if (playerInput != null)
            {
                // 이동만 비활성화 (UI 액션은 유지)
                playerInput.currentActionMap.FindAction("Move")?.Disable();
                // 전체 입력을 비활성화하지 않음 - Input.GetKeyDown은 계속 작동
            }
        }

        private void EnablePlayerInput()
        {
            if (playerInput != null)
            {
                // 이동 다시 활성화
                playerInput.currentActionMap.FindAction("Move")?.Enable();
            }
        }

        private void Update()
        {
            // ESC 키로 미니게임 중단 (디버그용)
            if (currentMinigame != null && Input.GetKeyDown(KeyCode.Escape))
            {
                AbortCurrentMinigame();
            }
        }
    }

    /// <summary>미니게임 프리팹 매핑</summary>
    [Serializable]
    public class MinigamePrefabMapping
    {
        public MinigameId id;
        public GameObject prefab;
    }
}