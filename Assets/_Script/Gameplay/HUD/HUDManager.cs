using System;
using System.Collections.Generic;
using Gimersia.Feature.Occupancy;
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
        public Image ProfileIcon;
        public TextMeshProUGUI Traits;
        public RectTransform RulesContainer;
        public TextMeshProUGUI RulesItemPrefab;
        public Button SubmitButton;
        public ProgressBar ProgressBar;
        LevelData _currentData;
        IDisposable[] _bag;
        
        List<TextMeshProUGUI> _ruleTexts = new ();
        void Start()
        {
            _levelSystem.StartLevel();
            SubmitButton.onClick.AddListener(OnSubmit);

            _bag = new IDisposable[] {
                EventBus<OccupantMovedEvent>.Bind(evt => {
                    Debug.Log("OccupantMovedEvent");
                    ProgressBar.SetLevel(CountOccupants());
                }),
            };
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

        public void UpdateCard(Entity data)
        {
            foreach (var text in _ruleTexts)
            {
                Destroy(text.gameObject);
            }
            _ruleTexts.Clear();
            
            ProfileIcon.sprite = data.Icon;
            Traits.text = data.occupant.Data.Traits.ToString();

            foreach (var rule in data.occupant.Rules)
            {
                var text = Instantiate(RulesItemPrefab, RulesContainer);
                _ruleTexts.Add(text);
                text.text = rule.WantText;
            }
        }

        public void UpdateLevel(LevelData data) // need level info
        {
            ProgressBar.SetTotalStep(data.TargetAnimalCount);
        }
        public int CountOccupants()
        {
            var stageGrids = _occupancy.GetStageGrid();
            int count = 0;
            using (ListPool<Occupant>.Get(out var occupants))
            {
                foreach (var grid in stageGrids)
                {
                    grid.GetOccupants(occupants);
                }

                count += occupants.Count;
            }
            return count;
        }
    }
}