using System;
using Alchemy.Inspector;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class JitteringAnimation : MonoBehaviour
{
    public Sprite[] Sprites;
    public Image Image;
    public float Duration = 1f;
    public bool PlayOnAwake = true;
    bool _isPlaying;
    void Awake()
    {
        if (PlayOnAwake) Play().Forget();
    }
    
    [Button]
    public async UniTaskVoid Play()
    {
        _isPlaying = true;
        while (_isPlaying)
        {
            for (int i = 0; i < Sprites.Length; i++)
            {
                Image.sprite = Sprites[i];
                await UniTask.Delay(TimeSpan.FromSeconds(Duration / Sprites.Length));
            }
        }
    }

    [Button]
    public void Stop()
    {
        _isPlaying = false;
    }
}
