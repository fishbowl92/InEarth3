using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    public static string nextScene;
    //[SerializeField]
    //SpriteRenderer progressBar;
    //public Animator loadingAnim;
    public TextMeshProUGUI Tip;
    public string[] Tips;
    public Animator anim;
    public RuntimeAnimatorController[] anims;
    private void Start()
    {
        Time.timeScale = 1;
        Tip.text = Tips[UnityEngine.Random.Range(0, Tips.Length)];
        anim.runtimeAnimatorController = anims[Random.Range(0, anims.Length)];
        //GetComponent<Utilleti>().setSprite();
        StartCoroutine(LoadScene());
    }

    //string nextSceneName;
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        //BGMManager.instans.SetBGM("LoadingScene", 1);
        SceneManager.LoadScene("LoadingScene");
    }
    IEnumerator LoadScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        yield return null;

        //float timer = 0.0f;
        while (!op.isDone && op.progress < 0.9f)
        {
            yield return null;
        }
        op.allowSceneActivation = true;

        /*while (progressBar.color.a > 0.05f)
        {
            yield return null;
            if (op.progress >= 0.85f)
            {
                curtain.a = Mathf.Lerp(curtain.a, 0f, Time.deltaTime);
                progressBar.color = curtain;  
            }
            else
            {
                curtain.a = Mathf.Lerp(curtain.a, 1f - op.progress, Time.deltaTime * 0.3f);
                progressBar.color = curtain;
            }
        }*/
        /*if (!hi)loadingAnim.SetTrigger("Start");
        while (!hi)
        {
            yield return null;
        }*/
        //op.allowSceneActivation = true;
    }
}