import React, { useEffect, useState } from 'react';

function TextToSpeechComponent({ textToSpeech }: { textToSpeech: string }) {
    const [text, setText] = useState(textToSpeech);
    const [voices, setVoices] = useState<SpeechSynthesisVoice[]>([]);
    const [selectedVoice, setSelectedVoice] = useState<SpeechSynthesisVoice | null>(null);

    useEffect(() => {
        if ('speechSynthesis' in window) {
            const availableVoices = window.speechSynthesis.getVoices();
            setVoices(availableVoices);
            if (availableVoices.length > 0) {
                setSelectedVoice(availableVoices[0]); // Select first voice by default
            }

            // Listen for voiceschanged event to update voices if they load asynchronously
            window.speechSynthesis.onvoiceschanged = () => {
                const updatedVoices = window.speechSynthesis.getVoices();
                setVoices(updatedVoices);
                if (!selectedVoice && updatedVoices.length > 0) {
                    setSelectedVoice(updatedVoices[0]);
                }
            };
        } else {
            console.warn("Speech Synthesis not supported in this browser.");
        }
    }, [selectedVoice]);

    const speakText = () => {
        if ('speechSynthesis' in window && text && selectedVoice) {
            const utterance = new SpeechSynthesisUtterance(text);
            utterance.voice = selectedVoice;
            window.speechSynthesis.speak(utterance);
        }
    };

    return (
        <div>
            {/* <textarea
                value={text}
                onChange={(e) => setText(e.target.value)}
                placeholder="Enter text to speak..."
            /> */}
            <select
                onChange={(e) =>
                    setSelectedVoice(voices.find((voice) => voice.name === e.target.value) || null)
                }
                value={selectedVoice ? selectedVoice.name : ''}
            >
                {voices.map((voice) => (
                    <option key={voice.name} value={voice.name}>
                        {voice.name} ({voice.lang})
                    </option>
                ))}
            </select>
            <button onClick={speakText}>Speak</button>
        </div>
    );
}

export default TextToSpeechComponent;