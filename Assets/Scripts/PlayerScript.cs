using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerScript : Photon.PunBehaviour, IPunObservable
{
    public static PlayerScript me;
    public bool isPlayer = true;
    public GameObject weapon;
    public ParticleSystem particles;

    bool isMine = false;
    RigidbodyFirstPersonController rfpc;
    float health = 1f;
    bool isFiring = false;
    Material healthBarMat;
    GameObject healthBar;

    void Awake()
    {
        healthBar = transform.FindChild("HealthBar").gameObject;
        healthBarMat = healthBar.GetComponent<MeshRenderer>().material;
        rfpc = GetComponent<RigidbodyFirstPersonController>();
        if (isPlayer)
        {
            if (photonView.isMine || !PhotonNetwork.connected)
            {
                GameObject cam = GameObject.FindWithTag("MainCamera");

                cam.transform.parent = transform;
                cam.transform.position = transform.position + Vector3.up * 0.8f;
                weapon.transform.parent = cam.transform;

                var bob = cam.GetComponent<HeadBob>();

                bob.rigidbodyFirstPersonController = rfpc;

                GetComponent<MeshRenderer>().enabled = false;
                healthBar.GetComponent<MeshRenderer>().enabled = false;

                isMine = true;
                me = this;
            }
            if (!photonView.isMine && PhotonNetwork.connected)
            {
                rfpc.enabled = false;
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            rfpc.enabled = false;
        }
	}

    void Update()
    {
        ProcessInputs();

        if (health <= 0)
        {
            if (PhotonNetwork.connected)
            {
                if (photonView.isMine)
                {
                    GameManager.instance.LeaveRoom();
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        if (!isPlayer)
        {
            isFiring = true;
        }

        if (isFiring)
        {
            if (!particles.isPlaying)
            {
                particles.Play();
            }
        }
        else
        {
            if (particles.isPlaying)
            {
                particles.Stop();
                particles.Clear();
            }
        }

        healthBarMat.SetFloat("_Health", health);
        healthBar.transform.LookAt(Camera.main.transform);
    }

    void FixedUpdate()
    {
        if (isFiring)
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 50))
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    hit.transform.gameObject.GetComponent<PlayerScript>().ReceiveDamage(this);
                }

            }
        }
    }

    void ProcessInputs()
    {
        if (!(isPlayer && (photonView.isMine || !PhotonNetwork.connected)))
            return;

        if (Input.GetButtonDown("Fire1"))
        {
            isFiring = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            isFiring = false;
        }
    }

    void ReceiveDamage(PlayerScript other)
    {
        if (other == this)
            return;
        if (photonView.isMine || !PhotonNetwork.connected)
        {
            health -= 0.025f;
        }
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(health);
            stream.SendNext(isFiring);
        }
        else
        {
            health = (float)stream.ReceiveNext();
            isFiring = (bool)stream.ReceiveNext();
        }
    }
}
