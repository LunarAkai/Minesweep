using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    
    public Tilemap tilemap { get; private set; }

    [SerializeField] private Tile tileUnknown;
    [SerializeField] private Tile tileEmpty;
    [SerializeField] private Tile tileMine;
    [SerializeField] private Tile tileExploded;
    [SerializeField] private Tile tileFlag;
    [SerializeField] private Tile tileNum1;
    [SerializeField] private Tile tileNum2;
    [SerializeField] private Tile tileNum3;
    [SerializeField] private Tile tileNum4;
    [SerializeField] private Tile tileNum5;
    [SerializeField] private Tile tileNum6;
    [SerializeField] private Tile tileNum7;
    [SerializeField] private Tile tileNum8;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        
    }

    public void Draw(Cell[,] state)
    {
        tilemap.ClearAllTiles();
        
        int width = state.GetLength(0);
        int height = state.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
                tilemap.SetTile(cell.position, GetTile(cell));
            }
        }
    }

    private Tile GetTile(Cell cell)
    {
        if (cell.revealed)
        {
            return GetRevealedTile(cell);
        } 
        else if (cell.flagged)
        {
            return tileFlag;
        }
        else
        {
            return tileUnknown;
        }
    }

    private Tile GetRevealedTile(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.Type.Empty: return tileEmpty;
            case Cell.Type.Mine: return cell.exploded ? tileExploded : tileMine;
            case Cell.Type.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    private Tile GetNumberTile(Cell cell)
    {
        switch (cell.number)
        {
            case 1: return tileNum1;
            case 2: return tileNum2;
            case 3: return tileNum3;
            case 4: return tileNum4;
            case 5: return tileNum5;
            case 6: return tileNum6;
            case 7: return tileNum7;
            case 8: return tileNum8;
            default: return null;
        }
    }
    
}
