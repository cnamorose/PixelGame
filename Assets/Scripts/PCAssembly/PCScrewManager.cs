using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 나사 배치와 체결 완료 상태를 관리합니다.
/// </summary>
public class PCScrewManager : MonoBehaviour
{
    [SerializeField] private PCScrewItem[] screwItems;
    [SerializeField] private PCScrewSlot[] screwSlots;
    [SerializeField] private GameObject screwsRoot;
    [SerializeField] private ScrewBoxLid screwBoxLid;
    [Tooltip("어떤 종류의 나사를 넣어도 슬롯 안에서는 이 공통 스프라이트로 표시합니다.")]
    [SerializeField] private Sprite installedScrewSprite;
    [SerializeField] private UnityEvent onAllScrewsTightened;
    [Header("SFX")]
    [SerializeField] private AudioClip screwTurnSfx;

    private int tightenedCount;
    private bool isUnlocked;

    public bool IsComplete => screwSlots.Length > 0 && tightenedCount >= screwSlots.Length;
    public Sprite InstalledScrewSprite => installedScrewSprite;
    public AudioClip ScrewTurnSfx => screwTurnSfx;

    private void Start()
    {
        foreach (PCScrewSlot slot in screwSlots)
            slot.SetHighlight(false);

        // 나사는 처음부터 화면에 보이지만, BeginDrag에서 isUnlocked를 확인해
        // 부품 조립이 끝나기 전에는 사용할 수 없게 합니다.
    }

    public void UnlockScrews()
    {
        if (isUnlocked)
            return;

        isUnlocked = true;
        if (screwBoxLid != null)
            screwBoxLid.Open();

    }

    public bool BeginDrag(PCScrewItem item)
    {
        if (!isUnlocked || item == null || item.IsInserted || item.IsTightened || !IsRegisteredScrew(item))
            return false;

        foreach (PCScrewSlot slot in screwSlots)
            slot.SetHighlight(true);

        return true;
    }

    private bool IsRegisteredScrew(PCScrewItem item)
    {
        foreach (PCScrewItem screw in screwItems)
        {
            if (screw == item)
                return true;
        }

        return false;
    }

    public void EndDrag(PCScrewItem item)
    {
        PCScrewSlot targetSlot = null;
        foreach (PCScrewSlot slot in screwSlots)
        {
            if (slot.CanAccept(item) && slot.IsWithinRange(item.transform.position))
            {
                targetSlot = slot;
                break;
            }
        }

        foreach (PCScrewSlot slot in screwSlots)
            slot.SetHighlight(false);

        if (targetSlot == null)
        {
            item.ReturnToStart();
            return;
        }

        targetSlot.Insert(item);
    }

    public void NotifyTightened(PCScrewItem item)
    {
        tightenedCount++;

        if (IsComplete)
            onAllScrewsTightened?.Invoke();
    }

}
