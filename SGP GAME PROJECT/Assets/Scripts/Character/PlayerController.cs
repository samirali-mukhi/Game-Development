﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] string name;
    [SerializeField] Sprite sprite;

	const float offssetY = 0.3f;
	
	private Vector2 input;
	private Character character;

	private void Awake()
	{
		character = GetComponent<Character>();
	}

	public void HandleUpdate()
	{
		if (!character.IsMoving)
		{
			input.x = Input.GetAxisRaw("Horizontal");
			input.y = Input.GetAxisRaw("Vertical");

			//Remove diagonal movement
			if (input.x != 0) input.y = 0;
			if (input != Vector2.zero)
			{
				StartCoroutine(character.Move(input,OnMoveOver));
			}
		}

		character.HandleUpdate();

		if (Input.GetKeyDown(KeyCode.Z))
		{
			Interact();
		}
	}

	//For interacting with NPCs
	void Interact()
	{
		var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
		var interactPos = transform.position + facingDir;

		// Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);
		var collider = Physics2D.OverlapCircle(interactPos , 0.3f , GameLayers.i.InteractableLayer);
		if (collider != null)
		{
			collider.GetComponent<Interactable>()?.Interact(transform);
		}
	}

	private void OnMoveOver() 
	{
		var colliders = (Physics2D.OverlapCircleAll(transform.position - new Vector3(0, offssetY), 0.2f, GameLayers.i.TrigerrableLayers));

		foreach(var collider in colliders)
		{
			var trigerrable = collider.GetComponent<IPlayerTriggerable>();
			if(trigerrable != null)
			{
				character.Animator.IsMoving =false;
				trigerrable.OnPlayerTriggered(this);
				break;
			}
		}
	}

	public string Name {
        get => name;
    }

    public Sprite Sprite {
        get => sprite;
    }
}