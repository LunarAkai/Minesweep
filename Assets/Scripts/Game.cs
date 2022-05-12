using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
      [SerializeField] private int width = 16;
      [SerializeField] private int height = 16;
      [SerializeField] private int mineCount = 32;
      [SerializeField] private GameObject gameOverScreen;
      [SerializeField] private GameObject settingsMenu;
      [SerializeField] private GameObject winScreen;
      [SerializeField] private TMP_Text timerText;

      private Board board;
      private BoardSize boardSize;
      private Cell[,] state;
      private bool gameover;
      private bool inSettingsMenu = false;
      private float timer = 0f;

      private int[] _boardSizes;
      private int _boardSizeIndex = 1;
      private int[] _mineCounter;
      

      public void OnValidate()
      {
            mineCount = Mathf.Clamp(mineCount, 0, width * height);
      }

      private void Awake()
      {
            boardSize = GetComponent<BoardSize>();
            board = GetComponentInChildren<Board>();
      }

      private void Start()
      {
            _boardSizes = boardSize.boardSizes;

            _mineCounter = boardSize.mineCounter;

            width = _boardSizes[_boardSizeIndex];
            height = _boardSizes[_boardSizeIndex];
            mineCount = _mineCounter[_boardSizeIndex];

            SetCameraSize();
            
            NewGame();
      }
      
      public void SetBoardIndex(int boardIndex)
      {
            _boardSizeIndex = boardIndex;
            width = _boardSizes[_boardSizeIndex];
            height = _boardSizes[_boardSizeIndex];
            mineCount = _mineCounter[_boardSizeIndex];
      }

      private void SetCameraSize()
      {
            if (_boardSizeIndex == 0)
            {
                  Camera.main.orthographicSize = 5f;
                  
            } else if (_boardSizeIndex == 1)
            {

                  Camera.main.orthographicSize = 10f;

            } else if (_boardSizeIndex == 2)
            {
                  Camera.main.orthographicSize = 18f;
                  
            } else if (_boardSizeIndex == 3)
            {
                  Camera.main.orthographicSize = 36f;
            }
            else
            {
                  Camera.main.orthographicSize = 10f;
            }

            Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
      }

      private void NewGame()
      {
            SetCameraSize();
            timer = 0f;
            state = new Cell[width, height];
            
            if (gameOverScreen.activeInHierarchy)
            {
                  gameOverScreen.SetActive(false);
            }
            
            if (winScreen.activeInHierarchy)
            {
                  winScreen.SetActive(false);
            }
            
            gameover = false;
            
            GenerateCells();
            GenerateMines();
            GenerateNumbers();

            
            board.Draw(state);
      }

      private void GenerateCells()
      {
            for (int x = 0; x < width; x++)
            {
                  for (int y = 0; y < height; y++)
                  {
                        Cell cell = new Cell();
                        cell.position = new Vector3Int(x, y, 0);
                        cell.type = Cell.Type.Empty;
                        state[x, y] = cell;
                  }
            }
      }

      private void GenerateMines()
      {
            for (int i = 0; i < mineCount; i++)
            {
                  int x = Random.Range(0, width);
                  int y = Random.Range(0, height);

                  while (state[x, y].type == Cell.Type.Mine)
                  {
                        x++;

                        if (x >= width)
                        {
                              x = 0;
                              y++;

                              if (y >= height)
                              {
                                    y = 0;
                              }
                        }
                  }
                  
                  state[x, y].type = Cell.Type.Mine;
            }
      }

      private void GenerateNumbers()
      {
            for (int x = 0; x < width; x++)
            {
                  for (int y = 0; y < height; y++)
                  {
                        Cell cell = state[x, y];

                        if (cell.type == Cell.Type.Mine)
                        {
                              continue;
                        }

                        cell.number = CountMines(x, y);

                        if (cell.number > 0)
                        {
                              cell.type = Cell.Type.Number;
                        }
                        
                        state[x, y] = cell;
                        
                  }
            }
      }

      private int CountMines(int cellX, int cellY)
      {
            int count = 0;

            for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
            {
                  for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
                  {
                        if (adjacentX == 0 && adjacentY == 0)
                        {
                              continue;
                        }
                        
                        int x = cellX + adjacentX;
                        int y = cellY + adjacentY; //adjacent =>  like a offset
                        
                        if (GetCell(x,y).type == Cell.Type.Mine)
                        {
                              count++;
                        }
                  }
            }
            
            return count;
      }

      private void Update()
      {
            
            
            timerText.text = "Time: " + Math.Round(timer, 4) + " s";
            
            if (Input.GetKeyDown(KeyCode.S))
            {
                  inSettingsMenu = !inSettingsMenu;

                  if (settingsMenu.activeInHierarchy)
                  {
                        settingsMenu.SetActive(false);
                  }
                  else
                  {
                        settingsMenu.SetActive(true);
                  }
                  
            }
            
            
            
            if (Input.GetKeyDown(KeyCode.R) && gameover)
            {
                  
                  NewGame();
            }
            
            else if (Input.GetKeyDown(KeyCode.Q) && gameover)
            {
                  Application.Quit();
            }
            
            else if (!gameover && !inSettingsMenu)
            {
                  
                  timer += Time.deltaTime;
                  
                  if (Input.GetMouseButtonDown(1))
                  {
                        Flag();
                  } else if (Input.GetMouseButtonDown(0))
                  {
                        Reveal();
                  }
            }
            
      }

      private void Flag()
      {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
            Cell cell = GetCell(cellPosition.x, cellPosition.y);

            if (cell.type == Cell.Type.Invalid || cell.revealed)
            {
                  return;
            }
            
            cell.flagged = !cell.flagged;
            state[cellPosition.x, cellPosition.y] = cell;
            board.Draw(state);
      }

      private void Reveal()
      {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
            Cell cell = GetCell(cellPosition.x, cellPosition.y);

            if (cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged)
            {
                  return;
            }

            switch (cell.type)
            {
                  case Cell.Type.Mine:
                        Explode(cell);
                        break;
                  case Cell.Type.Empty:
                        Flood(cell);
                        CheckWinCondition();
                        break;
                  default:
                        cell.revealed = true;
                        state[cellPosition.x, cellPosition.y] = cell;
                        CheckWinCondition();
                        break;
            }
            
            board.Draw(state);
      }

      private void Flood(Cell cell)
      {
            if (cell.revealed) return;
            if (cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) return;

            cell.revealed = true;
            state[cell.position.x, cell.position.y] = cell;

            if (cell.type == Cell.Type.Empty)
            {
                  Flood(GetCell(cell.position.x - 1, cell.position.y));
                  Flood(GetCell(cell.position.x + 1, cell.position.y));
                  Flood(GetCell(cell.position.x, cell.position.y - 1));
                  Flood(GetCell(cell.position.x, cell.position.y + 1));
            }
      }

      private void Explode(Cell cell)
      {
            //Debug.Log("Game Over");
            gameover = true;
            gameOverScreen.SetActive(true);

            cell.revealed = true;
            cell.exploded = true;
            state[cell.position.x, cell.position.y] = cell;

            for (int x = 0; x < width; x++)
            {
                  for (int y = 0; y < height; y++)
                  {
                        cell = state[x, y];
                        if (cell.type == Cell.Type.Mine)
                        {
                              cell.revealed = true;
                              state[x, y] = cell;
                        }
                  }
            }
      }

      private void CheckWinCondition()
      {
            for (int x = 0; x < width; x++)
            {
                  for (int y = 0; y < height; y++)
                  {
                        Cell cell = state[x, y];
                        if (cell.type != Cell.Type.Mine && !cell.revealed)
                        {
                              return;
                        }
                  }
            }

            winScreen.SetActive(true);
            //Debug.Log("You Win!");
            gameover = true;
            
            for (int x = 0; x < width; x++)
            {
                  for (int y = 0; y < height; y++)
                  {
                        Cell cell = state[x, y];
                        if (cell.type == Cell.Type.Mine)
                        {
                              cell.flagged = true;
                              state[x, y] = cell;
                        }
                  }
            }
      }
      
      private Cell GetCell(int x, int y)
      {
            if (isValid(x, y))
            {
                  return state[x, y];
            } else {
                  return new Cell();
            }
      }

      private bool isValid(int x, int y)
      {
            return x >= 0 && x < width && y >= 0 && y < height;
      }
      
}
