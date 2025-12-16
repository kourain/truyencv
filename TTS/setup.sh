# install uv
curl -LsSf https://astral.sh/uv/install.sh | sh

# install python
uv venv .venv --python3.11
source .venv/bin/activate

# download models
# model.pth
if [ ! -f "models/model.pth" ]; then
  curl -o models/model.pth https://drive.usercontent.google.com/download?id=1g5zUcsLaOIsUo9gDxISbMxBrzyzJAX30&export=download&authuser=1&confirm=t&uuid=f91c68bd-feb2-4a03-ba47-a95507a10e4f&at=ALWLOp4yk0tDiwmOPMyRW16oEBJ3:1765905307154
fi

# install requirements
uv sync

# install gcc for deepspeed
sudo apt update && sudo apt install -y build-essential

# install nvidia-cuda-toolkit
sudo apt install nvidia-cuda-toolkit