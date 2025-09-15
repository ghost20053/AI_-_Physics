using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countDownTimerText;
    [SerializeField] private float remainingTime = 300f; // Default 5 minutes

    private void Update()
    {
        if (PauseMenu.isPaused) return; // don’t tick if paused

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime <= 0)
        {
            remainingTime = 0;
            SceneManager.LoadScene("Menu");
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        countDownTimerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void ToggleUI(bool visible)
    {
        if (countDownTimerText != null)
        {
            countDownTimerText.gameObject.SetActive(visible);
        }
    }
}
