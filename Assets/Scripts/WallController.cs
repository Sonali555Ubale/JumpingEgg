using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WallController : MonoBehaviour
{
  //  public Rigidbody2D rb;
    public float offset = 0.002f;
    private void Update()
    {
        if (gameObject.name.Equals("wall_bottom"))
        {
            GameManager.instance.WallPositionY = gameObject.transform.position.y + offset;
            //+ (offset * Time.timeScale)
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EggController egg = collision.GetComponent<EggController>();
        if (egg != null)
        {
            GameManager.instance.playerDead();
            //egg.rb = new Rigidbody2D();
            //egg.rb.velocity = new Vector2(0f, 0f);
            //egg.rb.rotation = 360f;
            //egg.transform.position = new Vector3(0, -3.8f);
        }

        if (gameObject.name.Equals("wall_bottom"))
        {
            Debug.LogWarning(collision.name);
            Destroy(collision.gameObject);
        }

    }
}
