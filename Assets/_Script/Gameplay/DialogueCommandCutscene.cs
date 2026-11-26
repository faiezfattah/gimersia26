using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Gimersia.Feature.Occupancy;
using UnityEngine;
using UnityEngine.Pool;

namespace Gimersia.Gameplay {
    public class DialogueCommandCutscene
    {
        DialogueSystem _system;
        OccupancyManager _occupancy;
        Occupant _currentSpeaker;
        public DialogueCommandCutscene(DialogueSystem system, OccupancyManager occupancy)
        {
            _system = system;
            _occupancy = occupancy;
        }
        public async UniTaskVoid RunDialogueCommand(string[] commands)
        {
            using (ListPool<Occupant>.Get(out var occupants))
            {
                _occupancy.GetOccupants(occupants);
                _currentSpeaker = GetRandomOccupant(occupants);

                for (int i = 0; i < commands.Length; i++)
                {
                    switch (commands[i])
                    {
                        case "/switch":
                            _currentSpeaker = GetRandomOccupant(occupants);
                            
                            i += 1;
                            continue;
                        default:
                            _system.ShowDialogue(_currentSpeaker.transform, commands[i]);
                            await UniTask.Delay(1000);
                            _system.HideDialogue(_currentSpeaker.transform);
                            break;
                    }
                }
            }
        }
        Occupant GetRandomOccupant(List<Occupant> occupants)
        {
            Occupant newOccupant = _currentSpeaker;
            while (newOccupant == _currentSpeaker)
            {
                var rand = Random.Range(0, occupants.Count - 1);
                newOccupant = occupants[rand];
            }
            return newOccupant;
        }
    }
}