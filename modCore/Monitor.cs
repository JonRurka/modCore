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
        public ModApi modApi;
        public bool open = false;

        Console console;
        GameObject player;
        private ICollection<IPlugin> plugins;
        private List<string> _Text;
        private List<string> _reverseText;
        private DateTime thisDate;
        private string input = string.Empty;
        private float bottom;
        private float xOffset;
        private bool Opening = false;

        void Awake()
        {
            DontDestroyOnLoad(this);
            _Text = new List<string>();
            _reverseText = new List<string>();
            thisDate = new DateTime();
        }

        void Start()
        {
            //ModCore.Log("in-game console started.");
        }

        void Update()
        {
            /*if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                open = !open;
                if (open)
                    Opening = true;
            }*/
        }

        void OnGUI()
        {
            /*
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
                    GUI.SetNextControlName("inputBox");
                    input = GUI.TextField(new Rect(10, bottom, 1000, 20), input);
                    if (Event.current.keyCode == KeyCode.Return && !input.Equals(string.Empty))
                    {
                        submit(input);
                        input = string.Empty;
                    }

                    if (Event.current.keyCode == KeyCode.BackQuote && Opening)
                    {
                        GUI.FocusControl("inputBox");
                        if (input.Trim().Equals("`"))
                            input = string.Empty;
                        Opening = false;
                    }
                }

                for (int i = 0; i < _reverseText.Count; i++)
                {
                    GUI.Label(new Rect(xOffset, (bottom - 20) - (i * 20), 1000, 20), _reverseText[i]);
                }
            }*/
        }

        void OnLevelWasLoaded(int level)
        {
            if (level == 2)
            {
                GameObject consoleObj = GameObject.Find("Notifications");
                if (consoleObj != null)
                {
                    console = consoleObj.GetComponent<Console>();
                    modCore.console = console;
                    modApi.console = console;
                }
            }
            ModCore.Log("\"" + Application.loadedLevelName  + "\" (" + level + ") was loaded.");
        }

        public void submit(string _input)
        {

            //modCore.submit(_input, false);
        }

        public void Log(string _input)
        {
            string time = getTime();
            print("[" + time + "] > " + _input);
        }

        public void print(string _input)
        {
            string[] lines = _input.Split('\n');
            foreach (string line in lines)
            {
                _Text.Add(line);
                _reverseText = new List<string>(_Text);
                _reverseText.Reverse();
            }
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
