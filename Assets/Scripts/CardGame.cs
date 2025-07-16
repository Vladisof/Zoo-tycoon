using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardGame : MonoBehaviour
{
    public MoneyController moneyController;
    public GameObject canvasGame;
    public GameObject textCard;

    public AudioManager audioManager;
    public Button buttonIncreaseBet;
    public Button buttonDecreaseBet;
    public Button buttonStart;
    public Button buttonBack;
    public Button buttonSelectCard1;
    public Button buttonSelectCard2;
    public TextMeshProUGUI betText;
    public PlayerTouchMovement playerTouchMovement;
    public CameraFollow cameraFollow;
    public ActiveObjectIndicator activeObjectIndicator;
    public GameObject winPL;
    public TextMeshProUGUI winText;
    public GameObject indicatorSelector1;
    public GameObject indicatorSelector2;
    public GameObject gameObjectBeforeSelection;
    public GameObject gameObjectAfterSelection;

    public GameObject[] cardPrefabs;
    public Transform[] cardSpawnPoints;

    private GameObject[] spawnedCards = new GameObject[2];
    private int selectedCardIndex = -1;
    private int betAmount = 5;
    private int minimumBet = 5;
    private bool isGameStarted = false;

    private void Start()
    {
        UpdateBetUI();
        EventManager.Instance.onInteraction.AddListener(OnInteraction);
        buttonIncreaseBet.onClick.AddListener(IncreaseBet);
        buttonDecreaseBet.onClick.AddListener(DecreaseBet);
        buttonStart.onClick.AddListener(StartGame);
        buttonBack.onClick.AddListener(OnBackButtonClick);
        buttonSelectCard1.onClick.AddListener(() => OnCardSelected(0));
        buttonSelectCard2.onClick.AddListener(() => OnCardSelected(1));
        buttonStart.interactable = false;

        SpawnCards(false);
    }

    private void OnBackButtonClick()
    {
        StartCoroutine(AwaitTriggerStartGame());
        playerTouchMovement.OnActivate();
        activeObjectIndicator.ShowIndicator();
        textCard.SetActive(true);
        canvasGame.SetActive(false);
        ResetGame();
        gameObjectBeforeSelection.SetActive(false);
        gameObjectAfterSelection.SetActive(false);
    }
    
    private IEnumerator AwaitTriggerStartGame()
    {
        yield return new WaitForSeconds(3);
        isGameStarted = false;
    }

    private void OnInteraction(GameObject interactingObject)
    {
        switch (interactingObject.tag)
        {
            case "CardGame":
                if (!isGameStarted)
                {
                    textCard.SetActive(false);
                    isGameStarted = true;
                    playerTouchMovement.OnDeactivate();
                    activeObjectIndicator.HideIndicator();
                    playerTouchMovement.OnDeactivate();
                    canvasGame.SetActive(true);
                    gameObjectBeforeSelection.SetActive(true);
                    gameObjectAfterSelection.SetActive(false);
                }
                break;
        }
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

    private void StartGame()
    {
        if (!moneyController.SubtractMoney(betAmount))
        {
            Debug.Log("Недостаточно средств для ставки.");
            return;
        }
        indicatorSelector1.SetActive(false);
        indicatorSelector2.SetActive(false);
        buttonStart.interactable = false;
        buttonIncreaseBet.interactable = false;
        buttonDecreaseBet.interactable = false;
        gameObjectBeforeSelection.SetActive(false);
        gameObjectAfterSelection.SetActive(false);
        foreach (var card in spawnedCards)
        {
            StartCoroutine(RotateCard(card));
        }
    }

    private IEnumerator RotateCard(GameObject card)
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        Quaternion startRotation = card.transform.rotation;
        Quaternion endRotation = Quaternion.Euler(-90, startRotation.eulerAngles.y, startRotation.eulerAngles.z);
audioManager.PlaySound(1);
        while (elapsedTime < duration)
        {
            card.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(ShowResults());
        card.transform.rotation = endRotation;
    }

    private void SpawnCards(bool setVisible)
    {
        ClearSpawnedCards();

        for (int i = 0; i < cardSpawnPoints.Length; i++)
        {
            int randomIndex = Random.Range(0, cardPrefabs.Length);
            GameObject card = Instantiate(cardPrefabs[randomIndex], cardSpawnPoints[i].position, Quaternion.identity);

            if (!setVisible)
            {
                card.transform.rotation = Quaternion.Euler(90, 0, 0);
            }

            spawnedCards[i] = card;
        }
    }

    private void OnCardSelected(int cardIndex)
    {
        
        selectedCardIndex = cardIndex;
        if (selectedCardIndex == 0)
        {
            indicatorSelector1.SetActive(true);
            indicatorSelector2.SetActive(false);
        }
        else
        {
            indicatorSelector1.SetActive(false);
            indicatorSelector2.SetActive(true);
        }
        gameObjectBeforeSelection.SetActive(false);
        gameObjectAfterSelection.SetActive(true);
        buttonStart.interactable = true;
    }

    private IEnumerator ShowResults()
    {
        yield return new WaitForSeconds(1);

        int playerCardValue = GetCardValue(spawnedCards[selectedCardIndex]);
        int opponentCardValue = GetCardValue(spawnedCards[1 - selectedCardIndex]);
        gameObjectBeforeSelection.SetActive(true);
        gameObjectAfterSelection.SetActive(false);
        if (playerCardValue > opponentCardValue)
        {
            moneyController.AddMoney(betAmount * 2);
            winPL.SetActive(true);
            winText.text = betAmount * 2+"";
            Debug.Log("Победа! Вы выиграли x2.");
        }
        else if (playerCardValue == opponentCardValue)
        {
            moneyController.AddMoney(betAmount);
            Debug.Log("Ничья! Вы получили x1.");
        }
        else
        {
            Debug.Log("Проигрыш. Ставка потеряна.");
        }

        ResetGame();
    }

    private int GetCardValue(GameObject card)
    {
        string cardName = card.name;
        if (cardName.Contains("2")) return 6;
        if (cardName.Contains("3")) return 7;
        if (cardName.Contains("4")) return 8;
        if (cardName.Contains("5")) return 9;
        if (cardName.Contains("6")) return 10;
        if (cardName.Contains("7")) return 11;
        if (cardName.Contains("8")) return 12;
        if (cardName.Contains("9")) return 13;
        if (cardName.Contains("10")) return 14;
        return 0;
    }

    private void ClearSpawnedCards()
    {
        foreach (GameObject card in spawnedCards)
        {
            if (card != null) Destroy(card);
        }
        spawnedCards = new GameObject[2];
        selectedCardIndex = -1;
    }

    private void ResetGame()
    {
        ClearSpawnedCards();
        buttonStart.interactable = true;
        buttonIncreaseBet.interactable = true;
        buttonDecreaseBet.interactable = true;
        buttonStart.interactable = false;
        SpawnCards(false);
        
    }

    private void UpdateBetUI()
    {
        betText.text = $" {betAmount}";
    }
}
