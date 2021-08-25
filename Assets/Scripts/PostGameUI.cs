using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostGameUI : MonoBehaviour
{
    [Header("Rating Properties")]
    [SerializeField] [Tooltip("Defines how many fruits has to be in a bowl for bonus points")]
    private int massBonus = 10;
    [SerializeField] [Tooltip("Defines the amount of points for the mass bonus")]
    private int massPoints = 10;
    
    [Header("Displayed Info")]
    [SerializeField]
    private Text scoreText;

    [Space] 
    [SerializeField]
    private List<Text> bowlSumText;
    [SerializeField] 
    private List<Text> bowlFruitScoreTexts;
    [SerializeField]
    private List<Text> bowlMassScoreTexts;
    [SerializeField]
    private List<Text> bowlDiversityScoreTexts;
    [SerializeField]
    private List<Dropdown> bowlFruitLists;

    [Header("Buttons")] 
    [SerializeField]
    private GameObject buttonArea;
    [SerializeField]
    private Button playAgainButton;
    [SerializeField]
    private Button quitButton;

    [Header("Input")]
    [SerializeField]
    private InputAction skip;

    private bool _skipping = false;
    private int _score;
    private int[] _bowlSum;
    private int[] _singleFruitScore;
    private int[] _bowlMassScore;
    private int[] _diversityScore;
    private Dictionary<string, int>[] _differentFruits;

    // ENGINE METHODS
    
    private void Awake()
    {
        playAgainButton.onClick.AddListener(PlayAgain);
        quitButton.onClick.AddListener(Quit);
        
        buttonArea.SetActive(false);

        skip.performed += context => { _skipping = true; };
    }

    // BUTTON METHODS

    private void PlayAgain()
    {
        SceneManager.LoadScene("TestScene");
    }

    private void Quit()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    // LEVEL END LOGIC
    
    public void Initialize(List<CollectableFruitScriptableObject>[] bowls)
    {
        var length = bowls.Length;
        _bowlSum = new int[length];
        _singleFruitScore = new int[length];
        _bowlMassScore = new int[length];
        _diversityScore = new int[length];
        _differentFruits = new Dictionary<string, int>[length];

        for (var i = 0; i < length; i++)
        {
            _differentFruits[i] = new Dictionary<string, int>();
        }
        
        skip.Enable();
        
        StartCoroutine(Animation(bowls));
    }

    private IEnumerator Animation(List<CollectableFruitScriptableObject>[] bowls)
    {
        // wait to make sure UI is spawned properly
        yield return new WaitForSeconds(0.2f);
        
        // loop for rating stuff
        var max = bowls.Select(bowl => bowl.Count).Prepend(0).Max();
        for (var i = 0; i < max; i++)
        {
            for (var j = 0; j < bowls.Length; j++)
            {
                // return when the bowl doesn't have i items
                if(bowls[j].Count < max || bowls[j].Count == 0) continue;
                
                // give bonus points for x fruits in the bowl
                if (i % massBonus == 0 && i > 0)
                {
                    _bowlMassScore[j] += massPoints;
                    _score += massPoints;

                    // change the text for the mass bonus
                    bowlMassScoreTexts[j].text = _bowlMassScore[j].ToString();
                }
                
                // get points for the actual fruit that's in the bowl
                var points = bowls[j][i].points;
                _singleFruitScore[j] += points;
                _bowlSum[j] += points;
                _score += points;

                // change the texts
                bowlSumText[j].text = _bowlSum[j].ToString();
                bowlFruitScoreTexts[j].text = _singleFruitScore[j].ToString();
                scoreText.text = $"Score: {_score.ToString()}";

                // Add fruit to the array 
                if (_differentFruits[j].ContainsKey(bowls[j][i].fruitName))
                {
                    _differentFruits[j][bowls[j][i].fruitName] += 1;
                }
                else
                {
                    _differentFruits[j].Add(bowls[j][i].fruitName, 1);
                }
            }

            if (!_skipping) yield return new WaitForSeconds(0.1f);
        }

        if (!_skipping) yield return new WaitForSeconds(0.5f);

        // rate the diversity
        for (var i = 0; i < _differentFruits.Length; i++)
        {
            // continue when no fruits where in the bowl
            if (_differentFruits[i].Count <= 0)
            {
                bowlFruitLists[i].options = new List<Dropdown.OptionData> {new Dropdown.OptionData("None")};
                bowlFruitLists[i].gameObject.SetActive(true);
                if (!_skipping) yield return new WaitForSeconds(0.1f);
                continue;
            }
            
            // sort by number of fruits and calculate points based on the portion of the 
            var sorted = _differentFruits[i].OrderByDescending(key => key.Value);
            var rating = 100 - ((float) sorted.First().Value / bowls[i].Count) * 100;
            _diversityScore[i] = (int) Math.Ceiling(rating);
            
            // add scores
            _bowlSum[i] += _diversityScore[i];
            _score += _diversityScore[i];
            
            // change the texts
            bowlSumText[i].text = _bowlSum[i].ToString();
            bowlDiversityScoreTexts[i].text = _diversityScore[i].ToString();
            scoreText.text = $"Score: {_score.ToString()}";
            
            // add the fruits to combobox
            var dropdownData = sorted.Select(pair => new Dropdown.OptionData(
                $"{pair.Key} - {pair.Value.ToString()}")).ToList();
            bowlFruitLists[i].options = dropdownData;
            bowlFruitLists[i].gameObject.SetActive(true);
            
            yield return new WaitForSeconds(0.1f);
        }
        
        if (!_skipping) yield return new WaitForSeconds(0.5f);
        buttonArea.SetActive(true);
    }
}
