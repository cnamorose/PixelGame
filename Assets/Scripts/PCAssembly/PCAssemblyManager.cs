using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 드래그 중인 부품과 같은 종류의 슬롯만 보여주고,
/// 슬롯 반경 안에서 놓았을 때 부품을 정확한 위치에 스냅합니다.
/// </summary>
public class PCAssemblyManager : MonoBehaviour
{
    [SerializeField] private PCAssemblyItem[] assemblyItems;
    [SerializeField] private PCAssemblySlot[] assemblySlots;
    [SerializeField] private PCScrewManager screwManager;
    [SerializeField] private PCCoverDrop coverDrop;
    [SerializeField] private PCPowerOutlet powerOutlet;
    [SerializeField] private GameObject completionObject;
    [SerializeField] private UnityEvent onAssemblyComplete;
    [Header("SFX")]
    [SerializeField] private AudioClip partPlacedSfx;

    private int placedCount;
    private bool isComplete;

    private void Start()
    {
        if (completionObject != null)
            completionObject.SetActive(false);

        foreach (PCAssemblySlot slot in assemblySlots)
            slot.SetHighlight(false);

        // 조립 부품은 KeyboardMonster에서 세는 전투 보상과 별개입니다.
        // PC 조립 씬에서는 항상 준비된 작업용 부품으로 보여 줍니다.
    }

    public bool BeginDrag(PCAssemblyItem item)
    {
        if (isComplete || item == null || item.IsPlaced)
            return false;

        // 아이템을 잡으면 (맞는 슬롯만이 아니라) 비어 있는 모든 슬롯을 보여 준다.
        foreach (PCAssemblySlot slot in assemblySlots)
            slot.SetHighlight(true);

        return true;
    }

    public void EndDrag(PCAssemblyItem item)
    {
        PCAssemblySlot targetSlot = null;

        // [진단 로그] 문제 파악 후 이 블록은 지워도 됩니다.
        Debug.Log($"[Assembly] '{item.name}' 놓음 (PartId={item.PartId}), 위치={item.transform.position}. 슬롯 {assemblySlots.Length}개 검사:");
        foreach (PCAssemblySlot slot in assemblySlots)
        {
            bool canAccept = slot.CanAccept(item);
            bool inRange = slot.IsWithinSnapRange(item.transform.position);
            Debug.Log($"    - {slot.name}: CanAccept={canAccept} (Accepts={slot.AcceptedPart}), InRange={inRange}");
        }

        foreach (PCAssemblySlot slot in assemblySlots)
        {
            if (slot.CanAccept(item) && slot.IsWithinSnapRange(item.transform.position))
            {
                targetSlot = slot;
                break;
            }
        }

        foreach (PCAssemblySlot slot in assemblySlots)
            slot.SetHighlight(false);

        if (targetSlot == null)
        {
            item.ReturnToStart();
            return;
        }

        targetSlot.Place(item);
        placedCount++;

        if (placedCount >= assemblySlots.Length)
        {
            if (screwManager != null)
                screwManager.UnlockScrews();

            TryCompleteAssembly();
        }
    }

    private void Update()
    {
        if (!isComplete && placedCount >= assemblySlots.Length)
            TryCompleteAssembly();
    }

    private void TryCompleteAssembly()
    {
        if (screwManager != null && !screwManager.IsComplete)
            return;

        if (coverDrop != null)
        {
            coverDrop.Drop();
            if (!coverDrop.IsFinished)
                return;
        }

        if (powerOutlet != null)
        {
            powerOutlet.Unlock();
            if (!powerOutlet.IsPowerOnVisualReady)
                return;
        }

        CompleteAssembly();
    }

    private void CompleteAssembly()
    {
        if (isComplete)
            return;

        isComplete = true;
        PCAssemblyProgress.Complete();

        if (completionObject != null)
            completionObject.SetActive(true);

        onAssemblyComplete?.Invoke();
    }

    public void PlayPartPlacedSfx()
    {
        if (partPlacedSfx != null && AudioManager.Instance != null)
            AudioManager.Instance.PlayOneShotSFX(partPlacedSfx);
    }
}
