using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gimersia.Feature.Blackbar;
using Gimersia.Feature.Occupancy;
using UnityEngine;
using UnityEngine.Pool;

namespace Gimersia.Gameplay {
    public class CutsceneSystem
    {
        DialogueSystem _system;
        OccupancyManager _occupancy;
        Occupant _currentSpeaker;
        Camera _camera;
        BlackbarManager _blackbar;
        static Vector3 _defaultCameraPos = new Vector3(0, 0, -10);
        public CutsceneSystem(DialogueSystem system, OccupancyManager occupancy, Camera camera, BlackbarManager blackbar)
        {
            _system = system;
            _occupancy = occupancy;
            _camera = camera;
            _blackbar = blackbar;
        }

        public async UniTask PlayIntro(CarAnimation car, string[] commands)
        {
            _blackbar.Show(0);
            _camera.transform.SetParent(car.transform);
            _camera.transform.localPosition = _defaultCameraPos; // ignore z pos

            await car.AnimateIn();


            if (commands != null) await RunDialogueCommand(commands);

        }
        public void EndIntro()
        {
            _blackbar.Hide();
            _camera.transform.SetParent(null);
            _camera.transform.position = _defaultCameraPos; // ignore z pos
        }
        public async UniTask RunDialogueCommand(string[] commands)
        {
            if (commands == null || commands.Length == 0) return;

            using (ListPool<Occupant>.Get(out var occupants))
            {
                _occupancy.GetOccupants(occupants);
                if (occupants.Count == 0)
                {
                    Debug.LogWarning("RunDialogueCommand: No occupants found.");
                    return;
                }

                _currentSpeaker = null;
                _currentSpeaker = GetRandomOccupant(occupants);

                for (int i = 0; i < commands.Length; i++)
                {
                    switch (commands[i])
                    {
                        case "/switch":
                            _currentSpeaker = GetRandomOccupant(occupants);
                            break;
                        default:
                            _system.ShowDialogue(_currentSpeaker.transform, commands[i]);
                            await UniTask.Delay(1000);  // TODO await user input
                            _system.HideDialogue(_currentSpeaker.transform);
                            break;
                    }
                }
            }
        }
        Occupant GetRandomOccupant(List<Occupant> occupants)
        {
            if (occupants.Count == 0) return null;
            if (occupants.Count == 1) return occupants[0];

            Occupant newOccupant = _currentSpeaker;
            while (newOccupant == _currentSpeaker)
            {
                var rand = Random.Range(0, occupants.Count);
                newOccupant = occupants[rand];
            }
            return newOccupant;
        }
    }
}
