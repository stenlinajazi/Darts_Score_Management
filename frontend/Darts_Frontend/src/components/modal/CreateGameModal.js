import { createGame } from "../../services/apiService.js";

const CreateGameModal = (onSubmit) => {
  const modal = document.createElement("div");
  modal.id = "create-game-modal";
  modal.className = "modal";
  modal.style.display = "none";
  modal.innerHTML = `
    <div class="modal-content">
      <span id="create-modal-close" class="modal-close">×</span>
      <h2>Create New Game</h2>
      <div class="modal-form">
        <label for="player-ids">Player IDs (comma-separated):</label>
        <input type="text" id="player-ids" placeholder="e.g., 1,2" />
        <label for="starting-score">Starting Score:</label>
        <select id="starting-score">
          <option value="301">301</option>
          <option value="501" selected>501</option>
          <option value="701">701</option>
        </select>
        <label for="sets-to-win">Sets to Win:</label>
        <input type="number" id="sets-to-win" min="1" max="3" value="1" />
        <label for="legs-per-set">Legs per Set:</label>
        <input type="number" id="legs-per-set" min="1" max="3" value="1" />
        <label for="must-finish-on-double">Must Finish on Double:</label>
        <input type="checkbox" id="must-finish-on-double" checked />
        <button id="create-game-submit" class="create-game-btn">Create Game</button>
      </div>
    </div>
  `;

  document.body.appendChild(modal);

  const errorMessage = document.getElementById("create-game-error");

  document
    .getElementById("create-modal-close")
    .addEventListener("click", () => {
      modal.style.display = "none";
      modal.remove();
    });

  document
    .getElementById("create-game-submit")
    .addEventListener("click", async () => {
      const playerIds = document
        .getElementById("player-ids")
        .value.split(",")
        .map((id) => parseInt(id.trim()))
        .filter((id) => !isNaN(id));
      const startingScore = parseInt(
        document.getElementById("starting-score").value
      );
      const setsToWin = parseInt(document.getElementById("sets-to-win").value);
      const legsPerSet = parseInt(
        document.getElementById("legs-per-set").value
      );
      const mustFinishOnDouble = document.getElementById(
        "must-finish-on-double"
      ).checked;

      const gameData = {
        type: 1,
        playerIds,
        startingScore,
        settings: {
          setsToWin,
          legsPerSet,
          mustFinishOnDouble,
        },
      };

      try {
        const newGame = await createGame(gameData);
        onSubmit(newGame);
        modal.style.display = "none";
        modal.remove();
      } catch (error) {
        console.error("Error creating game:", error.message);
        errorMessage.textContent = "Failed to create game. Please try again.";
        errorMessage.style.display = "block";
      }
    });

  modal.style.display = "block";
};

export default CreateGameModal;
