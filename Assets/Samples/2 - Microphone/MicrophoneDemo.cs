using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Whisper.Utils;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

namespace Whisper.Samples
{
    /// <summary>
    /// Record audio clip from microphone and make a transcription.
    /// </summary>
    public class MicrophoneDemo : MonoBehaviour
    {
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;
        public bool streamSegments = true;
        public bool printLanguage = true;

        [Header("UI")] 
        public TextMeshProUGUI recordingText;
        public Dropdown languageDropdown;
        public Toggle translateToggle;
        public Toggle vadToggle;
        public ScrollRect scroll;
        public API_Manager apiManager;

        private string _buffer;

        private bool activated = false;
        private InputFieldPrompt inputField;
        private void Start()
        {
            inputField = GameObject.FindWithTag("input").GetComponent<InputFieldPrompt>();
        }

        private void Update()
        {
            if (inputField.activated == true) return;
            if (Input.GetKeyDown("y"))
            {
                if (!microphoneRecord.IsRecording)
                {
                    activated = true;
                    microphoneRecord.StartRecord();
                    recordingText.text = "Recording...";
                }
                else
                {
                    activated = false;
                    microphoneRecord.StopRecord();
                    recordingText.text = "Press Y to use your microphone instead.";
                }
            }
        }

        private void Awake()
        {
            whisper.OnNewSegment += OnNewSegment;
            
            microphoneRecord.OnRecordStop += OnRecordStop;
            
            languageDropdown.value = languageDropdown.options
                .FindIndex(op => op.text == whisper.language);
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

            translateToggle.isOn = whisper.translateToEnglish;
            translateToggle.onValueChanged.AddListener(OnTranslateChanged);

            vadToggle.isOn = microphoneRecord.vadStop;
            vadToggle.onValueChanged.AddListener(OnVadChanged);
        }

        private void OnVadChanged(bool vadStop)
        {
            microphoneRecord.vadStop = vadStop;
        }
        
        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            _buffer = "";

            print("Fetching...");
            var res = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
            if (res == null || !inputField.textField) {
                print("No result");
                return;
            }

            var text = res.Result;
            apiManager.RequestMemory(text);
        }
        
        private void OnLanguageChanged(int ind)
        {
            var opt = languageDropdown.options[ind];
            whisper.language = opt.text;
        }
        
        private void OnTranslateChanged(bool translate)
        {
            whisper.translateToEnglish = translate;
        }

        private void OnNewSegment(WhisperSegment segment)
        {
            if (!streamSegments || !inputField.textField)
                return;

            _buffer += segment.Text;
            inputField.textField.text = _buffer + "...";
        }
    }
}