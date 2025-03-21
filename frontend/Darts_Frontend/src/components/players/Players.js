import { getPlayers, deletePlayer } from "../../services/apiService.js";
import CreatePlayerModal from "../modal/CreatePlayerModal.js";
import UpdatePlayerModal from "../modal/UpdatePlayerModal.js";
import PlayerStatsModal from "../modal/PlayerStatsModal.js";

const Players = (root) => {
  const wrapper = document.createElement("div");
  wrapper.id = "players-container";
  wrapper.className = "view";
  wrapper.innerHTML = `
    <h2>Players</h2>
    <button id="create-player-btn" class="btn btn-primary">Create Player</button>
    <div id="players-table-container">
      <table id="players-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Username</th>
            <th>Profile Image</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody id="players-table-body"></tbody>
      </table>
    </div>
  `;

  root.appendChild(wrapper);

  const fetchAndRenderPlayers = async () => {
    try {
      const players = await getPlayers();
      const tbody = document.getElementById("players-table-body");
      tbody.innerHTML = "";
      players.forEach((player) => {
        const row = document.createElement("tr");
        row.innerHTML = `
          <td>${player.id}</td>
          <td>${player.name}</td>
          <td>${player.username}</td>
          <td><img src="${player.profileImageUrl}" alt="${player.name}" class="player-image" width="50" height="50" /></td>
          <td>
            <button class="update-btn btn btn-secondary" data-id="${player.id}">Update</button>
            <button class="delete-btn btn btn-danger" data-id="${player.id}">Delete</button>
            <button class="stats-btn btn btn-info" data-id="${player.id}">Stats</button>
          </td>
        `;
        tbody.appendChild(row);
      });

      document.querySelectorAll(".update-btn").forEach((button) => {
        button.addEventListener("click", () => {
          const playerId = parseInt(button.dataset.id);
          const player = players.find((p) => p.id === playerId);
          UpdatePlayerModal(player, () => fetchAndRenderPlayers());
        });
      });

      document.querySelectorAll(".delete-btn").forEach((button) => {
        button.addEventListener("click", async () => {
          const playerId = parseInt(button.dataset.id);
          if (
            confirm(`Are you sure you want to delete player ID ${playerId}?`)
          ) {
            try {
              await deletePlayer(playerId);
              fetchAndRenderPlayers();
            } catch (error) {
              console.error("Error deleting player:", error.message);
              alert("Failed to delete player. Please try again.");
            }
          }
        });
      });

      document.querySelectorAll(".stats-btn").forEach((button) => {
        button.addEventListener("click", () => {
          const playerId = parseInt(button.dataset.id);
          PlayerStatsModal(playerId);
        });
      });
    } catch (error) {
      console.error("Error fetching players:", error.message);
      const tbody = document.getElementById("players-table-body");
      tbody.innerHTML = `<tr><td colspan="5">Failed to load players. Please try again later.</td></tr>`;
    }
  };

  document.getElementById("create-player-btn").addEventListener("click", () => {
    CreatePlayerModal(() => fetchAndRenderPlayers());
  });

  fetchAndRenderPlayers();
};

export default Players;
