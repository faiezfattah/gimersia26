using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using Gimersia.Feature.Occupancy;
using LitMotion;
using LitMotion.Extensions;
using Oneiric.Core.EventBus;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Gimersia.Gameplay
{
    public class HUDManager : MonoBehaviour
    {
        [Inject] LevelSystem _levelSystem;
        [Inject] OccupancyManager _occupancy;
        public CanvasGroup CanvasGroup;
        [Title("Info Card")]
        public RectTransform CardRect;
        public Image ProfileIcon;
        public TextMeshProUGUI ProfileName;
        public TextMeshProUGUI Traits;
        public RectTransform RulesContainer;
        public TextMeshProUGUI RulesItemPrefab;
        [Title("Others")]
        public Button SubmitButton;
        public ProgressBar ProgressBar;
        LevelData _currentData;
        IDisposable[] _bag;

        List<TextMeshProUGUI> _ruleTexts = new ();
        void Start()
        {
            _levelSystem.StartLevel();
            HideCard();
            SubmitButton.onClick.AddListener(OnSubmit);

            _bag = new IDisposable[] {
                EventBus<OccupantMovedEvent>.Bind(evt =>
                {
                    bool wasStage = evt.OldGrid != null && evt.OldGrid.gridType == GridType.Stage;
                    bool isStage = evt.NewGrid != null && evt.NewGrid.gridType == GridType.Stage;

                    if (!wasStage && isStage) ProgressBar.AddLevel();
                    else if (wasStage && !isStage) ProgressBar.RemoveLevel();
                })
            };
        }
        public void Show()
        {
            CanvasGroup.alpha = 1;
            CanvasGroup.interactable = true;
            CanvasGroup.blocksRaycasts = true;
        }
        public void Hide()
        {
            CanvasGroup.alpha = 0;
            CanvasGroup.interactable = false;
            CanvasGroup.blocksRaycasts = false;
        }

        void OnDestroy()
        {
            SubmitButton.onClick.RemoveListener(OnSubmit);
            foreach (var item in _bag) item?.Dispose();
        }
        void OnSubmit()
        {
            _levelSystem.Submit();
        }

        public void UpdateCard(Entity entity)
        {
            foreach (var text in _ruleTexts)
            {
                Destroy(text.gameObject);
            }
            _ruleTexts.Clear();

            ProfileIcon.sprite = entity.Icon;
            ProfileIcon.color = new Color(1, 1, 1, 1);
            Traits.text = string.Join("\n", entity.occupant.Traits.ToString().Split(','));
            ProfileName.text = entity.name;

            foreach (var rule in entity.occupant.Rules)
            {
                var text = Instantiate(RulesItemPrefab, RulesContainer);
                _ruleTexts.Add(text);
                text.text = rule.WantText;
            }

            LMotion
                .Create(-100f, 100f, 0.3f)
                .WithEase(Ease.OutBack)
                .BindToAnchoredPosition3DY(CardRect);
        }
        public void HideCard()
        {
            LMotion
                .Create(100f, -100f, 0.3f)
                .WithEase(Ease.OutCubic)
                .BindToAnchoredPosition3DY(CardRect);

            ProfileIcon.sprite = null;
            ProfileIcon.color = new Color(1, 1, 1, 0);
            Traits.text = "";

            foreach (var text in _ruleTexts)
            {
                Destroy(text.gameObject);
            }

            _ruleTexts.Clear();
        }
        public void UpdateLevel(LevelData data)
        {
            ProgressBar.SetTotalStep(data.TargetAnimalCount);
        }
    }
}
