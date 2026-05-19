using System.Collections.Generic;
using IdleTapGame.Economy;
using IdleTapGame.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IdleTapGame.Core
{
    /// <summary>
    /// Single entry point. Drop one of these into an empty scene (or use the
    /// "IdleTapGame > Create Main Scene" editor menu) and press Play.
    /// Builds the entire HUD in code so there are no prefabs / asset GUIDs.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        private static readonly Color ColBackground = new Color(0.10f, 0.13f, 0.18f, 1f);
        private static readonly Color ColPanel      = new Color(0.16f, 0.20f, 0.27f, 0.95f);
        private static readonly Color ColRow        = new Color(0.20f, 0.25f, 0.33f, 1f);
        private static readonly Color ColAccent     = new Color(0.95f, 0.74f, 0.20f, 1f);
        private static readonly Color ColAccentDark = new Color(0.55f, 0.42f, 0.10f, 1f);
        private static readonly Color ColText       = new Color(0.96f, 0.97f, 0.99f, 1f);
        private static readonly Color ColIncome     = new Color(0.55f, 0.90f, 0.55f, 1f);

        private Font _font;

        private void Awake()
        {
            _font = GetBuiltinFont();

            EnsureGameManager();
            EnsureEventSystem();

            Canvas canvas = CreateCanvas();
            AddImage(FullStretch(NewRect("Background", canvas.transform)), ColBackground);

            RectTransform safe = FullStretch(NewRect("SafeArea", canvas.transform));
            safe.gameObject.AddComponent<SafeAreaFitter>();

            HudController hud = gameObject.AddComponent<HudController>();

            hud.GoldText = MakeText("Gold", safe, 96, TextAnchor.MiddleCenter, ColText);
            Anchor(hud.GoldText.rectTransform, 0f, 1f, 1f, 1f, 0.5f, 1f,
                new Vector2(0f, -30f), new Vector2(-60f, 150f));

            hud.PerSecondText = MakeText("PerSecond", safe, 44, TextAnchor.MiddleCenter, ColIncome);
            Anchor(hud.PerSecondText.rectTransform, 0f, 1f, 1f, 1f, 0.5f, 1f,
                new Vector2(0f, -190f), new Vector2(-60f, 60f));

            hud.TapButton = MakeButton("TapButton", safe, ColAccent, out Text tapLabel, 84);
            tapLabel.text = "TAP";
            Anchor(hud.TapButton.GetComponent<RectTransform>(), 0.5f, 1f, 0.5f, 1f, 0.5f, 1f,
                new Vector2(0f, -310f), new Vector2(520f, 520f));

            hud.TapUpgradeButton = MakeButton("TapUpgrade", safe, ColPanel, out Text upgLabel, 36);
            hud.TapUpgradeLabel = upgLabel;
            Anchor(hud.TapUpgradeButton.GetComponent<RectTransform>(), 0.5f, 1f, 0.5f, 1f, 0.5f, 1f,
                new Vector2(0f, -880f), new Vector2(780f, 150f));

            BuildGeneratorList(safe, hud);
            BuildOfflinePanel(canvas.transform, hud);
        }

        // ---- High level builders -------------------------------------------------

        private void BuildGeneratorList(RectTransform parent, HudController hud)
        {
            RectTransform scrollRT = NewRect("Generators", parent);
            Anchor(scrollRT, 0f, 0f, 1f, 0f, 0.5f, 0f,
                new Vector2(0f, 20f), new Vector2(-40f, 760f));
            AddImage(scrollRT, ColPanel);

            ScrollRect scroll = scrollRT.gameObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.scrollSensitivity = 30f;

            RectTransform viewport = FullStretch(NewRect("Viewport", scrollRT));
            viewport.gameObject.AddComponent<RectMask2D>();

            RectTransform content = NewRect("Content", viewport);
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.offsetMin = Vector2.zero;
            content.offsetMax = Vector2.zero;

            VerticalLayoutGroup vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(12, 12, 12, 12);
            vlg.spacing = 12f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scroll.viewport = viewport;
            scroll.content = content;

            IReadOnlyList<Generator> generators = GameManager.Instance.Generators;
            for (int i = 0; i < generators.Count; i++)
            {
                GeneratorRowUI row = BuildGeneratorRow(content, generators[i]);
                hud.Rows.Add(row);
            }
        }

        private GeneratorRowUI BuildGeneratorRow(RectTransform parent, Generator model)
        {
            RectTransform rowRT = NewRect("Row_" + model.Id, parent);
            AddImage(rowRT, ColRow);

            LayoutElement le = rowRT.gameObject.AddComponent<LayoutElement>();
            le.minHeight = 150f;
            le.preferredHeight = 150f;

            Text title = MakeText("Title", rowRT, 40, TextAnchor.LowerLeft, ColText);
            StretchAnchors(title.rectTransform, 0f, 0.5f, 0.62f, 1f,
                new Vector2(20f, 4f), new Vector2(-8f, -10f));

            Text prod = MakeText("Prod", rowRT, 34, TextAnchor.UpperLeft, ColIncome);
            StretchAnchors(prod.rectTransform, 0f, 0f, 0.62f, 0.5f,
                new Vector2(20f, 10f), new Vector2(-8f, -4f));

            Button buy = MakeButton("Buy", rowRT, ColAccentDark, out Text buyLabel, 32);
            StretchAnchors(buy.GetComponent<RectTransform>(), 0.64f, 0.12f, 1f, 0.88f,
                Vector2.zero, new Vector2(-12f, 0f));

            GeneratorRowUI rowUI = rowRT.gameObject.AddComponent<GeneratorRowUI>();
            rowUI.TitleText = title;
            rowUI.ProductionText = prod;
            rowUI.BuyButton = buy;
            rowUI.BuyLabel = buyLabel;
            rowUI.Bind(model);
            return rowUI;
        }

        private void BuildOfflinePanel(Transform canvas, HudController hud)
        {
            RectTransform dim = FullStretch(NewRect("OfflinePanel", canvas));
            AddImage(dim, new Color(0f, 0f, 0f, 0.7f));

            RectTransform box = NewRect("Box", dim);
            box.anchorMin = new Vector2(0.5f, 0.5f);
            box.anchorMax = new Vector2(0.5f, 0.5f);
            box.pivot = new Vector2(0.5f, 0.5f);
            box.anchoredPosition = Vector2.zero;
            box.sizeDelta = new Vector2(860f, 720f);
            AddImage(box, ColPanel);

            Text txt = MakeText("OfflineText", box, 42, TextAnchor.MiddleCenter, ColText);
            StretchAnchors(txt.rectTransform, 0f, 0.25f, 1f, 1f,
                new Vector2(40f, 0f), new Vector2(-40f, -30f));

            Button claim = MakeButton("Claim", box, ColAccent, out Text claimLabel, 40);
            claimLabel.text = "Prevzemi";
            Anchor(claim.GetComponent<RectTransform>(), 0.5f, 0f, 0.5f, 0f, 0.5f, 0f,
                new Vector2(0f, 40f), new Vector2(420f, 130f));

            hud.OfflinePanel = dim.gameObject;
            hud.OfflineText = txt;
            hud.OfflineClaimButton = claim;
            dim.gameObject.SetActive(false);
        }

        // ---- Infrastructure ------------------------------------------------------

        private static void EnsureGameManager()
        {
            if (GameManager.Instance == null)
                new GameObject("GameManager").AddComponent<GameManager>();
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
                return;

            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        private static Canvas CreateCanvas()
        {
            var go = new GameObject("Canvas");
            Canvas canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            go.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        // ---- UI helpers ----------------------------------------------------------

        private static RectTransform NewRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(parent, false);
            return rt;
        }

        private static RectTransform FullStretch(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            return rt;
        }

        private static void Anchor(RectTransform rt,
            float anchorMinX, float anchorMinY, float anchorMaxX, float anchorMaxY,
            float pivotX, float pivotY, Vector2 anchoredPos, Vector2 size)
        {
            rt.anchorMin = new Vector2(anchorMinX, anchorMinY);
            rt.anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            rt.pivot = new Vector2(pivotX, pivotY);
            rt.sizeDelta = size;
            rt.anchoredPosition = anchoredPos;
        }

        private static void StretchAnchors(RectTransform rt,
            float minX, float minY, float maxX, float maxY,
            Vector2 offsetMin, Vector2 offsetMax)
        {
            rt.anchorMin = new Vector2(minX, minY);
            rt.anchorMax = new Vector2(maxX, maxY);
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
        }

        private static Image AddImage(RectTransform rt, Color color)
        {
            Image img = rt.gameObject.AddComponent<Image>();
            img.color = color;
            return img;
        }

        private Text MakeText(string name, Transform parent, int fontSize,
            TextAnchor anchor, Color color)
        {
            RectTransform rt = NewRect(name, parent);
            Text text = rt.gameObject.AddComponent<Text>();
            text.font = _font;
            text.fontSize = fontSize;
            text.alignment = anchor;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        private Button MakeButton(string name, Transform parent, Color background,
            out Text label, int fontSize)
        {
            RectTransform rt = NewRect(name, parent);
            Image bg = AddImage(rt, background);

            Button button = rt.gameObject.AddComponent<Button>();
            button.targetGraphic = bg;
            ColorBlock cb = button.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(1.1f, 1.1f, 1.1f, 1f);
            cb.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            cb.disabledColor = new Color(0.45f, 0.45f, 0.45f, 0.6f);
            button.colors = cb;

            label = MakeText("Label", rt, fontSize, TextAnchor.MiddleCenter, ColText);
            FullStretch(label.rectTransform);
            return button;
        }

        private static Font GetBuiltinFont()
        {
            Font f = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (f == null)
                f = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return f;
        }
    }
}
