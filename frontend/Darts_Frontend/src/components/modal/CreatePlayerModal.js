import { createPlayer } from "../../services/apiService.js";

const CreatePlayerModal = (onSubmit) => {
  const modal = document.createElement("div");
  modal.id = "create-player-modal";
  modal.className = "modal";
  modal.style.display = "none";
  modal.innerHTML = `
    <div class="modal-content">
      <span id="create-player-modal-close" class="modal-close">×</span>
      <h2>Create New Player</h2>
      <div id="create-player-error" style="color: red; display: none;"></div>
      <div class="modal-form">
        <label for="player-name">Name:</label>
        <input type="text" id="player-name" placeholder="Enter name" />
        <label for="player-username">Username:</label>
        <input type="text" id="player-username" placeholder="Enter username" />
        <label for="player-image-url">Profile Image URL:</label>
        <input type="text" id="player-image-url" placeholder="Enter image URL" />
        <button id="create-player-submit" class="create-player-btn">Create Player</button>
      </div>
    </div>
  `;

  document.body.appendChild(modal);

  const errorMessage = document.getElementById("create-player-error");

  document
    .getElementById("create-player-modal-close")
    .addEventListener("click", () => {
      modal.style.display = "none";
      modal.remove();
    });

  document
    .getElementById("create-player-submit")
    .addEventListener("click", async () => {
      const name = document.getElementById("player-name").value.trim();
      const username = document.getElementById("player-username").value.trim();
      const profileImageUrl = document
        .getElementById("player-image-url")
        .value.trim();

      if (!name || !username) {
        errorMessage.textContent = "Name and username are required.";
        errorMessage.style.display = "block";
        return;
      }

      const playerData = {
        name,
        username,
        profileImageUrl: profileImageUrl || null,
      };

      try {
        await createPlayer(playerData);
        onSubmit();
        modal.style.display = "none";
        modal.remove();
      } catch (error) {
        console.error("Error creating player:", error.message);
        errorMessage.textContent = "Failed to create player. Please try again.";
        errorMessage.style.display = "block";
      }
    });

  modal.style.display = "block";
};

export default CreatePlayerModal;
