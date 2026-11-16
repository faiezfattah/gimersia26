using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DialogueSystem : MonoBehaviour
{
    public float yoffset = 1f;
    public Canvas Canvas;
    public DialogueBubble DialogueBubble;

    Dictionary<Transform, DialogueBubble> _dialogueBubbles = new();
    
    ObjectPool<DialogueBubble> _pool;
    Camera _cam;

    void Awake()
    {
        _cam = Camera.main;
        _pool = new ObjectPool<DialogueBubble>(
            createFunc: () => Instantiate(DialogueBubble),
            actionOnGet: static b => b.gameObject.SetActive(true),
            actionOnRelease: static b => b.gameObject.SetActive(false),
            actionOnDestroy: static b => Destroy(b.gameObject),
            collectionCheck: true,   // editor-only check for double release
            defaultCapacity: 3,
            maxSize: 100
        );
    }
    public void ShowDialogue(Transform target, string text)
    {
        if (!_dialogueBubbles.TryGetValue(target, out var bubble))
        {
            bubble = _pool.Get();
            bubble.transform.SetParent(Canvas.transform, false);
            _dialogueBubbles[target] = bubble;
        }
        var screenPosition = _cam.WorldToScreenPoint(target.position);
        var canvasRect = (RectTransform)Canvas.transform;
        RectTransformUtility
            .ScreenPointToLocalPointInRectangle(canvasRect, screenPosition, _cam, out var canvasPosition);
        
        ((RectTransform)bubble.transform).anchoredPosition = canvasPosition + new Vector2(0, yoffset);

        bubble.Show(text);
    }
    public void HideDialogue(Transform target)
    {
        if (!_dialogueBubbles.TryGetValue(target, out var bubble))
        {
            Debug.LogWarning($"No dialogue bubble found for target {target.name}");
            return;
        }
        
        _dialogueBubbles.Remove(target);
        _pool.Release(bubble);
    }
}
