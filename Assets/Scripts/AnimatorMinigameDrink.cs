using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorMinigameDrink : MonoBehaviour
{
    public Animator animator1;
    public Animator animator2;
    public Animator animator3;
    public MoneyController moneyController;
    public PlayerTouchMovement playerTouchMovement;
    public CameraFollow cameraFollow;
    public GameObject canvasGame;
    public GameObject textCard;
    public ActiveObjectIndicator activeObjectIndicator;
    public GameObject winPL;
    public TextMeshProUGUI winText;
    public GameObject indicatorSelector1;
    public GameObject indicatorSelector2;
    public GameObject indicatorSelector3;
    public AudioManager audioManager;
    public GameObject gameObjectBeforeSelection; // Активен пока игрок не выбран
    public GameObject gameObjectAfterSelection;  // Активен после выбора игрока

    public Button buttonIncreaseBet;
    public Button buttonDecreaseBet;
    public Button buttonStart;
    public Button buttonBack;
    public TextMeshProUGUI betText;

    private int selectedPlayer = 0; // 0 - первый, 1 - второй, 2 - третий
    private int betAmount = 5;
    private int minimumBet = 5;
    private bool isGameStarted = false;
    private int random;

    private void Start()
    {
        // Устанавливаем начальные значения
        UpdateBetUI();
        EventManager.Instance.onInteraction.AddListener(OnInteraction);
        // Назначаем кнопкам действия
        buttonIncreaseBet.onClick.AddListener(IncreaseBet);
        buttonDecreaseBet.onClick.AddListener(DecreaseBet);
        buttonStart.onClick.AddListener(StartGame);
        buttonBack.onClick.AddListener(onBackButtonClick);
        gameObjectBeforeSelection.SetActive(true);
        gameObjectAfterSelection.SetActive(false);
        buttonStart.interactable = false;
    }
    
    private void onBackButtonClick()
    {
        isGameStarted = true;
        textCard.SetActive(true);
        playerTouchMovement.OnActivate();
        activeObjectIndicator.ShowIndicator();
        canvasGame.SetActive(false);
        StartCoroutine(AwaitTrigerStartGame());
        gameObjectBeforeSelection.SetActive(false);
        gameObjectAfterSelection.SetActive(false);
    }
    
    private IEnumerator AwaitTrigerStartGame()
    {
        yield return new WaitForSeconds(5);
        isGameStarted = false;
    }
    private void IncreaseBet()
    {
        betAmount += 5;
        UpdateBetUI();
    }

    private void DecreaseBet()
    {
        if (betAmount > minimumBet)
        {
            betAmount -= 5;
            UpdateBetUI();
        }
    }

    private void OnInteraction(GameObject interactingObject)
    {
        switch (interactingObject.tag)
        {
            case "GameDrink":
                if (!isGameStarted)
                {
                    isGameStarted = true;
                    textCard.SetActive(false);
                    playerTouchMovement.OnDeactivate();
                    activeObjectIndicator.HideIndicator();
                    canvasGame.SetActive(true);
                    gameObjectBeforeSelection.SetActive(true);
                    gameObjectAfterSelection.SetActive(false);
                    buttonStart.interactable = false;
                    
                    Debug.Log("Взаимодействие с мини-игрой Drink.");
                }
                break;
        }
        
    }
    private void StartGame()
    {
        // Проверка баланса
        if (!moneyController.SubtractMoney(betAmount))
        {
            Debug.Log("Недостаточно средств для ставки.");
            return;
        }
        
        indicatorSelector1.SetActive(false);
        indicatorSelector2.SetActive(false);
        indicatorSelector3.SetActive(false);
        gameObjectBeforeSelection.SetActive(false);
        gameObjectAfterSelection.SetActive(false);
        audioManager.PlaySound(2);
        
        buttonStart.interactable = false;
        buttonIncreaseBet.interactable = false;
        buttonDecreaseBet.interactable = false;

        // Начинаем пить
        animator1.SetBool("Drink", true);
        animator2.SetBool("Drink", true);
        animator3.SetBool("Drink", true);

        random = Random.Range(0, 3);

        if (random == 0)
        {
            animator1.SetBool("Live", true);
            animator2.SetBool("Dead", true);
            animator3.SetBool("Dead", true);
        }
        else if (random == 1)
        {
            animator1.SetBool("Dead", true);
            animator2.SetBool("Live", true);
            animator3.SetBool("Dead", true);
        }
        else if (random == 2)
        {
            animator1.SetBool("Dead", true);
            animator2.SetBool("Dead", true);
            animator3.SetBool("Live", true);
        }

        StartCoroutine(AwaitEndDrink(5));
    }

    private IEnumerator AwaitEndDrink(int second)
    {
        yield return new WaitForSeconds(second);
        animator1.SetBool("Drink", false);
        animator2.SetBool("Drink", false);
        animator3.SetBool("Drink", false);
        animator1.SetBool("Live", false);
        animator2.SetBool("Live", false);
        animator3.SetBool("Live", false);
        animator1.SetBool("Dead", false);
        animator2.SetBool("Dead", false);
        animator3.SetBool("Dead", false);
        buttonStart.interactable = false;
        buttonIncreaseBet.interactable = true;
        buttonDecreaseBet.interactable = true;
        gameObjectBeforeSelection.SetActive(true);
        gameObjectAfterSelection.SetActive(false);
        if (selectedPlayer == random)
        {
            int reward = betAmount * 3;
            moneyController.AddMoney(reward);
            winPL.SetActive(true);
            winText.text = reward.ToString();
            Debug.Log($"Вы выиграли! Награда: {reward}");
        }
        else
        {
            Debug.Log("Вы проиграли!");
        }
    }


    private void UpdateBetUI()
    {
        betText.text = $"{betAmount}";
    }

    public void SelectPlayer(int playerIndex)
    {
        selectedPlayer = playerIndex;
        if (playerIndex == 0)
        {
            indicatorSelector1.SetActive(true);
            indicatorSelector2.SetActive(false);
            indicatorSelector3.SetActive(false);
        }
        else if (playerIndex == 1)
        {
            indicatorSelector1.SetActive(false);
            indicatorSelector2.SetActive(true);
            indicatorSelector3.SetActive(false);
        }
        else if (playerIndex == 2)
        {
            indicatorSelector1.SetActive(false);
            indicatorSelector2.SetActive(false);
            indicatorSelector3.SetActive(true);
        }
        buttonStart.interactable = true;
        gameObjectBeforeSelection.SetActive(false);
        gameObjectAfterSelection.SetActive(true);
        Debug.Log($"Выбран игрок: {playerIndex + 1}");
    }
}
