using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ANU.IngameDebug.Console.Converters;
using ANU.IngameDebug.Console.CommandLinePreprocessors;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Concurrent;

namespace ANU.IngameDebug.Console
{
    [DisallowMultipleComponent]
    [DebugCommandPrefix("console")]
    [DefaultExecutionOrder(-10_000)]
    public class DebugConsole : MonoBehaviour
    {
        private const float ColsoleInputHeightPercentage = 0.1f;
        private const float Padding = 10f;
        private const string PrefsHistoryKey = nameof(CommandLineHistory) + "-save";
        private const string PrefsDefinesKey = nameof(DefinesRegistry) + "-save";

        [SerializeField] private GameObject _content;
        [SerializeField] private TMP_InputField _input;
        [SerializeField] private Button _submit;
        [SerializeField] private Button _clear;
        [SerializeField] private Button _close;
        [Space]
        [SerializeField] private SuggestionPopUp _suggestions;
        [SerializeField] private InputFieldSuggestions _inputSuggestions;
        [Space]
        [SerializeField] private UITheme[] _themes;
        [SerializeField] private UITheme _currentTheme;

        private static readonly Regex _receivedMessageTypeRegex = new Regex(@"\[console:(?<type>.*?)\] ");
        private static ISuggestionsContext _suggestionsContext;
        private static CommandsSuggestionsContext _commandsContext;
        private static HistorySuggestionsContext _historyContext;
        private static DebugConsoleProcessor _processor = new DebugConsoleProcessor();
        private static IConsoleInput _consoleInput;
        private static ConcurrentQueue<UnityLog> _logs = new();

        internal static DebugConsole Instance { get; set; }
        public static bool IsOpened => Instance._content.activeInHierarchy;
        public static event Action IsOpenedChanged;

        private static ISuggestionsContext SuggestionsContext
        {
            get => _suggestionsContext;
            set
            {
                _suggestionsContext = value;
                if (Instance != null)
                    Instance._suggestions.Title = _suggestionsContext?.Title;
            }
        }

        public static event Action<UITheme> ThemeChanged;
        public static UITheme CurrentTheme
        {
            get => Instance == null ? null : Instance._currentTheme;
            private set
            {
                if (Instance._currentTheme == value)
                    return;
                Instance._currentTheme = value;
                ThemeChanged?.Invoke(CurrentTheme);

                LastThemeIndex = Array.IndexOf(Instance._themes, value);
            }
        }

        public static DebugConsoleProcessor Processor => _processor;

        public static ICommandsRouter Router { get => _processor.Router; set => _processor.Router = value; }
        public static bool ShowLogs { get => _processor.ShowLogs; set => _processor.ShowLogs = value; }
        public static ILogger Logger => _processor.Logger;
        public static IConverterRegistry Converters => _processor.Converters;
        public static ICommandInputPreprocessorRegistry Preprocessors => _processor.Preprocessors;
        public static IInstancesTargetRegistry InstanceTargets => _processor.InstanceTargets;
        public static ICommandsRegistry Commands => _processor.Commands;
        public static IDefinesRegistry Defines => _processor.Defines;

        private static CommandLineHistory CommandsHistory => _processor.CommandsHistory;
        internal static ILogger InputLogger => _processor.InputLogger;
        internal static LogsContainer Logs { get; } = new();

        private static int LastThemeIndex
        {
            get => PlayerPrefs.GetInt("ANU.IngameDebug.Console.LastTheme.Index", 0);
            set => PlayerPrefs.SetInt("ANU.IngameDebug.Console.LastTheme.Index", value);
        }

        /// <summary>
        /// if void - returns null
        /// if have one target - returns result as object
        /// if have multiple targets - returns IEnumerable<object> as object
        /// </summary>
        /// <param name="commandLine"></param>
        /// <param name="silent"></param>
        /// <returns></returns>
        public static ExecutionResult ExecuteCommand(string commandLine, bool silent = false)
            => _processor.ExecuteCommand(commandLine, silent);

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            Instance = this;
            ThemeChanged?.Invoke(CurrentTheme);

            if (CurrentTheme != null)
                CurrentTheme.Changed += () => ThemeChanged?.Invoke(CurrentTheme);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ClearStatic()
        {
            Instance = null;
            _logs.Clear();
        }

        private void Awake()
        {
            if (!Application.isPlaying)
                return;

            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Application.logMessageReceivedThreaded += LogMessageReceivedThreaded;

            _consoleInput = ConsoleInputFactory.GetInput();
            _commandsContext = new CommandsSuggestionsContext(Commands.Commands);
            _historyContext = new HistorySuggestionsContext(CommandsHistory);
            Close();

            _processor.Initialize();

            _input.onSubmit.AddListener(text => Submit());
            _input.onValueChanged.AddListener(text =>
            {
                if (text != CommandsHistory.Current)
                    CommandsHistory.Reset();

                DisplaySuggestions(text);
            });
            _submit.onClick.AddListener(Submit);
            _clear.onClick.AddListener(Logs.Clear);
            _close.onClick.AddListener(() =>
            {
                Close();
                _input.text = "";
            });

            _suggestions.Chosen += s =>
            {
                _input.text = s.ApplySuggestion(_input.text);

                _input.caretPosition = _input.text.Length;
                _input.ActivateInputField();
            };

            _suggestions.Hided += () => SuggestionsContext = _commandsContext;

            LoadCommandsHistory(CommandsHistory);
            LoadDefines(Defines);

            var themeIndex = LastThemeIndex;
            if (themeIndex >= 0 && themeIndex < _themes.Length)
                CurrentTheme = _themes[themeIndex];

            void Submit()
            {
                var text = _input.text;
                if (string.IsNullOrEmpty(text))
                    return;

                ExecuteCommand(text);

                _input.text = "";
                _input.ActivateInputField();
            }
        }

        // private void Start() => GetComponentInChildren<UIRectResizer>(includeInactive: true).RefreshConsoleSize();
        private void Start() => GetComponentInChildren<UICanvasScaler>(includeInactive: true).RefreshConsoleScale();

        private void OnDestroy()
        {
            Application.logMessageReceived -= LogMessageReceivedThreaded;
        }

        private void OnApplicationQuit()
        {
            SaveCommandsHistory(CommandsHistory);
            SaveDefines(Defines);
        }

        [DebugCommand(Description = "Set console theme at runtime. Pass index or name of wanted UITheme listed in DebugConsole Themes list")]
        private void SetTheme([OptAltNames("n")][OptValDynamic("console.list-theme-names")] string name)
        {
            if (_themes.Length == 0)
                throw new InvalidOperationException("Provide at least one item in DebugConsole Themes list");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Provide name of wanted UITheme listed in DebugConsole Themes list");

            var theme = _themes.FirstOrDefault(t => t.name == name);
            if (theme == null)
                throw new ArgumentException($"Theme \"{name}\" not found. Provide at least one item with name \"{name}\" in DebugConsole Themes list");

            CurrentTheme = theme;
        }

        [DebugCommand(DisplayOptions = CommandDisplayOptions.All & ~CommandDisplayOptions.Dashboard)]
        private IEnumerable<string> ListThemeNames()
        {
            for (int i = 0; i < _themes.Length; i++)
                yield return _themes[i].name;
        }

        private void Update()
        {
            if (!Application.isPlaying)
                return;
            HandleInput();
            HandleLogs();
        }

        private void HandleLogs()
        {
            while (_logs.TryDequeue(out var log))
            {
                if (!ShowLogs)
                    return;

                if (Instance == null)
                    return;

                var logType = ConsoleLogType.AppMessage;
                var match = _receivedMessageTypeRegex.Match(log.condition);

                if (match.Success && match.Index == 0)
                {
                    log.condition = log.condition.Substring(match.Length);
                    if (Enum.TryParse(typeof(ConsoleLogType), match.Groups["type"].Value, true, out var parsed))
                        logType = (ConsoleLogType)parsed;
                }

                Logs.Add(new Log(
                    logType,
                    log.type,
                    log.condition,
                    log.stackTrace
                ));
            }
        }

        private void HandleInput()
        {
            var control = _consoleInput.GetControl();
            var openPressed = _consoleInput.GetOpen();
            var dotPressed = _consoleInput.GetDot();
            var upPressed = _consoleInput.GetUp();
            var downPressed = _consoleInput.GetDown();
            var tabPressed = _consoleInput.GetTab();
            var escapePressed = _consoleInput.GetEscape();

            if (_content.activeInHierarchy && control)
            {
                if (openPressed)
                    SwitchContext();
                else if (dotPressed)
                    DisplaySuggestions(_input.text, forced: true);
            }
            else if (openPressed)
            {
                if (_content.activeSelf)
                    Close();
                else
                    Open();

                if (_content.activeInHierarchy)
                {
                    _input.ActivateInputField();
                    SuggestionsContext = _commandsContext;
                }
                _input.text = "";
            }

            if (!_content.activeInHierarchy)
                return;

            if (_suggestions.IsShown)
            {
                if (upPressed)
                    _suggestions.MoveUp();

                if (downPressed)
                    _suggestions.MoveDown();

                if (tabPressed)
                {
                    if (_suggestions.Selected == null)
                        _suggestions.MoveUp();
                    _suggestions.TryChooseCurrent();
                }

                if (escapePressed)
                {
                    SuggestionsContext = _commandsContext;
                    DisplaySuggestions(_input.text);
                    _suggestions.Deselect();
                    _input.ActivateInputField();
                }
            }
            else
            {
                if ((upPressed && CommandsHistory.TryMoveUp(out var command))
                || (downPressed && CommandsHistory.TryMoveDown(out command)))
                {
                    _input.text = command;
                    _input.caretPosition = _input.text.Length;
                }
            }
        }

        [DebugCommand(Description = "Clear console logs")]
        private void Clear() => Logs.Clear();

        private void SwitchContext()
        {
            if (SuggestionsContext == _historyContext)
                SuggestionsContext = _commandsContext;
            else if (SuggestionsContext == _commandsContext)
                SuggestionsContext = _historyContext;

            DisplaySuggestions(_input.text, forced: true);
        }

        [DebugCommand(DisplayOptions = CommandDisplayOptions.All & ~CommandDisplayOptions.Dashboard)]
        public static void Open()
        {
            Instance._content.SetActive(true);
            ExecuteCommand("console.refresh-size", silent: true);
            IsOpenedChanged?.Invoke();
        }

        [DebugCommand(DisplayOptions = CommandDisplayOptions.All & ~CommandDisplayOptions.Dashboard)]
        public static void Close()
        {
            Instance._content.SetActive(false);
            IsOpenedChanged?.Invoke();
        }

        private void DisplaySuggestions(string input, bool forced = false)
        {
            if (!forced && string.IsNullOrEmpty(input))
            {
                _suggestions.Hide();
                return;
            }

            _suggestions.Deselect();

            var suggestions = SuggestionsContext?.GetSuggestions(input);
            _suggestions.Suggestions = suggestions;

            if (suggestions.Any())
                _suggestions.Show();
            else
                _suggestions.Hide();
        }

        private static void LogMessageReceivedThreaded(string condition, string stackTrace, LogType type)
            => _logs.Enqueue(new UnityLog(condition, stackTrace, type));

        private void SaveCommandsHistory(CommandLineHistory history)
        {
            var list = history.Commands.ToList();
            var json = JsonUtility.ToJson(new SerializedCommandsHistory()
            {
                _list = list
            });
            PlayerPrefs.SetString(PrefsHistoryKey, json);
        }

        private void LoadCommandsHistory(CommandLineHistory history)
        {
            var list = JsonUtility.FromJson<SerializedCommandsHistory>(
                PlayerPrefs.GetString(PrefsHistoryKey)
            )?._list;
            history.Clear();

            if (list != null)
            {
                foreach (var command in list)
                    history.Record(command);
            }
        }

        private void LoadDefines(IDefinesRegistry defines)
        {
            var dict = JsonUtility.FromJson<SerializedDictionary<string, string>>(
                PlayerPrefs.GetString(PrefsDefinesKey)
            );
            defines.Clear();

            if (dict == null || dict.Pairs == null)
                return;

            foreach (var item in dict.Pairs)
                defines.Add(item.Key, item.Value);
        }

        private void SaveDefines(IDefinesRegistry defines)
        {
            var dict = new SerializedDictionary<string, string>
            {
                Pairs = new List<SerializedDictionary<string, string>.Pair>(defines.Defines.Select(kvp => new SerializedDictionary<string, string>.Pair
                {
                    Key = kvp.Key,
                    Value = kvp.Value
                }))
            };
            var json = JsonUtility.ToJson(dict);
            PlayerPrefs.SetString(PrefsDefinesKey, json);
        }

        private class SerializedCommandsHistory
        {
            public List<string> _list;
        }

        private class SerializedDictionary<TKey, TValue>
        {
            public List<Pair> Pairs;

            [Serializable]
            public class Pair
            {
                public TKey Key;
                public TValue Value;
            }
        }
    }

    internal struct UnityLog
    {
        public string condition;
        public string stackTrace;
        public LogType type;

        public UnityLog(string condition, string stackTrace, LogType type)
        {
            this.condition = condition;
            this.stackTrace = stackTrace;
            this.type = type;
        }
    }

    internal class UnityLogger : ILogger
    {
        public readonly Stack<bool> SilenceStack = new();

        private readonly ConsoleLogType _consoleLogType;

        public UnityLogger(ConsoleLogType consoleLogType)
            => _consoleLogType = consoleLogType;

        public bool IsSilenced => SilenceStack.Count > 0 && SilenceStack.Peek();

        public void LogReturnValue(object value, UnityEngine.Object context = null)
        {
            if (IsSilenced)
                return;

            var val = "";
            if (value is string str)
                val = str;
            else if (value is IEnumerable enumerable)
                val = "[" + string.Join(", ", enumerable.Cast<object>().Select(o => o?.ToString())) + "]";
            else if (value is IEnumerator enumerator)
                val = "[" + string.Join(", ", AsEnumerable(enumerator).Select(o => o?.ToString())) + "]";
            else
                val = value?.ToString();

            Debug.Log($"[console:{_consoleLogType}] {val}", context);
        }

        public void LogInfo(string message, UnityEngine.Object context)
        {
            if (IsSilenced)
                return;
            Debug.Log($"[console:{_consoleLogType}] {message}", context);
        }

        public void LogWarning(string message, UnityEngine.Object context)
        {
            if (IsSilenced)
                return;
            Debug.LogWarning($"[console:{_consoleLogType}] {message}", context);
        }

        public void LogError(string message, UnityEngine.Object context)
        {
            if (IsSilenced)
                return;
            Debug.LogError($"[console:{_consoleLogType}] {message}", context);
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            if (IsSilenced)
                return;
            Debug.LogException(new Exception($"[console:{_consoleLogType}] {exception.Message}", exception), context);
        }

        private IEnumerable<object> AsEnumerable(IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
    }

    public struct ExecutionResult
    {
        public readonly Type ReturnType;
        public readonly IReadOnlyList<SingleExecutionResult> ReturnValues;

        public ExecutionResult(Type returnType, IReadOnlyList<SingleExecutionResult> returnValues)
        {
            ReturnType = returnType;
            ReturnValues = returnValues;
        }
    }

    public struct SingleExecutionResult
    {
        public readonly object Target;
        public readonly object ReturnValue;

        public SingleExecutionResult(object target, object returnValue)
        {
            Target = target;
            ReturnValue = returnValue;
        }
    }
}