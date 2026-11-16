using Gimersia.Feature.Occupancy;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gimersia.Gameplay
{
    public class HUDManager : MonoBehaviour
    {
        [Inject] OccupancyManager _judgingSystem;
        public Image ProfileIcon;
        public TextMeshProUGUI Traits;
        public TextMeshProUGUI Rules;

        public Button SubmitButton;

        // we have no visual but we need to test it. just out put it into the ui for now
        public TextMeshProUGUI Log;

        void Start()
        {
            SubmitButton.onClick.AddListener(OnSubmit);
        }
        void OnDestroy()
        {
            SubmitButton.onClick.RemoveListener(OnSubmit);
        }
        void OnSubmit()
        {
            var violation = _judgingSystem.Judge();
            Log.text = "Violations:\n";
            foreach (var v in violation)
            {
                Log.text += v.ToString() + "\n";
            }
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