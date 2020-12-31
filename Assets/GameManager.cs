using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PlayerData
{
    List<Piece> piecePrefabs;
    List<Piece> pieces;
}
public enum GamePhase
{
    SETUP,
    PLAY
}
public class GameManager : MonoBehaviour
{
    int currentTurn;
    List<PlayerData> players;
    [SerializeField]
    GamePhase gamePhase;
    Board board;
    
    bool movePointer;
    Piece selectedPiece;
    // Start is called before the first frame update
    void Start()
    {
        gamePhase = GamePhase.SETUP;
    }

    bool SelectPiece(Vector2 mousePos)
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        ContactFilter2D filter2D = new ContactFilter2D
        {
            //Ignore trigger colliders
            useTriggers = false,
            //Use a layer mask
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Allies")
        };
        int numHit = 0;
        if ((numHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, filter2D, hits)) != 0)
        {
            for (int i = 0; i < numHit; i++)
            {
                selectedPiece = hits[i].transform.GetComponent<Piece>();
                return true;
            }
        }
        return false;
    }
    Vector2Int GetBoardPoint()
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        ContactFilter2D filter2D = new ContactFilter2D
        {
            //Ignore trigger colliders
            useTriggers = false,
            //Use a layer mask
            useLayerMask = true,
            layerMask = LayerMask.GetMask("Board")
        };
        int numHit = 0;
        if ((numHit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, filter2D, hits)) != 0)
        {
            for (int i = 0; i < numHit; i++)
            {
                return board.WorldToBoardSpace(hits[i].point);
            }
        }
        return -Vector2Int.one;
    }
    // Update is called once per frame
    void Update()
    {
        switch(gamePhase)
        {
            case GamePhase.SETUP:
                if (Input.GetMouseButtonDown(0))
                {
                    if(selectedPiece == null && SelectPiece(Input.mousePosition))
                    {
                        movePointer = true;
                    }else if(selectedPiece && movePointer)
                    {
                        Vector2Int p = GetBoardPoint();
                        if(board.IsInBounds(p))
                        {
                            board.Place(selectedPiece, p);
                        }
                    }
                    Debug.Log(GetBoardPoint());

                }
                break;
        }
    }
}
