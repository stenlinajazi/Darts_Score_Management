import { deleteGame } from "../../services/apiService.js";

const GameCard = (game, onDetailsClick, onDeleteClick) => {
  const winner = game.players.find((p) => p.playerId === game.winnerId);
  const row = document.createElement("tr");
  row.innerHTML = `
    <td>${game.id}</td>
    <td>${game.type}</td>
    <td>${game.startingScore}</td>
    <td>${game.setsToWin}</td>
    <td>${new Date(game.startedAt).toLocaleString()}</td>
    <td>${game.endedAt ? new Date(game.endedAt).toLocaleString() : "N/A"}</td>
    <td>${game.isComplete ? "Complete" : "In Progress"}</td>
    <td>${winner ? winner.playerName : "N/A"}</td>
    <td>${game.players
      .map((p) => `${p.playerName} (${p.setsWon} sets)`)
      .join(", ")}</td>
    <td>
      <button class="details-btn" data-game-id="${game.id}">Details</button>
      <button class="delete-btn" data-game-id="${game.id}">Delete</button>
    </td>
  `;

  row
    .querySelector(".details-btn")
    .addEventListener("click", () => onDetailsClick(game.id));

  row.querySelector(".delete-btn").addEventListener("click", async () => {
    if (confirm(`Are you sure you want to delete game ${game.id}?`)) {
      try {
        await deleteGame(game.id);
        row.remove();
        onDeleteClick();
      } catch (error) {
        console.error(`Failed to delete game ${game.id}:`, error.message);
        alert(`Error: ${error.message}`);
      }
    }
  });

  return row;
};

export default GameCard;
