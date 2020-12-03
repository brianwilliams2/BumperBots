using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class OnCollideSwitchTeam : MonoBehaviour
{
    public PunTeams.Team TeamToSwitchTo;
    private GameObject Player;

    public void OnTriggerEnter(Collider other)
    {
        // it's ridiculously easy to switch teams. you only have to make sure you do it for your own characters 
        // (this trigger is called on all clients, when a user's character enters the trigger...)

        // find a PhotonView and check if the character "isMine". Only then, set this client's player-team.
        PhotonView otherPv = other.GetComponent<PhotonView>();
        if (otherPv != null && otherPv.isMine)
        {
            PhotonNetwork.player.SetTeam(this.TeamToSwitchTo);
            Player = GameObject.FindWithTag("Player");
            if (TeamToSwitchTo == PunTeams.Team.red)
            {
                Player.GetComponent<MeshRenderer>().materials[2].color = Color.red;
                
            }
            if (TeamToSwitchTo == PunTeams.Team.blue)
            {
                Player.GetComponent<MeshRenderer>().materials[2].color = Color.blue;

            }
            
        }
    }
   
}
