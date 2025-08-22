# Unity 2D RL Agent — PPO (ML-Agents) + ONNX Inference

A tiny 2D Unity environment where an agent learns to reach a target with **PPO**.  
Trained via **Unity ML-Agents**, monitored in **TensorBoard**, and deployed to Unity with **ONNX/Barracuda** (no Python needed at runtime).
<img width="984" height="727" alt="2DRL_ss" src="https://github.com/user-attachments/assets/ec5efea0-25a1-45d3-9b43-aa4b6ea9bd1c" />

---

## 🔧 Requirements
- Unity (same major/minor version you used)
- (Optional for training) **Python 3.9 x64**

## ▶️ Run (Inference Only — no Python)
1. Open the project in Unity.
2. Agent → **Behavior Parameters**:
   - **Model:** your ONNX (e.g. `Assets/ML-Agents/Models/SmartAgent-*.onnx`)
   - **Behavior Type:** **Inference Only**
   - *(Tip)* Decision Requester → **Decision Period = 1–2** for smooth motion
3. Press **Play**.

## 🏋️ Train (Optional)
```bash
# from repo root
python -m venv venv && venv\Scripts\activate
pip install -r training/requirements.txt

# start trainer
mlagents-learn config.yaml --run-id=demo_run
# When it prints "Listening on port 5004", press Play in Unity
# Stop with Ctrl+C — it will export ONNX under results/<run_id>/
View metrics:

bash
Copy code
tensorboard --logdir results
```
🧩 Environment (what the agent sees/does)

Observations (6): agent_x, agent_y, target_x, target_y, vel_x, vel_y

Actions (2, continuous): moveX, moveY

Rewards:

-0.001 per step (finish faster)

prevDist - newDist (positive when moving toward the target)

+1.0 on reaching target

-1.0 out of bounds, -0.2 on timeout (episode reset)

Max Step: 1000

📈 Results (example)

Around ~220k steps: Cumulative Reward ≈ 4.2, Episode Length ≈ 7–8.

Policy exported to ONNX and runs in-editor reliably.

📁 Repo Structure
Assets/                # Unity scene & scripts (SmartAgent.cs, etc.)
Packages/
ProjectSettings/
config.yaml            # PPO config used for ML-Agents
training/requirements.txt
.gitignore
README.md

❗ Troubleshooting

UnityTimeOutException: enable Project Settings → Player → Run In Background.
During training set Behavior Type = Default.

Obs/Action mismatch warnings: keep obs size 6, actions 2 (continuous).

No episodes completing: ensure rewards aren’t impossible; Max Step ~1000 works well.

🧠 What this demonstrates

Defining observations/actions and reward shaping

PPO training with TensorBoard monitoring

Shipping a model via ONNX/Barracuda for runtime inference in Unity

➕ Next ideas (later)

Larger arena / smaller target (hard mode)

Moving target speed changes

Multi-agent tag (chaser vs evader)

