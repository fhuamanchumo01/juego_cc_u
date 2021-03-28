using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour
{
    private static Color selected_color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
    private static Candy previous_selected = null;
    private SpriteRenderer sprite_renderer;
    private bool is_selected = false;
    public int id;
    private Vector2[] adjacend_Directions = new Vector2[]
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };

    private void Awake()
    { // el metodo Awake se ejecuta antes del start
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    private void seleccionar_caramelo()
    {
        is_selected = true;
        sprite_renderer.color = selected_color;
        previous_selected = gameObject.GetComponent<Candy>();
    }

    private void desseleccionar_caramelo()
    {
        is_selected = false;
        sprite_renderer.color = Color.white;
        previous_selected = null;
    }

    private void OnMouseDown()
    {
        if (sprite_renderer.sprite == null || Tablero_manager.shared_instance.se_intercambia)
            return;
        if(is_selected)
            desseleccionar_caramelo();
        else
        {
            if (previous_selected == null)
                seleccionar_caramelo();
            else
            {
                if (puede_intercambiar())
                {
                    swap_sprite(previous_selected);
                    previous_selected.busca_todas_coincidencias();
                    previous_selected.desseleccionar_caramelo();
                    busca_todas_coincidencias();

                    GUIManager.sharedInstance.MoveCounter--;
                }
                else
                {
                   previous_selected.desseleccionar_caramelo();
                   seleccionar_caramelo();
                }
            }
        }
    }

    public void swap_sprite(Candy new_Candy)
    {
        if (sprite_renderer.sprite == new_Candy.GetComponent<SpriteRenderer>().sprite)
            return;
        // cambiar imagen (sprite)
        Sprite old_candy = new_Candy.sprite_renderer.sprite;
        new_Candy.sprite_renderer.sprite = this.sprite_renderer.sprite;
        this.sprite_renderer.sprite = old_candy;
        // cambiar id
        int temporal = new_Candy.id;
        new_Candy.id = this.id;
        this.id = temporal;

    }

    private GameObject dame_vecino(Vector2 direccion)
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direccion); //rayo
        if (hit.collider != null)
            return hit.collider.gameObject;
        else
            return null;
    }

    private List<GameObject> dame_todos_vecinos()
    {
        List<GameObject> vecinos = new List<GameObject>();
        foreach (Vector2 direction in adjacend_Directions)
        {
            vecinos.Add(dame_vecino(direction));
        }
        return vecinos;
    }
    private bool puede_intercambiar()
    {
        return dame_todos_vecinos().Contains(previous_selected.gameObject);
    }

    private List<GameObject> Find_match(Vector2 direccion)
    {
        List<GameObject> matching_candies = new List<GameObject>();
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, direccion);
        while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == sprite_renderer.sprite)
        {
            matching_candies.Add(hit.collider.gameObject);
            hit = Physics2D.Raycast(hit.collider.transform.position, direccion);
        }
        return matching_candies;
    }
    private bool clear_Match(Vector2[] direcciones)
    {
        List<GameObject> matching_candies = new List<GameObject>();
        foreach (Vector2 direccion in direcciones)
        {
            matching_candies.AddRange(Find_match(direccion));
        }
        if (matching_candies.Count >= Tablero_manager.Min_vecinos_Marca)
        {
            foreach (GameObject candy in matching_candies)
            {
                candy.GetComponent<SpriteRenderer>().sprite = null;
            }
            return true;
        }
        else
        {
             return false;
        }
           

    }

    public void busca_todas_coincidencias() //findAllMatches
    {
        if (sprite_renderer.sprite == null)
            return;
        bool hMatch = clear_Match(new Vector2[2]
        {
            Vector2.left,
            Vector2.right
            
        });
        bool vMatch = clear_Match(new Vector2[2]
        {
            Vector2.up,
            Vector2.down
        });
        if (hMatch || vMatch)
        {
            sprite_renderer.sprite = null;
            StopCoroutine(Tablero_manager.shared_instance.find_null_candies());
            StartCoroutine(Tablero_manager.shared_instance.find_null_candies());
        }
    }
}
