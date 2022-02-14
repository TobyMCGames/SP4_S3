using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class PlayerAction : MonoBehaviour
{
    private Rigidbody rigidbody;
    public GameObject bombPrefab;

    private JLGameManager gameManager;
    private PhotonView photonView;
    private PropObserver Prop_observer;
    private PlayerInfo playerInfo;

    private float InvisibilityTimer = 0f;
    private float StunTimer = 0f;
 
    void Awake()
    {
        gameManager = JLGameManager.Instance;
        photonView = GetComponent<PhotonView>();
        rigidbody = GetComponent<Rigidbody>();
        Prop_observer = gameObject.transform.Find("PointOfView").GetComponent<PropObserver>();
        playerInfo = GetComponent<PlayerInfo>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump") && photonView.IsMine)
        {
            photonView.RPC("PlantBomb", RpcTarget.AllViaServer, rigidbody.position);
        }
        if (Input.GetButtonDown("Interact") && photonView.IsMine && Prop_observer.GetProp() != null)
        {
            if (Prop_observer.GetProp().CompareTag("Prop") && playerInfo.GetTeam() == PlayerInfo.TEAMS.PROP)
            {
                PhotonView ChangeProp = Prop_observer.GetProp().GetComponent<PhotonView>();
                photonView.RPC("ChangePropObject", RpcTarget.AllViaServer, photonView.ViewID, ChangeProp.ViewID);
            }
            else if (Prop_observer.GetProp().CompareTag("AbilityOrb"))
            {
                photonView.RPC("CollectOrb", RpcTarget.AllViaServer, Prop_observer.GetProp().GetComponent<PhotonView>().ViewID);
            }
        }
        if (Input.GetButtonDown("Ultimate") && photonView.IsMine && playerInfo.IsAbilityAvailable())
        {
            playerInfo.SetAbilityAvailable(false);
            if (playerInfo.GetAbility() == PlayerInfo.ABILITY.SPAWN)
            {

            }
            else if (playerInfo.GetAbility() == PlayerInfo.ABILITY.INVISIBILITY)
            {
                photonView.RPC("TurnInvisible", RpcTarget.AllViaServer, photonView.ViewID, false, 10.0f);
            }
            else if (playerInfo.GetAbility() == PlayerInfo.ABILITY.REVEAL)
            {
                List<int> propList = gameManager.GetPropList();
                for (int i = 0; i < propList.Count; i++)
                {
                    GameObject gameObject = PhotonNetwork.GetPhotonView(propList[i]).gameObject;
                    Vector3 position = gameObject.transform.position;
                    Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    GameObject pillar = PhotonNetwork.Instantiate("LightBeam", position, rotation);
                    gameManager.GetComponent<PhotonView>().RPC("SetLightBeamToObject", RpcTarget.AllViaServer, pillar.GetComponent<PhotonView>().ViewID, gameObject.GetComponent<PhotonView>().ViewID, 10.0f, false);
                }
            }
            else if (playerInfo.GetAbility() == PlayerInfo.ABILITY.STUN)
            {
                List<int> propList = gameManager.GetPropList();
                for (int i = 0; i < propList.Count; i++)
                {
                    photonView.RPC("StunEntity", RpcTarget.AllViaServer, propList[i], false, 10.0f);
                }
            }
        }
        if (Input.GetButtonDown("Fire1") && photonView.IsMine && playerInfo.GetTeam() == PlayerInfo.TEAMS.HUNTER)
        {
            Quaternion direction = gameManager.MainCamera.transform.rotation;
            Vector3 pos = gameManager.MainCamera.transform.position;
            GameObject bullet = PhotonNetwork.Instantiate("BoxBullet", pos, direction);
            Vector3 AngleDir = new Vector3( bullet.transform.eulerAngles.x, bullet.transform.eulerAngles.y);
            bullet.GetComponent<PhotonView>().RPC("SetCreatedBullet", RpcTarget.AllViaServer, AngleDir.x, AngleDir.y, 10.0f, photonView.ViewID);
        }

        if (InvisibilityTimer > 0)
        {
            InvisibilityTimer -= Time.deltaTime;
            if (InvisibilityTimer <= 0)
            {
                photonView.RPC("TurnInvisible", RpcTarget.AllViaServer, photonView.ViewID, true, 0.0f);
            }
        }
        if (StunTimer > 0)
        {
            StunTimer -= Time.deltaTime;
            if (StunTimer <= 0)
            {
                List<int> propList = gameManager.GetPropList();
                for (int i = 0; i < propList.Count; i++)
                {
                    photonView.RPC("StunEntity", RpcTarget.AllViaServer, propList[i], true, 0.0f);
                }
            }
        }
    }

    [PunRPC]
    public void PlantBomb(Vector3 position)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject bomb = PhotonNetwork.Instantiate("Bomb", position, Quaternion.identity);
        }
    }

    [PunRPC]
    public void ChangePropObject(int playerID, int nextPropID)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        GameObject player = PhotonNetwork.GetPhotonView(playerID).gameObject;
        GameObject nextProp = PhotonNetwork.GetPhotonView(nextPropID).gameObject;

        GameObject currProp = player.transform.GetChild(0).gameObject;
        Vector3 pos = currProp.transform.position;
        Quaternion Rotation = currProp.transform.rotation;

        nextProp = PhotonNetwork.Instantiate(
            nextProp.name,
            pos,
            Rotation, 0);

        photonView.RPC("SwitchPlayerObject", RpcTarget.AllViaServer, 
            playerID, 
            nextProp.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    public void SwitchPlayerObject(int playerID, int nextpropID)
    {
        GameObject player = PhotonNetwork.GetPhotonView(playerID).gameObject;
        GameObject currProp = player.transform.GetChild(0).gameObject;
        GameObject nextProp = PhotonNetwork.GetPhotonView(nextpropID).gameObject;


        nextProp.transform.parent = player.transform;
        nextProp.transform.SetAsFirstSibling();

        Destroy(currProp);
        Debug.Log("Deleted");

        player.transform.Translate(Vector3.up * 0.5f);
        nextProp.transform.position = player.transform.position;
    }

    [PunRPC]
    public void CollectOrb(int orbID)
    {
        Destroy(PhotonNetwork.GetPhotonView(orbID).gameObject);

        if(photonView.IsMine)
            playerInfo.SetAbilityAvailable(true);
    }

    [PunRPC]
    public void TurnInvisible(int ID, bool state, float timer)
    {
        GameObject gameObject = PhotonNetwork.GetPhotonView(ID).gameObject;
        if (gameObject.transform.GetChild(0).GetComponent<MeshRenderer>())
            gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = state;
        else if (gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>())
            gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().enabled = state;

        if (photonView.IsMine && timer > 0f)
            InvisibilityTimer = timer;
    }

    [PunRPC]
    public void StunEntity(int ID, bool state, float timer)
    {
        Debug.Log("Setting movement to: " + state);
        GameObject gameObject = PhotonNetwork.GetPhotonView(ID).gameObject;
        if (gameObject.GetComponent<PlayerMovement>())
            gameObject.GetComponent<PlayerMovement>().enabled = state;

        if (photonView.IsMine && timer > 0f)
            StunTimer = timer;
    }
}
