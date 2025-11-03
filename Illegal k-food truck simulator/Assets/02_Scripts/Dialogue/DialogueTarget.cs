using UnityEngine;

namespace Dialogue
{
    /// <summary>
    /// 대화 상호작용 가능한 오브젝트
    /// 특정 CSV 대화 데이터와 연결되며, 대화 완료 시 보상 제공 가능
    /// </summary>
    public class DialogueTarget : MonoBehaviour
    {
        [Header("대화 설정")]
        [SerializeField] private TextAsset dialogueCSV;  // 대화 CSV 파일
        [SerializeField] private int startDialogueId;     // 시작 대화 ID

        [Header("대화 완료 보상")]
        [SerializeField] private ItemReward[] itemRewards;     // 지급할 아이템들
        [SerializeField] private RecipeDefinition[] recipeRewards; // 해금할 레시피들

        /// <summary>
        /// 대화 CSV 데이터 (읽기 전용)
        /// </summary>
        public TextAsset DialogueCSV => dialogueCSV;

        /// <summary>
        /// 시작 대화 ID (읽기 전용)
        /// </summary>
        public int StartDialogueId => startDialogueId;

        /// <summary>
        /// 대화 시작 시도
        /// </summary>
        /// <param name="dialogueManager">대화 매니저</param>
        /// <returns>대화 시작 성공 여부</returns>
        public bool TryStartDialogue(DialogueManager dialogueManager)
        {
            if (dialogueManager == null || dialogueCSV == null)
            {
                return false;
            }

            // 대화 매니저에 CSV 로드 및 시작 (자신을 타겟으로 전달)
            dialogueManager.LoadAndStartDialogue(dialogueCSV, startDialogueId, this);
            return true;
        }

        /// <summary>
        /// 대화 완료 시 호출 - 보상 지급
        /// </summary>
        public void OnDialogueComplete()
        {
            GiveItemRewards();
            UnlockRecipeRewards();
        }

        /// <summary>
        /// 아이템 보상 지급
        /// </summary>
        private void GiveItemRewards()
        {
            if (itemRewards == null || itemRewards.Length == 0) return;

            Inventory playerInventory = FindFirstObjectByType<Inventory>();
            if (playerInventory == null)
            {
                Debug.LogWarning("플레이어 인벤토리를 찾을 수 없습니다.");
                return;
            }

            foreach (var reward in itemRewards)
            {
                if (reward.item != null && reward.amount > 0)
                {
                    int added = playerInventory.AddItem(reward.item, reward.amount);
                    Debug.Log($"[DialogueTarget] 아이템 지급: {reward.item.DisplayName} x{added}");
                }
            }
        }

        /// <summary>
        /// 레시피 보상 해금
        /// </summary>
        private void UnlockRecipeRewards()
        {
            if (recipeRewards == null || recipeRewards.Length == 0) return;

            if (RecipeUnlockManager.Instance == null)
            {
                Debug.LogWarning("RecipeUnlockManager를 찾을 수 없습니다.");
                return;
            }

            foreach (var recipe in recipeRewards)
            {
                if (recipe != null)
                {
                    bool unlocked = RecipeUnlockManager.Instance.UnlockRecipe(recipe);
                    if (unlocked)
                    {
                        Debug.Log($"[DialogueTarget] 레시피 해금: {recipe.RecipeName}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 아이템 보상 데이터
    /// </summary>
    [System.Serializable]
    public class ItemReward
    {
        public ItemDefinition item;  // 지급할 아이템
        public int amount = 1;       // 지급할 수량
    }
}
