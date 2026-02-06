using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Splash
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] private Image dot;
        [SerializeField] private Image frame;
        [SerializeField] private Image[] leftHearts;
        [SerializeField] private Vector2[] leftHeartsPositions;
        [SerializeField] private Image[] rightHearts;
        [SerializeField] private Vector2[] rightHeartsPositions;
        [SerializeField] private Image logo;
        [SerializeField] private RectTransform tagsMask;
        
        [Header("CloseAnimation")]
        [SerializeField] private RectTransform roll;
        [SerializeField] private RectTransform mask;
        
        private Sequence _splashSequence;
        private Sequence _leftHearthSequence;
        private Sequence _rightHearthSequence;
        private Sequence _closingSequence;
        private readonly Color _offColor = new Color(1, 1, 1,  0);
        private readonly Vector2 _tagsMaskOffSizeDelta = new Vector2(1f, 137f);
        private readonly Vector2 _tagsMaskOnSizeDelta = new Vector2(632f, 137f);

        private void OnEnable()
        {
            ResetVisual();
            AnimateSplash();
        }

        private void OnDisable()
        {
            if (_splashSequence.isAlive)
                _splashSequence.Stop();
            if (_leftHearthSequence.isAlive)
                _leftHearthSequence.Stop();
            if (_rightHearthSequence.isAlive)
                _rightHearthSequence.Stop();
            if (_closingSequence.isAlive)
                _closingSequence.Stop();
        }

        private void ResetVisual()
        {
            dot.color = _offColor;
            frame.color = _offColor;
            logo.color = _offColor;
            tagsMask.sizeDelta = _tagsMaskOffSizeDelta;
            foreach (var leftHeart in leftHearts)
            {
                leftHeart.color = _offColor;
            }
            foreach (var rightHeart in rightHearts)
            {
                rightHeart.color = _offColor;
            }

            roll.anchoredPosition = new Vector2(0, -2980f);
            roll.gameObject.SetActive(false);
            mask.sizeDelta = new Vector2(mask.sizeDelta.x, 3040f);
        }

        private void AnimateSplash()
        {
            _splashSequence = Sequence.Create();
            _splashSequence.Group(Tween.Delay(0.5f));

            AnimateDot();
            AnimateFrame();
            AnimateLogo();

            _splashSequence.ChainDelay(2f);
            _splashSequence.OnComplete(AnimateClosing);
        }

        private void AnimateDot()
        {
            _splashSequence.Chain(
                Tween.Scale(
                    dot.rectTransform,
                    Vector3.one * 3f,
                    Vector3.one,
                    0.5f,
                    Ease.OutCirc
                )
            );
            _splashSequence.Group(
                Tween.Alpha(
                    dot,
                    0f,
                    1f,
                    0.5f,
                    Ease.OutCirc
                )
            );
        }

        private void AnimateFrame()
        {
            _splashSequence.Chain(
                Tween.LocalRotation(
                    frame.rectTransform,
                    new Vector3(0, 0, -10f),
                    new Vector3(0, 0, 10f),
                    0.6f,
                    Ease.InOutSine
                ).OnComplete(AnimateRightHearts)
            );
            _splashSequence.Group(
                Tween.Alpha(
                    frame,
                    0f,
                    1f,
                    0.3f,
                    Ease.OutCirc
                )
            );
            _splashSequence.Chain(
                Tween.LocalRotation(
                    frame.rectTransform,
                    new Vector3(0, 0, 10f),
                    new Vector3(0, 0, -5f),
                    0.6f,
                    Ease.InOutSine
                ).OnComplete(AnimateLeftHearts)
            );
            _splashSequence.Chain(
                Tween.LocalRotation(
                    frame.rectTransform,
                    new Vector3(0, 0, -5f),
                    new Vector3(0, 0, 0f),
                    0.6f,
                    Ease.InOutSine
                )
            );
        }

        private void AnimateRightHearts()
        {
            _rightHearthSequence = Sequence.Create();

            for (var i = 0; i < rightHearts.Length; i++)
            {
                _rightHearthSequence.Group(
                    Tween.Alpha(
                        rightHearts[i],
                        0f,
                        1f,
                        0.2f,
                        Ease.OutCirc
                    )
                );

                _rightHearthSequence.Group(
                    Tween.UIAnchoredPosition(
                        rightHearts[i].rectTransform,
                        Vector2.zero,
                        rightHeartsPositions[i],
                        3f,
                        Ease.OutCubic
                    )
                );
            }
        }

        private void AnimateLeftHearts()
        {
            _leftHearthSequence = Sequence.Create();

            for (var i = 0; i < leftHearts.Length; i++)
            {
                _leftHearthSequence.Group(
                    Tween.Alpha(
                        leftHearts[i],
                        0f,
                        1f,
                        0.2f,
                        Ease.OutCirc
                    )
                );

                _leftHearthSequence.Group(
                    Tween.UIAnchoredPosition(
                        leftHearts[i].rectTransform,
                        Vector2.zero,
                        leftHeartsPositions[i],
                        3f,
                        Ease.OutCubic
                    )
                );
            }
        }
        
        private void AnimateLogo()
        {
            _splashSequence.Chain(
                Tween.Alpha(
                    logo,
                    0f,
                    1f,
                    0.5f,
                    Ease.OutCirc
                )
            );
            _splashSequence.Group(
                Tween.Scale(
                    logo.rectTransform,
                    Vector3.one * 0.8f,
                    Vector3.one,
                    0.5f,
                    Ease.OutCirc
                )
            );
            _splashSequence.Chain(
                Tween.UISizeDelta(
                    tagsMask,
                    _tagsMaskOffSizeDelta,
                    _tagsMaskOnSizeDelta,
                    0.5f,
                    Ease.OutCirc
                )
            );
        }

        private void AnimateClosing()
        {
            roll.gameObject.SetActive(true);
            _closingSequence = Sequence.Create();
            _closingSequence.Group(
                Tween.UIAnchoredPosition(
                    roll,
                    new Vector2(0f, roll.rect.height),
                    1f,
                    Ease.InOutSine
                )
            );
            _closingSequence.Group(
                Tween.UISizeDelta(
                    mask,
                    new Vector2(mask.sizeDelta.x, 1f),
                    1f,
                    Ease.InOutSine
                )
            );
            _closingSequence.OnComplete(() => gameObject.SetActive(false));
        }
    }
}