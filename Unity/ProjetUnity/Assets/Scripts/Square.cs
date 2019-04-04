﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Square : MonoBehaviour
{
    public Sprite baseSprite;
    public Sprite moveSprite;
    public Sprite attackSprite;
    public Sprite inaccessibleSprite;
    public RuntimeAnimatorController selectedSquareAnim;

    private GameManager gm = GameManager.instance;
    private Character character = null;
    private Character lastCharacter = null;
    private bool canMoveIn = false;
    private Vector3 lastPos = new Vector3();


    public void OnMouseDown()
    {
        /*
         * Gros code en prévision
         */
        if (gm.GetPlayerTurn())
        {
            if (character != null)
            {
                gm.ChangeMove(character.movePoint.currentStat);
                gm.ChangeHealth(character.healthPoint.currentStat);
                gm.ChangeAttack(character.attackPoint.currentStat);
                GameObject selectedSquare = gm.GetSelectedSquare();
                if (gm.IsAlly(character.gameObject))
                {
                    if (selectedSquare == null)
                    {
                        gm.SetSelectedSquare(gameObject);
                        //GetComponent<Animator>().runtimeAnimatorController = selectedSquareAnim;
                        // TO CHANGE

                        //A changer nom de fonction
                        GameManager.instance.AttackSquares((int)transform.position.x, (int)transform.position.y, character.movePoint.currentStat, character.minDistAttack.currentStat, character.maxDistAttack.currentStat);
                    }
                    else if (selectedSquare.GetComponent<Square>().GetCharacter().Equals(character))
                    {
                        gm.ClearMovingTiles();
                        gm.SetSelectedSquare(null);
                        GetComponent<Animator>().runtimeAnimatorController = null;
                    }
                }
                else
                {
                    if (selectedSquare != null)
                    {
                        lastPos = new Vector3(selectedSquare.transform.position.x, selectedSquare.transform.position.y, 0);
                        lastCharacter = selectedSquare.GetComponent<Square>().GetCharacter();
                        int distance = (int)Mathf.Abs(lastPos.x - transform.position.x) + (int)Mathf.Abs(lastPos.y - transform.position.y);

                        //Verifie ennemi -> gm.IsEnemy(unit) + verifie a cote 
                        if (gm.IsEnemy(character.gameObject))
                        {
                            if (lastCharacter.GetComponent<Character>().maxDistAttack.baseStat >= distance && lastCharacter.GetComponent<Character>().minDistAttack.baseStat <= distance)
                            {
                                if (Attaque(character, lastCharacter))
                                    character = null;
                                gm.ClearMovingTiles();
                                gm.SetSelectedSquare(null);
                                GetComponent<Animator>().runtimeAnimatorController = null;
                            }
                        }
                    }
                }
            }
            else
            {
                GameObject unit = gm.GetSelectedSquare();
                if (canMoveIn)
                {
                    gm.ClearMovingTiles();
                    character = unit.GetComponent<Square>().GetCharacter();
                    unit.GetComponent<Square>().GetCharacter().GetComponent<Character>().Move(new Vector3(transform.position.x, transform.position.y, transform.position.z));
                    unit.GetComponent<Square>().SetCharacter(null);
                    gm.SetSelectedSquare(null);
                    GetComponent<Animator>().runtimeAnimatorController = null;

                    Vector3 start = unit.transform.position;
                    Vector3 finish = transform.position;
                    Vector3 calcul = new Vector3(Mathf.Abs(finish.x - start.x), Mathf.Abs(finish.y - start.y), 0);
                    int essai = Mathf.RoundToInt(calcul.x + calcul.y);
                    character.Move(essai);

                }
                else
                {
                    gm.ClearMovingTiles();
                    gm.SetSelectedSquare(null);
                    GetComponent<Animator>().runtimeAnimatorController = null;
                }
            }
        }
    }


    public Character GetCharacter()
    {
        return character;
    }

    public void SetCharacter(Character character)
    {
        this.character = character;
    }

    public void SetMovable(bool move)
    {
        canMoveIn = move;
    }

    public bool CanMoveIn()
    {
        return canMoveIn;
    }

    public void SetColor(Sprite color)
    {
        //Debug.Log(transform.position.x + ", " + transform.position.y + ", " + color);
        gameObject.GetComponent<SpriteRenderer>().sprite = color;
    }

    public bool Attaque(Character enemi, Character charac)
    {
        enemi.GetComponent<Character>().healthPoint.currentStat = enemi.GetComponent<Character>().healthPoint.currentStat - charac.GetComponent<Character>().attackPoint.baseStat;
        if (enemi.GetComponent<Character>().healthPoint.currentStat <= 0)
        {
            Destroy(enemi.gameObject);
            return true;
        }
        return false;
    }
}
