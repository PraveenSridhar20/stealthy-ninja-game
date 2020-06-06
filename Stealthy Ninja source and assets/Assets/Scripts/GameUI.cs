using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    public GameObject livesOverUI;
    public AudioSource win,lose;
    public AudioClip wc,lc;
    public static int i=0;
    bool gameIsOver;
    // Start is called before the first frame update
    void Start()
    {
       win.clip=wc;
       lose.clip=lc;
       Guard.OnGuardSpot+=ShowGameLoseUI; 
       Guard.OnGuardSpot+=lose.Play;
       Player.OnGameWin+=ShowGameWinUI;
       Player.OnGameWin+=win.Play;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameIsOver){
            if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene(i);
        }
    }
    void ShowGameWinUI()
    {
      OnGameOver(gameWinUI);
      i++;
      if(i>4){
          i=0;
          Player.lives=1;
      }
    }
    void ShowGameLoseUI(){
        Player.lives-=1;
       if(Player.lives<0){
           i=0;
        Player.lives=1;
        OnGameOver(livesOverUI);
       }
       else{
           OnGameOver(gameLoseUI);
       }
   }
   void OnGameOver(GameObject gameOverUI){
        gameOverUI.SetActive(true);
        gameIsOver=true;
        Guard.OnGuardSpot-=ShowGameLoseUI;
        Player.OnGameWin-=ShowGameWinUI;
        Guard.OnGuardSpot-=lose.Play;
        Player.OnGameWin-=win.Play;

   }
}
