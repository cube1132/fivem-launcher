using IdleTapGame.Core;
using IdleTapGame.Economy;
using IdleTapGame.Util;
using UnityEngine;
using UnityEngine.UI;

namespace IdleTapGame.UI
{
    /// <summary>
    /// One row in the generator list. References are wired up in code by
    /// <see cref="GameBootstrap"/>; no prefab / inspector setup required.
    /// </summary>
    public class GeneratorRowUI : MonoBehaviour
    {
        public Generator Model;
        public Text TitleText;
        public Text ProductionText;
        public Button BuyButton;
        public Text BuyLabel;

        public void Bind(Generator model)
        {
            Model = model;
            BuyButton.onClick.AddListener(OnBuyClicked);
            Refresh();
        }

        private void OnBuyClicked()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TryBuyGenerator(Model);
        }

        public void Refresh()
        {
            if (Model == null) return;

            TitleText.text = Model.DisplayName + "  x" + Model.Owned;
            ProductionText.text = BigNumberFormatter.Format(Model.ProductionPerSecond) + "/s";
            BuyLabel.text = "Kupi\n" + BigNumberFormatter.Format(Model.NextCost);

            bool affordable = GameManager.Instance != null &&
                              GameManager.Instance.Currency.Gold >= Model.NextCost;
            BuyButton.interactable = affordable;
        }
    }
}
