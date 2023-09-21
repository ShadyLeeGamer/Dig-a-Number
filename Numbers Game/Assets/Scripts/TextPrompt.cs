using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TextPrompt : MonoBehaviour
{
    TextMeshProUGUI textPromptTxt;
    [SerializeField] string[] prompts;
    public enum Prompts { TapOnPlane, SelectOption, CorrectOption, IncorrectOption }

    [SerializeField] float animTime;
    [SerializeField] float endPosYOffset;
    Vector3 startPos, endPos;

    bool visible;

    private void Awake()
    {
        textPromptTxt = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        startPos = transform.localPosition;
        endPos = startPos + (Vector3.up * endPosYOffset);
    }

    public async Task Set(Prompts prompt)
    {
        // Exit?
        if (visible)
        {
            await AnimateTextPromptTransition(endPos, startPos);
        }

        // Enter
        textPromptTxt.text = prompts[(int)prompt];
        await AnimateTextPromptTransition(startPos, endPos);
    }

    async Task AnimateTextPromptTransition(Vector3 start, Vector3 end)
    {
        float percent = 0f;
        while (percent < 1f)
        {
            percent += 1f / animTime * Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, end, percent);

            await Task.Yield();
        }

        visible = true;
    }
}