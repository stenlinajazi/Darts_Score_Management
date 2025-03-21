import { updatePlayer } from "../../services/apiService.js";

const UpdatePlayerModal = (player, onSubmit) => {
  const modal = document.createElement("div");
  modal.id = "update-player-modal";
  modal.className = "modal";
  modal.style.display = "none";
  modal.innerHTML = `
    <div class="modal-content">
      <span id="update-player-modal-close" class="modal-close">×</span>
      <h2>Update Player</h2>
      <div id="update-player-error" style="color: red; display: none;"></div>
      <div class="modal-form">
        <label for="player-name">Name:</label>
        <input type="text" id="player-name" value="${player.name}" />
        <label for="player-username">Username:</label>
        <input type="text" id="player-username" value="${player.username}" />
        <label for="player-image-url">Profile Image URL:</label>
        <input type="text" id="player-image-url" value="${
          player.profileImageUrl || ""
        }" />
        <button id="update-player-submit" class="update-player-btn">Update Player</button>
      </div>
    </div>
  `;

  document.body.appendChild(modal);

  const errorMessage = document.getElementById("update-player-error");

  document
    .getElementById("update-player-modal-close")
    .addEventListener("click", () => {
      modal.style.display = "none";
      modal.remove();
    });

  document
    .getElementById("update-player-submit")
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
        await updatePlayer(player.id, playerData);
        onSubmit();
        modal.style.display = "none";
        modal.remove();
      } catch (error) {
        console.error("Error updating player:", error.message);
        errorMessage.textContent = "Failed to update player. Please try again.";
        errorMessage.style.display = "block";
      }
    });

  modal.style.display = "block";
};

export default UpdatePlayerModal;
