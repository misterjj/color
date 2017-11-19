using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    public GameObject top;
    public GameObject left;
    public GameObject bottom;
    public GameObject right;

    public GameObject background;
    public List<Color> backgroundColors;

    public int thickness;

    public List<Material> colors;
    public List<string> colorsText;

    public GameObject cube;
    public GameObject text;
    public GameObject swpanPoint;
    private GameObject currentObj;

    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;
    public float velocity;

    private int score = 0;

    public Text scoreText;

    public int backgroundChangeStep;
    public int TextChangeStep;
    public int ColorChangeStep;

    public GameObject HUIGame;
    public GameObject HUILoss;
    public Text HUILossScore;
    public Text HUILossHighScore;

    private int highScore;

    // Use this for initialization
    void Start () {
        HUILoss.SetActive(false);
        HUIGame.SetActive(true);
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        initCubes();
        initColors();
        spawn();
    }
	
	// Update is called once per frame
	void Update () {
#if UNITY_ANDROID || UNITY_IOS
        swipeMobile();
#else
        swipeClick();
#endif
    }

    private void move(Vector2 direction)
    {
        currentObj.GetComponent<Rigidbody2D>().AddForce(direction * velocity, ForceMode2D.Force);
    }

    private void spawn()
    {
        if (score >= ColorChangeStep)
        {
            initColors();
        }
        int randColorIndex = Random.Range(0, colors.Count);
        Material color = colors[randColorIndex];
        if (score >= TextChangeStep)
        {
            currentObj = Instantiate(text, swpanPoint.transform);
            currentObj.GetComponent<TextMesh>().color = color.color;
            currentObj.GetComponent<TextMesh>().text = colorsText[Random.Range(0, colorsText.Count)];
        }
        else
        {
            currentObj = Instantiate(cube, swpanPoint.transform);
            currentObj.GetComponent<Renderer>().material = color;
        }
        currentObj.layer = 8 + randColorIndex;
        if (score >= backgroundChangeStep)
        {
            background.gameObject.GetComponent<Renderer>().material.color = backgroundColors[Random.Range(0, backgroundColors.Count)];
        }
    }

    private void initCubes()
    {
        var width = (float)(Camera.main.orthographicSize * 2.0 * Screen.width / Screen.height);
        var height = (float)(width * Screen.height / Screen.width);
        var ThicknessUnit = (float)(Camera.main.orthographicSize * 2.0 * thickness / Screen.height);

        bottom.GetComponent<Transform>().localScale = new Vector3(width, ThicknessUnit, 1);
        bottom.GetComponent<Transform>().localPosition = new Vector3(0, (-height / 2) + (ThicknessUnit / 2));

        top.GetComponent<Transform>().localScale = new Vector3(width, ThicknessUnit, 1);
        top.GetComponent<Transform>().localPosition = new Vector3(0, (height / 2) - (ThicknessUnit / 2));

        left.GetComponent<Transform>().localScale = new Vector3(ThicknessUnit, height, 1);
        left.GetComponent<Transform>().localPosition = new Vector3((-width / 2) + (ThicknessUnit / 2), 0);

        right.GetComponent<Transform>().localScale = new Vector3(ThicknessUnit, height, 1);
        right.GetComponent<Transform>().localPosition = new Vector3((width / 2) - (ThicknessUnit / 2), 0);

        background.GetComponent<Transform>().localScale = new Vector3(width, height, 1);
        Camera.main.GetComponent<BoxCollider2D>().size = new Vector2(width, height);
    }

    private void initColors()
    {
        // shuffle colord
        for (int i = 0; i < colors.Count; i++)
        {
            Material temp = colors[i];
            int randomIndex = Random.Range(i, colors.Count);
            colors[i] = colors[randomIndex];
            colors[randomIndex] = temp;
        }

        bottom.GetComponent<Renderer>().material = colors[0];
        bottom.layer = 8;
        left.GetComponent<Renderer>().material = colors[1];
        left.layer = 9;
        top.GetComponent<Renderer>().material = colors[2];
        top.layer = 10;
        right.GetComponent<Renderer>().material = colors[3];
        right.layer = 11;
    }

    public void swipeMobile()
    {
        if (Input.touches.Length > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                //save began touch 2d point
                firstPressPos = new Vector2(t.position.x, t.position.y);
            }
            if (t.phase == TouchPhase.Ended)
            {
                //save ended touch 2d point
                secondPressPos = new Vector2(t.position.x, t.position.y);

                //create vector from the two points
                currentSwipe = new Vector3(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

                //normalize the 2d vector
                currentSwipe.Normalize();
                move(currentSwipe);
            }
        }
    }

    public void swipeClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //save began touch 2d point
            firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            //save ended touch 2d point
            secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            //create vector from the two points
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);

            //normalize the 2d vector
            currentSwipe.Normalize();
            move(currentSwipe);
        }
    }

    public void win()
    {
        score++;
        scoreText.text = score.ToString();
        spawn();
    }

    public void loss()
    {
        currentObj.GetComponent<Rigidbody2D>().isKinematic = true;
        currentObj.GetComponent<Rigidbody2D>().freezeRotation = true;
        currentObj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        HUILossScore.text = score.ToString();
        HUILossHighScore.text = highScore.ToString();
        HUILoss.SetActive(true);
        HUIGame.SetActive(false);
    }

}
