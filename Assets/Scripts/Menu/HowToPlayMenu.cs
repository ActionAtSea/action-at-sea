using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HowToPlayMenu : MonoBehaviour
{

    public Sprite[]         howToPlayScreens;
    public Image            displayedImage = null;
    public Button           backButton = null;
    public Button           nextButton = null;
    private int             currentSpriteIndex = 0;
    private SoundManager    soundManager = null;

    // Use this for initialization
    void Start()
    {
        if (displayedImage != null)
        {
            currentSpriteIndex = 0;
            displayedImage.sprite = howToPlayScreens[currentSpriteIndex];
        }

        if(backButton != null)
        {
            backButton.interactable = false;
        }

        soundManager = SoundManager.Get();
    }

    public void NextButton()
    {
        if (displayedImage != null)
        {
            if (currentSpriteIndex < howToPlayScreens.Length-1)
            {
                ++currentSpriteIndex;
                displayedImage.sprite = howToPlayScreens[currentSpriteIndex];
            }

            CheckIfButtonsAreInteractable();
        }

        if(soundManager != null)
        {
            soundManager.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        }
    }

    public void BackButton()
    {
        if (displayedImage != null)
        {
            if (currentSpriteIndex > 0)
            {
                --currentSpriteIndex;
                displayedImage.sprite = howToPlayScreens[currentSpriteIndex];
            }

            CheckIfButtonsAreInteractable();
        }
        if (soundManager != null)
        {
            soundManager.PlaySound(SoundManager.SoundID.BUTTON_CLICK);
        }
    }

    /// <summary>
    /// Sets the displayed image to the first sprite in howToPlayScreens.
    /// </summary>
    public void ResetImages()
    {
        if (displayedImage != null)
        {
            currentSpriteIndex = 0;
            displayedImage.sprite = howToPlayScreens[currentSpriteIndex];
        }

        CheckIfButtonsAreInteractable();
    }

    /// <summary>
    /// Checks whether the next or back buttons should be disabled.
    /// </summary>
    private void CheckIfButtonsAreInteractable()
    {
        //Enables and disables the next button depending on whether there is 
        //another tutorial image to show.
        if (currentSpriteIndex < howToPlayScreens.Length - 1)
        {
            if (nextButton != null)
            {
                nextButton.interactable = true;
            }
        }
        else
        {
            if (nextButton != null)
            {
                nextButton.interactable = false;
            }
        }

        //Enables and disables the back button depending on whether there is 
        //a previous tutorial image to show.
        if (currentSpriteIndex > 0)
        {
            if (backButton != null)
            {
                backButton.interactable = true;
            }
        }
        else
        {
            if (backButton != null)
            {
                backButton.interactable = false;
            }
        }
    }
}