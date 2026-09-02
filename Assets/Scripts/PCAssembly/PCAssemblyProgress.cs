using UnityEngine;

/// <summary>
/// PC 조립 완료 상태만 씬 전환 뒤에도 유지합니다.
/// KeyboardMonster의 부품 카운트와는 의도적으로 분리되어 있습니다.
/// </summary>
public class PCAssemblyProgress : MonoBehaviour
{
    private static PCAssemblyProgress instance;

    public static bool IsComplete { get; private set; }

    public static void Complete()
    {
        IsComplete = true;
    }

    public static void ResetProgress()
    {
        EnsureInstance();
        IsComplete = false;
    }

    private static PCAssemblyProgress EnsureInstance()
    {
        if (instance != null)
            return instance;

        GameObject progressObject = new GameObject("PCAssemblyProgress");
        instance = progressObject.AddComponent<PCAssemblyProgress>();
        DontDestroyOnLoad(progressObject);
        return instance;
    }
}
