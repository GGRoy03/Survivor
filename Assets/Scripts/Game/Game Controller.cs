using Survivor.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameController
{
    //
    // Global Game Mode
    //

    public enum GameMode
    {
        Unknown   = 0,
        Gameplay  = 1,
        Inventory = 2,
        Dialogue  = 3,
        Paused    = 4,
        Finished  = 5,
    }

    private static Stack<GameMode> m_ModeStack       = new();
    private static int             m_ModeChangeFrame = 0;

    public static void PushGameMode(GameMode mode)
    {
        if (m_ModeStack != null)
        {
            m_ModeStack.Push(mode);
            m_ModeChangeFrame = Time.frameCount;
        }
    }
    
    public static void PopGameMode()
    {
        if(m_ModeStack != null)
        {
            m_ModeStack.Pop();
            m_ModeChangeFrame = Time.frameCount;
        }
    }

    public static bool IsGameMode(GameMode mode)
    {
        bool result = false;

        if(m_ModeStack != null)
        {
            if(m_ModeStack.Count > 0 && m_ModeChangeFrame != Time.frameCount)
            {
                result = mode == m_ModeStack.Peek();
            }
        }

        return result;
    }

    public static bool IsNewMode()
    {
        bool result = (Time.frameCount - 1) == m_ModeChangeFrame;
        return result;
    }

    //
    // Global Scene Management
    //

    public enum Scene
    {
        Menu = 0,
        Game = 1,
    }

    private static string SceneToName(Scene scene)
    {
        string result = scene switch
        {
            Scene.Game => "GameScene",
            Scene.Menu => "MenuScene",
            _ => null,
        };

        return result;
    }

    public static void SetScene(Scene scene)
    {
        string sceneAsName = SceneToName(scene);
        if(sceneAsName != null)
        {
            SceneManager.LoadScene(sceneAsName);
        }
    }
}
