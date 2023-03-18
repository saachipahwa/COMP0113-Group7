using UnityEngine;
using UnityEngine.SceneManagement;
using Ubiq.Messaging;

// Script used to restart game
// Does not work so is not currently part of the application 
public class ChangeScene : MonoBehaviour
{
    NetworkContext context;
    void Start()
    {
        context = NetworkScene.Register(this);
    }
    struct Message
    {
        public bool reset;
    }

    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var msg = message.FromJson<Message>();
        if (msg.reset)
        {
            LoadScene(false);
        }
    }
    public void LoadScene(bool owner = true)
    {
        if (owner)
        {
            context.SendJson(new Message()
            {
                reset = true
            });
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}


