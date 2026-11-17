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
        public TextMeshProUGUI Rules;

        public Button SubmitButton;

        // we have no visual but we need to test it. just out put it into the ui for now
        public TextMeshProUGUI Log;

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
            var res = _levelSystem.EvaluateLevel();
            if (res) _levelSystem.NextLevel();
            
        }

        public void UpdateCard(Entity data)
        {
            ProfileIcon.sprite = data.Icon;
            Traits.text = data.occupant.Data.Traits.ToString();
            Rules.text = string.Join("\n", data.occupant.Rules);
        }

        void UpdateLevel() // need level info
        {

        }
    }
}