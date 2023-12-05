using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Febucci.UI;
using Lean.Pool;
using MMTM;
using UnityEngine;
using Random = UnityEngine.Random;

public enum FontSize
{
    Tiny = 10,
    Small = 30,
    Middle = 60,
    Large = 90
}

public static class TextAppend
{
    // public static void ShowHeadText(string text,FontSize fontSize = FontSize.Small,float surtime = 1,Color color = default,Vector2 transfompos = default)
    // {
    //     TextShow.Show(text, FontSize.Small, surtime, color,MyTools.);
    // }
    public static void ShowHeadText(this GameObject self,string text,FontSize fontSize = FontSize.Small,float surtime = 1,Color color = default,Vector2 rectTransformPosition = default)
    {
        TextShow.Show(self.GetInstanceID(),text, FontSize.Small, surtime, color,rectTransformPosition);
    }
}

public class TextShow : MonoBehaviour
{
    public static TextShow instance;
    [SerializeField]
    private TextAnimator_TMP _textAnimatorTMP;

    private RectTransform rectTransform;
    [SerializeField]
    private string defaultText;

    private CancellationTokenSource tocken;
    public static void Show(int id_,string text,FontSize fontSize = FontSize.Small,float surtime = 1,Color color = default,Vector2 rectTransformPosition = default) => instance.Showtext(id_,text,fontSize,surtime,color,rectTransformPosition);
    public static void Show(string text,FontSize fontSize = FontSize.Small,float surtime = 1,Color color = default,Vector2 rectTransformPosition = default) => instance.Showtext(Random.Range(-10000,10000),text,fontSize,surtime,color,rectTransformPosition);
    private void Awake()
    {
        instance = this;
        alltext = new List<string>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Show(0,defaultText);
    }
    private List<string> alltext;

    public void Showtext(int id,string text,FontSize fontSize = FontSize.Small,float surtime = 1,Color color = default,Vector2 rectTransformPosition = default)
    {
        if(alltext.Contains(text + id))return;
        alltext.Add(text + id);
        if(color == default)color = Color.white;

        var textObj = LeanPool.Spawn(_textAnimatorTMP.gameObject,transform);
        PrefabCenter.Instance.RegisterPrefab(textObj);
        var text_TMP = PrefabCenter.Instance.GetComponent<TextAnimator_TMP>(textObj.GetInstanceID());

        text_TMP.TMProComponent.fontSize = (float)fontSize;
        text_TMP.TMProComponent.text =  $"<swing>{text}</swing>";
        text_TMP.TMProComponent.color = color;
        if (rectTransformPosition == default) rectTransformPosition = rectTransform.position;
        text_TMP.TMProComponent.rectTransform.position = rectTransformPosition;

        CloseWord(text_TMP,text + id,surtime).Forget();
    }

    async UniTaskVoid CloseWord(TextAnimator_TMP text_TMP,string text,float time)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(time));
        text_TMP.SetVisibilityEntireText(false);
        alltext.Remove(text);
        LeanPool.Despawn(text_TMP,1);
    }
}
