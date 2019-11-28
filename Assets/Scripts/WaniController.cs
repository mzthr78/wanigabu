using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaniController : MonoBehaviour
{
    AudioSource aud;
    public AudioClip BiteSE;
    public AudioClip OuchSE;

    float orgz;

    Collider collid;

    GameController gameController;

    private void Awake()
    {
        collid = GetComponent<BoxCollider>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        orgz = transform.localPosition.z;
        aud = GetComponent<AudioSource>();
    }

    bool forward = false;

    public bool Forward
    {
        set
        {
            forward = value;
        }
        get
        {
            return forward;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.IsPause) return;

        if (forward)
        {
            if (transform.localPosition.z - 1.15f > 0.01f)
            {
                collid.enabled = true;
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Lerp(transform.localPosition.z, 1.15f, 0.03f));
            } else
            {
                forward = false;
            }
        }
        else
        {
            if (orgz - transform.localPosition.z > 0.01f)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Lerp(transform.localPosition.z, orgz, 0.075f));
            }
            else
            {
                forward = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hammer")
        {
            aud.Stop();
            aud.PlayOneShot(OuchSE);
            forward = false;

            gameController.Hit();
        }

        if (other.name == "DeadLine")
        {
            collid.enabled = false;
            aud.Stop();
            aud.PlayOneShot(BiteSE);
            forward = false;

            gameController.Bitten();
        }
    }
}
