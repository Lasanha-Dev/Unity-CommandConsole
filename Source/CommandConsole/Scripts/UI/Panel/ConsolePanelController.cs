#if DevConsole
using IEnumerator = System.Collections.IEnumerator;

using TMP_InputField = TMPro.TMP_InputField;

using TMP_Dropdown = TMPro.TMP_Dropdown;

using Regex = System.Text.RegularExpressions.Regex;

using Gma.DataStructures.StringSearch;

using UnityEngine;

using System.Collections.Generic;

using System.Linq;

namespace Game.DevConsole
{
    internal sealed class ConsolePanelController : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _commandInputField;

        [SerializeField] private TMP_InputField _commandTextPrefab;

        [SerializeField] private TMP_Dropdown _suggestionDropdown;

        [SerializeField] private Transform _commandTextParent;

        [SerializeField] private int _maxSuggestionsCount;

        private readonly Color _defaultLogColor = Color.white;

        private readonly Color _warningLogColor = Color.yellow;

        private readonly Color _errorLogColor = Color.red;

        private readonly List<string> _invisibleCommands = new List<string>();

        private readonly Trie<string> _commandSuggestions = new Trie<string>();

        private readonly WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

        private const int COMMAND_LINE_SPLIT_INDEX = 0;

        private const int COMMAND_PARAMETER_SPLIT_INDEX = 1;

        private const string _commandLineParametersPattern = "<[^>]*>";

        private const string NEXT_SUGGESTIONS_SECTION_TEXT = "...";

        private void Awake()
        {
            _suggestionDropdown.onValueChanged.AddListener(OnSelectSuggestion);

            InitializeCommandInputFieldEvents();

            InitializeCommandSuggestions();

            Application.logMessageReceived += OnLogMessageReceived;

            SpawnTextPrefabOnConsole("Initialized Console", Color.green);
        }

        private void OnEnable()
        {
            StartCoroutine(FocusCommandInputField());
        }

        private void InitializeCommandInputFieldEvents()
        {
            _commandInputField.onSubmit.AddListener(OnSubmitCommand);

            _commandInputField.onValueChanged.AddListener(OnCommandInputFieldValueChanged);
        }

        private void InitializeCommandSuggestions()
        {
            foreach (string commandLine in CommandManager.CommmandLines)
            {
                _commandSuggestions.Add(commandLine, commandLine);
            }
        }

        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                SpawnTextPrefabOnConsole($"{condition} {stackTrace}", _errorLogColor);

                return;
            }

            if (Debug.isDebugBuild)
            {
                string[] stackLines = stackTrace.Split('\n');

                string fileNameLine = stackLines[1].Trim();

                int startIndex = fileNameLine.LastIndexOf("/") + 1;

                int endIndex = fileNameLine.LastIndexOf(":");

                string fileName = fileNameLine.Substring(startIndex, endIndex - startIndex);

                string lineNumber = fileNameLine.Substring(endIndex + 1);

                string formattedLog = string.Format("{0} ({1}): {2}", fileName, lineNumber, condition);

                SpawnTextPrefabOnConsole($"{formattedLog}", GetColorFromLogType(type));

                return;
            }

            SpawnTextPrefabOnConsole(condition, GetColorFromLogType(type));
        }

        private void OnSelectSuggestion(int suggestionIndex)
        {
            if (_suggestionDropdown.options[suggestionIndex].text == NEXT_SUGGESTIONS_SECTION_TEXT && _invisibleCommands.Count > 0)
            {
                UpdateDropdownSuggestions(_invisibleCommands);

                return;
            }

            if (_suggestionDropdown.options[suggestionIndex].text == NEXT_SUGGESTIONS_SECTION_TEXT)
            {
                return;
            }

            string commandLine = Regex.Replace(_suggestionDropdown.options[suggestionIndex].text, _commandLineParametersPattern, string.Empty);

            commandLine = commandLine.Replace(" ", "");

            _commandInputField.text = commandLine;

            StartCoroutine(FocusCommandInputField());
        }

        private void OnSubmitCommand(string command)
        {
            string[] commandParam = command.Split(' ');

            string commandLine = commandParam[COMMAND_LINE_SPLIT_INDEX];

            object commandParameter = commandParam.Length > 1 ? GetCommandParameter(commandParam[COMMAND_PARAMETER_SPLIT_INDEX]) : null;

            _commandInputField.text = string.Empty;

            SpawnTextPrefabOnConsole($"{commandLine} {commandParameter}", _defaultLogColor);

            CommandManager.ExecuteCommand(commandLine, commandParameter);

            StartCoroutine(FocusCommandInputField());
        }

        private void SpawnTextPrefabOnConsole(string text, Color textColor)
        {
            TMP_InputField consoleTextObject = Instantiate(_commandTextPrefab, _commandTextParent);

            consoleTextObject.text = text;

            consoleTextObject.textComponent.color = textColor;
        }

        private void OnCommandInputFieldValueChanged(string commandInput)
        {
            if (commandInput.Length == 0)
            {
                ResetSuggestionsDropdown();

                StartCoroutine(FocusCommandInputField());

                return;
            }

            List<string> commandSuggestions = (List<string>)_commandSuggestions.Retrieve(commandInput).ToList();

            if (commandSuggestions.Count == 1 && commandSuggestions[0] == commandInput)
            {
                return;
            }

            UpdateDropdownSuggestions(commandSuggestions);
        }

        private void UpdateDropdownSuggestions(List<string> suggestions)
        {
            ResetSuggestionsDropdown();

            if (suggestions.Count == 0)
            {
                StartCoroutine(ShowSuggestionsDropdown());

                return;
            }

            List<string> newSuggestions = new List<string>() { string.Empty };

            newSuggestions.AddRange(suggestions);

            _invisibleCommands.Clear();

            if (newSuggestions.Count > _maxSuggestionsCount)
            {
                for (int i = _maxSuggestionsCount; i < newSuggestions.Count; i++)
                {
                    _invisibleCommands.Add(newSuggestions[i]);
                }

                newSuggestions.RemoveRange(_maxSuggestionsCount, newSuggestions.Count - _maxSuggestionsCount);

                newSuggestions.Add(NEXT_SUGGESTIONS_SECTION_TEXT);
            }

            _suggestionDropdown.AddOptions(newSuggestions);

            StartCoroutine(ShowSuggestionsDropdown());
        }

        private void ResetSuggestionsDropdown()
        {
            _suggestionDropdown.Hide();

            _suggestionDropdown.ClearOptions();
        }

        private IEnumerator ShowSuggestionsDropdown()
        {
            yield return _waitForEndOfFrame;

            _suggestionDropdown.Show();

            yield return FocusCommandInputField();
        }

        private IEnumerator FocusCommandInputField()
        {
            yield return _waitForEndOfFrame;

            _commandInputField.Select();

            _commandInputField.caretPosition = _commandInputField.text.Length;
        }

        private Color GetColorFromLogType(LogType type)
        {
            if (type == LogType.Log)
            {
                return _defaultLogColor;
            }

            if (type == LogType.Warning)
            {
                return _warningLogColor;
            }

            if (type == LogType.Error || type == LogType.Exception)
            {
                return _errorLogColor;
            }

            return _defaultLogColor;
        }

        private object GetCommandParameter(string commandInput)
        {
            if (int.TryParse(commandInput, out int intValue))
            {
                return intValue;
            }

            if (float.TryParse(commandInput, out float floatValue))
            {
                return floatValue;
            }

            if (bool.TryParse(commandInput, out bool boolValue))
            {
                return boolValue;
            }

            return commandInput;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }
    }
}
#endif