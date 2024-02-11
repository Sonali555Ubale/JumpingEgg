using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public int score = 0;
    public Text scoreText;
    public EggController eggController;
    public string uiTag;

    // Update is called once per frame
    void Update()
    {
        /*switch (uiTag)
        {
            case "Coins":
                scoreText.text = "Coins " + eggController.coins;
                break;
            case "Score":
                scoreText.text = "Score "+eggController.score;
                break;
            default: break;
        }*/
    }
}
