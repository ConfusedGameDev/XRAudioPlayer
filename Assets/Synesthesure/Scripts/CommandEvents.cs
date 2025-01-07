using System;
using UnityEngine;
using UnityEngine.Events;
namespace Synesthesure
{
    public class CommandEvents : MonoBehaviour
    {
        [SerializeField] AudioSource audioSynchSource;

        public SendCommands sendCommands;
        public struct ListOfCommands
        {
            public string eventCommand;
            public string eventParameter;
        }
        public ListOfCommands[] CommandsList;
        bool commandsWerePushed;

        [SerializeField] private TextAsset commandEventsFile;
        [Tooltip("TimeStamp | Command | Parameter")]
        [TextArea(3, 300)]
        public string commandEvents;
        string[] lines;
        float[] timeStamps;
        bool[] triggered;
        string[] commands;
        string[] commmandParams;
        float previousPlayPosition = 0f;


        public void RefreshEventList()
        {
            Start();
        }
        void Start()
        {
            if (commandEventsFile) commandEvents = commandEventsFile.text;
            string[] theCommands;
            string[] tmpLines;
            tmpLines = commandEvents.Split(
            new[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None);
            string tmpStr;
            for (int i = 0; i < tmpLines.Length; i++)
            {
                tmpStr = tmpLines[i];
                if (tmpStr.IndexOf("|") > 0)
                {
                    int ii;
                    if (lines == null) { ii = 1; }
                    else { ii = lines.Length + 1; }
                    Array.Resize(ref lines, ii);
                    lines[ii - 1] = tmpStr;
                }
            }
            timeStamps = new float[lines.Length];
            triggered = new bool[lines.Length];
            commands = new string[lines.Length];
            commmandParams = new string[lines.Length];

            for (int i = 0; i < lines.Length; i++)
            {
                int ii = lines[i].IndexOf("|");
                if (ii > 0)
                {
                    theCommands = lines[i].Split('|');
                    timeStamps[i] = float.Parse(theCommands[0]);
                    commands[i] = theCommands[1];
                    if (theCommands.Length > 2)
                    {
                        commmandParams[i] = theCommands[2];
                    }
                    else
                    {
                        commmandParams[i] = " ";
                    }
                }
            }
        }

        void Update()
        {
            if (audioSynchSource.isPlaying)
            {
                // get events and send commands
                bool wasTriggered = false;
                for (int i = 0; i < timeStamps.Length; i++)
                {
                    if (i == timeStamps.Length - 1)
                    {
                        if ((!triggered[i]) && (timeStamps[i] < audioSynchSource.time))
                        {
                            triggered[i] = true;
                            wasTriggered = true;
                            CommandsToSend(commands[i], commmandParams[i]);
                        }
                    }
                    else if ((!triggered[i]) &&
                    (timeStamps[i] <= audioSynchSource.time) && (timeStamps[i + 1] >= timeStamps[i]))
                    {
                        triggered[i] = true;
                        wasTriggered = true;
                        CommandsToSend(commands[i], commmandParams[i]);
                    }
                }
                if (wasTriggered) PushCommandsNow(true);

                // reset if looped
                if (audioSynchSource.time < previousPlayPosition)
                {
                    for (int ii = 0; ii < timeStamps.Length; ii++)
                    {
                        triggered[ii] = false;
                    }
                }
                previousPlayPosition = audioSynchSource.time;
            }
        }


        void CommandsToSend(string command, string parameter)
        {
            if (commandsWerePushed)
            {// reset the commands buffer
                commandsWerePushed = false;
                CommandsList = null;
            }
            if (CommandsList == null)
            {
                CommandsList = new ListOfCommands[1];
            }
            else
            {// resize the command buffer
                int l = CommandsList.Length + 1;
                Array.Resize(ref CommandsList, l);
            }
            CommandsList[CommandsList.Length - 1].eventCommand = command;
            CommandsList[CommandsList.Length - 1].eventParameter = parameter;
        }

        void PushCommandsNow(bool pushNow)
        {
            // push all commands once per frame
            string tmpStr = "";
            for (int i = 0; i < CommandsList.Length; i++)
            {
                tmpStr = tmpStr + CommandsList[i].eventCommand + "|" +
                CommandsList[i].eventParameter + (char)127;
            }
            sendCommands.Invoke(tmpStr);
            commandsWerePushed = true;
        }
    }


    [System.Serializable]
    public class SendCommands : UnityEvent<string>
    {
    }

}