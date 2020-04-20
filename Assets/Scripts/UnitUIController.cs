using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyScripts
{
    public class UnitUIController : MonoBehaviour
    {
        [SerializeField] private Slider slider; // Slide component of healthbar
        [SerializeField] private Gradient gradient; // Gradient component of canvas
        [SerializeField] private Image fill;
        [SerializeField] private Camera mainCamera;
    
        private void Start()
        {
            mainCamera = Camera.main;
            slider = transform.GetComponentInChildren<Slider>(); // Accessing the slider component from the child
            slider.maxValue = transform.parent.GetComponent<EntityController>().Health; // Set the maximum value of the slider with the health value of the unit
            slider.value = transform.parent.GetComponent<EntityController>().Health; // Set the current value of the slider with the health value of the unit so that the bar is filled when you start the game
            fill.color = gradient.Evaluate(1f); // Making the initial color green
        }
    
        private void Update()
        {
            slider.value = transform.parent.GetComponent<EntityController>().Health; // Updating the health bar
            fill.color = gradient.Evaluate(slider.normalizedValue); // Getting the right color from the gradient for the health bar
            transform.rotation = mainCamera.transform.rotation;
    
            //To do: Move all the logic of the healthbar in a takeDamage function so that it costs less resources.
        }
    }

}
