using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [SerializeField] private string id; // 아이템 ID
    [SerializeField] private string displayName; // 아이템 표시 이름
    [SerializeField] private ItemType _type; // 아이템 타입
    [SerializeField] private string ingredientKey; // 재료 키
    [SerializeField] private Sprite icon; // 아이템 아이콘
    [SerializeField] private int maxStack = 99; // 최대 스택 수

    public string Id => id;
    public string DisplayName => displayName;
    public ItemType Type => _type;
    public string IngredientKey => ingredientKey;
    public Sprite Icon => icon;
    public int MaxStack => maxStack;
}
