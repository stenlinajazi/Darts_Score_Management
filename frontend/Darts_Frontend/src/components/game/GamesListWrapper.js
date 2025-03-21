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
      <tbody id="games-table-body"></tbody>
    </table>
  `;

  root.appendChild(wrapper);
  const gamesTableBody = document.getElementById("games-table-body");

  const renderGames = async () => {
    gamesTableBody.innerHTML = "";
    const games = await GamesList(
      (gameId) => GameDetailsModal(gameId, root),
      renderGames
    );
    games.forEach((gameRow) => gamesTableBody.appendChild(gameRow));
  };

  await renderGames();
};

export default GamesListWrapper;
