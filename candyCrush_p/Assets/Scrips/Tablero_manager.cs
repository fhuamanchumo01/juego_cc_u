using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tablero_manager : MonoBehaviour
{
    public static Tablero_manager shared_instance;
    public List<Sprite> prefabs = new List<Sprite>();
    public GameObject caramelo_actual;
    public int xSize, ySize;
    private GameObject[,] candies; // no vale [][]
    public bool se_intercambia { get; set; }
    int idx = -1;
    private Candy caramelo_seleccionado;
    public const int Min_vecinos_Marca = 2;
    
    void Start()
    {
        if (shared_instance == null)
            shared_instance = this;
        else
            Destroy(gameObject);
        
        Vector2 offset =caramelo_actual.GetComponent<BoxCollider2D>().size;
        crear_tablero_inicial(offset);

    }

    private void crear_tablero_inicial(Vector2 offset)
    {
        candies = new GameObject[xSize, ySize];
        float startX = this.transform.position.x;
        float startY = this.transform.position.y;
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                GameObject nuevo_caramelo = Instantiate(
                    caramelo_actual,
                    new Vector3(startX + (offset.x*i), startY + (offset.y*j), 0),
                    caramelo_actual.transform.rotation
                );
                nuevo_caramelo.name = string.Format("Candy[{0}][{1}]", i, j);
                do
                {
                    idx = Random.Range(0, prefabs.Count);
                } while ((i>0 && idx == candies[i-1,j].GetComponent<Candy>().id) 
                         || (j>0 && idx == candies[i,j-1].GetComponent<Candy>().id));
                
                Sprite sprite = prefabs[idx];
                nuevo_caramelo.GetComponent<SpriteRenderer>().sprite = sprite;
                nuevo_caramelo.GetComponent<Candy>().id = idx;
                nuevo_caramelo.transform.parent = this.transform;
                candies[i, j] = nuevo_caramelo;
            }
        }
    }

    public IEnumerator find_null_candies()
    {
        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < ySize; j++)
            {
                if (candies[i, j].GetComponent<SpriteRenderer>().sprite == null)
                {
                    yield return StartCoroutine(MakeCandiesFall(i, j));
                    break;
                }
            }
        }

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                candies[x,y].GetComponent<Candy>().busca_todas_coincidencias();
            }
        }
    }

    private IEnumerator MakeCandiesFall(int x, int yStart, float shift_delai=0.05f)
    {
        se_intercambia = true;

        List<SpriteRenderer> renderes = new List<SpriteRenderer>();
        int nullCandies = 0;
        for (int y= yStart ; y < ySize; y++)
        {
            SpriteRenderer Srenderer = candies[x, y].GetComponent<SpriteRenderer>();
            if (Srenderer.sprite == null)
            {
                nullCandies++;
            }

            renderes.Add(Srenderer);
        }

        for (int i = 0; i < nullCandies; i++)
        {
            GUIManager.sharedInstance.Score += 10;
            
            yield return new WaitForSeconds(shift_delai);
            for (int j = 0; j < renderes.Count-1; j++)
            {
                renderes[j].sprite = renderes[j + 1].sprite;
                renderes[j + 1].sprite = GetNewCandies(x,ySize-1);
            }
        }
        se_intercambia = false;
    }

    private Sprite GetNewCandies(int x, int y)
    {
        List<Sprite> posiblesCaramelos = new List<Sprite>();
        posiblesCaramelos.AddRange(prefabs);
        if (x > 0)
        {
            posiblesCaramelos.Remove(candies[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }

        if (x < xSize - 1)
        {
            posiblesCaramelos.Remove(candies[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }

        if (y > 0)
        {
            posiblesCaramelos.Remove(candies[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return posiblesCaramelos[Random.Range(0, posiblesCaramelos.Count)];
    }
}
