using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;

public class DialogueBubble : MonoBehaviour
{
    public TextMeshProUGUI TextBox;
    public float duration = 1f;
    
    CancellationTokenSource _cts;
    public void Show(string text)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        
        TextBox.text = text;
        LMotion
            .Create(0, text.Length, duration)
            .BindToMaxVisibleCharacters(TextBox)
            .ToUniTask(_cts.Token);

    }
    public void Hide()
    {
        _cts?.Cancel();
        TextBox.maxVisibleCharacters = 0;
    }
}
