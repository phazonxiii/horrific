using UnityEngine;
using UnityEngine.UI;

namespace HorrorEngine
{
    public class UIPlayerStatus : MonoBehaviour
    {
        [SerializeField] private UIStatusLine m_Line;
        [SerializeField] private Image m_StatusBg;
        [SerializeField] private Image m_StatusIcon;
        [SerializeField] private TMPro.TextMeshProUGUI m_StatusText;

        private Status m_Status;
        private Health m_Health;

        // --------------------------------------------------------------------

        private void Start()
        {
            if (GameManager.Exists)
            {
                if (GameManager.Instance.Player)
                {
                    BindToPlayer(GameManager.Instance.Player);
                }
                else
                {
                    GameManager.Instance.OnPlayerRegistered.AddListener(OnPlayerRegistered);
                }
            }
        }

        // --------------------------------------------------------------------

        private void OnEnable()
        {
            if (m_Health)
                UpdateStatus();
        }

        // --------------------------------------------------------------------

        private void OnPlayerRegistered(PlayerActor player)
        {
            BindToPlayer(player);
        }

        // --------------------------------------------------------------------

        private void BindToPlayer(PlayerActor player)
        {
            Debug.Assert(m_Health == null, "UIPlayerStatus was already bound to a player");

            m_Health = player.GetComponent<Health>();
            m_Status = player.GetComponent<Status>();

            if (m_Health)
                m_Health.OnHealthAltered.AddListener(OnHealthAltered);

            if (m_Status)
                m_Status.OnStatusChanged.AddListener(OnStatusChanged);

            GameManager.Instance.OnPlayerRegistered.RemoveListener(OnPlayerRegistered);

            UpdateStatus();
        }

        // --------------------------------------------------------------------

        private void OnDestroy()
        {
            if (m_Health)
                m_Health.OnHealthAltered.RemoveListener(OnHealthAltered);

            if (m_Status)
                m_Status.OnStatusChanged.RemoveListener(OnStatusChanged);
        }

        // --------------------------------------------------------------------

        private void OnHealthAltered(float prev, float current)
        {
            UpdateStatus();
        }

        // --------------------------------------------------------------------

        private void OnStatusChanged(StatusData newStatus)
        {
            UpdateStatus();
        }

        // --------------------------------------------------------------------

        private void UpdateStatus()
        {
            if (m_Status == null)
                return;

            var currentStatus = m_Status.GetCurrentStatus();

            if (currentStatus != null)
            {
                m_StatusBg.color = currentStatus.Color;
                m_StatusText.text = currentStatus.Text;
                m_Line.SetStatus(currentStatus);
            }
        }
    }
}
