using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JerryControl : MonoBehaviour
{
    public float rotateSpeed;

    public GameObject boinger;
    public GameObject[] boingers;

    public Material normal;
    public Material rainbow;

    private int phase;

    private Transform player;
    public GameObject boingBall;

    public GameObject linePrefab;
    private GameObject[] lineRens;

    public Transform cameraBounds;
    private bool isVincible;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        lineRens = new GameObject[0];

        isVincible = true;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < boingers.Length; i++)
        {
            boingers[i].transform.RotateAround(transform.position, Vector3.forward, Time.deltaTime * rotateSpeed);
            boingers[i].transform.Rotate(-3 / 2 * rotateSpeed * Time.deltaTime * Vector3.forward);
        }
    }

    public void NewBoingers(int num)
    {
        boingers = new GameObject[num];
        for (int i = 0; i < num; i++)
        {
            boingers[i] = Instantiate(boinger);
            boingers[i].SetActive(true);
            boingers[i].transform.parent = transform;
            boingers[i].transform.position = transform.position + Vector3.up * 5f;
            boingers[i].transform.RotateAround(transform.position, Vector3.forward, i * 360 / num);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !GameManager.isDead && isVincible)
        {
            SwitchFaze();
            collision.GetComponent<Rigidbody2D>().velocity = (collision.transform.position - transform.position).normalized * 10;
        }
    }

    private void SwitchFaze()
    {
        StopCoroutine(Lazers(1, 1));
        BobMovement move = GetComponent<BobMovement>();
        Clear(boingers);
        Clear(lineRens);

        GetComponent<SpriteRenderer>().material = rainbow;
        isVincible = false;
        Invoke("IsVincible", 3f);

        switch (phase)
        {
            case 0:
                foreach (CameraMovement c in FindObjectsOfType<CameraMovement>()) { c.targetBounds = cameraBounds; }
                StartCoroutine(Shoot());
                NewBoingers(2);
                StartCoroutine(Lazers(1, 5));
                move.enabled = true;
                FindObjectOfType<AudioManager>().StopAllSongs();
                FindObjectOfType<AudioManager>().Play("JerryTheme");
                break;
            case 1:
                NewBoingers(3);
                StartCoroutine(Lazers(3, 10));
                move.bobSpeed *= 1.5f;
                move.horizontalSpeed *= 1.5f;
                break;
            case 2:
                NewBoingers(5);
                StartCoroutine(Shoot());
                StartCoroutine(Lazers(5, -30));
                move.bobSpeed *= 1.5f;
                move.horizontalSpeed *= 1.5f;
                break;
            case 3:
                FindObjectOfType<AudioManager>().StopAllSongs();
                FindObjectOfType<AudioManager>().Play("JerryWin");
                Defeated();
                break;

        }

        phase += 1;
    }

    public void Defeated()
    {
        GameManager.menuManager.OpenPanel(GameManager.menuManager.transform.Find("Level Complete").gameObject);
        foreach (CameraMovement c in FindObjectsOfType<CameraMovement>()) { c.targetBounds = null; }
        Destroy(gameObject);
    }

    public void IsVincible()
    {
        isVincible = true;
        GetComponent<SpriteRenderer>().material = normal;
    }


    public IEnumerator Lazers(int num, float spinSpeed)
    {
        lineRens = new GameObject[num];

        for (int i = 0; i < num; i++)
        {
            lineRens[i] = Instantiate(linePrefab);
            lineRens[i].SetActive(true);
            lineRens[i].transform.position = transform.position;
            lineRens[i].transform.Rotate(i * 360 / num * Vector3.forward);
        }
        while (true)
        {
            foreach (var lineRen in lineRens)
            {
                lineRen.transform.Rotate(spinSpeed * Time.deltaTime * Vector3.forward);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, lineRen.transform.up);

                //not proformant at all im sorry
                lineRen.GetComponent<LineRenderer>().SetPosition(0, transform.position);
                lineRen.GetComponent<LineRenderer>().SetPosition(1, hit.point);
                Debug.DrawLine(transform.position, hit.point, Color.red);

                if (hit.collider.CompareTag("Player"))
                {
                    hit.collider.GetComponent<Player>().Hurt(1f);
                }

            }

            yield return null;
        }
    }
    public void Clear(GameObject[] list)
    {
        if (list.Length > 0)
        {
            foreach (GameObject item in list)
            {
                Destroy(item);
            }
        }
    }

    public IEnumerator Shoot()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            RandomFunctions.ShootFindVelocity(Mathf.Atan2(transform.position.y - player.position.y, transform.position.x - player.position.x), 3f, transform, player, boingBall, "null");
        }
    }
}
