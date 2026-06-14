using System.Collections;
using TMPro;
using UnityEngine;

using static Dialog;

public class TypeWriter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TextMesh      = null;

    [Header("Visual Effect")]
    [SerializeField] private float m_Interval;
    [SerializeField] private char  m_Cursor;

    [Header("Debug")]
    [SerializeField] private Dialog m_Dialog;

    //
    // Prototyping the text effects.
    //

    private void SetCharacterColor(int charIdx, TextMeshProUGUI textMesh, Color color)
    {
        TMP_TextInfo textInfo = textMesh.textInfo;
        if(charIdx >= 0 && charIdx < textInfo.characterCount)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[charIdx];
            if(charInfo.isVisible)
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
        }
    }


    //
    //
    //

    void Start()
    {
        StartDialog();
    }

public void StartDialog()
    {
        Dialog dialog = m_Dialog;

        StartCoroutine(DisplayDialog(dialog));

    }

    public void ForceShowDialogOrGoToNextOne()
    {
        //
        // Has to somehow hint at the coroutine what to set as the visible character count.
        // Maybe we just give up the coroutine idea. Force everything to go through something
        // like: Update Current Dialog or something? Or UpdateDialogContext? Which the player
        // always calls? Maybe just put a boolean on the class and call it a day.
        //
    }

    //
    // TODO:
    // x) Actual API
    // x) Text Color
    // x) Spacing between slices? Or just manual
    // x) Text Bubble
    // x) Text Bubble Color (Lerp)
    // x) Cleanups.
    //

    private IEnumerator DisplayDialog(Dialog dialog)
    {
        int   stringIndexInDialog = 0;
        int   sliceIndexInString  = 0;
        int   charIndexInString   = 0;
        int   charIndexInSlice    = 0;
        Color currentTextColor    =  Color.white;

        while (stringIndexInDialog < dialog.m_Strings.Length)
        {
            if(charIndexInString == 0)
            {
                //
                // As we first encounter the dialog string, we do two things:
                // 1) Inject the full string into the text mesh context.
                // 2) Set the color on every character to something we can't see. This allows us
                // to do the type-writter effect.
                //
                // TODO:
                // x) So this sort of an issue. As we inject the full string into the box it
                //    tries to fit the content. We might have to compute the text box size depending
                //    on the amount of visible characters.
                //

                string                 fullString = "";
                DialogCharacterSlice[] slices     = dialog.m_Strings[stringIndexInDialog].Slices;
                foreach(var slice in slices)
                {
                    fullString += slice.Content;
                }
                m_TextMesh.text = fullString;

                for (int charIdx = 0; charIdx < m_TextMesh.textInfo.characterCount; ++charIdx)
                {
                    SetCharacterColor(charIdx, m_TextMesh, Color.black);
                }

                //
                // This is important, otherwise the internal text-mesh data doesn't seem to be
                // updated, hence we never set the first character's color.
                //

                yield return null;
            }

            var currentString = dialog.m_Strings[stringIndexInDialog];
            var currentSlice  = currentString.Slices[sliceIndexInString];
            if(charIndexInSlice == 0)
            {
                currentTextColor = currentSlice.TextColor;
            }

            //
            // Maybe we want to skip invisible characters entirely? Though, I don't think it
            // looks good, because the cursor appears to go faster than normal?
            // Here we simply set the current character's color. We also have to both increment
            // the current slice index and the string wide index. Note that we rely on them both
            // for initialization logic.
            //

            SetCharacterColor(charIndexInString, m_TextMesh, currentTextColor);

            charIndexInSlice  += 1;
            charIndexInString += 1;

            //
            // If we ran out of the string we do two things:
            // 1) If the string has a transition time before the next one (... for example), then we wait on it
            //    before continuing.
            // 2) We reset the char index in slice to 0, since we are (maybe) going to process the next slice
            //    and increment the slice index into the current string for the same reason.
            //

            if(charIndexInSlice == currentSlice.Content.Length)
            {
                if (currentSlice.TransitionTime > 0.0f)
                {
                    yield return new WaitForSeconds(dialog.m_Strings[stringIndexInDialog].Slices[sliceIndexInString].TransitionTime);
                }

                charIndexInSlice    = 0;
                sliceIndexInString += 1;
            }

            //
            // If we reach the end of the slices, this means we are ready to process the next string.
            // We reset the slice state and increment the string index.
            //

            if(sliceIndexInString == currentString.Slices.Length)
            {
                sliceIndexInString   = 0;
                charIndexInString    = 0;
                stringIndexInDialog += 1;
            }

            //
            // This is the core of the typewritter effect, every time we get to here, we wait
            // for the specified interval.
            //

            yield return new WaitForSeconds(m_Interval);
        }
    }
}
