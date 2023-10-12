using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;



public class CharacterBattle : MonoBehaviour
{
    private Character_Base characterBase;
    private State state;
    private Vector3 slideTargetPosition;
    private Action onSlideComplete;
    private bool isPlayerTeam;
    private GameObject selectionCircleGameObject;
    private HealthSystem healthSystem;

    private enum State
    {
        Idle,
        Sliding,
        Busy,
    }
    private void Awake()
    {
        characterBase = GetComponent<Character_Base>();
        selectionCircleGameObject = transform.Find("SelectionCircle").gameObject;
        HideSelectionCircle();
        state = State.Idle;
    }
    private void Start() {
    }

    public void Setup(bool isPlayerTeam)
    {
        this.isPlayerTeam = isPlayerTeam;
        if (isPlayerTeam)
        {
            characterBase.SetAnimsSwordTwoHandedBack();
            characterBase.GetMaterial().mainTexture = BattleHandler.GetInstance().playerSpritesheet;
        }
        else
        {
            characterBase.SetAnimsSwordShield();
            characterBase.GetMaterial().mainTexture = BattleHandler.GetInstance().enemySpritesheet;
        }
        healthSystem = new HealthSystem(100);

        PlayAnimIdle();

    }

    private void PlayAnimIdle()
    {
        if (isPlayerTeam)
        {
            characterBase.PlayAnimIdle(new Vector3(+1, 0));
        }
        else
        {
            characterBase.PlayAnimIdle(new Vector3(-1, 0));
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Busy:
                break;
            case State.Sliding:
                float slideSpeed = 10f;
                transform.position += (slideTargetPosition - GetPosition()) * slideSpeed * Time.deltaTime;

                float reachedDistance = 1f;
                if (Vector3.Distance(GetPosition(), slideTargetPosition) < reachedDistance)
                {
                    transform.position = slideTargetPosition;
                    onSlideComplete();
                }
                break;
        }
    }


    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
        //In future add damage/health left system HERE

        //DamagePopup.Create(GetPosition(), damageAmount, false);
        characterBase.SetColorTint(new Color(1, 0, 0, 1f));
        
        if (healthSystem.IsDead())
        {
            //player or enemy died
            characterBase.PlayAnimLyingUp();
        }
    }

    public bool IsDead()
    {
        return healthSystem.IsDead();
    }

    public void Attack(CharacterBattle targetCharacterBattle, Action onAttackComplete)
    {
        Vector3 slideTargetPosition = targetCharacterBattle.GetPosition() + (GetPosition() - targetCharacterBattle.GetPosition()).normalized * 10f;
        Vector3 startingPosition = GetPosition();

        //move to enemy
        SlideToPosition(slideTargetPosition, () =>
        {
            //Arrive to enemy and attack
            state = State.Busy;
            Vector3 attackDir = (targetCharacterBattle.GetPosition() - GetPosition()).normalized;
            characterBase.PlayAnimAttack(attackDir, () => {
                //Target Hit
                int damageAmount = UnityEngine.Random.Range(20, 50);
                targetCharacterBattle.Damage(damageAmount); //edits how much damage is done
                }, () =>
            {
                //Attack Completed and move back
                SlideToPosition(slideTargetPosition, () =>
                {
                    //Slide back completed back to idle
                    state = State.Idle;
                    characterBase.PlayAnimIdle(attackDir);
                    onAttackComplete();
                });

            });

        });

    }

    private void SlideToPosition(Vector3 slideTargetPosition, Action onSlideComplete)
    {
        this.slideTargetPosition = slideTargetPosition;
        this.onSlideComplete = onSlideComplete;
        state = State.Sliding;
        if (slideTargetPosition.x > 0)
        {
            characterBase.PlayAnimSlideRight();
        }
        else
        {
            characterBase.PlayAnimSlideLeft();
                }


    }

    public void HideSelectionCircle()
    {
        selectionCircleGameObject.SetActive(false);
    }
    public void ShowSelectionCircle()
    {
        selectionCircleGameObject.SetActive(true);
    }







}