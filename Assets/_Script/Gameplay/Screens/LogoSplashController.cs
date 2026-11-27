using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gimersia.Gameplay.Screen
{
    public class LogoSplashController : MonoBehaviour
    {
        [SerializeField] private GameObject[] logos;
        [SerializeField] private float firstLogoPause = 0.5f;
        [SerializeField] private float fadeDuration = 0.2f;
        [SerializeField] private float slowLogoHold = 1.1f;
        [SerializeField] private float fastLogoHold = 0.25f;
        [SerializeField] private int fastCycles = 2;
        [SerializeField, Range(0.5f, 1f)] private float finalLogoScale = 0.6f;
        [SerializeField] private string nextSceneName = "MainMenu";

        private CanvasGroup[] _canvasGroups;
        private Vector3[] _baseLogoScales;

        private void Start()
        {
            if (logos == null || logos.Length == 0)
            {
                Debug.LogError("LogoSplashController requires at least one logo GameObject.", this);
                enabled = false;
                return;
            }

            _canvasGroups = new CanvasGroup[logos.Length];
            _baseLogoScales = new Vector3[logos.Length];
            for (int i = 0; i < logos.Length; i++)
            {
                if (logos[i] == null)
                {
                    Debug.LogError("LogoSplashController has an unassigned logo GameObject.", this);
                    enabled = false;
                    return;
                }

                _baseLogoScales[i] = logos[i].transform.localScale;
                _canvasGroups[i] = logos[i].GetComponent<CanvasGroup>();
                if (_canvasGroups[i] == null)
                {
                    Debug.LogError($"Logo '{logos[i].name}' needs a CanvasGroup component.", logos[i]);
                    enabled = false;
                    return;
                }

                _canvasGroups[i].alpha = 0f;
                _canvasGroups[i].interactable = false;
                _canvasGroups[i].blocksRaycasts = false;
            }

            PlaySequence(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask PlaySequence(CancellationToken cancellationToken)
        {
            await ShowLogo(0, true, cancellationToken);
            await Wait(firstLogoPause, cancellationToken);

            float shrinkDuration = slowLogoHold * (logos.Length - 1)
                + fastLogoHold * fastCycles * logos.Length;
            UniTask shrinking = ShrinkLogos(shrinkDuration, cancellationToken);

            for (int i = 1; i < logos.Length; i++)
            {
                await ShowLogo(i, false, cancellationToken);
                await Wait(slowLogoHold, cancellationToken);
            }

            for (int cycle = 0; cycle < fastCycles; cycle++)
            {
                for (int i = 0; i < logos.Length; i++)
                {
                    await ShowLogo(i, false, cancellationToken);
                    await Wait(fastLogoHold, cancellationToken);
                }
            }

            await shrinking;
            SceneManager.LoadScene(nextSceneName);
        }

        private async UniTask ShowLogo(int index, bool fadeIn, CancellationToken cancellationToken)
        {
            for (int i = 0; i < logos.Length; i++)
            {
                _canvasGroups[i].alpha = 0f;
            }

            CanvasGroup canvasGroup = _canvasGroups[index];
            if (!fadeIn)
            {
                canvasGroup.alpha = 1f;
                return;
            }

            await LMotion.Create(0f, 1f, fadeDuration)
                .WithEase(Ease.OutQuad)
                .BindToAlpha(canvasGroup)
                .ToUniTask(cancellationToken);

            canvasGroup.alpha = 1f;
        }

        private async UniTask ShrinkLogos(float duration, CancellationToken cancellationToken)
        {
            if (duration <= 0f)
            {
                SetLogoScale(finalLogoScale);
                return;
            }

            await LMotion.Create(1f, finalLogoScale, duration)
                .WithEase(Ease.Linear)
                .WithScheduler(MotionScheduler.UpdateRealtime)
                .Bind(SetLogoScale)
                .ToUniTask(cancellationToken);
        }

        private void SetLogoScale(float multiplier)
        {
            for (int i = 0; i < logos.Length; i++)
            {
                logos[i].transform.localScale = _baseLogoScales[i] * multiplier;
            }
        }

        private static UniTask Wait(float duration, CancellationToken cancellationToken)
        {
            return UniTask.Delay(
                TimeSpan.FromSeconds(duration),
                ignoreTimeScale: true,
                cancellationToken: cancellationToken);
        }
    }
}
