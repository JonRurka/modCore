using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using modCore;

namespace modCore
{
    public class Monitor : MonoBehaviour
    {
        public ModCore modCore;

        GameObject player;
        private ICollection<IPlugin> plugins;
        private List<string> _Text;
        private List<string> _reverseText;
        private DateTime thisDate;
        private string input = string.Empty;
        private float bottom;
        private float xOffset;
        private bool open = false;

        void Awake()
        {
            DontDestroyOnLoad(this);
            _Text = new List<string>();
            _reverseText = new List<string>();
            thisDate = new DateTime();
        }

        void Start()
        {
            modCore.Log("in-game console started.");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
                open = !open;
        }

        void OnGUI()
        {
            bottom = Screen.height * 19 / 20;
            if (Application.loadedLevel == 1)
            {
                xOffset = 10;
            }
            else
                xOffset = 500;
            
            if (open)
            {
                if (Application.loadedLevel == 1)
                {
                    GUI.Box(new Rect(5, 5, 1010, Screen.height - 10), "");
                    input = GUI.TextField(new Rect(10, bottom, 1000, 20), input);
                    if (Event.current.keyCode == KeyCode.Return && !input.Equals(string.Empty))
                    {
                        submit(input);
                        input = string.Empty;
                    }
                }
                
                for (int i = 0; i < _reverseText.Count; i++)
                {
                    GUI.Label(new Rect(xOffset, (bottom - 20) - (i * 20), 1000, 20), _reverseText[i]);
                }
            }
        }

        void OnLevelWasLoaded(int level)
        {
            modCore.Log("\"" + Application.loadedLevelName  + "\" (" + level + ") was loaded.");
        }

        public void submit(string _input)
        {
            modCore.submit(_input, false);
        }

        public void Log(string _input)
        {
            string time = getTime();
            print("[" + time + "] > " + _input);
        }

        public void print(string _input)
        {
            _Text.Add(_input);
            _reverseText = new List<string>(_Text);
            _reverseText.Reverse();
        }

        public string getTime()
        {
            thisDate = DateTime.Now;
            string hour = thisDate.Hour.ToString("00");
            string minute = thisDate.Minute.ToString("00");
            string second = thisDate.Second.ToString("00");
            string AmPm = string.Empty;
            if (int.Parse(hour) > 12)
            {
                hour = (int.Parse(hour) - 12).ToString("00");
                AmPm = "PM";
            }
            else
            {
                AmPm = "AM";
            }

            return hour + ":" + minute + ":" + second + " " + AmPm;
        }
    }
}
