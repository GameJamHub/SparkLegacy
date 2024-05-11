using System.Collections;
using System.Collections.Generic;
using Codebase.Audio;
using Codebase.Core;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private HealthBar m_healthBar;
    [SerializeField] private ElectricBar m_electricBar;
    [SerializeField] private GameObject m_gameover;

    protected override void Init() {}

    void OnEnable()
    {
        GameplayEvents.OnGameOver+=HandleOnGameOver;
    }

    public void UpdateHealthBar(float amount)
    {
        m_healthBar.ChangeFillAmount(amount);
    }

    public void UpdateElectricBar(float amount)
    {
        m_electricBar.ChangeFillAmount(amount);
    }

    public bool CanUseElectric(float amount)
    {
        return m_electricBar.ReserveAmount - amount>0;
    }

    private void HandleOnRestart()
    {
        InputManager.Instance.EnableInputs();
    }

    private void HandleOnGameOver()
    {
        AudioManager.Instance.PlayOneShotSFX(AudioManager.Instance.Audios.playerDead, AudioChannelData.CHANNEL_2);
        InputManager.Instance.DisableInputs();
        m_gameover.SetActive(true);
        Time.timeScale = 0;
    }

    void OnDisable()
    {
        GameplayEvents.OnGameOver-=HandleOnGameOver;
    }
}
