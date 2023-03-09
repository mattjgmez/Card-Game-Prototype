using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeHeroAnimEvents : MonoBehaviour
{
    // References to effect prefabs. These are set in the inspector
    [Header("Effects")]
    public GameObject           m_RunStopDust;
    public GameObject           m_JumpDust;
    public GameObject           m_LandingDust;
    public GameObject           m_DodgeDust;
    public GameObject           m_WallSlideDust;
    public GameObject           m_WallJumpDust;
    public GameObject           m_AirSlamDust;
    public GameObject           m_ParryEffect;

    private AudioManager_PrototypeHero m_audioManager;

    // Start is called before the first frame update
    void Start()
    {
        m_audioManager = AudioManager_PrototypeHero.instance;
    }

    // Animation Events
    // These functions are called inside the animation files
    void AE_runStop()
    {
        m_audioManager.PlaySound("RunStop");
    }

    void AE_footstep()
    {
        m_audioManager.PlaySound("Footstep");
    }

    void AE_Throw()
    {
        m_audioManager.PlaySound("Jump");
    }

    void AE_Parry()
    {
        m_audioManager.PlaySound("Parry");
    }

    void AE_ParryStance()
    {
        m_audioManager.PlaySound("DrawSword");
    }

    void AE_AttackAirSlam()
    {
        m_audioManager.PlaySound("DrawSword");
    }

    void AE_AttackAirLanding()
    {
        m_audioManager.PlaySound("AirSlamLanding");
    }

    void AE_Hurt()
    {
        m_audioManager.PlaySound("Hurt");
    }

    void AE_Death()
    {
        m_audioManager.PlaySound("Death");
    }

    void AE_SwordAttack()
    {
        m_audioManager.PlaySound("SwordAttack");
    }

    void AE_SheathSword()
    {
        m_audioManager.PlaySound("SheathSword");
    }

    void AE_Dodge()
    {
        m_audioManager.PlaySound("Dodge");
    }

    void AE_WallSlide()
    {
        //m_audioManager.GetComponent<AudioSource>().loop = true;
        if(!m_audioManager.IsPlaying("WallSlide")) 
            m_audioManager.PlaySound("WallSlide");
    }

    void AE_LedgeGrab()
    {
        m_audioManager.PlaySound("LedgeGrab");
    }

    void AE_LedgeClimb()
    {
        m_audioManager.PlaySound("RunStop");
    }
}
