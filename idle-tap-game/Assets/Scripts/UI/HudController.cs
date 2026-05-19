using System.Collections.Generic;
using IdleTapGame.Core;
using IdleTapGame.Util;
using UnityEngine;
using UnityEngine.UI;

namespace IdleTapGame.UI
{
    /// <summary>
    /// Binds the on-screen HUD to <see cref="GameManager"/> and refreshes it
    /// every frame. All references are assigned in code by
    /// <see cref="GameBootstrap"/>.
    /// </summary>
    public class HudController : MonoBehaviour
    {
        public Text GoldText;
        public Text PerSecondText;
        public Button TapButton;
        public Button TapUpgradeButton;
        public Text TapUpgradeLabel;

        public GameObject OfflinePanel;
        public Text OfflineText;
        public Button OfflineClaimButton;

        public List<GeneratorRowUI> Rows = new List<GeneratorRowUI>();

        private void Start()
        {
            TapButton.onClick.AddListener(OnTap);
            TapUpgradeButton.onClick.AddListener(OnTapUpgrade);
            OfflineClaimButton.onClick.AddListener(OnClaimOffline);

            ShowOfflineIfPending();
        }

        private void Update()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null) return;

            GoldText.text = BigNumberFormatter.Format(gm.Currency.Gold);
            PerSecondText.text = BigNumberFormatter.Format(gm.GetTotalProductionPerSecond()) + "/s";

            TapUpgradeLabel.text = "Tap moč  Lv " + gm.Tap.Level +
                                   "  (+" + BigNumberFormatter.Format(gm.Tap.TapPower) + "/tap)\n" +
                                   "Nadgradi: " + BigNumberFormatter.Format(gm.Tap.UpgradeCost);
            TapUpgradeButton.interactable = gm.Currency.Gold >= gm.Tap.UpgradeCost;

            for (int i = 0; i < Rows.Count; i++)
                Rows[i].Refresh();
        }

        private void OnTap()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.HandleTap();
        }

        private void OnTapUpgrade()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TryUpgradeTap();
        }

        private void ShowOfflineIfPending()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null || !gm.PendingOffline.HasEarnings)
            {
                OfflinePanel.SetActive(false);
                return;
            }

            int minutes = Mathf.RoundToInt((float)(gm.PendingOffline.SecondsAway / 60d));
            OfflineText.text = "Dobrodošel nazaj!\n\nMedtem ko te ni bilo (~" + minutes +
                               " min) so generatorji zaslužili:\n\n" +
                               BigNumberFormatter.Format(gm.PendingOffline.Earned) + " zlata";
            OfflinePanel.SetActive(true);
        }

        private void OnClaimOffline()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.AcknowledgeOffline();
            OfflinePanel.SetActive(false);
        }
    }
}
