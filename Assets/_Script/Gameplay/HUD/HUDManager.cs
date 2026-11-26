using System.Collections.Generic;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gimersia.Gameplay
{
    public class HUDManager : MonoBehaviour
    {
        [Inject] LevelSystem _levelSystem;
        public Image ProfileIcon;
        public TextMeshProUGUI Traits;
        public RectTransform RulesContainer;
        public TextMeshProUGUI RulesItemPrefab;

        public Button SubmitButton;
        List<TextMeshProUGUI> _ruleTexts = new ();
        void Start()
        {
            _levelSystem.StartLevel();
            SubmitButton.onClick.AddListener(OnSubmit);
        }
        void OnDestroy()
        {
            SubmitButton.onClick.RemoveListener(OnSubmit);
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

        void UpdateLevel() // need level info
        {

        }
    }
}