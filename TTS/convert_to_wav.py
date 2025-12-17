from pydub import AudioSegment
import os


def chuyen_doi_am_thanh(
    input_path,
    output_path="result.wav",
    giay_bat_dau=0,
    audio_len=16,
    sample_rate=24000,
    bit_depth=16,
    channels=1,
    bitrate="384k",
):
    """
    Chuyển đổi file âm thanh sang định dạng WAV với các thông số tùy chỉnh

    Parameters:
        input_path: Đường dẫn file âm thanh đầu vào
        output_path: Đường dẫn file WAV đầu ra (mặc định: result.wav)
        giay_bat_dau: Giây bắt đầu cắt (mặc định: 0)
        audio_len: Độ dài âm thanh tính bằng giây (mặc định: 16)
        sample_rate: Tần số lấy mẫu Hz (mặc định: 24000)
        bit_depth: Độ sâu bit (mặc định: 16)
        channels: Số kênh - 1=mono, 2=stereo (mặc định: 1)
        bitrate: Tốc độ bit (mặc định: 384k)
    """

    try:
        # Check if the input file exists
        if not os.path.exists(input_path):
            raise FileNotFoundError(f"Input file not found: {input_path}")

        # Read the audio file
        audio = AudioSegment.from_file(input_path)

        # Convert to mono
        if channels == 1:
            audio = audio.set_channels(1)
        elif channels == 2:
            audio = audio.set_channels(2)

        # Set sample rate
        audio = audio.set_frame_rate(sample_rate)

        # Set sample width
        audio = audio.set_sample_width(bit_depth // 8)

        # Cut audio from start second
        mili_giay_bat_dau = giay_bat_dau * 1000
        mili_giay_ket_thuc = mili_giay_bat_dau + (audio_len * 1000)

        # Check audio length
        if mili_giay_bat_dau >= len(audio):
            raise ValueError(
                f"Giây bắt đầu ({giay_bat_dau}s) vượt quá độ dài âm thanh ({len(audio) / 1000}s)"
            )

        audio_cut = audio[mili_giay_bat_dau:mili_giay_ket_thuc]

        # Export file WAV
        print(f"Đang xuất file: {output_path}")
        audio_cut.export(
            output_path,
            format="wav",
            bitrate=bitrate,
            parameters=["-ar", str(sample_rate)],
        )

        print("Chuyển đổi thành công!")
        print("Thông số:")
        print(f"  - Độ dài: {audio_len} giây")
        print(f"  - Bit rate: {bitrate}")
        print(f"  - Kênh: {'fono' if channels == 1 else 'Stereo'}")
        print(f"  - Sample rate: {sample_rate} Hz")
        print(f"  - Sample size: {bit_depth} bit")
        print(f"  - Giây bắt đầu: {giay_bat_dau}s")

        return output_path

    except Exception as e:
        print(f"Lỗi: {str(e)}")
        return None


# Ví dụ sử dụng
if __name__ == "__main__":
    # Thay đổi đường dẫn file của bạn tại đây
    file_dau_vao = "am_thanh_goc.mp3"  # Có thể là mp3, wav, ogg, flac, v.v.
    file_dau_ra = "result.wav"

    # Chuyển đổi âm thanh bắt đầu từ giây thứ 10
    chuyen_doi_am_thanh(
        input_path=file_dau_vao,
        output_path=file_dau_ra,
        giay_bat_dau=10,  # cắt từ giây thứ 10
        audio_len=16,
        sample_rate=24000,
        bit_depth=16,
        channels=1,
        bitrate="384k",
    )
