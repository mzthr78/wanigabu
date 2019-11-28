using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum WaniStatus
{
    none,
    start,
    eatyou,
    wani1,
    wani2,
    wani3,
    wani4,
    wani5,
    angry,
    wani6,
    finish,
    bye,
}

// <string, float>
struct WaniProgress
{
    public WaniStatus status;
    public float time;

    public WaniProgress(WaniStatus s, float t)
    {
        status = s;
        time = t;
    }
}

public class GameController : MonoBehaviour
{
    GvrControllerInputDevice gvrController;

    float elapsed = 0; //経過時間

    //Dictionary<int, int> timing = new Dictionary<int, int>();

    AudioSource aud;

    public AudioClip BGM2;
    public AudioClip EatYouSE;
    public AudioClip AngrySE;
    public AudioClip SurrenderSE;

    Queue<WaniProgress> WaniQ = new Queue<WaniProgress>();
    public GameObject[] Wani;

    public GameObject Hammer;

    public GameObject DeadLine;

    int HitCount = 0;
    int BittenCount = 0;

    public Text HitText;
    public Text BittenText;
    public Text ScoreText;

    public GameObject Menu;

    bool pause = false;
    public bool IsPause
    {
        get
        {
            return pause;
        }
    }

    private void Awake()
    {
        /*
        */
        WaniQ.Enqueue(new WaniProgress(WaniStatus.start, 2));
        WaniQ.Enqueue(new WaniProgress(WaniStatus.eatyou, 3));
        WaniQ.Enqueue(new WaniProgress(WaniStatus.wani1, 10));
        WaniQ.Enqueue(new WaniProgress(WaniStatus.wani2, 10));
        WaniQ.Enqueue(new WaniProgress(WaniStatus.wani3, 10));
        WaniQ.Enqueue(new WaniProgress(WaniStatus.wani4, 10));
        WaniQ.Enqueue(new WaniProgress(WaniStatus.wani5, 10));
        WaniQ.Enqueue(new WaniProgress(WaniStatus.none, 1.5f));
        WaniQ.Enqueue(new WaniProgress(WaniStatus.angry, 2));
        WaniQ.Enqueue(new WaniProgress(WaniStatus.wani6, 10));
        WaniQ.Enqueue(new WaniProgress(WaniStatus.finish, 7));
        WaniQ.Enqueue(new WaniProgress(WaniStatus.bye, 0));

        gvrController = GvrControllerInput.GetDevice(GvrControllerHand.Dominant);

        //やっぱやめた
        /*
        TextAsset textAsset = new TextAsset();
        textAsset = Resources.Load("timing", typeof(TextAsset)) as TextAsset;
        string text = textAsset.text;
        string[] line = text.Split('\n');
        for (int i = 0; i < line.Length; i++)
        {
            string[] col = line[i].Split(',');
            timing.Add(int.Parse(col[0]), int.Parse(col[1]));
        }
        */

        aud = GetComponent<AudioSource>();

        Menu.SetActive(false);
    }

    WaniProgress currProg;

    // Start is called before the first frame update
    void Start()
    {
        currProg = WaniQ.Dequeue();
        EnableDeadLine(false);

        HitCount = 0;
        BittenCount = 0;

        HitText.text = 0.ToString();
        BittenText.text = 0.ToString();
        ScoreText.text = 0.ToString();
    }

    float span = 0;
    float interval = 0;
    int counter = 0;
    int[] dat = { 0, 1, 2, 3, 4 };

    float clickStartTime;
    bool IsDoubleClick = false;

    // Update is called once per frame
    void Update()
    {
        //if (gvrController.)

        // Double Click Judge
        if (Input.GetMouseButtonDown(0) || gvrController.GetButtonDown(GvrControllerButton.App))
        {
            IsDoubleClick = false;
            if (clickStartTime > 0)
            {
                if ((Time.time - clickStartTime) < 0.3f)
                {
                    IsDoubleClick = true;
                }
            }
            clickStartTime = Time.time;
        }

        if (IsDoubleClick)
        {
            Pause(!pause);
            IsDoubleClick = false;
        }

        if (IsPause) return;

        HitText.text = HitCount.ToString();
        BittenText.text = BittenCount.ToString();

        if (currProg.status == WaniStatus.bye) return;

        elapsed += Time.deltaTime;
        span += Time.deltaTime;
        interval -= Time.deltaTime;

        if (span > currProg.time)
        {
            span = 0;
            counter = 0;
            currProg = WaniQ.Dequeue();
            Debug.Log(elapsed + " status = " + currProg.status);

            switch (currProg.status)
            {
                case WaniStatus.eatyou:
                    aud.volume = 1;
                    aud.PlayOneShot(EatYouSE);
                    StartCoroutine(PlayBGM(1));
                    break;
                case WaniStatus.angry:
                    aud.Stop();
                    aud.volume = 1;
                    aud.PlayOneShot(AngrySE);

                    EnableDeadLine(true);
                    StartCoroutine(PlayBGM(1, true));
                    break;
                case WaniStatus.finish:
                    aud.Stop();
                    StartCoroutine(Result());
                    break;
                case WaniStatus.bye:
                    aud.volume = 1;
                    aud.PlayOneShot(SurrenderSE);
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (interval > 0) return;


            switch (currProg.status)
            {
                case WaniStatus.wani1:
                    if (counter == 0)
                    {
                        Wani[2].GetComponent<WaniController>().Forward = true;
                    }
                    else
                    {
                        Wani[Random.Range(0, 5)].GetComponent<WaniController>().Forward = true;
                    }
                    interval = 2;
                    break;
                case WaniStatus.wani2:
                    Shuffle(dat);

                    Wani[dat[0]].GetComponent<WaniController>().Forward = true;
                    Wani[dat[1]].GetComponent<WaniController>().Forward = true;

                    interval = 2;

                    break;
                case WaniStatus.wani3:

                    Shuffle(dat);

                    Wani[dat[0]].GetComponent<WaniController>().Forward = true;
                    Wani[dat[1]].GetComponent<WaniController>().Forward = true;

                    if (counter == 0 || counter == 2 || counter == 3)
                    {
                        Wani[dat[2]].GetComponent<WaniController>().Forward = true;
                    }

                    interval = 2;
                    break;
                case WaniStatus.wani4:
                    if (counter == 0)
                    {
                        Wani[0].GetComponent<WaniController>().Forward = true;
                        Shuffle(dat);
                    }
                    else if (counter < 5)
                    {
                        Wani[counter].GetComponent<WaniController>().Forward = true;
                    }
                    else
                    {
                        Wani[dat[counter % 5]].GetComponent<WaniController>().Forward = true;
                    }

                    interval = 1;

                    break;
                case WaniStatus.wani5:
                    if (counter % 2 == 0)
                    {
                        Shuffle(dat);
                        Wani[dat[0]].GetComponent<WaniController>().Forward = true;
                        interval = 0.3f;
                    }
                    else
                    {
                        Wani[dat[1]].GetComponent<WaniController>().Forward = true;
                        interval = 1.7f;
                    }

                    break;
                case WaniStatus.wani6:
                    if (counter % 5 == 0)
                    {
                        Shuffle(dat);
                    }
                    Wani[dat[counter % 5]].GetComponent<WaniController>().Forward = true;
                    interval = 0.3f;
                    break;
                default:
                    break;
            }

            counter++;
        }


    }

    IEnumerator PlayBGM(float wait = 0, bool tempup = false)
    {
        yield return new WaitForSeconds(wait);

        if (tempup) aud.clip = BGM2;
        aud.volume = 0.1f;
        aud.loop = true;
        aud.Play();
    }

    void Shuffle(int[] src)
    {
        for (int i = 0; i < src.Length - 1; i++)
        {
            int r = Random.Range(0, src.Length);
            int tmp = src[r];
            src[r] = src[i];
            src[i] = tmp;
        }

    }

    public void Hit()
    {
        this.HitCount++;
    }

    public void Bitten()
    {
        this.BittenCount++;
    }

    IEnumerator Result()
    {
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < 3; i++)
        {
            HitText.enabled = false;
            BittenText.enabled = false;
            yield return new WaitForSeconds(0.5f);
            HitText.enabled = true;
            BittenText.enabled = true;
            yield return new WaitForSeconds(0.5f);
        }

        int score = 0;
        if ((HitCount - BittenCount) > 0) score = HitCount - BittenCount;

        ScoreText.text = score.ToString();

        for (int i = 0; i < 2; i++)
        {
            ScoreText.enabled = false;
            yield return new WaitForSeconds(0.5f);
            ScoreText.enabled = true;
            yield return new WaitForSeconds(0.5f);
        }

        yield return null;
    }

    void EnableDeadLine(bool b = true)
    {
        //return;
        DeadLine.GetComponent<Collider>().enabled = b;
    }

    public void Pause(bool b = true)
    {
        this.pause = b;

        if (b)
        {
            Menu.SetActive(true);
            Hammer.SetActive(false);
            aud.Pause();
        }
        else
        {
            Menu.SetActive(false);
            Hammer.SetActive(true);
            aud.UnPause();
        }
    }
}
