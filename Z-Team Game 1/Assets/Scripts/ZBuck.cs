using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZBuck : MonoBehaviour
{
    private enum ZBuckState { Entering, Standing, Exiting }
    private const float ENTER_TIME = 0.5f;
    private const float EXIT_TIME = 0.5f;

    Player player;
    private ZBuckState state;
    private Vector3 enterTarget;
    private float timer;
    private ushort value;

    /// <summary>
    /// Initialize the zbuck
    /// </summary>
    /// <param name="enterTarget"></param>
    public void Init(Vector3 enterTarget, ushort value)
    {
        this.enterTarget = enterTarget;
        timer = 0;
        state = ZBuckState.Entering;
        this.value = value;
        player = GameManager.Instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            //Lerp in direction
            case ZBuckState.Entering:
                timer += Time.deltaTime / ENTER_TIME;
                transform.position = Vector3.Lerp(transform.position, enterTarget, timer);
                if(timer > 1)
                {
                    state = ZBuckState.Standing;
                }
                break;
            
            //Stand until player is near
            case ZBuckState.Standing:
                if(Vector3.SqrMagnitude(player.transform.position - transform.position) < Player.ZBUCK_COLLECTION_RADIUS)
                {
                    timer = 0;
                    state = ZBuckState.Exiting;
                }
                break;
            
            //Lerp to player
            case ZBuckState.Exiting:
                timer += Time.deltaTime / EXIT_TIME;
                transform.position = Vector3.Lerp(transform.position, player.transform.position, timer);
                if (timer > 1)
                {
                    player.AddZBucks(value);
                    Destroy(gameObject);
                }
                break;

            default:
                break;
        }
    }
}
