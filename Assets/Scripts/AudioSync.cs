using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent (typeof(AudioSource))]
public class AudioSync : NetworkBehaviour
{
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip[] clips;

    public override void PreStartClient()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(int clipID)
    {
        if(clipID == -1)
        {
            clipID = Random.Range(0, clips.Length);
        }
        CmdSendServerSoundClip(clipID);
    }

    [Command]
    void CmdSendServerSoundClip(int clipID)
    {
        RpcSendSoundClipToClients(clipID);
    }

    [ClientRpc]
    void RpcSendSoundClipToClients(int clipID)
    {
        audioSource.PlayOneShot(clips[clipID]);
    }
}
