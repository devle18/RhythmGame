using UnityEngine;

/// <summary>
/// Quản lý việc hiển thị chữ ALL PERFECT lên màn hình sau khi đạt được.
/// Tách rời hoàn toàn khỏi logic đếm thời gian.
/// </summary>
public class AllPerfectUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Kéo AllPerfectManager vào đây")]
    [SerializeField] private AllPerfectManager _allPerfectManager;

    [Tooltip("Kéo cục Image chữ ALL PERFECT (đã bị ẩn) vào đây")]
    [SerializeField] private GameObject _allPerfectDisplayObject;

    private void OnEnable()
    {
        if (_allPerfectManager != null)
        {
            _allPerfectManager.OnAllPerfectAchieved += ShowAllPerfectText;
        }
    }

    private void OnDisable()
    {
        if (_allPerfectManager != null)
        {
            _allPerfectManager.OnAllPerfectAchieved -= ShowAllPerfectText;
        }
    }

    private void Start()
    {
        // Khởi đầu thì tắt hình đi
        if (_allPerfectDisplayObject != null)
        {
            _allPerfectDisplayObject.SetActive(false);
        }
    }

    /// <summary>
    /// Được gọi tự động khi hết 3 giây chờ từ AllPerfectManager
    /// </summary>
    private void ShowAllPerfectText()
    {
        if (_allPerfectDisplayObject != null)
        {
            _allPerfectDisplayObject.SetActive(true);
            Debug.Log("<color=yellow>[AllPerfectUI]</color> Đã bật hỉnh ảnh ALL PERFECT lên màn hình!");
            
            // Sau này bạn Nghĩa có thể gọi Animator hoặc Play Sound ở ngay dòng này:
            // GetComponent<Animator>().Play("AllPerfectPopUp");
            // GetComponent<AudioSource>().Play();
        }
    }
}
