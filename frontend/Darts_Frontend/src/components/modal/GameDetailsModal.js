import { fetchGameDetails } from "../../services/apiService.js";
import { formatThrowText } from "../../services/gameService.js";

const GameDetailsModal = async (gameId, root) => {
  const modal = document.createElement("div");
  modal.id = "game-details-modal";
  modal.className = "modal";
  modal.style.display = "none";
  modal.innerHTML = `
    <div class="modal-content">
      <span id="game-details-modal-close" class="modal-close">×</span>
      <h2>Game Details</h2>
      <div id="game-details-content"></div>
    </div>
  `;

  root.appendChild(modal);

  const gameDetailsContent = document.getElementById("game-details-content");

  let game;
  try {
    game = await fetchGameDetails(gameId);
    if (!game || !game.players) {
      throw new Error("Invalid game data");
    }
  } catch (error) {
    console.error("Error fetching game details:", error);
    gameDetailsContent.innerHTML = `<p style="color: red;">Failed to load game details. Please try again later.</p>`;
    modal.style.display = "block";
    return;
  }

  console.log("Game details response:", game);

  gameDetailsContent.innerHTML = `
    <p><strong>Game ID:</strong> ${game.id}</p>
    <p><strong>Type:</strong> ${game.type}</p>
    <p><strong>Starting Score:</strong> ${game.startingScore}</p>
    <p><strong>Sets to Win:</strong> ${game.setsToWin}</p>
    <p><strong>Started At:</strong> ${new Date(
      game.startedAt
    ).toLocaleString()}</p>
    <p><strong>Ended At:</strong> ${
      game.endedAt ? new Date(game.endedAt).toLocaleString() : "N/A"
    }</p>
    <p><strong>Status:</strong> ${
      game.isComplete ? "Complete" : "In Progress"
    }</p>
    <p><strong>Winner:</strong> ${
      game.players.find((p) => p.playerId === game.winnerId)?.playerName ||
      "N/A"
    }</p>
  `;

  game.players.forEach((player) => {
    const playerSection = document.createElement("div");
    const gameStats = player.gameStats || {
      setsWin: 0,
      legsWin: 0,
      ppd: 0,
      first9PPD: 0,
      checkoutPercentage: "0",
      count60Plus: 0,
      count100Plus: 0,
      count140Plus: 0,
      count180s: 0,
    };
    const setStats = player.setStats || [];
    const legStats = player.legStats || [];

    playerSection.innerHTML = `
      <h3>${player.playerName}</h3>
      <h4>Game Stats</h4>
      <table>
        <tr><th>Sets Won</th><td>${gameStats.setsWin}</td></tr>
        <tr><th>Legs Won</th><td>${gameStats.legsWin}</td></tr>
        <tr><th>PPD</th><td>${gameStats.ppd}</td></tr>
        <tr><th>First 9 PPD</th><td>${gameStats.first9PPD}</td></tr>
        <tr><th>Checkout %</th><td>${gameStats.checkoutPercentage}</td></tr>
        <tr><th>60+ Scores</th><td>${gameStats.count60Plus}</td></tr>
        <tr><th>100+ Scores</th><td>${gameStats.count100Plus}</td></tr>
        <tr><th>140+ Scores</th><td>${gameStats.count140Plus}</td></tr>
        <tr><th>180s</th><td>${gameStats.count180s}</td></tr>
      </table>
      <h4>Set Stats</h4>
      <table>
        <thead>
          <tr>
            <th>Set ID</th>
            <th>Legs Won</th>
            <th>PPD</th>
            <th>First 9 PPD</th>
            <th>Checkout %</th>
            <th>60+ Scores</th>
            <th>100+ Scores</th>
            <th>140+ Scores</th>
            <th>180s</th>
          </tr>
        </thead>
        <tbody>
          ${
            setStats.length > 0
              ? setStats
                  .map(
                    (set) => `
            <tr>
              <td>${set.setId}</td>
              <td>${set.legsWin}</td>
              <td>${set.ppd}</td>
              <td>${set.first9PPD}</td>
              <td>${set.checkoutPercentage}</td>
              <td>${set.count60Plus}</td>
              <td>${set.count100Plus}</td>
              <td>${set.count140Plus}</td>
              <td>${set.count180s}</td>
            </tr>
          `
                  )
                  .join("")
              : "<tr><td colspan='9'>No set stats available</td></tr>"
          }
        </tbody>
      </table>
      <h4>Leg Stats</h4>
      ${
        legStats.length > 0
          ? legStats
              .map(
                (leg) => `
        <div>
          <h5>Leg ID: ${leg.legId}</h5>
          <table>
            <tr><th>PPD</th><td>${leg.ppd}</td></tr>
            <tr><th>First 9 PPD</th><td>${leg.first9PPD}</td></tr>
            <tr><th>Total Throws</th><td>${leg.totalThrows}</td></tr>
            <tr><th>Checkout %</th><td>${leg.checkoutPercentage}</td></tr>
            <tr><th>60+ Scores</th><td>${leg.count60Plus}</td></tr>
            <tr><th>100+ Scores</th><td>${leg.count100Plus}</td></tr>
            <tr><th>140+ Scores</th><td>${leg.count140Plus}</td></tr>
            <tr><th>180s</th><td>${leg.count180s}</td></tr>
          </table>
          <h5>Throw History</h5>
          <div class="throw-history">
            ${
              leg.history && leg.history.length > 0
                ? leg.history
                    .map(
                      (turn) => `
              <p>Turn ID: ${turn.turnId}, Ending Score: ${turn.endingScore}</p>
              <ul>
                ${
                  turn.throws && turn.throws.length > 0
                    ? turn.throws
                        .map(
                          (throwData) =>
                            `<li>${formatThrowText(throwData)} (${
                              throwData.segment * throwData.multiplier
                            } points)</li>`
                        )
                        .join("")
                    : "<li>No throws recorded</li>"
                }
              </ul>
            `
                    )
                    .join("")
                : "<p>No throw history available</p>"
            }
          </div>
        </div>
      `
              )
              .join("")
          : "<p>No leg stats available</p>"
      }
    `;
    gameDetailsContent.appendChild(playerSection);
  });

  modal.style.display = "block";

  document
    .getElementById("game-details-modal-close")
    .addEventListener("click", () => {
      modal.style.display = "none";
      modal.remove();
    });
};

export default GameDetailsModal;
