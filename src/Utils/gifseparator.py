
from PIL import Image
import os
import sys

def extract_gif_frames(gif_path, output_folder="frames"):
    os.makedirs(output_folder, exist_ok=True)

    gif = Image.open(gif_path)

    frame_count = 0

    try:
        while True:
            # Convert frame to RGBA to preserve transparency
            frame = gif.convert("RGBA")

            output_path = os.path.join(
                output_folder,
                f"{frame_count:04d}.png"
            )

            frame.save(output_path)
            frame_count += 1

            gif.seek(gif.tell() + 1)

    except EOFError:
        pass

    print(f"Extracted {frame_count} frames to '{output_folder}'")


if __name__ == "__main__":
    gif_path = sys.argv[1] if len(sys.argv) > 1 else "effect.gif"
    extract_gif_frames(gif_path)