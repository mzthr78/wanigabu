using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    GvrControllerInputDevice gvrController;
    AudioSource aud;

    public AudioClip CoinSE;

    int credit = 0;

    public Button StartButton;
    public Text CreditText;

    private void Awake()
    {
        gvrController = GvrControllerInput.GetDevice(GvrControllerHand.Dominant);
        aud = GetComponent<AudioSource>();
        StartButton.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) || gvrController.GetButtonDown(GvrControllerButton.App))
        {
            aud.volume = 1;
            aud.PlayOneShot(CoinSE);
            StartCoroutine(FadeOut());
            credit++;
        }

        CreditText.text = credit.ToString();

        if (credit > 0)
        {
            StartButton.interactable = true;
        }
        else
        {
            StartButton.interactable = false;
        }
    }

    public void OnStartClick()
    {
        if (credit > 0) credit--;
        SceneManager.LoadScene("GameScene");
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1f);
        aud.volume = 0.3f;
    }
}
