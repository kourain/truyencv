"""Script kiểm tra streaming TTS endpoint và phát audio trực tiếp."""

import requests
import pyaudio
from collections import deque
import threading

TTS_URL = "http://127.0.0.1:8000/tts/stream"

SAMPLE_RATE = 24000
CHANNELS = 1
BUFFER_SIZE = 50


def speaker(stream:pyaudio.Stream, buffer: deque, *started: bool):
    while started:
        if buffer.__len__() > 0:
            stream.write(buffer.popleft())


def test_streaming_tts():
    print("Đang gửi request đến TTS streaming endpoint...")

    payload = {
        "text": """Trò chuyện một chút, Diệp Phong cùng Hạ Thu liền cho tới hai ngày sau buổi hòa nhạc.
Diệp Phong thần sắc có chút tiếc nuối: "Đáng tiếc ta không có c·ướp được buổi hòa nhạc vé vào cửa, bằng không thì liền có thể đi hiện trường nghe ngươi buổi hòa nhạc."
Hạ Thu lại mỉm cười, ôn nhu trấn an nói: "Không có việc gì, trên mạng cũng có thể nghe."
"Nói thì nói như thế, nhưng hiện trường cùng trên mạng nghe là không giống, Hạ Hạ, ngươi về sau còn sẽ tới Trung Hải tổ chức buổi hòa nhạc sao?"
Diệp Phong gật gật đầu: "Ta năm nay mới vừa lên đại học năm 1."
"Dạng này a. . ." Hạ Thu gật gật đầu, muốn nói lại thôi.
Nhìn Diệp Phong mặc, gia đình điều kiện hẳn là rất bình thường. """,
        "normalize": True,
        "reference_audio": "nu-calm.wav",
        "chunk_size": 20,
    }

    try:
        p = pyaudio.PyAudio()

        stream = p.open(
            format=pyaudio.paInt16,
            channels=CHANNELS,
            rate=SAMPLE_RATE,
            output=True,
            frames_per_buffer=8192,
        )

        print("Đang nhận và phát audio stream (buffering)...")

        buffer = deque(maxlen=500)
        started = False
        speaker_thread = threading.Thread(target=speaker, args=(stream, buffer, started), daemon=True)
        with requests.post(TTS_URL, data=payload, stream=True) as response:
            response.raise_for_status()

            for chunk in response.iter_content(chunk_size=8192):
                if chunk:
                    buffer.append(chunk)

                    print(".", end="", flush=True)
                    if len(buffer) > BUFFER_SIZE and not started:
                        started = True
                        speaker_thread.start()
            if not started:
                started = True
                speaker_thread.start()
        while buffer.__len__() > 0:
            pass
        started = False
        stream.stop_stream()
        stream.close()
        p.terminate()
        print("\n✅ Hoàn tất phát audio!")

    except requests.exceptions.RequestException as e:
        print(f"❌ Lỗi khi gọi API: {e}")
    except Exception as e:
        print(f"❌ Lỗi: {e}")


if __name__ == "__main__":
    test_streaming_tts()
