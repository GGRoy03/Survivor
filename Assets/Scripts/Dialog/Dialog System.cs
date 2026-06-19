using System;
using System.Collections;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CanvasGroup     m_TextBubbleGroup;
    [SerializeField] private Image           m_TextBubble;
    [SerializeField] private TextMeshProUGUI m_TextBubbleText;
    [SerializeField] private Image[]         m_CinematicBars;
                     private Camera          m_Camera;

    [Header("Text Display")]
    [SerializeField] private float m_Interval;

    [Header("GUI Animation")]
    [SerializeField] private float           m_CinematicDuration;
    [SerializeField] private DialogAnimation m_CameraAnimation;
    [SerializeField] private DialogAnimation m_BarAnimation;
    [SerializeField] private DialogAnimation m_TextBubbleAnimation;

    private enum DialogState
    {
        DisplayingTextAsTypewriter = 0,
        WaitingForNextSegment      = 1,
        BeginDisplayingNextString  = 2,
        Idle                       = 3,
    }

    private DialogState m_CurrentState = DialogState.Idle;
    private Dialog      m_CurrentDialog;
    private int         m_StringIndexInDialog;
    private int         m_CharIndexInString;
    private int         m_SliceIndexInString;
    private Coroutine   m_CinematicHandle;
    private Coroutine   m_AnimateBubbleColorHandle;
    private Coroutine   m_DisplayTextTypewriterHandle;
    private float       m_CinematicElapsed;

    public bool CanChangeDialogState => m_CinematicHandle == null;

    //
    // NOTE:
    // x) This is terrible, but whatever. Will change next time.
    //

    [System.Serializable]
    private struct DialogAnimation
    {
        public float InDialogValue;
        public float NotInDialogValue;
    }

    private float AnimateDialog(DialogAnimation animation, float progress)
    {
        Debug.Assert(progress >= 0.0f && progress <= 1.0f);

        var result = Mathf.Lerp(animation.NotInDialogValue, animation.InDialogValue, progress);
        return result;
    }

    private void Start()
    {
        m_Camera = Camera.main;
    }

    //
    //
    //

    public float EaseInOutCubic(float t)
    {
        float result;

        if(t < 0.5f)
        {
            result = 4.0f * t * t * t;
        }
        else
        {
            result = 1.0f - Mathf.Pow(-2.0f * t + 2.0f, 3.0f) / 2.0f;
        }

        result = Mathf.Clamp(result, 0.0f, 1.0f);

        return result;
    }



    //
    //
    //

    public void EnterDialog(Dialog dialog)
    {
        if (m_CinematicHandle == null && dialog != null)
        {
            m_CurrentState    = DialogState.BeginDisplayingNextString;
            m_CurrentDialog   = dialog;
            m_CinematicHandle = StartCoroutine(AnimateDialogCinematic(1.0f));
        }
    }

    //
    // NOTE:
    // x) There's a weird bug which I am unable to fix where sometimes trying to skip the slices
    //    as we are displaying the text normally completely breaks the layout.
    //

    public bool UpdateDialog(bool skipTextSegment)
    {
        switch(m_CurrentState)
        {
            case DialogState.DisplayingTextAsTypewriter:
                {
                    var currentString = m_CurrentDialog.m_Strings[m_StringIndexInDialog];

                    if(skipTextSegment)
                    {
                        //
                        // As we are displaying the text, if the player is trying to skip
                        // ahread, we first start by stopping the normal display of text.
                        //

                        if(m_DisplayTextTypewriterHandle != null)
                        {
                            StopCoroutine(m_DisplayTextTypewriterHandle);
                            m_DisplayTextTypewriterHandle = null;
                        }

                       //
                       // We then display all of the remaining slices instantly to the player.
                       //

                        var currentSlice     = currentString.Slices[m_SliceIndexInString];
                        int runningCharIndex = 0;
                        foreach (var slice in currentString.Slices)
                        {
                            SetCharacterColor(runningCharIndex, slice.Content.Length, m_TextBubbleText, slice.TextColor);
                            runningCharIndex += currentSlice.Content.Length;

                        }
                        m_SliceIndexInString = currentString.Slices.Length;

                        //
                        // Finally update the state.
                        //

                        m_CurrentState = DialogState.WaitingForNextSegment;
                    }
                    else if (m_SliceIndexInString < currentString.Slices.Length)
                    {
                        if(m_DisplayTextTypewriterHandle == null)
                        {
                            m_DisplayTextTypewriterHandle = StartCoroutine(DisplayTextTypewriter(m_CurrentDialog.m_Strings[m_StringIndexInDialog]));
                        }
                    }

                    break;
                }

            case DialogState.BeginDisplayingNextString:
                {
                    Debug.Assert(m_CharIndexInString == 0);

                    //
                    // TODO:
                    // x) I think we need to clear the text bubble as well.
                    //

                    string fullString = "";
                    var    slices     = m_CurrentDialog.m_Strings[m_StringIndexInDialog].Slices;
                    foreach (var slice in slices)
                    {
                        fullString += slice.Content;
                    }
                    m_TextBubbleText.text = fullString;

                    //
                    // We also need to start animating the bubble color
                    // towards what the string requests.
                    //

                    if (m_AnimateBubbleColorHandle != null)
                    {
                        StopCoroutine(m_AnimateBubbleColorHandle);
                    }
                    Color prevColor = m_TextBubble.color;
                    Color nextColor = m_CurrentDialog.m_Strings[m_StringIndexInDialog].BackgroundColor;
                    m_AnimateBubbleColorHandle = StartCoroutine(AnimateTextBubbleColor(prevColor, nextColor, 2.0f));

                    //
                    // We can then start displaying the text normallly
                    //

                    m_CurrentState = DialogState.DisplayingTextAsTypewriter;

                    break;
                }

            case DialogState.WaitingForNextSegment:
                {
                    //
                    // When we're done displaying one of the string contained in the dialog, we
                    // wait for the player input before displaying the next string.
                    //

                    if(skipTextSegment)
                    {
                        if(m_StringIndexInDialog + 1 >= m_CurrentDialog.m_Strings.Length)
                        {
                            if (m_CinematicHandle != null)
                            {
                                StopCoroutine(m_CinematicHandle);
                            }
                            m_CinematicHandle = StartCoroutine(AnimateDialogCinematic(-1.0f));

                            m_CurrentState        = DialogState.Idle;
                            m_CharIndexInString   = 0;
                            m_SliceIndexInString  = 0;
                            m_StringIndexInDialog = 0;

                        }
                        else
                        {
                            m_StringIndexInDialog += 1;
                            m_SliceIndexInString   = 0;
                            m_CharIndexInString    = 0;
                            m_CurrentState         = DialogState.BeginDisplayingNextString;
                        }
                    }
                    break;
                }

            case DialogState.Idle:
                {
                    break;
                }
        }

        return m_CurrentState == DialogState.Idle;
    }

    private IEnumerator AnimateDialogCinematic(float timeFactor)
    {
        while (m_CinematicElapsed >= 0.0f && m_CinematicElapsed <= m_CinematicDuration)
        {
            m_CinematicElapsed += timeFactor * Time.deltaTime;

            float progress = EaseInOutCubic(m_CinematicElapsed / m_CinematicDuration);

            foreach (var cinematicBar in m_CinematicBars)
            {
                var   barRect = cinematicBar.GetComponent<RectTransform>();
                float barSize = AnimateDialog(m_BarAnimation, progress);
                barRect.sizeDelta = new(barRect.sizeDelta.x, barSize);
            }

            m_TextBubbleGroup.alpha   = AnimateDialog(m_TextBubbleAnimation, progress);
            m_Camera.orthographicSize = AnimateDialog(m_CameraAnimation, progress);

            yield return null;
        }

        m_CinematicElapsed = Mathf.Clamp(m_CinematicElapsed, 0.0f, m_CinematicDuration);
        m_CinematicHandle  = null;
    }
    
    private void SetCharacterColor(int start, int count, TextMeshProUGUI textMesh, Color color)
    {
        if(textMesh != null && start >= 0)
        {
            int charIdx  = start;
            var textInfo = textMesh.textInfo;
            while((charIdx < start + count) && charIdx < textInfo.characterCount)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[charIdx];
                if (charInfo.isVisible)
                {
                    int materialIdx  = charInfo.materialReferenceIndex;
                    var vertexColors = textInfo.meshInfo[materialIdx].colors32;

                    int vertexIdx = charInfo.vertexIndex;
                    vertexColors[vertexIdx + 0] = color;
                    vertexColors[vertexIdx + 1] = color;
                    vertexColors[vertexIdx + 2] = color;
                    vertexColors[vertexIdx + 3] = color;

                    textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                }

                ++charIdx;
            }
        }
    }

    private IEnumerator AnimateTextBubbleColor(Color prevColor, Color nextColor, float duration)
    {
        float elapsed = 0.0f;
        while(elapsed < duration)
        {
            float progress = Mathf.Clamp(elapsed / duration, 0.0f, 1.0f);

            m_TextBubble.color = Color.Lerp(prevColor, nextColor, progress);
            elapsed           += Time.deltaTime;

            yield return null;
        }

        m_TextBubble.color = nextColor;
    }

    private IEnumerator DisplayTextTypewriter(Dialog.DialogString currentString)
    {
        int charIndexInSlice = 0;

        while(m_SliceIndexInString < currentString.Slices.Length)
        {
            var currentSlice = currentString.Slices[m_SliceIndexInString];

            SetCharacterColor(m_CharIndexInString, 1, m_TextBubbleText, currentSlice.TextColor);
            m_CharIndexInString += 1;
            charIndexInSlice    += 1;

            if (charIndexInSlice >= currentSlice.Content.Length)
            {
                if (currentSlice.TransitionTime > 0.0f)
                {
                    yield return new WaitForSeconds(currentSlice.TransitionTime);
                }

                charIndexInSlice      = 0;
                m_SliceIndexInString += 1;
            }
            
            yield return new WaitForSeconds(m_Interval);
        }

        m_DisplayTextTypewriterHandle = null;
        m_CurrentState                = DialogState.WaitingForNextSegment;
    }
}
