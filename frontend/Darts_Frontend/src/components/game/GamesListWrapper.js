import GamesList from "./GameList.js";
import GameDetailsModal from "../modal/GameDetailsModal.js";

const GamesListWrapper = async (root) => {
  const wrapper = document.createElement("div");
  wrapper.id = "games-list-container";
  wrapper.className = "view";
  wrapper.innerHTML = `
    <h2>Games List</h2>
    <table id="games-table">
      <thead>
        <tr>
          <th>Game ID</th>
          <th>Type</th>
          <th>Starting Score</th>
          <th>Sets to Win</th>
          <th>Started At</th>
          <th>Ended At</th>
          <th>Status</th>
          <th>Winner</th>
          <th>Players</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody id="games-table-body">
        <tr><td colspan="10" class="loading-message">Loading games...</td></tr>
      </tbody>
    </table>
  `;

  root.appendChild(wrapper);
  const gamesTableBody = document.getElementById("games-table-body");

  const renderGames = async () => {
    gamesTableBody.innerHTML = `
      <tr><td colspan="10" class="loading-message">Loading games...</td></tr>
    `;

    try {
      const games = await GamesList(
        (gameId) => GameDetailsModal(gameId, root),
        renderGames
      ); //defining  onDetailsClick, onDeleteClick event handlers
      //Handle showing game details when the "Details" button is clicked --> onDetailsClick
      //Refresh or update the games list after a game is deleted -->onDeleteClick
      gamesTableBody.innerHTML = "";
      games.forEach((gameRow) => gamesTableBody.appendChild(gameRow));
    } catch (error) {
      gamesTableBody.innerHTML = `
        <tr>
          <td colspan="10" class="error-message">
            Failed to load games: ${error.message || "Unknown error"}.
            <button id="retry-btn" class="btn btn-secondary">Retry</button>
          </td>
        </tr>
      `;
      document
        .getElementById("retry-btn")
        .addEventListener("click", renderGames);
    }
  };

  await renderGames();
};

export default GamesListWrapper;
